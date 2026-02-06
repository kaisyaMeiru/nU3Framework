using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Collections.Generic;
using nU3.Data;
using nU3.Data.Repositories;
using System.Data.SQLite;
using Microsoft.Extensions.Configuration;
using nU3.Connectivity;
using nU3.Connectivity.Implementations;

namespace nU3.Bootstrapper
{
    /// <summary>
    /// 모듈 로더: 화면 모듈 및 플러그인을 서버와 동기화하고 로드하는 책임을 가집니다.
    /// </summary>
    public class ModuleLoader : IDisposable
    {
        private readonly IDBAccessService _db;
        private readonly SQLiteModuleRepository _moduleRepo;
        private readonly string _stagingPath;
        private string _installPath;
        private readonly bool _isRuntimeDirFixed;
        
        // HTTP 전송 서비스
        private readonly IFileTransferService _fileTransferService;
        private readonly string _serverModulePath;
        private readonly bool _useServerTransfer;

        public ModuleLoader(IDBAccessService dbService, IConfiguration configuration)
        {
            _db = dbService;
            _moduleRepo = new SQLiteModuleRepository(_db);

            // 스테이징 경로 초기화 (ApplicationData 폴더)
            _stagingPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "nU3.Framework", "Cache", "Modules");
            if (!Directory.Exists(_stagingPath)) Directory.CreateDirectory(_stagingPath);

            // 런타임 설치 경로 초기화
            var configRuntimeDir = configuration.GetValue<string>("RuntimeDirectory");
            if (!string.IsNullOrWhiteSpace(configRuntimeDir))
            {
                _installPath = Path.Combine(configRuntimeDir.TrimEnd('\\', '/'), "Modules");
                _isRuntimeDirFixed = true;
            }
            else
            {
                _installPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Modules");
                _isRuntimeDirFixed = false;
            }

            if (!Directory.Exists(_installPath)) Directory.CreateDirectory(_installPath);

            // 서버 연결 설정 로드
            var serverEnabled = configuration.GetValue<bool>("ServerConnection:Enabled", false);
            var baseUrl = configuration.GetValue<string>("ServerConnection:BaseUrl") ?? "https://localhost:64229";
            _serverModulePath = configuration.GetValue<string>("ModuleStorage:ServerPath") ?? "Modules";
            _useServerTransfer = configuration.GetValue<bool>("ModuleStorage:UseServerTransfer", true) && serverEnabled;

            if (_useServerTransfer)
            {
                _fileTransferService = new HttpFileTransferClient(baseUrl);
                FileLogger.Info("서버 연결 활성화");
                FileLogger.Info($"  BaseUrl: {baseUrl}");
                FileLogger.Info($"  ServerModulePath: {_serverModulePath}");
                FileLogger.Info($"  StagingPath: {_stagingPath}");
                FileLogger.Info($"  InstallPath: {_installPath}");
            }
            else
            {
                _fileTransferService = null!;
                FileLogger.Warning("서버 연결이 비활성화되어 업데이트가 불가능합니다.");
            }
        }

        public void EnsureDatabaseInitialized()
        {
            // 초기화는 서버에서 수행되므로 연결 확인 정도로 대체 가능
            _db.Connect();
        }

        public void CheckAndLoadModules(string aShellFilePath)
        {
            // 설정 파일에 RuntimeDirectory가 없는 경우에만 파라미터 기반으로 업데이트
            if (!_isRuntimeDirFixed && !string.IsNullOrEmpty(aShellFilePath))
            {
                var shellDir = Path.GetDirectoryName(aShellFilePath);
                if (!string.IsNullOrEmpty(shellDir))
                {
                    _installPath = Path.Combine(shellDir, "Modules");
                    if (!Directory.Exists(_installPath)) Directory.CreateDirectory(_installPath);
                }
            }

            if (_useServerTransfer && _fileTransferService != null)
            {
                CheckAndDownloadModules();
            }
        }

