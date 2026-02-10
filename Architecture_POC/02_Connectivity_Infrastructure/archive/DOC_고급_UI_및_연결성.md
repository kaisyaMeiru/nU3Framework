  # [Phase2] 고급 UI 및 연결성 - 구현 명세

  이 문서는 `nU3.Framework`의 Phase 2에서 구현된 전문 UI 컨트롤 및 실제 백엔드 서비스와의 연결성 구현 결과를 설명합니다.

  ---

  ## 1. DevExpress UI 표준 통합 (구현 완료)

  핵심 UI 기본 클래스들이 `nU3.Core.UI`에 통합되어 모든 모듈이 일관된 디자인과 기능을 유지합니다.

  - **기본 클래스**:
    - **BaseWorkControl**: `XtraUserControl`을 상속하며, MDI 자식 화면의 생명주기(Activated, Deactivated)를 관리합니다.
    - **BaseWorkForm**: `XtraForm`을 상속하며, 팝업 및 독립 창의 기본 규격을 제공합니다.
  
  - **Shell 현대화**:
    - **RibbonControl**: 상단 메뉴 시스템을 리본 인터페이스로 전환 완료.
    - **DocumentManager**: MDI 환경을 `TabbedView`로 구성하여 다중 차트 업무 환경 지원.
    - **SkinManager**: DevExpress의 다양한 스킨(테마)을 사용자 기호에 맞춰 변경 가능.

  ## 2. Service Agent 레이어 (ConnectivityManager)

  REST API 통신을 추상화하여 개발자가 HTTP 클라이언트를 직접 관리하지 않도록 설계되었습니다.

  - **ConnectivityManager**: `DB`, `File`, `Log` 접근을 싱글톤으로 관리.
  - **HttpDBAccessClient**: SQL 쿼리 및 프로시저 호출을 REST 엔드포인트로 변환.
  - **보안 기능**: `UserSession`과 연동하여 모든 요청에 JWT(Bearer) 토큰을 자동 포함.

  ## 3. Event Aggregator (통신 중계자)

  모듈 간의 결합도를 낮추기 위해 Pub/Sub 패턴의 `EventAggregator`가 구현되었습니다.

  - **작동 원리**: 직접적인 객체 참조 없이 `EventBus.Publish()`와 `EventBus.Subscribe()`를 통해 데이터 교환.
  - **메모리 관리**: `WeakReference`를 활용하여 탭이 닫힐 때 자동으로 구독이 해제되어 메모리 누수 방지.
  - **주요 이벤트**: `PatientSelectedEvent`, `ModuleActivatedEvent`, `NavigationRequestEvent` 등.

  ---

  상세 구현 작업 (다음 단계)

  Task 2.1: DevExpress 기본 클래스
  페르소나: 프론트엔드 전문가
  우선순위: 중요
  설명: 모든 향후 모듈이 상속받을 기본 클래스 생성

  Task 2.2: Event Aggregator
  페르소나: 아키텍트
  우선순위: 높음
  설명: 모듈의 결합도 해제에 필수적

  Task 2.3: Login 및 DI Container
  페르소나: 백엔드/아키텍트
  우선순위: 높음
  설명: 수동 인스턴스화에서 DI 컨테이너로 전환하여 관리성 개선
