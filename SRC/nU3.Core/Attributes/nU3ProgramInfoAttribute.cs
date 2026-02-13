using System;
using System.Linq;

namespace nU3.Core.Attributes
{
    /// <summary>
    /// 화면(프로그램) 폼을 식별하기 위한 메타데이터 어트리뷰트입니다.
    /// 모듈 시스템에서 폼을 등록하거나 동적으로 로드할 때 사용됩니다.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class nU3ProgramInfoAttribute : Attribute
    {
        /// <summary>
        /// 프로그램의 표시 이름(예: "외래 환자 등록").
        /// </summary>
        public string ProgramName { get; }

        /// <summary>
        /// 고유한 프로그램 ID(예: "OPD001").
        /// null일 경우 시스템이 생성하거나 클래스명을 사용할 수 있습니다.
        /// </summary>
        public string ProgramId { get; }

        /// <summary>
        /// 이 화면에 접근하기 위한 권한 레벨. 기본값은 1입니다.
        /// </summary>
        public int AuthLevel { get; set; }

        /// <summary>
        /// 이 화면이 현재 사용 가능한지 여부. 기본값은 true입니다.
        /// </summary>
        public bool IsUse { get; set; }

        /// <summary>
        /// 이 화면이 속한 비즈니스 시스템(예: "EMR", "ADM", "NUR").
        /// </summary>
        public string SystemType { get; }

        /// <summary>
        /// 서브 시스템(예: "AD", "IN", "OP").
        /// </summary>
        public string SubSystem { get; }

        /// <summary>
        /// 폼이 구현된 DLL 이름(네임스페이스가 포함된 어셈블리 이름).
        /// 예: "nU3.Modules.EMR.IN.Worklist"
        /// </summary>
        public string DllName { get; }
        
        /// <summary>
        /// 확장자 없이 단순화한 DLL 파일 이름(예: "Worklist").
        /// </summary>
        public string SimpleDllName { get; }
        
        /// <summary>
        /// 타입 해상도를 위한 전체 클래스 이름(네임스페이스 포함).
        /// 예: "nU3.Modules.EMR.IN.Worklist.PatientListControl"
        /// </summary>
        public string FullClassName { get; }
        
        /// <summary>
        /// 네임스페이스를 제외한 단순 클래스 이름(예: "PatientListControl").
        /// </summary>
        public string ClassName => FullClassName?.Split('.').LastOrDefault() ?? string.Empty;
        
        /// <summary>
        /// 폼 유형(예: CHILD, POPUP, SDI). 기본값은 "CHILD"입니다.
        /// </summary>
        public string FormType { get; set; } = "CHILD"; //CHILD, POPUP, SDI

        /// <summary>
        /// 선언 타입과 기본 정보를 이용해 ProgramInfo 어트리뷰트를 초기화합니다.
        /// namespace 및 어셈블리 정보를 분석하여 SystemType, SubSystem 등을 추출합니다.
        /// </summary>
        public nU3ProgramInfoAttribute(Type declaringType, string programName, string programId, string formType = "CHILD")
        {
            this.ProgramName = programName;
            this.ProgramId = programId;
            this.FormType = formType;
            this.AuthLevel = 1;
            this.IsUse = true;
            
            // FullClassName 설정
            this.FullClassName = declaringType.FullName;
            
            // DLL 이름(어셈블리 이름) 설정
            this.DllName = declaringType.Assembly.GetName().Name;
            
            // SimpleDllName 계산 (일반적으로 네임스페이스의 마지막 부분)
            var dllParts = this.DllName.Split('.');
            this.SimpleDllName = dllParts.Length >= 5 ? dllParts[4] : dllParts.LastOrDefault() ?? this.DllName;

            // 네임스페이스 파싱으로 SystemType 및 SubSystem 추출
            var namespaceParts = declaringType.Namespace?.Split('.') ?? Array.Empty<string>();
            
            // 기대 형태: nU3.Modules.{SystemType}.{SubSystem}.{ModuleName}
            if (namespaceParts.Length >= 3 && namespaceParts[0] == "nU3" && namespaceParts[1] == "Modules")
            {
                this.SystemType = namespaceParts[2]; // 예: ADM, EMR, NUR 등
                this.SubSystem = namespaceParts.Length >= 4 ? namespaceParts[3] : null; // 예: AD, IN, OP 등
            }
            else
            {
                // 기본값
                this.SystemType = "UnKnown";
                this.SubSystem = "Un";
            }
        }
        
        /// <summary>
        /// 해당 프로그램이 위치할 것으로 예상되는 DLL 경로를 반환합니다.
        /// 예: "EMR/IN/nU3.Modules.EMR.IN.Worklist.dll"
        /// </summary>
        public string GetExpectedDllPath()
        {
            var parts = new[] { SystemType, SubSystem, $"{DllName}.dll" }
                .Where(p => !string.IsNullOrWhiteSpace(p));
            return string.Join("/", parts);
        }
        
        /// <summary>
        /// 모듈 ID를 생성합니다.
        /// 형식: PROG_{SystemType}_{SubSystem}_{SimpleDllName}
        /// 예: PROG_EMR_IN_Worklist
        /// </summary>
        public string GetModuleId()
        {
            return $"PROG_{SystemType}_{SubSystem}_{SimpleDllName}";
        }
    }
}
