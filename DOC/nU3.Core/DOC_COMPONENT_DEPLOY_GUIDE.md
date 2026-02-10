# Framework Component 배포 시스템

## 개요

화면 모듈(`SYS_MODULE_MST`)과 별도로, Framework DLL, 공용 라이브러리, 실행파일 등을 관리하는 시스템입니다.

## 기존 시스템 vs 확장 시스템

| 구분 | 화면 모듈 | Framework 컴포넌트 |
|------|----------|-------------------|
| 테이블 | `SYS_MODULE_MST`, `SYS_MODULE_VER` | `SYS_COMPONENT_MST`, `SYS_COMPONENT_VER` |
| 설치 경로 | `Modules/{Category}/{SubSystem}/` | 유연 (`InstallPath` 지정) |
| 배포 단위 | DLL (화면 포함) | DLL, EXE, 설정파일 등 |
| 로딩 방식 | 런타임 동적 로드 | 앱 시작 전 사전 배포 |

## DB 스키마

### SYS_COMPONENT_MST (컴포넌트 마스터)

```sql
CREATE TABLE SYS_COMPONENT_MST (
    COMPONENT_ID TEXT PRIMARY KEY,     -- 예: "nU3.Core", "DevExpress.XtraEditors"
    COMPONENT_TYPE INTEGER NOT NULL,   -- 0:Screen, 1:Framework, 2:SharedLib, 3:Exe, ...
    COMPONENT_NAME TEXT NOT NULL,      -- 표시명
    FILE_NAME TEXT NOT NULL,           -- 파일명 (nU3.Core.dll)
    INSTALL_PATH TEXT,                 -- 설치 경로 (상대경로, 빈값=루트)
    GROUP_NAME TEXT,                   -- 그룹 (Framework, DevExpress, Oracle)
    IS_REQUIRED INTEGER DEFAULT 0,     -- 필수 여부
    AUTO_UPDATE INTEGER DEFAULT 1,     -- 자동 업데이트
    DESCRIPTION TEXT,
    PRIORITY INTEGER DEFAULT 100,      -- 설치 우선순위 (낮을수록 먼저)
    DEPENDENCIES TEXT,                 -- 의존성 (쉼표 구분)
    REG_DATE TEXT,
    MOD_DATE TEXT,
    IS_ACTIVE TEXT DEFAULT 'Y'
);
```

### SYS_COMPONENT_VER (버전 관리)

```sql
CREATE TABLE SYS_COMPONENT_VER (
    COMPONENT_ID TEXT,
    VERSION TEXT,
    FILE_HASH TEXT,                    -- SHA256 해시
    FILE_SIZE INTEGER,
    STORAGE_PATH TEXT,                 -- 서버 저장 경로
    MIN_FRAMEWORK_VER TEXT,            -- 최소 Framework 버전
    MAX_FRAMEWORK_VER TEXT,            -- 최대 Framework 버전
    DEPLOY_DESC TEXT,
    RELEASE_NOTE_URL TEXT,
    REG_DATE TEXT,
    DEL_DATE TEXT,                     -- Soft delete
    IS_ACTIVE TEXT DEFAULT 'Y',
    PRIMARY KEY (COMPONENT_ID, VERSION)
);
```

### ComponentType 열거형

```csharp
public enum ComponentType
{
    ScreenModule = 0,     // 화면 모듈 (기존 방식)
    FrameworkCore = 1,    // nU3.Core.dll 등
    SharedLibrary = 2,    // DevExpress, Oracle 등
    Executable = 3,       // nU3.Shell.exe 등
    Configuration = 4,    // appsettings.json 등
    Resource = 5,         // 이미지, 아이콘 등
    Plugin = 6,           // 플러그인
    Other = 99
}
```

---

## 배포 흐름

```
┌────────────────────────────────────────────────────────────────┐
│                    서버 (Deployer Tool)                         │
├────────────────────────────────────────────────────────────────┤
│  1. DLL/EXE 파일 선택                                          │
│  2. 메타데이터 추출 (버전, 해시)                                │
│  3. DB 등록 (SYS_COMPONENT_MST, SYS_COMPONENT_VER)             │
│  4. 서버 저장소에 파일 복사                                     │
└────────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌────────────────────────────────────────────────────────────────┐
│                   클라이언트 (Bootstrapper)                     │
├────────────────────────────────────────────────────────────────┤
│  1. DB에서 활성 버전 목록 조회                                  │
│  2. 로컬 설치 현황 확인                                         │
│  3. 업데이트 필요 컴포넌트 판별                                 │
│  4. 서버에서 다운로드 → 캐시                                    │
│  5. 해시 검증                                                  │
│  6. 설치 경로에 복사                                            │
└────────────────────────────────────────────────────────────────┘
```

---

## 사용 예시

### 1. Deployer에서 컴포넌트 배포

