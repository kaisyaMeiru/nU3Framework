# nU3ProgramInfoAttribute 기반 동적 로딩 가이드

## 개요

`nU3ProgramInfoAttribute`만으로 DB 조회 없이 동적 로딩, 메뉴 구성, 객체 생성이 가능합니다.

## ? 장점

### 1. DB 의존성 최소화
**기존 방식 (DB 의존):**
```
메뉴 클릭 
→ DB에서 ProgId로 Program 조회 
→ ModuleId 획득 
→ DB에서 ModuleId로 Module 조회 
→ DLL 경로 조합 
→ DLL 로드 
→ DB에서 ClassName 조회 
→ 타입 찾기 
→ 인스턴스 생성
```

**새로운 방식 (Attribute 기반):**
```
메뉴 클릭 
→ Attribute에서 직접 정보 획득 
→ DLL 로드 
→ 인스턴스 생성
```

### 2. 자동화된 정보 추출

```csharp
[nU3ProgramInfo(typeof(PatientListControl), "환자 목록", "EMR_PATIENT_LIST_001")]
public class PatientListControl : BaseWorkControl
{
    // 이 속성 하나로 모든 정보 자동 추출:
    // - SystemType: "EMR"
    // - SubSystem: "IN"
    // - DllName: "nU3.Modules.EMR.IN.Worklist"
    // - ClassName: "nU3.Modules.EMR.IN.Worklist.PatientListControl"
    // - DLL Path: "EMR/IN/nU3.Modules.EMR.IN.Worklist.dll"
    // - ModuleId: "PROG_EMR_IN_nU3.Modules.EMR.IN.Worklist"
}
```

## 사용 방법

### 1. 프로그램 정의 (개발자)

```csharp
namespace nU3.Modules.EMR.IN.Worklist
{
    [nU3ProgramInfo(typeof(PatientListControl), "환자 목록", "EMR_PATIENT_LIST_001")]
    public class PatientListControl : BaseWorkControl
    {
        public PatientListControl()
        {
            // 구현
        }
    }
}
```

### 2. 메뉴 구성 (DB 최소 의존)

```csharp
// ModuleLoaderService에서 모든 프로그램 속성 캐시 획득
var programAttributes = moduleLoader.GetProgramAttributes();

// DB 조회 없이 메뉴 트리 구성
foreach (var menuItem in menuItems)
{
    if (programAttributes.TryGetValue(menuItem.ProgId, out var attr))
    {
        // ? DB 조회 없이 속성 정보 사용
        Console.WriteLine($"Menu: {attr.ProgramName}");
        Console.WriteLine($"System: {attr.SystemType}/{attr.SubSystem}");
        Console.WriteLine($"Auth: {attr.AuthLevel}");
        Console.WriteLine($"FormType: {attr.FormType}");
    }
}
```

### 3. 프로그램 동적 로드

#### 방법 A: ProgId만으로 인스턴스 생성

```csharp
// ? 가장 간단한 방법
var instance = moduleLoader.CreateProgramInstance("EMR_PATIENT_LIST_001");
if (instance is BaseWorkControl control)
{
    // 사용
    panel.Controls.Add(control);
}
```

#### 방법 B: Attribute 정보로 로드

```csharp
var attr = moduleLoader.GetProgramAttribute("EMR_PATIENT_LIST_001");
if (attr != null)
{
    // DLL 경로는 자동 계산됨
    var type = moduleLoader.LoadProgramByAttribute(attr);
    if (type != null)
    {
        var instance = Activator.CreateInstance(type);
        // 사용
    }
}
```

#### 방법 C: 헬퍼 메서드 활용

```csharp
var attr = moduleLoader.GetProgramAttribute("EMR_PATIENT_LIST_001");

// DLL 경로 자동 생성
string dllPath = attr.GetExpectedDllPath();
// → "EMR/IN/nU3.Modules.EMR.IN.Worklist.dll"

// ModuleId 자동 생성
string moduleId = attr.GetModuleId();
// → "PROG_EMR_IN_nU3.Modules.EMR.IN.Worklist"
```

## 실제 활용 예시

### 예시 1: 메뉴 클릭 처리

```csharp
private void OnMenuItemClick(string progId)
{
    // ? DB 조회 없이 프로그램 정보 확인
    var attr = _moduleLoader.GetProgramAttribute(progId);
    if (attr == null)
    {
        MessageBox.Show("프로그램 정보를 찾을 수 없습니다.");
        return;
    }
    
    // ? 권한 체크 (DB 조회 없이)
    if (attr.AuthLevel > _currentUser.AuthLevel)
    {
        MessageBox.Show("권한이 부족합니다.");
        return;
    }
    
    // ? FormType에 따라 처리 (DB 조회 없이)
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
    // ? DB 조회 없이 인스턴스 생성
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

### 예시 2: 프로그램 검색

```csharp
// ? DB 조회 없이 모든 프로그램 정보 검색
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

### 예시 3: 시스템별 프로그램 목록

