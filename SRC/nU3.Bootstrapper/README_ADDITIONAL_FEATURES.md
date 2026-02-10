# nU3 Bootstrapper - 부가 기능 가이드

## 개요

nU3 Bootstrapper는 프레임워크 실행 전에 필요한 환경을 준비하는 핵심 컴포넌트입니다.

### 주요 기능

1. **모듈 배포 및 업데이트**
   - 화면 모듈 (nU3.Modules.*.dll) 자동 업데이트
   - Framework 컴포넌트 (nU3.Core.dll 등) 업데이트
   - 공통 라이브러리 (DevExpress, Oracle 등) 업데이트

2. **Legacy 컴포넌트 관리**
   - OCX/ActiveX 컨트롤 등록
   - COM DLL 등록/해제
   - 자동 등록 및 수동 등록 지원

3. **Custom URI 등록**
   - 프로토콜 핸들러 등록 (예: `nu3://open?screen=PatientList`)
   - 웹 브라우저에서 직접 애플리케이션 실행 가능

---

## 1. Legacy 컴포넌트 관리

### 1.1 설정 방법 (appsettings.json)

```json
{
  "LegacyComponents": [
    {
      "Name": "Sample OCX Control",
      "Path": "LegacyComponents\\SampleControl.ocx",
      "AutoRegister": true,
      "Description": "Sample ActiveX Control"
    },
    {
      "Name": "Legacy DLL Component",
      "Path": "LegacyComponents\\LegacyComponent.dll",
      "AutoRegister": false,
      "Description": "Legacy COM DLL"
    }
  ]
}
```

### 1.2 설정 항목 설명

| 항목 | 설명 | 예시 |
|------|------|------|
| `Name` | 컴포넌트 이름 (표시용) | "Sample OCX Control" |
| `Path` | 컴포넌트 파일 경로 (상대/절대) | "LegacyComponents\\SampleControl.ocx" |
| `AutoRegister` | 자동 등록 여부 | true / false |
| `Description` | 설명 (선택) | "Sample ActiveX Control" |

### 1.3 지원 파일 형식

- **.ocx** - ActiveX Control
- **.dll** - COM DLL
- **기타 COM 컴포넌트**

### 1.4 등록 방식

1. **자동 등록** (`AutoRegister: true`)
   - Bootstrapper 시작 시 자동으로 등록
   - `DllRegisterServer()` 함수 호출
   - 실패 시 `regsvr32.exe` 사용

2. **수동 등록** (`AutoRegister: false`)
   - 등록하지 않고 파일만 배포
   - 필요 시 수동으로 등록

### 1.5 등록 로그 확인

```
=== Legacy 컴포넌트 등록 ===
[INFO] 컴포넌트 등록 시도: C:\App\LegacyComponents\SampleControl.ocx
[INFO] 컴포넌트 등록 성공 (DllRegisterServer): C:\App\LegacyComponents\SampleControl.ocx
[INFO] 등록 성공한 컴포넌트: 1개
  - C:\App\LegacyComponents\SampleControl.ocx
=== Legacy 컴포넌트 등록 완료 ===
```

### 1.6 수동 등록/해제 (코드 예시)

```csharp
using nU3.Bootstrapper.Services;

// 등록
var manager = new LegacyComponentManager();
bool success = manager.RegisterComponent(@"C:\App\LegacyComponents\SampleControl.ocx");

// 등록 해제
bool unregisterSuccess = manager.UnregisterComponent(@"C:\App\LegacyComponents\SampleControl.ocx");

// 등록된 컴포넌트 목록 확인
var registeredComponents = manager.GetRegisteredComponents();
foreach (var component in registeredComponents)
{
    Console.WriteLine($"등록됨: {component}");
}
```

---

## 2. Custom URI 등록

### 2.1 설정 방법 (appsettings.json)

```json
{
  "CustomUri": {
    "Enabled": true,
    "Scheme": "nu3",
    "Description": "nU3 Framework Protocol"
  }
}
```

### 2.2 설정 항목 설명

| 항목 | 설명 | 예시 |
|------|------|------|
| `Enabled` | Custom URI 등록 여부 | true / false |
| `Scheme` | URI 스킴 (프로토콜 이름) | "nu3" |
| `Description` | 설명 (레지스트리에 표시) | "nU3 Framework Protocol" |

### 2.3 사용 예시

#### 웹 브라우저에서 실행

```html
<!-- HTML 링크 -->
<a href="nu3://open?screen=PatientList">환자 목록 열기</a>

<!-- JavaScript -->
<script>
function openApp(screen) {
    window.location.href = `nu3://open?screen=${screen}`;
}
</script>

<button onclick="openApp('PatientList')">환자 목록</button>
<button onclick="openApp('DoctorSchedule')">의사 스케줄</button>
```

#### 명령줄에서 실행

```bash
# Windows
start nu3://open?screen=PatientList

# PowerShell
Start-Process "nu3://open?screen=PatientList"
```

#### C# 코드에서 실행

```csharp
using System.Diagnostics;

