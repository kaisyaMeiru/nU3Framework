using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using nU3.Core.Attributes;
using nU3.Models;

namespace nU3.Tools.Deployer.Services
{
    /// <summary>
    /// DLL(모듈) 파일의 메타데이터를 추출하고 검증하는 유틸리티 클래스입니다.
    /// 
    /// 주요 역할:
    /// - 파일명 규칙(nU3.Modules.{System}.{SubSys}.{Name}.dll) 검사
    /// - 어셈블리 로드 및 버전, 어셈블리명 추출
    /// - 타입을 스캔하여 nU3ProgramInfoAttribute가 붙은 화면(프로그램)을 식별하고 DTO로 변환
    /// - 네임스페이스 / DLL명 / 어트리뷰트 간의 일관성 검증
    /// </summary>
    public class DllMetadataParser
    {
        // 규약: nU3.Modules.{System}.{SubSys}.{Name}.dll
        // 예: nU3.Modules.EMR.IN.Worklist.dll
        private static readonly Regex NamingPattern = new Regex(@"^nU3\.Modules\.([^\.]+)\.([^\.]+)\.(.+)\.dll$", RegexOptions.IgnoreCase);

        /// <summary>
        /// 지정한 DLL 경로를 파싱하여 ParsedModuleInfo를 반환합니다.
        /// 반환된 정보에는 파일 메타데이터, 포함된 프로그램(프로그램 DTO) 목록, 검증 오류 목록이 포함됩니다.
        /// </summary>
        /// <param name="dllPath">검사할 DLL 파일의 전체 경로</param>
        /// <returns>파싱 및 검증 결과를 포함한 ParsedModuleInfo 객체</returns>
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

            // 1. 네이밍 규약 검사
            var match = NamingPattern.Match(result.FileName);
            if (match.Success)
            {
                result.SystemType = match.Groups[1].Value.ToUpper(); // EMR
                result.SubSystem = match.Groups[2].Value.ToUpper();  // IN
                result.ModuleName = match.Groups[3].Value;           // Worklist

                // ModuleId 규칙: PROG_{SystemType}_{SubSystem}_{SimpleDllName}
                result.ModuleId = $"PROG_{result.SystemType}_{result.SubSystem}_{result.ModuleName}";
            }
            else
            {
                // 파일명이 규약에 맞지 않으면 예외를 던집니다.
                throw new ArgumentException("파일 이름이 규약을 따르지 않습니다: nU3.Modules.{System}.{SubSys}.{Name}.dll");
            }

            // 2. 어셈블리 정보 (리플렉션)
            try
            {
                // Assembly.LoadFrom을 사용하여 어셈블리를 로드하고 검사합니다.
                var assembly = Assembly.LoadFrom(dllPath);
                var version = assembly.GetName().Version;
                result.Version = version != null ? version.ToString() : "1.0.0.0";
                result.AssemblyName = assembly.GetName().Name;

                // 3. 어셈블리 내 타입을 순회하면서 화면(프로그램) 어트리뷰트 스캔
                foreach (var type in assembly.GetTypes())
                {
                    var attr = type.GetCustomAttribute<nU3ProgramInfoAttribute>();
                    if (attr != null)
                    {
                        // 어트리뷰트와 네임스페이스/파일명 간의 일관성 검증 수행
                        var validationErrors = ValidateProgramInfo(type, attr, result);

                        var programDto = new ProgramDto
                        {
                            ProgId = attr.ProgramId ?? type.Name,
                            ProgName = attr.ProgramName,
                            ClassName = attr.FullClassName,
                            AuthLevel = attr.AuthLevel,
                            ModuleId = result.ModuleId,
                            SystemType = attr.SystemType,
                            SubSystem = attr.SubSystem,
                            DllName = attr.DllName
                        };

                        if (validationErrors.Count > 0)
                        {
                            // 타입 단위 검증 오류를 결과에 추가 (타입명과 함께)
                            result.ValidationErrors.AddRange(validationErrors.Select(e =>
                                $"[{type.Name}] {e}"));
                        }

                        result.Programs.Add(programDto);
                    }
                }
            }
            catch (Exception ex)
            {
                // 어셈블리 검사 실패시 의미있는 메시지로 래핑하여 예외 던짐
                throw new Exception($"어셈블리 검사 실패: {ex.Message}");
            }

