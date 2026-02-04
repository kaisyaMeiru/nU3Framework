# FullClassName vs ClassName 사용 가이드

## 개요

`nU3ProgramInfoAttribute`는 두 가지 클래스명 속성을 제공합니다:
- `FullClassName`: Namespace 포함 전체 경로 (타입 해상도용)
- `ClassName`: 클래스명만 (표시용)

## 속성 정의

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

## 자동 설정

```csharp
namespace nU3.Modules.EMR.IN.Worklist
{
    [nU3ProgramInfo(typeof(PatientListControl), "환자 목록", "EMR_PATIENT_LIST_001")]
    public class PatientListControl : BaseWorkControl
    {
        // 자동으로 설정됨:
        // FullClassName = "nU3.Modules.EMR.IN.Worklist.PatientListControl"
        // ClassName = "PatientListControl" (계산 속성)
    }
}
```

## 사용 시나리오

### 1. ? FullClassName 사용 (타입 로딩)

```csharp
// DLL에서 타입을 찾을 때
var assembly = Assembly.LoadFile(dllPath);
var type = assembly.GetType(attr.FullClassName);
// ? 정확하게 타입을 찾음

// 로그에 상세 정보 기록
Console.WriteLine($"Loading: {attr.FullClassName}");
// → "Loading: nU3.Modules.EMR.IN.Worklist.PatientListControl"
```

### 2. ? ClassName 사용 (UI 표시)

```csharp
// 그리드에 클래스명 표시
dataGridView.Rows.Add(new[] 
{
    attr.ProgramId,
    attr.ProgramName,
    attr.ClassName,  // ? 간결한 표시
    attr.SystemType
});

// 툴팁에 간단한 정보 표시
toolTip.SetToolTip(button, $"클래스: {attr.ClassName}");
// → "클래스: PatientListControl"

// 로그에 요약 정보
Console.WriteLine($"Opening {attr.ClassName}...");
// → "Opening PatientListControl..."
```

### 3. ? 검증/디버깅 (FullClassName)

```csharp
// 검증 오류 메시지
var errors = new List<string>();
if (!IsValidNamespace(attr.FullClassName))
{
    errors.Add($"Invalid class: {attr.FullClassName}");
    // → "Invalid class: nU3.Modules.EMR.IN.Worklist.PatientListControl"
}

// 상세 로깅
_logger.Debug($"Resolving type: {attr.FullClassName} from {attr.DllName}");
```

### 4. ? 검색 (ClassName)

```csharp
// 사용자가 클래스명으로 검색
var searchResults = _moduleLoader.GetProgramAttributes()
    .Values
    .Where(attr => attr.ClassName.Contains(searchKeyword, StringComparison.OrdinalIgnoreCase))
    .ToList();

// 예: "Patient" 검색
// → PatientListControl, PatientDetailControl 등 발견
```

## 비교표

| 용도 | FullClassName | ClassName |
|------|---------------|-----------|
| 타입 로딩 | ? 필수 | ? 불가 |
| UI 표시 | ?? 너무 길음 | ? 권장 |
| 로그 (상세) | ? 권장 | ?? 정보 부족 |
| 로그 (요약) | ?? 너무 길음 | ? 권장 |
| 검색 | ?? 복잡함 | ? 권장 |
| 디버깅 | ? 권장 | ?? 정보 부족 |
| DB 저장 | ? 권장 | ? 부족 |

## 실제 예시

### 예시 1: 프로그램 로딩

```csharp
public Type LoadProgram(nU3ProgramInfoAttribute attr)
{
    // ? FullClassName으로 타입 로드
    var type = assembly.GetType(attr.FullClassName);
    
    if (type != null)
    {
        // ? ClassName으로 간단한 로그
        Console.WriteLine($"? Loaded {attr.ClassName}");
    }
    else
    {
        // ? FullClassName으로 상세한 오류 메시지
        Console.WriteLine($"? Failed to load {attr.FullClassName}");
    }
    
    return type;
}
```

