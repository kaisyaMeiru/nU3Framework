# /sg:workflow [Phase1] 기본 아키텍처 및 모듈 시스템

  워크플로우 전략: 체계적
  페르소나: 아키텍트 (Primary), 백엔드, 프론트엔드
  상태: **완료됨**
  컨텍스트: 플러그인 아키텍처를 사용한 엔터프라이즈 WinForms (DevExpress) 프레임워크

  이 워크플로우는 Module (물리적) → Program (논리적) → Menu (시각적) 계층 구조와 Shadow Copy 배포 메커니즘을 설정합니다.

  ---

  ## 구현 로드맵 (상태 보고)

  ### Phase 1.1: 핵심 계약 및 메타데이터 (기초)
  목표: 모든 비즈니스 모듈이 준수해야 하는 표준 인터페이스와 속성을 정의합니다.
  상태: **완료됨**

    - [x] `Core.dll` 어셈블리 정의:
        - [x] IModule 인터페이스 생성 (라이프사이클 후크: Initialize, Dispose)
        - [x] IWorkForm 인터페이스 생성 (모든 MDI 자식을 위한 기본 계약)
        - [x] ScreenInfoAttribute 클래스 구현 (요구사항에 지정된 대로)
        - [x] UserSession 싱글톤 정의 (전역 상태 관리)

    - [x] 기본 폼 구현 (`GMIS.Framework`):
        - [x] BaseWorkControl: DevExpress.XtraEditors.XtraUserControl 상속 + IWorkForm
        - [x] BasePopupForm: DevExpress.XtraEditors.XtraForm 상속
        - [x] BaseWorkControl.OnLoad에 AuthLevel 확인 로직 추가

  ### Phase 1.2: 데이터베이스 및 배포 API (공급망)
  목표: Modules, Versions, 및 Menus를 관리할 스키마 설정.
  상태: **완료됨** (`nU3.Data` 및 SQLite를 통해 구현)

    - [x] 데이터베이스 스키마 구현 (`LocalDatabaseManager.cs`):
        - [x] SYS_MODULE_MST 생성: Module ID, Name, 고정 파일이름
        - [x] SYS_MODULE_VER 생성: 버전 관리, 해시 (SHA256), 경로
        - [x] SYS_PROG_MST 생성: [ScreenInfo]에서 자동 생성 (ProgID ↔ ClassName 매핑)
        - [x] SYS_MENU 생성: 계층 구조, MenuID를 ProgID에 연결

    - [x] 배포 서비스 (`nU3.Connectivity` 및 `Bootstrapper`):
        - [x] Server Storage를 위한 로컬 파일 시뮬레이션 구현
        - [x] 해싱 및 버전 확인 구현

  ### Phase 1.3: 모듈 관리 및 로더 엔진 (엔진)
  목표: 로컬 vs 서버 버전 비교, shadow-copy, 및 DLL 로드를 위한 로직.
  상태: **완료됨**

    - [x] Bootstrapper (Launcher):
        - [x] "버전 확인" 구현: 로컬 DB vs Server Storage 비교
        - [x] "다운로더" 구현: 변경된 DLL을 `AppData/nU3.Framework/Cache` (Staging)로 다운로드
        - [x] "런타임 동기화" 구현: Shell 실행 전 Cache에서 `Modules/` 폴더로 복사

    - [x] 리플렉션 및 레지스트리 서비스 (`nU3.Shell`):
        - [x] AssemblyLoader 구현: 현재 컨텍스트에서 DLL 로드
        - [x] MetadataScanner 구현: 로드된 DLL에서 [ScreenInfo] 속성 스캔
        - [x] ProgramRegistry 구현: ScreenId (String) → Type (Class) 매핑

  ### Phase 1.4: 메인 Shell UI 및 탐색 (컨테이너)
  목표: 로드된 폼을 호스팅하는 MDI 컨테이너.
  상태: **완료됨**

    - [x] MainForm 구성:
        - [x] RibbonControl (상단) 및 StatusBar (하단) 설정
        - [x] MDI 동작을 위한 DocumentManager 구성 (View: TabbedView)
        - [x] 탐색 트리를 위한 NavBarControl 설정

    - [x] 메뉴 빌딩 로직:
        - [x] SQLite에서 SYS_MENU 데이터 가져오기
        - [x] NavBarControl에서 재귀적 트리 노드 빌드
        - [x] 클릭 이벤트를 MenuExecutor에 바인딩

  ### Phase 1.5: 개발자 도구 (활성화 도구)
  목표: 모듈을 등록하고 업로드할 수 있는 개발자용 도구.
  상태: **완료됨**

    - [x] Module Builder/Uploader 도구 (`nU3.Tools.Deployer`):
        - [x] Modules 및 Categories 등록
        - [x] 버전 업로드 (SHA256 계산)
        - [x] 메뉴 편집기 (시각적 드래그 앤 드롭)

  ---

  ## Phase 1 회고
  핵심 기초가 안정적입니다. 모듈은 독립적으로 개발되어, Deployer를 통해 등록되고, Bootstrapper를 통해 클라이언트로 배포될 수 있습니다. 로컬 클라이언트 구성을 위한 SQLite 사용은 강력한 오프라인 기능과 빠른 메뉴 로딩을 가능하게 합니다.