        public void CheckAndDownloadModules()
        {
            FileLogger.Info("모듈 업데이트 확인 중...");

            string homeDirectory = "";
            try
            {
                if (_fileTransferService != null)
                {
                    homeDirectory = _fileTransferService.GetHomeDirectory() ?? "";
                    homeDirectory = homeDirectory.Replace('\\', '/').TrimEnd('/');
                    FileLogger.Debug($"서버 홈 디렉토리: {homeDirectory}");
                }
            }
            catch (Exception ex)
            {
                FileLogger.Warning("서버 홈 디렉토리 조회 실패", ex);
            }

            var modules = _moduleRepo.GetAllModules();
            var activeVersions = _moduleRepo.GetActiveVersions();
            int updateCount = 0;

            foreach (var module in modules)
            {
                var activeVer = activeVersions.FirstOrDefault(v => v.ModuleId == module.ModuleId);
                if (activeVer == null)
                {
                    FileLogger.Debug($"모듈 {module.ModuleId}({module.ModuleName})의 활성 버전이 없습니다. 건너뜁니다.");
                    continue;
                }

                string relativePath = module.FileName;
                if (!string.IsNullOrEmpty(module.Category))
                {
                    relativePath = Path.Combine(module.Category, relativePath);
                    if (!string.IsNullOrEmpty(module.SubSystem))
                    {
                        var dir = Path.GetDirectoryName(relativePath) ?? "";
                        relativePath = Path.Combine(dir, module.SubSystem, module.FileName);
                    }
                }

                string stagingFile = Path.Combine(_stagingPath, relativePath);
                string installFile = Path.Combine(_installPath, relativePath);

                bool needUpdate = false;

                if (!File.Exists(stagingFile) || CalculateFileHash(stagingFile) != activeVer.FileHash)
                {
                    try
                    {
                        string serverCategoryPath = module.Category ?? "";
                        if (!string.IsNullOrEmpty(module.SubSystem))
                        {
                            serverCategoryPath = $"{serverCategoryPath}/{module.SubSystem}".TrimStart('/');
                        }
                        
                        string serverRelativePath = string.IsNullOrEmpty(serverCategoryPath) 
                            ? module.FileName 
                            : $"{serverCategoryPath}/{module.FileName}";

                        string serverUrlPath = string.IsNullOrEmpty(homeDirectory)
                            ? $"{_serverModulePath}/{serverRelativePath}"
                            : $"{homeDirectory}/{_serverModulePath}/{serverRelativePath}";

                        string stagingDir = Path.GetDirectoryName(stagingFile)!;
                        if (!Directory.Exists(stagingDir)) Directory.CreateDirectory(stagingDir);

                        FileLogger.ModuleOperation(module.ModuleId, module.ModuleName, "다운로드 시작", $"URL: {serverUrlPath}");
                        
                        if (_fileTransferService.DownloadFile(serverUrlPath, stagingFile))
                        {
                            var fileInfo = new FileInfo(stagingFile);
                            FileLogger.ModuleOperation(module.ModuleId, module.ModuleName, "다운로드 완료", $"크기: {fileInfo.Length:N0} bytes");
                            needUpdate = true;
                            System.Threading.Thread.Sleep(50);
                        }
                        else
                        {
                            FileLogger.Warning($"모듈 다운로드 실패: {module.ModuleName}");
                            continue;
                        }
                    }
                    catch (Exception ex)
                    {
                        FileLogger.Error($"모듈 다운로드 중 오류: {module.ModuleName}", ex);
                        continue;
                    }
                }

                if (needUpdate || !File.Exists(installFile))
                {
                    try
                    {
                        string installDir = Path.GetDirectoryName(installFile)!;
                        if (!Directory.Exists(installDir))
                        {
                            Directory.CreateDirectory(installDir);
                            FileLogger.Debug($"  설치 디렉토리 생성: {installDir}");
                        }

                        File.Copy(stagingFile, installFile, true);
                        updateCount++;
                        FileLogger.ModuleOperation(module.ModuleId, module.ModuleName, "설치 완료", $"경로: {installFile}");
                    }
                    catch (Exception ex)
                    {
                        FileLogger.Error($"모듈 설치 실패: {module.ModuleName}", ex);
                    }
                }
            }

            if (updateCount > 0)
                FileLogger.Info($"{updateCount}개 모듈이 업데이트되었습니다.");
            else
                FileLogger.Info("모든 모듈이 최신 상태입니다.");
        }

        private string CalculateFileHash(string filePath)
        {
            if (!File.Exists(filePath)) return string.Empty;
            
            try
            {
                using (var sha256 = SHA256.Create())
                using (var stream = File.OpenRead(filePath))
                {
                    var hash = sha256.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
            catch
            {
                return string.Empty;
            }
        }

        public void Dispose()
        {
            if (_fileTransferService is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
}
