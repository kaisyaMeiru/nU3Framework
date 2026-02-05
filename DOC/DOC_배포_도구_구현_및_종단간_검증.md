  /sg:task 배포 도구 구현 및 종단간 검증

  전략: 체계적
  컨텍스트: PoC (더미 모듈)에서 기능적 배포 시스템으로 이동
  목표: 개발자/관리자를 위한 WinForms 도구를 생성하여 DLL을 업로드, 데이터베이스에 등록, 메뉴 구성, 클라이언트(Shell)에서의 정확한 다운로드 및 실행 검증

  ---

  작업 계층 구조

  Epic: 배포 및 구성 시스템 (DEP)

  Story: 모듈 등록 및 업로드 도구 (DEP-01)
  목표: 새 모듈을 등록하고 버전을 업로드할 수 있는 UI 도구
    - [ ] Task DEP-01-A: nU3.Tools.Deployer 프로젝트 생성 (WinForms)
    - [ ] Task DEP-01-B: "모듈 마스터 관리" UI 구현 (SYS_MODULE_MST CRUD)
    - [ ] Task DEP-01-C: "버전 업로드" UI 구현
        - DLL 파일 선택
        - 해시/크기 자동 계산
        - "서버 스토리지" (시뮬레이션 폴더)로 복사
        - SYS_MODULE_VER에 삽입

  Story: 메뉴 구성 관리자 (DEP-02)
  목표: 등록된 프로그램에서 메뉴 트리를 구성하는 UI
    - [ ] Task DEP-02-A: "프로그램 스캐너" 구현
        - 리플렉션을 통해 DLL 로드
        - [ScreenInfo] 속성 스캔
        - SYS_PROG_MST에 업서트 (Upsert)
    - [ ] Task DEP-02-B: "메뉴 트리 편집기" 구현
        - SYS_MENU 계층 구조를 빌드하는 드래그 앤 드롭 인터페이스
        - 메뉴 노드를 PROG_ID에 매핑

  Story: 종단간 검증 (DEP-03)
  목표: Shell 코드를 수정하지 않고 새 모듈로 시스템 작동을 증명
    - [ ] Task DEP-03-A: 새 테스트 모듈 nU3.Modules.Clinic 생성 (실제 시나리오)
    - [ ] Task DEP-03-B: Deployer 도구를 사용하여 Clinic.dll 업로드
    - [ ] Task DEP-03-C: Deployer 도구를 사용하여 메뉴 "Clinic > Outpatient" 생성
    - [ ] Task DEP-03-D: Bootstrapper 시작 → Shell 실행하고 "Clinic" 메뉴가 나타나고 열리는지 검증

  ---

  실행 계획

  우선 DEP-01 (Deployer 도구)부터 시작합니다.

  Step 1: nU3.Tools.Deployer 프로젝트 생성
  Step 2: 모듈 마스터 CRUD UI 구현
