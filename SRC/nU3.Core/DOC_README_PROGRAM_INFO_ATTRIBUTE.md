# nU3ProgramInfoAttribute 사용 가이드

## 개요

`nU3ProgramInfoAttribute`는 프로그램 모듈의 메타데이터를 정의하는 속성입니다. 
- 네임스페이스에서 자동으로 시스템 타입과 서브시스템을 추출
- `BaseWorkControl`의 `ProgramID`와 `ProgramTitle`을 자동으로 설정

## ? 주요 기능

### 1. 네임스페이스 자동 파싱
`typeof(클래스명)`을 사용하면 네임스페이스에서 자동으로 `SystemType`과 `SubSystem`을 추출합니다.

### 2. 자동 속성 주입
`BaseWorkControl`의 `ProgramID`와 `ProgramTitle`이 자동으로 `nU3ProgramInfo` 속성에서 가져옵니다.

## 사용 방법

### ? 권장 방법 (자동 추출)

```csharp
namespace nU3.Modules.ADM.AD.Deployer
{
    // typeof 사용 - SystemType과 SubSystem 자동 추출
    // ProgramID와 ProgramTitle도 자동 설정
    [nU3ProgramInfo(typeof(DeployerWorkControl), "ADM AD Deployer", "ADM_AD_00001")]
    public class DeployerWorkControl : BaseWorkControl
    {
        public DeployerWorkControl()
        {
            // ProgramID와 ProgramTitle을 수동으로 설정할 필요 없음!
            // 자동으로 nU3ProgramInfo에서 가져옴
            
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

**자동 설정 결과:**
- `SystemType`: `"ADM"` (네임스페이스에서 자동 추출)
- `SubSystem`: `"AD"` (네임스페이스에서 자동 추출)
- `ProgramID`: `"ADM_AD_00001"` (BaseWorkControl에서 자동 사용)
- `ProgramTitle`: `"ADM AD Deployer"` (BaseWorkControl에서 자동 사용)

### ? 이전 방법 (비권장)

```csharp
namespace nU3.Modules.ADM.AD.Deployer
{
    [nU3ProgramInfo("ADM", "ADM AD Deployer", "ADM_AD_00001")]
    public class DeployerWorkControl : BaseWorkControl
    {
        public DeployerWorkControl()
        {
            // 불필요한 중복 코드!
            ProgramID = "ADM_AD_DEPLOYER";
            ProgramTitle = "ADM Deployer";
            
            // ...
        }
    }
}
```

## 네임스페이스 파싱 규칙

네임스페이스는 다음 형식을 따라야 합니다:

```
nU3.Modules.{SystemType}.{SubSystem}.{ModuleName}
```

### 파싱 결과

| Namespace | SystemType | SubSystem |
|-----------|------------|-----------|
| `nU3.Modules.EMR.IN.Worklist` | EMR | IN |
| `nU3.Modules.ADM.AD.Deployer` | ADM | AD |
| `nU3.Modules.NUR.OP.Schedule` | NUR | OP |
| `nU3.Modules.EMR.Worklist` | EMR | null |
| `nU3.Modules.LAB.Analysis` | LAB | null |

## 상세 예시

### 예시 1: 관리자 모듈

```csharp
namespace nU3.Modules.ADM.AD.Deployer
{
    /// <summary>
    /// 배포 관리 화면
    /// - SystemType: "ADM" (자동)
    /// - SubSystem: "AD" (자동)
    /// - ProgramID: "ADM_AD_00001" (자동)
    /// - ProgramTitle: "배포 관리" (자동)
    /// </summary>
    [nU3ProgramInfo(typeof(DeployerWorkControl), "배포 관리", "ADM_AD_00001")]
    public class DeployerWorkControl : BaseWorkControl
    {
        public DeployerWorkControl()
        {
            // ProgramID, ProgramTitle 설정 불필요!
            InitializeLayout();
        }
    }
}
```

### 예시 2: EMR 입원 모듈

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
            // 모든 속성이 자동으로 설정됨
            InitializeLayout();
            LoadSampleData();
        }
    }
}
```

