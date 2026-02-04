using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using nU3.Core.Attributes;
using nU3.Core.Repositories;
using nU3.Models;

namespace nU3.Core.Services
{
    /// <summary>
    /// 모듈 로더 서비스는 런타임에 모듈(DLL)을 탐색하고 로드하여 프로그램 인스턴스를 생성하는 책임을 가집니다.
    /// 
    /// 주요 기능:
    /// - 서버 또는 스테이징 영역에서 모듈 파일을 캐시로 다운로드하고 런타임 경로로 배포
    /// - DLL을 로드하여 nU3ProgramInfoAttribute가 붙은 타입들을 등록(ProgId -> Type)
    /// - 모듈 버전 관리를 수행하여 필요한 경우 최신 버전으로 업데이트
    /// - 동적 로딩 및 인스턴스 생성 지원(버전 체크 옵션 포함)
    /// 
    /// 설계 노트:
    /// - 캐시, 스테이징, 런타임 경로를 별도로 관리하여 배포 및 롤백을 단순화합니다.
    /// - 파일 무결성은 SHA256 해시로 검증합니다.
    /// - 실제 서버 다운로드는 현재 스테이징 폴더 복사로 구현되어 있으나 필요 시 원격 전송 로직으로 대체 가능
    /// </summary>
    public class ModuleLoaderService
    {
        private readonly IModuleRepository _moduleRepo;
        private readonly Dictionary<string, Type> _progRegistry;
        private readonly Dictionary<string, nU3ProgramInfoAttribute> _progAttributeCache;
        private readonly Dictionary<string, string> _loadedModuleVersions; // ModuleId -> Version
        private readonly string _cachePath;
        private readonly string _stagingPath;
        private readonly string _runtimePath;

