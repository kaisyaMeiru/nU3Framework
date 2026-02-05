  /sg:workflow [Phase2] 고급 UI 및 연결성

  워크플로우 전략: 체계적
  페르소나: 아키텍트 (Primary), 프론트엔드 (DevExpress), 백엔드
  컨텍스트: 전문 UI 컨트롤 및 실제 백엔드 서비스로 nU3.Framework 확장
  목표: DevExpress 구성 요소 통합, Service Agent 패턴 구현, 실제 API 통신 설정

  ---

  구현 로드맵

  Phase 2.1: DevExpress 기본 프레임워크 통합 (UI 표준)
  목표: 표준 WinForms 컨트롤을 DevExpress 동등물로 교체하고 테마 시스템 설정
  MCP 컨텍스트: DevExpress.XtraEditors, RibbonControl, GridControl, LayoutControl

    - [ ] 핵심 UI 기본 클래스 (`nU3.Core.UI`):
        - [ ] IWorkForm 업데이트하여 DevExpress 상호작용 지원
        - [ ] BaseXtraForm (XtraForm 상속) 생성 - 팝업용
        - [ ] BaseWorkControl (XtraUserControl 상속) 생성 - MDI 자식용
        - [ ] LayoutManager 구현: LayoutControl을 통해 마진, 폰트, 컨트롤 간격 표준화

    - [ ] Shell 현대화 (`nU3.Shell`):
        - [ ] MenuStrip을 RibbonControl로 교체
        - [ ] DocumentManager (표준)을 DevExpress DocumentManager (TabbedView)로 교체
        - [ ] SkinManager 구현: 사용자가 DevExpress 스킨(테마) 전환 가능
        - [ ] SplashScreenManager 구현: 모듈 로드 중 로딩 상태 표시

    - [ ] 그리드 및 데이터 표준:
        - [ ] BaseGridControl 생성: 정렬, 필터링, "Server Mode" 지원 기본값으로 활성화된 사전 구성 그리드
        - [ ] GridHelper 구현: Excel/PDF로 내보내기 유틸리티

  Phase 2.2: Service Agent 레이어 (연결성)
  목표: 인터페이스 기반 클라이언트(Refit/RestSharp)를 사용하여 HTTP 통신 추상화
  MCP 컨텍스트: Refit, Newtonsoft.Json, Polly (재시도 정책)

    - [ ] 네트워크 인프라 (`nU3.Core.Network`):
        - [ ] Refit 및 Microsoft.Extensions.Http 설치
        - [ ] AuthHeaderHandler 구현: 헤더에 JWT 자동 주입
        - [ ] GlobalExceptionHandler 구현: 401/500 오류 캐치 및 UI 알림 트리거

    - [ ] 서비스 인터페이스:
        - [ ] IAuthService 정의: 로그인, 로그아웃, RefreshToken
        - [ ] IPatientService 정의: 검색, 상세정보 조회
        - [ ] IDeploymentService 정의: 모듈 확인을 위한 로컬 DB 시뮬레이션 대체

    - [ ] 의존성 주입 설정:
        - [ ] Autofac 또는 Microsoft.Extensions.DependencyInjection을 nU3.Shell에 통합
        - [ ] Service Agents 등록 (RestService.For<IAuthService>)

  Phase 2.3: 보안 및 세션 관리 (가드)
  목표: 실제 로그인 흐름 및 사용자 컨텍스트 관리 구현

    - [ ] 로그인 모듈 (`nU3.Modules.Login`):
        - [ ] LoginForm (다이얼로그) 생성
        - [ ] IAuthService 통합하여 자격 증명 검증
        - [ ] 성공 시 UserSession 싱글톤 초기화

    - [ ] 세션 컨텍스트:
        - [ ] UserSession: UserId, Token, DepartmentId 저장
        - [ ] PatientContext: 전역 현재 환자 상태 (방송용)

  Phase 2.4: Event Aggregator (메신저)
  목표: 직접 참조 없이 Forms 간 통신 활성화
  MCP 컨텍스트: Pub/Sub 패턴, System.Reactive 또는 커스텀 EventBus

    - [ ] 이벤트 버스 구현:
        - [ ] IEventAggregator 인터페이스 생성
        - [ ] Weak references(약한 참조)를 사용한 EventAggregator 구현 (메모리 누수 방지)
        - [ ] 표준 이벤트 정의: PatientSelectedEvent, OrderSignedEvent

    - [ ] 통합:
        - [ ] BaseWorkControl에 IEventAggregator 주입
        - [ ] 데모 생성: PatientSearchModule이 이벤트 발행 → PatientDetailModule이 UI 업데이트

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
