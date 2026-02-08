# nU3.Framework POC 중간 보고서
**차세대 의료시스템 프레임워크 아키텍처 검토**
> 버전: 1.0
> 프로젝트: nU3.Framework 

---

## 📋 목차

1. [POC 구현 개념 및 설명](#1-poc-구현-개념-및-설명-40)
2. [개발적 요소](#2-개발적-요소-30)
3. [개선해야 할 사항 및 추후 필수 구현 사항](#3-개선해야-할-사항-및-추후-필수-구현-사항-30)
4. [로드맵 및 계획](#4-로드맵-및-계획)

---

# 1. POC 구현 개념 및 설명 (40%)

## 1.1 프로젝트 개요

### 배경 및 목적
nU3.Framework는 대형 병원 환경을 위한 차세대 의료시스템 EMR(Electronic Medical Record) 플랫폼으로 설계되었습니다. SI(System Integration) 프로젝트의 특성상 다수의 개발자가 동시에 별도 업무시스템을 개발해야 하며, 각 시스템은 독립적이면서도 상호 연계성을 확보해야 합니다.

### 핵심 요구사항
- **독립적 모듈 개발**: 다수 개발자가 동시에 개발 가능하도록 DLL 참조 관계 최소화
- **동적 로딩**: Reflection 기반의 동적 모듈 로딩 시스템
- **실시간 업데이트**: 24시간 운영 중인 시스템의 핫-디플로이(Hot Deployment) 지원
- **IPC 통신**: Shell 간, Screen 간 환자 정보 등 데이터 전달
- **하드웨어 연동**: Serial/USB/카드리더기 연동
- **AI 시대 준비**: 표준화된 아키텍처와 확장 가능한 구조

### 기술 스택
| 카테고리 | 기술 | 버전 |
|---------|------|------|
| **Framework** | .NET | 8.0 |
| **UI Platform** | WinForms | - |
| **UI Components** | DevExpress | 23.2.9 |
| **Database (Local)** | SQLite | - |
| **Database (Server)** | Oracle | - |
| **Backend** | Spring Boot | (미완료) |
| **Architecture** | Plugin + Event-Driven | - |

## 1.2 아키텍처 설계

### 전체 아키텍처 개요

```
┌──────────────────────────────────────────────────────────────────────────┐
│                         Client Workstation                               │
├──────────────────────────────────────────────────────────────────────────┤
│                                                                          │
│  ┌────────────────────────────────────────────────────────────────┐    │
│  │  nU3.Bootstrapper (Entry Point)                               │    │
│  │  - Version Check                                               │    │
│  │  - Shadow Copy (Cache → Runtime)                               │    │
│  │  - Launch Shell                                                │    │
│  └────────────────────────────────────────────────────────────────┘    │
│                              ↓                                           │
│  ┌────────────────────────────────────────────────────────────────┐    │
│  │  nU3.Shell (Main Container - DevExpress MDI)                  │    │
│  │  - RibbonControl, StatusBar, DocumentManager                   │    │
│  │  - Dynamic Menu (NavBarControl)                                │    │
│  │  - Dependency Injection Container                               │    │
│  └────────────────────────────────────────────────────────────────┘    │
│                              ↓                                           │
│  ┌────────────────────────────────────────────────────────────────┐    │
│  │  Module Layer (Plugin Architecture)                             │    │
│  │  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐              │    │
│  │  │ ADM Module  │  │ EMR Module  │  │ OT Module   │              │    │
│  │  │ (DLL A)     │  │ (DLL B)     │  │ (DLL C)     │              │    │
│  │  └──────┬──────┘  └──────┬──────┘  └──────┬──────┘              │    │
│  └─────────┼─────────────────┼─────────────────┼──────────────────┘    │
│            │                 │                 │                          │
│            ↓                 ↓                 ↓                          │
│  ┌────────────────────────────────────────────────────────────────┐    │
│  │  Core Framework Layer                                           │    │
│  │  - IModule, IWorkForm Interfaces                              │    │
│  │  - Event Aggregator (Pub/Sub)                                  │    │
│  │  - WorkContext Manager                                         │    │
│  │  - UserSession (Global State)                                  │    │
│  └────────────────────────────────────────────────────────────────┘    │
│                              ↓                                           │
│  ┌────────────────────────────────────────────────────────────────┐    │
│  │  Data Access Layer                                             │    │
│  │  - SQLite Local DB (Configuration)                             │    │
│  │  - HttpDBAccessClient (Server Connectivity)                    │    │
│  │  - Unit of Work Pattern                                       │    │
│  └────────────────────────────────────────────────────────────────┘    │
│                              ↓                                           │
│            IPC (Named Pipes / gRPC)                                 │
│                              ↓                                           │
│  ┌────────────────────────────────────────────────────────────────┐    │
│  │  Server Side (Future)                                           │    │
│  │  - nU3.Server.Host (ASP.NET Core API)                          │    │
│  │  - Spring Boot Backend (Existing)                              │    │
│  │  - Oracle Database                                             │    │
│  └────────────────────────────────────────────────────────────────┘    │
│                                                                          │
└──────────────────────────────────────────────────────────────────────────┘
```

### 계층별 설명

#### Layer 1: Bootstrapper (진입점)
- **역할**: 애플리케이션 시작 전 초기화, 배포, 무결성 검사
- **주요 기능**:
  1. **버전 체크**: 로컬 SQLite DB(`SYS_MODULE_VER`)와 서버 버전 비교
  2. **Shadow Copy**: `%AppData%\nU3.Framework\Cache`(Staging) → `[AppDir]\Modules`(Runtime)
  3. **해시 검증**: SHA256 해시를 통한 파일 무결성 확인
  4. **Shell 실행**: `nU3.Shell.exe` 프로세스 시작

#### Layer 2: Core Framework (공용 계약)
- **nU3.Core**: 핵심 인터페이스와 서비스
  - `IModule`: 모듈 라이프사이클 (Initialize/Dispose)
  - `IWorkForm`: 모든 비즈니스 화면의 기본 계약
  - `UserSession`: 전역 싱글톤 인증 상태 관리
  - `WorkContext`: 작업 컨텍스트 공유
- **nU3.Core.UI**: UI 기반 클래스
  - `BaseWorkControl`: DevExpress.XtraEditors.XtraUserControl 상속 + IWorkForm
  - `BasePopupForm`: DevExpress.XtraEditors.XtraForm 상속
- **nU3.Core.UI.Components**: DevExpress 컨트롤 래핑
  - 표준화 강제 (기본 스타일, 다국어, 권한 제어)

#### Layer 3: Shell (컨테이너)
- **nU3.Shell**: 기본 WinForms Shell
- **nU3.MainShell**: DevExpress 기반 메인 Shell
- **기능**:
  - MDI 컨테이너 (Tabbed DocumentManager)
  - 동적 메뉴 생성 (SQLite `SYS_MENU` 기반)
  - 모듈 지연 로딩 (Lazy Loading)
  - DI 컨테이너 (Microsoft.Extensions.DependencyInjection)

#### Layer 4: Module Layer (비즈니스 로직)
- **구조**: `Modules\[Category]\[SubSystem]\[Name].dll`
- **카테고리**:
  - **ADM**: 관리 (Admin)
  - **EMR**: 전자의무기록 (Electronic Medical Record)
    - **IN**: 입원 (Inpatient)
    - **OT**: 수술실 (Operating Theater)
    - **CL**: 진료 (Clinic)
- **격리**: 모듈 간 직접 참조 없음, Event Aggregator로 통신

#### Layer 5: Data & Connectivity
- **nU3.Data**: SQLite 리포지토리
  - `SYS_MODULE_MST`: 모듈 마스터
  - `SYS_PROG_MST`: 프로그램 마스터 (Screen 정보)
  - `SYS_MENU`: 메뉴 구조
  - `SYS_MODULE_VER`: 버전 관리
- **nU3.Connectivity**: 서버 연결
  - `HttpDBAccessClient`: HTTP 기반 DB 엑세스
  - `HttpFileTransferClient`: 파일 전송

## 1.3 핵심 기능 설명

### 3.1 동적 모듈 로딩 (Dynamic Module Loading)

#### 구현 개념
- **AssemblyLoadContext** 사용: .NET Core/5+의 어셈블리 로딩 컨텍스트 활용
- **MetadataScanner**: 로드된 DLL에서 `[ScreenInfo]` 속성 스캔
- **ProgramRegistry**: `ScreenId` (String) → `Type` (Class) 매핑

#### 실행 흐름
1. 사용자가 메뉴 클릭
2. SQLite `SYS_MENU`에서 `PROG_ID` 조회
3. `SYS_PROG_MST`에서 `CLASS_NAME`과 `MODULE_ID` 확인
4. 모듈 DLL이 로드되지 않았으면 로드 후 `IModule.Initialize()` 호출
5. DI 컨테이너를 통해 인스턴스 생성
6. DocumentManager (Tabbed View)에 추가

#### 코드 예시 (개념)
```csharp
// Module Loader
public class ModuleLoader
{
    private AssemblyLoadContext _alc;

    public void LoadModule(string dllPath)
    {
        _alc = new AssemblyLoadContext(dllPath, isCollectible: true);
        var assembly = _alc.LoadFromAssemblyPath(dllPath);

        // IModule 구현 클래스 찾기
        var moduleType = assembly.GetTypes()
            .FirstOrDefault(t => typeof(IModule).IsAssignableFrom(t));

        if (moduleType != null)
        {
            var module = (IModule)Activator.CreateInstance(moduleType);
            module.Initialize();
        }
    }
}
```

### 3.2 이벤트 기반 통신 (Event-Driven Communication)

#### 구현 개념
- **Pub/Sub 패턴**: Prism Library의 `EventAggregator` 패턴 활용
- **느슨한 결합**: 모듈 간 직접 참조 없이 메시지 주고받기

#### 활용 사례
1. **환자 선택 이벤트**:
   - 환자 조회 화면(A)에서 환자 선택
   - `PatientSelectedEvent` 발행 (Publish)
   - 진료 화면(B), 입원 화면(C)가 이벤트 구독 (Subscribe)
   - 자동으로 데이터 갱신

2. **Shell 간 통신**:
   - Named Pipes 또는 gRPC (Localhost) 사용
   - 메인 Shell과 서브 Shell 간 환자 정보 동기화

#### 코드 예시 (개념)
```csharp
// Event Aggregator
public class PatientSelectedEventPayload
{
    public string PatientId { get; set; }
    public string PatientName { get; set; }
    public string Source { get; set; }  // 발생지 Screen ID
}

// 발행 (Publish)
EventBus?.GetEvent<PatientSelectedEvent>()
    .Publish(new PatientSelectedEventPayload
    {
        PatientId = "P001",
        PatientName = "홍길동",
        Source = ScreenId
    });

// 구독 (Subscribe)
protected override void OnScreenActivated()
{
    EventBus?.GetEvent<PatientSelectedEvent>()
        .Subscribe(OnPatientSelected);
}

private void OnPatientSelected(object payload)
{
    if (payload is PatientSelectedEventPayload evt && evt.Source == ScreenId)
        return;  // 자신의 이벤트 무시

    // 데이터 갱신
    LoadPatientData(evt.PatientId);
}
```

### 3.3 Shadow Copy 배포 (Hot Deployment)

#### 문제 상황
Windows OS는 실행 중인 DLL을 덮어쓸 수 없음 → 배포 중 서비스 중단 발생

#### 해결 전략: 2단계 배포

```
┌────────────────────────────────────────────────────────────────────┐
│  배포 흐름                                                         │
├────────────────────────────────────────────────────────────────────┤
│                                                                    │
│  1. 서버 → 클라이언트 다운로드                                     │
│     ServerStorage → %AppData%\nU3.Framework\Cache (Staging)         │
│                                                                    │
│  2. Shell 종료 확인 (Bootstrapper 실행)                           │
│     DLL이 잠겨 있지 않음                                          │
│                                                                    │
│  3. Staging → Runtime 복사                                         │
│     %AppData%\... \Cache → [AppDir]\Modules                        │
│                                                                    │
│  4. Shell 재실행                                                   │
│     최신 버전 로드                                                  │
│                                                                    │
└────────────────────────────────────────────────────────────────────┘
```

#### 장점
- **무중단 배포**: Shell 실행 중에도 업데이트 가능
- **안전한 업데이트**: Staging 영역에서 무결성 검증 후 Runtime으로 복사
- **롤백 용이**: 이전 버전 유지

### 3.4 메타데이터 기반 발견 (Metadata-Driven Discovery)

#### 구현 개념
- **Attribute 기반**: `[nU3ProgramInfo]` 속성으로 화면 정보 정의
- **자동 등록**: Deployer 도구가 속성을 스캔하여 DB에 자동 등록

#### 속성 정의
```csharp
[nU3ProgramInfo(
    typeof(MyModuleScreen),
    "화면 표시 이름",
    "MODULE_ID",
    "CHILD")]

public class MyModuleScreen : BaseWorkControl
{
    public override string ScreenId => "MY_SCREEN_001";
    protected override void OnScreenActivated() { }
}
```

#### DB 스키마 자동 생성
- Deployer가 모듈 DLL 스캔
- `[nU3ProgramInfo]` 속성 추출
- `SYS_PROG_MST`, `SYS_MENU`에 자동 등록

## 1.4 현재 구현 상태

### ✅ 완료된 기능 (Phase 1)
| 카테고리 | 기능 | 상태 |
|---------|------|------|
| **아키텍처** | 모듈형 플러그인 시스템 | ✅ 완료 |
| **아키텍처** | 동적 DLL 로딩 (ModuleLoaderService) | ✅ 완료 |
| **아키텍처** | Attribute 기반 메타데이터 | ✅ 완료 |
| **아키텍처** | 이벤트 기반 모듈 통신 (PubSub EventAggregator) | ✅ 완료 |
| **아키텍처** | WorkContext 공유 시스템 | ✅ 완료 |
| **UI** | WinForms 기반 Shell (DevExpress) | ✅ 완료 |
| **UI** | BaseWorkControl 기반 클래스 | ✅ 완료 |
| **UI** | 메뉴 동적 생성 | ✅ 완료 |
| **로깅** | 파일 기반 로깅 (LogManager) | ✅ 완료 |
| **로깅** | 감사 로그 (AuditLogger) | ✅ 완료 |
| **에러 처리** | 크래시 리포트 | ✅ 완료 |
| **에러 처리** | 스크린샷 자동 캡처 | ✅ 완료 |
| **연결성** | HTTP 기반 서버 연결 | ✅ 완료 |
| **연결성** | 파일 전송 (HttpFileTransferClient) | ✅ 완료 |
| **연결성** | DB 엑세스 (HttpDBAccessClient) | ✅ 완료 |
| **배포** | 컴포넌트 업데이트 시스템 | ✅ 완료 |
| **배포** | 버전 관리 (ComponentVerDto) | ✅ 완료 |
| **데이터** | SQLite 리포지토리 | ✅ 완료 |
| **데이터** | 기본 DTO 모델 (환자, 사용자 등) | ✅ 완료 |

### 🚧 진행 중인 기능 (Phase 2)
| 카테고리 | 기능 | 상태 |
|---------|------|------|
| **보안** | UserSession 기본 구현 | 🚧 진행 중 |
| **보안** | 권한 제어 (AuthLevel) | 🚧 진행 중 |
| **서버** | ASP.NET Core API 서버 | 🚧 진행 중 |
| **서버** | Spring Boot 연동 | ⏳ 계획 중 |

### ❌ 미구현 기능 (Phase 3+)
- JWT/OAuth 2.0 인증
- RBAC/ABAC 권한 제어
- HL7/FHIR 의료 표준
- 외부 시스템 연동 (LIS, PACS, ORIS)
- 단위/통합 테스트
- CI/CD 파이프라인
- 모니터링 및 로그 중앙화
- Docker/Kubernetes 배포

---

# 2. 개발적 요소 (30%)

## 2.1 기술 스택 상세

### 2.1.1 프레임워크 및 런타임
| 기술 | 버전 | 용도 |
|------|------|------|
| .NET | 8.0 | 기본 런타임 |
| C# | 12.0 | 개발 언어 |

### 2.1.2 UI 프레임워크
| 기술 | 버전 | 용도 |
|------|------|------|
| WinForms | - | 기본 UI 플랫폼 |
| DevExpress WinForms | 23.2.9 | UI 컴포넌트 |
| DevExpress DevExpress.TemplateKit | 23.2.9 | 템플릿 키트 |

### 2.1.3 데이터베이스
| 기술 | 용도 |
|------|------|
| SQLite | 로컬 설정/메타데이터 저장 |
| Oracle | 중앙 데이터베이스 (백엔드) |
| Microsoft.Data.SqlClient | SQL Server 연동 (추가) |

### 2.1.4 네트워크 및 통신
| 기술 | 용도 |
|------|------|
| System.Net.Http | HTTP 통신 |
| System.Net.Http.Json | JSON 직렬화/역직렬화 |
| Named Pipes | 로컬 IPC (계획) |
| gRPC | 로컬 IPC (계획) |

### 2.1.5 의존성 주입
| 기술 | 버전 | 용도 |
|------|------|------|
| Microsoft.Extensions.DependencyInjection | 8.0.0 | DI 컨테이너 |

## 2.2 코드 구조 및 패턴

### 2.2.1 프로젝트 구조
```
SRC/
├── nU3.Core/                    # 프레임워크 코어
│   ├── Interfaces/               # 핵심 인터페이스
│   │   ├── IModule.cs
│   │   ├── IWorkForm.cs
│   │   ├── IAuthenticationService.cs
│   │   └── IWorkContextProvider.cs
│   ├── Context/                  # 컨텍스트 관리
│   │   └── WorkContext.cs
│   ├── Events/                   # 이벤트 시스템
│   ├── Logging/                  # 로깅
│   ├── Enums/                    # 열거형
│   └── Attributes/               # nU3ProgramInfo 등
├── nU3.Core.UI/                # UI 기반 클래스
│   └── Shell/                   # ShellFormBase
├── nU3.Core.UI.Components/      # nU3 래핑 컨트롤
├── nU3.Data/                   # 데이터 엑세스 레이어
│   └── Repositories/            # SQLite 리포지토리
├── nU3.Models/                 # DTO 모델
├── nU3.Shell/                  # 메인 쉘 (WinForms)
├── nU3.MainShell/              # 메인 쉘 (DevExpress)
├── nU3.Bootstrapper/           # 애플리케이션 부트스트래퍼
├── nU3.Connectivity/           # 서버 연결
├── nU3.Tools.Deployer/         # 배포 도구
├── Servers/
│   ├── nU3.Server.Host/        # ASP.NET Core API 서버
│   └── nU3.Server.Connectivity/ # 서버 연결 서비스
└── Modules/
    ├── ADM/                     # 관리 모듈
    └── EMR/                    # 전자의무기록 모듈
        ├── IN/                 # 입원 (Inpatient)
        ├── OT/                 # 수술실 (Operating Theater)
        └── CL/                 # 진료 (Clinic)
```

### 2.2.2 디자인 패턴 활용

#### Plugin Pattern
```csharp
// Core 인터페이스
public interface IModule
{
    void Initialize();
    void Dispose();
}

// 모듈 구현
public class EmrModule : IModule
{
    public void Initialize()
    {
        // 모듈 초기화 로직
    }

    public void Dispose()
    {
        // 리소스 정리
    }
}
```

#### Unit of Work Pattern
```csharp
public interface IUnitOfWork : IDisposable
{
    IRepository<T> Repository<T>() where T : class;
    Task<int> CommitAsync();
    Task RollbackAsync();
}

public class UnitOfWork : IUnitOfWork
{
    private readonly IDbConnection _connection;
    private readonly IDbTransaction _transaction;
    private readonly Dictionary<Type, object> _repositories;

    public async Task<int> CommitAsync()
    {
        try
        {
            return await _transaction.CommitAsync();
        }
        catch
        {
            await _transaction.RollbackAsync();
            throw;
        }
    }
}
```

#### Event Aggregator Pattern (Pub/Sub)
```csharp
public interface IEventAggregator
{
    TEventType GetEvent<TEventType>() where TEventType : EventBase, new();
}

public abstract class EventBase
{
    public virtual void Subscribe(Action<object> action, bool keepSubscriberReferenceAlive = false)
    {
        // 구독 로직
    }

    public virtual void Publish(object payload)
    {
        // 발행 로직
    }
}
```

#### Factory Pattern (Module Factory)
```csharp
public interface IModuleFactory
{
    IModule CreateModule(string moduleId);
}

public class ModuleFactory : IModuleFactory
{
    private readonly IServiceProvider _serviceProvider;

    public IModule CreateModule(string moduleId)
    {
        var moduleType = GetModuleType(moduleId);
        return (IModule)_serviceProvider.GetService(moduleType);
    }
}
```

### 2.2.3 명명 규칙

| 요소 | 규칙 | 예시 |
|------|------|------|
| **Classes** | PascalCase | `PatientListControl` |
| **Interfaces** | I-prefixed | `IShellForm`, `IWorkForm` |
| **DTOs** | **Dto** suffix | `PatientInfoDto` |
| **Enums** | PascalCase | `ComponentType` |
| **Methods** | PascalCase | `LoadData()` |
| **Private fields** | _camelCase | `_instance` |
| **Events** | **Event** suffix | `PatientSelectedEvent` |
| **Event Payloads** | **EventPayload** suffix | `PatientSelectedEventPayload` |
| **Exceptions** | **Exception** suffix | `AuthenticationException` |

### 2.2.4 코드 스타일 가이드

#### Imports & Namespaces
```csharp
// 1. System imports (alphabetical)
using System;
using System.Threading.Tasks;

// 2. Third-party (DevExpress)
using DevExpress.XtraEditors;

// 3. nU3 internal
using nU3.Core;
using nU3.Models;
```

#### Nullable Reference Types
```csharp
public string? ServerUrl { get; }
public Task<T> GetAsync<T>(string id);
```

#### Async/Await Pattern
```csharp
public async Task LoadDataAsync(CancellationToken token = default)
{
    token.ThrowIfCancellationRequested();
    var data = await _dbClient.ExecuteDataTableAsync(sql, token)
        .ConfigureAwait(false);
}
```

#### Error Handling & Logging
```csharp
try
{
    LogManager.Info(message, category);
}
catch
{
    // Never throw on non-fatal paths
}

// Always log exceptions with context
catch (Exception ex)
{
    LogManager.Error($"Operation failed: {ex.Message}", category, ex);
    throw; // Re-throw critical errors
}
```

## 2.3 배포 및 통합

### 2.3.1 모듈 배포 워크플로우

```
┌────────────────────────────────────────────────────────────────────┐
│  Module Development Workflow                                      │
├────────────────────────────────────────────────────────────────────┤
│                                                                    │
│  1. 개발자가 모듈 개발 (Visual Studio)                              │
│     - nU3.Core.dll 참조                                            │
│     - BaseWorkControl 상속                                          │
│     - [nU3ProgramInfo] 속성 추가                                   │
│                                                                    │
│  2. 빌드 (Debug/Release)                                            │
│     - .dll 파일 생성                                                │
│                                                                    │
│  3. Deployer 도구로 등록                                            │
│     - nU3.Tools.Deployer 실행                                       │
│     - 모듈 업로드 (SHA256 계산)                                     │
│     - 메뉴 편집기로 메뉴 구성                                      │
│                                                                    │
│  4. 서버에 배포                                                     │
│     - ServerStorage 폴더에 복사                                     │
│                                                                    │
│  5. 클라이언트 업데이트                                             │
│     - Bootstrapper 실행 시 자동 다운로드                            │
│     - Shadow Copy 후 실행                                           │
│                                                                    │
└────────────────────────────────────────────────────────────────────┘
```

### 2.3.2 Deployer 도구 기능

#### 기능 목록
1. **모듈 등록**: 모듈 DLL 업로드 및 메타데이터 등록
2. **버전 관리**: 버전 번호 및 해시 관리
3. **메뉴 편집기**: 시각적 드래그 앤 드롭 메뉴 구성
4. **배포**: ServerStorage로 배포

#### 사용 예시 (개념)
```csharp
// Deployer
var deployer = new ModuleDeployer();

// 모듈 업로드
await deployer.UploadModuleAsync(
    moduleId: "MOD_EMR_CLINIC",
    dllPath: "Modules/EMR/CL/nU3.Modules.EMR.CL.Component.dll",
    version: "1.0.0.0");

// 메뉴 구성
deployer.ConfigureMenu(new MenuItem
{
    MenuId = "MENU_EMR_CLINIC",
    MenuName = "진료",
    ParentId = null,
    ProgId = "PROG_EMR_CLINIC_001"
});
```

### 2.3.3 Bootstrapper 워크플로우

#### 초기화 단계
1. **LocalDatabaseManager 초기화**:
   - DB 경로: `%AppData%\nU3.Framework\Database\nU3_Local.db`
   - 스키마 초기화 (`InitializeSchema()`)

2. **경로 설정**:
   - Staging Path: `%AppData%\nU3.Framework\Cache`
   - Install Path: `[ExeDirectory]\Modules`

3. **버전 체크 및 다운로드**:
   - `SYS_MODULE_VER` 확인
   - ServerStorage에서 다운로드 (Staging으로)

4. **설치 및 실행**:
   - Staging → Runtime 복사
   - `nU3.Shell.exe` 실행

## 2.4 데이터 관리

### 2.4.1 로컬 SQLite 스키마

#### SYS_MODULE_MST (모듈 마스터)
| 컬럼 | 타입 | 설명 |
|------|------|------|
| MODULE_ID | VARCHAR(50) | 모듈 ID (PK) |
| MODULE_NAME | VARCHAR(100) | 모듈 이름 |
| CATEGORY | VARCHAR(50) | 카테고리 (EMR, ADM) |
| IS_USE | CHAR(1) | 사용 여부 (Y/N) |

#### SYS_MODULE_VER (모듈 버전)
| 컬럼 | 타입 | 설명 |
|------|------|------|
| MODULE_ID | VARCHAR(50) | 모듈 ID (FK) |
| VERSION | VARCHAR(20) | 버전 (1.0.0.0) |
| FILE_HASH | VARCHAR(64) | SHA256 해시 |
| FILE_PATH | VARCHAR(255) | 파일 경로 |
| UPLOAD_DATE | DATETIME | 업로드 날짜 |

#### SYS_PROG_MST (프로그램 마스터)
| 컬럼 | 타입 | 설명 |
|------|------|------|
| PROG_ID | VARCHAR(50) | 프로그램 ID (PK) |
| PROG_NAME | VARCHAR(100) | 프로그램 이름 |
| CLASS_NAME | VARCHAR(255) | 전체 클래스명 |
| MODULE_ID | VARCHAR(50) | 모듈 ID (FK) |
| AUTH_LEVEL | INT | 권한 레벨 |

#### SYS_MENU (메뉴)
| 컬럼 | 타입 | 설명 |
|------|------|------|
| MENU_ID | VARCHAR(50) | 메뉴 ID (PK) |
| MENU_NAME | VARCHAR(100) | 메뉴 이름 |
| PARENT_ID | VARCHAR(50) | 부모 메뉴 ID (FK) |
| PROG_ID | VARCHAR(50) | 프로그램 ID (FK) |
| SORT_ORDER | INT | 정렬 순서 |

### 2.4.2 HTTP 기반 데이터 통신

#### HttpDBAccessClient
```csharp
public class HttpDBAccessClient : IDBAccessService
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;

    public async Task<DataTable> ExecuteDataTableAsync(
        string sql,
        Dictionary<string, object> parameters)
    {
        var request = new DbRequest
        {
            Sql = sql,
            Parameters = parameters
        };

        var response = await _httpClient.PostAsJsonAsync(
            $"{_baseUrl}/api/db/query",
            request);

        return await response.Content.ReadFromJsonAsync<DataTable>();
    }
}
```

### 2.4.3 오프라인 모드 (계획)

#### 전략
- **로컬 SQLite 캐시**: 자주 조회하는 데이터 캐싱
- **연결 감지**: `NetworkChange.NetworkAvailabilityChanged`
- **자동 동기화**: 연결 복구 시 변경 사항 동기화

---

# 3. 개선해야 할 사항 및 추후 필수 구현 사항 (30%)

## 3.1 보안 및 인증/권한 (P0 - CRITICAL)

### 현재 상태
```csharp
// UserSession - 기본 세션 관리
public class UserSession
{
    public string UserId { get; private set; }
    public string UserName { get; private set; }
    public int AuthLevel { get; private set; }  // 숫자 기반 (0-9)
    public bool IsLoggedIn => !string.IsNullOrEmpty(UserId);
}
```

### 부족한 기능
| 기능 | 우선순위 | 설명 |
|------|---------|------|
| JWT/OAuth 2.0 토큰 인증 | P0 | 현재 세션만 존재, 토큰 기반 인증 없음 |
| RBAC (Role-Based Access Control) | P0 | 현재 AuthLevel(숫자)만 존재, 역할 기반이 아님 |
| ABAC (Attribute-Based Access Control) | P1 | 속성 기반 권한 제어 없음 |
| 다요소 인증 (MFA) | P1 | 2FA/다요소 인증 없음 |
| 세션 관리 | P0 | 타임아웃, 재발급, 동시 로그인 제어 없음 |
| 암호화 | P0 | 데이터베이스 암호화 (at-rest), 전송 암호화 (in-transit) |
| 감사 로그 (HIPAA 준수) | P0 | 의료 민감 정보 접근 기록 부족 |
| API Key 인증 | P1 | 서버 API 인증 체계 없음 |
| Client Certificate | P1 | 상호 인증 (mTLS) 지원 없음 |
| 비밀번호 정책 | P0 | 복잡성, 만료, 이력 관리 없음 |

### 구현 필요: JWT 인증 서비스
```csharp
public interface IAuthenticationService
{
    Task<AuthResult> AuthenticateAsync(LoginRequest request);
    Task<string> RefreshTokenAsync(string refreshToken);
    Task LogoutAsync(string token);
}

public class JwtAuthenticationService : IAuthenticationService
{
    private readonly JwtBearerOptions _options;
    private readonly ITokenRepository _tokenRepository;

    public async Task<AuthResult> AuthenticateAsync(LoginRequest request)
    {
        // 1. 사용자 검증
        var user = await _userRepository.FindByEmailAsync(request.Email);
        if (user == null || !_encryptionService.VerifyHash(request.Password, user.PasswordHash))
        {
            return AuthResult.Fail("Invalid credentials");
        }

        // 2. JWT 토큰 생성
        var accessToken = GenerateJwtToken(user);
        var refreshToken = GenerateRefreshToken();

        // 3. 리프레시 토큰 저장
        await _tokenRepository.SaveAsync(user.UserId, refreshToken);

        return AuthResult.Success(accessToken, refreshToken);
    }

    private string GenerateJwtToken(UserInfoDto user)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserId),
            new Claim(JwtRegisteredClaimNames.Name, user.UserName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("role", user.UserRole.ToString()),
            new Claim("dept_code", user.DepartmentCode?.ToString() ?? "")
        };

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: _options.SigningCredentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
```

### 구현 필요: RBAC 권한 서비스
```csharp
public interface IAuthorizationService
{
    Task<bool> HasAccessAsync(string userId, string resource, string action);
    Task<Permission> GetPermissionsAsync(string userId);
}

public class RoleBasedAuthorizationService : IAuthorizationService
{
    private readonly IRoleRepository _roleRepository;
    private readonly IPermissionRepository _permissionRepository;

    public async Task<bool> HasAccessAsync(string userId, string resource, string action)
    {
        // 1. 사용자 역할 조회
        var roles = await _roleRepository.GetUserRolesAsync(userId);

        // 2. 각 역할의 권한 확인
        foreach (var role in roles)
        {
            var permissions = await _permissionRepository.GetRolePermissionsAsync(role.RoleId);

            if (permissions.Any(p =>
                p.Resource == resource &&
                p.Actions.Contains(action) &&
                p.IsAllowed))
            {
                return true;
            }
        }

        return false;
    }
}
```

## 3.2 데이터 관리 (P0 - CRITICAL)

### 현재 상태
```csharp
// SQLite 리포지토리 - 단순 CRUD만 구현
public class SQLiteComponentRepository : IComponentRepository
{
    private readonly LocalDatabaseManager _db;

    public List<ComponentMstDto> GetAllComponents() { ... }
    public ComponentMstDto GetComponent(string componentId) { ... }
    public void SaveComponent(ComponentMstDto component) { ... }
}
```

### 부족한 기능
| 기능 | 우선순위 | 설명 |
|------|---------|------|
| Oracle/SQL Server 복제 | P0 | 현재 SQLite만 지원, 중앙 DB 연동 필요 |
| 마이그레이션 시스템 | P0 | DB 스키마 버전 관리 없음 |
| 자동 백업/복구 | P0 | 데이터 손실 방지를 위한 백업 시스템 |
| 데이터 검증 레이어 | P1 | 입력 데이터 검증, 비즈니스 규칙 적용 |
| Connection Pooling | P1 | 성능 최적화를 위한 커넥션 풀링 |
| 트랜잭션 관리 | P0 | 분산 트랜잭션, 롤백 지원 |
| 데이터 캐싱 | P1 | Redis/MemoryCache 도입 |
| Soft Delete | P1 | 논리적 삭제 지원 |
| Auditing (자동) | P0 | 데이터 변경 자동 기록 |
| 데이터 동기화 | P1 | 오프라인/온라인 동기화 |

### 구현 필요: Unit of Work 패턴
```csharp
public class UnitOfWork : IUnitOfWork
{
    private readonly IDbConnection _connection;
    private readonly IDbTransaction _transaction;
    private readonly Dictionary<Type, object> _repositories;

    public UnitOfWork(IDbConnectionFactory connectionFactory)
    {
        _connection = connectionFactory.CreateConnection();
        _connection.Open();
        _transaction = _connection.BeginTransaction();
        _repositories = new Dictionary<Type, object>();
    }

    public IRepository<T> Repository<T>() where T : class
    {
        if (_repositories.TryGetValue(typeof(T), out var repository))
        {
            return (IRepository<T>)repository;
        }

        var newRepository = new Repository<T>(_connection, _transaction);
        _repositories[typeof(T)] = newRepository;
        return newRepository;
    }

    public async Task<int> CommitAsync()
    {
        try
        {
            var result = await _transaction.CommitAsync();
            return result;
        }
        catch
        {
            await _transaction.RollbackAsync();
            throw;
        }
    }

    public async Task RollbackAsync()
    {
        await _transaction.RollbackAsync();
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _connection?.Dispose();
    }
}
```

### 구현 필요: 마이그레이션 서비스
```csharp
public class MigrationService : IMigrationService
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly IList<IMigration> _migrations;

    public async Task ApplyMigrationsAsync()
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        using var transaction = connection.BeginTransaction();

        try
        {
            // 1. 마이그레이션 히스토리 테이블 확인
            await EnsureMigrationHistoryTableAsync(connection, transaction);

            // 2. 현재 버전 확인
            var currentVersion = await GetCurrentVersionAsync(connection, transaction);

            // 3. 보류 중인 마이그레이션 적용
            var pendingMigrations = _migrations
                .Where(m => m.Version > currentVersion)
                .OrderBy(m => m.Version);

            foreach (var migration in pendingMigrations)
            {
                await migration.UpAsync(connection, transaction);
                await RecordMigrationAsync(connection, transaction, migration);
            }

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}
```

## 3.3 의료 전문 기능 (P1 - ESSENTIAL)

### 현재 상태
```csharp
// 기본 DTO만 존재
public class PatientInfoDto
{
    public string PatientId { get; set; }
    public string PatientName { get; set; }
    public DateTime BirthDate { get; set; }
    // ... 기본 정보만 포함
}
```

### 부족한 기능
| 기능 | 우선순위 | 설명 |
|------|---------|------|
| HL7 FHIR 데이터 모델 | P1 | HL7 v2/v3, FHIR R4 표준 지원 |
| ICD-10 코드 통합 | P1 | 질병 분류 코드 시스템 |
| DRG 그룹핑 | P1 | 진료별 그룹 (Diagnosis Related Groups) |
| 임상 워크플로우 엔진 | P1 | 진료 과정 자동화 |
| 의약품 상호작용 검사 | P0 | 약물 간 상호작용, 부작용 경고 |
| 알러지 관리 | P0 | 환자 알러지 기록 및 경고 |
| EMR/EHR 표준 준수 | P1 | HL7 CDA, CCD 지원 |
| DICOM 영상 지원 | P1 | 의료 영상 표준, PACS 연동 |
| 임상결과 통합 | P1 | 검사결과(LIS), 진단결과(RIS) |
| 처방/오더 시스템 | P1 | 전자처방, 검사 오더 |

### 구현 필요: HL7 메시지 처리
```csharp
public class Hl7Service : IHl7Service
{
    private readonly IMessageQueueService _messageQueue;

    public async Task<Hl7Message> ParseMessageAsync(string rawMessage)
    {
        try
        {
            // HL7 파싱
            var parsedMessage = Hl7MessageParser.Parse(rawMessage);

            // 메시지 타입 확인
            var messageType = parsedMessage.MessageType;

            // 로깅
            await LogHl7MessageAsync(parsedMessage, Direction.Inbound);

            return parsedMessage;
        }
        catch (Hl7ParseException ex)
        {
            await LogErrorAsync("HL7 parse error", ex);
            throw;
        }
    }

    public async Task SendAdtMessageAsync(AdtMessage message)
    {
        // HL7 메시지 생성
        var hl7Message = new AdtMessageBuilder()
            .SetMessageType("ADT^A01")  // 입원 등록
            .SetSendingFacility("HOSPITAL")
            .SetReceivingFacility("LIS")
            .SetPatient(message.Patient)
            .SetVisit(message.Visit)
            .Build();

        // 메시지 큐에 전송
        await _messageQueue.PublishAsync("hl7.adt", hl7Message);

        await LogHl7MessageAsync(hl7Message, Direction.Outbound);
    }
}
```

### 구현 필요: 약물 상호작용 검사
```csharp
public class ClinicalDecisionSupportService : IClinicalDecisionSupportService
{
    private readonly IDrugInteractionRepository _interactionRepository;

    public async Task<List<DrugInteraction>> CheckDrugInteractionsAsync(List<Drug> drugs)
    {
        var interactions = new List<DrugInteraction>();

        // 모든 약물 조합 확인
        for (int i = 0; i < drugs.Count; i++)
        {
            for (int j = i + 1; j < drugs.Count; j++)
            {
                var interaction = await _interactionRepository
                    .FindInteractionAsync(drugs[i].DrugCode, drugs[j].DrugCode);

                if (interaction != null)
                {
                    interaction.DrugA = drugs[i];
                    interaction.DrugB = drugs[j];
                    interactions.Add(interaction);
                }
            }
        }

        // 심각도 순 정렬
        return interactions
            .OrderByDescending(i => i.Severity)
            .ToList();
    }
}
```

## 3.4 외부 시스템 연동 (P1 - ESSENTIAL)

### 현재 상태
```csharp
// HTTP API 기본 연결만 존재
public class HttpDBAccessClient
{
    public async Task<DataTable> ExecuteDataTableAsync(string sql, Dictionary<string, object> parameters)
    {
        // 기본 HTTP POST 구현
    }
}
```

### 부족한 기능
| 기능 | 우선순위 | 설명 |
|------|---------|------|
| HL7 v2/v3 메시지 처리 | P1 | ADT, ORM, ORU 등 HL7 메시지 파싱/생성 |
| DICOM PACS 연동 | P1 | 의료 영상 저장/조회 (Q/R SCP) |
| 보험청구 시스템 연동 | P1 | EDI 837 청구, 835 수령 |
| 검사장비(LIS) 연동 | P1 | 검사결과 수신 (ASTM, HL7) |
| 수술장비(ORIS) 연동 | P1 | 수술 스케줄링, 장비 통합 |
| SOAP 웹 서비스 | P2 | 레거시 시스템 SOAP 연동 |
| RESTful API 통합 | P1 | 표준 REST API 클라이언트 |
| 메시지 큐 | P1 | RabbitMQ/Azure Service Bus |
| 웹훅 | P2 | 외부 시스템 알림 |

### 구현 필요: LIS 연동
```csharp
public class LisService : ILisService
{
    private readonly IHl7Service _hl7Service;

    public async Task<List<LabResult>> GetLabResultsAsync(string orderNumber)
    {
        // ORM^O01 메시지 전송 (검사 결과 조회)
        var request = new OrmMessageBuilder()
            .SetMessageType("ORM^O01")
            .SetOrderNumber(orderNumber)
            .SetQueryControlCode("QD")  // Query - Display
            .Build();

        await _hl7Service.SendOrmMessageAsync(request);

        // 응답 대기 (메시지 큐 구독)
        var results = await _hl7Service.WaitForResponseAsync<OruMessage>(
            messageType: "ORU^R01",
            correlationId: request.ControlId,
            timeout: TimeSpan.FromSeconds(30)
        );

        return results.ExtractLabResults();
    }

    public async Task SendLabOrderAsync(LabOrder order)
    {
        // ORM^O01 메시지 전송 (검사 오더)
        var message = new OrmMessageBuilder()
            .SetMessageType("ORM^O01")
            .SetControlCode("NW")  // New Order
            .SetOrder(order)
            .Build();

        await _hl7Service.SendOrmMessageAsync(message);
    }
}
```

## 3.5 테스트 인프라 (P0 - CRITICAL)

### 현재 상태
- ❌ 테스트 프로젝트 없음
- ❌ 단위 테스트 없음
- ❌ 통합 테스트 없음
- ❌ E2E 테스트 없음
- ❌ 테스트 커버리지 도구 없음
- ❌ CI/CD 파이프라인 없음

### 부족한 기능
| 기능 | 우선순위 | 설명 |
|------|---------|------|
| 단위 테스트 (xUnit/NUnit) | P0 | 모든 클래스 단위 테스트 |
| 통합 테스트 | P0 | 서비스 간 통합 테스트 |
| E2E 테스트 | P1 | UI 자동화 테스트 |
| 테스트 커버리지 | P0 | 80% 이상 커버리지 목표 |
| 모킹 프레임워크 | P0 | Moq/NSubstitute 도입 |
| CI/CD 파이프라인 | P1 | GitHub Actions/Azure DevOps |
| 테스트 데이터 관리 | P1 | 테스트용 시드 데이터 |
| 성능 테스트 | P2 | 부하 테스트, 스트레스 테스트 |

### 구현 필요: 단위 테스트
```csharp
// Tests/Unit/nU3.Core.Tests/UserSessionTests.cs
public class UserSessionTests
{
    [Fact]
    public void IsLoggedIn_ShouldReturnFalse_WhenUserIdIsNull()
    {
        // Arrange
        var session = new UserSession();

        // Act
        var result = session.IsLoggedIn;

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void SetSession_ShouldPopulateAllProperties()
    {
        // Arrange
        var session = new UserSession();

        // Act
        session.SetSession("user123", "John Doe", "DEPT001", 5);

        // Assert
        Assert.Equal("user123", session.UserId);
        Assert.Equal("John Doe", session.UserName);
        Assert.Equal("DEPT001", session.DeptCode);
        Assert.Equal(5, session.AuthLevel);
        Assert.True(session.IsLoggedIn);
    }
}
```

### 구현 필요: 통합 테스트
```csharp
// Tests/Integration/nU3.Api.IntegrationTests/PatientServiceIntegrationTests.cs
public class PatientServiceIntegrationTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture;

    public PatientServiceIntegrationTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task GetPatientAsync_ShouldReturnPatient_WhenExists()
    {
        // Arrange
        var service = new PatientService(_fixture.DbConnection);
        var patientId = "P001";

        // Act
        var patient = await service.GetPatientAsync(patientId);

        // Assert
        Assert.NotNull(patient);
        Assert.Equal(patientId, patient.PatientId);
    }
}
```

### 구현 필요: CI/CD 파이프라인
```yaml
# .github/workflows/ci-cd.yml
name: CI/CD Pipeline
on:
  push:
    branches: [ main, develop ]
  pull_request:
    branches: [ main ]
jobs:
  build-and-test:
    runs-on: windows-latest

    steps:
    - uses: actions/checkout@v3

    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: '8.0.x'

    - name: Restore dependencies
      run: dotnet restore

    - name: Build
      run: dotnet build --configuration Release --no-restore

    - name: Run unit tests
      run: dotnet test --filter "Category=Unit" --collect:"XPlat Code Coverage"

    - name: Run integration tests
      run: dotnet test --filter "Category=Integration"

    - name: Generate coverage report
      run: reportgenerator -reports:**/coverage.cobertura.xml -targetdir:coverage

    - name: Upload coverage
      uses: codecov/codecov-action@v3
      with:
        files: ./coverage/cobertura.xml

    - name: Build Docker image
      run: docker build -t nu3-server:${{ github.sha }} .

    - name: Push to registry
      if: github.ref == 'refs/heads/main'
      run: docker push nu3-server:${{ github.sha }}
```

## 3.6 모니터링 & 옵저버빌리티 (P1 - HIGH)

### 현재 상태
```csharp
// 기본 로그만 존재
LogManager.Info("Message", "Category");
```

### 부족한 기능
| 기능 | 우선순위 | 설명 |
|------|---------|------|
| APM (Application Performance Monitoring) | P1 | Application Insights, New Relic |
| 중앙화 로깅 | P1 | ELK Stack, Splunk |
| 메트릭 수집 | P1 | Prometheus, Grafana |
| 분산 추적 | P1 | OpenTelemetry, Jaeger |
| 알림 시스템 | P1 | PagerDuty, Slack, MS Teams |
| Health Check | P0 | Liveness/Readiness 프로브 |
| 용량 계획 | P2 | 로그 분석, 예측 |

### 구현 필요: OpenTelemetry 추적
```csharp
public class TracingService : ITracingService
{
    private readonly TracerProvider _tracerProvider;
    private readonly Tracer _tracer;

    public TracingService()
    {
        _tracerProvider = Sdk.CreateTracerProviderBuilder()
            .AddSource("nU3.Framework")
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddRedisInstrumentation()
            .AddJaegerExporter(options =>
            {
                options.AgentHost = "jaeger";
                options.AgentPort = 6831;
            })
            .Build();

        _tracer = TracerProvider.Default.GetTracer("nU3.Framework");
    }

    public IDisposable StartSpan(string operationName, Dictionary<string, string> tags = null)
    {
        var spanBuilder = _tracer
            .SpanBuilder(operationName)
            .SetSpanKind(SpanKind.Internal);

        if (tags != null)
        {
            foreach (var tag in tags)
            {
                spanBuilder.SetAttribute(tag.Key, tag.Value);
            }
        }

        var span = spanBuilder.StartSpan();
        return new SpanScope(span);
    }

    public async Task<T> TraceAsync<T>(string operationName, Func<Task<T>> operation, Dictionary<string, string> tags = null)
    {
        using var span = StartSpan(operationName, tags);

        try
        {
            var result = await operation();
            span.SetStatus(Status.Ok);
            return result;
        }
        catch (Exception ex)
        {
            span.SetStatus(Status.Error, ex.Message);
            span.RecordException(ex);
            throw;
        }
    }
}
```

### 구현 필요: Prometheus 메트릭
```csharp
public class MetricsService : IMetricsService
{
    private readonly Counter _counter;
    private readonly Histogram _histogram;
    private readonly Gauge _gauge;

    public MetricsService()
    {
        var factory = Metrics.WithCustomRegistry(...);

        _counter = factory.CreateCounter(
            "nu3_requests_total",
            "Total number of requests",
            new CounterConfiguration
            {
                LabelNames = new[] { "method", "endpoint", "status" }
            });

        _histogram = factory.CreateHistogram(
            "nu3_request_duration_seconds",
            "Request duration in seconds",
            new HistogramConfiguration
            {
                LabelNames = new[] { "method", "endpoint" }
            });

        _gauge = factory.CreateGauge(
            "nu3_active_users",
            "Number of active users");
    }

    public void RecordCounter(string name, double value, Dictionary<string, string> tags = null)
    {
        var labelValues = GetLabelValues(tags);
        _counter.WithLabels(labelValues).Inc(value);
    }

    public void RecordTiming(string name, TimeSpan duration, Dictionary<string, string> tags = null)
    {
        var labelValues = GetLabelValues(tags);
        _histogram.WithLabels(labelValues).Observe(duration.TotalSeconds);
    }
}
```

## 3.7 배포 & DevOps (P1 - HIGH)

### 현재 상태
- ❌ 수동 배포
- ❌ Docker 컨테이너화 없음
- ❌ Kubernetes 오케스트레이션 없음
- ❌ 자동화된 롤백 없음
- ❌ 환경 관리 체계 부족

### 부족한 기능
| 기능 | 우선순위 | 설명 |
|------|---------|------|
| Docker 컨테이너화 | P1 | 모든 서비스 Docker 이미지화 |
| Kubernetes 오케스트레이션 | P1 | K8s 배포 매니페스트 |
| Blue/Green 배포 | P1 | 무중단 배포 |
| 롤백 자동화 | P1 | 배포 실패 시 자동 롤백 |
| 환경 관리 | P1 | Dev/Staging/Prod 환경 분리 |
| Helm 차트 | P2 | 패키지 관리 |
| GitOps | P2 | ArgoCD/Flux |

### 구현 필요: Dockerfile
```dockerfile
# Dockerfile for nU3.Server.Host
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["nU3.Server.Host/nU3.Server.Host.csproj", "Servers/nU3.Server.Host/"]
RUN dotnet restore "Servers/nU3.Server.Host/nU3.Server.Host.csproj"
COPY . .
WORKDIR "/src/Servers/nU3.Server.Host"
RUN dotnet build "nU3.Server.Host.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "nU3.Server.Host.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "nU3.Server.Host.dll"]
```

### 구현 필요: Kubernetes Deployment
```yaml
# k8s/deployment.yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: nu3-server
  labels:
    app: nu3-server
spec:
  replicas: 3
  selector:
    matchLabels:
      app: nu3-server
  template:
    metadata:
      labels:
        app: nu3-server
        version: v1.0.0
    spec:
      containers:
      - name: nu3-server
        image: nu3-server:1.0.0
        ports:
        - containerPort: 80
        - containerPort: 443
        env:
        - name: ASPNETCORE_ENVIRONMENT
          value: "Production"
        - name: ConnectionStrings__DefaultConnection
          valueFrom:
            secretKeyRef:
              name: nu3-secrets
              key: database-connection
        livenessProbe:
          httpGet:
            path: /health/live
            port: 80
          initialDelaySeconds: 30
          periodSeconds: 10
        readinessProbe:
          httpGet:
            path: /health/ready
            port: 80
          initialDelaySeconds: 30
          periodSeconds: 10
        resources:
          requests:
            memory: "256Mi"
            cpu: "250m"
          limits:
            memory: "512Mi"
            cpu: "500m"
---
apiVersion: v1
kind: Service
metadata:
  name: nu3-server-service
spec:
  selector:
    app: nu3-server
  ports:
  - protocol: TCP
    port: 80
    targetPort: 80
  type: LoadBalancer
```

## 3.8 문서화 (P2 - MEDIUM)

### 부족한 기능
| 기능 | 우선순위 | 설명 |
|------|---------|------|
| API 문서 자동화 | P1 | Swagger/OpenAPI v3 |
| 아키텍처 결정 기록 (ADR) | P2 | ADR 포맷으로 의사결정 문서화 |
| 코드 예제 & 튜토리얼 | P2 | 개발자 온보딩 가이드 |
| 사용자 매뉴얼 | P2 | 최종 사용자 가이드 |
| 개발자 가이드 | P1 | 프레임워크 사용 가이드 |
| 배포 가이드 | P1 | 운영 팀 배포 가이드 |

---

# 4. 로드맵 및 계획

## 4.1 우선순위 매트릭스

### P0 - CRITICAL (즉시 구현 필요)
| 항목 | 예상 소요시간 | 비용 | 영향도 |
|------|------------|------|--------|
| 보안 (JWT, RBAC, 암호화) | 4-6주 | 중 | 매우 높음 |
| 테스트 인프라 (단위/통합 테스트) | 3-4주 | 낮 | 높음 |
| 데이터 관리 (마이그레이션, 백업) | 3-5주 | 중 | 매우 높음 |
| 트랜잭션 관리 | 2-3주 | 낮 | 매우 높음 |

### P1 - HIGH (다음 3개월 내)
| 항목 | 예상 소요시간 | 비용 | 영향도 |
|------|------------|------|--------|
| 의료 전문 기능 (HL7, FHIR, DICOM) | 6-8주 | 높 | 매우 높음 |
| 외부 시스템 연동 (LIS, ORIS, 보험) | 4-6주 | 중 | 높음 |
| 모니터링 & 로그 (APM, ELK) | 3-4주 | 중 | 높음 |
| 배포 자동화 (Docker, CI/CD) | 3-5주 | 중 | 높음 |
| 성능 최적화 (쿼리, 캐싱) | 2-4주 | 낮 | 높음 |
| 비동기 프로그래밍 | 2-3주 | 낮 | 높음 |

### P2 - MEDIUM (6개월 이내)
| 항목 | 예상 소요시간 | 비용 | 영향도 |
|------|------------|------|--------|
| 문서화 (API, 아키텍처) | 4-6주 | 낮 | 중 |
| DI 완전 구현 | 2-3주 | 낮 | 중 |
| 메시지 버스 (MassTransit) | 3-4주 | 중 | 중 |
| CQRS 패턴 | 4-5주 | 중 | 중 |
| 이벤트 소싱 | 6-8주 | 높 | 중 |

## 4.2 추천 로드맵

### 단계 1: 기본 토대 마련 (1-2개월)
**목표**: 테스트 가능하고, 보안이 확보된 기반 구축

#### 주 1-2: 테스트 인프라 구축
- [ ] xUnit 프로젝트 생성
- [ ] 모킹 프레임워크 설정 (Moq)
- [ ] CI/CD 파이프라인 기본 구성
- [ ] 코드 커버리지 50% 달성

#### 주 3-4: 보안 레이어 추가
- [ ] JWT 인증 서비스 구현
- [ ] RBAC 권한 서비스 구현
- [ ] 암호화 서비스 (AES, SHA256)
- [ ] 세션 관리 (타임아웃, 재발급)

#### 주 5-6: 데이터 레이어 개선
- [ ] Unit of Work 패턴 구현
- [ ] 마이그레이션 시스템 구현
- [ ] 백업 서비스 구현
- [ ] 캐싱 서비스 (Redis)

#### 주 7-8: 비동기 프로그래밍
- [ ] async/await 패턴 적용
- [ ] CancellationToken 사용
- [ ] 비동기 리포지토리

### 단계 2: 의료 표준 통합 (3-4개월)
**목표**: 의료 표준(HL7, FHIR) 준수

#### 주 9-12: HL7 통합
- [ ] HL7 파서 구현
- [ ] ADT 메시지 처리
- [ ] ORM/ORU 메시지 처리
- [ ] HL7 메시지 큐

#### 주 13-16: FHIR 서비스
- [ ] FHIR R4 모델 도입
- [ ] Patient Resource 구현
- [ ] Observation Resource 구현
- [ ] FHIR 서버 연동

#### 주 17-20: 임상결과 통합
- [ ] LIS 연동 (HL7)
- [ ] 검사결과 DTO 확장
- [ ] 결과 알림 이벤트
- [ ] 검사결과 캐싱

#### 주 21-24: 감사 로그 (HIPAA)
- [ ] 민감 정보 접근 로그
- [ ] 데이터 변경 로그
- [ ] 보고서 생성
- [ ] 로그 보존 정책

### 단계 3: 외부 연동 (5-6개월)
**목표**: 주요 외부 시스템 연동

#### 주 25-28: LIS 연동 완료
- [ ] 검사 오더 전송
- [ ] 검사결과 수신
- [ ] 실시간 구독
- [ ] 재시도/오류 처리

#### 주 29-32: ORIS 연동
- [ ] 수술 스케줄 동기화
- [ ] 수술실 상태 조회
- [ ] 장비 예약
- [ ] 수술실 배정

#### 주 33-36: DICOM PACS
- [ ] DICOM 파서 구현
- [ ] 이미지 저장 (C-STORE SCP)
- [ ] 이미지 조회 (C-FIND SCP)
- [ ] 이미지 가져오기 (C-MOVE SCP)

#### 주 37-40: 메시지 큐
- [ ] RabbitMQ 설정
- [ ] 비동기 메시징
- [ ] 메시지 순서 보장
- [ ] 데드레터 큐

### 단계 4: 운영 자동화 (7-8개월)
**목표**: 안정적인 배포/운영

#### 주 41-44: Docker 컨테이너화
- [ ] Dockerfile 작성
- [ ] docker-compose 설정
- [ ] 개발 환경 컨테이너화
- [ ] 로컬 테스트 환경

#### 주 45-48: CI/CD 파이프라인
- [ ] GitHub Actions 설정
- [ ] 빌드/테스트 자동화
- [ ] Docker 이미지 빌드/Push
- [ ] 배포 파이프라인

#### 주 49-52: Kubernetes 배포
- [ ] K8s 매니페스트 작성
- [ ] Helm 차트 작성
- [ ] 스테이징 환경 배포
- [ ] 블루/그린 배포

#### 주 53-56: 모니터링
- [ ] Prometheus + Grafana
- [ ] OpenTelemetry 추적
- [ ] ELK 스택 (로그)
- [ ] APM 도구 (Application Insights)
- [ ] 알림 시스템 (Slack/PagerDuty)

## 4.3 구현 체크리스트

### 보안
- [ ] JWT 인증 서비스 구현
- [ ] JWT 리프레시 토큰
- [ ] RBAC 권한 서비스
- [ ] 암호화 서비스 (AES, SHA256)
- [ ] 비밀번호 해싱 (bcrypt/Argon2)
- [ ] 세션 관리 (타임아웃, 재발급)
- [ ] 감사 로그 (민감 정보 접근)
- [ ] API Key 인증
- [ ] HTTPS 강제 (Production)

### 데이터 관리
- [ ] Unit of Work 패턴
- [ ] Generic Repository
- [ ] 마이그레이션 시스템
- [ ] 백업/복구 서비스
- [ ] 캐싱 서비스 (Redis)
- [ ] Soft Delete
- [ ] Auditing (자동 로그)
- [ ] 데이터 동기화
- [ ] Connection Pooling
- [ ] 트랜잭션 롤백

### 의료 표준
- [ ] HL7 v2 파서
- [ ] HL7 ADT 메시지 처리
- [ ] HL7 ORM/ORU 메시지 처리
- [ ] FHIR R4 모델
- [ ] FHIR Patient Resource
- [ ] FHIR Observation Resource
- [ ] ICD-10 코드 서비스
- [ ] DRG 그룹핑
- [ ] 약물 상호작용 검사
- [ ] 알러지 경고
- [ ] DICOM 파서
- [ ] DICOM C-STORE SCP
- [ ] DICOM C-FIND SCP
- [ ] DICOM C-MOVE SCP

### 외부 연동
- [ ] LIS 연동 (HL7)
- [ ] ORIS 연동
- [ ] PACS 연동
- [ ] 보험청구 시스템 (EDI 837)
- [ ] 메시지 큐 (RabbitMQ)
- [ ] 웹훅
- [ ] SOAP 웹 서비스

### 테스트
- [ ] 단위 테스트 (80% 커버리지)
- [ ] 통합 테스트
- [ ] E2E 테스트 (Selenium/Playwright)
- [ ] 성능 테스트
- [ ] 모킹 프레임워크 (Moq)
- [ ] 테스트 데이터 시드
- [ ] CI/CD 파이프라인

### 모니터링
- [ ] Health Check (Liveness/Readiness)
- [ ] Prometheus 메트릭
- [ ] Grafana 대시보드
- [ ] OpenTelemetry 추적
- [ ] ELK 스택
- [ ] APM 도구
- [ ] 알림 시스템

### 배포
- [ ] Docker 컨테이너화
- [ ] Docker Compose
- [ ] Kubernetes 배포
- [ ] Helm 차트
- [ ] CI/CD 파이프라인
- [ ] 블루/그린 배포
- [ ] 롤백 자동화

### 문서화
- [ ] API 문서 (Swagger/OpenAPI v3)
- [ ] 아키텍처 결정 기록 (ADR)
- [ ] 개발자 가이드
- [ ] 배포 가이드
- [ ] 사용자 매뉴얼
- [ ] 코드 예제

## 4.4 결론 및 제언

### 현재 상태 요약
nU3.Framework는 모듈형 플러그인 아키텍처, 이벤트 기반 통신, 로깅 시스템 등 **기본적인 프레임워크 기능이 잘 구현**되어 있습니다. 특히 다음과 같은 강점이 있습니다:

1. **확장 가능한 아키텍처**: 플러그인 패턴과 동적 로딩을 통해 모듈이 독립적으로 개발 가능
2. **안전한 배포**: Shadow Copy 기반의 핫 디플로이로 24시간 운영 가능
3. **느슨한 결합**: Event Aggregator를 통해 모듈 간 직접 참조 없음

하지만 대형 의료시스템으로서 필요한 다음 핵심 기능들이 **부족**합니다:

### 우선 구현 필요 사항
1. **보안 (P0)**: JWT, RBAC, 암호화, 감사 로그
2. **테스트 (P0)**: 단위/통합 테스트, CI/CD 파이프라인
3. **데이터 관리 (P0)**: 마이그레이션, 백업, 트랜잭션
4. **의료 표준 (P1)**: HL7, FHIR, DICOM, 약물 상호작용
5. **외부 연동 (P1)**: LIS, PACS, ORIS, 보험청구
6. **모니터링 (P1)**: APM, ELK, Prometheus
7. **배포 (P1)**: Docker, Kubernetes, CI/CD

### 추천 로드맵
약 **8개월**의 계획된 로드맵을 통해 이러한 부족한 기능들을 단계적으로 구현하면, nU3.Framework는 **안전하고, 규정 준수하며, 확장 가능한 대형 의료시스템 프레임워크**로 성장할 수 있을 것입니다.

### 성공 요인
1. **테스트 주도 개발**: 테스트 인프라 먼저 구축 후 기능 개발
2. **보안 우선**: 의료 민감 정보 보호를 위한 보안 레이어 우선 구현
3. **표준 준수**: HL7, FHIR, HIPAA 등 의료 표준 준수
4. **자동화**: CI/CD, 테스트, 배포 자동화로 개발 효율성 확보
5. **문서화**: API, 아키텍처, 배포 가이드 문서화

### 최종 권고
nU3.Framework는 **잘 설계된 기반**을 가지고 있으며, 우선순위에 따라 부족한 기능들을 체계적으로 구현하면 **차세대 의료시스템의 핵심 플랫폼**으로 성장할 수 있습니다. 특히 보안, 테스트, 의료 표준 준수가 성공의 핵심 요소입니다.

---

**문서 버전**: 1.0
**최종 수정일**: 2026-02-07
**작성자**: Architecture Review Team
**승인자**: [Pending]

---

## 📚 참고 자료

### 의료 표준
- [HL7 Standards](https://www.hl7.org/)
- [FHIR Specification](https://hl7.org/fhir/)
- [DICOM Standard](https://www.dicomstandard.org/)
- [ICD-10](https://www.cdc.gov/nchs/icd/icd10cm.htm)

### 보안
- [OWASP Top 10](https://owasp.org/www-project-top-ten/)
- [HIPAA Security Rule](https://www.hhs.gov/hipaa/for-professionals/security/laws-regulations/)

### 아키텍처
- [Microsoft Architecture Guide](https://docs.microsoft.com/en-us/azure/architecture/)
- [DDD Patterns](https://martinfowler.com/tags/domain%20driven%20design.html)

### DevExpress
- [DevExpress Documentation](https://docs.devexpress.com/)
- [DevExpress WinForms](https://docs.devexpress.com/WindowsForms/)
