using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Microsoft.Extensions.Configuration;
using nU3.Connectivity;
using nU3.Connectivity.Implementations;
using nU3.Core.Repositories;
using nU3.Data;
using nU3.Data.Repositories;
using nU3.Models;

namespace nU3.Bootstrapper
{
    /// <summary>
    /// 컴포넌트(프레임워크 DLL/EXE) 로더입니다.
    /// 
    /// 책임:
    /// - 로컬 DB에 등록된 컴포넌트 버전을 조회하여 설치 경로와 해시를 비교
    /// - 서버 또는 로컬 저장소에서 컴포넌트 파일을 다운로드하여 캐시 저장
    /// - 캐시에서 설치 경로로 복사(패치) 수행
    /// - 전체 업데이트 흐름을 관리하고 진행 이벤트를 발생시킵니다
    /// 
    /// 변경 사항:
    /// - 로컬 파일 복사(Fallback) 로직 제거됨. 오직 HTTP를 통해서만 다운로드.
    /// </summary>
    public class ComponentLoader : IDisposable
    {
        private readonly IComponentRepository _componentRepo;
        private readonly IFileTransferService _fileTransferService;
        private readonly string _serverComponentPath;
        private readonly string _cachePath;
        private readonly string _installPath;
        private readonly bool _useServerTransfer;
        public string ServerPath = string.Empty;

        /// <summary>
        /// 컴포넌트 로드 상태를 외부로 알리기 위한 이벤트입니다.
        /// 구독자는 ComponentUpdateEventArgs를 받아 UI 또는 로그를 갱신할 수 있습니다.
        /// </summary>
        public event EventHandler<ComponentUpdateEventArgs>? UpdateProgress;

        /// <summary>
        /// ComponentLoader 생성자
        /// </summary>
        /// <param name="dbManager">로컬 DB 매니저 (컴포넌트 메타데이터 조회용)</param>
        /// <param name="configuration">앱 설정(IConfiguration) - 서버 경로 및 옵션 로드</param>
        /// <param name="installPath">설치 루트 경로(선택). null이면 현재 애플리케이션 베이스 디렉토리 사용</param>
        public ComponentLoader(LocalDatabaseManager dbManager, IConfiguration configuration, string? installPath = null)
        {
            _componentRepo = new SQLiteComponentRepository(dbManager);

            // 캐시 경로: %AppData%\nU3.Framework\Cache\Components
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);            
            _cachePath = Path.Combine(appData, "nU3.Framework", "Cache", "Components");
            if (!Directory.Exists(_cachePath))
                Directory.CreateDirectory(_cachePath);

            // 설치 기본 경로 설정
            // 1. 설정 파일(appsettings.json)의 RuntimeDirectory 우선
            // 2. 생성자 파라미터로 전달된 installPath
            // 3. 현재 애플리케이션의 베이스 디렉토리
            var configRuntimeDir = configuration.GetValue<string>("RuntimeDirectory");
            _installPath = !string.IsNullOrWhiteSpace(configRuntimeDir) 
                ? configRuntimeDir 
                : (installPath ?? AppDomain.CurrentDomain.BaseDirectory);

            _installPath = _installPath.TrimEnd('\\', '/');
            if (!Directory.Exists(_installPath))
                Directory.CreateDirectory(_installPath);

            // 서버 연결 및 경로 설정 읽기
            var serverEnabled = configuration.GetValue<bool>("ServerConnection:Enabled", false);
            var baseUrl = configuration.GetValue<string>("ServerConnection:BaseUrl") ?? "https://localhost:64229";
            _serverComponentPath = configuration.GetValue<string>("ComponentStorage:ServerPath") ?? "Patch";
            _useServerTransfer = configuration.GetValue<bool>("ComponentStorage:UseServerTransfer", true) && serverEnabled;

            // FileTransferService 초기화
            if (_useServerTransfer)
            {
                _fileTransferService = new HttpFileTransferClient(baseUrl);

                var homeDirectory = _fileTransferService.GetHomeDirectory();
                ServerPath = string.IsNullOrWhiteSpace(homeDirectory) ? $"{_serverComponentPath}" : $"{homeDirectory.TrimEnd('/', '\\')}/{_serverComponentPath}";

                FileLogger.Info($"서버 연결 활성화");
                FileLogger.Info($"  BaseUrl: {baseUrl}");
                FileLogger.Info($"  ServerPath: {ServerPath}");
                FileLogger.Info($"  CachePath: {_cachePath}");
                FileLogger.Info($"  InstallPath: {_installPath}");
            }
            else
            {
                // 서버 전송 비활성화 시 경고 (다운로드 불가)
                _fileTransferService = null!;
                FileLogger.Warning("서버 연결이 비활성화되어 있습니다.");
            }
        }

