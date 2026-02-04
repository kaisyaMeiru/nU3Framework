using System;

namespace nU3.Models
{
    public class ModuleMstDto
    {
        public string ModuleId { get; set; }
        public string Category { get; set; }
        public string SubSystem { get; set; }
        public string ModuleName { get; set; }
        public string FileName { get; set; }
        public DateTime RegDate { get; set; }
    }

    public class ModuleVerDto
    {
        public string ModuleId { get; set; }
        public string Version { get; set; }
        public string FileHash { get; set; }
        public long FileSize { get; set; }
        public string StoragePath { get; set; }
        public string DeployDesc { get; set; }
        public DateTime? DelDate { get; set; }
        public string Category { get; set; } // Joined from MST
    }

    /// <summary>
    /// Framework Component Types
    /// </summary>
    public enum ComponentType
    {
        /// <summary>화면 모듈 DLL (기존 방식)</summary>
        ScreenModule = 0,
        
        /// <summary>Framework 핵심 DLL (nU3.Core.dll 등)</summary>
        FrameworkCore = 1,
        
        /// <summary>공용 라이브러리 DLL (DevExpress, Oracle.ManagedDataAccess 등)</summary>
        SharedLibrary = 2,
        
        /// <summary>실행 파일 (nU3.Shell.exe, nU3.Bootstrapper.exe 등)</summary>
        Executable = 3,
        
        /// <summary>설정 파일 (appsettings.json 등)</summary>
        Configuration = 4,
        
        /// <summary>리소스 파일 (이미지, 아이콘 등)</summary>
        Resource = 5,
        
        /// <summary>플러그인 DLL</summary>
        Plugin = 6,

        /// <summary>json</summary>
        Json = 7,

        /// <summary>xml</summary>
        Xml = 8,

        /// <summary>json</summary>
        Image = 9,

        /// <summary>기타</summary>
        Other = 99
    }

    /// <summary>
    /// Framework Component Master DTO
    /// 화면 모듈이 아닌 Framework DLL, 공용 라이브러리, 실행파일 등을 관리
    /// </summary>
    public class ComponentMstDto
    {
        /// <summary>컴포넌트 고유 ID (예: "nU3.Core", "DevExpress.XtraEditors", "nU3.Shell")</summary>
        public string ComponentId { get; set; }
        
        /// <summary>컴포넌트 유형</summary>
        public ComponentType ComponentType { get; set; }
        
        /// <summary>컴포넌트 이름 (표시용)</summary>
        public string ComponentName { get; set; }
        
        /// <summary>파일명 (예: "nU3.Core.dll", "nU3.Shell.exe")</summary>
        public string FileName { get; set; }
        
        /// <summary>설치 경로 (상대 경로, 예: "", "plugins", "resources/images")</summary>
        public string InstallPath { get; set; }
        
        /// <summary>그룹/카테고리 (예: "Framework", "DevExpress", "Oracle")</summary>
        public string GroupName { get; set; }
        
        /// <summary>필수 여부 (필수 컴포넌트는 항상 최신 버전 유지)</summary>
        public bool IsRequired { get; set; }
        
        /// <summary>자동 업데이트 여부</summary>
        public bool AutoUpdate { get; set; } = true;
        
        /// <summary>설명</summary>
        public string Description { get; set; }
        
        /// <summary>우선순위 (낮을수록 먼저 설치, 의존성 순서)</summary>
        public int Priority { get; set; }
        
        /// <summary>의존 컴포넌트 ID 목록 (쉼표 구분)</summary>
        public string Dependencies { get; set; }
        
        /// <summary>등록일</summary>
        public DateTime RegDate { get; set; }
        
        /// <summary>수정일</summary>
        public DateTime? ModDate { get; set; }
        
        /// <summary>활성 여부</summary>
        public string IsActive { get; set; } = "Y";
    }

    /// <summary>
    /// Framework Component Version DTO
    /// </summary>
    public class ComponentVerDto
    {
        /// <summary>컴포넌트 ID</summary>
        public string ComponentId { get; set; }
        
        /// <summary>버전 (예: "1.0.0.0", "23.2.9")</summary>
        public string Version { get; set; }
        
        /// <summary>파일 해시 (SHA256)</summary>
        public string FileHash { get; set; }
        
        /// <summary>파일 크기 (bytes)</summary>
        public long FileSize { get; set; }
        
        /// <summary>서버 저장 경로 (절대 경로)</summary>
        public string StoragePath { get; set; }
        
        /// <summary>최소 지원 Framework 버전</summary>
        public string MinFrameworkVersion { get; set; }
        
        /// <summary>최대 지원 Framework 버전 (null = 제한 없음)</summary>
        public string MaxFrameworkVersion { get; set; }
        
        /// <summary>배포 설명</summary>
        public string DeployDesc { get; set; }
        
        /// <summary>릴리즈 노트 URL</summary>
        public string ReleaseNoteUrl { get; set; }
        
        /// <summary>등록일</summary>
        public DateTime RegDate { get; set; }
        
        /// <summary>삭제일 (soft delete)</summary>
        public DateTime? DelDate { get; set; }
        
        /// <summary>활성 버전 여부 (배포 대상)</summary>
        public string IsActive { get; set; } = "Y";

        // Joined fields
        /// <summary>컴포넌트 유형 (Join)</summary>
        public ComponentType ComponentType { get; set; }
        
        /// <summary>컴포넌트 이름 (Join)</summary>
        public string ComponentName { get; set; }
        
        /// <summary>설치 경로 (Join)</summary>
        public string InstallPath { get; set; }
        
        /// <summary>그룹명 (Join)</summary>
        public string GroupName { get; set; }
    }

    /// <summary>
    /// 클라이언트 컴포넌트 설치 현황
    /// </summary>
    public class ClientComponentDto
    {
        /// <summary>컴포넌트 ID</summary>
        public string ComponentId { get; set; }
        
        /// <summary>설치된 버전</summary>
        public string InstalledVersion { get; set; }
        
        /// <summary>설치 경로</summary>
        public string InstalledPath { get; set; }
        
        /// <summary>설치일</summary>
        public DateTime InstalledDate { get; set; }
        
        /// <summary>파일 해시 (무결성 확인용)</summary>
        public string FileHash { get; set; }
    }
}
