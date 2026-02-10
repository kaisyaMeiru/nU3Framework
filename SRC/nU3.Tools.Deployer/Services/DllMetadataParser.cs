using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using nU3.Core.Attributes;
using nU3.Models;

namespace nU3.Tools.Deployer.Services
{
    /// <summary>
    /// DLL(모듈) 메타데이터를 분석하고 검증하는 유틸리티 클래스입니다.
    /// </summary>
    public class DllMetadataParser
    {
        // 규칙: nU3.Modules.{System}.{SubSys}.{Name}.dll
        private static readonly Regex NamingPattern = new Regex(@"^nU3\.Modules\.([^\.]+)\.([^\.]+)\.(.+)\.dll$", RegexOptions.IgnoreCase);

        /// <summary>
        /// 주어진 DLL 경로를 파싱하여 ParsedModuleInfo를 반환합니다.
        /// MetadataLoadContext를 사용하여 메타데이터만 읽고 즉시 언로드합니다.
        /// </summary>
        public ParsedModuleInfo Parse(string dllPath)
        {
            var fileInfo = new FileInfo(dllPath);
            var result = new ParsedModuleInfo
            {
                FullPath = dllPath,
                FileName = fileInfo.Name,
                FileSize = fileInfo.Length,
                ValidationErrors = new List<string>()
            };

            // 1. 네이밍 규칙 검사
            var match = NamingPattern.Match(result.FileName);
            if (match.Success)
            {
                result.SystemType = match.Groups[1].Value.ToUpper();
                result.SubSystem = match.Groups[2].Value.ToUpper();
                result.ModuleName = match.Groups[3].Value;
                result.ModuleId = $"PROG_{result.SystemType}_{result.SubSystem}_{result.ModuleName}";
            }
            else
            {
                throw new ArgumentException("파일 이름이 규칙에 맞지 않습니다: nU3.Modules.{System}.{SubSys}.{Name}.dll");
            }

            // 2. MetadataLoadContext를 사용하여 메타데이터만 로드 (실행 불가, 메모리 격리)
            try
            {
                // 런타임 어셈블리 경로 수집 (CoreLib 등)
                var runtimeAssemblies = Directory.GetFiles(RuntimeEnvironment.GetRuntimeDirectory(), "*.dll");
                var paths = new List<string>(runtimeAssemblies);
                
                // 대상 DLL 추가
                paths.Add(dllPath);

                // 의존성 어셈블리 추가 (DLL과 같은 폴더의 모든 DLL)
                var dllDir = Path.GetDirectoryName(dllPath);
                if (!string.IsNullOrEmpty(dllDir))
                {
                    paths.AddRange(Directory.GetFiles(dllDir, "*.dll"));
                    
                    // 상위 디렉토리도 검색 (프레임워크 DLL이 상위에 있을 수 있음)
                    var parentDir = Directory.GetParent(dllDir);
                    if (parentDir != null && parentDir.Exists)
                    {
                        paths.AddRange(Directory.GetFiles(parentDir.FullName, "*.dll", SearchOption.TopDirectoryOnly));
                    }
                }
                
                // 현재 실행 중인 어셈블리들의 위치도 추가
                var executingDir = AppDomain.CurrentDomain.BaseDirectory;
                if (!string.IsNullOrEmpty(executingDir) && Directory.Exists(executingDir))
                {
                    paths.AddRange(Directory.GetFiles(executingDir, "*.dll", SearchOption.TopDirectoryOnly));
                }
                
                // 중복 제거
                paths = paths.Distinct(StringComparer.OrdinalIgnoreCase).ToList();

                var resolver = new PathAssemblyResolver(paths);
                using var mlc = new MetadataLoadContext(resolver);

                var assembly = mlc.LoadFromAssemblyPath(dllPath);
                var version = assembly.GetName().Version;
                result.Version = version != null ? version.ToString() : "1.0.0.0";
                result.AssemblyName = assembly.GetName().Name;

                // 3. 어셈블리 내 타입을 순회하며 화면 정보 추출
                foreach (var type in assembly.GetTypes())
                {
                    // MetadataLoadContext에서는 GetCustomAttribute<T>() 대신 GetCustomAttributesData() 사용
                    var attrData = type.GetCustomAttributesData()
                        .FirstOrDefault(a => a.AttributeType.FullName == typeof(nU3ProgramInfoAttribute).FullName);

                    if (attrData != null)
                    {
                        // 속성 인자 추출 (생성자 파라미터)
                        var ctorArgs = attrData.ConstructorArguments;
                        string progId = ctorArgs.Count > 0 ? ctorArgs[0].Value?.ToString() : null;
                        string progName = ctorArgs.Count > 1 ? ctorArgs[1].Value?.ToString() : null;

                        // Named 속성 추출
                        var namedArgs = attrData.NamedArguments.ToDictionary(
                            na => na.MemberName,
                            na => na.TypedValue.Value?.ToString()
                        );

                        namedArgs.TryGetValue("AuthLevel", out var authLevelStr);
                        namedArgs.TryGetValue("SystemType", out var systemType);
                        namedArgs.TryGetValue("SubSystem", out var subSystem);
                        namedArgs.TryGetValue("DllName", out var dllName);
                        namedArgs.TryGetValue("FullClassName", out var fullClassName);

                        var validationErrors = ValidateProgramInfo(type, systemType, subSystem, result);

                        var programDto = new ProgramDto
                        {
                            ProgId = progId ?? type.Name,
                            ProgName = progName,
                            ClassName = fullClassName ?? type.FullName,
                            AuthLevel = int.TryParse(authLevelStr, out var authLevel) ? authLevel : 0,
                            ModuleId = result.ModuleId,
                            SystemType = systemType,
                            SubSystem = subSystem,
                            DllName = dllName
                        };

                        if (validationErrors.Count > 0)
                        {
                            result.ValidationErrors.AddRange(validationErrors.Select(e =>
                                $"[{type.Name}] {e}"));
                        }

                        result.Programs.Add(programDto);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"어셈블리 메타데이터 검사 실패: {ex.Message}", ex);
            }

            return result;
        }

        private List<string> ValidateProgramInfo(Type type, string attrSystemType, string attrSubSystem, ParsedModuleInfo moduleInfo)
        {
            var errors = new List<string>();

            // 1. 네임스페이스 검증
            var namespaceParts = type.Namespace?.Split('.') ?? Array.Empty<string>();
            string nsSystemType = null;
            string nsSubSystem = null;

            if (namespaceParts.Length >= 3 && namespaceParts[0] == "nU3" && namespaceParts[1] == "Modules")
            {
                nsSystemType = namespaceParts[2];
                nsSubSystem = namespaceParts.Length >= 4 ? namespaceParts[3] : null;
            }

            var dllSystemType = moduleInfo.SystemType;
            var dllSubSystem = moduleInfo.SubSystem;

            if (!string.Equals(nsSystemType, dllSystemType, StringComparison.OrdinalIgnoreCase))
            {
                errors.Add($"SystemType 불일치: Namespace='{nsSystemType}' vs DLL='{dllSystemType}'");
            }

            if (!string.Equals(nsSystemType, attrSystemType, StringComparison.OrdinalIgnoreCase))
            {
                errors.Add($"SystemType 불일치: Namespace='{nsSystemType}' vs Attribute='{attrSystemType}'");
            }

            if (!string.Equals(dllSystemType, attrSystemType, StringComparison.OrdinalIgnoreCase))
            {
                errors.Add($"SystemType 불일치: DLL='{dllSystemType}' vs Attribute='{attrSystemType}'");
            }

            if (!string.Equals(nsSubSystem, dllSubSystem, StringComparison.OrdinalIgnoreCase))
            {
                errors.Add($"SubSystem 불일치: Namespace='{nsSubSystem}' vs DLL='{dllSubSystem}'");
            }

            if (!string.Equals(nsSubSystem, attrSubSystem, StringComparison.OrdinalIgnoreCase))
            {
                errors.Add($"SubSystem 불일치: Namespace='{nsSubSystem}' vs Attribute='{attrSubSystem}'");
            }

            if (!string.Equals(dllSubSystem, attrSubSystem, StringComparison.OrdinalIgnoreCase))
            {
                errors.Add($"SubSystem 불일치: DLL='{dllSubSystem}' vs Attribute='{attrSubSystem}'");
            }

            return errors;
        }
    }

    public class ParsedModuleInfo
    {
        public string FullPath { get; set; }
        public string FileName { get; set; }
        public string AssemblyName { get; set; }
        public string ModuleId { get; set; }
        public string ModuleName { get; set; }
        public string SystemType { get; set; } 
        public string SubSystem { get; set; }
        public string Version { get; set; }
        public long FileSize { get; set; }
        public List<ProgramDto> Programs { get; set; } = new List<ProgramDto>();
        public List<string> ValidationErrors { get; set; } = new List<string>();
        
        public bool HasValidationErrors => ValidationErrors.Count > 0;
        
        public string GetValidationSummary()
        {
            if (!HasValidationErrors) return "검증 성공";
            
            var sb = new StringBuilder();
            sb.AppendLine($"{ValidationErrors.Count}개의 검증 오류:");
            foreach (var error in ValidationErrors)
            {
                sb.AppendLine($"  - {error}");
            }
            return sb.ToString();
        }
    }
}