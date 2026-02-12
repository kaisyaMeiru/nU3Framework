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
using nU3.Core.Interfaces;
using nU3.Core.Configuration;

namespace nU3.Core.Services
{
    /// <summary>
    /// 플러그인 모듈을 위한 격리된 AssemblyLoadContext입니다.
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
            // 공유 어셈블리는 기본 컨텍스트에서 로드합니다.
            // 이렇게 하면 DI(의존성 주입) 시스템이 타입을 올바르게 인식할 수 있습니다.
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
                return null; // 기본 컨텍스트에서 로드하도록 함
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
        public const string MODULES_DIR = PathConstants.ModuleDirectoryStr; 
        public const string FRAMEWORKS_DIR = PathConstants.PatchDirectoryStr; 

        private readonly IModuleRepository _moduleRepo;
        private readonly IComponentRepository _compRepo;
        private readonly IProgramRepository _progRepo;
        private readonly IFileTransferService _fileTransfer;
        private readonly IConfiguration _configuration;
        private readonly bool _skipModuleUpdates;

        /// <summary>
        /// 컴포넌트 업데이트 진행 상태를 외부로 알리기 위한 이벤트입니다.
        /// </summary>
        public event EventHandler<ComponentUpdateEventArgs>? UpdateProgress;

        /// <summary>
        /// 모듈 버전 충돌 감지 시 발생하는 이벤트입니다.
        /// </summary>
        public event EventHandler<ModuleVersionConflictEventArgs>? VersionConflict;

        private readonly Dictionary<string, Type> _progRegistry;
        private readonly Dictionary<string, nU3ProgramInfoAttribute> _progAttributeCache;
        private readonly Dictionary<string, string> _loadedModuleVersions; // ModuleId -> Version
        private readonly Dictionary<string, WeakReference> _loadContexts; // ModuleId -> LoadContext (Hot Reload용)
        private readonly Dictionary<string, string> _shadowCopyPaths; // ModuleId -> Shadow Path (Hot Reload용)
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
            _configuration = configuration;