        public ModuleLoaderService(IModuleRepository moduleRepo)
        {
            _moduleRepo = moduleRepo;
            _progRegistry = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);
            _progAttributeCache = new Dictionary<string, nU3ProgramInfoAttribute>(StringComparer.OrdinalIgnoreCase);
            _loadedModuleVersions = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            _runtimePath = Path.Combine(baseDir, "Modules");
            _cachePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), 
                "nU3.Framework", "Cache");
            _stagingPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), 
                "nU3.Framework", "ServerStorage");

            EnsureDirectories();
        }

        private void EnsureDirectories()
        {
            if (!Directory.Exists(_runtimePath)) Directory.CreateDirectory(_runtimePath);
            if (!Directory.Exists(_cachePath)) Directory.CreateDirectory(_cachePath);
            if (!Directory.Exists(_stagingPath)) Directory.CreateDirectory(_stagingPath);
        }

        public Dictionary<string, Type> GetProgramRegistry() => _progRegistry;
        
        /// <summary>
        /// DB에서 읽어온 nU3ProgramInfoAttribute 캐시를 반환합니다.
        /// </summary>
        public Dictionary<string, nU3ProgramInfoAttribute> GetProgramAttributes() => _progAttributeCache;

        /// <summary>
        /// 캐시 및 런타임의 모듈을 확인하고 필요한 경우 업데이트 및 로드를 수행합니다.
        /// </summary>
        public void LoadAllModules()
        {
            CheckAndUpdateModules();
            LoadModulesFromRuntime();
        }

        /// <summary>
        /// DB(또는 스테이징)에 있는 모듈 파일과 캐시/런타임 파일을 비교하여
        /// 변경사항이 있으면 캐시로 다운로드하고 런타임에 배포합니다.
        /// </summary>
        public void CheckAndUpdateModules()
        {
            var modules = _moduleRepo.GetAllModules();
            var activeVersions = _moduleRepo.GetActiveVersions();
            int updateCount = 0;

            foreach (var module in modules)
            {
                var activeVersion = activeVersions.FirstOrDefault(v => 
                    string.Equals(v.ModuleId, module.ModuleId, StringComparison.OrdinalIgnoreCase));
                
                if (activeVersion == null) continue;

                string relativePath = Path.Combine(
                    module.Category ?? "Common", 
                    module.SubSystem ?? "Common", 
                    module.FileName);

                string cacheFile = Path.Combine(_cachePath, relativePath);
                string runtimeFile = Path.Combine(_runtimePath, relativePath);
                string serverFile = Path.Combine(_stagingPath, relativePath);

                bool needsUpdate = false;

                // 1. 캐시에 서버 파일이 없으면 또는 해시가 다르면 서버에서 캐시로 복사
                if (!File.Exists(cacheFile) && File.Exists(serverFile))
                {
                    DownloadToCache(serverFile, cacheFile, module.ModuleName, activeVersion.Version);
                    needsUpdate = true;
                }
                else if (File.Exists(cacheFile) && File.Exists(serverFile))
                {
                    string cacheHash = CalculateFileHash(cacheFile);
                    string serverHash = CalculateFileHash(serverFile);
                    
                    if (cacheHash != serverHash)
                    {
                        DownloadToCache(serverFile, cacheFile, module.ModuleName, activeVersion.Version);
                        needsUpdate = true;
                    }
                }

                // 2. 캐시가 존재하면 런타임에 배포 필요 여부 검사
                if (File.Exists(cacheFile))
                {
                    if (!File.Exists(runtimeFile))
                    {
                        needsUpdate = true;
                    }
                    else
                    {
                        string cacheHash = CalculateFileHash(cacheFile);
                        string runtimeHash = CalculateFileHash(runtimeFile);
                        
                        if (cacheHash != runtimeHash)
                        {
                            needsUpdate = true;
                        }
                    }

                    if (needsUpdate)
                    {
                        DeployToRuntime(cacheFile, runtimeFile, module.ModuleName, activeVersion.Version);
                        _loadedModuleVersions[module.ModuleId] = activeVersion.Version;
                        updateCount++;
                    }
                }
            }

            if (updateCount > 0)
            {
                Console.WriteLine($"[ModuleLoader] {updateCount} modules updated.");
            }
        }

        /// <summary>
        /// 특정 ProgId/ModuleId에 대해 DB의 활성 버전과 로드된 버전을 비교하여
        /// 필요하면 업데이트 수행 후 true를 반환합니다.
        /// </summary>
        public bool EnsureModuleUpdated(string progId, string moduleId)
        {
            // 1. DB에서 활성 버전 정보 조회
            var activeVersion = _moduleRepo.GetActiveVersions()
                .FirstOrDefault(v => string.Equals(v.ModuleId, moduleId, StringComparison.OrdinalIgnoreCase));
            
            if (activeVersion == null)
            {
                Console.WriteLine($"[ModuleLoader] No active version found for module: {moduleId}");
                return false;
            }

            // 2. 현재 로드된 버전과 비교
            if (_loadedModuleVersions.TryGetValue(moduleId, out var loadedVersion))
            {
                if (string.Equals(loadedVersion, activeVersion.Version, StringComparison.OrdinalIgnoreCase))
                {
                    // 이미 최신 버전
                    return true;
                }
            }

            // 3. 업데이트 필요
            var module = _moduleRepo.GetAllModules()
                .FirstOrDefault(m => string.Equals(m.ModuleId, moduleId, StringComparison.OrdinalIgnoreCase));
            
            if (module == null) return false;

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
            string serverFile = Path.Combine(_stagingPath, relativePath);

            try
            {
                // 캐시에서 다운로드 필요 여부 판단
                if (!File.Exists(cacheFile) || NeedsDownload(cacheFile, serverFile, version))
                {
                    if (File.Exists(serverFile))
                    {
                        DownloadToCache(serverFile, cacheFile, module.ModuleName, version.Version);
                    }
                    else
                    {
                        Console.WriteLine($"[ModuleLoader] Server file not found: {serverFile}");
                        return false;
                    }
                }

                // 캐시에서 런타임으로 배포
                if (File.Exists(cacheFile))
                {
                    DeployToRuntime(cacheFile, runtimeFile, module.ModuleName, version.Version);
                    
                    // 모듈 다시 로드
                    ReloadModule(runtimeFile, module.ModuleId, version.Version);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ModuleLoader] Error updating module {module.ModuleId}: {ex.Message}");
            }

            return false;
        }

        private bool NeedsDownload(string cacheFile, string serverFile, ModuleVerDto version)
        {
            if (!File.Exists(serverFile)) return false;
            
            // 파일 해시 비교
            string cacheHash = CalculateFileHash(cacheFile);
            return !string.Equals(cacheHash, version.FileHash, StringComparison.OrdinalIgnoreCase);
        }

        private void DownloadToCache(string serverFile, string cacheFile, string moduleName, string version)
        {
            try
            {
                string cacheDir = Path.GetDirectoryName(cacheFile);
                if (!Directory.Exists(cacheDir)) 
                    Directory.CreateDirectory(cacheDir);

                File.Copy(serverFile, cacheFile, true);
                Console.WriteLine($"[ModuleLoader] Downloaded to cache: {moduleName} v{version}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ModuleLoader] Error downloading {moduleName}: {ex.Message}");
            }
        }

        private void DeployToRuntime(string cacheFile, string runtimeFile, string moduleName, string version)
        {
            try
            {
                string runtimeDir = Path.GetDirectoryName(runtimeFile);
                if (!Directory.Exists(runtimeDir)) 
                    Directory.CreateDirectory(runtimeDir);

                File.Copy(cacheFile, runtimeFile, true);
                Console.WriteLine($"[ModuleLoader] Deployed to runtime: {moduleName} v{version}");
            }
            catch (IOException ex)
            {
                Console.WriteLine($"[ModuleLoader] Module {moduleName} is locked (in use). Update will apply on restart.");
                throw; // Re-throw to handle at higher level
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ModuleLoader] Error deploying {moduleName}: {ex.Message}");
                throw;
            }
        }

        private void LoadModulesFromRuntime()
        {
            if (!Directory.Exists(_runtimePath)) 
                return;

            var dlls = Directory.GetFiles(_runtimePath, "*.dll", SearchOption.AllDirectories);
            
            foreach (var dll in dlls)
            {
                LoadAssembly(dll, null, null);
            }

            Console.WriteLine($"[ModuleLoader] Loaded {_progRegistry.Count} programs from {dlls.Length} assemblies.");
        }

        private void LoadAssembly(string dllPath, string moduleId, string version)
        {
            try
            {
                var assembly = Assembly.LoadFile(dllPath);
                
                // 제공되지 않은 경우 어셈블리 버전 가져오기
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
                        
                        // 모듈 버전 추적
                        if (!string.IsNullOrEmpty(moduleId))
                        {
                            _loadedModuleVersions[moduleId] = version;
                        }
                        else
                        {
                            // 특성에서 moduleId 생성
                            var autoModuleId = attr.GetModuleId();
                            _loadedModuleVersions[autoModuleId] = version;
                        }
                        
                        Console.WriteLine($"[ModuleLoader] Registered: {attr.ProgramId} -> {type.FullName} (v{version})");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ModuleLoader] Error loading {Path.GetFileName(dllPath)}: {ex.Message}");
            }
        }

        private void ReloadModule(string dllPath, string moduleId, string version)
        {
            try
            {
                var assembly = Assembly.LoadFile(dllPath);
                
                foreach (var type in assembly.GetTypes())
                {
                    var attr = type.GetCustomAttribute<nU3ProgramInfoAttribute>();
                    if (attr != null)
                    {
                        _progRegistry[attr.ProgramId] = type;
                        _progAttributeCache[attr.ProgramId] = attr;
                        _loadedModuleVersions[moduleId] = version;
                        Console.WriteLine($"[ModuleLoader] Reloaded: {attr.ProgramId} (v{version})");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ModuleLoader] Error reloading {Path.GetFileName(dllPath)}: {ex.Message}");
            }
        }

        private string CalculateFileHash(string filePath)
        {
            using (var sha256 = SHA256.Create())
            using (var stream = File.OpenRead(filePath))
            {
                var hash = sha256.ComputeHash(stream);
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
        }

        /// <summary>
        /// ProgId에 해당하는 Type을 반환합니다. 캐시에 없으면 null을 반환합니다.
        /// </summary>
        public Type GetProgramType(string progId)
        {
            if (_progRegistry.TryGetValue(progId, out Type type))
                return type;
            
            return null;
        }
        
        /// <summary>
        /// ProgId에 해당하는 nU3ProgramInfoAttribute를 반환합니다.
        /// DB에서 조회한 속성 캐시를 확인합니다.
        /// </summary>
        public nU3ProgramInfoAttribute GetProgramAttribute(string progId)
        {
            if (_progAttributeCache.TryGetValue(progId, out var attr))
                return attr;
            
            return null;
        }
        
        /// <summary>
        /// nU3ProgramInfoAttribute로부터 DLL을 찾아 타입을 동적으로 로드하고 반환합니다.
        /// 동적 로드 시 로드된 타입을 캐시에 저장합니다.
        /// </summary>
        public Type LoadProgramByAttribute(nU3ProgramInfoAttribute attr)
        {
            // Check cache first
            if (_progRegistry.TryGetValue(attr.ProgramId, out var cachedType))
                return cachedType;
            
            // Construct dll path in runtime folder
            var dllPath = Path.Combine(_runtimePath, attr.SystemType, attr.SubSystem ?? string.Empty, $"{attr.DllName}.dll");
            
            if (!File.Exists(dllPath))
            {
                Console.WriteLine($"[ModuleLoader] DLL not found: {dllPath}");
                return null;
            }
            
            try
            {
                var assembly = Assembly.LoadFile(dllPath);
                var type = assembly.GetType(attr.FullClassName);
                
                if (type != null)
                {
                    _progRegistry[attr.ProgramId] = type;
                    _progAttributeCache[attr.ProgramId] = attr;
                    Console.WriteLine($"[ModuleLoader] Dynamically loaded: {attr.ProgramId} from {attr.FullClassName}");
                    return type;
                }
                else
                {
                    Console.WriteLine($"[ModuleLoader] Type not found: {attr.FullClassName} in {dllPath}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ModuleLoader] Error loading {attr.FullClassName}: {ex.Message}");
            }
            
            return null;
        }
        
        /// <summary>
        /// ProgId, ModuleId, Version 체크 후 프로그램 인스턴스를 생성합니다.
        /// 필요 시 모듈을 업데이트하고 인스턴스를 생성합니다.
        /// </summary>
        public object CreateProgramInstanceWithVersionCheck(string progId, string moduleId)
        {
            // 1. 모듈 업데이트 확인
            if (!EnsureModuleUpdated(progId, moduleId))
            {
                Console.WriteLine($"[ModuleLoader] Failed to update module for ProgId: {progId}");
                return null;
            }

            // 2. 타입 조회
            var type = GetProgramType(progId);
            
            if (type == null)
            {
                // 동적 로드 시도
                var attr = GetProgramAttribute(progId);
                if (attr != null)
                {
                    type = LoadProgramByAttribute(attr);
                }
            }
            
            // 3. 인스턴스 생성
            if (type != null)
            {
                try
                {
                    var instance = Activator.CreateInstance(type);
                    Console.WriteLine($"[ModuleLoader] Created instance: {progId} (Module: {moduleId})");
                    return instance;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ModuleLoader] Error creating instance of {type.FullName}: {ex.Message}");
                }
            }
            
            return null;
        }
        
        /// <summary>
        /// ProgId로 등록된 프로그램 인스턴스를 생성합니다. (버전 체크 미실행)
        /// </summary>
        public object CreateProgramInstance(string progId)
        {
            var type = GetProgramType(progId);
            
            if (type == null)
            {
                // 캐시된 속성 정보로 동적 로드 시도
                var attr = GetProgramAttribute(progId);
                if (attr != null)
                {
                    type = LoadProgramByAttribute(attr);
                }
            }
            
            if (type != null)
            {
                try
                {
                    return Activator.CreateInstance(type);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ModuleLoader] Error creating instance of {type.FullName}: {ex.Message}");
                }
            }
            
            return null;
        }
        
        /// <summary>
        /// 로드된 모듈의 버전을 반환합니다. 없으면 null
        /// </summary>
        public string GetLoadedModuleVersion(string moduleId)
        {
            return _loadedModuleVersions.TryGetValue(moduleId, out var version) ? version : null;
        }
    }
}
