using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Security.Cryptography;
using nU3.Core.Attributes;
using nU3.Core.Repositories;
using nU3.Models;
using nU3.Connectivity;
using nU3.Core.Configuration;

namespace nU3.Core.Services
{
    /// <summary>
    /// 플러그인 모듈을 위한 격리된 AssemblyLoadContext
    /// Hot Deploy와 언로드를 지원합니다.
    /// </summary>
    internal class PluginLoadContext : AssemblyLoadContext
    {
        private readonly AssemblyDependencyResolver _resolver;

        public PluginLoadContext(string pluginPath) : base(isCollectible: true)
        {
            _resolver = new AssemblyDependencyResolver(pluginPath);
        }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            // 공유 어셈블리는 기본 컨텍스트에서 로드
            // 이렇게 하면 DI가 타입을 인식할 수 있음
            var sharedAssemblies = new[]
            {
                "nU3.Core",
                "nU3.Core.UI",
                "nU3.Models",
                "nU3.Connectivity",
                "System.Runtime",
                "System.Runtime.InteropServices",
                "Microsoft.Extensions.DependencyInjection.Abstractions"
            };

            if (sharedAssemblies.Any(name => assemblyName.Name?.StartsWith(name) == true))
            {
                return null; // 기본 컨텍스트에서 로드
            }

            string assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);
            if (assemblyPath != null)
            {
                return LoadFromAssemblyPath(assemblyPath);
            }

