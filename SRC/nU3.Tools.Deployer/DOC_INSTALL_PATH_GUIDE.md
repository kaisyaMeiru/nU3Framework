# InstallPath 설정 가이드

## 개념

### 경로 구성

```
최종 설치 경로 = {BaseInstallPath} + {InstallPath} + {FileName}
                 ─────────┬────────   ─────┬─────   ────┬────
                        앱 실행 위치    상대 경로    파일명
```

### BaseInstallPath (실행 경로 - 자동)

클라이언트가 설치된 루트 디렉토리:

```csharp
string baseInstallPath = AppDomain.CurrentDomain.BaseDirectory;
```

**실제 경로 예시:**
- 일반 설치: `C:\Program Files\nU3.Shell\`
- 개발 환경: `D:\Projects\nU3.Framework\bin\Debug\`
- 포터블: `D:\MyApps\nU3\`

---

## InstallPath 설정 예시

### 1. 루트 디렉토리 (빈값 또는 null)

```
InstallPath: ""
FileName: "nU3.Core.dll"

→ C:\Program Files\nU3.Shell\nU3.Core.dll
```

**사용 대상:**
- Framework 핵심 DLL (nU3.Core.dll, nU3.Core.UI.dll)
- 실행 파일 (nU3.Shell.exe)
- 공용 라이브러리 (DevExpress.*.dll, Oracle.*.dll)
- 설정 파일 (appsettings.json)

### 2. 서브 폴더 (단일 깊이)

```
InstallPath: "plugins"
FileName: "MyPlugin.dll"

→ C:\Program Files\nU3.Shell\plugins\MyPlugin.dll
```

**사용 대상:**
- 플러그인
- 확장 모듈

### 3. 중첩 폴더 (여러 깊이)

```
InstallPath: "resources\\images"
FileName: "logo.png"

→ C:\Program Files\nU3.Shell\resources\images\logo.png
```

**사용 대상:**
- 리소스 파일
- 테마/스킨 파일
- 문서 파일

### 4. 화면 모듈 (기존 방식과 분리)

화면 모듈은 **별도 시스템** (`SYS_MODULE_MST`)에서 관리:

```
경로: {BaseInstallPath}\Modules\{Category}\{SubSystem}\{FileName}

예: C:\Program Files\nU3.Shell\Modules\EMR\IN\nU3.Modules.EMR.IN.Worklist.dll
```

---

## 컴포넌트 유형별 InstallPath 권장 사항

| ComponentType | InstallPath | 예시 |
|--------------|-------------|------|
| **FrameworkCore** | `""` | nU3.Core.dll → 루트 |
| **SharedLibrary** | `""` | DevExpress.XtraEditors.dll → 루트 |
| **Executable** | `""` | nU3.Shell.exe → 루트 |
| **Configuration** | `""` 또는 `"config"` | appsettings.json → 루트 또는 config\ |
| **Plugin** | `"plugins"` | MyPlugin.dll → plugins\ |
| **Resource** | `"resources"` 또는 `"resources\images"` | logo.png → resources\images\ |

---

## 실전 시나리오

### 시나리오 1: Framework 핵심 DLL 배포

```csharp
// Smart Deploy로 nU3.Core.dll 배포
// 자동 감지:
ComponentId: "nU3.Core"
ComponentName: "nU3 Core Library"
FileName: "nU3.Core.dll"
ComponentType: FrameworkCore
InstallPath: ""              ← 루트 (빈값)
GroupName: "Framework"
Priority: 10
IsRequired: true

// 클라이언트 설치:
→ C:\Program Files\nU3.Shell\nU3.Core.dll
```

### 시나리오 2: DevExpress 라이브러리

```csharp
ComponentId: "DevExpress.XtraEditors"
FileName: "DevExpress.XtraEditors.dll"
ComponentType: SharedLibrary
InstallPath: ""              ← 루트 (빈값)
GroupName: "DevExpress"
Priority: 81

// 클라이언트 설치:
→ C:\Program Files\nU3.Shell\DevExpress.XtraEditors.dll
```

### 시나리오 3: 플러그인

```csharp
ComponentId: "ReportPlugin"
FileName: "nU3.Plugin.Report.dll"
ComponentType: Plugin
InstallPath: "plugins"       ← 서브 폴더
GroupName: "Plugins"
Priority: 200

// 클라이언트 설치:
→ C:\Program Files\nU3.Shell\plugins\nU3.Plugin.Report.dll
```

### 시나리오 4: 리소스 파일

```csharp
ComponentId: "CompanyLogo"
FileName: "logo.png"
ComponentType: Resource
InstallPath: "resources\\images"  ← 중첩 폴더
GroupName: "Resources"
Priority: 500

// 클라이언트 설치:
→ C:\Program Files\nU3.Shell\resources\images\logo.png
```

---

## Deployer UI에서 설정 방법

### Smart Deploy 시 자동 추론

```csharp
// 파일 선택: C:\Dev\nU3.Core.dll

