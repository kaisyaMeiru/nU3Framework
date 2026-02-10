  # FullClassName vs ClassName 사용 가이드

  ## 개요

  `nU3ProgramInfoAttribute`에는 두 가지 속성이 포함됩니다:
  - `FullClassName`: Namespace 포함 전체 클래스 이름 (타입 해결)
  - `ClassName`: 클래스 이름 (표준)

  ## 속성 구현

  ```csharp
  public class nU3ProgramInfoAttribute : Attribute
  {
      /// <summary>
      /// Full class name including namespace for type resolution.
      /// Example: "nU3.Modules.EMR.IN.Worklist.PatientListControl"
      /// </summary>
      public string FullClassName { get; }

      /// <summary>
      /// Simple class name without namespace.
      /// Example: "PatientListControl"
      /// </summary>
      public string ClassName => FullClassName?.Split('.').LastOrDefault() ?? string.Empty;
  }
  ```

  ## 사용 예시

  ```csharp
  namespace nU3.Modules.EMR.IN.Worklist
  {
      [nU3ProgramInfo(typeof(PatientListControl), "환자 목록", "EMR_PATIENT_LIST_001")]
      public class PatientListControl : BaseWorkControl
      {
          // 자동 설정됨:
          // FullClassName = "nU3.Modules.EMR.IN.Worklist.PatientListControl"
          // ClassName = "PatientListControl" (가장 마지막 부분)
      }
  }
  ```

  ## 사용 처리

  ### 1. FullClassName 사용 (타입 로드)

  ```csharp
  // DLL에서 타입을 찾고 로드
  var assembly = Assembly.LoadFile(dllPath);
  var type = assembly.GetType(attr.FullClassName);
  // ✅ 확실히고 타입을 찾음

  // 로그에 출력
  Console.WriteLine($"Loading: {attr.FullClassName}");
  // 결과: "Loading: nU3.Modules.EMR.IN.Worklist.PatientListControl"
  ```

  ### 2. ClassName 사용 (UI 표시)

  ```csharp
  // 목록에 클래스 이름 표시
  dataGridView.Rows.Add(new[]
  {
      attr.ProgramId,
      attr.ProgramName,
      attr.ClassName,  // ✅ UI: 간단
      attr.SystemType
  });

  // 팝업 메시지에서 표시
  toolTip.SetToolTip(button, $"클래스: {attr.ClassName}");
  // 결과: "클래스: PatientListControl"

  // 로그에 출력
  Console.WriteLine($"Opening {attr.ClassName}...");
  // 결과: "Opening PatientListControl..."
  ```

  ### 3. 검증/예외 (FullClassName)

  ```csharp
  // 올바른 예외 메시지
  var errors = new List<string>();
  if (!IsValidNamespace(attr.FullClassName))
  {
      errors.Add($"Invalid class: {attr.FullClassName}");
      // 결과: "Invalid class: nU3.Modules.EMR.IN.Worklist.PatientListControl"
  }

  // 디버그 로그
  _logger.Debug($"Resolving type: {attr.FullClassName} from {attr.DllName}");
  ```

  ### 4. 검색 (ClassName)

  ```csharp
  // 사용자가 클래스 이름으로 검색
  var searchResults = _moduleLoader.GetProgramAttributes()
      .Values
      .Where(attr => attr.ClassName.Contains(searchKeyword, StringComparison.OrdinalIgnoreCase))
      .ToList();

  // 예: "Patient" 검색
  // 예결과: PatientListControl, PatientDetailControl 포함
  ```

  ## 표준

  | 용도 | FullClassName | ClassName |
  |------|---------------|-----------|
  | 타입 해결 | ✅ 필수 | ❌ 불가 |
  | UI 표시 | ❌ 너무 길 | ✅ 간단 |
  | 로그 (간단) | ✅ 필요 | ❌ 생략 가능 |
  | 로그 (상세) | ❌ 너무 길 | ✅ 필요 |
  | 검색 | ❌ 어려움 | ✅ 용이 |
  | 태깅 | ✅ 필요 | ❌ 불가 |
  | DB 저장 | ✅ 필요 | ❌ 불가 |

  ## 사용 예시

  ### 예시 1: 프로그램 로드

  ```csharp
  public Type LoadProgram(nU3ProgramInfoAttribute attr)
  {
      // FullClassName으로 타입 로드
      var type = assembly.GetType(attr.FullClassName);

      if (type != null)
      {
          // ClassName으로 로그 출력
          Console.WriteLine($"✅ Loaded {attr.ClassName}");
      }
      else
      {
          // FullClassName으로 실패 메시지
          Console.WriteLine($"❌ Failed to load {attr.FullClassName}");
      }

      return type;
  }
  ```

  ### 예시 2: 목록 표시

  ```csharp
  // DeployManagementControl.cs
  private void DisplayProgramList()
  {
      foreach (var attr in programAttributes)
      {
          dgvPrograms.Rows.Add(new[]
          {
              attr.ProgramId,
              attr.ProgramName,
              attr.ClassName,        // ✅ UI: 간단
              attr.SystemType,
              attr.SubSystem,
              attr.FullClassName     // ✅ 상세 정보 (추가)
          });
      }
  }
  ```

  ### 예시 3: 검증

  ```csharp
  private List<string> ValidateProgram(nU3ProgramInfoAttribute attr)
  {
      var errors = new List<string>();

      // FullClassName으로 검증 수행
      if (!attr.FullClassName.StartsWith("nU3.Modules"))
      {
          errors.Add($"[{attr.ClassName}] Invalid namespace: {attr.FullClassName}");
          // 결과 "[PatientListControl] Invalid namespace: nU3.Modules.EMR.IN.Worklist.PatientListControl"
      }

      return errors;
  }
  ```

  ### 예시 4: 메뉴 생성

  ```csharp
  private TreeNode CreateMenuNode(nU3ProgramInfoAttribute attr)
  {
      var node = new TreeNode
      {
          // ClassName으로 메뉴 텍스트 표시
          Text = $"{attr.ClassName} ({attr.ProgramId})",
          // 결과: "PatientListControl (EMR_PATIENT_LIST_001)"

          // FullClassName으로 Tag 설정 (추후 사용)
          Tag = new { attr.FullClassName, attr.ProgramId },

          // ClassName으로 툴팁 텍스트
          ToolTipText = $"클래스: {attr.ClassName}\n시스템: {attr.SystemType}/{attr.SubSystem}"
      };

      return node;
  }
  ```

  ### 예시 5: 로그 출력

  ```csharp
  // 간단 로그 (ClassName)
  Console.WriteLine($"[INFO] Opening {attr.ClassName}...");
  // 결과 "[INFO] Opening PatientListControl..."

  // 상세 로그 (FullClassName)
  _logger.Debug($"Loading type: {attr.FullClassName} from {attr.DllName}.dll");
  // 결과 "Loading type: nU3.Modules.EMR.IN.Worklist.PatientListControl from nU3.Modules.EMR.IN.Worklist.dll"

  // 에러 로그 (FullClassName)
  _logger.Error($"Failed to instantiate {attr.FullClassName}: {ex.Message}");
  ```

  ## 성능 최적화

  ### ClassName 캐싱

  ```csharp
  // ClassName 단축 계산
  public string ClassName => FullClassName?.Split('.').LastOrDefault() ?? string.Empty;

  // 반복 호출 방지를 위한 캐싱
  private string _cachedClassName;
  public string GetClassName()
  {
      if (_cachedClassName == null)
          _cachedClassName = FullClassName?.Split('.').LastOrDefault() ?? string.Empty;
      return _cachedClassName;
  }
  ```

  **캐싱을 사용하는 이유:**
  - String.Split이 상대적으로 느림 (~0.001ms)
  - UI 표시 로직에서 반복 호출이 잦음
  - 메모리에 비용이 있지만 성능 이점이 큼

  ## 권장 사항

  ### DO

  ```csharp
  // 타입 해결에는 FullClassName
  var type = assembly.GetType(attr.FullClassName);

  // UI에는 ClassName
  label.Text = attr.ClassName;

  // DB에는 FullClassName 저장
  INSERT INTO SYS_PROGRAM (CLASS_NAME) VALUES (@FullClassName);

  // 검색에는 ClassName
  where attr.ClassName.Contains(keyword)

  // 상세 로그에는 FullClassName
  _logger.Debug($"Type: {attr.FullClassName}");

  // 간단 로그에는 ClassName
  Console.WriteLine($"Loaded {attr.ClassName}");
  ```

  ### DON'T

  ```csharp
  // 타입 해결에는 ClassName 금지
  var type = assembly.GetType(attr.ClassName);  // null 반환!

  // UI에는 FullClassName 표시 (너무 길)
  label.Text = attr.FullClassName;  // "nU3.Modules.EMR.IN.Worklist.PatientListControl"

  // DB에는 ClassName 저장 (불필요한 생략)
  INSERT INTO SYS_PROGRAM (CLASS_NAME) VALUES (@ClassName);  // "PatientListControl"만 저장
  ```

  ## 요약

  ### FullClassName
  - **용도**: 타입 해결, 상세 로그, DB 저장
  - **장점**: 명확하고 완전한 정보
  - **예시**: "nU3.Modules.EMR.IN.Worklist.PatientListControl"

  ### ClassName
  - **용도**: UI 표시, 간단 로그, 검색
  - **장점**: 간결하고 가독성 좋음
  - **예시**: "PatientListControl"

  ### 두 속성을 모두 사용하여 적절한 상황에 따라 선택하세요! ✅

  ## 라이선스 정보

  (c) 2024 nU3 Framework
