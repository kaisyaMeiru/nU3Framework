  🎯 의료 시스템 UI Framework 엔터프라이즈 구축 완전 가이드

  - 언어: C#
  - Platform: Winform
  - Framework : .net 8
  - 패턴 : Code-behind 기본 코드 구조
  - UI Component: DevExpress WinForm 25.2.3 (nuGet DevExpress.Win, DevExpress.Win.Design)
  - DevExpress Template Kit for Visual Studio
  (In Visual Studio, select Extensions → Manage Extensions… to run Extension Manager. Type DevExpress Template Kit for Visual Studio in the search box and click Install)



  🎯 의료 시스템 UI Framework 엔터프라이즈 개발을 위해 가이드를 작성하려고 합니다.

  -실제로 대형 병원에 사용할 프레임워크를 설계하고 구현하려고 하므로 필수 고려사항 중 빠진 부분이나 보충해야하는 것을 설명해주세요.
  #백엔드는 Spring 및 Oracle DB가 기본적으로 사용되고 이미 완료된 상태라고 가정합니다.
  #프론트엔드는 WinForm + DevExpress Component를 활용하여 화면을 구성할 예정입니다.
  #백엔드와 프론트엔드 간의 DataLayer는 미정이므로 이 부분에 대한 검증 및 가이드가 필요합니다.
  #프론트엔드에서 UI Framework를 잘 갖춰진 구성 및 구조를 만들기 위해 가이드가 필요합니다.

  ┌────────────────────────────────────────┐
  │   의료 EMR 시스템 권장 아키텍처              │
  ├────────────────────────────────────────┤
  │                                              │
  │  UI Layer: WinForm + DevExpress              │
  │  └─ GridControl, XtraForm, etc.             │
  │                                              │
  │                                              │
  │  Services: REST API (async/await)           │
  │  └─ PatientService, DoctorService, etc.    │
  │                                              │
  │  Backend: Spring Boot (Java)                │
  │  └─ REST API, JPA, MySQL                   │
  │                                              │
  └────────────────────────────────────────┘


  🔧 기술적 측면 (13개)
  | #  | 항목        | 핵심 내용                                |
  | -- | --------- | ------------------------------------ |
  | 1  | 네트워크 통신   | HTTP/2, Gzip 압축, 연결 풀, 프록시 지원        |
  | 2  | 성능 최적화    | 백그라운드 로드, 페이징, Lazy Loading, 메모리 최적화 |
  | 3  | 캐싱 전략     | 3단계 캐싱(메모리/Redis/DB), 무효화 전략         |
  | 4  | 오프라인 모드   | SQLite 로컬 DB, 연결 감지, 자동 동기화          |
  | 5  | 국제화       | 다국어, 날짜 포맷, 통화 설정                    |
  | 6  | 테마/UI     | 동적 테마 변경, 다크 모드 자동 감지                |
  | 7  | 보안        | 비밀번호 정책, 세션 타임아웃, SSL/TLS            |
  | 8  | 데이터 동기화   | 양방향 동기화, 충돌 해결                       |
  | 9  | 사용자 선호도   | 창 위치/크기, GridLayout, 최근 검색어 저장       |
  | 10 | 검색 기능     | 고급 검색, 자동완성, 페이징                     |
  | 11 | 인쇄/Export | Excel, PDF, CSV 내보내기                 |
  | 12 | 실시간 알림    | SignalR, WebSocket, Toast 알림         |
  | 13 | 모니터링      | CPU/메모리/디스크 모니터링, 임계값 경고             |

  📊 운영 측면 (13개)
  | #  | 항목        | 핵심 내용                    |
  | -- | --------- | ------------------------ |
  | 14 | 마이그레이션    | 레거시 데이터 변환, 검증, 일괄 처리    |
  | 15 | API 버전 관리 | 버전 호환성 검증                |
  | 16 | 재시도 로직    | Polly, 서킷 브레이커, Timeout  |
  | 17 | 데이터 검증    | FluentValidation, 복합 검증  |
  | 18 | 병렬 처리     | 배치 처리, 대량 작업, 스레드 안전     |
  | 19 | 접근성       | 스크린 리더, 색상 대비, Tab order |
  | 20 | 감정 분석     | 기능 사용 추적, 성능 메트릭 수집      |
  | 21 | 라이선스 관리   | 컴퓨터 고유 ID, 유효기간 검증       |
  | 22 | 멀티 사이트    | 여러 병원 지원, 동적 API URL 변경  |
  | 23 | 기술 지원     | 내장 도움말, 콘텍스트 도움말         |
  | 24 | 규제 준수     | 개인정보 보호, 5년 보관, 암호화      |
  | 25 | 사용자 온보딩   | 첫 실행 마법사, 기본 설정          |
  | 26 | 백그라운드 작업  | 비동기 작업 관리, 진행률 보고        |


  - 사용자 권한관리 (화면 오픈 및 저장,조회,수정등 )
  - 모듈관리 (배포 및 업데이트 관리도구)
  - 메뉴관리
  - 프로그램 마스터
  - 로그 관리
  - Shell (Mdi, Sdi)
  - 패치 및 런쳐
  - Data Access Layer
  - 의료 시스템 특성 상, 화면이 많고 많은 개발자가 동시에 개발을 하기 때문에 DLL 참조 관계가 엮이면 안됨.
  Reflection 기능을 활용하여 최대한 DLL을 동적으로 불러와서 사용할 수 있어야 하며 사용자 별로 화면 내의 권한을 관리할 수 있는 구조여야 함.
  만약, 화면을 로드 중에 화면이 업데이트된 DLL이 있는 경우에는 업데이트하여 화면을 불러와야 하는 경우도 있음.
  24시간 운영되는 업무화면의 경우, 실시간으로 업데이트되어 최신 화면을 불러와야 함.

  - 하나의 업무화면에서 다른 업무화면을 호출하는 경우도 있는데,  이 때 환자 정보를 전달하거나 받을 수 있도록 하는 IPC 기능이 필요할 것으로 보임.
    (shell to shell 또는 screen to screen)

  - Serial/USB/카드리더기 하드웨어 연동
  - 알림 및 화면 메세지 ,  외부 메세지 연동
  - X-Platform to Winform Project Converter



  대규모 의료 시스템(EMR)의 UI Framework 설계는 **안정성(Patient Safety)**, **확장성(Scalability)**, **배포 유연성(Deployment)**이 가장 중요합니다. 작성해주신 초안은 기능적 요구사항을 잘 망라하고 있으나, **구조적 아키텍처(Architecture)**와 **데이터 통신 전략(Integration)**, 그리고 **하드웨어 제어** 측면에서 보완해야 할 중요한 포인트들이 있습니다.

  특히 **"DLL 참조 관계를 끊고(Decoupling)", "실시간 업데이트(Hot-swapping/Shadow Copy)", "Data Layer 검증"**에 대한 구체적인 가이드를 중심으로 보완 사항을 정리해 드립니다.

  ---

  ### 1. Data Access Layer (DAL) 검증 및 가이드

  **현재 상태:** 백엔드(Java/Spring)와 프론트엔드(C#/WinForm) 사이의 연결 고리가 미정입니다.
  **핵심 문제:** Java의 DTO 구조와 C#의 Model 구조가 다르며, UI 코드에서 직접 REST API를 호출하면 유지보수가 불가능해집니다.

  #### ✅ 보완 전략: C# 측 Service Agent Layer 도입

  UI 개발자가 HTTP 통신 코드를 직접 작성하지 않도록 추상화해야 합니다.

  * **구조 제안:** `UI (Presenter)` → `Service Agent (Interface)` → `Infrastructure (HTTP Client)` → `Spring Boot`
  * **기술 스택:**
  * **Refit** 또는 **RestSharp**: REST API를 C# Interface로 선언형으로 관리 (생산성 향상).
  * **DTO Mapper**: Java의 `snake_case` JSON을 C#의 `PascalCase` 프로퍼티로 자동 매핑 (예: `Newtonsoft.Json` 설정 또는 `AutoMapper`).


  * **필수 기능:**
  * **Global Exception Handler:** HTTP 401(토큰 만료), 500(서버 에러) 발생 시 공통 로직 처리 (예: 토큰 재발급 후 재요청).
  * **Request Interceptor:** 모든 요청 헤더에 인증 토큰(JWT) 및 클라이언트 버전 정보 자동 주입.


  ---

  ### 2. 아키텍처 및 모듈화 (DLL 참조 끊기)

  **요구사항:** 많은 개발자가 동시에 개발, DLL 참조 관계 제거, 동적 로딩.

  #### ✅ 보완 전략: 플러그인 아키텍처 (Plugin Architecture) & DI Container

  단순한 Reflection 사용을 넘어, **의존성 주입(Dependency Injection)** 컨테이너를 활용해야 합니다.

  * **Core Interface 정의:** `IModule`, `IMenu`, `IPatientContext` 등 공통 인터페이스만 정의된 `Core.dll`을 모든 개발자가 참조합니다.
  * **Dependency Injection (DI):** `Autofac` 또는 `Microsoft.Extensions.DependencyInjections` 사용.
  * Shell은 구체적인 화면 DLL을 참조하지 않고, 실행 시점에 특정 폴더(`Modules/`)에 있는 DLL을 스캔하여 메모리에 로드합니다.


  * **화면 식별 체계:** 모든 화면(Form)은 고유 ID(ScreenID)를 가지며, DB에 메타데이터(DLL 경로, Class Name, 권한)로 관리되어야 합니다.

  ---

  ### 3. 배포 및 업데이트 전략 (Hot Deployment)

  **요구사항:** 24시간 운영, 화면 로드 중 업데이트 감지, DLL 잠금(Locking) 방지.

  #### ✅ 보완 전략: Shadow Copying & Versioning

  Windows OS 특성상 실행 중인 DLL은 덮어쓸 수 없습니다. 이를 해결하기 위한 전략입니다.

  1. **Shadow Copying (섀도 카피):**
  * 애플리케이션 실행 시, 원본 DLL 폴더를 사용하지 않고, 사용자 PC의 임시 폴더(Temp)로 DLL을 복사한 후 로드합니다.
  * 이렇게 하면 원본 DLL 파일은 잠기지 않으므로, 백그라운드에서 패치 다운로드가 가능합니다.


  2. **Module Versioning Loader:**
  * 화면을 여는 순간(Menu Click), 로컬의 DLL 버전과 서버의 DLL 버전을 비교합니다.
  * 서버 버전이 높으면 해당 DLL만 다운로드 → Shadow Copy 폴더로 복사 → `Assembly.LoadFile`로 메모리 로드 → 화면 출력.


  3. **AppDomain 분리 (선택 사항):**
  * 과거에는 `AppDomain`을 나누어 DLL을 언로드했으나, .NET Core/5+ 환경에서는 `AssemblyLoadContext`를 사용하여 특정 어셈블리만 언로드/리로드 하는 구조를 고려할 수 있습니다. (구현 난이도 높음, Shadow Copying 권장)



  ---

  ### 4. 프로세스 간 통신 (IPC & Context Management)

  **요구사항:** Shell to Shell, Screen to Screen 데이터 전달 (환자 정보 등).

  #### ✅ 보완 전략: Event Aggregator & Patient Context Manager

  직접적인 객체 참조 없이 메시지를 주고받아야 합니다.

  1. **Event Aggregator (Pub/Sub 패턴):**
  * A화면이 "환자 변경 이벤트"를 발행(Publish)하면, B, C화면이 이를 구독(Subscribe)하여 자동으로 데이터를 갱신합니다.
  * Prism Library의 `EventAggregator` 패턴을 WinForm에 맞게 구현하여 사용합니다.


  2. **Global Context Manager:**
  * **CurrentPatient (선택된 환자)**, **CurrentUser (로그인 의사)** 등의 전역 상태를 관리하는 싱글톤 서비스입니다.
  * Shell은 2개 이상(멀티 프로세스) 실행될 경우, **Named Pipes** 또는 **gRPC** (Localhost)를 사용하여 메인 Shell과 서브 Shell 간의 환자 정보를 동기화해야 합니다.



  ---

  ### 5. 의료 시스템 특화 추가 고려사항 (Missing Parts)

  의료 현장의 특수성을 고려하여 다음 항목들을 설계 가이드에 추가해야 합니다.

  | # | 항목 | 핵심 내용 및 고려사항 |
  | --- | --- | --- |
  | **27** | **로컬 하드웨어 통합** | EMR은 바코드 스캐너, 카드리더기, 듀얼 모니터, PACS 연동이 필수입니다. <br>
  <br> - **Serial/USB Port 모니터링 모듈** 필요 <br>
  <br> - 하드웨어 제어 로직을 별도 `Device Service`로 분리하여 UI 스레드 멈춤 방지 |
  | **28** | **UI 스레드 방어 (Freezing)** | 의사가 차트를 조회할 때 화면이 멈추면 안 됩니다. <br>
  <br> - DevExpress `GridControl`의 **Server Mode** 또는 **Instant Feedback Mode** 필수 적용 가이드 (대용량 데이터 비동기 바인딩) |
  | **29** | **사용자 정의 매크로** | 의사들은 반복되는 텍스트(상용구)를 매우 많이 사용합니다. <br>
  <br> - 텍스트 에디터 내 **상용구(Auto-text) 기능** 및 단축키 바인딩 프레임워크 |
  | **30** | **Tab & Workspace 관리** | 한 번에 여러 환자 차트를 열어두고 탭으로 오가는 경우가 많습니다. <br>
  <br> - 메모리 누수 방지를 위한 **Tab 닫기 시 리소스 해제(Dispose) 강제화** 로직 |
  | **31** | **오류 보고 시스템** | 프로그램 충돌 시 **자동 덤프(Dump) 생성** 및 서버 전송 기능. (Stack Trace + 당시 입력 파라미터 캡처) |
  | **32** | **Form Converter** | 기존 솔루션(X-Platform 등)을 WinForm으로 변환할 때 100% 자동은 불가능합니다. <br>
  <br> - **Layout XML 파서**를 만들어 DevExpress 컨트롤로 1차 변환해주는 도구 개발 필요 |

  ---

  ### 6. 수정된 아키텍처 다이어그램 제안

  기존 다이어그램에 **Module Loader, Event Bus, Service Agent**를 명시하여 구조를 구체화했습니다.

  ```text
  ┌──────────────────────────────────────────────────────────────┐
  │  Client Workstation (Doctor's PC)                            │
  ├──────────────────────────────────────────────────────────────┤
  │  [ Bootstrapper & Launcher ]                                 │
  │   - Version Check & Shadow Copy (Local Cache)                │
  ├───────────────────────┬──────────────────────────────────────┤
  │  [ Shell (Main App) ] │  IPC (Named Pipes / gRPC)            │
  │                       ▼                                      │
  │  ┌────────────────────────────────────────────────────────┐  │
  │  │ UI Layer (Plugin Architecture)                         │  │
  │  │ ┌──────────────┐  ┌──────────────┐  ┌──────────────┐   │  │
  │  │ │ Outpatient   │  │ Admission    │  │ Nursing      │   │  │
  │  │ │ (DLL A)      │  │ (DLL B)      │  │ (DLL C)      │   │  │
  │  │ └──────┬───────┘  └──────┬───────┘  └──────┬───────┘   │  │
  │  │        │                 │                 │           │  │
  │  │   [ Presenter / ViewModel (Business Logic) ]           │  │
  │  │        │                 │                 │           │  │
  │  └────────┼─────────────────┼─────────────────┼───────────┘  │
  │           │                 │                 │              │
  │  ┌────────▼─────────────────▼─────────────────▼───────────┐  │
  │  │ Core Framework (Common DLL)                            │  │
  │  │ - Event Aggregator (Screen-to-Screen Msg)              │  │
  │  │ - Patient Context Manager (Global State)               │  │
  │  │ - Device Manager (Scanner, Reader)                     │  │
  │  └──────────────────────────┬─────────────────────────────┘  │
  │                             │                                │
  │  ┌──────────────────────────▼─────────────────────────────┐  │
  │  │ Data Access Layer (Service Agent)                      │  │
  │  │ - RestClient (Refit/RestSharp)                         │  │
  │  │ - DTO Mapper & Validation                              │  │
  │  │ - Offline Local DB (SQLite)                            │  │
  │  └──────────────────────────┬─────────────────────────────┘  │
  │                             │ HTTPS / JSON                   │
  └─────────────────────────────┼────────────────────────────────┘
                                ▼
  ┌──────────────────────────────────────────────────────────────┐
  │  Server Side                                                 │
  │  [ API Gateway / Load Balancer ]                             │
  │  [ Spring Boot Backend ] <---> [ Oracle DB ]                 │
  └──────────────────────────────────────────────────────────────┘

  ```

  ### 7. 다음 단계 제안 (Action Plan)

  현재 단계에서 가장 우선적으로 정해야 할 것은 **"통신 표준"**와 **"배포 구조"**입니다. 다음의 순서로 가이드를 구체화하시길 권장합니다.

  1. **Base Interface 정의:** `IForm`, `IService`, `IContext` 등 모든 개발자가 상속받아야 할 기본 클래스 설계.
  2. **프로토타입 개발:**
  * 로그인 → 메인 프레임 로드 → 메뉴 클릭 시 **서버에서 DLL 다운로드 후 실행(Shadow Copy)** → 화면에 데이터 바인딩까지의 전체 사이클을 검증하는 "Vertical Prototype"을 먼저 만드십시오.


  3. **DevExpress 테마 및 표준 스타일 가이드:** 그리드, 버튼, 폰트 등의 통일성을 위한 `BaseControl` 상속 구조 정의.

  **혹시 "Shadow Copying을 활용한 배포 로직"이나 "Event Aggregator의 구체적인 구현 예시(C#)"가 필요하시다면 말씀해주세요.**