### 예시 3: 팝업 폼

```csharp
namespace nU3.Modules.EMR.OP.Registration
{
    /// <summary>
    /// 외래 환자 등록 팝업
    /// FormType을 "POPUP"으로 지정
    /// </summary>
    [nU3ProgramInfo(typeof(PatientRegisterPopup), "환자 등록", "EMR_OP_REG_POPUP", "POPUP")]
    public class PatientRegisterPopup : BaseWorkControl
    {
        // FormType = "POPUP"
        // SystemType = "EMR"
        // SubSystem = "OP"
    }
}
```

### 예시 4: 권한 레벨 지정

```csharp
namespace nU3.Modules.ADM.AD.UserManagement
{
    /// <summary>
    /// 사용자 관리 (관리자 전용)
    /// AuthLevel = 0 (관리자만 접근)
    /// </summary>
    [nU3ProgramInfo(typeof(UserManagementControl), "사용자 관리", "ADM_AD_USER_001", AuthLevel = 0)]
    public class UserManagementControl : BaseWorkControl
    {
        // AuthLevel = 0 (관리자 전용)
    }
}
```

## BaseWorkControl 자동 속성

`BaseWorkControl`을 상속받으면 다음 속성이 자동으로 설정됩니다:

```csharp
public class YourControl : BaseWorkControl
{
    // ? 자동으로 nU3ProgramInfo에서 가져옴
    public override string ProgramID { get; }
    
    // ? 자동으로 nU3ProgramInfo에서 가져옴
    public override string ProgramTitle { get; }
    
    public YourControl()
    {
        // ? 수동 설정 불필요!
        // ProgramID = "...";     // 불필요
        // ProgramTitle = "...";  // 불필요
        
        // 바로 로직 구현
        InitializeLayout();
    }
}
```

## 생성자 시그니처

### 자동 파싱 생성자 (권장)

```csharp
public nU3ProgramInfoAttribute(
    Type declaringType,        // typeof(클래스명)
    string programName,        // 프로그램 표시 이름
    string programId,          // 고유 프로그램 ID
    string formType = "CHILD"  // CHILD, POPUP, SDI
)
```

### 명시적 생성자 (하위 호환)

```csharp
public nU3ProgramInfoAttribute(
    string systemType,         // 시스템 타입 (예: "EMR", "ADM")
    string programName,        // 프로그램 표시 이름
    string programId,          // 고유 프로그램 ID
    string formType = "CHILD"  // CHILD, POPUP, SDI
)
```

## 속성 정보

### 자동 설정 속성

| 속성 | 출처 | 설명 |
|------|------|------|
| `ProgramID` | nU3ProgramInfo | BaseWorkControl에서 자동 사용 |
| `ProgramTitle` | nU3ProgramInfo | BaseWorkControl에서 자동 사용 |
| `SystemType` | Namespace | 네임스페이스에서 자동 추출 |
| `SubSystem` | Namespace | 네임스페이스에서 자동 추출 |

### 수동 설정 속성

| 속성 | 타입 | 기본값 | 설명 |
|------|------|--------|------|
| `FormType` | string | "CHILD" | 폼 타입: CHILD, POPUP, SDI |
| `AuthLevel` | int | 1 | 권한 레벨 (0=관리자, 1~99=일반) |
| `IsUse` | bool | true | 활성화 여부 |

## 네임스페이스 규칙 권장사항

### 시스템 타입 (SystemType)

- **EMR**: Electronic Medical Record (전자의무기록)
- **ADM**: Administration (관리)
- **NUR**: Nursing (간호)
- **LAB**: Laboratory (검사)
- **RAD**: Radiology (영상의학)
- **PHA**: Pharmacy (약국)
- **BIL**: Billing (청구)

### 서브시스템 (SubSystem)

- **IN**: Inpatient (입원)
- **OP**: Outpatient (외래)
- **ER**: Emergency Room (응급실)
- **AD**: Administration (관리)
- **SCH**: Schedule (스케줄)
- **WL**: Worklist (워크리스트)

