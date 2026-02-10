# 시각적 아키텍처 문서
## nU3.Framework 시스템 아키텍처

**버전:** 1.0
**날짜:** 2026년 2월
**형식:** Mermaid 다이어그램 (GitHub, VS Code, Mermaid Live Editor에서 렌더링)

---

## 📋 목차

1. [시스템 컨텍스트 다이어그램](#시스템-컨텍스트-다이어그램)
2. [컨테이너 다이어그램](#컨테이너-다이어그램)
3. [컴포넌트 다이어그램](#컴포넌트-다이어그램)
4. [시퀀스 다이어그램](#시퀀스-다이어그램)
5. [배포 다이어그램](#배포-다이어그램)
6. [데이터 흐름 다이어그램](#데이터-흐름-다이어그램)
7. [모듈 로딩 흐름](#모듈-로딩-흐름)
8. [업데이트 배포 흐름](#업데이트-배포-흐름)

---

## 시스템 컨텍스트 다이어그램

외부 엔티티와 nU3.Framework 시스템 간의 상호 작용을 보여줍니다.

```mermaid
graph TB
    subgraph "의료 시스템"
        direction LR
        HL7[HL7 V2.x 게이트웨이]
        FHIR[FHIR 서버]
        DICOM[DICOM PACS]
        LIS[LIS 검사실]
        INSURANCE[보험 EDI]
    end

    subgraph "nU3.Framework"
        direction TB
        Shell[nU3.Shell]
        Core[nU3.Core]
        Connectivity[nU3.Connectivity]
        Data[(Oracle DB)]
        LocalDB[(SQLite)]
    end

    subgraph "사용자"
        Doctor[의사]
        Nurse[간호사]
        Admin[시스템 관리자]
    end

    Doctor -->|주문| Shell
    Nurse -->|진료 기록| Shell
    Admin -->|구성| Shell

    Shell -->|사용자 동작| Core
    Core -->|데이터 요청| Data
    Core -->|구성| LocalDB

    Connectivity -->|SQL 쿼리| Data
    Connectivity -->|파일 전송| LIS
    Connectivity -->|로그 업로드| INSURANCE

    HL7 -->|ADT 메시지| Connectivity
    FHIR -->|리소스| Connectivity
    DICOM -->|이미지| Connectivity
```

**범례:**
- **실선 화살표:** 직접 통신
- **파선 화살표:** API/Web 서비스 호출
- **데이터베이스:** SQLite (로컬) / Oracle (서버)

---

## 컨테이너 다이어그램

고차 수준 아키텍처 컨테이너 및 그 관계를 보여줍니다.

```mermaid
graph TB
    subgraph "nU3.Framework 플랫폼"
        direction TB

        subgraph "클라이언트 계층"
            Client[nU3.Client<br/>(WinForms 애플리케이션)]
        end

        subgraph "인프라 계층"
            Bootstrapper[nU3.Bootstrapper<br/>(배포 및 로딩)]
            Connectivity[nU3.Connectivity<br/>(HTTP 클라이언트)]
        end

        subgraph "코어 계층"
            Core[nU3.Core<br/>(기본 클래스 및 인터페이스)]
            Security[nU3.Security<br/>(JWT 및 RBAC)]
        end

        subgraph "애플리케이션 계층"
            Shell[nU3.Shell<br/>(MDI 컨테이너)]
            Modules[nU3.Modules.<br/>(비즈니스 모듈)]
        end

        subgraph "데이터 계층"
            LocalDB[(SQLite<br/>(구 configuration))]
            OracleDB[(Oracle<br/>(비즈니스 데이터))]
        end

        Client --> Bootstrapper
        Client --> Shell
        Client --> Connectivity

        Bootstrapper --> Core
        Connectivity --> Core
        Connectivity --> OracleDB

        Core --> Shell
        Shell --> Modules

        Core --> LocalDB
        Core --> Security
        Shell --> Security
    end

    style Client fill:#e1f5ff
    style Shell fill:#fff4e1
    style Modules fill:#ffe1e1
    style OracleDB fill:#e1ffe1
```

---

## 컴포넌트 다이어그램

상세 컴포넌트 보기 및 내부 아키텍처를 보여줍니다.

```mermaid
classDiagram
    class BaseWorkControl {
        <<interface>>
        +string ScreenId
        +string ScreenTitle
        +OnActivated()
        +OnDeactivated()
        +OnBeforeClose()
        +RegisterDisposable(IDisposable)
        +ReleaseResources()
        +HasPermission(permission)
    }

    class nU3ProgramInfo {
        +Type ControlType
        +string DisplayTitle
        +string ProgramId
        +string ModuleCategory
    }

    class EventAggregator {
        +Publish~T~(payload)
        +Subscribe~T~(Action~T~)
        +Unsubscribe~T~(Action~T~)
    }

    class UserSession {
        +Authenticate(username, password)
        +ValidateToken(token)
        +GetCurrentUser() User
        +GetPermissions() List~Permission~
    }

    class ConnectivityManager {
        +GetDbClient() HttpDBAccessClient
        +GetFileClient() HttpFileTransferClient
        +GetLogClient() HttpLogUploadClient
    }

    class ModuleLoader {
        +LoadModule(moduleId) Assembly
        +UnloadModule(moduleId)
        +GetLoadedModules() List~ModuleInfo~
    }

    BaseWorkControl <|-- BaseWorkForm
    BaseWorkControl <|-- PatientListControl

    nU3ProgramInfo --> BaseWorkControl : 정의

    EventAggregator ..> EventAggregator : 이벤트 버스

    UserSession --> EventAggregator : 인증 이벤트 발행
    UserSession --> ConnectivityManager : 관리

    ConnectivityManager --> ConnectivityManager : 여러 HTTP 클라이언트 관리
    ModuleLoader --> EventAggregator : 모듈 로드/언로드 알림
    ModuleLoader --> BaseWorkControl : 모듈 인스턴스화
```

---

## 시퀀스 다이어그램

### 모듈 로딩 시퀀스

사용자가 메뉴 항목을 클릭하고 모듈이 동적으로 로드되는 방법을 보여줍니다.

```mermaid
sequenceDiagram
    participant User as 👤 사용자
    participant Shell as nU3.Shell
    participant Menu as 메뉴 시스템
    participant Loader as ModuleLoader
    participant DI as DI 컨테이너
    participant Module as 모듈 인스턴스
    participant DB as SQLite

    User->>Shell: 메뉴 항목 클릭
    Shell->>Menu: GetMenuItem(menuId)
    Menu-->>Shell: PROG_ID 반환

    Shell->>DB: SYS_MENU에서 PROG_ID 조회
    DB-->>Shell: 모듈 메타데이터 반환

    Shell->>DB: SYS_MODULE_VER에서 버전 조회
    DB-->>Shell: 버전 및 파일 해시 반환

    alt 모듈 미로딩
        Shell->>Loader: LoadModule(moduleId)
        Loader->>DI: CreateModule(assembly)
        DI-->>Module: BaseWorkControl 인스턴스 반환
        Module->>Module: OnActivated()
        Module-->>Shell: ModuleActivatedEvent
        Shell->>User: MDI 탭에 모듈 표시
    else 모듈 이미 로드됨
        Shell->>Module: 기존 인스턴스 표시
        Module->>Module: OnActivated()
    end

    Note over Module: 비즈니스 로직 실행...
```

### 이벤트 기반 통신

이벤트 에그리게이터를 사용한 모듈 간 통신을 보여줍니다.

```mermaid
sequenceDiagram
    participant ModA as 모듈 A<br/>(환자 목록)
    participant EventBus as EventAggregator
    participant ModB as 모듈 B<br/>(검사 주문)
    participant ModC as 모듈 C<br/>(방사선)
    participant ModD as 모듈 D<br/>(약국)

    ModA->>EventBus: PatientSelectedEvent{patient} 발행
    EventBus->>EventBus: 구독자에 라우팅

    EventBus->>ModB: OnPatientSelected(patient)
    ModB->>ModB: 환자별 검사 주문 로드

    EventBus->>ModC: OnPatientSelected(patient)
    ModC->>ModC: 환자별 방사선 이미지 로드

    EventBus->>ModD: OnPatientSelected(patient)
    ModD->>ModD: 약물 상호작용 확인

    Note over ModB,ModD: 모든 모듈이 환자 선택에<br/>자동으로 반응
```

### 업데이트 배포 흐름

시스템이 모듈을 업데이트하며 재시작 없이 작동하는 방법을 보여줍니다.

```mermaid
sequenceDiagram
    participant User as 👤 사용자
    participant Shell as nU3.Shell
    participant Bootstrap as Bootstrapper
    participant Server as 서버 저장소
    participant Cache as 스테이징 캐시
    participant SQLite as SQLite DB

    User->>Shell: 애플리케이션 시작
    Shell->>Bootstrap: 업데이트 확인

    Bootstrap->>Server: GET /api/modules/versions
    Server-->>Bootstrap: 최신 버전 반환

    Bootstrap->>SQLite: SYS_MODULE_VER 쿼리
    SQLite-->>Bootstrap: 로컬 버전 반환

    loop 각 모듈에 대해
        Bootstrap->>Bootstrap: 버전 비교
        alt 새 버전 있음
            Bootstrap->>Server: 새 DLL 다운로드
            Server-->>Bootstrap: DLL 파일 반환

            Bootstrap->>Cache: 스테이징 캐시에 저장
            Note right of Cache: SHA256 해시 검증

            Bootstrap->>SQLite: 버전 업데이트
            Bootstrap->>Shell: 업데이트 완료 알림
        end
    end

    Shell->>User: "업데이트가 성공적으로 적용되었습니다."
    Note over Shell: 다음 실행 시 새 버전 로드
```

---

## 배포 다이어그램

물리적 배포 아키텍처를 보여줍니다.

```mermaid
graph TB
    subgraph "병원 네트워크"
        direction LR

        subgraph "워크스테이션 (여러 대)"
            WS1[WinForms 워크스테이션 1]
            WS2[WinForms 워크스테이션 2]
            WS3[WinForms 워크스테이션 3]
        end

        subgraph "파일 서버"
            FS[공유 파일 서버<br/>(모듈, 구성)]
        end

        subgraph "애플리케이션 서버"
            AppServer[nU3.Server.Host<br/>(ASP.NET Core API)]
        end

        subgraph "데이터베이스 서버"
            OracleDB[(Oracle DB)]
            SQLite[(SQLite 로컬 구성)]
        end

        subgraph "아카이브 서버"
            Archive[아카이브 서버<br/>(모듈 아카이브)]
        end
    end

    WS1 --> FS
    WS2 --> FS
    WS3 --> FS

    WS1 -.->|HTTPS| AppServer
    WS2 -.->|HTTPS| AppServer
    WS3 -.->|HTTPS| AppServer

    AppServer --> OracleDB
    AppServer --> Archive
```

---

## 데이터 흐름 다이어그램

### 인증 흐름

사용자가 로그인하고 인증되는 과정을 보여줍니다.

```mermaid
sequenceDiagram
    participant UI as UI 계층
    participant Auth as UserSession
    participant DB as Oracle DB
    participant JWT as JWT 서비스
    participant TokenStore as 토큰 저장소

    UI->>Auth: Authenticate(username, password)

    Auth->>DB: 사용자 자격 증명 확인

    alt 사용자 존재
        DB-->>Auth: 사용자 데이터 반환
        Auth->>Auth: 비밀번호 해시 검증
        Auth->>JWT: GenerateToken(user)
        JWT-->>Auth: JWT 토큰 + 리프레시 토큰

        Auth->>TokenStore: 토큰 저장 (리프레시 토큰)
        Auth->>Auth: 세션 쿠키 설정

        Auth-->>UI: Return JWT + 사용자 정보
    else 사용자 미존재
        DB-->>Auth: 빈 결과
        Auth-->>UI: throw AuthenticationException
    end
```

### CRUD 작업 흐름

표준 데이터 액세스 패턴을 보여줍니다.

```mermaid
sequenceDiagram
    participant UI as UI 계층<br/>(BaseWorkControl)
    participant Service as Service Agent<br/>(IPatientServiceAgent)
    participant DAL as 데이터 액세스 계층<br/>(HttpDBAccessClient)
    participant Server as 서버 API
    participant DB as Oracle DB

    UI->>UI: 사용자가 "저장" 버튼 클릭

    UI->>Service: GetPatientByIdAsync(id)
    Service->>DAL: GET /api/patients/{id}
    DAL->>Server: HTTP GET
    Server->>DB: SELECT * FROM PATIENTS WHERE ID=?
    DB-->>Server: 환자 데이터 반환
    Server-->>DAL: JSON 응답
    DAL-->>Service: PagedResultDto<PatientListDto>
    Service-->>UI: 환자 상세 반환

    UI->>Service: UpdatePatientAsync(patientDto)
    Service->>DAL: POST /api/patients/{id}
    DAL->>Server: HTTP POST
    Server->>DB: UPDATE PATIENTS SET ...
    DB-->>Server: 영향된 행 반환
    Server-->>DAL: HTTP 200 OK
    DAL-->>Service: BaseResponseDto
    Service-->>UI: 성공 응답
```

---

## 모듈 로딩 흐름

메타데이터 발견 및 모듈 로딩의 단계별 프로세스를 보여줍니다.

```mermaid
graph TD
    Start[시작: 애플리케이션 시작] --> Scan[nU3ProgramInfo 속성 스캔]

    Scan --> Found[속성 발견]

    Found --> Filter{카테고리 필터링}

    Filter -->|일치| Load[AssemblyLoadContext를 통해 어셈블리 로드]
    Filter -->|불일치| Skip[모듈 건너뜀]

    Load --> Verify{무결성 검증}
    Verify -->|SHA256 불일치| Reject[모듈 거부<br/>파일 손상됨]
    Verify -->|OK| Cache[어셈블리 캐싱]

    Cache --> Resolve{의존성 확인}
    Resolve -->|의존성 OK| Register[DI 컨테이너 등록]
    Resolve -->|누락된 의존성| Fail[로딩 실패<br/>의존성 누락]

    Register --> Instantiate[모듈 인스턴스화]
    Instantiate --> Lifecycle[OnActivated() 호출]
    Lifecycle --> Complete[완료: 모듈 준비]
```

---

## 업데이트 배포 흐름

모듈 업데이트의 단계별 프로세스를 보여줍니다.

```mermaid
graph TD
    Start[시작: 업데이트 확인] --> Query[서버에서 최신 버전 쿼리]

    Query --> Compare{버전 확인}

    Compare -->|업데이트됨| Download[새 DLL 다운로드]
    Download --> Save[스테이징 캐시에 저장]
    Save --> Verify{SHA256 해시 검증}

    Verify -->|불일치| Error[오류: 파일 손상<br/>다시 다운로드]
    Verify -->|OK| Swap[실행 중인 버전과 교체]

    Swap --> UpdateDB[SQLite 버전 테이블 업데이트]
    UpdateDB --> Notify[애플리케이션 알림]
    Notify --> Check{의존성 확인}
    Check -->|OK| Success[업데이트 완료]
    Check -->|실패| Rollback[스테이징 롤백]
    Rollback --> Error

    Compare -->|최신 상태| Done[모듈 최신 상태]

    Done --> End[종료]
    Success --> End
    Error --> End
```

---

## 컴포넌트 관계

### 의존성 그래프

모듈 의존성을 보여줍니다.

```mermaid
graph LR
    subgraph "코어 인프라"
        Core[Core]
        Shell[Shell]
    end

    subgraph "비즈니스 모듈"
        Mod1[진료 모듈]
        Mod2[검사 모듈]
        Mod3[방사선 모듈]
        Mod4[약국 모듈]
        Mod5[EMR 문서화]
    end

    Core --> Mod1
    Core --> Mod2
    Core --> Mod3
    Core --> Mod4
    Core --> Mod5

    Mod1 --> Mod2
    Mod2 --> Mod3
    Mod3 --> Mod4

    Shell -.->|이벤트 버스| Mod1
    Shell -.->|이벤트 버스| Mod2
    Shell -.->|이벤트 버스| Mod3

    style Core fill:#ff9999
    style Shell fill:#99ccff
    style Mod1 fill:#99ff99
    style Mod2 fill:#99ff99
    style Mod3 fill:#99ff99
    style Mod4 fill:#99ff99
    style Mod5 fill:#99ff99
```

---

## 아키텍처 계층

### 5계층 아키텍처 다이어그램

```mermaid
graph TB
    subgraph "Layer 5: 데이터 계층"
        DataLayer[(Oracle DB)]
    end

    subgraph "Layer 4: 연결성 계층"
        Connect[nU3.Connectivity<br/>HTTP 클라이언트<br/>연결 풀링]
    end

    subgraph "Layer 3: 쉘 계층"
        Shell[nU3.Shell<br/>(MDI 컨테이너)<br/>모듈 로딩<br/>이벤트 에그리게이터]
    end

    subgraph "Layer 2: 코어 계층"
        Core[nU3.Core<br/>(BaseWorkControl)<br/>(nU3ProgramInfo)<br/>(EventAggregator)]
    end

    subgraph "Layer 1: 부트스트래퍼 계층"
        Boot[nU3.Bootstrapper<br/>(배포)<br/>(어셈블리 로딩)<br/>(버전 제어)]
    end

    Boot --> Core
    Core --> Shell
    Shell --> Connect
    Connect --> DataLayer
```

---

## 의료 표준 통합

### HL7 ADT 메시지 흐름

```mermaid
sequenceDiagram
    participant HIS as 병원 정보 시스템
    participant HL7[HL7 게이트웨이]
    participant Server as 서버 API
    participant DB as Oracle DB
    participant Patient as 환자 모듈

    HIS->>HL7: ADT^A01 (환자 입원)
    HL7->>HL7: 메시지 파싱

    HL7->>Server: POST /api/hl7/adt
    Server->>Server: 메시지 구조 검증
    Server->>DB: INSERT INTO PATIENTS (...)
    DB-->>Server: 성공
    Server-->>HL7: HTTP 200 OK
    HL7-->>HIS: ACK (승인)

    Server->>Patient: PatientCreatedEvent 발행
    Patient->>Patient: 환자 데이터 로드
    Patient-->>Server: 환자 로드 완료
```

### FHIR 리소스 매핑

```mermaid
graph LR
    subgraph "외부 시스템"
        FHIR[FHIR API]
        DICOM[DICOM 서버]
    end

    subgraph "nU3 Framework"
        Adapter[리소스 어댑터<br/>(Service)]
        Core[코어 계층]
    end

    subgraph "로컬 시스템"
        Oracle[(Oracle DB)]
        Modules[비즈니스 모듈]
    end

    FHIR -->|GET /Patient| Adapter
    Adapter -->|엔티티로 매핑| Core
    Core --> Oracle

    DICOM -->|GET 이미지| Adapter
    Adapter -->|메타데이터 추출| Core
    Core --> Modules
```

---

## 보안 아키텍처

### JWT 토큰 흐름

```mermaid
sequenceDiagram
    participant UI as UI 계층
    participant Auth as 인증 서비스
    participant TokenGen as 토큰 생성기
    participant DB as 데이터베이스

    UI->>Auth: POST /api/auth/login
    Auth->>DB: 자격 증명 검증
    DB-->>Auth: 사용자 데이터

    Auth->>TokenGen: CreateToken(user)
    TokenGen->>TokenGen: 비공개 키로 서명
    TokenGen-->>Auth: JWT 토큰 + 리프레시 토큰

    Auth-->>UI: 토큰 반환
    UI->>UI: 토큰 메모리에 저장

    Note over UI: API 호출

    UI->>API: GET /api/patients<br/>Header: Authorization: Bearer <JWT>
    API->>Auth: ValidateToken(token)
    Auth->>TokenGen: 서명 검증
    TokenGen-->>Auth: 유효함
    Auth-->>API: 사용자 클레임
    API-->>UI: 데이터
```

### RBAC 권한 확인

```mermaid
graph TB
    Start[시작: 사용자 동작] --> GetToken[JWT 토큰 가져오기]

    GetToken --> Decode[JWT 페이로드 디코딩]
    Decode --> Extract[권한 추출]
    Extract --> Map[권한을 역할로 매핑]

    Map --> Check{권한 확인}

    Check -->|권한 있음| Allow[권한 부여]
    Check -->|권한 없음| Deny[권한 거부]
    Check -->|토큰 없음| RequireLogin[로그인 필요]

    Allow --> Execute[동작 실행]
    Deny --> Throw[UnauthorizedException 발생]
    RequireLogin -> Redirect[로그인 페이지로 리다이렉트]

    Execute --> Audit[감사 로그 항목 생성]
    Throw --> Audit

    Audit --> End[종료]
    Redirect --> End
```

---

## 부록: Mermaid 다이어그램 베스트 프랙티스

### 렌더링 옵션

1. **GitHub:**
   - Mermaid 다이어그램 자동 렌더링
   - 특별한 문법 필요 없음

2. **VS Code:**
   - "Markdown Preview Mermaid Support" 확장 설치
   - `Ctrl+Shift+V`로 미리보기

3. **온라인:**
   - [Mermaid Live Editor](https://mermaid.live/)
   - 다이어그램 복사하여 렌더링

### 사용된 다이어그램 유형

| 유형 | 목적 | 예시 |
|------|------|------|
| **graph TB/LR** | 고차 수준 아키텍처 | 시스템 컨텍스트, 배포 |
| **sequenceDiagram** | 워크플로우 상호 작용 | 모듈 로딩, 이벤트 흐름, 업데이트 |
| **classDiagram** | 클래스 관계 | 컴포넌트 구조 |
| **stateDiagram** | 상태 전환 | 로그인 흐름, 업데이트 배포 |
| **ERD** | 데이터베이스 스키마 | 모듈 구조 |

---

**문서 버전:** 1.0
**최종 업데이트:** 2026년 2월
**다음 검토:** 2026년 4월 (Phase 2 완료 후)