        /// <summary>
        /// 설치 루트 경로를 반환합니다.
        /// </summary>
        public string InstallPath => _installPath;

        /// <summary>
        /// 로컬에 설치되어 있지 않거나 해시가 다른 컴포넌트 목록을 반환합니다.
        /// (우선순위에 따라 정렬하여 반환)
        /// </summary>
        public List<ComponentUpdateInfo> CheckForUpdates()
        {
            var updates = new List<ComponentUpdateInfo>();
            var activeVersions = _componentRepo.GetActiveVersions();

            foreach (var version in activeVersions)
            {
                var component = _componentRepo.GetComponent(version.ComponentId);
                if (component == null) continue;

                var installFile = GetInstallPath(component);
                var updateInfo = new ComponentUpdateInfo
                {
                    ComponentId = version.ComponentId,
                    ComponentName = component.ComponentName,
                    ComponentType = component.ComponentType,
                    FileName = component.FileName,
                    ServerVersion = version.Version,
                    FileSize = version.FileSize,
                    IsRequired = component.IsRequired,
                    Priority = component.Priority,
                    InstallPath = installFile,
                    StoragePath = version.StoragePath,
                    GroupName = component.GroupName ?? "Other"
                };

                if (!File.Exists(installFile))
                {
                    updateInfo.UpdateType = UpdateType.NewInstall;
                    updateInfo.LocalVersion = null;
                    updates.Add(updateInfo);
                    FileLogger.ComponentOperation(updateInfo.ComponentId, "신규 설치 필요", $"파일 존재하지 않음: {installFile}");
                }
                else
                {
                    var localHash = CalculateFileHash(installFile);
                    if (!string.Equals(localHash, version.FileHash, StringComparison.OrdinalIgnoreCase))
                    {
                        updateInfo.UpdateType = UpdateType.Update;
                        updateInfo.LocalVersion = GetFileVersion(installFile);
                        updates.Add(updateInfo);
                        FileLogger.ComponentOperation(updateInfo.ComponentId, "업데이트 필요", $"해시 불일치 - Local: {localHash}, Server: {version.FileHash}");
                        FileLogger.Debug($"  로컬 버전: {updateInfo.LocalVersion}, 서버 버전: {version.Version}");
                    }
                }
            }

            // 우선순위에 따라 정렬하여 반환
            return updates.OrderBy(u => u.Priority).ToList();
        }

        /// <summary>
        /// 반드시 설치되어야 하는 필수 컴포넌트 목록을 반환합니다.
        /// </summary>
        public List<ComponentUpdateInfo> GetMissingRequiredComponents()
        {
            return CheckForUpdates()
                .Where(u => u.IsRequired && u.UpdateType == UpdateType.NewInstall)
                .ToList();
        }

        /// <summary>
        /// 데이터베이스에 등록된 모든 컴포넌트(신규 설치 포함)를 순차적으로 다운로드하고 설치합니다.
        /// 진행 상태는 UpdateProgress 이벤트를 통해 발생합니다.
        /// </summary>
        /// <returns>업데이트 결과</returns>
        public ComponentUpdateResult UpdateAll()
        {
            var result = new ComponentUpdateResult();
            var updates = CheckForUpdates();

            if (!updates.Any())
            {
                result.Success = true;
                result.Message = "모든 컴포넌트가 최신 상태입니다.";
                return result;
            }

            int total = updates.Count;
            int current = 0;

            foreach (var update in updates)
            {
                current++;

                RaiseProgress(new ComponentUpdateEventArgs
                {
                    Phase = UpdatePhase.Downloading,
                    ComponentId = update.ComponentId,
                    ComponentName = update.ComponentName,
                    CurrentIndex = current,
                    TotalCount = total,
                    PercentComplete = (int)((current - 0.5) / total * 100)
                });

                try
                {
                    FileLogger.ComponentOperation(update.ComponentId, "다운로드 시작", $"서버 경로: {update.StoragePath}");
                    
                    // 1. 서버에서 캐시로 다운로드 (HTTP Only)
                    var cacheFile = DownloadToCache(update);
                    FileLogger.ComponentOperation(update.ComponentId, "다운로드 완료", $"캐시 경로: {cacheFile}");

                    RaiseProgress(new ComponentUpdateEventArgs
                    {
                        Phase = UpdatePhase.Installing,
                        ComponentId = update.ComponentId,
                        ComponentName = update.ComponentName,
                        CurrentIndex = current,
                        TotalCount = total,
                        PercentComplete = (int)(current / (double)total * 100)
                    });

                    // 2. 캐시에서 설치 경로로 복사(패치)
                    InstallFromCache(cacheFile, update.InstallPath);
                    FileLogger.ComponentOperation(update.ComponentId, "설치 완료", $"설치 경로: {update.InstallPath}, 버전: {update.ServerVersion}");

                    result.UpdatedComponents.Add(update.ComponentId);

                    // 서버 부하 방지를 위한 미세 지연 (429 에러 예방)
                    System.Threading.Thread.Sleep(50);
                }
                catch (Exception ex)
                {
                    result.FailedComponents.Add((update.ComponentId, ex.Message));
                    FileLogger.Error($"컴포넌트 업데이트 실패: {update.ComponentName}", ex);

                    RaiseProgress(new ComponentUpdateEventArgs
                    {
                        Phase = UpdatePhase.Failed,
                        ComponentId = update.ComponentId,
                        ComponentName = update.ComponentName,
                        ErrorMessage = ex.Message
                    });
                }
            }

            result.Success = !result.FailedComponents.Any();
            result.Message = result.Success
                ? $"{result.UpdatedComponents.Count}개 컴포넌트가 성공적으로 업데이트되었습니다."
                : $"{result.UpdatedComponents.Count}개 성공, {result.FailedComponents.Count}개 실패";

            RaiseProgress(new ComponentUpdateEventArgs
            {
                Phase = UpdatePhase.Completed,
                CurrentIndex = total,
                TotalCount = total,
                PercentComplete = 100
            });

            return result;
        }

