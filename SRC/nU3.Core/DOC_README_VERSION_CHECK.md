# ModuleLoaderService 버전 체크 가이드

## 개요

프로그램 실행 시 **ProgId, ModuleId, Version**을 모두 체크하여 최신 버전 자동 업데이트 기능이 추가되었습니다.

## ?? 업데이트 흐름

```
[메뉴 클릭]
    ↓
[ProgId, ModuleId 획득]
    ↓
[DB에서 최신 버전 조회]
    ↓
[로컬 버전과 비교]
    ↓
    ├─ 같음 → 바로 실행
    └─ 다름 → 업데이트
           ↓
       [Remote Server]
           ↓ Download
       [Cache]
           ↓ Deploy
       [Runtime]
           ↓ Reload
       [실행]
```

## 주요 변경사항

### 1. **버전 추적**

```csharp
private readonly Dictionary<string, string> _loadedModuleVersions;
// ModuleId → Version 매핑
```

**로드된 모듈의 버전을 메모리에 저장:**
```csharp
_loadedModuleVersions["PROG_EMR_IN_Worklist"] = "1.0.2.0"
```

### 2. **버전 체크 메서드**

```csharp
/// <summary>
/// 프로그램 실행 전 버전 체크 및 업데이트
/// </summary>
public bool EnsureModuleUpdated(string progId, string moduleId)
{
    // 1. DB에서 최신 버전 조회
    var activeVersion = _moduleRepo.GetActiveVersions()
        .FirstOrDefault(v => v.ModuleId == moduleId);
    
    // 2. 로드된 버전과 비교
    if (_loadedModuleVersions.TryGetValue(moduleId, out var loadedVersion))
    {
        if (loadedVersion == activeVersion.Version)
            return true;  // 이미 최신
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

## 사용 방법

### 방법 1: 버전 체크 포함 (권장 ?)

```csharp
// MainShellForm.cs - 메뉴 클릭 처리
private void OnMenuItemClick(string menuId)
{
    var menuRepo = Program.ServiceProvider.GetRequiredService<IMenuRepository>();
    var menu = menuRepo.GetMenuById(menuId);
    
    if (string.IsNullOrEmpty(menu.ProgId))
        return;
    
    // ? ProgId, ModuleId 모두 사용
    var progRepo = Program.ServiceProvider.GetRequiredService<IProgramRepository>();
    var program = progRepo.GetProgramById(menu.ProgId);
    
    if (program == null)
        return;
    
    // ? 버전 체크 및 자동 업데이트
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

### 방법 2: 간단한 방식 (버전 체크 없음)

```csharp
// 테스트나 간단한 경우
var instance = _moduleLoader.CreateProgramInstance(progId);
```

### 방법 3: 수동 버전 체크

```csharp
// 사전에 버전 체크
if (_moduleLoader.EnsureModuleUpdated(progId, moduleId))
{
    var instance = _moduleLoader.CreateProgramInstance(progId);
}
```

## 실제 시나리오

### 시나리오 1: 최신 버전 (업데이트 불필요)

```
사용자: 환자 목록 메뉴 클릭
    ↓
시스템:
  - ProgId: "EMR_PATIENT_LIST_001"
  - ModuleId: "PROG_EMR_IN_Worklist"
  - DB Version: "1.0.2.0"
  - Loaded Version: "1.0.2.0"
    ↓
결과: ? 버전 일치, 바로 실행
시간: ~50ms
```

### 시나리오 2: 업데이트 필요

```
사용자: 환자 목록 메뉴 클릭
    ↓
시스템:
  - ProgId: "EMR_PATIENT_LIST_001"
  - ModuleId: "PROG_EMR_IN_Worklist"
  - DB Version: "1.0.3.0" ← 새 버전!
  - Loaded Version: "1.0.2.0"
    ↓
자동 업데이트:
  1. [Server] → [Cache] Download (1~5초)
  2. [Cache] → [Runtime] Deploy (~100ms)
  3. Reload Assembly (~200ms)
    ↓
결과: ? 업데이트 완료 후 실행
시간: ~1~6초
```

### 시나리오 3: 첫 실행 (DLL 없음)

```
사용자: 신규 모듈 메뉴 클릭
    ↓
시스템:
  - ProgId: "NEW_MODULE_001"
  - ModuleId: "PROG_EMR_NEW_Module"
  - Loaded Version: null ← 없음!
    ↓
자동 다운로드:
  1. [Server] → [Cache] Download
  2. [Cache] → [Runtime] Deploy
  3. Load Assembly
    ↓
결과: ? 다운로드 후 실행
```

## DB 스키마 활용

### SYS_MODULE_MST (모듈 마스터)

```sql
SELECT 
    MODULE_ID,      -- "PROG_EMR_IN_Worklist"
    MODULE_NAME,    -- "환자 관리 모듈"
    CATEGORY,       -- "EMR"
    SUB_SYSTEM,     -- "IN"
    FILE_NAME       -- "nU3.Modules.EMR.IN.Worklist.dll"
FROM SYS_MODULE_MST
WHERE MODULE_ID = 'PROG_EMR_IN_Worklist';
```

### SYS_MODULE_VER (버전 정보)

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

### SYS_PROGRAM (프로그램 정보)

```sql
SELECT 
    PROG_ID,        -- "EMR_PATIENT_LIST_001"
    MODULE_ID,      -- "PROG_EMR_IN_Worklist"
    PROG_NAME,      -- "환자 목록"
    IS_ACTIVE       -- "Y"
FROM SYS_PROGRAM
WHERE PROG_ID = 'EMR_PATIENT_LIST_001';
```

## 메뉴 구성 최적화

### 기존 방식 (비효율적)

```csharp
// ? DB 조회 많음
foreach (var menu in menus)
{
    var program = progRepo.GetProgram(menu.ProgId);  // DB 조회
    var module = moduleRepo.GetModule(program.ModuleId);  // DB 조회
    
    CreateMenuItem(menu.MenuName, program.ProgName);
}
```

### 개선된 방식 (효율적)

```csharp
// ? 한 번에 조회
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
            // ? DB 조회 없이 모든 정보 사용
            CreateMenuItem(
                menu.MenuName,
                attr.ProgramName,
                program.ModuleId,  // 실행 시 버전 체크용
                attr.SystemType,
                attr.AuthLevel
            );
        }
    }
}
```

## 로깅

### 버전 체크 로그

```
[ModuleLoader] Checking version for PROG_EMR_IN_Worklist
[ModuleLoader] DB Version: 1.0.3.0
[ModuleLoader] Loaded Version: 1.0.2.0
[ModuleLoader] Update needed
```

### 다운로드 로그

```
[ModuleLoader] Downloading: 환자 관리 모듈 v1.0.3.0
[ModuleLoader] Server: D:\ServerStorage\EMR\IN\nU3.Modules.EMR.IN.Worklist.dll
[ModuleLoader] Cache: C:\Users\...\AppData\...\Cache\EMR\IN\...
[ModuleLoader] Downloaded to cache: 환자 관리 모듈 v1.0.3.0
```

### 배포 로그

```
[ModuleLoader] Deploying: 환자 관리 모듈 v1.0.3.0
[ModuleLoader] Cache: C:\Users\...\Cache\EMR\IN\...
[ModuleLoader] Runtime: C:\Program Files\nU3.Shell\Modules\EMR\IN\...
[ModuleLoader] Deployed to runtime: 환자 관리 모듈 v1.0.3.0
```

### 로드 로그

```
[ModuleLoader] Reloaded: EMR_PATIENT_LIST_001 (v1.0.3.0)
[ModuleLoader] Created instance: EMR_PATIENT_LIST_001 (Module: PROG_EMR_IN_Worklist)
```

## 오류 처리

### 1. 서버 파일 없음

```csharp
if (!File.Exists(serverFile))
{
    MessageBox.Show(
        "서버에서 모듈을 찾을 수 없습니다.\n관리자에게 문의하세요.",
        "모듈 로드 실패",
        MessageBoxButtons.OK,
        MessageBoxIcon.Error
    );
    return null;
}
```

### 2. 파일 잠김 (실행 중)

```csharp
catch (IOException ex)
{
    MessageBox.Show(
        "모듈이 사용 중입니다.\n프로그램을 재시작하면 업데이트가 적용됩니다.",
        "업데이트 예약됨",
        MessageBoxButtons.OK,
        MessageBoxIcon.Information
    );
}
```

### 3. 버전 정보 없음

```csharp
if (activeVersion == null)
{
    MessageBox.Show(
        "모듈 버전 정보를 찾을 수 없습니다.",
        "버전 확인 실패",
        MessageBoxButtons.OK,
        MessageBoxIcon.Warning
    );
}
```

## 성능 최적화

### 1. 비동기 다운로드 (선택적)

```csharp
public async Task<bool> EnsureModuleUpdatedAsync(string progId, string moduleId)
{
    // 백그라운드에서 다운로드
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

### 3. 캐시 사전 로드

```csharp
// 애플리케이션 시작 시
_moduleLoader.LoadAllModules();
// → 모든 DLL을 미리 로드하여 메모리 캐시 구축
```

## 마이그레이션 체크리스트

### 기존 코드 변경

- [ ] 메뉴 클릭 핸들러 수정
- [ ] `CreateProgramInstance()` → `CreateProgramInstanceWithVersionCheck()` 변경
- [ ] ProgId + ModuleId 함께 전달
- [ ] 오류 처리 추가

### DB 스키마 확인

- [ ] `SYS_MODULE_VER` 테이블 존재
- [ ] `VERSION` 컬럼 존재
- [ ] `FILE_HASH` 컬럼 존재
- [ ] `IS_ACTIVE` 컬럼 존재

### 테스트

- [ ] 최신 버전 실행 (업데이트 불필요)
- [ ] 구버전 실행 (자동 업데이트)
- [ ] 신규 모듈 실행 (첫 다운로드)
- [ ] 서버 파일 없는 경우
- [ ] 파일 잠김 상태 처리

## 결론

? **완벽한 버전 관리 시스템!**

- **ProgId**: 프로그램 식별
- **ModuleId**: 모듈 식별
- **Version**: 버전 체크 및 자동 업데이트

```
메뉴 클릭 → 버전 체크 → 자동 업데이트 → 실행
```

사용자는 **항상 최신 버전**을 자동으로 사용하게 됩니다! ??

## 라이선스

? 2024 nU3 Framework
