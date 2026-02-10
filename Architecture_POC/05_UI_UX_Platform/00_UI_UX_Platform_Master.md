# UI/UX 플랫폼 마스터 문서 (UI/UX Platform Master Document)

**버전:** 1.0 (통합본)
**날짜:** 2026-02-10
**컨텍스트:** nU3.Framework (UI 기반, 통신, 에러 처리, 로깅)

---

## 1. 개요

UI/UX 플랫폼 계층은 DevExpress WinForms를 사용하여 일관되고 견고하며 상호 작용하는 사용자 인터페이스를 구축하기 위한 기본 클래스와 서비스를 제공합니다.

---

## 2. BaseWorkControl (`nU3.Core.UI`)

모든 MDI 자식 화면의 표준 기본 클래스입니다.

### 2.1 주요 기능
*   **컨텍스트 (`WorkContext`):** 환자, 사용자, 파라미터 공유.
*   **보안 (`ModulePermissions`):** 선언적 검사 (`CanRead`, `CanUpdate`).
*   **라이프사이클:** `OnScreenActivated()`, `OnScreenDeactivated()`, `OnBeforeClose()`.
*   **리소스 관리:**
    *   `RegisterDisposable(object)`: 자동 정리.
    *   `CancellationToken`: 닫기 시 자동 취소.
*   **인프라 접근:** 내장된 `Connectivity`, `Logger`, `AuditLogger`.

### 2.2 사용법
```csharp
[nU3ProgramInfo(typeof(MyControl), "내 화면", "MY_001", "CHILD")]
public class MyControl : BaseWorkControl
{
    protected override async void OnScreenActivated()
    {
        if (!CanRead) return;
        await Connectivity.DB.ExecuteDataTableAsync("SELECT...", CancellationToken);
    }
}
```

---

## 3. 통신 (`EventAggregator`)

모듈 간의 약결합 통신입니다.

### 3.1 아키텍처
*   **패턴:** `IEventAggregator`를 통한 Pub/Sub.
*   **메커니즘:** WeakReferences (메모리 누수 방지).
*   **범위:** 전역 (Shell 레벨).

### 3.2 표준 이벤트
*   `PatientSelectedEvent`: 환자 컨텍스트 변경 브로드캐스트.
*   `ModuleActivatedEvent`: 활성화 시 `BaseWorkControl`에 의해 자동 발행.
*   `NavigationRequestEvent`: 다른 화면 열기 요청.

### 3.3 흐름
`MainShell` (또는 모듈 A) -> `Publish(Event)` -> `EventAggregator` -> `Subscribe(Handler)` -> 모듈 B.

---

## 4. 에러 리포팅 (`CrashReporter`)

처리되지 않은 예외의 자동 처리입니다.

### 4.1 기능
*   **포착:** UI 스레드, 비-UI 스레드, Task 예외.
*   **증거:** 스크린샷 + 전체 스택 트레이스 + 시스템 정보.
*   **전송:**
    *   **로컬:** `%AppData%\nU3.Framework\CrashLogs\`.
    *   **이메일:** SMTP (설정된 경우).
    *   **서버:** `ConnectivityManager`를 통한 즉시 업로드.

---

## 5. 로깅 및 감사 (`LogManager`)

포괄적인 로깅 시스템입니다.

### 5.1 파일 로깅
*   **경로:** `%AppData%\nU3.Framework\LOG\`.
*   **형식:** `{PC}_{IP}_{Date}.log`.
*   **레벨:** Trace, Debug, Info, Warning, Error, Critical.
*   **자동 업로드:** 매일 02:00 또는 에러 발생 시.

### 5.2 감사 로깅 (Audit Logging)
*   **경로:** `%AppData%\nU3.Framework\AUDIT\`.
*   **형식:** JSON Lines (`.json`).
*   **액션:** Create, Read, Update, Delete, Login, Print, Export.
*   **내용:** 누가, 언제, 무엇을 (OldValue/NewValue).

### 5.3 사용법
```csharp
// 표준
LogManager.Info("처리 시작", "MyModule");

// 감사
LogManager.LogAudit(AuditAction.Update, "Patient", "P001", "이름 변경");
```

---

## 6. 구현 상태 (Phase 5)

*   **완료됨:**
    *   라이프사이클/컨텍스트/리소스를 포함한 `BaseWorkControl`.
    *   `EventAggregator` 구현.
    *   스크린샷 및 업로드가 포함된 `CrashReporter`.
    *   파일 및 감사를 지원하는 `LogManager`.

*   **다음 단계:**
    *   UI 테마 관리자 (스킨).
    *   사용자 정의 단축키/매크로.
