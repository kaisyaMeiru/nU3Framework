namespace nU3.Tools.Deployer.Models
{
    /// <summary>
    /// 배포 도구에서 사용하는 모듈 파일 항목 모델입니다.
    /// 
    /// 이 클래스는 파일 시스템에서 스캔한 모듈(예: DLL) 파일의 메타데이터를 보관합니다.
    /// Deployer UI나 비교 로직에서 사용되며, 파일 경로, 크기, 모듈 식별 정보(시스템/서브시스템/모듈ID 등)를 포함합니다.
    /// </summary>
    public class ModuleFileItem
    {
        /// <summary>
        /// 파일 이름(파일명 및 확장자)
        /// 예: "nU3.Modules.EMR.IN.Worklist.dll"
        /// </summary>
        public string FileName { get; set; } = string.Empty;

        /// <summary>
        /// 파일의 전체 경로(절대 경로)
        /// 예: "C:\\nU3\" 또는 서버 스테이징 폴더 내 경로
        /// </summary>
        public string FullPath { get; set; } = string.Empty;

        /// <summary>
        /// 기준 폴더를 기준으로 한 상대 경로(배포 시의 경로 구조 유지용)
        /// 예: "EMR\\IN\\nU3.Modules.EMR.IN.Worklist.dll"
        /// </summary>
        public string RelativePath { get; set; } = string.Empty;

        /// <summary>
        /// 파일 크기(바이트 단위)
        /// </summary>
        public long SizeBytes { get; set; }

        /// <summary>
        /// 모듈이 속한 시스템 타입(예: "EMR", "ADM", "NUR" 등)
        /// 이 값은 폴더 구조나 메타데이터에서 유추하여 설정됩니다.
        /// </summary>
        public string SystemType { get; set; } = string.Empty;

        /// <summary>
        /// 서브 시스템(예: "IN", "OP" 등)
        /// </summary>
        public string SubSystem { get; set; } = string.Empty;

        /// <summary>
        /// 모듈 식별자(ModuleId). 보통 DB에 등록된 모듈 ID와 매핑됩니다.
        /// </summary>
        public string ModuleId { get; set; } = string.Empty;

        /// <summary>
        /// 모듈의 사람 읽기용 이름(예: Worklist)
        /// </summary>
        public string ModuleName { get; set; } = string.Empty;

        /// <summary>
        /// 파일에 포함된/메타데이터로부터 읽어온 버전 정보(예: "1.2.3")
        /// </summary>
        public string Version { get; set; } = string.Empty;

        /// <summary>
        /// 파일에 포함된 프로그램(폼) 개수 등 부가 정보(스캔 과정에서 집계)
        /// </summary>
        public int ProgramCount { get; set; }

        /// <summary>
        /// 사람에게 보여주기 편한 크기 문자열을 반환합니다.
        /// - 바이트(B), 킬로바이트(KB), 메가바이트(MB), 기가바이트(GB) 단위로 포맷합니다.
        /// - 소수 첫째 자리까지 표시합니다.
        /// </summary>
        public string SizeText
        {
            get
            {
                if (SizeBytes < 1024) return $"{SizeBytes} B";
                if (SizeBytes < 1024 * 1024) return $"{(SizeBytes / 1024d):0.0} KB";
                if (SizeBytes < 1024 * 1024 * 1024) return $"{(SizeBytes / 1024d / 1024d):0.0} MB";
                return $"{(SizeBytes / 1024d / 1024d / 1024d):0.0} GB";
            }
        }
    }
}
