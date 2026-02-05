  # nU3ProgramInfoAttribute 자동 로드 가이드

  ## 개요

  `nU3ProgramInfoAttribute`를 사용하면 DB 조회 없이 자동 로드, 메뉴 자동 생성, 인스턴스 자동 생성을 할 수 있습니다.

  ## 두 가지 접근 방식

  ### 1. DB 기반 (DB 조회)
  **기존 방식 (DB 조회):**
  ```
  메뉴 클릭
    ↓ DB로부터 ProgId로 Program 조회
    ↓ ModuleId 획득
    ↓ DB로부터 ModuleId로 Module 조회
    ↓ DLL 경로 계산
    ↓ DLL 로드
    ↓ DB로부터 ClassName 조회
    ↓ 타입 찾음
    ↓ 인스턴스 생성
  ```

  **개선 방식 (Attribute 기반):**
  ```
  메뉴 클릭
    ↓ Attribute를 통해 프로그램 속성 취득
    ↓ DLL 로드
    ↓ 인스턴스 생성
  ```

  ### 2. 자동화된 접근 방식

  ```csharp
  [nU3ProgramInfo(typeof(PatientListControl), "환자 목록", "EMR_PATIENT_LIST_001")]
  public class PatientListControl : BaseWorkControl
  {
      // 자동 설정되는 속성들:
      // - SystemType: "EMR"
      // - SubSystem: "IN"
      // - DllName: "nU3.Modules.EMR.IN.Worklist"
      // - ClassName: "nU3.Modules.EMR.IN.Worklist.PatientListControl"
      // - DLL Path: "EMR/IN/nU3.Modules.EMR.IN.Worklist.dll"
      // - ModuleId: "PROG_EMR_IN_nU3.Modules.EMR.IN.Worklist"
  }
  ```

  ## 사용 처리

  ### 1. 프로그램 로드 (단순)

  ```csharp
  namespace nU3.Modules.EMR.IN.Worklist
  {
      [nU3ProgramInfo(typeof(PatientListControl), "환자 목록", "EMR_PATIENT_LIST_001")]
      public class PatientListControl : BaseWorkControl
      {
          public PatientListControl()
          {
              // 초기화
          }
      }
  }
  ```

  ### 2. 메뉴 생성 (DB 최소화)

  ```csharp
  // ModuleLoaderService에서 프로그램 속성 캐시 획득
  var programAttributes = moduleLoader.GetProgramAttributes();

  // DB 조회 필요 메뉴 목록 반복
  foreach (var menuItem in menuItems)
  {
      if (programAttributes.TryGetValue(menuItem.ProgId, out var attr))
      {
          // DB 조회 필요 속성만 메뉴 생성
          Console.WriteLine($"Menu: {attr.ProgramName}");
          Console.WriteLine($"System: {attr.SystemType}/{attr.SubSystem}");
          Console.WriteLine($"Auth: {attr.AuthLevel}");
          Console.WriteLine($"FormType: {attr.FormType}");
      }
  }
  ```

  ### 3. 프로그램 로드

  #### 방법 A: ProgId로 인스턴스 생성

  ```csharp
  // 메뉴 ID로 인스턴스 생성
  var instance = moduleLoader.CreateProgramInstance("EMR_PATIENT_LIST_001");
  if (instance is BaseWorkControl control)
  {
      // 추가
      panel.Controls.Add(control);
  }
  ```

  #### 방법 B: Attribute로 인스턴스 생성

  ```csharp
  var attr = moduleLoader.GetProgramAttribute("EMR_PATIENT_LIST_001");
  if (attr != null)
  {
      // DLL 경로로 인스턴스 생성
      var type = moduleLoader.LoadProgramByAttribute(attr);
      if (type != null)
      {
          var instance = Activator.CreateInstance(type);
          // 추가
      }
  }
  ```

  #### 방법 C: 메서드 활용

  ```csharp
  var attr = moduleLoader.GetProgramAttribute("EMR_PATIENT_LIST_001");

  // DLL 경로로 인스턴스 생성
  string dllPath = attr.GetExpectedDllPath();
  // 결과: "EMR/IN/nU3.Modules.EMR.IN.Worklist.dll"

  // ModuleId로 인스턴스 생성
  string moduleId = attr.GetModuleId();
  // 결과: "PROG_EMR_IN_nU3.Modules.EMR.IN.Worklist"
  ```

  ## 사용 상세 처리

  ### 처리 1: 메뉴 클릭 처리

  ```csharp
  private void OnMenuItemClick(string progId)
  {
      // DB 조회 필요 프로그램 속성 확인
      var attr = _moduleLoader.GetProgramAttribute(progId);
      if (attr == null)
      {
          MessageBox.Show("프로그램 속성을 찾을 수 없습니다.");
          return;
      }

      // 권한 확인 (DB 조회 필요)
      if (attr.AuthLevel > _currentUser.AuthLevel)
      {
          MessageBox.Show("권한이 없습니다.");
          return;
      }

      // FormType에 따라 처리 (DB 조회 필요)
      switch (attr.FormType)
      {
          case "POPUP":
              OpenAsPopup(progId);
              break;
          case "SDI":
              OpenAsSDI(progId);
              break;
          default: // CHILD
              OpenAsChild(progId);
              break;
      }
  }

  private void OpenAsChild(string progId)
  {
      // DB 조회 필요 인스턴스 생성
      var instance = _moduleLoader.CreateProgramInstance(progId);
      if (instance is BaseWorkControl control)
      {
          var tabPage = new XtraTabPage(control.ProgramTitle);
          tabPage.Controls.Add(control);
          xtraTabControl.TabPages.Add(tabPage);
          xtraTabControl.SelectedTabPage = tabPage;
      }
  }
  ```

  ### 처리 2: 프로그램 검색

  ```csharp
  // DB 조회 필요 메뉴에 대한 프로그램 속성 검색
  var allPrograms = _moduleLoader.GetProgramAttributes();

  var searchResults = allPrograms.Values
      .Where(attr => attr.ProgramName.Contains(searchKeyword) ||
                     attr.ProgramId.Contains(searchKeyword))
      .Where(attr => attr.AuthLevel <= _currentUser.AuthLevel)
      .OrderBy(attr => attr.SystemType)
      .ThenBy(attr => attr.SubSystem)
      .ToList();

  foreach (var attr in searchResults)
  {
      Console.WriteLine($"{attr.SystemType}/{attr.SubSystem} - {attr.ProgramName} ({attr.ProgramId})");
  }
  ```

  ### 처리 3: 시스템별 프로그램 그룹화

  ```csharp
  // DB 조회 필요 메뉴를 시스템별로 그룹화
  var programsBySystem = _moduleLoader.GetProgramAttributes()
      .Values
      .GroupBy(attr => attr.SystemType)
      .ToDictionary(g => g.Key, g => g.ToList());

  // EMR 시스템의 프로그램 출력
  if (programsBySystem.TryGetValue("EMR", out var emrPrograms))
  {
      foreach (var attr in emrPrograms)
      {
          Console.WriteLine($"  [{attr.SubSystem}] {attr.ProgramName}");
      }
  }
  ```

  ### 처리 4: 최근 프로그램 (Quick Access)

  ```csharp
  // DB로부터 ProgId 목록 조회, 속성만 Attribute 조회
  var recentProgIds = GetRecentPrograms(); // DB로부터 ProgId 목록 조회

  foreach (var progId in recentProgIds)
  {
      var attr = _moduleLoader.GetProgramAttribute(progId);
      if (attr != null)
      {
          var quickButton = new Button
          {
              Text = attr.ProgramName,
              Tag = progId,
              ToolTip = $"{attr.SystemType}/{attr.SubSystem}"
          };
          quickButton.Click += (s, e) => OpenProgram(progId);
          quickAccessPanel.Controls.Add(quickButton);
      }
  }
  ```

  ## DB 최적화

  ### 복잡한 DB 구조
  ```sql
  -- 복잡: 모든 정보가 DB에 저장
  CREATE TABLE SYS_PROGRAM (
      PROG_ID VARCHAR(50),
      MODULE_ID VARCHAR(50),
      PROG_NAME VARCHAR(100),
      CLASS_NAME VARCHAR(200),     -- ❌ 중복
      AUTH_LEVEL INT,              -- ❌ 중복
      FORM_TYPE VARCHAR(10),       -- ❌ 중복
      SYSTEM_TYPE VARCHAR(10),     -- ❌ 중복
      SUB_SYSTEM VARCHAR(10)       -- ❌ 중복
  );
  ```

  ### 개선된 DB 구조 (최소화)
  ```sql
  -- 개선: 필요한 필드만 저장
  CREATE TABLE SYS_PROGRAM (
      PROG_ID VARCHAR(50) PRIMARY KEY,
      MODULE_ID VARCHAR(50),       -- ❌ 필요 (Attribute로 대체 가능)
      IS_ACTIVE CHAR(1),           -- ❌ 필요 (활성화 여부)
      PROG_TYPE INT                -- ❌ 필요 (타입 구분)
  );

  -- 프로그램 속성은 모두 nU3ProgramInfoAttribute로 저장!
  ```

  ## 장점 비교

  | 항목 | 기존 | 개선 방식 |
  |------|----------|-------------|
  | DB 조회 횟수 | 3-4회 | 0-1회 |
  | 권한 중복 | DB + Code | Code로 |
  | 메뉴 생성 속도 | 느림 (DB 조회) | 빠름 (메모리 캐시) |
  | 확장성 | DB + Code 최소화 | Code로 확장 |
  | 메타데이터 | DB + DLL | DLL로 |
  | 사용자 정의 | 불가 | 가능 (Attribute로) |

  ## 구현 팁

  ### 1. 초기 로드 시 캐시 생성
  ```csharp
  // 프로그램 시작 시 모든 DLL을 메모리에 로드
  moduleLoader.LoadAllModules();
  // 모든 DLL 정보를 _progAttributeCache에 저장
  ```

  ### 2. DLL 버전 확인 시 캐시 활용
  ```csharp
  // DLL 버전 확인 시
  moduleLoader.CheckAndUpdateModules();
  // 변경된 DLL을 다시 로드하여 캐시 갱신
  ```

  ### 3. DB 필요 정보 vs Attribute 필요 정보
  ```csharp
  // DB로부터 필요한 정보:
  // - 프로그램 버전 (ModuleId, Version)
  // - 활성화 여부 (IsActive)
  // - 메뉴 설정 (MenuId, ParentId, SortOrd)
  // - 사용자-프로그램 매핑

  // Attribute로부터 필요한 정보:
  // - ClassName (Attribute로)
  // - ProgramName (Attribute로)
  // - SystemType (Attribute로)
  // - SubSystem (Attribute로)
  // - AuthLevel (Attribute로)
  // - FormType (Attribute로)
  ```

  ## 플러그인 프로그래밍 가이드

  ### Step 1: Attribute 사용 확인 (완료!)
  ```csharp
  [nU3ProgramInfo(typeof(YourControl), "프로그램이름", "PROG_ID")]
  // ClassName은 자동으로 설정됩니다
  ```

  ### Step 2: ModuleLoaderService 활용 (완료!)
  ```csharp
  // GetProgramAttributes() 메서드 활용
  // CreateProgramInstance() 메서드 활용
  ```

  ### Step 3: 메뉴/화면 로드 로직 변경
  ```csharp
  // Before:
  var program = _programRepo.GetProgram(progId);
  var module = _moduleRepo.GetModule(program.ModuleId);
  var dllPath = CalculatePath(module);
  var type = LoadType(dllPath, program.ClassName);

  // After:
  var instance = _moduleLoader.CreateProgramInstance(progId);
  ```

  ### Step 4: DB 최소화 (추천)
  ```sql
  -- CLASS_NAME, SYSTEM_TYPE, SUB_SYSTEM 같은 컬럼 삭제
  ALTER TABLE SYS_PROGRAM DROP COLUMN CLASS_NAME;
  ALTER TABLE SYS_PROGRAM DROP COLUMN SYSTEM_TYPE;
  -- ...
  ```

  ## 요약

  **추천 방식으로 로드할 것!**

  `nU3ProgramInfoAttribute`를 사용하면:
  1. DLL 경로 인식
  2. 타입 자동 로드
  3. 인스턴스 자동 생성
  4. 메뉴 자동 생성 (DB 최소화)
  5. 권한 확인
  6. FormType 처리

  구현하는 것이 훨씬 간단합니다! ✅

  ## 라이선스 정보

  (c) 2024 nU3 Framework
