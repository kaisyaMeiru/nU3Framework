# ModuleId 명명 규칙 변경 가이드

## 개요

ModuleId 생성 규칙이 DLL 전체 이름에서 **간단한 DLL 이름**으로 변경되었습니다.

## 변경 사항

### 기존 규칙 (변경 전)
```
PROG_{SystemType}_{SubSystem}_{FullDllName}
```

**예시:**
```
DLL: nU3.Modules.EMR.IN.Worklist.dll
ModuleId: PROG_EMR_IN_nU3.Modules.EMR.IN.Worklist  ? (너무 길고 중복)
```

### 새로운 규칙 (변경 후)
```
PROG_{SystemType}_{SubSystem}_{SimpleDllName}
```

**예시:**
```
DLL: nU3.Modules.EMR.IN.Worklist.dll
ModuleId: PROG_EMR_IN_Worklist  ? (간결하고 명확)
```

## 장점

### 1. **간결성**
```
기존: PROG_EMR_IN_nU3.Modules.EMR.IN.Worklist (43자)
새로운: PROG_EMR_IN_Worklist (21자)
```

### 2. **가독성**
```
? PROG_EMR_IN_Worklist
   └─ 시스템: EMR
   └─ 서브시스템: IN
   └─ 모듈: Worklist

? PROG_EMR_IN_nU3.Modules.EMR.IN.Worklist
   └─ 시스템: EMR (중복)
   └─ 서브시스템: IN (중복)
   └─ 모듈: nU3.Modules.EMR.IN.Worklist (중복된 정보)
```

### 3. **예측 가능성**
```
DLL 이름 규칙: nU3.Modules.{System}.{SubSystem}.{Module}.dll
ModuleId 규칙: PROG_{System}_{SubSystem}_{Module}

? 마지막 부분(Module)만 ModuleId에 사용
? 일정한 패턴으로 DLL 경로 예측 가능
```

## 실제 예시

### 예시 1: EMR 모듈

**DLL 파일:**
```
nU3.Modules.EMR.IN.Worklist.dll
```

**ModuleId 생성:**
```csharp
// SimpleDllName 추출
var dllName = "nU3.Modules.EMR.IN.Worklist";
var parts = dllName.Split('.');
var simpleName = parts[4];  // "Worklist"

// ModuleId 생성
var moduleId = $"PROG_EMR_IN_{simpleName}";
// → "PROG_EMR_IN_Worklist"
```

### 예시 2: ADM 모듈

**DLL 파일:**
```
nU3.Modules.ADM.AD.Deployer.dll
```

**ModuleId:**
```
PROG_ADM_AD_Deployer
```

### 예시 3: 다양한 모듈

| DLL 파일 | SystemType | SubSystem | SimpleName | ModuleId |
|----------|------------|-----------|------------|----------|
| nU3.Modules.EMR.IN.Worklist.dll | EMR | IN | Worklist | PROG_EMR_IN_Worklist |
| nU3.Modules.EMR.OP.Clinic.dll | EMR | OP | Clinic | PROG_EMR_OP_Clinic |
| nU3.Modules.ADM.AD.Deployer.dll | ADM | AD | Deployer | PROG_ADM_AD_Deployer |
| nU3.Modules.NUR.IN.NursingStation.dll | NUR | IN | NursingStation | PROG_NUR_IN_NursingStation |

## 구현 세부사항

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
    /// 간단한 DLL 이름 (마지막 부분만)
    /// 예: "Worklist"
    /// </summary>
    public string SimpleDllName { get; }
    
    public nU3ProgramInfoAttribute(Type declaringType, ...)
    {
        this.DllName = declaringType.Assembly.GetName().Name;
        
        // SimpleDllName 추출
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

## 경로 예측

### DLL 경로 자동 계산

```csharp
// ModuleId로 DLL 경로 예측
var moduleId = "PROG_EMR_IN_Worklist";

// 파싱
var parts = moduleId.Split('_');
var systemType = parts[1];      // EMR
var subSystem = parts[2];       // IN
var simpleName = parts[3];      // Worklist

// DLL 이름 재구성
var dllName = $"nU3.Modules.{systemType}.{subSystem}.{simpleName}.dll";
// → "nU3.Modules.EMR.IN.Worklist.dll"

// 전체 경로
var dllPath = Path.Combine(_runtimePath, systemType, subSystem, dllName);
// → "C:\...\Modules\EMR\IN\nU3.Modules.EMR.IN.Worklist.dll"
```

### Attribute에서 자동 계산

```csharp
var attr = moduleLoader.GetProgramAttribute(progId);

// ? DLL 경로 자동 생성
var dllPath = attr.GetExpectedDllPath();
// → "EMR/IN/nU3.Modules.EMR.IN.Worklist.dll"

// ? ModuleId 자동 생성
var moduleId = attr.GetModuleId();
// → "PROG_EMR_IN_Worklist"
```

## 마이그레이션

### 기존 데이터 마이그레이션

```sql
-- 기존 ModuleId 확인
SELECT MODULE_ID, FILE_NAME FROM SYS_MODULE_MST;

-- 예시 결과:
-- MODULE_ID: PROG_EMR_IN_nU3.Modules.EMR.IN.Worklist
-- FILE_NAME: nU3.Modules.EMR.IN.Worklist.dll

-- 새로운 ModuleId로 업데이트
UPDATE SYS_MODULE_MST
SET MODULE_ID = 'PROG_EMR_IN_Worklist'
WHERE MODULE_ID = 'PROG_EMR_IN_nU3.Modules.EMR.IN.Worklist';

-- SYS_MODULE_VER도 동일하게 업데이트
UPDATE SYS_MODULE_VER
SET MODULE_ID = 'PROG_EMR_IN_Worklist'
WHERE MODULE_ID = 'PROG_EMR_IN_nU3.Modules.EMR.IN.Worklist';

-- SYS_PROGRAM도 동일하게 업데이트
UPDATE SYS_PROGRAM
SET MODULE_ID = 'PROG_EMR_IN_Worklist'
WHERE MODULE_ID = 'PROG_EMR_IN_nU3.Modules.EMR.IN.Worklist';
```

### 자동 마이그레이션 스크립트

```sql
-- 모든 ModuleId를 새로운 형식으로 변환
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
IsValidModuleId("PROG_EMR_IN_Worklist");  // ? true
IsValidModuleId("PROG_EMR_IN_nU3.Modules.EMR.IN.Worklist");  // ? false
```

### DLL 이름 재구성 검증

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
// → "nU3.Modules.EMR.IN.Worklist.dll" ?
```

## 장점 요약

| 항목 | 기존 (DLL 전체 이름) | 새로운 (Simple DLL) |
|------|---------------------|---------------------|
| 길이 | 43자 | 21자 |
| 가독성 | 낮음 (중복) | 높음 (간결) |
| 예측 가능성 | 보통 | 높음 |
| DB 저장 공간 | 많음 | 적음 |
| UI 표시 | 부적합 | 적합 |
| 일관성 | 중복 정보 | 일관된 패턴 |

## 결론

? **ModuleId는 이제 간결하고 명확합니다!**

```
PROG_{SystemType}_{SubSystem}_{SimpleDllName}
```

- **간결**: 불필요한 중복 제거
- **명확**: 시스템/서브시스템/모듈 구분
- **예측 가능**: 일정한 패턴으로 DLL 경로 계산 가능
- **효율적**: DB 저장 공간 절약

## 라이선스

? 2024 nU3 Framework
