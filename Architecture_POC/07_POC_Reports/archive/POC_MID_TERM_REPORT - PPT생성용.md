# nU3.Framework POC 중간 보고서
**차세대 의료시스템 프레임워크 아키텍처 검토**
> 프로젝트: nU3.Framework v9.0
---

## 📋 목차

1. [POC 구현 개념 및 설명](#1-poc-구현-개념)
2. [개발적 요소](#2-개발적-요소)
3. [개선해야 할 사항 및 추후 필수 구현 사항](#3-개선해야-할-사항-및-추후-필수-구현-사항)
4. [로드맵 및 계획](#4-로드맵-및-계획)

---

# 1. POC 구현 개념 및 설명

## 1.1 프로젝트 개요

### 배경 및 목적
nU3.Framework는 대형 병원 환경을 위한 차세대 의료시스템 EMR(Electronic Medical Record) 플랫폼으로 설계되었습니다. SI(System Integration) 프로젝트의 특성상 다수의 개발자가 동시에 별도 업무시스템을 개발해야 하며, 각 시스템은 독립적이면서도 상호 연계성을 확보해야 합니다.

### 핵심 요구사항
- **독립적 모듈 개발**: 다수 개발자가 동시에 개발 가능하도록 DLL 참조 관계 최소화
- **동적 로딩**: Reflection 기반의 동적 모듈 로딩 시스템
- **실시간 업데이트**: 24시간 운영 중인 시스템의 핫-디플로이(Hot Deployment) 지원
- **IPC 통신**: Shell 간, Screen 간 환자 정보 등 데이터 전달
- **하드웨어 연동**: Serial/USB/카드리더기 연동

### 기술 스택
| 카테고리 | 기술 | 버전 |
|---------|------|------|
| **Framework** | .NET | 8.0 |
| **UI Platform** | WinForms | - |
| **UI Components** | DevExpress | 23.2.9 |
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
   - 메인 Shell과 서브 Shell 간 환자 정보 동기화( Named Pipes )

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
제공된 nU3.Framework POC 중간 보고서를 바탕으로, **실제 업무 화면을 개발해야 하는 개발자(Application Developer)**의 관점에서 당장 무엇이 부족하며, 이를 어떻게 확인하고 진행해야 하는지 구체적으로 정리해 드립니다.
현재 프레임워크는 '집을 짓기 위한 기초 공사(아키텍처, 배포, 통신)'는 끝났으나, '인테리어와 가구(업무 화면 기능)'를 채워 넣기 위한 도구와 부품이 부족한 상태입니다.

| 카테고리 | 기능 | 상태 |
|---------|------|------|
| **보안** | UserSession 기본 구현 | 🚧 진행 중 |
| **보안** | 권한 제어 (AuthLevel) | 🚧 진행 중 |
| **서버** | ASP.NET Core API 서버 | 🚧 진행 중 |
| **서버** | Spring Boot 연동 | ⏳ 계획 중 |
카테고리,기능 (세부 항목),상태,비고
공통 UI,표준 공통 팝업 (환자/부서/상병/수가 검색),🚧 진행 중,개발자용 표준 부품 (Bricks)
공통 UI,그리드 표준 래퍼 (DevExpress Grid 확장),🚧 진행 중,헤더/페이징/엑셀 출력 표준화
공통 UI,표준 레이아웃 템플릿 (조회/입력/저장 패턴),🚧 진행 중,BaseWorkControl 기능 확장
데이터,표준 CRUD 코딩 패턴 가이드,🚧 진행 중,개발자용 Reference 패턴 (Plumbing)
데이터,복합 트랜잭션 처리 매커니즘,🚧 진행 중,다중 테이블 저장 로직 표준화
데이터,기준 정보(Master Data) 캐싱 전략,🚧 진행 중,로컬 SQLite vs 서버 API 조회 기준 수립
보안,UserSession & 권한 제어 (RBAC),🚧 진행 중,AuthLevel 기반 버튼/메뉴 제어
서버,ASP.NET Core API 서버 인프라,🚧 진행 중,Backend 연동 모듈 최적화
연동,레거시 호환 래퍼 (OCX / 기존 리포트),🚧 진행 중,구시스템 모듈 수용 (Bridge)
연동,주변기기 인터페이스 (바코드/카드리더기),🚧 진행 중,하드웨어 통신 공통화
개발환경,샘플 참조 모듈 (Reference Module),🚧 진행 중,"""복사해서 쓰기"" 가능한 표준 소스"
개발환경,로컬 디버깅 및 Mock 데이터 환경,🚧 진행 중,서버 독립적 개발 환경 구축 (Manual)


### ❌ 미구현 기능 (Phase 3+)  
의료 표준 및 외부 연동 확장

카테고리,기능 (세부 항목),상태,비고
|---------|------|------|------|
의료 표준,HL7 v2/v3 & FHIR 인터페이스,⏳ 계획 중,국내외 의료 정보 교환 표준 준수
의료 표준,DICOM 기반 PACS 연동 모듈,⏳ 계획 중,의료 영상 저장 및 조회 연동
외부 연동,검사장비(LIS) 데이터 인터페이스,⏳ 계획 중,검사 결과 수신 및 자동 매핑
외부 연동,보험청구(EDI) 시스템 연동,⏳ 계획 중,진료비 청구 및 심사 결과 수신
데이터,백엔드 Data Access Layer (DAL) 완성,⏳ 계획 중,Oracle 서버 사이드 비즈니스 로직 최적화
데이터,통합 기준정보 관리 (MDM),⏳ 계획 중,전사적 코드/마스터 데이터 관리 체계
품질/테스트,단위 테스트 (Unit Test) 프레임워크,⏳ 계획 중,핵심 비즈니스 로직 무결성 검증
품질/테스트,통합 테스트 및 자동화 시나리오,⏳ 계획 중,화면 간 연동 및 시스템 전체 프로세스 점검
품질/테스트,레거시 모듈 호환성 전수 검토,⏳ 계획 중,기존 OCX/DLL 전환 안정성 테스트
보안,SSO & 생체 인증 연동,⏳ 계획 중,사용자 편의성 및 보안 강화
통계/분석,데이터 대시보드 및 리포팅 툴,⏳ 계획 중,병원 경영 및 임상 데이터 통계 시각화

---

# 2. 개발적 요소 

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
| SQLite | POC 용 데이터베이스  |
| Oracle | 중앙 데이터베이스 (백엔드) |

### 2.1.4 네트워크 및 통신
| 기술 | 용도 |
|------|------|
| System.Net.Http | HTTP 통신 |
| System.Net.Http.Json | JSON 직렬화/역직렬화 |
| Named Pipes | 로컬 IPC (계획) |

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


---

# 3. 개선해야 할 사항 및 추후 필수 구현 사항 

## 3.1 외부 시스템 연동 (P1 - ESSENTIAL)
| 기능 | 우선순위 | 설명 |
|------|---------|------|
| 개발자 공통 Componet | P0 | 공통코드조회창 등 |
| 보험청구 시스템 연동 | P1 | EDI 837 청구, 835 수령 |
| 검사장비(LIS) 연동 | P1 | 검사결과 수신 (ASTM, HL7) |
| 수술장비(ORIS) 연동 | P1 | 수술 스케줄링, 장비 통합 |
| 레거시 모듈 연동  | P1 | OCX, 레포트등 기타  |
| HL7 v2/v3 메시지 처리 | P1 | ADT, ORM, ORU 등 HL7 메시지 파싱/생성 |
| DICOM PACS 연동 | P1 | 의료 영상 저장/조회 (Q/R SCP) |
| 웹훅 | P2 | 외부 시스템 알림 |

## 3.2 배포 & DevOps (P1 - HIGH)

---

# 4. 로드맵 및 계획

## 4.1 우선순위 매트릭스

### P0 - CRITICAL (즉시 구현 필요)
| 항목 | 예상 소요시간 | 비용 | 영향도 |
|------|------------|------|--------|
| 공통모듈 | 4-6주 | 높 | 높음 |
| 기준정보 관리  | 3-5주 | 중 | 매우 높음 |
| 서버연동 | 2-3주 | 낮 | 매우 높음 |

### P1 - HIGH (다음 3개월 내)
| 항목 | 예상 소요시간 | 비용 | 영향도 |
|------|------------|------|--------|
| 보안 (JWT, RBAC, 암호화) | 4-6주 | 중 | 매우 높음 |
| 의료 전문 기능 (HL7, FHIR, DICOM) | 6-8주 | 높 | 매우 높음 |
| 외부 시스템 연동 (LIS, ORIS, 보험) | 4-6주 | 중 | 높음 |
| 모니터링 & 로그 | 3-4주 | 중 | 높음 |
| 배포 자동화 | 3-5주 | 중 | 높음 |
| 성능 최적화 (쿼리, 캐싱) | 2-4주 | 낮 | 높음 |
| 비동기 프로그래밍 | 2-3주 | 낮 | 높음 |

### P2 - MEDIUM (6개월 이내)
| 항목 | 예상 소요시간 | 비용 | 영향도 |
|------|------------|------|--------|
| 문서화 (API, 아키텍처) | 4-6주 | 낮 | 중 |
| DI 완전 구현 | 2-3주 | 낮 | 중 |
| 메시지 버스 (MassTransit) | 3-4주 | 중 | 중 |


## 4.3 구현 체크리스트

### 보안
- [ ] JWT 인증 서비스 구현 
- [ ] SSO 서비스
- [ ] 세션 관리 (타임아웃, 재발급(Refresh 토큰 검토))
- [ ] 권한 서비스
- [ ] 감사 로그 (민감 정보 접근)
- [ ] HTTPS 강제 (Production)

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
- [ ] PACS 연동
- [ ] 보험청구 시스템 (EDI 837)
- [ ] 웹훅

### 문서화
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
1. **개발자 편의 컴포넌트 개발 및 제공
2. **데이터 관리 (P0)**: 마이그레이션, 백업, 트랜잭션
3. **의료 표준 (P1)**: HL7, FHIR, DICOM, 약물 상호작용
4. **외부 연동 (P1)**: LIS, PACS, ORIS, 보험청구
5. **배포 (P1) 자동화 및 형상관리 **

### 추천 로드맵
약 **10개월**의 계획된 로드맵을 통해 이러한 부족한 기능들을 단계적으로 구현하면, nU3.Framework는 **안전하고, 규정 준수하며, 확장 가능한 대형 의료시스템 프레임워크**로 성장할 수 있을 것입니다.

### 성공 요인
1. **테스트 주도 개발**: 테스트 인프라 먼저 구축 후 기능 개발
2. **보안 우선**: 의료 민감 정보 보호를 위한 보안 레이어 우선 구현
3. **표준 준수**: HL7, FHIR, HIPAA 등 의료 표준 준수
4. **자동화**: 테스트, 배포 자동화로 개발 효율성 확보
5. **문서화**: API, 아키텍처, 배포 가이드 문서화

### 최종 권고
nU3.Framework는 **잘 설계된 기반**을 가지고 있으며, 우선순위에 따라 부족한 기능들을 체계적으로 구현하면 **차세대 의료시스템의 핵심 플랫폼**으로 성장할 수 있습니다. 특히 보안, 테스트, 의료 표준 준수가 성공의 핵심 요소입니다.

---
