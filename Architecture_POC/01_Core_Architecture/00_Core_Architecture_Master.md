# 핵심 아키텍처 마스터 문서 (Core Architecture Master Document)

**버전:** 1.0 (통합본)
**날짜:** 2026-02-10
**컨텍스트:** nU3.Framework (의료 정보 시스템 프레임워크)

---

## 1. 시스템 개요

**nU3.Framework**는 대형 병원 정보 시스템(EMR)을 위해 설계된 모듈형, 메타데이터 기반의 WinForms 애플리케이션입니다. 엄격한 플러그인 아키텍처를 사용하여 "Shell"(컨테이너)과 "Modules"(비즈니스 로직)를 분리합니다.

### 핵심 기술 스택
*   **언어:** C# 12.0
*   **플랫폼:** WinForms (MDI)
*   **프레임워크:** .NET 8.0
*   **UI 컴포넌트:** DevExpress WinForms v23.2.9
*   **데이터베이스:** 
    *   **서버:** Oracle (비즈니스 데이터)
    *   **로컬:** SQLite (클라이언트 설정, 메타데이터)

### 핵심 아키텍처 패턴
1.  **플러그인 아키텍처:** 모듈은 `AssemblyLoadContext`를 통해 런타임에 동적으로 로드됩니다.
2.  **단계적 배포 (Staged Deployment):** 업데이트는 배포 전 무결성을 검증하기 위해 캐시에 먼저 스테이징되며, 이를 통해 DLL 잠금 현상을 방지합니다 (섀도우 카피 전략).
3.  **로컬 우선 구성 (Local-First Configuration):** 클라이언트 설정(메뉴, 모듈 버전)은 성능과 복원력을 위해 로컬 SQLite 데이터베이스에 저장됩니다.
4.  **중재자 패턴 (Mediator Pattern):** `EventAggregator`를 통해 모듈 간의 약결합(Decoupled) 통신을 가능하게 합니다.
5.  **서비스 에이전트 (Service Agent):** `ConnectivityManager`가 인프라 통신(DB, 파일, 로그)을 추상화합니다.

---

## 2. 논리 아키텍처 계층

### Layer 1: 부트스트래퍼 (`nU3.Bootstrapper`)
*   **책임:** 배포, 무결성 검사, 실행.
*   **프로세스:**
    1.  **연결:** HTTP를 통해 서버 저장소 확인.
    2.  **비교:** 로컬 SQLite(`SYS_MODULE_VER`)와 버전 비교.
    3.  **다운로드:** 새 DLL을 스테이징 영역(`%AppData%\nU3.Framework\Cache`)으로 가져옴.
    4.  **설치:** 스테이징 영역에서 런타임 영역(`[AppDir]\Modules`)으로 동기화.
    5.  **실행:** `nU3.Shell.exe` 시작.

### Layer 2: 코어 (`nU3.Core` & `nU3.Data`)
*   **`IBaseWorkControl`**: 모든 비즈니스 화면에 대한 표준 인터페이스.
*   **`nU3ProgramInfoAttribute`**: 화면 발견을 위한 메타데이터 (`[nU3ProgramInfo]`).
*   **`UserSession`**: 인증 및 JWT 관리를 위한 전역 싱글톤.
*   **`LocalDatabaseManager`**: SQLite 스키마 관리.
*   **`EventAggregator`**: 약결합 통신을 위한 Pub/Sub 시스템.

### Layer 3: 쉘 (`nU3.Shell`)
*   **역할:** MDI 컨테이너.
*   **기능:**
    *   의존성 주입 (Microsoft.Extensions.DependencyInjection).
    *   동적 메뉴 구성 (SQLite `SYS_MENU` 기반).
    *   모듈 지연 로딩 (Lazy Loading).

