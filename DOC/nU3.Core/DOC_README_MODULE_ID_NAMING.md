  # ModuleId 명명 규칙 가이드

  ## 개요

  ModuleId 명명 규칙은 DLL 전체 이름에서 **마지막 부분의 DLL 이름**을 사용합니다.

  ## 명명 패턴

  ### 단순 패턴 (간단)
  ```
  PROG_{SystemType}_{SubSystem}_{FullDllName}
  ```

  **예시:**
  ```
  DLL: nU3.Modules.EMR.IN.Worklist.dll
  ModuleId: PROG_EMR_IN_nU3.Modules.EMR.IN.Worklist  ⚠️ (너무 길고 중복)
  ```

  ### 개선 패턴 (권장)
  ```
  PROG_{SystemType}_{SubSystem}_{SimpleDllName}
  ```

  **예시:**
  ```
  DLL: nU3.Modules.EMR.IN.Worklist.dll
  ModuleId: PROG_EMR_IN_Worklist  ✅ (명확하고 간결)
  ```

  ## 장점 비교

  ### 1. **가독성**
  ```
  기존: PROG_EMR_IN_nU3.Modules.EMR.IN.Worklist (43자)
  개선: PROG_EMR_IN_Worklist (21자)
  ```

  ### 2. **명확성**
  ```
  ✅ PROG_EMR_IN_Worklist
     전체 시스템: EMR
     하위 시스템: IN
     모듈 이름: Worklist

  ❌ PROG_EMR_IN_nU3.Modules.EMR.IN.Worklist
     전체 시스템: EMR (중복)
     하위 시스템: IN (중복)
     모듈 이름: nU3.Modules.EMR.IN.Worklist (중복 및 길이)
  ```

  ### 3. **유연성**
  ```
  DLL 이름 패턴: nU3.Modules.{System}.{SubSystem}.{Module}.dll
  ModuleId 패턴: PROG_{System}_{SubSystem}_{Module}

  ✅ 모듈 이름(Module) 부분이 ModuleId로 사용
  ✅ DLL 파일 이름과 매핑 가능
  ```

  ## 예시 실현

  ### 예시 1: EMR 시스템

  **DLL 형식:**
  ```
  nU3.Modules.EMR.IN.Worklist.dll
  ```

  **ModuleId 생성:**
  ```csharp
  // SimpleDllName 형식
  var dllName = "nU3.Modules.EMR.IN.Worklist";
  var parts = dllName.Split('.');
  var simpleName = parts[4];  // "Worklist"

  // ModuleId 생성
  var moduleId = $"PROG_EMR_IN_{simpleName}";
  // 결과: "PROG_EMR_IN_Worklist"
  ```

  ### 예시 2: ADM 시스템

  **DLL 형식:**
  ```
  nU3.Modules.ADM.AD.Deployer.dll
  ```

  **ModuleId:**
  ```
  PROG_ADM_AD_Deployer
  ```

  ### 예시 3: 다른 시스템

  | DLL 형식 | SystemType | SubSystem | SimpleName | ModuleId |
  |----------|------------|-----------|------------|----------|
  | nU3.Modules.EMR.IN.Worklist.dll | EMR | IN | Worklist | PROG_EMR_IN_Worklist |
  | nU3.Modules.EMR.OP.Clinic.dll | EMR | OP | Clinic | PROG_EMR_OP_Clinic |
  | nU3.Modules.ADM.AD.Deployer.dll | ADM | AD | Deployer | PROG_ADM_AD_Deployer |
  | nU3.Modules.NUR.IN.NursingStation.dll | NUR | IN | NursingStation | PROG_NUR_IN_NursingStation |

  ## 속성 구현

  ### 1. nU3ProgramInfoAttribute

  ```csharp
  public class nU3ProgramInfoAttribute : Attribute
  {
      /// <summary>
      /// DLL 전체 이름
      /// 예: "nU3.Modules.EMR.IN.Worklist"
      /// </summary>
      public string DllName { get; }

      /// <summary>
      /// 단순 DLL 이름 (마지막 부분)
      /// 예: "Worklist"
      /// </summary>
      public string SimpleDllName { get; }

      public nU3ProgramInfoAttribute(Type declaringType, ...)
      {
          this.DllName = declaringType.Assembly.GetName().Name;

          // SimpleDllName 설정
          var dllParts = this.DllName.Split('.');
          this.SimpleDllName = dllParts.Length >= 5
              ? dllParts[4]                    // 5번째 부분
              : dllParts.LastOrDefault()       // 마지막 부분
              ?? this.DllName;                 // fallback
      }

      /// <summary>
      /// ModuleId 생성
      /// 형식: PROG_{SystemType}_{SubSystem}_{SimpleDllName}
      /// </summary>
      public string GetModuleId()
      {
          return $"PROG_{SystemType}_{SubSystem}_{SimpleDllName}";
      }
  }
  ```

  ### 2. DllMetadataParser

  ```csharp
  public ParsedModuleInfo Parse(string dllPath)
  {
      // Naming Pattern: nU3.Modules.{System}.{SubSys}.{Name}.dll
      var match = NamingPattern.Match(fileName);

      if (match.Success)
      {
          var systemType = match.Groups[1].Value;  // EMR
          var subSystem = match.Groups[2].Value;   // IN
          var moduleName = match.Groups[3].Value;  // Worklist

          // ModuleId = PROG_{System}_{SubSystem}_{ModuleName}
          result.ModuleId = $"PROG_{systemType}_{subSystem}_{moduleName}";
      }
  }
  ```

  ## 역추적 구현

  ### DLL 이름으로 역추적

  ```csharp
  // ModuleId에서 DLL 이름 역추적
  var moduleId = "PROG_EMR_IN_Worklist";

  // 분해
  var parts = moduleId.Split('_');
  var systemType = parts[1];      // EMR
  var subSystem = parts[2];       // IN
  var simpleName = parts[3];      // Worklist

  // DLL 이름 복원
  var dllName = $"nU3.Modules.{systemType}.{subSystem}.{simpleName}.dll";
  // 결과: "nU3.Modules.EMR.IN.Worklist.dll"

  // 어셈블리 로드
  var dllPath = Path.Combine(_runtimePath, systemType, subSystem, dllName);
  // 결과: "C:\...\Modules\EMR\IN\nU3.Modules.EMR.IN.Worklist.dll"
  ```

  ### Attribute로 역추적

  ```csharp
  var attr = moduleLoader.GetProgramAttribute(progId);

  // DLL 이름으로 역추적
  var dllPath = attr.GetExpectedDllPath();
  // 결과: "EMR/IN/nU3.Modules.EMR.IN.Worklist.dll"

  // ModuleId로 역추적
  var moduleId = attr.GetModuleId();
  // 결과: "PROG_EMR_IN_Worklist"
  ```

  ## 플러그인 프로그래밍 체크리스트

  ### 기존 ModuleId 업데이트

  ```sql
  -- 기존 ModuleId 확인
  SELECT MODULE_ID, FILE_NAME FROM SYS_MODULE_MST;

  -- 기존 데이터:
  -- MODULE_ID: PROG_EMR_IN_nU3.Modules.EMR.IN.Worklist
  -- FILE_NAME: nU3.Modules.EMR.IN.Worklist.dll

  -- 개선된 ModuleId로 업데이트
  UPDATE SYS_MODULE_MST
  SET MODULE_ID = 'PROG_EMR_IN_Worklist'
  WHERE MODULE_ID = 'PROG_EMR_IN_nU3.Modules.EMR.IN.Worklist';

  -- SYS_MODULE_VER도 업데이트하고 연결
  UPDATE SYS_MODULE_VER
  SET MODULE_ID = 'PROG_EMR_IN_Worklist'
  WHERE MODULE_ID = 'PROG_EMR_IN_nU3.Modules.EMR.IN.Worklist';

  -- SYS_PROGRAM도 업데이트하고 연결
  UPDATE SYS_PROGRAM
  SET MODULE_ID = 'PROG_EMR_IN_Worklist'
  WHERE MODULE_ID = 'PROG_EMR_IN_nU3.Modules.EMR.IN.Worklist';
  ```

  ### 자동 업데이트 스크립트

  ```sql
  -- 기존 ModuleId를 개선된 형식으로 교체
  UPDATE SYS_MODULE_MST
  SET MODULE_ID =
      CASE
          WHEN MODULE_ID LIKE 'PROG_%_%_nU3.Modules.%' THEN
              'PROG_' +
              SUBSTRING(MODULE_ID, 6, CHARINDEX('_', MODULE_ID, 6) - 6) + '_' +  -- SystemType
              SUBSTRING(MODULE_ID, CHARINDEX('_', MODULE_ID, 6) + 1,
                        CHARINDEX('_', MODULE_ID, CHARINDEX('_', MODULE_ID, 6) + 1) -
                        CHARINDEX('_', MODULE_ID, 6) - 1) + '_' +  -- SubSystem
              REVERSE(SUBSTRING(REVERSE(MODULE_ID), 1,
                               CHARINDEX('.', REVERSE(MODULE_ID)) - 1))  -- SimpleName
          ELSE MODULE_ID
      END
  WHERE MODULE_ID LIKE 'PROG_%_%_nU3.Modules.%';
  ```

  ## 검증

  ### ModuleId 형식 검증

  ```csharp
  public bool IsValidModuleId(string moduleId)
  {
      // 형식: PROG_{SystemType}_{SubSystem}_{SimpleName}
      var pattern = @"^PROG_[A-Z]+_[A-Z]+_[A-Za-z]+$";
      return Regex.IsMatch(moduleId, pattern);
  }

  // 예시
  IsValidModuleId("PROG_EMR_IN_Worklist");  // ✅ true
  IsValidModuleId("PROG_EMR_IN_nU3.Modules.EMR.IN.Worklist");  // ❌ false
  ```

  ### DLL 이름 복원 검증

  ```csharp
  public string ReconstructDllName(string moduleId)
  {
      var parts = moduleId.Split('_');
      if (parts.Length != 4 || parts[0] != "PROG")
          throw new ArgumentException("Invalid ModuleId format");

      var systemType = parts[1];
      var subSystem = parts[2];
      var simpleName = parts[3];

      return $"nU3.Modules.{systemType}.{subSystem}.{simpleName}.dll";
  }

  // 예시
  ReconstructDllName("PROG_EMR_IN_Worklist");
  // 결과: "nU3.Modules.EMR.IN.Worklist.dll" ✅
  ```

  ## 비교

  | 항목 | 기존 (DLL 전체 이름) | 개선 (Simple DLL) |
  |------|---------------------|---------------------|
  | 길이 | 43자 | 21자 |
  | 가독성 | 어려움 (중복) | 용이 (명확) |
  | 유연성 | 낮음 | 높음 |
  | DB 검색 | 어려움 | 용이 |
  | UI 표시 | 어려움 | 용이 |
  | 확장성 | 복잡 | 간단 |

  ## 요약

  **ModuleId는 간결하고 명확하게 작성합니다!**

  ```
  PROG_{SystemType}_{SubSystem}_{SimpleDllName}
  ```

  - **간결**: 중복 제거로 명확
  - **명확**: 시스템/하위시스템/모듈 명시
  - **명명 유연**: DLL 이름과 매핑 용이
  - **활용성**: DB 검색 용이

  ## 라이선스 정보

  (c) 2024 nU3 Framework
