using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using nU3.Core.Attributes;
using nU3.Core.Configuration;
using nU3.Core.Interfaces;
using nU3.Core.Logging;
using nU3.Core.Repositories;
using nU3.Models;

namespace nU3.Core.Services
{
    /// <summary>
    /// 플러그인 모듈 로드를 위한 격리된 AssemblyLoadContext입니다.
    /// 이 클래스는 개별 모듈이 서로 독립적인 어셈블리 버전을 가질 수 있도록 보장하며,
    /// 필요 시 해당 컨텍스트만 언로드(Unload)하여 Hot Deploy를 가능하게 합니다.
    /// </summary>
    internal class PluginLoadContext : AssemblyLoadContext
    {
        private readonly AssemblyDependencyResolver _resolver;

        /// <summary>
        /// 플러그인 경로를 기반으로 새로운 로드 컨텍스트를 초기화합니다.
        /// isCollectible을 true로 설정하여 나중에 이 컨텍스트가 언로드될 수 있도록 합니다.
        /// </summary>
        /// <param name="pluginPath">모듈 DLL이 위치한 실제 경로</param>
        public PluginLoadContext(string pluginPath) : base(isCollectible: true)
        {
            _resolver = new AssemblyDependencyResolver(pluginPath);
        }

        /// <summary>
        /// 어셈블리 로드 요청 시 호출됩니다. 
        /// 프레임워크 공유 어셈블리와 모듈 전용 어셈블리를 구분하여 로드합니다.
        /// </summary>
        protected override Assembly? Load(AssemblyName assemblyName)
        {
            // 1. 프레임워크 전반에서 공유되어야 하는 핵심 어셈블리 목록입니다.
            // 이 어셈블리들은 항상 Default Load Context에서 로드되어야 타입 호환성 문제가 발생하지 않습니다.
            var sharedAssemblies = new[]
            {
                "nU3.Core",
                "nU3.Core.UI",
                "nU3.Models",
                "nU3.Connectivity",
                "nU3.Business.Common",
                "System.Runtime",
                "System.Runtime.InteropServices",
                "Microsoft.Extensions.DependencyInjection",
                "Microsoft.Extensions.DependencyInjection.Abstractions",
                "Microsoft.Extensions.Configuration"
            };

            // 공유 어셈블리 패턴 확인
            if (sharedAssemblies.Any(name => assemblyName.Name?.StartsWith(name) == true))
            {
                return null; // null을 반환하면 런타임이 Default Context에서 로드를 시도합니다.
            }

            // 2. [Domain Common] 모듈들 간의 공유 데이터 모델이나 인터페이스를 담은 DLL들입니다.
            // *.Common 패턴은 관례적으로 공유 어셈블리로 처리합니다.
            if (assemblyName.Name != null && 
               (assemblyName.Name.EndsWith(".Common") || assemblyName.Name.Contains(".Modules.Common")))
            {
                return null;
            }

            // 3. 해당 모듈 전용의 의존성 어셈블리를 경로에서 찾아 로드합니다.
            string? assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);
            if (assemblyPath != null)
            {
                return LoadFromAssemblyPath(assemblyPath);
            }

            return null;
        }

