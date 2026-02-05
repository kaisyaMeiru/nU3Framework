  nU3.Bootstrapper 워크플로우

  nU3.Bootstrapper는 환경 초기화, 데이터베이스 준비, 중앙 "서버" (시뮬레이션)에서 안전한 스테이징 영역으로 모듈 다운로드/업데이트, 실행 영역에 설치, 마지막으로 메인 Shell 애플리케이션 실행을 책임지는 진입점 애플리케이션입니다.

  1. 초기화 단계 (Program.Main)

    1. 시작: 콘솔 애플리케이션 시작
    2. ModuleLoader 인스턴스화:
        * LocalDatabaseManager 생성:
            * DB 경로를 %AppData%\nU3.Framework\Database\nU3_Local.db로 설정
            * DB가 존재하지 않으면 InitializeSchema()를 호출하여 스키마 초기화
        * SQLiteModuleRepository 초기화
        * 경로 설정:
            * Staging 경로: %AppData%\nU3.Framework\Cache (안전한 다운로드/업데이트 영역)
            * Install 경로: [ExeDirectory]\Modules (런타임 실행 영역)
    3. 데이터베이스 초기화 보장:
        * loader.EnsureDatabaseInitialized() 호출 (_dbManager.InitializeSchema()에 프록시됨)
        * 테이블 (SYS_MODULE_MST, SYS_MODULE_VER, SYS_PROG_MST, SYS_MENU) 존재 확인
        * 마이그레이션 확인 수행 (CATEGORY, SUBSYSTEM, AUTH_LEVEL과 같은 컬럼이 누락된 경우 추가)

  2. Seeding 단계 (Seeder.SeedDummyData)

    * 참고: 이것은 현재 테스트/개발용으로 하드코딩되어 있습니다.
    1. 더미 DLL 찾기: 로컬 bin 또는 프로젝트 경로에서 nU3.Modules.ADM.AD.Deployer.dll (또는 유사 더미 모듈) 찾기
    2. 해시 계산: 찾은 DLL의 SHA256 해시 계산
    3. 데이터베이스 삽입/업데이트:
        * SYS_MODULE_MST: 모듈 등록
        * SYS_MODULE_VER: 해시 및 경로와 함께 버전 1.0.0.0 등록
        * SYS_PROG_MST: 프로그램 ID 및 클래스 이름 등록
        * SYS_MENU: 테이블이 비어있으면 기본 메뉴 항목 삽입

  3. 업데이트 및 설치 단계 (loader.CheckAndLoadModules)

  이것은 CheckAndDownloadModules()에서 구현된 핵심 로직입니다.

    1. 모듈 목록 가져오기: SQLiteModuleRepository를 통해 SYS_MODULE_MST에서 모든 모듈 검색
    2. 모듈 반복: 활성 모듈 (IS_USE = 'Y')별로:
        * A. Staging에 다운로드 (업데이트 확인):
            * 대상: %AppData%\nU3.Framework\Cache\[SubSystem]\[FileName]
            * 확인: 파일이 누락된 경우 (또는 실제 시나리오에서는 서버와 Hash/Version이 다른 경우)
            * 동작: "ServerStorage"(%AppData%\nU3.Framework\ServerStorage)에서 Staging으로 파일 복사
        * B. Runtime에 설치 (배포):
            * 대상: [ExeDirectory]\Modules\[SubSystem]\[FileName]
            * 확인: Runtime 폴더에 파일이 누락된 경우 또는 Staging이 최근 업데이트된 경우
            * 동작: Staging에서 Runtime으로 파일 복사
            * 이점: nU3.Shell.exe가 아직 실행되지 않았으므로 [ExeDirectory]\Modules에 있는 DLL이 잠기지 않아 안전한 덮어쓰기 가능

  4. 실행 단계 (Program.Main)

    1. Shell 찾기: [ExeDirectory]에서 nU3.Shell.exe 찾기
    2. 실행: Process.Start를 사용하여 nU3.Shell.exe 실행
    3. 종료: Bootstrapper 종료

  ---

  주요 아키텍처 특징

    * 이중 단계 배포:
        * Stage 1 (다운로드): %AppData%로 (사용자 쓰기 가능, 잠금 문제 없음)
        * Stage 2 (설치): [ExeDir]로 (빠른 로컬 로드, 표준 .NET 로딩 컨텍스트)
    * Shadow Copying (수동): 플러그인을 위한 .NET의 내장 Shadow Copy 기능을 관리하기 복잡할 수 있으므로 "부트 시점 복사" 전략을 구현합니다.
    * 중앙 DB: %AppData%의 알려진 경로를 사용하여 Bootstrapper (관리자) 및 Shell (사용자)이 동일한 메타데이터를 공유합니다.

  이 워크플로우는 nU3.Shell이 시작될 때 업데이트 프로세스 중 파일 잠금 충돌 없이 항상 로컬 캐시된 모듈의 최신 버전으로 실행되도록 보장합니다.