### 네임스페이스 구조 예시

```
nU3.Modules.{System}.{SubSystem}.{Module}

예시:
nU3.Modules.EMR.IN.Worklist          → 입원 워크리스트
nU3.Modules.EMR.OP.Registration      → 외래 접수
nU3.Modules.ADM.AD.UserManagement    → 관리자 - 사용자 관리
nU3.Modules.NUR.IN.VitalSigns        → 간호 - 입원 바이탈 사인
nU3.Modules.LAB.OP.OrderEntry        → 검사실 - 외래 처방 입력
```

## 마이그레이션 가이드

### Before (이전 방식)

```csharp
namespace nU3.Modules.ADM.AD.Deployer
{
    [nU3ProgramInfo("ADM", "ADM AD Deployer", "ADM_AD_DEPLOYER")]
    public class DeployerWorkControl : BaseWorkControl
    {
        public DeployerWorkControl()
        {
            ProgramID = "ADM_AD_DEPLOYER";      // ? 중복
            ProgramTitle = "ADM Deployer";       // ? 중복
            // ...
        }
    }
}
```

### After (새 방식)

```csharp
namespace nU3.Modules.ADM.AD.Deployer
{
    [nU3ProgramInfo(typeof(DeployerWorkControl), "ADM AD Deployer", "ADM_AD_DEPLOYER")]
    public class DeployerWorkControl : BaseWorkControl
    {
        public DeployerWorkControl()
        {
            // ? ProgramID, ProgramTitle 설정 불필요!
            // ? SystemType, SubSystem 자동 추출!
            // ...
        }
    }
}
```

### 장점

1. **코드 간소화**: ProgramID와 ProgramTitle을 중복으로 설정할 필요 없음
2. **타이핑 감소**: SystemType과 SubSystem을 매번 입력할 필요 없음
3. **오타 방지**: 네임스페이스와 속성 값의 불일치 방지
4. **유지보수성**: 한 곳(nU3ProgramInfo)에서만 관리
5. **일관성**: 네임스페이스 규칙을 따르면 자동으로 올바른 값 설정

## 런타임 속성 확인

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

// BaseWorkControl 인스턴스에서 직접 접근
var control = new DeployerWorkControl();
Console.WriteLine($"ProgramID: {control.ProgramID}");        // "ADM_AD_00001"
Console.WriteLine($"ProgramTitle: {control.ProgramTitle}");  // "ADM AD Deployer"
```

## 주의사항

1. **네임스페이스 규칙 준수**: `nU3.Modules.{SystemType}.{SubSystem}` 형식을 따라야 자동 추출이 작동합니다.
2. **typeof 사용**: 자동 추출을 사용하려면 첫 번째 인자로 `typeof(클래스명)`을 전달해야 합니다.
3. **BaseWorkControl 상속**: `ProgramID`와 `ProgramTitle` 자동 설정은 `BaseWorkControl`을 상속한 경우에만 작동합니다.
4. **수동 오버라이드 가능**: 필요한 경우 `ProgramID`나 `ProgramTitle`을 수동으로 오버라이드할 수 있습니다.

## FAQ

### Q: ProgramID를 수동으로 변경할 수 있나요?
A: 네, 필요한 경우 오버라이드할 수 있습니다:

```csharp
[nU3ProgramInfo(typeof(MyControl), "내 컨트롤", "MY_CTRL_001")]
public class MyControl : BaseWorkControl
{
    public MyControl()
    {
        // 특별한 경우 수동 설정 가능
        ProgramID = "CUSTOM_ID";
    }
}
```

### Q: 네임스페이스가 규칙과 다르면 어떻게 되나요?
A: `SystemType`이 "COMMON"으로 설정되고 `SubSystem`은 null이 됩니다.

### Q: 하위 호환성은 보장되나요?
A: 네, 기존 명시적 생성자도 여전히 사용 가능합니다.

## 라이선스

? 2024 nU3 Framework