        /// <summary>
        /// 특정 컴포넌트를 강제로 업데이트합니다.
        /// </summary>
        /// <param name="componentId">업데이트할 컴포넌트 ID</param>
        /// <returns>성공 여부</returns>
        public bool UpdateComponent(string componentId)
        {
            var version = _componentRepo.GetActiveVersion(componentId);
            if (version == null) return false;

            var component = _componentRepo.GetComponent(componentId);
            if (component == null) return false;

            try
            {
                var update = new ComponentUpdateInfo
                {
                    ComponentId = componentId,
                    ComponentName = component.ComponentName,
                    FileName = component.FileName,
                    InstallPath = GetInstallPath(component),
                    StoragePath = version.StoragePath,
                    GroupName = component.GroupName ?? "Other"
                };

                var cacheFile = DownloadToCache(update);
                InstallFromCache(cacheFile, update.InstallPath);
                return true;
            }
            catch (Exception ex)
            {
                FileLogger.Error($"컴포넌트 업데이트 실패: {componentId}", ex);
                return false;
            }
        }

        #region Private Methods

        /// <summary>
        /// 컴포넌트의 설치 전체 경로를 반환합니다.
        /// RuntimeDirectory를 기준으로 하며, 모듈은 Modules 폴더로, 그 외는 루트(또는 지정 경로)로 설정합니다.
        /// </summary>
        private string GetInstallPath(ComponentMstDto component)
        {
            string basePath = _installPath;

            // 모듈(ScreenModule) 타입은 강제로 Modules 폴더 하위로 배포
            if (component.ComponentType == ComponentType.ScreenModule)
            {
                return Path.Combine(basePath, "Modules", component.FileName);
            }

            // 그 외 컴포넌트는 DB의 InstallPath를 따르거나 루트에 배포            
            var relativePath = component.InstallPath.Replace(ServerPath, InstallPath);
            return relativePath;

            //return Path.Combine(basePath, relativePath);
        }

        /// <summary>
        /// 컴포넌트를 캐시로 다운로드합니다.
        /// - 서버 전송이 활성화된 경우 IFileTransferService를 사용하여 서버에서 다운로드 시도
        /// - 서버 전송이 비활성화되거나 실패하면 예외 발생 (Fallback 로직 제거됨)
        /// </summary>
        private string DownloadToCache(ComponentUpdateInfo update)
        {
            var cacheFile = string.Empty;

            if (_useServerTransfer && _fileTransferService != null)
            {
                // 서버 홈 디렉토리 조회 (업로드 시 사용된 루트 경로와 일치시키기 위함)
                string homeDirectory = "";
                try 
                {
                    homeDirectory = _fileTransferService.GetHomeDirectory() ?? "";
                    FileLogger.Debug($"서버 홈 디렉토리: {homeDirectory}");
                }
                catch (Exception ex)
                {
                    FileLogger.Warning("서버 홈 디렉토리 조회 실패", ex);
                }
                
                string cacheFullPath = update.StoragePath.Replace(homeDirectory, _cachePath);

                cacheFile = Path.Combine(_cachePath, update.FileName);
                var cacheDir = Path.GetDirectoryName(cacheFullPath);

                if (!string.IsNullOrEmpty(cacheDir) && !Directory.Exists(cacheDir))
                    Directory.CreateDirectory(cacheDir);



                //// 서버 경로 구성: [HomeDirectory]/Patch/{FileName}
                var success = _fileTransferService.DownloadFile(update.StoragePath, cacheFile);

                if (!success)
                {
                    throw new InvalidOperationException($"파일 다운로드 실패: {update.StoragePath}");
                }

                var downloadedFileInfo = new FileInfo(cacheFile);
                FileLogger.Debug($"  다운로드 완료: {update.FileName}, 크기: {downloadedFileInfo.Length:N0} bytes");
            }
            else
            {
                throw new InvalidOperationException($"서버 연결이 설정되지 않아 '{update.FileName}'을(를) 다운로드할 수 없습니다.");
            }

            return cacheFile;
        }