자동 설정:
  ComponentId: "nU3.Core"         ← 파일명에서 추출
  FileName: "nU3.Core.dll"        ← 파일명
  InstallPath: ""                 ← 기본값 (루트)
  
  // 규칙:
  if (fileName.StartsWith("nU3.Core")) 
      ComponentType = FrameworkCore, Priority = 10-20
  else if (fileName.StartsWith("DevExpress")) 
      ComponentType = SharedLibrary, Priority = 81-100
  else if (fileName.EndsWith(".exe"))
      ComponentType = Executable, Priority = 1-10
```

### 수동 조정

UI에서 InstallPath 필드에 직접 입력:

```
┌────────────────────────────────────────────┐
│ 설치 경로 (상대): [                     ] │
│ 빈값=루트, plugins, resources\images 등   │ ← 도움말
│ 최종: {실행경로}\{설치경로}\{파일명}      │ ← 예시
└────────────────────────────────────────────┘

입력 예시:
  "" → C:\Program Files\nU3.Shell\nU3.Core.dll
  "plugins" → C:\Program Files\nU3.Shell\plugins\MyPlugin.dll
  "resources\images" → C:\Program Files\nU3.Shell\resources\images\logo.png
```

---

## 코드에서 경로 계산

### ComponentUpdateService 내부

```csharp
private string GetInstallPath(ComponentMstDto component)
{
    // 상대 경로 조합
    var relativePath = string.IsNullOrEmpty(component.InstallPath) 
        ? component.FileName 
        : Path.Combine(component.InstallPath, component.FileName);
    
    // BaseInstallPath와 결합
    return Path.Combine(_installBasePath, relativePath);
}

// 예시 실행:
// _installBasePath = "C:\Program Files\nU3.Shell\"
// component.InstallPath = "plugins"
// component.FileName = "MyPlugin.dll"
// → "C:\Program Files\nU3.Shell\plugins\MyPlugin.dll"
```

---

## 서버 저장 경로 (StoragePath)

서버에서는 **절대 경로**로 저장:

```csharp
// Deployer에서 파일 업로드 시
string serverBasePath = _getServerStoragePath();  // D:\ServerStorage
string storagePath = Path.Combine(
    serverBasePath, 
    "Components",      // 고정
    component.GroupName,   // Framework, DevExpress 등
    component.FileName
);

// 예: D:\ServerStorage\Components\Framework\nU3.Core.dll
```

클라이언트는 이 `StoragePath`에서 파일을 다운로드합니다.

---

## 폴더 구조 비교

### 서버 (절대 경로)

```
D:\ServerStorage\
├── Components\              ← Framework 컴포넌트
│   ├── Framework\
│   │   ├── nU3.Core.dll
│   │   ├── nU3.Core.UI.dll
│   │   └── nU3.Shell.exe
│   ├── DevExpress\
│   │   ├── DevExpress.Data.dll
│   │   └── DevExpress.XtraEditors.dll
│   └── Plugins\
│       └── MyPlugin.dll
│
└── EMR\                     ← 화면 모듈 (기존)
    └── IN\
        └── nU3.Modules.EMR.IN.Worklist.dll
```

### 클라이언트 (상대 경로)

```
C:\Program Files\nU3.Shell\   ← BaseInstallPath
├── nU3.Shell.exe            ← InstallPath: ""
├── nU3.Core.dll             ← InstallPath: ""
├── nU3.Core.UI.dll          ← InstallPath: ""
├── DevExpress.Data.dll      ← InstallPath: ""
│
├── plugins\                 ← InstallPath: "plugins"
│   └── MyPlugin.dll
│
├── resources\               ← InstallPath: "resources"
│   └── images\              ← InstallPath: "resources\images"
│       └── logo.png
│
└── Modules\                 ← 화면 모듈 (별도 시스템)
    └── EMR\
        └── IN\
            └── nU3.Modules.EMR.IN.Worklist.dll
```

---

## 설정 체크리스트

배포 전 확인 사항:

- [ ] **InstallPath**: 상대 경로인가? (절대 경로 X)
- [ ] **슬래시**: 백슬래시(`\`) 사용 (Windows)
- [ ] **루트 설치**: InstallPath는 빈값(`""`)
- [ ] **서브 폴더**: 폴더 구분자 사용 (`plugins`, `resources\images`)
- [ ] **우선순위**: EXE(1-10) → Core DLL(11-20) → Shared DLL(81-100)

---

## FAQ

**Q: InstallPath를 `C:\Program Files\` 같은 절대 경로로 설정할 수 있나요?**  
A: 안 됩니다. 반드시 **상대 경로**만 사용해야 합니다. BaseInstallPath는 자동으로 결정됩니다.

**Q: 화면 모듈도 InstallPath로 관리하나요?**  
A: 아니요. 화면 모듈은 기존 `SYS_MODULE_MST` 시스템 (Category/SubSystem)을 사용합니다.

**Q: InstallPath에 `..` 상위 경로를 쓸 수 있나요?**  
A: 권장하지 않습니다. 보안상 BaseInstallPath 하위에만 설치해야 합니다.

**Q: 여러 버전을 동시에 설치할 수 있나요?**  
A: 아니요. 하나의 컴포넌트는 하나의 활성 버전만 유지합니다. 업데이트 시 기존 파일을 덮어씁니다.
