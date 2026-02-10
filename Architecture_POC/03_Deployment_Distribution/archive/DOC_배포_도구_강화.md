  배포 도구 강화

  워크플로우 전략: 체계적
  컨텍스트: 모듈 등록, 버전 관리, 메뉴 구성을 자동화하면서 명명 규칙을 적용하여 nU3.Tools.Deployer 강화
  목표: DLL에서 메타데이터를 자동 추출하고 네임스페이스/파일명 명명 규칙을 표준화하여 수동 입력 오류 감소

  ---

  구현 로드맵

  Phase 1: 규칙 강제 및 메타데이터 추출
  목표: DLL 파일명 및 속성을 파싱하여 데이터 입력 자동화
    - [ ] 명명 규칙 로직:
        - 형식: nU3.Modules.{System}.{SubSys}.{Name}.dll
        - 파서: 파일명에서 System (EMR), SubSys (IO), Name 추출
    - [ ] 속성 강화:
        - ScreenInfoAttribute 업데이트하여 DllVersion 포함 (또는 AssemblyInfo에서 읽기)
        - Deployer 업데이트하여 AssemblyFileVersion 읽기

  Phase 2: 강화된 Deployer UI/UX
  목표: "업로드 및 등록" 워크플로우 간소화
    - [ ] 스마트 업로드 마법사:
        - Step 1: DLL 선택
        - Step 2: 자동 파싱 Name/System/Version. 확인을 위해 표시
        - Step 3: 자동 스캔 ScreenInfo 속성. 찾은 화면 목록 표시
        - Step 4: DB에 커밋 (SYS_MODULE_MST, SYS_MODULE_VER, SYS_PROG_MST)
    - [ ] 검증: nU3.Modules.* 패턴과 일치하지 않는 DLL 거부

  Phase 3: 역할 기반 메뉴 관리 (RBAC)
  목표: 메뉴를 특정 Roles/AuthLevels에 매핑
    - [ ] 데이터베이스 업데이트:
        - SYS_MENU에 AUTH_LEVEL 또는 ROLE_ID 추가 (또는 별도 SYS_MENU_ROLE 테이블)
    - [ ] 메뉴 편집기 업데이트:
        - 각 메뉴 노드에 대해 "최소 권한 수준" 또는 "허용된 역할" 설정 허용

  ---

  작업 계층 구조

  Epic: 배포 도구 자동화 (DEP-AUTO)

  Story: 스마트 메타데이터 추출 (DEP-AUTO-01)
    - [ ] Task A: DllNameParser 클래스 구현
    - [ ] Task B: ScanAndRegisterPrograms 업데이트하여 Assembly에서 버전 추출
    - [ ] Task C: DeployerForm 리팩토링하여 "스마트 업로드" 흐름 사용

  Story: 규칙 강제 (DEP-AUTO-02)
    - [ ] Task A: DeployerForm에 검증 로직 추가
    - [ ] Task B: Namespace != Filename 패턴인 경우 오류 표시
