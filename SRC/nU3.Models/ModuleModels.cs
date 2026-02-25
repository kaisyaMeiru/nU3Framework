using System;
using System.Collections.Generic;

namespace nU3.Models
{
    public class ModuleMstDto
    {
        public string ModuleId { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string SubSystem { get; set; } = string.Empty;
        public string ModuleName { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public DateTime RegDate { get; set; }
    }

    public class ModuleVerDto
    {
        public string ModuleId { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public string FileHash { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public string StoragePath { get; set; } = string.Empty;
        public string DeployDesc { get; set; } = string.Empty;
        public DateTime? DelDate { get; set; }
        public string Category { get; set; } = string.Empty; // Joined from MST
    }

    /// <summary>
    /// Framework Component Types
    /// </summary>
    public enum ComponentType
    {
        /// <summary>화면 모듈 DLL (자동 로드)</summary>
        ScreenModule = 0,
        
        /// <summary>Framework 핵심 DLL (nU3.Core.dll 등)</summary>
        FrameworkCore = 1,
        
        /// <summary>공통 라이브러리 DLL (DevExpress, Oracle 등)</summary>
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

        /// <summary>image</summary>
        Image = 9,

        /// <summary>기타</summary>
        Other = 99
    }

    /// <summary>
    /// Framework Component Master DTO
    /// </summary>
    public class ComponentMstDto
    {
        public string ComponentId { get; set; } = string.Empty;
        public ComponentType ComponentType { get; set; }
        public string ComponentName { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string InstallPath { get; set; } = string.Empty;
        public string GroupName { get; set; } = string.Empty;
        public bool IsRequired { get; set; }
        public bool AutoUpdate { get; set; } = true;
        public string Description { get; set; } = string.Empty;
        public int Priority { get; set; }
        public string Dependencies { get; set; } = string.Empty;
        public DateTime RegDate { get; set; }
        public DateTime? ModDate { get; set; }
        public string IsActive { get; set; } = "Y";
    }

    /// <summary>
    /// Framework Component Version DTO
    /// </summary>
    public class ComponentVerDto
    {
        public string ComponentId { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public string FileHash { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public string StoragePath { get; set; } = string.Empty;
        public string MinFrameworkVersion { get; set; } = string.Empty;
        public string MaxFrameworkVersion { get; set; } = string.Empty;
        public string DeployDesc { get; set; } = string.Empty;
        public string ReleaseNoteUrl { get; set; } = string.Empty;
        public DateTime RegDate { get; set; }
        public DateTime? DelDate { get; set; }
        public string IsActive { get; set; } = "Y";

        // Joined fields
        public ComponentType ComponentType { get; set; }
        public string ComponentName { get; set; } = string.Empty;
        public string InstallPath { get; set; } = string.Empty;
        public string GroupName { get; set; } = string.Empty;
    }

    /// <summary>
    /// 클라이언트 컴포넌트 설치 현황
    /// </summary>
    public class ClientComponentDto
    {
        public string ComponentId { get; set; } = string.Empty;
        public string InstalledVersion { get; set; } = string.Empty;
        public string InstalledPath { get; set; } = string.Empty;
        public DateTime InstalledDate { get; set; }
        public string FileHash { get; set; } = string.Empty;
    }

    #region Update Models (Moved from ComponentLoader)

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
    /// <summary>
    /// 에러 리포팅을 위한 이메일 설정 정보
    /// </summary>
    public class EmailSettings
    {
        public string SmtpServer { get; set; } = "smtp.gmail.com";
        public int SmtpPort { get; set; } = 587;
        public bool EnableSsl { get; set; } = true;
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? FromEmail { get; set; }
        public string? FromName { get; set; } = "nU3 Framework";
        public string? ToEmail { get; set; }
        public int TimeoutMs { get; set; } = 30000;
    }

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