// Custom URI로 애플리케이션 실행
Process.Start(new ProcessStartInfo
{
    FileName = "nu3://open?screen=PatientList",
    UseShellExecute = true
});
```

### 2.4 URI 파라미터 전달

애플리케이션에서 URI 파라미터를 받아 처리할 수 있습니다:

```csharp
// MainShell Program.cs
static void Main(string[] args)
{
    // args[0]: "nu3://open?screen=PatientList&patientId=P001"
    
    if (args.Length > 0)
    {
        var uri = new Uri(args[0]);
        var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
        
        var screen = query["screen"];  // "PatientList"
        var patientId = query["patientId"];  // "P001"
        
        // 화면 열기
        OpenScreen(screen, patientId);
    }
    
    Application.Run(new MainShellForm());
}
```

### 2.5 등록 확인

#### 레지스트리 확인

```
경로: HKEY_CURRENT_USER\SOFTWARE\Classes\nu3

[nu3]
  (기본값) = "URL:nU3 Framework Protocol"
  URL Protocol = ""
  
  [DefaultIcon]
    (기본값) = "C:\App\nU3.Shell.exe,0"
  
  [shell\open\command]
    (기본값) = "C:\App\nU3.Shell.exe" "%1"
```

#### 코드로 확인

```csharp
using nU3.Bootstrapper.Services;

var manager = new CustomUriManager("nu3", @"C:\App\nU3.Shell.exe");
bool isRegistered = manager.IsRegistered();

if (isRegistered)
{
    Console.WriteLine("Custom URI가 등록되어 있습니다.");
}
else
{
    Console.WriteLine("Custom URI가 등록되어 있지 않습니다.");
    manager.Register();
}
```

### 2.6 등록 해제

#### 수동 해제 (코드)

```csharp
using nU3.Bootstrapper.Services;

var manager = new CustomUriManager("nu3", string.Empty);
bool success = manager.Unregister();

if (success)
{
    Console.WriteLine("Custom URI 해제 완료");
}
```

#### Configuration에서 해제

```csharp
using nU3.Bootstrapper.Services;
using Microsoft.Extensions.Configuration;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

bool success = CustomUriManager.UnregisterFromConfiguration(configuration);
```

---

## 3. 모듈 배포 및 업데이트

### 3.1 기존 기능 강화

1. **Framework 컴포넌트 업데이트**
   - `SYS_COMPONENT_MST` 및 `SYS_COMPONENT_VER` 테이블 사용
   - nU3.Core.dll, nU3.Core.UI.dll 등 자동 업데이트

2. **화면 모듈 업데이트**
   - `SYS_MODULE_MST` 및 `SYS_MODULE_VER` 테이블 사용
   - nU3.Modules.*.dll 자동 업데이트

3. **업데이트 UI 개선**
   - 진행 상황 표시
   - 실패한 컴포넌트 로그
   - 재시도 기능

### 3.2 SyncMode 설정

```json
{
  "ModuleStorage": {
    "SyncMode": "Minimum"
  }
}
```

| 모드 | 설명 |
|------|------|
| `Minimum` | 필수 컴포넌트만 업데이트 |
| `Full` | 모든 컴포넌트 업데이트 |
| `Selective` | 선택적 업데이트 (설정 필요) |

---

## 4. 실행 흐름

```
┌─────────────────────────────────────────────────────────────┐
│                    nU3.Bootstrapper.exe                      │
├─────────────────────────────────────────────────────────────┤
│                                                             │
│  1. Configuration 로드 (appsettings.json)                   │
│     ↓                                                       │
│  2. DB 연결 확인                                            │
│     ↓                                                       │
│  3. 모듈 업데이트 (ModuleLoaderService)                      │
│     - Framework 컴포넌트 업데이트                            │
│     - 화면 모듈 업데이트                                     │
│     ↓                                                       │
│  4. Legacy 컴포넌트 등록 (OCX, ActiveX)                     │
│     - LegacyComponents 설정 읽기                            │
│     - AutoRegister = true인 컴포넌트 등록                   │
│     ↓                                                       │
│  5. Custom URI 등록                                        │
│     - CustomUri:Enabled = true인 경우 등록                  │
│     ↓                                                       │
│  6. Shell 실행 (nU3.Shell.exe)                             │
│     - RuntimeDirectory\MainExecutable 실행                  │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

---

## 5. 트러블슈팅

### 5.1 Legacy 컴포넌트 등록 실패

**증상**: OCX/DLL 등록이 실패합니다.

**해결 방법**:

1. **관리자 권한으로 실행**
   ```bash
   # 관리자 권한으로 실행
   runas /user:Administrator nU3.Bootstrapper.exe
   ```

2. **수동 등록**
   ```bash
   # 명령 프롬프트 (관리자)
   regsvr32 "C:\App\LegacyComponents\SampleControl.ocx"
   
   # 등록 해제
   regsvr32 /u "C:\App\LegacyComponents\SampleControl.ocx"
   ```