```csharp
// ComponentDeployControl에서 Smart Deploy
var componentRepo = Program.ServiceProvider.GetRequiredService<IComponentRepository>();

// 단일 파일 배포
componentRepo.SaveComponent(new ComponentMstDto
{
    ComponentId = "nU3.Core",
    ComponentName = "nU3 Core Library",
    FileName = "nU3.Core.dll",
    ComponentType = ComponentType.FrameworkCore,
    InstallPath = "",  // 루트
    GroupName = "Framework",
    IsRequired = true,
    AutoUpdate = true,
    Priority = 10
});

componentRepo.AddVersion(new ComponentVerDto
{
    ComponentId = "nU3.Core",
    Version = "1.0.0.0",
    FileHash = "a1b2c3d4...",
    FileSize = 245760,
    StoragePath = @"D:\ServerStorage\Components\Framework\nU3.Core.dll",
    IsActive = "Y"
});
```

### 2. Bootstrapper에서 업데이트 체크

```csharp
var updateService = new ComponentUpdateService(componentRepo, installPath);

// 업데이트 확인
var updates = updateService.CheckForUpdates();
if (updates.Any())
{
    Console.WriteLine($"{updates.Count}개 업데이트 가능");
    
    // 업데이트 실행
    var progress = new Progress<ComponentUpdateProgressEventArgs>(p =>
    {
        Console.WriteLine($"[{p.Phase}] {p.CurrentComponentName} ({p.PercentComplete}%)");
    });
    
    var result = await updateService.UpdateAllAsync(progress, cancellationToken);
    
    if (result.Success)
        Console.WriteLine("모든 업데이트 완료!");
    else
        Console.WriteLine($"일부 실패: {string.Join(", ", result.FailedComponents.Select(f => f.ComponentId))}");
}
```

### 3. WinForms에서 업데이트 UI

```csharp
private async void CheckAndUpdateComponents()
{
    var updateService = new ComponentUpdateService(_componentRepo);
    var updates = updateService.CheckForUpdates();
    
    if (!updates.Any())
    {
        toolStripStatus.Text = "최신 버전입니다.";
        return;
    }
    
    if (MessageBox.Show($"{updates.Count}개 업데이트가 있습니다. 지금 업데이트하시겠습니까?",
        "업데이트 확인", MessageBoxButtons.YesNo) != DialogResult.Yes)
        return;
    
    try
    {
        var result = await AsyncOperationHelper.ExecuteWithProgressAsync(
            this,
            "컴포넌트 업데이트 중...",
            async (ct, progress) =>
            {
                var updateProgress = new Progress<ComponentUpdateProgressEventArgs>(p =>
                {
                    progress.Report(new BatchOperationProgress
                    {
                        TotalItems = p.TotalComponents,
                        CompletedItems = p.CurrentIndex,
                        CurrentItem = p.CurrentComponentName,
                        PercentComplete = p.PercentComplete
                    });
                });
                
                return await updateService.UpdateAllAsync(updateProgress, ct);
            });
        
        if (result.Success)
        {
            MessageBox.Show("업데이트가 완료되었습니다.\n변경사항 적용을 위해 프로그램을 재시작해주세요.",
                "완료", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
    catch (OperationCanceledException)
    {
        MessageBox.Show("업데이트가 취소되었습니다.", "취소");
    }
}
```

---

## 설치 경로 예시

```
설치 루트 (예: C:\Program Files\nU3.Shell\)
│
├── nU3.Shell.exe                 ← InstallPath: ""
├── nU3.Core.dll                  ← InstallPath: ""
├── nU3.Core.UI.dll               ← InstallPath: ""
├── nU3.Connectivity.dll          ← InstallPath: ""
├── appsettings.json              ← InstallPath: ""
│
├── DevExpress.Data.dll           ← InstallPath: ""
├── DevExpress.XtraEditors.dll    ← InstallPath: ""
│
├── plugins\                      ← InstallPath: "plugins"
│   └── MyPlugin.dll
│
├── resources\                    ← InstallPath: "resources"
│   └── images\
│       └── logo.png
│
└── Modules\                      ← 기존 화면 모듈 (별도 시스템)
    └── EMR\
        └── IN\
            └── nU3.Modules.EMR.IN.Worklist.dll
```

---

## 우선순위 가이드

| Priority | 유형 | 예시 |
|----------|------|------|
| 1-10 | 실행파일 | nU3.Shell.exe, nU3.Bootstrapper.exe |
| 11-20 | Framework 핵심 | nU3.Core.dll, nU3.Core.UI.dll |
| 21-50 | Framework 확장 | nU3.Data.dll, nU3.Connectivity.dll |
| 51-80 | 필수 라이브러리 | Oracle.ManagedDataAccess.dll |
| 81-100 | UI 라이브러리 | DevExpress.*.dll |
| 100+ | 기타 | 플러그인, 리소스 |

---

## 관련 파일

| 파일 | 설명 |
|------|------|
| `nU3.Models\ModuleModels.cs` | DTO (ComponentMstDto, ComponentVerDto 등) |
| `nU3.Core\Repositories\IComponentRepository.cs` | Repository 인터페이스 |
| `nU3.Data\Repositories\SQLiteComponentRepository.cs` | SQLite 구현 |
| `nU3.Data\LocalDatabaseManager.cs` | DB 스키마 (테이블 생성) |
| `nU3.Core\Services\ComponentUpdateService.cs` | 클라이언트 업데이트 서비스 |
| `nU3.Tools.Deployer\Views\ComponentDeployControl.cs` | Deployer UI |