            _progRegistry = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);
            _progAttributeCache = new Dictionary<string, nU3ProgramInfoAttribute>(StringComparer.OrdinalIgnoreCase);
            _loadedModuleVersions = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            _loadContexts = new Dictionary<string, WeakReference>(StringComparer.OrdinalIgnoreCase);
            _shadowCopyPaths = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            // 개발 환경 설정 읽기
            _skipModuleUpdates = configuration?.GetValue<bool>("Environment:SkipModuleUpdates") ?? false;

            // 중앙 집중식 경로 설정 사용
            _runtimePath = configuration?.GetValue<string>("RuntimeDirectory");
            if (string.IsNullOrEmpty(_runtimePath))
                _runtimePath = AppDomain.CurrentDomain.BaseDirectory;

            _cachePath = PathConstants.CacheDirectory;
            _shadowCopyDirectory = Path.Combine(_cachePath, PathConstants.ShadowDirectoryStr);

            Debug.WriteLine($"[ModuleLoader] 서비스 초기화됨. 실행 경로: {_runtimePath}");
            if (_skipModuleUpdates)
            {
                Debug.WriteLine($"[ModuleLoader] 개발 모드: 모듈 업데이트가 비활성화되었습니다.");
            }
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
        /// (Bootstrapper가 이미 최신화를 수행했다고 가정하고, 로컬 로드만 수행합니다)
        /// </summary>
        public void LoadAllModules()
        {
            Debug.WriteLine("[ModuleLoader] 모든 모듈 로드 시작.");
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

                // 실행 파일 기준 경로 계산 (컴포넌트는 보통 루트 또는 특정 폴더)
                string installFile = Path.Combine(_runtimePath, comp.ComponentId);

                // 화면 모듈 타입인 경우 Modules 폴더로 경로 강제 설정
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

            // 2. 모듈 체크 (SYS_MODULE_MST) - Full 모드인 경우 실행
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
                    string installFile = Path.Combine(_runtimePath, MODULES_DIR, relativePath);

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
                            Priority = 100, // 모듈은 컴포넌트 이후에 업데이트
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

            // 개발 환경에서 동기화 건너뛰기
            if (_skipModuleUpdates)
            {
                Debug.WriteLine($"[ModuleLoader] 개발 모드: 서버 동기화 건너뜀");
                result.Success = true;
                result.Message = "개발 모드: 서버 동기화가 비활성화되었습니다.";
                RaiseProgress(new ComponentUpdateEventArgs { Phase = UpdatePhase.Completed, PercentComplete = 100 });
                return result;
            }

            if (_fileTransfer == null)
            {
                result.Success = false;
                result.Message = "파일 전송 서비스를 사용할 수 없습니다.";
                return result;
            }

            Debug.WriteLine($"[ModuleLoader] 동기화 시작. 모드: {syncMode}");
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
                        throw new Exception("다운로드에 실패했습니다.");
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
            // 개발 환경에서 모듈 업데이트 건너뛰기
            if (_skipModuleUpdates)
            {
                Debug.WriteLine($"[ModuleLoader] 개발 모드: 모듈 업데이트 건너뜀 (ModuleId={moduleId})");
                
                // 이미 로드된 모듈이 있으면 true 반환
                if (_loadedModuleVersions.ContainsKey(moduleId))
                {
                    Debug.WriteLine($"[ModuleLoader] 개발 모드: 기존 로드된 모듈 사용 (ModuleId={moduleId})");
                    return true;
                }

                // 로컬 파일이 존재하는지 확인하고 로드 시도
                var moduleDto = _moduleRepo.GetAllModules()
                    .FirstOrDefault(m => string.Equals(m.ModuleId, moduleId, StringComparison.OrdinalIgnoreCase));

                if (moduleDto != null)
                {
                    string relativePath = Path.Combine(
                        moduleDto.Category ?? "Common",
                        moduleDto.SubSystem ?? "Common",
                        moduleDto.FileName);

                    string relativePathWithModules = Path.Combine(MODULES_DIR, relativePath);
                    string runtimeFile = Path.Combine(_runtimePath, relativePathWithModules);

                    if (File.Exists(runtimeFile))
                    {
                        Debug.WriteLine($"[ModuleLoader] 개발 모드: 로컬 파일 발견, 로드 시도 (Path={runtimeFile})");
                        try
                        {
                            LoadAssembly(runtimeFile, moduleId, "dev-local");
                            return true;
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"[ModuleLoader] 개발 모드: 로컬 파일 로드 실패 - {ex.Message}");
                            return false;
                        }
                    }
                }

                Debug.WriteLine($"[ModuleLoader] 개발 모드: 로컬 파일이 존재하지 않음 (ModuleId={moduleId})");
                return false;
            }

            Debug.WriteLine($"[ModuleLoader] 모듈 업데이트 확인: ModuleId={moduleId}");

            var activeVersion = _moduleRepo.GetActiveVersions()
                .FirstOrDefault(v => string.Equals(v.ModuleId, moduleId, StringComparison.OrdinalIgnoreCase));

            if (activeVersion == null)
            {
                Debug.WriteLine($"[ModuleLoader] !!! DB에서 해당 모듈의 활성 버전을 찾을 수 없음: {moduleId}");
                return false;
            }

            var module = _moduleRepo.GetAllModules()
                .FirstOrDefault(m => string.Equals(m.ModuleId, moduleId, StringComparison.OrdinalIgnoreCase));

            if (module == null)
            {
                Debug.WriteLine($"[ModuleLoader] !!! DB에서 모듈 정의를 찾을 수 없음: {moduleId}");
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

            // runtime / cache에는 "Modules" 루트를 포함하도록 구성
            string relativePathWithModules = Path.Combine(MODULES_DIR, relativePath);

            string cacheFile = Path.Combine(_cachePath, relativePathWithModules);
            string runtimeFile = Path.Combine(_runtimePath, relativePathWithModules);

            // 서버 경로 구성
            string serverCategoryPath = module.Category ?? "";
            if (!string.IsNullOrEmpty(module.SubSystem))
            {
                serverCategoryPath = $"{serverCategoryPath}/{module.SubSystem}".TrimStart('/');
            }

            string serverRelativePath = string.IsNullOrEmpty(serverCategoryPath)
                ? module.FileName
                : $"{serverCategoryPath}/{module.FileName}";

            string serverUrlPath = $"{MODULES_DIR}/{serverRelativePath}";

            Debug.WriteLine($"[ModuleLoader] 모듈 검사 중: {module.ModuleName} (Hash={version.FileHash})");

            try
            {
                // 1. 런타임(실행) DLL을 최우선으로 검사 (DB 해시와 비교)
                bool isRuntimeValid = File.Exists(runtimeFile) &&
                                      string.Equals(CalculateFileHash(runtimeFile), version.FileHash, StringComparison.OrdinalIgnoreCase);

                if (isRuntimeValid)
                {
                    Debug.WriteLine($"[ModuleLoader] 런타임 파일이 유효함: {runtimeFile}");
                    if (!_loadedModuleVersions.ContainsKey(module.ModuleId))
                    {
                        ReloadModule(runtimeFile, module.ModuleId, version.Version);
                    }
                    return true;
                }

                // 2. 런타임이 유효하지 않으면 다운로드 시도 (캐시는 임시 저장소 역할)
                bool downloaded = false;
                if (_fileTransfer != null)
                {
                    bool isCacheValid = File.Exists(cacheFile) &&
                                        string.Equals(CalculateFileHash(cacheFile), version.FileHash, StringComparison.OrdinalIgnoreCase);

                    if (isCacheValid)
                    {
                        Debug.WriteLine($"[ModuleLoader] 유효한 캐시 파일 발견: {cacheFile}");
                        downloaded = true;
                    }
                    else if (DownloadToCache(serverUrlPath, cacheFile, module.ModuleName, version.Version))
                    {
                        downloaded = true;
                    }
                }

                if (!downloaded)
                {
                    Debug.WriteLine($"[ModuleLoader] !!! {module.ModuleName} 업데이트 다운로드 실패");
                    return false;
                }

                // 3. 최신화된 캐시를 런타임으로 배포 시도
                DeployToRuntime(cacheFile, runtimeFile, module.ModuleName, version.Version);
                ReloadModule(runtimeFile, module.ModuleId, version.Version);

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ModuleLoader] !!! 모듈 {module.ModuleId} 업데이트 중 오류 발생: {ex.Message}");
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

                Debug.WriteLine($"[ModuleLoader] 서버에서 다운로드 시도: {serverUrlPath}");
                if (_fileTransfer.DownloadFile(serverUrlPath, cacheFile))
                {
                    Debug.WriteLine($"[ModuleLoader] 캐시 다운로드 완료: {moduleName} v{version}");
                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ModuleLoader] !!! {moduleName} 다운로드 중 오류 발생: {ex.Message}");
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
                Debug.WriteLine($"[ModuleLoader] 런타임으로 배포 완료: {moduleName} v{version}");
            }
            catch (IOException)
            {
                // 파일 잠금 발생 시 - Shadow Copy를 통해 Hot Deploy 가능
                Debug.WriteLine($"[ModuleLoader] 런타임 파일 잠김: {moduleName}. Hot Deploy를 위해 섀도 복사본을 사용합니다.");
                Debug.WriteLine($"[ModuleLoader] 원본 파일은 다음 재시작 시 업데이트됩니다. 섀도 복사본을 통해 즉시 사용 가능합니다.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ModuleLoader] !!! {moduleName} 배포 중 오류 발생: {ex.Message}");
                throw;
            }
        }

        private void LoadModulesFromRuntime()
        {
            // 실제 구현은 필요에 따라 활성화
            return;

            if (!Directory.Exists(_runtimePath))
            {
                Debug.WriteLine($"[ModuleLoader] 런타임 경로가 존재하지 않음: {_runtimePath}");
                return;
            }

            var dlls = Directory.GetFiles(_runtimePath, "*.dll", SearchOption.AllDirectories);
            Debug.WriteLine($"[ModuleLoader] 런타임 경로에서 {dlls.Length}개의 어셈블리를 발견함.");

            foreach (var dll in dlls)
            {
                LoadAssembly(dll, null, null);
            }

            Debug.WriteLine($"[ModuleLoader] 총 등록된 프로그램 수: {_progRegistry.Count}개.");
        }

        private void LoadAssembly(string dllPath, string moduleId, string version)
        {
            try
            {
                // 기본 컨텍스트에서 LoadFrom 사용 (공유 어셈블리는 DI 호환성이 필요하므로)
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

                        Debug.WriteLine($"[ModuleLoader] 등록됨: {attr.ProgramId} -> {type.FullName} (v{version})");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ModuleLoader] !!! {Path.GetFileName(dllPath)} 로드 중 오류 발생: {ex.Message}");
            }
        }

        private void ReloadModule(string dllPath, string moduleId, string version)
        {
            try
            {
                // 이미 로드된 버전이 있는지 확인합니다.
                if (_loadedModuleVersions.TryGetValue(moduleId, out var currentVersion))
                {
                    // 동일한 버전이면 다시 로드하지 않고 건너뜁니다.
                    if (string.Equals(currentVersion, version, StringComparison.OrdinalIgnoreCase))
                    {
                        Debug.WriteLine($"[ModuleLoader] 모듈 {moduleId} v{version}이 이미 로드되어 있습니다. 기존 버전을 재사용합니다.");
                        return;
                    }

                    // ⚠️ 다른 버전이 감지된 경우 - 버전 불일치 경고 발생
                    Debug.WriteLine($"[ModuleLoader] !!! 버전 충돌 감지 !!!");
                    Debug.WriteLine($"[ModuleLoader] 모듈: {moduleId}");
                    Debug.WriteLine($"[ModuleLoader] 현재 버전: {currentVersion}");
                    Debug.WriteLine($"[ModuleLoader] 요청 버전: {version}");
                    Debug.WriteLine($"[ModuleLoader] 조치: 타입 불일치 방지를 위해 기존(현재) 버전을 유지합니다.");
                    Debug.WriteLine($"[ModuleLoader] 권장 사항: 이 모듈의 모든 인스턴스를 닫고 프로그램을 재시작하십시오.");

                    // 버전 충돌 이벤트 발생
                    RaiseVersionConflict(moduleId, currentVersion, version);

                    return;
                }

                // 최초 로드 시 - Shadow Copy(섀도 복사본) 생성
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
                        Debug.WriteLine($"[ModuleLoader] 로드됨: {attr.ProgramId} -> v{version} (Shadow: {shadowPath})");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ModuleLoader] !!! {Path.GetFileName(dllPath)} 다시 로드 중 오류 발생: {ex.Message}");
            }
        }

        /// <summary>
        /// Shadow Copy(섀도 복사본)를 생성하여 원본 파일 잠금을 방지합니다.
        /// Hot Deploy를 지원하면서도 DI 호환성을 유지할 수 있게 합니다.
        /// </summary>
        private string CreateShadowCopy(string originalPath, string moduleId, string version)
        {
            try
            {
                // 모듈별 섀도 디렉토리 생성
                string moduleShadowDir = Path.Combine(_shadowCopyDirectory, moduleId, version);
                if (!Directory.Exists(moduleShadowDir))
                {
                    Directory.CreateDirectory(moduleShadowDir);
                }

                // 섀도 파일 경로 설정
                string shadowPath = Path.Combine(moduleShadowDir, Path.GetFileName(originalPath));

                // 동일 버전의 파일이 이미 존재하면 재사용
                if (File.Exists(shadowPath))
                {
                    Debug.WriteLine($"[ModuleLoader] 기존 섀도 복사본 사용: {shadowPath}");
                    return shadowPath;
                }

                // 원본 파일을 섀도 위치로 복사
                File.Copy(originalPath, shadowPath, overwrite: true);

                // 같은 디렉토리에 있는 의존성 DLL들도 함께 복사 시도
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
                                // 의존성 복사 실패는 무시 (이미 사용 중이거나 불필요할 수 있음)
                            }
                        }
                    }
                }

                // 섀도 경로 추적 (향후 정리용)
                _shadowCopyPaths[moduleId] = moduleShadowDir;

                Debug.WriteLine($"[ModuleLoader] 섀도 복사본 생성 완료: {originalPath} -> {shadowPath}");
                return shadowPath;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ModuleLoader] !!! 섀도 복사본 생성 실패: {ex.Message}");
                // 실패 시 최후의 수단으로 원본 경로 반환 (재시작 시에만 업데이트 가능하게 됨)
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
                Debug.WriteLine($"[ModuleLoader] !!! {filePath}의 해시 계산 중 오류 발생: {ex.Message}");
                return string.Empty;
            }
        }

        public Type GetProgramType(string progId)
        {
            // 단순히 캐시를 반환하지 않고, DB를 통해 실시간 업데이트 여부를 먼저 확인합니다.
            // Shell이 기동 중인 상태에서도 새로 배포된 DLL(해시 변경)을 즉시 인지하기 위함입니다.

            if (_progRepo == null)
            {
                Debug.WriteLine("[ModuleLoader] !!! 오류: _progRepo가 NULL입니다.");
                return _progRegistry.TryGetValue(progId, out var cachedType) ? cachedType : null;
            }

            var progDto = _progRepo.GetProgramByProgId(progId);
            if (progDto != null && !string.IsNullOrEmpty(progDto.ModuleId))
            {
                // 1. 업데이트 및 로컬 정합성 체크 수행
                // 해시가 다르면 새 DLL을 다운로드하고 ReloadModule을 호출합니다.
                if (EnsureModuleUpdated(progId, progDto.ModuleId))
                {
                    // 2. 체크 완료 후 레지스트리에서 최신 타입을 반환합니다.
                    if (_progRegistry.TryGetValue(progId, out Type type))
                    {
                        return type;
                    }
                    else
                    {
                        // 3. 레지스트리에 없는 경우 특성(Attribute) 기반으로 직접 로드 시도
                        var attr = GetProgramAttribute(progId);
                        if (attr != null) return LoadProgramByAttribute(attr);
                    }
                }
            }
            else
            {
                // DB 매핑 정보가 없는 경우 기존 레지스트리를 확인합니다.
                if (_progRegistry.TryGetValue(progId, out var type)) return type;
                Debug.WriteLine($"[ModuleLoader] !!! 프로그램 '{progId}'를 DB나 레지스트리에서 찾을 수 없습니다.");
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

            var dllPath = Path.Combine(_runtimePath, MODULES_DIR, attr.SystemType, attr.SubSystem ?? string.Empty, $"{attr.DllName}.dll");

            if (!File.Exists(dllPath))
            {
                Debug.WriteLine($"[ModuleLoader] DLL을 찾을 수 없음: {dllPath}");
                return null;
            }

            try
            {
                // 기본 로드 컨텍스트에서 로드 (DI 호환성 유지)
                var assembly = Assembly.LoadFrom(dllPath);
                var type = assembly.GetType(attr.FullClassName);

                if (type != null)
                {
                    _progRegistry[attr.ProgramId] = type;
                    _progAttributeCache[attr.ProgramId] = attr;
                    Debug.WriteLine($"[ModuleLoader] 동적 로드 완료: {attr.ProgramId} (From: {attr.FullClassName})");
                    return type;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ModuleLoader] !!! {attr.FullClassName} 로드 중 오류 발생: {ex.Message}");
            }

            return null;
        }

        public object CreateProgramInstanceWithVersionCheck(string progId, string moduleId)
        {
            if (!EnsureModuleUpdated(progId, moduleId))
            {
                Debug.WriteLine($"[ModuleLoader] !!! ProgId: {progId}에 대한 모듈 업데이트 실패");
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
                    Debug.WriteLine($"[ModuleLoader] !!! {type.FullName}의 인스턴스 생성 중 오류 발생: {ex.Message}");
                }
            }

            return null;
        }

        public object CreateProgramInstance(string progId)
        {
            Debug.WriteLine($"[ModuleLoader] >>> 프로그램 인스턴스 생성 요청됨: {progId}");

            var type = GetProgramType(progId);
            if (type != null)
            {
                try
                {
                    return Activator.CreateInstance(type);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[ModuleLoader] !!! 인스턴스 생성 중 오류 발생: {ex.Message}");
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
        /// 특정 모듈이 현재 사용 중(로드됨)인지 확인합니다.
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
    /// 모듈 버전 충돌 이벤트의 인자 클래스입니다.
    /// </summary>
    public class ModuleVersionConflictEventArgs : EventArgs
    {
        public string ModuleId { get; set; } = string.Empty;
        public string CurrentVersion { get; set; } = string.Empty;
        public string RequestedVersion { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
    }
}