            return result;
        }

        /// <summary>
        /// 타입의 네임스페이스, DLL에서 유추한 시스템/서브시스템, 그리고 어트리뷰트의 값들이 서로 일치하는지 검증합니다.
        /// 불일치 항목은 문자열 메시지로 수집되어 반환됩니다.
        /// </summary>
        private List<string> ValidateProgramInfo(Type type, nU3ProgramInfoAttribute attr, ParsedModuleInfo moduleInfo)
        {
            var errors = new List<string>();

            // 1. 네임스페이스 검증: nU3.Modules.{SystemType}.{SubSystem}.{ModuleName}
            var namespaceParts = type.Namespace?.Split('.') ?? Array.Empty<string>();
            string nsSystemType = null;
            string nsSubSystem = null;

            if (namespaceParts.Length >= 3 && namespaceParts[0] == "nU3" && namespaceParts[1] == "Modules")
            {
                nsSystemType = namespaceParts[2];
                nsSubSystem = namespaceParts.Length >= 4 ? namespaceParts[3] : null;
            }

            // 2. DLL 이름으로부터 추출된 시스템/서브시스템
            var dllSystemType = moduleInfo.SystemType;
            var dllSubSystem = moduleInfo.SubSystem;

            // 3. 어트리뷰트에 설정된 시스템/서브시스템
            var attrSystemType = attr.SystemType;
            var attrSubSystem = attr.SubSystem;

            // SystemType 일관성 검증
            if (!string.Equals(nsSystemType, dllSystemType, StringComparison.OrdinalIgnoreCase))
            {
                errors.Add($"SystemType 불일치: 네임스페이스='{nsSystemType}' vs DLL='{dllSystemType}'");
            }

            if (!string.Equals(nsSystemType, attrSystemType, StringComparison.OrdinalIgnoreCase))
            {
                errors.Add($"SystemType 불일치: 네임스페이스='{nsSystemType}' vs 어트리뷰트='{attrSystemType}'");
            }

            if (!string.Equals(dllSystemType, attrSystemType, StringComparison.OrdinalIgnoreCase))
            {
                errors.Add($"SystemType 불일치: DLL='{dllSystemType}' vs 어트리뷰트='{attrSystemType}'");
            }

            // SubSystem 일관성 검증
            if (!string.Equals(nsSubSystem, dllSubSystem, StringComparison.OrdinalIgnoreCase))
            {
                errors.Add($"SubSystem 불일치: 네임스페이스='{nsSubSystem}' vs DLL='{dllSubSystem}'");
            }

            if (!string.Equals(nsSubSystem, attrSubSystem, StringComparison.OrdinalIgnoreCase))
            {
                errors.Add($"SubSystem 불일치: 네임스페이스='{nsSubSystem}' vs 어트리뷰트='{attrSubSystem}'");
            }

            if (!string.Equals(dllSubSystem, attrSubSystem, StringComparison.OrdinalIgnoreCase))
            {
                errors.Add($"SubSystem 불일치: DLL='{dllSubSystem}' vs 어트리뷰트='{attrSubSystem}'");
            }

            return errors;
        }
    }

    /// <summary>
    /// DLL 파싱 결과를 담는 모델 클래스입니다.
    /// </summary>
    public class ParsedModuleInfo
    {
        public string FullPath { get; set; }
        public string FileName { get; set; }
        public string AssemblyName { get; set; }
        public string ModuleId { get; set; }
        public string ModuleName { get; set; }
        public string SystemType { get; set; } // Category
        public string SubSystem { get; set; }
        public string Version { get; set; }
        public long FileSize { get; set; }
        public List<ProgramDto> Programs { get; set; } = new List<ProgramDto>();
        public List<string> ValidationErrors { get; set; } = new List<string>();
        
        public bool HasValidationErrors => ValidationErrors.Count > 0;
        
        public string GetValidationSummary()
        {
            if (!HasValidationErrors) return "모든 검증 통과";
            
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
