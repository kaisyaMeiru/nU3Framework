  # ModuleLoaderService 버전 확인 가이드

  ## 개요

  프로그램 실행 시 **ProgId, ModuleId, Version**을 확인하여 이전에 로드된 모듈이 있는지 확인하고, 필요한 경우 새 버전을 다운로드하여 로드합니다.

  ## 메인 처리 흐름

  ```
  [메뉴 클릭]
      ↓
  [ProgId, ModuleId 획득]
      ↓
  [DB에 있는 모듈 조회]
      ↓
  [업데이트 필요 여부 확인]
      ↓
  필요 시 즉시 업데이트
      ├─ [Remote Server]
      │     ├─ Download
      │     └─ [Cache]
      │           ├─ Deploy
      │           └─ [Runtime]
      │                 └─ Reload
  ```

  ## 핵심 메서드

  ### 1. **메모리에 있는 모듈 버전**

  ```csharp
  private readonly Dictionary<string, string> _loadedModuleVersions;
  // ModuleId와 Version 매핑
  ```

  **로드된 모듈을 메모리에 저장:**
  ```csharp
  _loadedModuleVersions["PROG_EMR_IN_Worklist"] = "1.0.2.0"
  ```

  ### 2. **버전 확인 메서드**

  ```csharp
  /// <summary>
  /// 프로그램 실행 시 필요한 모듈이 있는지 확인하고 업데이트할지 결정
  /// </summary>
  public bool EnsureModuleUpdated(string progId, string moduleId)
  {
      // 1. DB에 있는 활성 버전 조회
      var activeVersion = _moduleRepo.GetActiveVersions()
          .FirstOrDefault(v => v.ModuleId == moduleId);

      // 2. 로드된 모듈 확인
      if (_loadedModuleVersions.TryGetValue(moduleId, out var loadedVersion))
      {
          if (loadedVersion == activeVersion.Version)
              return true;  // 이미 최신 버전
      }

      // 3. 업데이트 필요
      return UpdateSingleModule(module, activeVersion);
  }
  ```

  ### 3. **자동 업데이트 메서드**

  ```csharp
  private bool UpdateSingleModule(ModuleMstDto module, ModuleVerDto version)
  {
      // 1. Server → Cache (Download)
      if (NeedsDownload(cacheFile, serverFile, version))
      {
          DownloadToCache(serverFile, cacheFile, module.ModuleName, version.Version);
      }

      // 2. Cache → Runtime (Deploy)
      DeployToRuntime(cacheFile, runtimeFile, module.ModuleName, version.Version);

      // 3. Reload
      ReloadModule(runtimeFile, module.ModuleId, version.Version);
  }
  ```

  ## 사용 예시

  ### 사용 예시 1: 버전 확인 및 업데이트 (기본)

  ```csharp
  // MainShellForm.cs - 메뉴 클릭 처리
  private void OnMenuItemClick(string menuId)
  {
      var menuRepo = Program.ServiceProvider.GetRequiredService<IMenuRepository>();
      var menu = menuRepo.GetMenuById(menuId);

      if (string.IsNullOrEmpty(menu.ProgId))
          return;

      // ProgId, ModuleId 얻기
      var progRepo = Program.ServiceProvider.GetRequiredService<IProgramRepository>();
      var program = progRepo.GetProgramById(menu.ProgId);

      if (program == null)
          return;

      // 버전 확인하고 필요시 업데이트
      var instance = _moduleLoader.CreateProgramInstanceWithVersionCheck(
          program.ProgId,
          program.ModuleId
      );

      if (instance is BaseWorkControl control)
      {
          OpenProgramInTab(control);
      }
  }
  ```

  ### 사용 예시 2: 업데이트 안 함 (버전 확인 안 함)

  ```csharp
  // 데모를 위한 경우
  var instance = _moduleLoader.CreateProgramInstance(progId);
  ```

  ### 사용 예시 3: 모듈 업데이트 확인

  ```csharp
  // 모듈 업데이트 확인
  if (_moduleLoader.EnsureModuleUpdated(progId, moduleId))
  {
      var instance = _moduleLoader.CreateProgramInstance(progId);
  }
  ```

  ## 성능 처리

  ### 처리 시나리오 1: 버전 확인 (단순)

  ```
  상황: 새로운 메뉴 클릭
      ↓
  시스템:
    - ProgId: "EMR_PATIENT_LIST_001"
    - ModuleId: "PROG_EMR_IN_Worklist"
    - DB Version: "1.0.2.0"
    - Loaded Version: "1.0.2.0"
      ↓
  결과: 이미 최신 배포, 즉시 로드
  시간: ~50ms
  ```

  ### 처리 시나리오 2: 업데이트 필요

  ```
  상황: 새로운 메뉴 클릭
      ↓
  시스템:
    - ProgId: "EMR_PATIENT_LIST_001"
    - ModuleId: "PROG_EMR_IN_Worklist"
    - DB Version: "1.0.3.0" (업데이트 필요!)
    - Loaded Version: "1.0.2.0"
      ↓
  자동 업데이트:
    1. [Server] → [Cache] Download (1~5초)
    2. [Cache] → [Runtime] Deploy (~100ms)
    3. Reload Assembly (~200ms)
      ↓
  결과: 업데이트 후 즉시 로드
  시간: ~1~6초
  ```

  ### 처리 시나리오 3: 최초 로드 (DLL 다운로드)

  ```
  상황: 새로운 모듈 메뉴
      ↓
  시스템:
    - ProgId: "NEW_MODULE_001"
    - ModuleId: "PROG_EMR_NEW_Module"
    - Loaded Version: null (최초 로드!)
      ↓
  자동 다운로드:
    1. [Server] → [Cache] Download
    2. [Cache] → [Runtime] Deploy
    3. Load Assembly
      ↓
  결과: 다운로드 후 즉시 로드
  ```

  ## DB 키워드 활용

  ### SYS_MODULE_MST (모듈 마스터)

  ```sql
  SELECT
      MODULE_ID,      -- "PROG_EMR_IN_Worklist"
      MODULE_NAME,    -- "환자 목록 폼"
      CATEGORY,       -- "EMR"
      SUB_SYSTEM,     -- "IN"
      FILE_NAME       -- "nU3.Modules.EMR.IN.Worklist.dll"
  FROM SYS_MODULE_MST
  WHERE MODULE_ID = 'PROG_EMR_IN_Worklist';
  ```

  ### SYS_MODULE_VER (버전 마스터)

  ```sql
  SELECT
      MODULE_ID,      -- "PROG_EMR_IN_Worklist"
      VERSION,        -- "1.0.3.0"
      FILE_HASH,      -- "a1b2c3d4..."
      FILE_SIZE,      -- 245760
      STORAGE_PATH,   -- "D:\ServerStorage\EMR\IN\..."
      IS_ACTIVE       -- "Y"
  FROM SYS_MODULE_VER
  WHERE MODULE_ID = 'PROG_EMR_IN_Worklist'
    AND IS_ACTIVE = 'Y'
  ORDER BY REG_DATE DESC
  LIMIT 1;
  ```

  ### SYS_PROGRAM (프로그램 마스터)

  ```sql
  SELECT
      PROG_ID,        -- "EMR_PATIENT_LIST_001"
      MODULE_ID,      -- "PROG_EMR_IN_Worklist"
      PROG_NAME,      -- "환자 목록"
      IS_ACTIVE       -- "Y"
  FROM SYS_PROGRAM
  WHERE PROG_ID = 'EMR_PATIENT_LIST_001';
  ```

  ## 메뉴 동적 로딩

  ### 최적화 (단순)

  ```csharp
  // DB 조회 최적화
  foreach (var menu in menus)
  {
      var program = progRepo.GetProgram(menu.ProgId);  // DB 조회
      var module = moduleRepo.GetModule(program.ModuleId);  // DB 조회

      CreateMenuItem(menu.MenuName, program.ProgName);
  }
  ```

  ### 최적화 (고급)

  ```csharp
  // 메뉴와 관련 정보 조회 최적화
  var menus = menuRepo.GetAllMenus();
  var programs = progRepo.GetAllPrograms()
      .ToDictionary(p => p.ProgId);
  var attrs = _moduleLoader.GetProgramAttributes();

  foreach (var menu in menus)
  {
      if (programs.TryGetValue(menu.ProgId, out var program))
      {
          if (attrs.TryGetValue(menu.ProgId, out var attr))
          {
              // DB 조회를 필요로 하는 경우 메뉴 정보만 생성
              CreateMenuItem(
                  menu.MenuName,
                  attr.ProgramName,
                  program.ModuleId,  // 버전 확인 가능
                  attr.SystemType,
                  attr.AuthLevel
              );
          }
      }
  }
  ```

  ## 로그

  ### 버전 확인 로그

  ```
  [ModuleLoader] Checking version for PROG_EMR_IN_Worklist
  [ModuleLoader] DB Version: 1.0.3.0
  [ModuleLoader] Loaded Version: 1.0.2.0
  [ModuleLoader] Update needed
  ```

  ### 다운로드 로그

  ```
  [ModuleLoader] Downloading: 환자 목록 폼 v1.0.3.0
  [ModuleLoader] Server: D:\ServerStorage\EMR\IN\nU3.Modules.EMR.IN.Worklist.dll
  [ModuleLoader] Cache: C:\Users\...\AppData\...\Cache\EMR\IN\...
  [ModuleLoader] Downloaded to cache: 환자 목록 폼 v1.0.3.0
  ```

  ### 배포 로그

  ```
  [ModuleLoader] Deploying: 환자 목록 폼 v1.0.3.0
  [ModuleLoader] Cache: C:\Users\...\Cache\EMR\IN\...
  [ModuleLoader] Runtime: C:\Program Files\nU3.Shell\Modules\EMR\IN\...
  [ModuleLoader] Deployed to runtime: 환자 목록 폼 v1.0.3.0
  ```

  ### 로드 로그

  ```
  [ModuleLoader] Reloaded: EMR_PATIENT_LIST_001 (v1.0.3.0)
  [ModuleLoader] Created instance: EMR_PATIENT_LIST_001 (Module: PROG_EMR_IN_Worklist)
  ```

  ## 오류 처리

  ### 1. 서버 파일 존재 안 함

  ```csharp
  if (!File.Exists(serverFile))
  {
      MessageBox.Show(
          "필요한 서버 파일을 찾을 수 없습니다.\n관리자에게 문의하세요.",
          "모듈 다운로드 실패",
          MessageBoxButtons.OK,
          MessageBoxIcon.Error
      );
      return null;
  }
  ```

  ### 2. 다운로드 실패 (권한 문제)

  ```csharp
  catch (IOException ex)
  {
      MessageBox.Show(
          "파일 다운로드에 실패했습니다.\n프로그램을 관리자 권한으로 실행하면 업데이트가 가능합니다.",
          "업데이트 실패",
          MessageBoxButtons.OK,
          MessageBoxIcon.Information
      );
  }
  ```

  ### 3. 버전 확인 실패

  ```csharp
  if (activeVersion == null)
  {
      MessageBox.Show(
          "활성 버전을 찾을 수 없습니다.",
          "버전 확인 실패",
          MessageBoxButtons.OK,
          MessageBoxIcon.Warning
      );
  }
  ```

  ## 병렬 처리 최적화

  ### 1. 비동기 업데이트 (최신)

  ```csharp
  public async Task<bool> EnsureModuleUpdatedAsync(string progId, string moduleId)
  {
      // 병렬로 업데이트
      await Task.Run(() => UpdateSingleModule(module, version));
  }
  ```

  ### 2. 병렬 업데이트 (Bootstrap)

  ```csharp
  public void CheckAndUpdateModulesParallel()
  {
      var modules = _moduleRepo.GetAllModules();

      Parallel.ForEach(modules, new ParallelOptions { MaxDegreeOfParallelism = 4 },
          module =>
          {
              // 병렬 업데이트
              UpdateSingleModule(module, GetActiveVersion(module.ModuleId));
          });
  }
  ```

  ### 3. 캐시 미리 로드

  ```csharp
  // 프로그램 시작 전 모든 DLL 미리 로드
  _moduleLoader.LoadAllModules();
  // 사용할 때마다 메모리 캐시에서 즉시 로드
  ```

  ## 플러그인 프로그래밍 체크리스트

  ### 핵심 업데이트

  - [ ] 메뉴 클릭 처리 최적화
  - [ ] `CreateProgramInstance()` → `CreateProgramInstanceWithVersionCheck()` 변경
  - [ ] ProgId + ModuleId 조합 사용
  - [ ] 오류 처리 추가

  ### DB 키워드 확인

  - [ ] `SYS_MODULE_VER` 테이블 확인
  - [ ] `VERSION` 컬럼 확인
  - [ ] `FILE_HASH` 컬럼 확인
  - [ ] `IS_ACTIVE` 컬럼 확인

  ### 테스트

  - [ ] 버전 확인 (업데이트 필요 시)
  - [ ] 자동 업데이트 (인터넷 필요)
  - [ ] 최초 로드 (새 모듈)
  - [ ] 업데이트 성공 시 로그
  - [ ] 업데이트 실패 시 오류 처리

  ## 요약

  **합리적인 버전 확인 시스템!**

  - **ProgId**: 프로그램 식별
  - **ModuleId**: 모듈 식별
  - **Version**: 버전 확인 및 자동 업데이트 메커니즘

  ```
  메뉴 클릭 시 버전 확인 및 자동 업데이트 시스템 활성화
  ```

  개발자는 **항상 최신 버전**을 사용하여 프로그램을 실행합니다!

  ## 라이선스 정보

  (c) 2024 nU3 Framework
