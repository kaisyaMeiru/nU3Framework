  # nU3ProgramInfoAttribute 사용 가이드

  ## 개요

  `nU3ProgramInfoAttribute`는 프로그램에 대한 기본 정보를 저장하는 속성입니다.
  - 네임스페이스 정보를 자동으로 분석하여 시스템 타입과 하위 시스템을 추출
  - `BaseWorkControl`의 `ProgramID`와 `ProgramTitle`을 자동으로 설정

  ## 주요 속성

  ### 1. 네임스페이스 자동 분석
  `typeof(클래스이름)`를 지정하면 네임스페이스에서 `SystemType`과 `SubSystem`을 자동으로 분석합니다.

  ### 2. 속성 자동 설정
  `BaseWorkControl`의 `ProgramID`와 `ProgramTitle`을 자동으로 `nU3ProgramInfo` 속성으로 설정합니다.

  ## 사용 예시

  ### 정확한 사용 방식 (자동 설정)

  ```csharp
  namespace nU3.Modules.ADM.AD.Deployer
  {
      // typeof 지정 - SystemType과 SubSystem 자동 분석
      // ProgramID와 ProgramTitle 자동 설정
      [nU3ProgramInfo(typeof(DeployerWorkControl), "ADM AD Deployer", "ADM_AD_00001")]
      public class DeployerWorkControl : BaseWorkControl
      {
          public DeployerWorkControl()
          {
              // ProgramID와 ProgramTitle은 프로그램에서 직접 설정해야 합니다!
              // 자동으로 nU3ProgramInfo 속성이 설정됩니다

              var label = new Label
              {
                  Dock = DockStyle.Fill,
                  Text = "ADM AD Deployer module"
              };
              Controls.Add(label);
          }
      }
  }
  ```

  **자동 설정 내용:**
  - `SystemType`: `"ADM"` (네임스페이스 자동 분석)
  - `SubSystem`: `"AD"` (네임스페이스 자동 분석)
  - `ProgramID`: `"ADM_AD_00001"` (BaseWorkControl에서 자동 설정)
  - `ProgramTitle`: `"ADM AD Deployer"` (BaseWorkControl에서 자동 설정)

  ### 편리한 사용 방식 (검증)

  ```csharp
  namespace nU3.Modules.ADM.AD.Deployer
  {
      [nU3ProgramInfo("ADM", "ADM AD Deployer", "ADM_AD_00001")]
      public class DeployerWorkControl : BaseWorkControl
      {
          public DeployerWorkControl()
          {
              // 수동으로 중복 설정!
              ProgramID = "ADM_AD_DEPLOYER";
              ProgramTitle = "ADM Deployer";

              // ...
          }
      }
  }
  ```

  ## 네임스페이스 규칙

  네임스페이스 구조는 다음과 같습니다:

  ```
  nU3.Modules.{SystemType}.{SubSystem}.{ModuleName}
  ```

  ## 예시 분석

  ### 예시 1: 배포 프로그램

  ```csharp
  namespace nU3.Modules.ADM.AD.Deployer
  {
      /// <summary>
      /// 배포 프로그램 화면
      /// - SystemType: "ADM" (자동)
      /// - SubSystem: "AD" (자동)
      /// - ProgramID: "ADM_AD_00001" (자동)
      /// - ProgramTitle: "배포 프로그램" (자동)
      /// </summary>
      [nU3ProgramInfo(typeof(DeployerWorkControl), "배포 프로그램", "ADM_AD_00001")]
      public class DeployerWorkControl : BaseWorkControl
      {
          public DeployerWorkControl()
          {
              // ProgramID, ProgramTitle 직접 설정 가능!
              InitializeLayout();
          }
      }
  }
  ```

  ### 예시 2: EMR 화면

  ```csharp
  namespace nU3.Modules.EMR.IN.Worklist
  {
      /// <summary>
      /// 입원 환자 목록
      /// - SystemType: "EMR" (자동)
      /// - SubSystem: "IN" (자동)
      /// - ProgramID: "EMR_PATIENT_LIST_001" (자동)
      /// - ProgramTitle: "환자 목록" (자동)
      /// </summary>
      [nU3ProgramInfo(typeof(PatientListControl), "환자 목록", "EMR_PATIENT_LIST_001")]
      public class PatientListControl : BaseWorkControl
      {
          public PatientListControl()
          {
              // 속성에 의해 자동으로 설정됩니다
              InitializeLayout();
              LoadSampleData();
          }
      }
  }
  ```

  ### 예시 3: 팝업

  ```csharp
  namespace nU3.Modules.EMR.OP.Registration
  {
      /// <summary>
      /// 외래 환자 목록 팝업
      /// FormType은 "POPUP"으로 설정
      /// </summary>
      [nU3ProgramInfo(typeof(PatientRegisterPopup), "환자 목록", "EMR_OP_REG_POPUP", "POPUP")]
      public class PatientRegisterPopup : BaseWorkControl
      {
          // FormType = "POPUP"
          // SystemType = "EMR"
          // SubSystem = "OP"
      }
  }
  ```

  ### 예시 4: 인증 수준 다름

  ```csharp
  namespace nU3.Modules.ADM.AD.UserManagement
  {
      /// <summary>
      /// 관리자 화면 (관리자만 사용)
      /// AuthLevel = 0 (관리자 권한)
      /// </summary>
      [nU3ProgramInfo(typeof(UserManagementControl), "관리자 화면", "ADM_AD_USER_001", AuthLevel = 0)]
      public class UserManagementControl : BaseWorkControl
      {
          // AuthLevel = 0 (관리자 권한)
      }
  }
  ```

  ## BaseWorkControl 속성

  `BaseWorkControl`에 상속되어 있는 속성을 자동으로 설정합니다:

  ```csharp
  public class YourControl : BaseWorkControl
  {
      // 자동으로 nU3ProgramInfo 속성을 따름
      public override string ProgramID { get; }

      // 자동으로 nU3ProgramInfo 속성을 따름
      public override string ProgramTitle { get; }

      public YourControl()
      {
          // ProgramID = "...";     // 직접 설정
          // ProgramTitle = "...";  // 직접 설정

          // 직접 설정할 필요 없음
          InitializeLayout();
      }
  }
  ```

  ## 속성 생성자

  ### 정확한 생성자 (클래스 타입 지정)

  ```csharp
  public nU3ProgramInfoAttribute(
      Type declaringType,        // typeof(클래스이름)
      string programName,        // 프로그램 표시 이름
      string programId,          // 식별 프로그램 ID
      string formType = "CHILD"  // CHILD, POPUP, SDI
  )
  ```

  ### 편리한 생성자 (문자열 지정)

  ```csharp
  public nU3ProgramInfoAttribute(
      string systemType,         // 시스템 타입 (예: "EMR", "ADM")
      string programName,        // 프로그램 표시 이름
      string programId,          // 식별 프로그램 ID
      string formType = "CHILD"  // CHILD, POPUP, SDI
  )
  ```

  ## 속성 값

  ### 자동 설정 속성

  | 속성 | 소스 | 설명 |
  |------|------|------|
  | `ProgramID` | nU3ProgramInfo | BaseWorkControl에서 자동 설정 |
  | `ProgramTitle` | nU3ProgramInfo | BaseWorkControl에서 자동 설정 |
  | `SystemType` | Namespace | 네임스페이스에서 자동 분석 |
  | `SubSystem` | Namespace | 네임스페이스에서 자동 분석 |

  ### 옵션 속성

  | 속성 | 타입 | 기본값 | 설명 |
  |------|------|--------|------|
  | `FormType` | string | "CHILD" | 폼 타입: CHILD, POPUP, SDI |
  | `AuthLevel` | int | 1 | 권한 수준 (0=관리자, 1~99=일반) |
  | `IsUse` | bool | true | 활성화 여부 |

  ## 네임스페이스 규칙 상세

  ### 시스템 타입 (SystemType)

  - **EMR**: Electronic Medical Record (전자 의무 기록)
  - **ADM**: Administration (행정)
  - **NUR**: Nursing (간호)
  - **LAB**: Laboratory (검사)
  - **RAD**: Radiology (방사선 영상)
  - **PHA**: Pharmacy (약국)
  - **BIL**: Billing (청구)

  ### 하위 시스템 (SubSystem)

  - **IN**: Inpatient (입원)
  - **OP**: Outpatient (외래)
  - **ER**: Emergency Room (응급실)
  - **AD**: Administration (행정)
  - **SCH**: Schedule (일정 스케줄)
  - **WL**: Worklist (워크리스트)

  ### 네임스페이스 구조 예시

  ```
  nU3.Modules.{System}.{SubSystem}.{Module}

  예시:
  nU3.Modules.EMR.IN.Worklist          → 입원 워크리스트
  nU3.Modules.EMR.OP.Registration      → 외래 등록
  nU3.Modules.ADM.AD.UserManagement    → 관리자 관리 - 사용자 관리
  nU3.Modules.NUR.IN.VitalSigns        → 간호 - 입원 생체 신호 관리
  nU3.Modules.LAB.OP.OrderEntry        → 검사 - 외래 검사 지시 입력
  ```

  ## 플러그인 프로그래밍 가이드

  ### Before (기존 방식)

  ```csharp
  namespace nU3.Modules.ADM.AD.Deployer
  {
      [nU3ProgramInfo("ADM", "ADM AD Deployer", "ADM_AD_DEPLOYER")]
      public class DeployerWorkControl : BaseWorkControl
      {
          public DeployerWorkControl()
          {
              ProgramID = "ADM_AD_DEPLOYER";      // 중복
              ProgramTitle = "ADM Deployer";       // 중복
              // ...
          }
      }
  }
  ```

  ### After (개선 방식)

  ```csharp
  namespace nU3.Modules.ADM.AD.Deployer
  {
      [nU3ProgramInfo(typeof(DeployerWorkControl), "ADM AD Deployer", "ADM_AD_DEPLOYER")]
      public class DeployerWorkControl : BaseWorkControl
      {
          public DeployerWorkControl()
          {
              // ProgramID, ProgramTitle은 직접 설정 가능!
              // SystemType, SubSystem은 자동 설정!
              // ...
          }
      }
  }
  ```

  ### 개선점

  1. **자동 설정**: ProgramID와 ProgramTitle 중복 방지
  2. **타입 안전**: SystemType과 SubSystem을 입력하여 안전
  3. **규칙 준수**: 네임스페이스 속성에서 규칙 준수
  4. **검증 용이**: 모든 정보가 한 속성(nU3ProgramInfo)에 저장
  5. **유지보수성**: 네임스페이스 규칙을 준수하면 안전

  ## 속성 확인

  ```csharp
  var attr = typeof(DeployerWorkControl).GetCustomAttribute<nU3ProgramInfoAttribute>();
  if (attr != null)
  {
      Console.WriteLine($"SystemType: {attr.SystemType}");      // "ADM"
      Console.WriteLine($"SubSystem: {attr.SubSystem}");        // "AD"
      Console.WriteLine($"ProgramName: {attr.ProgramName}");    // "ADM AD Deployer"
      Console.WriteLine($"ProgramId: {attr.ProgramId}");        // "ADM_AD_00001"
      Console.WriteLine($"FormType: {attr.FormType}");          // "CHILD"
  }

  // BaseWorkControl 인스턴스에서도 속성 확인
  var control = new DeployerWorkControl();
  Console.WriteLine($"ProgramID: {control.ProgramID}");        // "ADM_AD_00001"
  Console.WriteLine($"ProgramTitle: {control.ProgramTitle}");  // "ADM AD Deployer"
  ```

  ## 팁

  1. **네임스페이스 규칙 준수**: `nU3.Modules.{SystemType}.{SubSystem}` 형식을 사용하여 자동 분석을 할 수 있습니다.
  2. **typeof 지정**: 자동 설정을 하려면 첫 번째 매개변수로 `typeof(클래스이름)`를 지정해야 합니다.
  3. **BaseWorkControl 상속**: `ProgramID`와 `ProgramTitle`은 `BaseWorkControl`에서 재정의하여 사용합니다.
  4. **수동 설정 허용**: 필요한 경우 `ProgramID`와 `ProgramTitle`을 직접 설정할 수 있습니다.

  ## FAQ

  ### Q: ProgramID를 직접 설정할 수 있나요?
  A: 네, 프로그램 개발 시 직접 설정할 수 있습니다:

  ```csharp
  [nU3ProgramInfo(typeof(MyControl), "내 컨트롤", "MY_CTRL_001")]
  public class MyControl : BaseWorkControl
  {
      public MyControl()
      {
          // 특별한 설정이 필요할 때 직접 설정
          ProgramID = "CUSTOM_ID";
      }
  }
  ```

  ### Q: 네임스페이스 규칙이 다르다면 어떻게 하나요?
  A: `SystemType`은 "COMMON"으로 설정하고 `SubSystem`은 null로 설정합니다.

  ### Q: 생성자로 직접 설정할 수 있나요?
  A: 네, 속성의 필수 매개변수는 문자열로 직접 설정할 수 있습니다.

  ## 라이선스 정보

  (c) 2024 nU3 Framework