### 예시 2: 그리드 표시

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
            attr.ClassName,        // ? UI: 간결
            attr.SystemType,
            attr.SubSystem,
            attr.FullClassName     // ? 상세 정보 (숨김 가능)
        });
    }
}
```

### 예시 3: 검증 메시지

```csharp
private List<string> ValidateProgram(nU3ProgramInfoAttribute attr)
{
    var errors = new List<string>();
    
    // ? FullClassName으로 정확한 검증
    if (!attr.FullClassName.StartsWith("nU3.Modules"))
    {
        errors.Add($"[{attr.ClassName}] Invalid namespace: {attr.FullClassName}");
        // → "[PatientListControl] Invalid namespace: nU3.Modules.EMR.IN.Worklist.PatientListControl"
    }
    
    return errors;
}
```

### 예시 4: 메뉴 구성

```csharp
private TreeNode CreateMenuNode(nU3ProgramInfoAttribute attr)
{
    var node = new TreeNode
    {
        // ? ClassName으로 간결한 표시
        Text = $"{attr.ClassName} ({attr.ProgramId})",
        // → "PatientListControl (EMR_PATIENT_LIST_001)"
        
        // ? FullClassName을 Tag에 저장 (나중에 사용)
        Tag = new { attr.FullClassName, attr.ProgramId },
        
        // ? ClassName으로 간단한 툴팁
        ToolTipText = $"클래스: {attr.ClassName}\n시스템: {attr.SystemType}/{attr.SubSystem}"
    };
    
    return node;
}
```

### 예시 5: 로깅 전략

```csharp
// 요약 로그 (ClassName)
Console.WriteLine($"[INFO] Opening {attr.ClassName}...");
// → "[INFO] Opening PatientListControl..."

// 상세 로그 (FullClassName)
_logger.Debug($"Loading type: {attr.FullClassName} from {attr.DllName}.dll");
// → "Loading type: nU3.Modules.EMR.IN.Worklist.PatientListControl from nU3.Modules.EMR.IN.Worklist.dll"

// 오류 로그 (FullClassName)
_logger.Error($"Failed to instantiate {attr.FullClassName}: {ex.Message}");
```

## 성능 고려사항

### ClassName은 계산 속성

```csharp
// ClassName은 매번 계산됨
public string ClassName => FullClassName?.Split('.').LastOrDefault() ?? string.Empty;

// 반복 호출 시 캐시 고려
private string _cachedClassName;
public string GetClassName()
{
    if (_cachedClassName == null)
        _cachedClassName = FullClassName?.Split('.').LastOrDefault() ?? string.Empty;
    return _cachedClassName;
}
```

**하지만 성능 영향은 미미함:**
- String.Split은 매우 빠름 (~0.001ms)
- UI 표시나 로그는 자주 호출되지 않음
- 메모리 절약이 더 중요

## 권장 사항

### ? DO

```csharp
// ? 타입 로딩에는 FullClassName
var type = assembly.GetType(attr.FullClassName);

// ? UI 표시에는 ClassName
label.Text = attr.ClassName;

// ? DB에는 FullClassName 저장
INSERT INTO SYS_PROGRAM (CLASS_NAME) VALUES (@FullClassName);

// ? 검색에는 ClassName
where attr.ClassName.Contains(keyword)

// ? 상세 로그에는 FullClassName
_logger.Debug($"Type: {attr.FullClassName}");

// ? 요약 로그에는 ClassName
Console.WriteLine($"Loaded {attr.ClassName}");
```

### ? DON'T

```csharp
// ? 타입 로딩에 ClassName 사용
var type = assembly.GetType(attr.ClassName);  // null 반환!

// ? UI에 FullClassName 표시 (너무 길음)
label.Text = attr.FullClassName;  // "nU3.Modules.EMR.IN.Worklist.PatientListControl"

// ? DB에 ClassName만 저장 (정보 손실)
INSERT INTO SYS_PROGRAM (CLASS_NAME) VALUES (@ClassName);  // "PatientListControl"만
```

## 결론

### FullClassName
- **용도**: 타입 해상도, 상세 로깅, DB 저장
- **특징**: 정확하고 완전한 정보
- **예시**: "nU3.Modules.EMR.IN.Worklist.PatientListControl"

### ClassName
- **용도**: UI 표시, 요약 로그, 사용자 검색
- **특징**: 간결하고 읽기 쉬움
- **예시**: "PatientListControl"

### 두 속성을 함께 사용하여 최적의 경험 제공! ??

## 라이선스

? 2024 nU3 Framework