### Layer 4: 연결성 (`nU3.Connectivity`)
*   **`ConnectivityManager`**: 모든 원격 서비스에 대한 싱글톤 진입점.
*   **`HttpDBAccessClient`**: REST 기반 SQL 실행.
*   **`HttpFileTransferClient`**: 파일 작업.
*   **`HttpLogUploadClient`**: 로그 관리.

### Layer 5: 모듈 (`nU3.Modules.*`)
*   **구조:** `Modules/[Category]/[SubSystem]/[Name].dll`
*   **설계:** `BaseWorkControl`을 상속하며, 서로 분리되어 있고 이벤트를 통해 통신합니다.

---

## 3. 핵심 인터페이스 및 기본 클래스

### `IBaseWorkControl` (인터페이스)
모든 작업 화면에 대한 계약을 정의합니다.
*   **`ProgramID`**: 고유 화면 ID.
*   **`ProgramTitle`**: 표시 제목.

### `BaseWorkControl` (구현체)
`XtraUserControl`을 상속하고 `IBaseWorkControl`을 구현합니다.
*   **라이프사이클:** `OnActivated()`, `OnDeactivated()`, `OnBeforeClose()`.
*   **컨텍스트:** `WorkContext` (환자, 사용자, 파라미터).
*   **리소스:** `RegisterDisposable()`, `ReleaseResources()`, `CancellationToken`.
*   **보안:** `HasPermission()`, `CanRead`, `CanUpdate`.

### `nU3ProgramInfoAttribute`
모듈 발견을 위한 메타데이터입니다.
```csharp
[nU3ProgramInfo(typeof(MyControl), "제목", "PROG_ID", "CHILD")]
public class MyControl : BaseWorkControl { ... }
```

### `EventAggregator`
약결합 통신을 지원합니다.
*   `Publish<T>(payload)`: 이벤트 전송.
*   `Subscribe<T>(action)`: 이벤트 수신 (WeakReference 사용).

---

## 4. 데이터 모델 (로컬 SQLite)

| 테이블 | 설명 | 주요 컬럼 |
|-------|-------------|-------------|
| **SYS_MODULE_MST** | 설치된 모듈 | `MODULE_ID`, `FILE_NAME`, `CATEGORY` |
| **SYS_PROG_MST** | 화면 정의 | `PROG_ID`, `CLASS_NAME`, `AUTH_LEVEL` |
| **SYS_MENU** | 메뉴 구조 | `MENU_ID`, `PARENT_ID`, `PROG_ID` |
| **SYS_MODULE_VER** | 버전 제어 | `VERSION`, `FILE_HASH` |

---

## 5. 실행 흐름 (Execution Flow)

1.  **부팅:** 부트스트래퍼가 모듈 업데이트 -> 쉘 실행.
2.  **쉘 초기화:** 로컬 DB 연결 -> 메뉴(NavBar) 구성.
3.  **사용자 동작:** 사용자가 메뉴 항목 클릭.
4.  **해석:** 쉘이 `SYS_MENU` -> `SYS_PROG_MST`에서 `PROG_ID` 조회.
5.  **로딩:**
    *   DLL 로드 여부 확인.
    *   없으면 `AssemblyLoadContext`를 통해 로드.
6.  **인스턴스화:** DI를 통해 `BaseWorkControl` 인스턴스 생성.
7.  **활성화:** `OnActivated()` 호출, `ModuleActivatedEvent` 발행.
8.  **표시:** MDI 탭에 표시.

---

## 6. 구현 상태 (Phase 1)

*   **완료됨:**
    *   핵심 인터페이스 및 속성.
    *   기본 UI 클래스 (`BaseWorkControl`, `BaseWorkForm`).
    *   로컬 SQLite 스키마 및 매니저.
    *   부트스트래퍼 (업데이트 및 실행).
    *   배포 도구 (Deployer).
    *   쉘 UI (Ribbon, NavBar, TabbedView).

*   **대기/다음:**
    *   고급 UI 컴포넌트.
    *   실시간 데이터 바인딩 최적화.