        /// <summary>
        /// 비관리(Unmanaged) DLL(C++ 등) 로드가 필요한 경우 호출됩니다.
        /// </summary>
        protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
        {
            string? libraryPath = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
            if (libraryPath != null)
            {
                return LoadUnmanagedDllFromPath(libraryPath);
            }

            return IntPtr.Zero;
        }
    }

    /// <summary>
    /// 모듈 로더 서비스는 런타임에 DLL을 탐색, 동기화, 로드하고 인스턴스를 생성하는 핵심 서비스입니다.
    /// 주요 기능:
    /// - 서버 버전과 로컬 파일의 해시 비교를 통한 자동 업데이트
    /// - Shadow Copy를 통한 실행 중인 파일의 Hot Deploy 지원
    /// - 격리된 Load Context를 통한 어셈블리 버전 충돌 방지
    /// </summary>
    public class ModuleLoaderService
    {
        public const string MODULES_DIR = PathConstants.ModuleDirectoryStr; 
        public const string FRAMEWORKS_DIR = PathConstants.PatchDirectoryStr; 

        private readonly IModuleRepository _moduleRepo;
        private readonly IComponentRepository _compRepo;
        private readonly IProgramRepository _progRepo;
        private readonly IFileTransferService _fileTransfer;
        private readonly IConfiguration _configuration;
        private readonly bool _skipModuleUpdates;

        // --- 스레드 안전성을 위해 ConcurrentDictionary 및 lock 사용 ---
        private readonly ConcurrentDictionary<string, Type> _progRegistry;
        private readonly ConcurrentDictionary<string, nU3ProgramInfoAttribute> _progAttributeCache;
        private readonly ConcurrentDictionary<string, string> _loadedModuleVersions;
        private readonly ConcurrentDictionary<string, string> _shadowCopyPaths;
        private readonly object _syncLock = new object();

        private readonly string _cachePath;
        private readonly string _runtimePath;
        private readonly string _shadowCopyDirectory;

        /// <summary>
        /// 컴포넌트 업데이트 진행 상태를 외부(UI 등)로 알리기 위한 이벤트입니다.
        /// </summary>
        public event EventHandler<ComponentUpdateEventArgs>? UpdateProgress;

        /// <summary>
        /// 모듈 버전 충돌(이미 로드된 것과 다른 버전 요청) 발생 시 통지되는 이벤트입니다.
        /// </summary>
        public event EventHandler<ModuleVersionConflictEventArgs>? VersionConflict;

        public ModuleLoaderService(
            IModuleRepository moduleRepo,
            IComponentRepository compRepo,
            IProgramRepository progRepo,
            IFileTransferService fileTransfer,
            IConfiguration configuration)
        {
            _moduleRepo = moduleRepo;
            _compRepo = compRepo;
            _progRepo = progRepo;
            _fileTransfer = fileTransfer;
            _configuration = configuration;

            _progRegistry = new ConcurrentDictionary<string, Type>(StringComparer.OrdinalIgnoreCase);
            _progAttributeCache = new ConcurrentDictionary<string, nU3ProgramInfoAttribute>(StringComparer.OrdinalIgnoreCase);
            _loadedModuleVersions = new ConcurrentDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            _shadowCopyPaths = new ConcurrentDictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            // 개발 환경 설정 읽기 (디버깅 시 매번 다운로드하는 불편함 해소용)
            _skipModuleUpdates = configuration?.GetValue<bool>("Environment:SkipModuleUpdates") ?? false;

            // 런타임 경로 설정
            _runtimePath = configuration?.GetValue<string>("RuntimeDirectory") ?? AppDomain.CurrentDomain.BaseDirectory;
            _cachePath = PathConstants.CacheDirectory;
            _shadowCopyDirectory = Path.Combine(_cachePath, PathConstants.ShadowDirectoryStr);

            // [Domain Common] 위치가 변경된 공유 어셈블리 로드 지원
            AppDomain.CurrentDomain.AssemblyResolve += OnAssemblyResolve;

            LogManager.Info($"[ModuleLoader] 서비스 초기화됨. 실행 경로: {_runtimePath}", "System");
            if (_skipModuleUpdates)
            {
                LogManager.Warning("[ModuleLoader] 개발 모드: 모듈 자동 업데이트가 비활성화되었습니다.", "System");
            }

            EnsureDirectories();
        }

        private void EnsureDirectories()
        {
            try
            {
                if (!Directory.Exists(_runtimePath)) Directory.CreateDirectory(_runtimePath);
                if (!Directory.Exists(_cachePath)) Directory.CreateDirectory(_cachePath);
                if (!Directory.Exists(_shadowCopyDirectory)) Directory.CreateDirectory(_shadowCopyDirectory);
            }
            catch (Exception ex)
            {
                LogManager.Error("[ModuleLoader] 디렉토리 생성 실패", "System", ex);
            }
        }

        /// <summary>
        /// 현재 로드된 프로그램 타입 레지스트리를 반환합니다.
        /// </summary>
        public IDictionary<string, Type> GetProgramRegistry() => _progRegistry;

        /// <summary>
        /// 현재 캐싱된 프로그램 특성(Attribute) 정보를 반환합니다.
        /// </summary>
        public IDictionary<string, nU3ProgramInfoAttribute> GetProgramAttributes() => _progAttributeCache;

        /// <summary>
        /// 런타임 경로의 모든 DLL을 검색하여 모듈 특성이 있는 클래스를 로드합니다.
        /// </summary>
        public void LoadAllModules()
        {
            LogManager.Info("[ModuleLoader] 모든 로컬 모듈 로드 시작", "System");
            LoadModulesFromRuntime();
        }

        /// <summary>
        /// 서버 버전과 비교하여 업데이트가 필요한 항목 목록을 비동기로 반환합니다.
        /// </summary>
        public async Task<List<ComponentUpdateInfo>> CheckForUpdatesAsync(string syncMode)
        {
            return await Task.Run(() => CheckForUpdates(syncMode)).ConfigureAwait(false);
        }

        /// <summary>
        /// 서버 버전과 비교하여 업데이트가 필요한 항목 목록을 반환합니다.
        /// </summary>
        public List<ComponentUpdateInfo> CheckForUpdates(string syncMode)
        {
            var updates = new List<ComponentUpdateInfo>();

            try
            {
                // 1. 프레임워크 핵심 컴포넌트 체크 (SYS_COMPONENT_MST)
                var activeComponents = _compRepo?.GetActiveVersions() ?? new List<ComponentVerDto>();
                foreach (var ver in activeComponents)
                {
                    var comp = _compRepo?.GetComponent(ver.ComponentId);
                    if (comp == null) continue;

                    string installFile = comp.ComponentType == ComponentType.ScreenModule 
                        ? Path.Combine(_runtimePath, comp.FileName)
                        : Path.Combine(_runtimePath, comp.ComponentId);

                    if (!File.Exists(installFile) || CalculateFileHash(installFile) != ver.FileHash)
                    {
                        updates.Add(CreateUpdateInfoFromComponent(comp, ver, installFile));
                    }
                }

                // 2. 비즈니스 모듈 체크 (SYS_MODULE_MST) - Full 동기화 모드인 경우
                if (string.Equals(syncMode, "Full", StringComparison.OrdinalIgnoreCase))
                {
                    var targetModules = _moduleRepo.GetAllModules();
                    var activeModuleVers = _moduleRepo.GetActiveVersions();

                    foreach (var module in targetModules)
                    {
                        var ver = activeModuleVers.FirstOrDefault(v => v.ModuleId == module.ModuleId);
                        if (ver == null) continue;

                        string relativePath = GetModuleRelativePath(module);
                        string installFile = Path.Combine(_runtimePath, MODULES_DIR, relativePath);

                        if (!File.Exists(installFile) || CalculateFileHash(installFile) != ver.FileHash)
                        {
                            updates.Add(CreateUpdateInfoFromModule(module, ver, installFile));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.Error("[ModuleLoader] 업데이트 체크 중 오류 발생", "System", ex);
            }

            return updates.OrderBy(u => u.Priority).ToList();
        }

        /// <summary>
        /// 서버와 통신하여 필요한 모든 파일을 업데이트합니다. (비동기 지원)
        /// </summary>
        public async Task<ComponentUpdateResult> SyncWithServerAsync(string syncMode = "Minimum", CancellationToken ct = default)
        {
            var result = new ComponentUpdateResult();

            if (_skipModuleUpdates)
            {
                LogManager.Info("[ModuleLoader] 개발 모드: 서버 동기화 건너뜀", "System");
                result.Success = true;
                result.Message = "개발 모드 동기화 비활성";
                RaiseProgress(new ComponentUpdateEventArgs { Phase = UpdatePhase.Completed, PercentComplete = 100 });
                return result;
            }

            if (_fileTransfer == null)
            {
                result.Success = false;
                result.Message = "파일 전송 서비스를 사용할 수 없습니다.";
                return result;
            }

            LogManager.Info($"[ModuleLoader] 서버 동기화 시작 (모드: {syncMode})", "System");
            RaiseProgress(new ComponentUpdateEventArgs { Phase = UpdatePhase.Checking });

            var updates = await CheckForUpdatesAsync(syncMode).ConfigureAwait(false);
            if (!updates.Any())
            {
                result.Success = true;
                result.Message = "모든 항목이 최신 상태입니다.";
                RaiseProgress(new ComponentUpdateEventArgs { Phase = UpdatePhase.Completed, PercentComplete = 100 });
                return result;
            }

            int total = updates.Count;
            int current = 0;

            foreach (var update in updates)
            {
                if (ct.IsCancellationRequested) break;
                current++;
                
                RaiseProgress(new ComponentUpdateEventArgs
                {
                    Phase = UpdatePhase.Downloading,
                    ComponentId = update.ComponentId,
                    ComponentName = update.ComponentName,
                    CurrentIndex = current,
                    TotalCount = total,
                    PercentComplete = (int)((current - 0.5) / (double)total * 100)
                });

                try
                {
                    string cacheFile = Path.Combine(_cachePath, update.FileName);
                    
                    // 비동기 다운로드 및 배포
                    bool success = await DownloadToCacheAsync(update.StoragePath ?? "", cacheFile, update.ComponentName, update.ServerVersion).ConfigureAwait(false);
                    if (success)
                    {
                        RaiseProgress(new ComponentUpdateEventArgs
                        {
                            Phase = UpdatePhase.Installing,
                            ComponentId = update.ComponentId,
                            ComponentName = update.ComponentName,
                            CurrentIndex = current,
                            TotalCount = total,
                            PercentComplete = (int)(current / (double)total * 100)
                        });

                        DeployToRuntime(cacheFile, update.InstallPath, update.ComponentName, update.ServerVersion);
                        result.UpdatedComponents.Add(update.ComponentId);
                    }
                    else throw new Exception("다운로드 실패");
                }
                catch (Exception ex)
                {
                    LogManager.Error($"[ModuleLoader] {update.ComponentName} 업데이트 실패", "System", ex);
                    result.FailedComponents.Add((update.ComponentId, ex.Message));
                }
            }

            result.Success = !result.FailedComponents.Any();
            result.Message = result.Success ? $"{result.UpdatedComponents.Count}개 업데이트 완료" : "업데이트 중 일부 오류 발생";
            RaiseProgress(new ComponentUpdateEventArgs { Phase = UpdatePhase.Completed, PercentComplete = 100 });

            return result;
        }

        /// <summary>
        /// 하위 호환성을 위한 동기 동기화 메서드입니다.
        /// </summary>
        public ComponentUpdateResult SyncWithServer(string syncMode = "Minimum")
        {
            return SyncWithServerAsync(syncMode).GetAwaiter().GetResult();
        }

        /// <summary>
        /// 화면 이동 시 호출되며, 해당 프로그램이 속한 모듈이 최신인지 확인하고 로드합니다.
        /// </summary>
        public async Task<bool> EnsureModuleUpdatedAsync(string progId, string moduleId)
        {
            if (_skipModuleUpdates)
            {
                if (_loadedModuleVersions.ContainsKey(moduleId)) return true;
                return await LoadModuleLocalOnlyAsync(moduleId).ConfigureAwait(false);
            }

            var activeVersion = _moduleRepo.GetActiveVersions()
                .FirstOrDefault(v => string.Equals(v.ModuleId, moduleId, StringComparison.OrdinalIgnoreCase));

            if (activeVersion == null)
            {
                LogManager.Warning($"[ModuleLoader] 활성 버전을 찾을 수 없음: {moduleId}", "System");
                return false;
            }

            var module = _moduleRepo.GetAllModules()
                .FirstOrDefault(m => string.Equals(m.ModuleId, moduleId, StringComparison.OrdinalIgnoreCase));

            if (module == null) return false;

            return await UpdateSingleModuleAsync(module, activeVersion).ConfigureAwait(false);
        }

        /// <summary>
        /// 동기 버전의 모듈 업데이트 보장 메서드입니다.
        /// </summary>
        public bool EnsureModuleUpdated(string progId, string moduleId)
        {
            return EnsureModuleUpdatedAsync(progId, moduleId).GetAwaiter().GetResult();
        }

        private async Task<bool> UpdateSingleModuleAsync(ModuleMstDto module, ModuleVerDto version)
        {
            string relativePath = GetModuleRelativePath(module);
            string relativePathWithModules = Path.Combine(MODULES_DIR, relativePath);

            string cacheFile = Path.Combine(_cachePath, relativePathWithModules);
            string runtimeFile = Path.Combine(_runtimePath, relativePathWithModules);
            string serverUrlPath = $"{MODULES_DIR}/{GetModuleServerPath(module)}";

            try
            {
                // 1. 현재 로컬 파일이 DB의 해시값과 일치하는지 확인
                if (File.Exists(runtimeFile) && string.Equals(CalculateFileHash(runtimeFile), version.FileHash, StringComparison.OrdinalIgnoreCase))
                {
                    if (!_loadedModuleVersions.ContainsKey(module.ModuleId))
                    {
                        ReloadModule(runtimeFile, module.ModuleId, version.Version);
                    }
                    return true;
                }

                // 2. 일치하지 않으면 캐시 확인 후 다운로드
                bool downloaded = false;
                if (File.Exists(cacheFile) && string.Equals(CalculateFileHash(cacheFile), version.FileHash, StringComparison.OrdinalIgnoreCase))
                {
                    downloaded = true;
                }
                else
                {
                    downloaded = await DownloadToCacheAsync(serverUrlPath, cacheFile, module.ModuleName, version.Version).ConfigureAwait(false);
                }

                if (!downloaded) return false;

                // 3. 런타임 배포 및 로드
                DeployToRuntime(cacheFile, runtimeFile, module.ModuleName, version.Version);
                ReloadModule(runtimeFile, module.ModuleId, version.Version);

                return true;
            }
            catch (Exception ex)
            {
                LogManager.Error($"[ModuleLoader] {module.ModuleId} 모듈 개별 업데이트 실패", "System", ex);
                return false;
            }
        }

        private async Task<bool> DownloadToCacheAsync(string serverUrlPath, string cacheFile, string moduleName, string version)
        {
            if (_fileTransfer == null) return false;
            
            try
            {
                string? cacheDir = Path.GetDirectoryName(cacheFile);
                if (!string.IsNullOrEmpty(cacheDir) && !Directory.Exists(cacheDir))
                    Directory.CreateDirectory(cacheDir);

                bool success = await _fileTransfer.DownloadFileAsync(serverUrlPath, cacheFile).ConfigureAwait(false);
                if (success)
                {
                    LogManager.Debug($"[ModuleLoader] 다운로드 완료: {moduleName} (v{version})", "System");
                    return true;
                }
            }
            catch (Exception ex)
            {
                LogManager.Error($"[ModuleLoader] 다운로드 중 오류: {moduleName}", "System", ex);
            }
            return false;
        }

        /// <summary>
        /// 파일을 실행 경로(Runtime)로 복사합니다. 파일이 사용 중인 경우 Shadow Copy 전략을 위해 경고만 남깁니다.
        /// </summary>
        private void DeployToRuntime(string cacheFile, string runtimeFile, string moduleName, string version)
        {
            try
            {
                string? runtimeDir = Path.GetDirectoryName(runtimeFile);
                if (!string.IsNullOrEmpty(runtimeDir) && !Directory.Exists(runtimeDir))
                    Directory.CreateDirectory(runtimeDir);

                File.Copy(cacheFile, runtimeFile, true);
                LogManager.Info($"[ModuleLoader] 파일 배포 완료: {moduleName} v{version}", "System");
            }
            catch (IOException)
            {
                // 파일이 사용 중일 때 발생하는 일반적인 현상입니다.
                // ReloadModule 메서드에서 Shadow Copy를 생성하여 이 문제를 해결합니다.
                LogManager.Warning($"[ModuleLoader] {moduleName} 파일이 사용 중입니다. Shadow Copy를 통해 로드됩니다.", "System");
            }
            catch (Exception ex)
            {
                LogManager.Error($"[ModuleLoader] 배포 실패: {moduleName}", "System", ex);
                throw;
            }
        }

        private void LoadModulesFromRuntime()
        {
            string modulesPath = Path.Combine(_runtimePath, MODULES_DIR);
            if (!Directory.Exists(modulesPath))
            {
                LogManager.Debug($"[ModuleLoader] 모듈 디렉토리가 존재하지 않습니다: {modulesPath}", "System");
                return;
            }

            // 전체 실행 경로가 아닌 'Modules' 폴더만 한정하여 검색 (성능 최적화)
            var dlls = Directory.GetFiles(modulesPath, "*.dll", SearchOption.AllDirectories);
            LogManager.Info($"[ModuleLoader] 모듈 디렉토리에서 {dlls.Length}개의 후보 어셈블리를 발견했습니다.", "System");
            
            foreach (var dll in dlls)
            {
                LoadAssembly(dll, null, null);
            }
        }

        /// <summary>
        /// 어셈블리를 로드하고 nU3ProgramInfoAttribute가 설정된 클래스를 레지스트리에 등록합니다.
        /// </summary>
        private void LoadAssembly(string dllPath, string? moduleId, string? version)
        {
            try
            {
                // DI 및 타입 공유를 위해 기본 Load Context(Default) 사용
                var assembly = Assembly.LoadFrom(dllPath);
                version ??= assembly.GetName().Version?.ToString() ?? "1.0.0.0";

                foreach (var type in assembly.GetTypes())
                {
                    var attr = type.GetCustomAttribute<nU3ProgramInfoAttribute>();
                    if (attr != null)
                    {
                        _progRegistry[attr.ProgramId] = type;
                        _progAttributeCache[attr.ProgramId] = attr;

                        string effectiveModuleId = moduleId ?? attr.GetModuleId();
                        _loadedModuleVersions[effectiveModuleId] = version;

                        LogManager.Debug($"[ModuleLoader] 등록: {attr.ProgramId} (v{version})", "System");
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.Trace($"[ModuleLoader] 어셈블리 로드 스킵 ({Path.GetFileName(dllPath)}): {ex.Message}");
            }
        }

        /// <summary>
        /// 모듈을 다시 로드하거나 최초 로드 시 Shadow Copy를 생성하여 로드합니다.
        /// </summary>
        private void ReloadModule(string dllPath, string moduleId, string version)
        {
            try
            {
                if (_loadedModuleVersions.TryGetValue(moduleId, out var currentVersion))
                {
                    // 이미 로드된 버전과 동일하면 무시
                    if (string.Equals(currentVersion, version, StringComparison.OrdinalIgnoreCase)) return;

                    // 버전 충돌 발생 (프로그램 재시작 권고)
                    LogManager.Warning($"[ModuleLoader] 버전 충돌: {moduleId} (로드됨:{currentVersion}, 요청:{version})", "System");
                    RaiseVersionConflict(moduleId, currentVersion, version);
                    return;
                }

                // Shadow Copy를 생성하여 원본 DLL 잠금 방지
                string shadowPath = CreateShadowCopy(dllPath, moduleId, version);

                var assembly = Assembly.LoadFrom(shadowPath);
                foreach (var type in assembly.GetTypes())
                {
                    var attr = type.GetCustomAttribute<nU3ProgramInfoAttribute>();
                    if (attr != null)
                    {
                        _progRegistry[attr.ProgramId] = type;
                        _progAttributeCache[attr.ProgramId] = attr;
                        _loadedModuleVersions[moduleId] = version;
                    }
                }
                LogManager.Info($"[ModuleLoader] 모듈 로드 완료: {moduleId} (v{version})", "System");
            }
            catch (Exception ex)
            {
                LogManager.Error($"[ModuleLoader] 모듈 다시 로드 실패: {moduleId}", "System", ex);
            }
        }

        /// <summary>
        /// 원본 DLL을 캐시의 Shadow 폴더로 복사하여 로드함으로써 원본 파일의 쓰기 권한을 유지합니다.
        /// 이를 통해 프로그램 실행 중에도 백그라운드에서 원본 DLL을 업데이트할 수 있습니다.
        /// </summary>
        private string CreateShadowCopy(string originalPath, string moduleId, string version)
        {
            try
            {
                string moduleShadowDir = Path.Combine(_shadowCopyDirectory, moduleId, version);
                if (!Directory.Exists(moduleShadowDir)) Directory.CreateDirectory(moduleShadowDir);

                string shadowPath = Path.Combine(moduleShadowDir, Path.GetFileName(originalPath));

                if (File.Exists(shadowPath)) return shadowPath;

                // 주 DLL 복사
                File.Copy(originalPath, shadowPath, overwrite: true);

                // 같은 폴더 내의 의존성 DLL들도 함께 복사 (로컬 의존성 해결용)
                string? originalDir = Path.GetDirectoryName(originalPath);
                if (!string.IsNullOrEmpty(originalDir))
                {
                    foreach (var depFile in Directory.GetFiles(originalDir, "*.dll"))
                    {
                        if (Path.GetFileName(depFile) == Path.GetFileName(originalPath)) continue;
                        
                        string depShadowPath = Path.Combine(moduleShadowDir, Path.GetFileName(depFile));
                        try { File.Copy(depFile, depShadowPath, true); } catch { /* 복사 실패 무시 */ }
                    }
                }

                _shadowCopyPaths[moduleId] = moduleShadowDir;
                return shadowPath;
            }
            catch (Exception ex)
            {
                LogManager.Error($"[ModuleLoader] Shadow Copy 생성 실패: {moduleId}", "System", ex);
                return originalPath;
            }
        }

        /// <summary>
        /// 파일의 SHA256 해시값을 계산하여 문자열로 반환합니다.
        /// </summary>
        private string CalculateFileHash(string filePath)
        {
            if (!File.Exists(filePath)) return string.Empty;

            try
            {
                using var sha256 = SHA256.Create();
                using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                var hash = sha256.ComputeHash(stream);
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
            catch (Exception ex)
            {
                LogManager.Trace($"[ModuleLoader] 해시 계산 오류 ({Path.GetFileName(filePath)}): {ex.Message}");
                return string.Empty;
            }
        }

        /// <summary>
        /// 프로그램 ID에 해당하는 타입을 반환합니다. 비동기로 업데이트 여부를 먼저 확인합니다.
        /// </summary>
        public async Task<Type?> GetProgramTypeAsync(string progId)
        {
            if (_progRepo == null) return _progRegistry.GetValueOrDefault(progId);

            var progDto = _progRepo.GetProgramByProgId(progId);
            if (progDto != null && !string.IsNullOrEmpty(progDto.ModuleId))
            {
                if (await EnsureModuleUpdatedAsync(progId, progDto.ModuleId).ConfigureAwait(false))
                {
                    if (_progRegistry.TryGetValue(progId, out Type? type)) return type;
                    
                    var attr = GetProgramAttribute(progId);
                    if (attr != null) return LoadProgramByAttribute(attr);
                }
            }
            else if (_progRegistry.TryGetValue(progId, out var cachedType)) return cachedType;

            return null;
        }

        /// <summary>
        /// 하위 호환성을 위한 동기 타입 조회 메서드입니다.
        /// </summary>
        public Type? GetProgramType(string progId) => GetProgramTypeAsync(progId).GetAwaiter().GetResult();

        public nU3ProgramInfoAttribute? GetProgramAttribute(string progId)
        {
            return _progAttributeCache.GetValueOrDefault(progId);
        }

        /// <summary>
        /// 특성 정보를 기반으로 어셈블리를 찾아 타입을 동적으로 로드합니다.
        /// </summary>
        public Type? LoadProgramByAttribute(nU3ProgramInfoAttribute attr)
        {
            if (_progRegistry.TryGetValue(attr.ProgramId, out var cachedType)) return cachedType;

            string dllPath = Path.Combine(_runtimePath, MODULES_DIR, attr.SystemType, attr.SubSystem ?? string.Empty, $"{attr.DllName}.dll");
            if (!File.Exists(dllPath)) return null;

            try
            {
                var assembly = Assembly.LoadFrom(dllPath);
                var type = assembly.GetType(attr.FullClassName);

                if (type != null)
                {
                    _progRegistry[attr.ProgramId] = type;
                    _progAttributeCache[attr.ProgramId] = attr;
                    return type;
                }
            }
            catch (Exception ex)
            {
                LogManager.Error($"[ModuleLoader] 특성 기반 로드 실패: {attr.FullClassName}", "System", ex);
            }
            return null;
        }

        /// <summary>
        /// 프로그램 인스턴스를 생성합니다. 버전 체크와 업데이트를 포함합니다. (비동기)
        /// </summary>
        public async Task<object?> CreateProgramInstanceAsync(string progId)
        {
            LogManager.Debug($"[ModuleLoader] 프로그램 인스턴스 생성 시도: {progId}", "System");

            var type = await GetProgramTypeAsync(progId).ConfigureAwait(false);
            if (type == null) return null;

            try
            {
                return Activator.CreateInstance(type);
            }
            catch (Exception ex)
            {
                LogManager.Error($"[ModuleLoader] {progId} 인스턴스 생성 중 오류 발생", "System", ex);
                return null;
            }
        }

        /// <summary>
        /// 하위 호환성을 위한 동기 인스턴스 생성 메서드입니다.
        /// </summary>
        public object? CreateProgramInstance(string progId) => CreateProgramInstanceAsync(progId).GetAwaiter().GetResult();

        public string? GetLoadedModuleVersion(string moduleId)
        {
            return _loadedModuleVersions.GetValueOrDefault(moduleId);
        }

        private void RaiseProgress(ComponentUpdateEventArgs args) => UpdateProgress?.Invoke(this, args);

        private void RaiseVersionConflict(string moduleId, string current, string requested)
        {
            VersionConflict?.Invoke(this, new ModuleVersionConflictEventArgs
            {
                ModuleId = moduleId,
                CurrentVersion = current,
                RequestedVersion = requested,
                Timestamp = DateTime.Now
            });
        }

        /// <summary>
        /// 기본 로드 컨텍스트에서 어셈블리를 찾지 못했을 때 호출됩니다.
        /// Modules 폴더 내의 Domain Common DLL을 찾아 로드하거나 필요한 경우 서버에서 즉시 다운로드합니다.
        /// </summary>
        private Assembly? OnAssemblyResolve(object? sender, ResolveEventArgs args)
        {
            var assemblyName = new AssemblyName(args.Name);
            string name = assemblyName.Name ?? "";

            if (name.EndsWith(".Common") || name.Contains(".Modules.Common"))
            {
                // 1. 온디맨드 다운로드 시도
                string? downloadedPath = EnsureDomainCommonDownloaded(name);
                if (!string.IsNullOrEmpty(downloadedPath) && File.Exists(downloadedPath))
                {
                    return Assembly.LoadFrom(downloadedPath);
                }

                // 2. 로컬 검색 Fallback
                string modulesRoot = Path.Combine(_runtimePath, MODULES_DIR);
                if (Directory.Exists(modulesRoot))
                {
                    try 
                    {
                        var files = Directory.GetFiles(modulesRoot, $"{name}.dll", SearchOption.AllDirectories);
                        var targetFile = files.FirstOrDefault();
                        if (!string.IsNullOrEmpty(targetFile)) return Assembly.LoadFrom(targetFile);
                    }
                    catch { }
                }
            }
            return null;
        }

        /// <summary>
        /// 필요한 공통 도메인 DLL이 로컬에 없는 경우 서버에서 즉시 다운로드합니다.
        /// </summary>
        private string? EnsureDomainCommonDownloaded(string assemblyName)
        {
            if (_compRepo == null || _fileTransfer == null || _skipModuleUpdates) return null;

            try
            {
                string componentId = $"{assemblyName}.dll";
                var component = _compRepo.GetComponent(componentId);
                var activeVer = _compRepo.GetActiveVersions().FirstOrDefault(v => v.ComponentId == componentId);
                
                if (component == null || activeVer == null) return null;

                string installPathRel = component.InstallPath;
                if (string.IsNullOrEmpty(installPathRel)) 
                    installPathRel = Path.Combine(MODULES_DIR, component.GroupName ?? "Common", component.FileName);

                string runtimeFile = Path.Combine(_runtimePath, installPathRel);

                if (File.Exists(runtimeFile) && string.Equals(CalculateFileHash(runtimeFile), activeVer.FileHash, StringComparison.OrdinalIgnoreCase))
                    return runtimeFile;

                // 동기 메서드 내에서 호출되므로 동기 다운로드 사용
                string cacheFile = Path.Combine(_cachePath, component.FileName);
                if (_fileTransfer.DownloadFile(activeVer.StoragePath, cacheFile))
                {
                    DeployToRuntime(cacheFile, runtimeFile, component.ComponentName, activeVer.Version);
                    return runtimeFile;
                }
            }
            catch (Exception ex)
            {
                LogManager.Error($"[ModuleLoader] DomainCommon 온디맨드 다운로드 실패: {assemblyName}", "System", ex);
            }
            return null;
        }

        // --- Helper Methods ---

        private async Task<bool> LoadModuleLocalOnlyAsync(string moduleId)
        {
            return await Task.Run(() => {
                var moduleDto = _moduleRepo.GetAllModules().FirstOrDefault(m => string.Equals(m.ModuleId, moduleId, StringComparison.OrdinalIgnoreCase));
                if (moduleDto == null) return false;

                string runtimeFile = Path.Combine(_runtimePath, MODULES_DIR, GetModuleRelativePath(moduleDto));
                if (File.Exists(runtimeFile))
                {
                    LoadAssembly(runtimeFile, moduleId, "dev-local");
                    return true;
                }
                return false;
            }).ConfigureAwait(false);
        }

        private string GetModuleRelativePath(ModuleMstDto module) => Path.Combine(module.Category ?? "Common", module.SubSystem ?? "Common", module.FileName);

        private string GetModuleServerPath(ModuleMstDto module)
        {
            string path = module.Category ?? "";
            if (!string.IsNullOrEmpty(module.SubSystem)) path = $"{path}/{module.SubSystem}".TrimStart('/');
            return string.IsNullOrEmpty(path) ? module.FileName : $"{path}/{module.FileName}";
        }

        private ComponentUpdateInfo CreateUpdateInfoFromComponent(ComponentMstDto comp, ComponentVerDto ver, string installPath) => new()
        {
            ComponentId = comp.ComponentId,
            ComponentName = comp.ComponentName,
            ComponentType = comp.ComponentType,
            FileName = comp.FileName,
            ServerVersion = ver.Version,
            FileSize = ver.FileSize,
            IsRequired = comp.IsRequired,
            Priority = comp.Priority,
            InstallPath = installPath,
            StoragePath = ver.StoragePath,
            GroupName = comp.GroupName ?? "Framework"
        };

        private ComponentUpdateInfo CreateUpdateInfoFromModule(ModuleMstDto module, ModuleVerDto ver, string installPath) => new()
        {
            ComponentId = module.ModuleId,
            ComponentName = module.ModuleName,
            ComponentType = ComponentType.ScreenModule,
            FileName = module.FileName,
            ServerVersion = ver.Version,
            FileSize = ver.FileSize,
            IsRequired = false,
            Priority = 100,
            InstallPath = installPath,
            StoragePath = ver.StoragePath,
            GroupName = module.Category ?? "Modules"
        };
    }

    public class ModuleVersionConflictEventArgs : EventArgs
    {
        public string ModuleId { get; set; } = string.Empty;
        public string CurrentVersion { get; set; } = string.Empty;
        public string RequestedVersion { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
    }
}