3. **의존성 확인**
   - OCX/DLL이 의존하는 다른 파일이 있는지 확인
   - Dependency Walker 등의 도구 사용

### 5.2 Custom URI 등록 실패

**증상**: Custom URI가 동작하지 않습니다.

**해결 방법**:

1. **레지스트리 확인**
   ```
   HKEY_CURRENT_USER\SOFTWARE\Classes\nu3
   ```

2. **수동 등록**
   ```csharp
   var manager = new CustomUriManager("nu3", @"C:\App\nU3.Shell.exe");
   manager.Register();
   ```

3. **브라우저 재시작**
   - Custom URI 등록 후 브라우저 재시작 필요

### 5.3 모듈 업데이트 실패

**증상**: 모듈이 업데이트되지 않습니다.

**해결 방법**:

1. **서버 연결 확인**
   ```json
   {
     "ServerConnection": {
       "Enabled": true,
       "BaseUrl": "http://localhost:64228"
     }
   }
   ```

2. **로그 확인**
   ```
   Logs\nU3_Bootstrapper_{yyyyMMdd}.log
   ```

3. **수동 업데이트**
   - Deployer 도구에서 수동으로 모듈 업로드

---

## 6. 예제 시나리오

### 6.1 시나리오 1: ActiveX 컨트롤 배포

#### 1단계: OCX 파일 준비

```
C:\App\LegacyComponents\
  └── PatientCardReader.ocx  (카드 리더기 ActiveX)
```

#### 2단계: 설정 추가

```json
{
  "LegacyComponents": [
    {
      "Name": "Patient Card Reader",
      "Path": "LegacyComponents\\PatientCardReader.ocx",
      "AutoRegister": true,
      "Description": "환자 카드 리더기 ActiveX"
    }
  ]
}
```

#### 3단계: Bootstrapper 실행

```bash
nU3.Bootstrapper.exe
```

#### 4단계: Shell에서 사용

```csharp
// PatientRegistrationForm.cs
private void InitializeCardReader()
{
    // COM Interop을 통해 OCX 사용
    var cardReader = new PatientCardReaderLib.CardReader();
    cardReader.Initialize();
    cardReader.CardInserted += OnCardInserted;
}
```

### 6.2 시나리오 2: 웹에서 애플리케이션 실행

#### 1단계: Custom URI 등록

```json
{
  "CustomUri": {
    "Enabled": true,
    "Scheme": "nu3",
    "Description": "nU3 Framework Protocol"
  }
}
```

#### 2단계: 웹 페이지 작성

```html
<!DOCTYPE html>
<html>
<head>
    <title>nU3 Web Integration</title>
</head>
<body>
    <h1>환자 관리 시스템</h1>
    
    <!-- 환자 목록 열기 -->
    <a href="nu3://open?screen=PatientList">환자 목록</a>
    
    <!-- 특정 환자 상세 정보 -->
    <a href="nu3://open?screen=PatientDetail&patientId=P001">환자 P001</a>
    
    <!-- 의사 스케줄 -->
    <a href="nu3://open?screen=DoctorSchedule&doctorId=D001">의사 D001 스케줄</a>
</body>
</html>
```

#### 3단계: Shell에서 URI 처리

```csharp
// MainShell Program.cs
static void Main(string[] args)
{
    if (args.Length > 0 && args[0].StartsWith("nu3://"))
    {
        // URI 파싱
        var uri = new Uri(args[0]);
        var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
        
        var screen = query["screen"];
        var parameters = new Dictionary<string, string>();
        
        foreach (var key in query.AllKeys)
        {
            if (key != "screen")
            {
                parameters[key] = query[key];
            }
        }
        
        // 화면 열기
        Application.Run(new MainShellForm(screen, parameters));
    }
    else
    {
        Application.Run(new MainShellForm());
    }
}
```

---

## 7. 참고 자료

### 7.1 관련 파일

| 파일 | 설명 |
|------|------|
| `nU3.Bootstrapper/Program.cs` | 메인 진입점 |
| `nU3.Bootstrapper/Services/LegacyComponentManager.cs` | Legacy 컴포넌트 관리 |
| `nU3.Bootstrapper/Services/CustomUriManager.cs` | Custom URI 관리 |
| `nU3.Bootstrapper/appsettings.json` | 설정 파일 |

### 7.2 외부 링크

- [Custom URI Schemes (Microsoft Docs)](https://docs.microsoft.com/en-us/previous-versions/windows/internet-explorer/ie-developer/platform-apis/aa767914(v=vs.85))
- [regsvr32 명령어 (Microsoft Docs)](https://docs.microsoft.com/en-us/windows-server/administration/windows-commands/regsvr32)
- [COM 프로그래밍 가이드](https://docs.microsoft.com/en-us/dotnet/standard/native-interop/com-interop)

---

## 8. 라이선스

? 2024 nU3 Framework

---

**버전**: 1.0  
**최종 수정일**: 2024-02-XX  
**작성자**: nU3 Framework Team
