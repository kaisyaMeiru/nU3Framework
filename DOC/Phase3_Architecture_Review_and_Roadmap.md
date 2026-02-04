# nU3 Framework 아키텍처 리뷰 및 고도화 계획서

## 1. 개요 (Executive Summary)

본 문서는 nU3 Framework의 현재 구현 상태를 점검하고, 대형 의료 SI 프로젝트의 요구사항인 **"개발자 독립성"**과 **"유기적 연결성"**을 확보하기 위한 기술적 개선 방향을 제시합니다. 
현재 프레임워크는 .NET 8 및 DI(의존성 주입) 기반의 현대적인 아키텍처로 성공적으로 전환되었으며, 안정적인 통신 레이어를 확보했습니다. 이제는 **개발 생산성(Productivity)**과 **표준화(Standardization)**에 초점을 맞춘 고도화가 필요합니다.

---

## 2. 현재 상태 분석 (As-Is Review)

### ✅ 성공적인 전환 요소
1.  **현대적인 DI 컨테이너 도입**: 
    *   `ModuleLoaderService` 및 UI 컨트롤(`BaseWorkControl`) 생성 시 `ActivatorUtilities`를 적용하여, 개발자가 복잡한 인스턴스 관리 없이 생성자 주입만으로 자원(`IDBAccessService` 등)을 사용할 수 있게 되었습니다.
2.  **안정적인 Connectivity**:
    *   `HttpClient` Singleton 패턴 적용 및 `ConfigureAwait(false)` 처리를 통해, UI 스레드 차단(Deadlock) 및 소켓 고갈(Socket Exhaustion) 문제를 근본적으로 해결했습니다.
3.  **모듈 독립성 확보**:
    *   `nU3ProgramInfoAttribute`를 통한 메타데이터 기반 로딩 방식을 정립하여, 각 모듈(DLL)이 서로의 참조 없이도 Shell에 플러그인 될 수 있는 구조를 갖췄습니다.

### ⚠️ 개선이 필요한 취약점 (Gap Analysis)
1.  **비즈니스 로직과 UI의 강한 결합**: 
    *   현재 샘플(`WorklistControl`)을 보면 `SearchAsync` 내부에서 SQL을 직접 호출하고 있습니다. 이는 화면 복잡도가 높아질수록 유지보수를 어렵게 만듭니다.
2.  **표준 UI 패턴 부재**:
    *   모든 화면을 `BaseWorkControl`에서 처음부터 그리는 것은 비효율적입니다. 조회 화면, 입력 화면, 팝업 등에 대한 표준 템플릿이 필요합니다.
3.  **데이터 흐름의 추상화 부족**:
    *   `EventAggregator`가 존재하지만, 모듈 간 데이터를 주고받는 표준 프로토콜(Contract)이 명시적이지 않습니다.

---

## 3. 고도화 개선 제안 (To-Be Strategy)

### 과제 1: 개발자 독립성을 위한 "Biz Logic Layer" 분리
화면 개발자가 UI 코드(`Control.cs`) 내에 SQL이나 복잡한 로직을 작성하지 않도록 **View**와 **Logic**을 분리해야 합니다.

*   **제안**: **MVP (Model-View-Presenter) 변형 패턴** 도입
    *   `BaseWorkControl` (View): 순수하게 UI 그리리기 및 이벤트 전달만 담당.
    *   `BizPresenter` (Logic): `IDBAccessService`를 주입받아 데이터 처리 후 View에 바인딩.
    *   **효과**: 개발자는 "화면 그리기"와 "데이터 처리"를 분리하여 생각할 수 있으며, 로직만 별도로 단위 테스트가 가능해집니다.

### 과제 2: "표준 템플릿(Template)" 제공으로 생산성 향상
대형 SI는 수천 개의 화면을 개발합니다. 공통적인 패턴을 캡슐화해야 합니다.

*   **제안**: `nU3.Core.UI.Templates` 네임스페이스 신설
    *   `SearchGridControl`: 상단 조회조건 패널 + 중앙 그리드 + 페이징 처리가 미리 구현된 부모 클래스.
    *   `DetailEntryControl`: 라벨-입력 컨트롤 쌍을 쉽게 배치하고, 유효성 검사(Validation)가 내장된 부모 클래스.
    *   **효과**: 개발자는 상속 후 `OnSearch()` 만 오버라이드하면 조회 화면이 완성됩니다.

### 과제 3: 유기적 연결성을 위한 "Smart Context"
현재 `WorkContext`는 단순히 데이터를 담는 그릇입니다. 이를 더 지능적으로 만들어야 합니다.

*   **제안**: **Reactive Context** 도입
    *   특정 환자(`CurrentPatient`)가 변경되면, 이를 구독하고 있는 모든 화면의 데이터가 자동으로 갱신되거나 초기화되는 로직을 프레임워크 레벨에서 제어.
    *   **효과**: 개발자가 일일이 이벤트를 구독하는 코드를 작성하지 않아도, 프레임워크가 알아서 "환자가 바뀌었으니 초기화하세요"라고 지시합니다.

---

## 4. 실행 로드맵 (Action Plan)

### [Phase 1: 구조 표준화] (우선순위 높음)
1.  **`BaseBizLogic` 클래스 설계**: 모듈 개발자가 상속받아 비즈니스 로직을 구현할 기본 클래스.
2.  **`StandardResult<T>` 도입**: 서버 통신 및 로직 수행 결과를 통일된 규격(`Success`, `Message`, `Data`, `ErrorCode`)으로 반환.

### [Phase 2: UI 템플릿 구현]
1.  **조회 템플릿 (`SearchTemplate`)**: 자동 조회, 엑셀 다운로드, 그리드 상태 저장 기능 내장.
2.  **입력 템플릿 (`EntryTemplate`)**: `DataAnnotation` 기반의 자동 유효성 검증 기능 내장.

### [Phase 3: 개발 도구 지원]
1.  **Module Runner**: 전체 Shell을 실행하지 않고, 특정 DLL(모듈)만 가볍게 띄워서 테스트할 수 있는 경량 실행기 제공.

---

이 문서는 프로젝트의 성공적인 수행을 위한 기술적 지침서(Guideline)로 활용됩니다.
승인해 주시면 **[Phase 1: 구조 표준화]** 작업을 위한 코드 구현을 시작하겠습니다.