```csharp
// ? DB 조회 없이 시스템별 그룹화
var programsBySystem = _moduleLoader.GetProgramAttributes()
    .Values
    .GroupBy(attr => attr.SystemType)
    .ToDictionary(g => g.Key, g => g.ToList());

// EMR 시스템의 모든 프로그램
if (programsBySystem.TryGetValue("EMR", out var emrPrograms))
{
    foreach (var attr in emrPrograms)
    {
        Console.WriteLine($"  [{attr.SubSystem}] {attr.ProgramName}");
    }
}
```

### 예시 4: 최근 사용 프로그램 (Quick Access)

```csharp
// ? DB에서 ProgId 목록만 조회, 상세 정보는 Attribute에서
var recentProgIds = GetRecentPrograms(); // DB에서 ProgId만 조회

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

## DB 역할 변경

### 기존 DB 역할
```sql
-- 기존: 모든 정보를 DB에 저장
CREATE TABLE SYS_PROGRAM (
    PROG_ID VARCHAR(50),
    MODULE_ID VARCHAR(50),
    PROG_NAME VARCHAR(100),
    CLASS_NAME VARCHAR(200),     -- ? 중복
    AUTH_LEVEL INT,              -- ? 중복
    FORM_TYPE VARCHAR(10),       -- ? 중복
    SYSTEM_TYPE VARCHAR(10),     -- ? 중복
    SUB_SYSTEM VARCHAR(10)       -- ? 중복
);
```

### 새로운 DB 역할 (최소화)
```sql
-- 새로운: 런타임 정보만 저장
CREATE TABLE SYS_PROGRAM (
    PROG_ID VARCHAR(50) PRIMARY KEY,
    MODULE_ID VARCHAR(50),       -- ? 필요 (버전 관리)
    IS_ACTIVE CHAR(1),           -- ? 필요 (활성화 여부)
    PROG_TYPE INT                -- ? 필요 (타입 구분)
);

-- 나머지 정보는 모두 nU3ProgramInfoAttribute에서 가져옴!
```

## 장점 정리

| 항목 | 기존 방식 | 새로운 방식 |
|------|----------|-------------|
| DB 조회 횟수 | 3-4회 | 0-1회 |
| 정보 중복 | DB + Code | Code만 |
| 메뉴 구성 속도 | 느림 (DB 조회) | 빠름 (메모리 캐시) |
| 유지보수 | DB + Code 동기화 | Code만 관리 |
| 배포 | DB + DLL | DLL만 |
| 오프라인 지원 | 불가 | 가능 (Attribute 기반) |

## 주의사항

### 1. 초기 로딩 시 캐시 구축
```csharp
// 애플리케이션 시작 시 한 번만 실행
moduleLoader.LoadAllModules();
// → 모든 DLL 스캔하여 _progAttributeCache 구축
```

### 2. DLL 업데이트 시 캐시 갱신
```csharp
// 새 DLL 배포 후
moduleLoader.CheckAndUpdateModules();
// → 변경된 DLL만 다시 로드하여 캐시 갱신
```

### 3. DB는 여전히 필요한 경우
```csharp
// ? DB가 필요한 경우:
// - 버전 관리 (ModuleId, Version)
// - 활성화 여부 (IsActive)
// - 메뉴 구조 (MenuId, ParentId, SortOrd)
// - 사용자별 권한 (User-Program mapping)

// ? DB가 불필요한 경우:
// - ClassName (Attribute에서)
// - ProgramName (Attribute에서)
// - SystemType (Attribute에서)
// - SubSystem (Attribute에서)
// - AuthLevel (Attribute에서)
// - FormType (Attribute에서)
```

## 마이그레이션 가이드

### Step 1: Attribute 정보 확장 (완료!)
```csharp
[nU3ProgramInfo(typeof(YourControl), "프로그램명", "PROG_ID")]
// → ClassName 자동 설정됨
```

### Step 2: ModuleLoaderService 업그레이드 (완료!)
```csharp
// GetProgramAttributes() 메서드 사용 가능
// CreateProgramInstance() 메서드 사용 가능
```

### Step 3: 메뉴/화면 로딩 코드 수정
```csharp
// Before:
var program = _programRepo.GetProgram(progId);
var module = _moduleRepo.GetModule(program.ModuleId);
var dllPath = CalculatePath(module);
var type = LoadType(dllPath, program.ClassName);

// After:
var instance = _moduleLoader.CreateProgramInstance(progId);
```

### Step 4: DB 스키마 간소화 (선택)
```sql
-- CLASS_NAME, SYSTEM_TYPE, SUB_SYSTEM 등 컬럼 제거
ALTER TABLE SYS_PROGRAM DROP COLUMN CLASS_NAME;
ALTER TABLE SYS_PROGRAM DROP COLUMN SYSTEM_TYPE;
-- ...
```

## 결론

? **당신의 생각이 완전히 맞습니다!**

`nU3ProgramInfoAttribute`만으로:
1. DLL 경로 자동 계산
2. 타입 동적 로드
3. 인스턴스 생성
4. 메뉴 구성 (DB 최소 의존)
5. 권한 체크
6. FormType 처리

모두 가능합니다! ??

## 라이선스

? 2024 nU3 Framework