        /// <summary>
        /// 캐시에서 설치 경로로 복사(패치)합니다. 파일이 사용 중일 경우 몇 번 재시도합니다.
        /// </summary>
        private void InstallFromCache(string cacheFile, string installPath)
        {
            var installDir = Path.GetDirectoryName(installPath);
            if (!string.IsNullOrEmpty(installDir) && !Directory.Exists(installDir))
            {
                Directory.CreateDirectory(installDir);
                FileLogger.Debug($"  설치 디렉토리 생성: {installDir}");
            }

            // 파일 잠금 경합 시 재시도
            int retries = 3;
            while (retries > 0)
            {
                try
                {
                    File.Copy(cacheFile, installPath, true);
                    FileLogger.Debug($"  파일 복사 완료: {Path.GetFileName(installPath)}");
                    return;
                }
                catch (IOException) when (retries > 1)
                {
                    FileLogger.Warning($"  파일 복사 재시도 중... 남은 횟수: {retries - 1}, 경로: {installPath}");
                    retries--;
                    System.Threading.Thread.Sleep(500);
                }
            }

            throw new IOException($"설치 불가: {Path.GetFileName(installPath)} - 파일이 사용 중일 수 있습니다.");
        }

        /// <summary>
        /// 파일의 SHA256 해시를 소문자 16진수 문자열로 반환합니다.
        /// </summary>
        private static string CalculateFileHash(string filePath)
        {
            using var sha256 = SHA256.Create();
            using var stream = File.OpenRead(filePath);
            var hash = sha256.ComputeHash(stream);
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }

        /// <summary>
        /// 파일의 어셈블리 버전 또는 파일 버전을 반환합니다.
        /// </summary>
        private static string GetFileVersion(string filePath)
        {
            try
            {
                var assemblyName = System.Reflection.AssemblyName.GetAssemblyName(filePath);
                return assemblyName.Version?.ToString() ?? "1.0.0.0";
            }
            catch
            {
                var versionInfo = System.Diagnostics.FileVersionInfo.GetVersionInfo(filePath);
                return versionInfo.FileVersion ?? "1.0.0.0";
            }
        }

        /// <summary>
        /// UpdateProgress 이벤트를 발생시킵니다.
        /// </summary>
        private void RaiseProgress(ComponentUpdateEventArgs args)
        {
            UpdateProgress?.Invoke(this, args);
        }

        #endregion

        public void Dispose()
        {
            if (_fileTransferService is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }

    #region Models

    /// <summary>
    /// 업데이트 유형 (신규 설치 또는 교체 업데이트)
    /// </summary>
    public enum UpdateType
    {
        NewInstall,
        Update
    }

    /// <summary>
    /// 업데이트 진행 단계
    /// </summary>
    public enum UpdatePhase
    {
        Checking,
        Downloading,
        Installing,
        Completed,
        Failed
    }

    /// <summary>
    /// 업데이트 컴포넌트 정보 전송용 DTO
    /// </summary>
    public class ComponentUpdateInfo
    {
        public string ComponentId { get; set; } = "";
        public string ComponentName { get; set; } = "";
        public ComponentType ComponentType { get; set; }
        public string FileName { get; set; } = "";
        public string? LocalVersion { get; set; }
        public string ServerVersion { get; set; } = "";
        public long FileSize { get; set; }
        public UpdateType UpdateType { get; set; }
        public bool IsRequired { get; set; }
        public int Priority { get; set; }
        public string InstallPath { get; set; } = "";
        public string? StoragePath { get; set; }
        public string GroupName { get; set; } = "Other";
    }

    /// <summary>
    /// 업데이트 진행 이벤트 인자
    /// </summary>
    public class ComponentUpdateEventArgs : EventArgs
    {
        public UpdatePhase Phase { get; set; }
        public string ComponentId { get; set; } = "";
        public string? ComponentName { get; set; }
        public int CurrentIndex { get; set; }
        public int TotalCount { get; set; }
        public int PercentComplete { get; set; }
        public string? ErrorMessage { get; set; }
    }

    /// <summary>
    /// 업데이트 결과 목록
    /// </summary>
    public class ComponentUpdateResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = "";
        public List<string> UpdatedComponents { get; set; } = new();
        public List<(string ComponentId, string Error)> FailedComponents { get; set; } = new();
    }

    #endregion
}