            return null;
        }

        protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
        {
            string libraryPath = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
            if (libraryPath != null)
            {
                return LoadUnmanagedDllFromPath(libraryPath);
            }

            return IntPtr.Zero;
        }
    }

    /// <summary>
    /// 모듈 로더 서비스는 런타임에 모듈(DLL)을 탐색하고 로드하며, 프로그램 인스턴스를 생성하는 책임을 수행합니다.
    /// 로컬에 파일이 없거나 업데이트가 필요한 경우 서버에서 자동으로 다운로드합니다.
    /// </summary>
    public class ModuleLoaderService
    {
        private readonly IModuleRepository _moduleRepo;
        private readonly IComponentRepository _compRepo;
        private readonly IProgramRepository _progRepo;
        private readonly IFileTransferService _fileTransfer;
        private readonly bool _useServerTransfer;
        private readonly string _serverModulePath;

        /// <summary>
        /// 컴포넌트 로드 상태를 외부로 알리기 위한 이벤트입니다.
        /// </summary>
        public event EventHandler<ComponentUpdateEventArgs>? UpdateProgress;

        /// <summary>
        /// 모듈 버전 충돌 감지 시 알림 이벤트입니다.
        /// </summary>
        public event EventHandler<ModuleVersionConflictEventArgs>? VersionConflict;

        private readonly Dictionary<string, Type> _progRegistry;
        private readonly Dictionary<string, nU3ProgramInfoAttribute> _progAttributeCache;
        private readonly Dictionary<string, string> _loadedModuleVersions; // ModuleId -> Version
        private readonly Dictionary<string, WeakReference> _loadContexts; // ModuleId -> LoadContext (for hot reload)
        private readonly Dictionary<string, string> _shadowCopyPaths; // ModuleId -> Shadow Path (for hot reload)
        private readonly string _cachePath;
        private readonly string _runtimePath;
        private readonly string _shadowCopyDirectory;

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

            _progRegistry = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);
            _progAttributeCache = new Dictionary<string, nU3ProgramInfoAttribute>(StringComparer.OrdinalIgnoreCase);
            _loadedModuleVersions = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            _loadContexts = new Dictionary<string, WeakReference>(StringComparer.OrdinalIgnoreCase);
            _shadowCopyPaths = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            // 중앙 집중식 경로 상수 사용
            _runtimePath = configuration?.GetValue<string>("RuntimeDirectory");
            if (string.IsNullOrEmpty(_runtimePath))
                _runtimePath = AppDomain.CurrentDomain.BaseDirectory;

            _cachePath = PathConstants.CacheDirectory;
            _shadowCopyDirectory = Path.Combine(_cachePath, "Shadow");
            
            // 서버 설정 로드
            var serverEnabled = configuration.GetValue<bool>("ServerConnection:Enabled", false);
            _serverModulePath = configuration.GetValue<string>("ModuleStorage:ServerPath") ?? "Modules";
            _useServerTransfer = configuration.GetValue<bool>("ModuleStorage:UseServerTransfer", true) && serverEnabled;

            Debug.WriteLine($"[ModuleLoader] Service Initialized. ServerEnabled: {serverEnabled}, UseTransfer: {_useServerTransfer}");
            EnsureDirectories();
        }

        private void EnsureDirectories()
        {
            if (!Directory.Exists(_runtimePath)) Directory.CreateDirectory(_runtimePath);
            if (!Directory.Exists(_cachePath)) Directory.CreateDirectory(_cachePath);
            if (!Directory.Exists(_shadowCopyDirectory)) Directory.CreateDirectory(_shadowCopyDirectory);
        }

        public Dictionary<string, Type> GetProgramRegistry() => _progRegistry;
        
        /// <summary>
        /// DB에서 읽어온 nU3ProgramInfoAttribute 캐시를 반환합니다.
        /// </summary>
        public Dictionary<string, nU3ProgramInfoAttribute> GetProgramAttributes() => _progAttributeCache;

        /// <summary>
        /// 모든 모듈을 검사하고 로드합니다.
        /// (Bootstrapper가 이미 최신화를 수행했다고 가정하고, 로컬 로드만 수행)
        /// </summary>
        public void LoadAllModules()
        {
            Debug.WriteLine("[ModuleLoader] LoadAllModules started.");
            LoadModulesFromRuntime();
        }

        /// <summary>
        /// 업데이트가 필요한 항목 목록을 반환합니다.
        /// </summary>
        public List<ComponentUpdateInfo> CheckForUpdates(string syncMode)
        {
            var updates = new List<ComponentUpdateInfo>();

            // 1. 컴포넌트 체크 (SYS_COMPONENT_MST)
            var activeComponents = _compRepo?.GetActiveVersions() ?? new List<ComponentVerDto>();
            foreach (var ver in activeComponents)
            {
                var comp = _compRepo?.GetComponent(ver.ComponentId);
                if (comp == null) continue;

                string relativePath = comp.FileName;
                //if (!string.IsNullOrEmpty(comp.GroupName)) 
                //    relativePath = Path.Combine(comp.GroupName, relativePath);
                
                // 실행 파일 기준 경로 계산 (컴포넌트는 보통 루트 또는 특정 폴더)
                string installFile = Path.Combine(_runtimePath, comp.ComponentId);
                
                // 화면 모듈 타입인 경우 Modules 폴더로 강제
                if (comp.ComponentType == ComponentType.ScreenModule)
                    installFile = Path.Combine(_runtimePath, comp.FileName);

                if (!File.Exists(installFile) || CalculateFileHash(installFile) != ver.FileHash)
                {
                    updates.Add(new ComponentUpdateInfo
                    {
                        ComponentId = comp.ComponentId,
                        ComponentName = comp.ComponentName,
                        ComponentType = comp.ComponentType,
                        FileName = comp.FileName,
                        ServerVersion = ver.Version,
                        FileSize = ver.FileSize,
                        IsRequired = comp.IsRequired,
                        Priority = comp.Priority,
                        InstallPath = installFile,
                        StoragePath = ver.StoragePath,
                        GroupName = comp.GroupName ?? "Framework"
                    });
                }
            }

            // 2. 모듈 체크 (SYS_MODULE_MST) - Full 모드인 경우
            if (string.Equals(syncMode, "Full", StringComparison.OrdinalIgnoreCase))
            {
                var targetModules = _moduleRepo.GetAllModules();
                var activeModuleVers = _moduleRepo.GetActiveVersions();

                foreach (var module in targetModules)
                {
                    var ver = activeModuleVers.FirstOrDefault(v => v.ModuleId == module.ModuleId);
                    if (ver == null) continue;

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
                    string installFile = Path.Combine(_runtimePath, relativePath);

                    if (!File.Exists(installFile) || CalculateFileHash(installFile) != ver.FileHash)
                    {
                        updates.Add(new ComponentUpdateInfo
                        {
                            ComponentId = module.ModuleId,
                            ComponentName = module.ModuleName,
                            ComponentType = ComponentType.ScreenModule,
                            FileName = module.FileName,
                            ServerVersion = ver.Version,
                            FileSize = ver.FileSize,
                            IsRequired = false,
                            Priority = 100, // 모듈은 컴포넌트 이후에
                            InstallPath = installFile,
                            StoragePath = ver.StoragePath,
                            GroupName = module.Category ?? "Modules"
                        });
                    }
                }
            }

            return updates.OrderBy(u => u.Priority).ToList();
        }

        /// <summary>
        /// 서버와 동기화를 수행합니다. (Bootstrapper 통합)
        /// </summary>
        public ComponentUpdateResult SyncWithServer(string syncMode = "Minimum")
        {
            var result = new ComponentUpdateResult();
            if (!_useServerTransfer || _fileTransfer == null)
            {
                result.Success = true;
                result.Message = "Server transfer disabled.";
                return result;
            }

            Debug.WriteLine($"[ModuleLoader] Starting sync. Mode: {syncMode}");
            RaiseProgress(new ComponentUpdateEventArgs { Phase = UpdatePhase.Checking });

            var updates = CheckForUpdates(syncMode);
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
                    string serverUrlPath = update.StoragePath ?? "";
                    string cacheFile = Path.Combine(_cachePath, update.FileName);
                    
                    if (DownloadToCache(serverUrlPath, cacheFile, update.ComponentName, update.ServerVersion))
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
                    else
                    {
                        throw new Exception("Download failed");
                    }
                }
                catch (Exception ex)
                {
                    result.FailedComponents.Add((update.ComponentId, ex.Message));
                    RaiseProgress(new ComponentUpdateEventArgs
                    {
                        Phase = UpdatePhase.Failed,
                        ComponentId = update.ComponentId,
                        ComponentName = update.ComponentName,
                        ErrorMessage = ex.Message
                    });
                }
                System.Threading.Thread.Sleep(50);
            }

            result.Success = !result.FailedComponents.Any();
            result.Message = result.Success ? $"{result.UpdatedComponents.Count}개 항목 업데이트 완료" : "업데이트 중 오류 발생";
            
            RaiseProgress(new ComponentUpdateEventArgs { Phase = UpdatePhase.Completed, PercentComplete = 100 });
            return result;
        }

        private void RaiseProgress(ComponentUpdateEventArgs args)
        {
            UpdateProgress?.Invoke(this, args);
        }

        /// <summary>
        /// 특정 모듈이 최신 상태인지 확인하고 필요 시 업데이트합니다.
        /// </summary>
        public bool EnsureModuleUpdated(string progId, string moduleId)
        {
            Debug.WriteLine($"[ModuleLoader] EnsureModuleUpdated: ModuleId={moduleId}");
            
            var activeVersion = _moduleRepo.GetActiveVersions()
                .FirstOrDefault(v => string.Equals(v.ModuleId, moduleId, StringComparison.OrdinalIgnoreCase));
            
            if (activeVersion == null)
            {
                Debug.WriteLine($"[ModuleLoader] !!! No active version found in DB for module: {moduleId}");
                return false;
            }

            if (_loadedModuleVersions.TryGetValue(moduleId, out var loadedVersion))
            {
                if (string.Equals(loadedVersion, activeVersion.Version, StringComparison.OrdinalIgnoreCase))
                {
                    Debug.WriteLine($"[ModuleLoader] Module {moduleId} already up to date (v{loadedVersion})");
                    return true;
                }
            }

            var module = _moduleRepo.GetAllModules()
                .FirstOrDefault(m => string.Equals(m.ModuleId, moduleId, StringComparison.OrdinalIgnoreCase));
            
            if (module == null)
            {
                Debug.WriteLine($"[ModuleLoader] !!! Module definition not found in DB: {moduleId}");
                return false;
            }

            return UpdateSingleModule(module, activeVersion);
        }

        private bool UpdateSingleModule(ModuleMstDto module, ModuleVerDto version)
        {
            string relativePath = Path.Combine(
                module.Category ?? "Common", 
                module.SubSystem ?? "Common", 
                module.FileName);

            string cacheFile = Path.Combine(_cachePath, relativePath);
            string runtimeFile = Path.Combine(_runtimePath, relativePath);
            
            // 서버 경로 구성
            string serverCategoryPath = module.Category ?? "";
            if (!string.IsNullOrEmpty(module.SubSystem))
            {
                serverCategoryPath = $"{serverCategoryPath}/{module.SubSystem}".TrimStart('/');
            }
            
            string serverRelativePath = string.IsNullOrEmpty(serverCategoryPath) 
                ? module.FileName 
                : $"{serverCategoryPath}/{module.FileName}";

            string serverUrlPath = $"{_serverModulePath}/{serverRelativePath}";

            Debug.WriteLine($"[ModuleLoader] Checking module: {module.ModuleName} (Hash={version.FileHash})");

            try
            {
                // 1. 런타임(실행) DLL을 최우선으로 검사 (DB 해시와 비교)
                bool isRuntimeValid = File.Exists(runtimeFile) && 
                                      string.Equals(CalculateFileHash(runtimeFile), version.FileHash, StringComparison.OrdinalIgnoreCase);

                if (isRuntimeValid)
                {
                    Debug.WriteLine($"[ModuleLoader] Runtime file is valid: {runtimeFile}");
                    if (!_loadedModuleVersions.ContainsKey(module.ModuleId))
                    {
                         ReloadModule(runtimeFile, module.ModuleId, version.Version);
                    }
                    return true;
                }

                // 2. 런타임이 유효하지 않으면 무조건 다운로드 시도 (캐시는 임시 저장소)
                bool downloaded = false;
                if (_useServerTransfer && _fileTransfer != null)
                {
                    bool isCacheValid = File.Exists(cacheFile) &&
                                        string.Equals(CalculateFileHash(cacheFile), version.FileHash, StringComparison.OrdinalIgnoreCase);
                    
                    if (isCacheValid)
                    {
                        Debug.WriteLine($"[ModuleLoader] Valid cache file found: {cacheFile}");
                        downloaded = true;
                    }
                    else if (DownloadToCache(serverUrlPath, cacheFile, module.ModuleName, version.Version))
                    {
                        downloaded = true;
                    }
                }
                
                if (!downloaded)
                {
                    Debug.WriteLine($"[ModuleLoader] !!! Failed to download update for {module.ModuleName}");
                    return false; 
                }

                // 3. 최신화된 캐시를 런타임으로 배포 시도
                DeployToRuntime(cacheFile, runtimeFile, module.ModuleName, version.Version);
                ReloadModule(runtimeFile, module.ModuleId, version.Version);
                
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ModuleLoader] !!! Error updating module {module.ModuleId}: {ex.Message}");
            }

            return false;
        }

        private bool DownloadToCache(string serverUrlPath, string cacheFile, string moduleName, string version)
        {
            try
            {
                string cacheDir = Path.GetDirectoryName(cacheFile);
                if (!Directory.Exists(cacheDir)) 
                    Directory.CreateDirectory(cacheDir);

                Debug.WriteLine($"[ModuleLoader] Downloading from server: {serverUrlPath}");
                if (_fileTransfer.DownloadFile(serverUrlPath, cacheFile))
                {
                    Debug.WriteLine($"[ModuleLoader] Downloaded to cache: {moduleName} v{version}");
                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ModuleLoader] !!! Error downloading {moduleName}: {ex.Message}");
            }
            return false;
        }

        private void DeployToRuntime(string cacheFile, string runtimeFile, string moduleName, string version)
        {
            try
            {
                string runtimeDir = Path.GetDirectoryName(runtimeFile);
                if (!Directory.Exists(runtimeDir)) 
                    Directory.CreateDirectory(runtimeDir);

                File.Copy(cacheFile, runtimeFile, true);
                Debug.WriteLine($"[ModuleLoader] Deployed to runtime: {moduleName} v{version}");
            }
            catch (IOException ioEx)
            {
                // 파일 잠금 - Shadow Copy로 Hot Deploy 가능
                Debug.WriteLine($"[ModuleLoader] Runtime file locked: {moduleName}. Using shadow copy for hot deploy.");
                Debug.WriteLine($"[ModuleLoader] Original file will be updated on next restart. Shadow copy allows immediate use.");
                
                // Shadow copy 방식이므로 예외를 던지지 않음
                // 다음 ReloadModule에서 shadow copy 사용
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ModuleLoader] !!! Error deploying {moduleName}: {ex.Message}");
                throw;
            }
        }

        private void LoadModulesFromRuntime()
        {
            return;

            if (!Directory.Exists(_runtimePath)) 
            {
                Debug.WriteLine($"[ModuleLoader] Runtime path does not exist: {_runtimePath}");
                return;
            }

            var dlls = Directory.GetFiles(_runtimePath, "*.dll", SearchOption.AllDirectories);
            Debug.WriteLine($"[ModuleLoader] Found {dlls.Length} assemblies in runtime path.");
            
            foreach (var dll in dlls)
            {
                LoadAssembly(dll, null, null);
            }

            Debug.WriteLine($"[ModuleLoader] Total Registered: {_progRegistry.Count} programs.");
        }

        private void LoadAssembly(string dllPath, string moduleId, string version)
        {
            try
            {
                // 기본 컨텍스트에서 LoadFrom 사용 (공유 어셈블리는 DI 호환성 필요)
                // 플러그인이 아닌 기본 프레임워크 어셈블리는 기본 컨텍스트 사용
                var assembly = Assembly.LoadFrom(dllPath);
                
                if (string.IsNullOrEmpty(version))
                {
                    version = assembly.GetName().Version?.ToString() ?? "Unknown";
                }
                
                foreach (var type in assembly.GetTypes())
                {
                    var attr = type.GetCustomAttribute<nU3ProgramInfoAttribute>();
                    if (attr != null)
                    {
                        _progRegistry[attr.ProgramId] = type;
                        _progAttributeCache[attr.ProgramId] = attr;
                        
                        if (!string.IsNullOrEmpty(moduleId))
                        {
                            _loadedModuleVersions[moduleId] = version;
                        }
                        else
                        {
                            var autoModuleId = attr.GetModuleId();
                            _loadedModuleVersions[autoModuleId] = version;
                        }
                        
                        Debug.WriteLine($"[ModuleLoader] Registered: {attr.ProgramId} -> {type.FullName} (v{version})");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ModuleLoader] !!! Error loading {Path.GetFileName(dllPath)}: {ex.Message}");
            }
        }

        private void ReloadModule(string dllPath, string moduleId, string version)
        {
            try
            {
                // 이미 로드된 버전 확인
                if (_loadedModuleVersions.TryGetValue(moduleId, out var currentVersion))
                {
                    // 같은 버전이면 스킵
                    if (string.Equals(currentVersion, version, StringComparison.OrdinalIgnoreCase))
                    {
                        Debug.WriteLine($"[ModuleLoader] Module {moduleId} v{version} already loaded. Reusing existing version.");
                        return;
                    }
                    
                    // ⚠️ 다른 버전 감지 - 버전 불일치 경고
                    Debug.WriteLine($"[ModuleLoader] !!! VERSION CONFLICT DETECTED !!!");
                    Debug.WriteLine($"[ModuleLoader] Module: {moduleId}");
                    Debug.WriteLine($"[ModuleLoader] Current Version: {currentVersion}");
                    Debug.WriteLine($"[ModuleLoader] Requested Version: {version}");
                    Debug.WriteLine($"[ModuleLoader] Action: Using EXISTING version to prevent type mismatch.");
                    Debug.WriteLine($"[ModuleLoader] Recommendation: Close all instances of this module and restart.");
                    
                    // 버전 충돌 이벤트 발생
                    RaiseVersionConflict(moduleId, currentVersion, version);
                    
                    // 기존 버전 유지 (타입 불일치 방지)
                    return;
                }
                
                // 최초 로드 - Shadow Copy 생성
                string shadowPath = CreateShadowCopy(dllPath, moduleId, version);
                
                // LoadFrom을 사용하여 기본 로드 컨텍스트에서 로드 (DI 호환성 유지)
                var assembly = Assembly.LoadFrom(shadowPath);
                
                foreach (var type in assembly.GetTypes())
                {
                    var attr = type.GetCustomAttribute<nU3ProgramInfoAttribute>();
                    if (attr != null)
                    {
                        _progRegistry[attr.ProgramId] = type;
                        _progAttributeCache[attr.ProgramId] = attr;
                        _loadedModuleVersions[moduleId] = version;
                        Debug.WriteLine($"[ModuleLoader] Loaded: {attr.ProgramId} -> v{version} (Shadow: {shadowPath})");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ModuleLoader] !!! Error reloading {Path.GetFileName(dllPath)}: {ex.Message}");
            }
        }

        /// <summary>
        /// Shadow Copy를 생성하여 원본 파일 잠금을 방지합니다.
        /// Hot Deploy를 지원하면서도 DI 호환성을 유지합니다.
        /// </summary>
        private string CreateShadowCopy(string originalPath, string moduleId, string version)
        {
            try
            {
                // 모듈별 Shadow 디렉토리 생성
                string moduleShadowDir = Path.Combine(_shadowCopyDirectory, moduleId, version);
                if (!Directory.Exists(moduleShadowDir))
                {
                    Directory.CreateDirectory(moduleShadowDir);
                }

                // Shadow 파일 경로
                string shadowPath = Path.Combine(moduleShadowDir, Path.GetFileName(originalPath));

                // 이미 존재하면 재사용 (같은 버전)
                if (File.Exists(shadowPath))
                {
                    Debug.WriteLine($"[ModuleLoader] Using existing shadow copy: {shadowPath}");
                    return shadowPath;
                }

                // 원본 파일을 Shadow 위치로 복사
                File.Copy(originalPath, shadowPath, overwrite: true);

                // 의존성 DLL도 복사 (같은 디렉토리에 있는 경우)
                string originalDir = Path.GetDirectoryName(originalPath);
                if (!string.IsNullOrEmpty(originalDir))
                {
                    foreach (var depFile in Directory.GetFiles(originalDir, "*.dll"))
                    {
                        if (Path.GetFileName(depFile) != Path.GetFileName(originalPath))
                        {
                            string depShadowPath = Path.Combine(moduleShadowDir, Path.GetFileName(depFile));
                            try
                            {
                                File.Copy(depFile, depShadowPath, overwrite: true);
                            }
                            catch
                            {
                                // 의존성 복사 실패는 무시 (이미 로드되어 있을 수 있음)
                            }
                        }
                    }
                }

                // Shadow 경로 추적 (정리용)
                _shadowCopyPaths[moduleId] = moduleShadowDir;

                Debug.WriteLine($"[ModuleLoader] Created shadow copy: {originalPath} -> {shadowPath}");
                return shadowPath;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ModuleLoader] !!! Failed to create shadow copy: {ex.Message}");
                // Shadow copy 실패 시 원본 사용 (재시작 필요)
                return originalPath;
            }
        }

        private string CalculateFileHash(string filePath)
        {
            if (!File.Exists(filePath)) return string.Empty;
            
            try
            {
                using (var sha256 = SHA256.Create())
                using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    var hash = sha256.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ModuleLoader] !!! Error calculating hash for {filePath}: {ex.Message}");
                return string.Empty;
            }
        }

        public Type GetProgramType(string progId)
        {
            // 1. 메모리에 이미 있다면 즉시 반환
            if (_progRegistry.TryGetValue(progId, out Type type))
                return type;
            
            // 2. 메모리에 없으면 DB 조회 시도 (On-Demand 로딩)
            Debug.WriteLine($"[ModuleLoader] Type for '{progId}' not found in registry. Checking DB...");
            
            if (_progRepo == null)
            {
                Debug.WriteLine("[ModuleLoader] !!! Error: _progRepo is NULL.");
                return null;
            }

            var progDto = _progRepo.GetProgramByProgId(progId);
            if (progDto != null && !string.IsNullOrEmpty(progDto.ModuleId))
            {
                Debug.WriteLine($"[ModuleLoader] Found mapping in DB: {progId} -> {progDto.ModuleId}");
                
                if (EnsureModuleUpdated(progId, progDto.ModuleId))
                {
                    if (_progRegistry.TryGetValue(progId, out type))
                    {
                        Debug.WriteLine($"[ModuleLoader] Successfully loaded type for {progId}");
                        return type;
                    }
                    else
                    {
                        var attr = GetProgramAttribute(progId);
                        if (attr != null) return LoadProgramByAttribute(attr);
                    }
                }
            }
            else
            {
                Debug.WriteLine($"[ModuleLoader] !!! Program '{progId}' not found in DB.");
            }

            return null;
        }
        
        public nU3ProgramInfoAttribute GetProgramAttribute(string progId)
        {
            if (_progAttributeCache.TryGetValue(progId, out var attr))
                return attr;
            
            return null;
        }
        
        public Type LoadProgramByAttribute(nU3ProgramInfoAttribute attr)
        {
            if (_progRegistry.TryGetValue(attr.ProgramId, out var cachedType))
                return cachedType;
            
            var dllPath = Path.Combine(_runtimePath, attr.SystemType, attr.SubSystem ?? string.Empty, $"{attr.DllName}.dll");
            
            if (!File.Exists(dllPath))
            {
                Debug.WriteLine($"[ModuleLoader] DLL not found: {dllPath}");
                return null;
            }
            
            try
            {
                // LoadFrom을 사용하여 기본 로드 컨텍스트에서 로드 (DI 호환성 유지)
                // 필요 시 Shadow Copy 사용 (이미 ReloadModule에서 처리됨)
                var assembly = Assembly.LoadFrom(dllPath);
                var type = assembly.GetType(attr.FullClassName);
                
                if (type != null)
                {
                    _progRegistry[attr.ProgramId] = type;
                    _progAttributeCache[attr.ProgramId] = attr;
                    Debug.WriteLine($"[ModuleLoader] Dynamically loaded: {attr.ProgramId} from {attr.FullClassName}");
                    return type;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ModuleLoader] !!! Error loading {attr.FullClassName}: {ex.Message}");
            }
            
            return null;
        }
        
        public object CreateProgramInstanceWithVersionCheck(string progId, string moduleId)
        {
            if (!EnsureModuleUpdated(progId, moduleId))
            {
                Debug.WriteLine($"[ModuleLoader] !!! Failed to update module for ProgId: {progId}");
                return null;
            }

            var type = GetProgramType(progId);
            if (type == null)
            {
                var attr = GetProgramAttribute(progId);
                if (attr != null) type = LoadProgramByAttribute(attr);
            }
            
            if (type != null)
            {
                try
                {
                    return Activator.CreateInstance(type);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[ModuleLoader] !!! Error creating instance of {type.FullName}: {ex.Message}");
                }
            }
            
            return null;
        }
        
        public object CreateProgramInstance(string progId)
        {
            Debug.WriteLine($"[ModuleLoader] >>> CreateProgramInstance Requested: {progId}");

            var type = GetProgramType(progId);
            if (type != null)
            {
                try
                {
                    return Activator.CreateInstance(type);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[ModuleLoader] !!! Error creating instance: {ex.Message}");
                    return null;
                }
            }

            var attr = GetProgramAttribute(progId);
            if (attr != null)
            {
                type = LoadProgramByAttribute(attr);
                if (type != null) return Activator.CreateInstance(type);
            }

            if (_progRepo == null) return null;

            var progDto = _progRepo.GetProgramByProgId(progId);
            if (progDto != null && !string.IsNullOrEmpty(progDto.ModuleId))
            {
                if (EnsureModuleUpdated(progId, progDto.ModuleId))
                {
                    type = GetProgramType(progId);
                    if (type == null)
                    {
                        attr = GetProgramAttribute(progId);
                        if (attr != null) type = LoadProgramByAttribute(attr);
                    }

                    if (type != null)
                    {
                        try { return Activator.CreateInstance(type); }
                        catch { }
                    }
                }
            }
            
            return null;
        }

        public string GetLoadedModuleVersion(string moduleId)
        {
            return _loadedModuleVersions.TryGetValue(moduleId, out var version) ? version : null;
        }

        /// <summary>
        /// 특정 모듈이 현재 사용 중인지 확인합니다.
        /// </summary>
        public bool IsModuleInUse(string moduleId)
        {
            return _loadedModuleVersions.ContainsKey(moduleId);
        }

        /// <summary>
        /// 버전 충돌 이벤트를 발생시킵니다.
        /// </summary>
        private void RaiseVersionConflict(string moduleId, string currentVersion, string requestedVersion)
        {
            VersionConflict?.Invoke(this, new ModuleVersionConflictEventArgs
            {
                ModuleId = moduleId,
                CurrentVersion = currentVersion,
                RequestedVersion = requestedVersion,
                Timestamp = DateTime.Now
            });
        }
    }

    /// <summary>
    /// 모듈 버전 충돌 이벤트 인자
    /// </summary>
    public class ModuleVersionConflictEventArgs : EventArgs
    {
        public string ModuleId { get; set; }
        public string CurrentVersion { get; set; }
        public string RequestedVersion { get; set; }
        public DateTime Timestamp { get; set; }
    }
}