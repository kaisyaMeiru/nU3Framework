# nU3.Framework - Assembly 로드 전략 및 Hot Deploy 구현

> 작성일: 2026-02-08  
> 대상: SI 프로젝트 다수 개발자 환경  
> 아키텍처: 플러그인 기반 모듈 시스템 (DI + EventBus)

---

## ?? 목차

1. [개요](#개요)
2. [LoadFile vs LoadFrom vs AssemblyLoadContext 비교](#loadfile-vs-loadfrom-vs-assemblyloadcontext-비교)
3. [SI 프로젝트 요구사항](#si-프로젝트-요구사항)
4. [최종 채택 전략: LoadFrom + Shadow Copy](#최종-채택-전략-loadfrom--shadow-copy)
5. [구현 상세](#구현-상세)
6. [버전 충돌 처리](#버전-충돌-처리)
7. [Hot Deploy 시나리오](#hot-deploy-시나리오)
8. [개발팀 가이드라인](#개발팀-가이드라인)
9. [문제 해결 가이드](#문제-해결-가이드)

---

## ?? 개요

### 프로젝트 특성

nU3.Framework는 **SI 프로젝트 환경**에서 **다수의 개발팀**이 **독립적으로 모듈을 개발**하고, **Shell을 통해 통합 실행**하는 구조입니다.

```
개발팀 A → Module A.dll (독립 개발)
개발팀 B → Module B.dll (독립 개발)
개발팀 C → Module C.dll (독립 개발)
         ↓
    EventBus + DI로 통합
         ↓
      Shell 실행
```

### 핵심 요구사항

| 요구사항 | 설명 | 중요도 |
|---------|------|--------|
| **독립 개발** | 각 팀이 서로 영향 없이 개발 | ??? |
| **DI 지원** | Shell의 `ActivatorUtilities.CreateInstance` | ??? |
| **EventBus 통신** | 모듈 간 메시지 교환 | ??? |
| **Hot Deploy** | 실행 중 모듈 업데이트 | ?? |
| **버전 충돌 방지** | 타입 일관성 보장 | ??? |

---

## ?? LoadFile vs LoadFrom vs AssemblyLoadContext 비교

### Assembly 로드 방식 비교표

| 기준 | LoadFrom | LoadFile | AssemblyLoadContext | 권장 |
|------|----------|----------|---------------------|------|
| **DI 호환성** | ? 완벽 | ? 불가능 | ?? 제한적 | **LoadFrom** |
| **EventBus 통신** | ? 완벽 | ? 타입 불일치 | ?? 제한적 | **LoadFrom** |
| **독립 버전 관리** | ?? 정책 필요 | ? 완벽 | ? 완벽 | **ALC** |
| **Hot Deploy** | ?? Shadow Copy 필요 | ? 가능 | ? 가능 | **ALC** |
| **공유 의존성** | ? 자동 공유 | ? 중복 로드 | ? 제어 가능 | **LoadFrom** |
| **파일 잠금** | ? 발생 | ? 발생 | ? 발생 | 모두 동일 |
| **격리성** | ? 없음 | ? 완벽 | ? 완벽 | **ALC** |
| **구현 복잡도** | ? 낮음 | ? 낮음 | ?? 높음 | **LoadFrom** |
| **메모리 사용** | ? 효율적 | ? 중복 | ? 효율적 | **LoadFrom** |

### 1?? Assembly.LoadFrom (현재 채택)

```csharp
// 기본 로드 컨텍스트에서 로드
var assembly = Assembly.LoadFrom(dllPath);
```

**장점:**
- ? DI 완벽 지원 (`ActivatorUtilities.CreateInstance` 정상 작동)
- ? EventBus 타입 호환성 (모든 모듈이 같은 타입으로 인식)
- ? 공유 어셈블리 자동 관리 (nU3.Core, nU3.Models 등)
- ? 구현 간단

**단점:**
- ?? 같은 파일 경로는 캐싱됨 (재로드 불가)
- ?? Hot Deploy에 Shadow Copy 필요
- ?? 버전 충돌 시 정책 필요

**SI 프로젝트 적합성:** ? **최적**

### 2?? Assembly.LoadFile

```csharp
// 매번 새로운 인스턴스 로드
var assembly = Assembly.LoadFile(dllPath);
```

**장점:**
- ? 완전한 격리 (같은 DLL 여러 버전 로드 가능)
- ? Hot Deploy 가능 (재로드 자유)

**단점:**
- ? **DI 불가능** (타입이 기본 컨텍스트와 호환되지 않음)
- ? **EventBus 타입 불일치** (각 로드마다 다른 타입)
- ? 공유 의존성 중복 로드 (메모리 낭비)
- ? static 변수, 싱글톤이 공유되지 않음

**SI 프로젝트 적합성:** ? **부적합**

### 3?? AssemblyLoadContext

```csharp
// 커스텀 로드 컨텍스트
var context = new PluginLoadContext(dllPath);
var assembly = context.LoadFromAssemblyPath(dllPath);
```

**장점:**
- ? 완전한 격리
- ? 언로드 가능 (`isCollectible: true`)
- ? Hot Deploy 완벽 지원
- ? 의존성 제어 가능

**단점:**
- ? **DI 타입 불일치** (기본 컨텍스트와 격리)
- ? **EventBus 타입 불일치** (각 컨텍스트마다 다른 타입)
- ?? 구현 복잡도 높음
- ?? 공유 어셈블리 관리 필요

**SI 프로젝트 적합성:** ?? **조건부 적합** (DI/EventBus 포기 시)

---

## ??? SI 프로젝트 요구사항

### 아키텍처 구조

```
┌──────────────────────────────────────────────────────┐
│              nU3.Shell (메인 호스트)                  │
│  ┌────────────────────────────────────────────────┐  │
│  │          ServiceProvider (DI Container)        │  │
│  │  - IBizLogicFactory                            │  │
│  │  - IEventAggregator (EventBus)                 │  │
│  │  - IAuthenticationService                      │  │
│  │  - IMenuRepository                             │  │
│  └────────────────────────────────────────────────┘  │
└──────────────────────────────────────────────────────┘
                         ▼
        ┌────────────────┴────────────────┐
        ▼                                 ▼
┌───────────────────┐           ┌───────────────────┐
│   Module A.dll    │           │   Module B.dll    │
│  (Team A 개발)     │           │  (Team B 개발)     │
│                   │           │                   │
│  public class     │           │  public class     │
│  FormA :          │           │  FormB :          │
│  BaseWorkControl  │           │  BaseWorkControl  │
│  {                │           │  {                │
│    // DI 생성자   │           │    // DI 생성자   │
│    public FormA(  │           │    public FormB(  │
│      IBizLogic,   │?─────────?│      IEventBus,   │
│      IEventBus)   │ EventBus  │      IBizLogic)   │
│    { }            │   통신     │    { }            │
│  }                │           │  }                │
└───────────────────┘           └───────────────────┘
```

### 필수 기능

#### 1. DI (Dependency Injection)

```csharp
// Shell에서 모듈 인스턴스 생성
var control = (Control)ActivatorUtilities.CreateInstance(_serviceProvider, type);

// ? 모듈에서 생성자 주입
public FormA(IBizLogicFactory logicFactory, IEventAggregator eventBus)
{
    _logicFactory = logicFactory;
    _eventBus = eventBus;
}
```

**요구사항:**
- Shell의 `ServiceProvider`가 모듈의 타입을 인식해야 함
- 모듈이 **기본 로드 컨텍스트**에 로드되어야 함

#### 2. EventBus (모듈 간 통신)

```csharp
// Module A에서 이벤트 발행
_eventAggregator.GetEvent<PatientSelectedEvent>()
    .Publish(new PatientSelectedEventPayload 
    { 
        Patient = patient,
        Source = "ModuleA"
    });

// Module B에서 이벤트 수신
_eventAggregator.GetEvent<PatientSelectedEvent>()
    .Subscribe(OnPatientSelected);

void OnPatientSelected(PatientSelectedEventPayload payload)
{
    // payload 처리
}
```

**요구사항:**
- 모든 모듈이 **같은 타입**으로 인식해야 함
- `PatientSelectedEventPayload` 타입이 모든 컨텍스트에서 동일해야 함

#### 3. 공유 어셈블리

```
공유 어셈블리 (모든 모듈이 공통 사용):
├─ nU3.Core (프레임워크 코어)
├─ nU3.Core.UI (UI 기반 클래스)
├─ nU3.Models (DTO 모델)
├─ nU3.Connectivity (서버 연결)
└─ Microsoft.Extensions.DependencyInjection.Abstractions
```

**요구사항:**
- 모든 모듈이 **같은 인스턴스**를 공유해야 함
- 중복 로드 방지 (메모리 절약)

---

## ? 최종 채택 전략: LoadFrom + Shadow Copy

### 전략 개요

```
┌─────────────────────────────────────────────────────┐
│              LoadFrom (기본 로드 컨텍스트)           │
│  ┌──────────────────────────────────────────────┐   │
│  │ ? DI 완벽 지원                              │   │
│  │ ? EventBus 타입 호환                        │   │
│  │ ? 공유 어셈블리 자동 관리                   │   │
│  └──────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────┘
                      +
┌─────────────────────────────────────────────────────┐
│          Shadow Copy (파일 잠금 회피)                │
│  ┌──────────────────────────────────────────────┐   │
│  │ ? Hot Deploy 가능                           │   │
│  │ ? 원본 파일 잠금 방지                       │   │
│  │ ? 버전별 디렉토리 관리                      │   │
│  └──────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────┘
                      +
┌─────────────────────────────────────────────────────┐
│           버전 충돌 정책 (타입 일관성)               │
│  ┌──────────────────────────────────────────────┐   │
│  │ ? 같은 모듈은 같은 버전 사용                │   │
│  │ ? 버전 충돌 감지 및 경고                    │   │
│  │ ? 사용자에게 재시작 권장                    │   │
│  └──────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────┘
```

### 왜 이 전략인가?

| 항목 | LoadFrom | LoadFile | ALC | 선택 이유 |
|------|----------|----------|-----|-----------|
| **DI 지원** | ? | ? | ?? | DI는 필수 |
| **EventBus** | ? | ? | ?? | 모듈 간 통신 필수 |
| **공유 의존성** | ? | ? | ?? | 메모리 효율 |
| **Hot Deploy** | ?? | ? | ? | Shadow Copy로 해결 |
| **구현 복잡도** | ? | ? | ? | 단순함 선호 |

**결론:** LoadFrom이 SI 프로젝트에 가장 적합

---

## ?? 구현 상세

### 1. 디렉토리 구조

```
C:\nU3Framework\
├── Runtime\               # 실행 디렉토리
│   ├── nU3.Shell.exe
│   ├── nU3.Core.dll       # 공유 어셈블리
│   ├── nU3.Models.dll     # 공유 어셈블리
│   └── Modules\
│       ├── OCS\
│       │   └── IN\
│       │       └── OrderEntry.dll   # 모듈 DLL
│       └── EMR\
│           └── IN\
│               └── Worklist.dll
│
└── Cache\                 # 캐시 디렉토리
    ├── Downloads\         # 다운로드 임시 저장
    └── Shadow\            # Shadow Copy 디렉토리
        ├── OCS_IN_OrderEntry\
        │   ├── 1.0.0\     # 버전별 디렉토리
        │   │   ├── OrderEntry.dll
        │   │   └── Dependencies.dll
        │   └── 1.1.0\
        │       └── OrderEntry.dll
        └── EMR_IN_Worklist\
            └── 1.0.0\
                └── Worklist.dll
```

### 2. ModuleLoaderService 핵심 코드

#### 2.1 초기화

```csharp
public class ModuleLoaderService
{
    private readonly Dictionary<string, Type> _progRegistry;
    private readonly Dictionary<string, string> _loadedModuleVersions;  // ModuleId -> Version
    private readonly Dictionary<string, string> _shadowCopyPaths;        // ModuleId -> Shadow Path
    
    private readonly string _runtimePath;
    private readonly string _cachePath;
    private readonly string _shadowCopyDirectory;
    
    public ModuleLoaderService(/* ... */)
    {
        _progRegistry = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);
        _loadedModuleVersions = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        _shadowCopyPaths = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        
        _runtimePath = configuration?.GetValue<string>("RuntimeDirectory") 
            ?? AppDomain.CurrentDomain.BaseDirectory;
        _cachePath = PathConstants.CacheDirectory;
        _shadowCopyDirectory = Path.Combine(_cachePath, "Shadow");
        
        EnsureDirectories();
    }
    
    private void EnsureDirectories()
    {
        if (!Directory.Exists(_runtimePath)) 
            Directory.CreateDirectory(_runtimePath);
        if (!Directory.Exists(_cachePath)) 
            Directory.CreateDirectory(_cachePath);
        if (!Directory.Exists(_shadowCopyDirectory)) 
            Directory.CreateDirectory(_shadowCopyDirectory);
    }
}
```

#### 2.2 Shadow Copy 생성

```csharp
/// <summary>
/// Shadow Copy를 생성하여 원본 파일 잠금을 방지합니다.
/// Hot Deploy를 지원하면서도 DI 호환성을 유지합니다.
/// </summary>
private string CreateShadowCopy(string originalPath, string moduleId, string version)
{
    try
    {
        // 모듈별 Shadow 디렉토리 생성
        string moduleShadowDir = Path.Combine(_shadowCopyDirectory, moduleId, version);
        if (!Directory.Exists(moduleShadowDir))
        {
            Directory.CreateDirectory(moduleShadowDir);
        }

        // Shadow 파일 경로
        string shadowPath = Path.Combine(moduleShadowDir, Path.GetFileName(originalPath));

        // 이미 존재하면 재사용 (같은 버전)
        if (File.Exists(shadowPath))
        {
            Debug.WriteLine($"[ModuleLoader] Using existing shadow copy: {shadowPath}");
            return shadowPath;
        }

        // 원본 파일을 Shadow 위치로 복사
        File.Copy(originalPath, shadowPath, overwrite: true);

        // 의존성 DLL도 복사 (같은 디렉토리에 있는 경우)
        string originalDir = Path.GetDirectoryName(originalPath);
        if (!string.IsNullOrEmpty(originalDir))
        {
            foreach (var depFile in Directory.GetFiles(originalDir, "*.dll"))
            {
                if (Path.GetFileName(depFile) != Path.GetFileName(originalPath))
                {
                    string depShadowPath = Path.Combine(moduleShadowDir, Path.GetFileName(depFile));
                    try
                    {
                        File.Copy(depFile, depShadowPath, overwrite: true);
                    }
                    catch
                    {
                        // 의존성 복사 실패는 무시 (이미 로드되어 있을 수 있음)
                    }
                }
            }
        }

        // Shadow 경로 추적 (정리용)
        _shadowCopyPaths[moduleId] = moduleShadowDir;

        Debug.WriteLine($"[ModuleLoader] Created shadow copy: {originalPath} -> {shadowPath}");
        return shadowPath;
    }
    catch (Exception ex)
    {
        Debug.WriteLine($"[ModuleLoader] !!! Failed to create shadow copy: {ex.Message}");
        // Shadow copy 실패 시 원본 사용 (재시작 필요)
        return originalPath;
    }
}
```

#### 2.3 ReloadModule (버전 충돌 감지)

```csharp
private void ReloadModule(string dllPath, string moduleId, string version)
{
    try
    {
        // 이미 로드된 버전 확인
        if (_loadedModuleVersions.TryGetValue(moduleId, out var currentVersion))
        {
            // 같은 버전이면 스킵
            if (string.Equals(currentVersion, version, StringComparison.OrdinalIgnoreCase))
            {
                Debug.WriteLine($"[ModuleLoader] Module {moduleId} v{version} already loaded. Reusing existing version.");
                return;
            }
            
            // ?? 다른 버전 감지 - 버전 불일치 경고
            Debug.WriteLine($"[ModuleLoader] !!! VERSION CONFLICT DETECTED !!!");
            Debug.WriteLine($"[ModuleLoader] Module: {moduleId}");
            Debug.WriteLine($"[ModuleLoader] Current Version: {currentVersion}");
            Debug.WriteLine($"[ModuleLoader] Requested Version: {version}");
            Debug.WriteLine($"[ModuleLoader] Action: Using EXISTING version to prevent type mismatch.");
            Debug.WriteLine($"[ModuleLoader] Recommendation: Close all instances of this module and restart.");
            
            // 버전 충돌 이벤트 발생
            RaiseVersionConflict(moduleId, currentVersion, version);
            
            // 기존 버전 유지 (타입 불일치 방지)
            return;
        }
        
        // 최초 로드 - Shadow Copy 생성
        string shadowPath = CreateShadowCopy(dllPath, moduleId, version);
        
        // LoadFrom을 사용하여 기본 로드 컨텍스트에서 로드 (DI 호환성 유지)
        var assembly = Assembly.LoadFrom(shadowPath);
        
        foreach (var type in assembly.GetTypes())
        {
            var attr = type.GetCustomAttribute<nU3ProgramInfoAttribute>();
            if (attr != null)
            {
                _progRegistry[attr.ProgramId] = type;
                _progAttributeCache[attr.ProgramId] = attr;
                _loadedModuleVersions[moduleId] = version;
                Debug.WriteLine($"[ModuleLoader] Loaded: {attr.ProgramId} -> v{version} (Shadow: {shadowPath})");
            }
        }
    }
    catch (Exception ex)
    {
        Debug.WriteLine($"[ModuleLoader] !!! Error reloading {Path.GetFileName(dllPath)}: {ex.Message}");
    }
}
```

#### 2.4 DeployToRuntime (파일 잠금 처리)

```csharp
private void DeployToRuntime(string cacheFile, string runtimeFile, string moduleName, string version)
{
    try
    {
        string runtimeDir = Path.GetDirectoryName(runtimeFile);
        if (!Directory.Exists(runtimeDir)) 
            Directory.CreateDirectory(runtimeDir);

        File.Copy(cacheFile, runtimeFile, true);
        Debug.WriteLine($"[ModuleLoader] Deployed to runtime: {moduleName} v{version}");
    }
    catch (IOException ioEx)
    {
        // ? 파일 잠금 - Shadow Copy로 Hot Deploy 가능
        Debug.WriteLine($"[ModuleLoader] Runtime file locked: {moduleName}. Using shadow copy for hot deploy.");
        Debug.WriteLine($"[ModuleLoader] Original file will be updated on next restart. Shadow copy allows immediate use.");
        
        // Shadow copy 방식이므로 예외를 던지지 않음
        // 다음 ReloadModule에서 shadow copy 사용
    }
    catch (Exception ex)
    {
        Debug.WriteLine($"[ModuleLoader] !!! Error deploying {moduleName}: {ex.Message}");
        throw;
    }
}
```

---

## ?? 버전 충돌 처리

### 문제 시나리오

```
시간순서:
1. A.dll v1.0 로드 → Shadow Copy 생성
2. Shell에서 FormA 실행 (A.dll v1.0의 타입)
3. 서버에서 A.dll v1.1 다운로드
4. FormB 실행 요청 → A.dll v1.1의 Shadow Copy 생성 요청
5. ?? 버전 충돌 감지!
```

### 발생 가능한 문제

| 문제 | 설명 | 위험도 |
|------|------|--------|
| **타입 불일치** | FormA(v1.0)와 FormB(v1.1)가 서로 다른 Assembly | ?? 높음 |
| **공유 상태 깨짐** | static 변수, 싱글톤이 두 버전에서 별도 존재 | ?? 높음 |
| **EventBus 오류** | 타입 검사 실패 (`is`, `as` 연산자) | ?? 높음 |
| **DI 충돌** | 같은 인터페이스, 다른 구현 버전 | ?? 중간 |

### 해결 방법: 버전 강제 정책

```csharp
// 버전 충돌 이벤트
public event EventHandler<ModuleVersionConflictEventArgs>? VersionConflict;

// 버전 충돌 이벤트 인자
public class ModuleVersionConflictEventArgs : EventArgs
{
    public string ModuleId { get; set; }
    public string CurrentVersion { get; set; }
    public string RequestedVersion { get; set; }
    public DateTime Timestamp { get; set; }
}
```

#### Shell에서 이벤트 구독

```csharp
// nUShell.cs 생성자
_moduleLoader.VersionConflict += OnModuleVersionConflict;

private void OnModuleVersionConflict(object sender, ModuleVersionConflictEventArgs e)
{
    // UI 스레드에서 실행
    if (this.InvokeRequired)
    {
        this.Invoke(new Action(() => OnModuleVersionConflict(sender, e)));
        return;
    }

    // 로그 기록
    LogManager.Warning(
        $"모듈 버전 충돌 감지 - Module: {e.ModuleId}, " +
        $"Current: v{e.CurrentVersion}, Requested: v{e.RequestedVersion}", 
        "Shell");

    // 사용자에게 알림
    var result = XtraMessageBox.Show(
        $"?? 모듈 버전 불일치 감지\n\n" +
        $"모듈: {e.ModuleId}\n" +
        $"현재 로드된 버전: v{e.CurrentVersion}\n" +
        $"요청된 버전: v{e.RequestedVersion}\n\n" +
        $"타입 불일치를 방지하기 위해 현재 버전을 계속 사용합니다.\n\n" +
        $"권장 사항:\n" +
        $"- 이 모듈의 모든 인스턴스(탭)를 닫으세요.\n" +
        $"- 프로그램을 재시작하세요.\n\n" +
        $"프로그램을 지금 재시작하시겠습니까?",
        "모듈 버전 충돌",
        MessageBoxButtons.YesNo,
        MessageBoxIcon.Warning);

    if (result == DialogResult.Yes)
    {
        LogManager.Info("사용자가 버전 충돌 해결을 위해 재시작 선택", "Shell");
        Application.Restart();
        Environment.Exit(0);
    }
}
```

---

## ?? Hot Deploy 시나리오

### 시나리오 1: 정상 업데이트 (프로그램 실행 안 됨)

```
1. 서버에서 Module A v1.1 다운로드
   └─> Cache/Downloads/ModuleA.dll

2. Runtime으로 복사 시도
   ├─> ? 성공 (파일 잠금 없음)
   └─> Runtime/Modules/OCS/IN/OrderEntry.dll (v1.1)

3. ReloadModule 호출
   ├─> Shadow Copy 생성
   │   └─> Cache/Shadow/OCS_IN_OrderEntry/1.1.0/OrderEntry.dll
   ├─> LoadFrom(shadowPath)
   └─> ? v1.1 로드 완료

4. 사용자가 FormA 실행
   └─> ? v1.1 인스턴스 사용
```

### 시나리오 2: Hot Deploy (프로그램 실행 중)

```
1. FormA v1.0 실행 중
   └─> Runtime/Modules/OCS/IN/OrderEntry.dll (v1.0) 잠김

2. 서버에서 Module A v1.1 다운로드
   └─> Cache/Downloads/ModuleA.dll (v1.1)

3. Runtime으로 복사 시도
   ├─> ? IOException (파일 잠김)
   └─> 예외 무시 (계속 진행)

4. ReloadModule 호출
   ├─> ?? 버전 충돌 감지 (v1.0 vs v1.1)
   ├─> 기존 v1.0 유지 (타입 일관성)
   └─> VersionConflict 이벤트 발생

5. 사용자에게 알림
   ┌────────────────────────────────┐
   │ ?? 모듈 버전 불일치 감지       │
   │ 현재: v1.0, 요청: v1.1         │
   │ 재시작하시겠습니까?            │
   │     [예]    [아니오]           │
   └────────────────────────────────┘

6a. 사용자가 [아니오] 선택
    └─> FormA 계속 사용 (v1.0)

6b. 사용자가 [예] 선택
    ├─> Application.Restart()
    └─> 재시작 후 v1.1 로드
```

### 시나리오 3: 같은 버전 재로드

```
1. FormA v1.0 로드됨
   └─> _loadedModuleVersions["OCS_IN_OrderEntry"] = "1.0.0"

2. FormB 실행 요청 (같은 모듈)
   └─> ReloadModule 호출

3. 버전 확인
   ├─> currentVersion = "1.0.0"
   ├─> requestedVersion = "1.0.0"
   └─> ? 같은 버전 → 스킵

4. 기존 타입 재사용
   └─> _progRegistry에서 타입 반환
```

---

## ?? 개발팀 가이드라인

### 1. 모듈 개발 규칙

#### 네임스페이스 규칙
```csharp
namespace nU3.Modules.{System}.{SubSystem}.{ModuleName}

// 예시
namespace nU3.Modules.OCS.IN.OrderEntry  // ? 올바른 형식
namespace nU3.Modules.EMR.IN.Worklist   // ? 올바른 형식
namespace MyModule                       // ? 잘못된 형식
```

#### Attribute 사용
```csharp
[nU3ProgramInfo(typeof(OrderEntryControl), "처방 입력", "OCS_IN_ORDER_001")]
public class OrderEntryControl : BaseWorkControl
{
    private readonly IBizLogicFactory _logicFactory;
    private readonly IEventAggregator _eventBus;
    
    // ? DI 생성자
    public OrderEntryControl(
        IBizLogicFactory logicFactory,
        IEventAggregator eventBus)
    {
        _logicFactory = logicFactory;
        _eventBus = eventBus;
        
        InitializeComponent();
    }
}
```

### 2. 공유 어셈블리 버전 통일

```xml
<!-- 모든 모듈 프로젝트에서 동일한 버전 사용 -->
<ItemGroup>
  <ProjectReference Include="..\..\..\..\nU3.Core\nU3.Core.csproj" />
  <ProjectReference Include="..\..\..\..\nU3.Models\nU3.Models.csproj" />
  <ProjectReference Include="..\..\..\..\nU3.Connectivity\nU3.Connectivity.csproj" />
</ItemGroup>
```

**중요:** 공유 어셈블리는 반드시 같은 버전 사용!

### 3. EventBus 사용 패턴

#### 이벤트 정의 (nU3.Core.Events)
```csharp
// 이벤트 클래스
public class PatientSelectedEvent : PubSubEvent<PatientSelectedEventPayload> { }

// 페이로드
public class PatientSelectedEventPayload
{
    public PatientInfoDto Patient { get; set; }
    public string Source { get; set; }
}
```

#### 모듈에서 이벤트 발행
```csharp
// Module A에서 환자 선택 이벤트 발행
_eventBus.GetEvent<PatientSelectedEvent>()
    .Publish(new PatientSelectedEventPayload
    {
        Patient = selectedPatient,
        Source = "OrderEntry"
    });
```

#### 모듈에서 이벤트 구독
```csharp
// Module B에서 환자 선택 이벤트 수신
_eventBus.GetEvent<PatientSelectedEvent>()
    .Subscribe(OnPatientSelected);

private void OnPatientSelected(PatientSelectedEventPayload payload)
{
    // UI 스레드 체크
    if (this.InvokeRequired)
    {
        this.Invoke(new Action(() => OnPatientSelected(payload)));
        return;
    }
    
    // 환자 정보 업데이트
    UpdatePatientInfo(payload.Patient);
}
```

### 4. 버전 관리 정책

#### 모듈 버전 규칙
```
Major.Minor.Patch 형식 사용

- Major: 호환되지 않는 변경 (API 변경, 타입 변경)
- Minor: 하위 호환 기능 추가
- Patch: 버그 수정

예시:
1.0.0 → 초기 버전
1.1.0 → 새 기능 추가 (하위 호환)
2.0.0 → API 변경 (호환 불가)
```

#### 배포 시 주의사항
```
1. 같은 모듈의 여러 버전은 동시 사용 불가
2. 버전 업데이트 시 통합 테스트 필수
3. 공유 어셈블리 버전 변경 시 모든 모듈 재빌드
4. Hot Deploy는 보조 수단 (재시작 권장)
```

---

## ?? DI 호환성 상세

### Shell의 DI Container

```csharp
// Program.cs
var services = new ServiceCollection();

// 공유 서비스 등록
services.AddSingleton<IBizLogicFactory, BizLogicFactory>();
services.AddSingleton<IEventAggregator, EventAggregator>();
services.AddScoped<IAuthenticationService, AuthenticationService>();

var serviceProvider = services.BuildServiceProvider();
```

### 모듈 인스턴스 생성

```csharp
// nUShell.cs
private Control CreateProgramContent(Type type)
{
    // ? ActivatorUtilities가 DI를 수행
    var content = (Control)ActivatorUtilities.CreateInstance(_serviceProvider, type);
    
    // ? LoadFrom으로 로드된 타입도 정상 작동
    // ? LoadFile로 로드된 타입은 타입 불일치로 실패
    
    return content;
}
```

### 왜 LoadFrom만 DI가 작동하는가?

```csharp
// ActivatorUtilities 내부 동작
public static object CreateInstance(IServiceProvider provider, Type instanceType, ...)
{
    // 1. 타입 검사
    var constructor = instanceType.GetConstructors()[0];
    var parameters = constructor.GetParameters();
    
    // 2. 파라미터 타입이 ServiceProvider의 타입과 호환되는지 확인
    foreach (var param in parameters)
    {
        var service = provider.GetService(param.ParameterType);
        
        // ? LoadFrom: 같은 로드 컨텍스트 → 타입 일치
        // ? LoadFile: 다른 로드 컨텍스트 → 타입 불일치
        
        if (service == null && !param.IsOptional)
            throw new InvalidOperationException($"No service for type '{param.ParameterType}'");
    }
    
    // 3. 인스턴스 생성
    return Activator.CreateInstance(instanceType, args);
}
```

---

## ?? EventBus 호환성 상세

### EventBus 타입 검사

```csharp
// EventAggregator 내부
public class PubSubEvent<TPayload>
{
    private readonly List<Action<TPayload>> _subscriptions;
    
    public void Publish(TPayload payload)
    {
        foreach (var handler in _subscriptions)
        {
            // ? LoadFrom: TPayload 타입이 모든 모듈에서 동일
            // ? LoadFile: TPayload 타입이 각 로드마다 다름
            handler(payload);
        }
    }
    
    public void Subscribe(Action<TPayload> handler)
    {
        _subscriptions.Add(handler);
    }
}
```

### 타입 일치 예시

```csharp
// Module A (LoadFrom)
_eventBus.GetEvent<PatientSelectedEvent>()
    .Publish(new PatientSelectedEventPayload { Patient = patient });
// ? PatientSelectedEventPayload 타입: Assembly "nU3.Models, Version=1.0.0"

// Module B (LoadFrom)
_eventBus.GetEvent<PatientSelectedEvent>()
    .Subscribe(OnPatientSelected);
// ? PatientSelectedEventPayload 타입: Assembly "nU3.Models, Version=1.0.0"
// ? 타입 일치 → 정상 수신

// Module C (LoadFile)
_eventBus.GetEvent<PatientSelectedEvent>()
    .Subscribe(OnPatientSelected);
// ? PatientSelectedEventPayload 타입: Assembly "nU3.Models, Version=1.0.0" (다른 인스턴스)
// ? 타입 불일치 → 수신 실패
```

---

## ?? Hot Deploy 작동 원리

### 1. Shadow Copy 메커니즘

```
원본 파일 (Runtime):
├─ Runtime/Modules/OCS/IN/OrderEntry.dll (v1.0) ← 잠김 (실행 중)

Shadow Copy (Cache):
└─ Cache/Shadow/OCS_IN_OrderEntry/
   ├── 1.0.0/
   │   └── OrderEntry.dll ← LoadFrom에서 로드됨
   └── 1.1.0/
       └── OrderEntry.dll ← 새 버전 (다운로드됨)
```

### 2. 파일 잠금 회피

```csharp
// DeployToRuntime
try
{
    File.Copy(cacheFile, runtimeFile, true);  // 원본 업데이트 시도
}
catch (IOException)
{
    // ? 파일 잠김 → 예외 무시
    // ? Shadow Copy가 대신 사용됨
    // ? 재시작 시 원본 업데이트됨
}
```

### 3. 버전별 Shadow 관리

```csharp
// Shadow Copy 경로
string moduleShadowDir = Path.Combine(_shadowCopyDirectory, moduleId, version);
// 예: Cache/Shadow/OCS_IN_OrderEntry/1.1.0/

// ? 버전별로 디렉토리 분리
// ? 같은 버전은 재사용
// ? 다른 버전은 별도 관리
```

---

## ?? 개발팀 가이드라인

### 모듈 개발 체크리스트

- [ ] **네임스페이스 규칙 준수** (`nU3.Modules.{System}.{SubSystem}.{Module}`)
- [ ] **nU3ProgramInfoAttribute 사용** (`[nU3ProgramInfo(typeof(MyControl), ...)]`)
- [ ] **BaseWorkControl 상속** (또는 BaseWorkForm)
- [ ] **DI 생성자 구현** (필요한 서비스 주입)
- [ ] **EventBus 구독/발행** (모듈 간 통신)
- [ ] **ILifecycleAware 구현** (생명주기 관리)
- [ ] **공유 어셈블리 버전 통일** (nU3.Core, nU3.Models 등)
- [ ] **버전 관리** (Major.Minor.Patch)

### 모듈 템플릿

```csharp
using System;
using System.Windows.Forms;
using nU3.Core.UI;
using nU3.Core.Events;
using nU3.Core.Logic;
using nU3.Core.Attributes;
using nU3.Core.Interfaces;

namespace nU3.Modules.{System}.{SubSystem}.{ModuleName}
{
    [nU3ProgramInfo(
        typeof(MyModuleControl), 
        "모듈 이름", 
        "{SYSTEM}_{SUBSYSTEM}_MODULE_001")]
    public partial class MyModuleControl : BaseWorkControl, ILifecycleAware
    {
        private readonly IBizLogicFactory _logicFactory;
        private readonly IEventAggregator _eventBus;
        
        // ? DI 생성자
        public MyModuleControl(
            IBizLogicFactory logicFactory,
            IEventAggregator eventBus)
        {
            _logicFactory = logicFactory;
            _eventBus = eventBus;
            
            InitializeComponent();
            SubscribeToEvents();
        }
        
        // ? EventBus 구독
        private void SubscribeToEvents()
        {
            _eventBus?.GetEvent<PatientSelectedEvent>()
                .Subscribe(OnPatientSelected);
        }
        
        private void OnPatientSelected(PatientSelectedEventPayload payload)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => OnPatientSelected(payload)));
                return;
            }
            
            // 환자 정보 업데이트
            UpdatePatientInfo(payload.Patient);
        }
        
        // ? Lifecycle 구현
        public void OnActivated()
        {
            // 탭 활성화 시
        }
        
        public void OnDeactivated()
        {
            // 탭 비활성화 시
        }
        
        public bool CanClose()
        {
            // 저장되지 않은 데이터 확인
            if (HasUnsavedChanges())
            {
                var result = MessageBox.Show(
                    "저장되지 않은 데이터가 있습니다. 닫으시겠습니까?",
                    "확인",
                    MessageBoxButtons.YesNo);
                return result == DialogResult.Yes;
            }
            return true;
        }
    }
}
```

### 2. 버전 관리 정책

#### AssemblyInfo 설정

```csharp
// Properties/AssemblyInfo.cs 또는 .csproj
[assembly: AssemblyVersion("1.0.0")]
[assembly: AssemblyFileVersion("1.0.0")]
```

```xml
<!-- .csproj -->
<PropertyGroup>
  <AssemblyVersion>1.0.0</AssemblyVersion>
  <FileVersion>1.0.0</FileVersion>
  <Version>1.0.0</Version>
</PropertyGroup>
```

#### 버전 업데이트 시나리오

```
개발:
1. 기능 추가/수정
2. 버전 증가 (1.0.0 → 1.1.0)
3. 빌드

테스트:
4. 로컬 테스트
5. 통합 테스트 (다른 모듈과 함께)
6. EventBus 통신 확인

배포:
7. Deployer로 서버에 업로드
8. DB에 버전 정보 등록
9. 클라이언트 자동 업데이트 (Bootstrapper)
```

### 3. 의존성 관리

#### 권장 의존성 구조

```
각 모듈 프로젝트:
├─ nU3.Core (필수) ?
├─ nU3.Core.UI (필수) ?
├─ nU3.Models (필수) ?
├─ nU3.Connectivity (필수) ?
├─ DevExpress.* (UI 라이브러리) ?
└─ 모듈 전용 라이브러리 (선택) ??
```

**주의사항:**
- ?? 모듈 전용 라이브러리는 반드시 같은 디렉토리에 배포
- ?? Shadow Copy 시 자동으로 복사됨
- ?? 공유 라이브러리와 이름 중복 방지

---

## ??? 문제 해결 가이드

### 문제 1: DI 생성자 주입 실패

**증상:**
```
InvalidOperationException: No service for type 'IBizLogicFactory' has been registered.
```

**원인:**
- Shell의 `ServiceProvider`에 서비스가 등록되지 않음

**해결:**
```csharp
// Program.cs에 서비스 등록
services.AddSingleton<IBizLogicFactory, BizLogicFactory>();
```

### 문제 2: EventBus 이벤트 수신 안 됨

**증상:**
```
이벤트를 발행해도 다른 모듈에서 수신되지 않음
```

**원인:**
- 타입 불일치 (LoadFile 사용 또는 버전 충돌)

**해결:**
```csharp
// 1. LoadFrom 사용 확인
var assembly = Assembly.LoadFrom(shadowPath);  // ?

// 2. 공유 어셈블리 버전 통일 확인
// nU3.Core, nU3.Models 버전이 모든 모듈에서 동일해야 함
```

### 문제 3: 버전 충돌 경고

**증상:**
```
?? 모듈 버전 불일치 감지
현재 로드된 버전: v1.0.0
요청된 버전: v1.1.0
```

**원인:**
- 모듈이 이미 로드된 상태에서 새 버전 요청

**해결:**
```
1. 해당 모듈의 모든 탭 닫기
2. 프로그램 재시작
3. 새 버전 자동 로드
```

### 문제 4: Hot Deploy 실패

**증상:**
```
IOException: The process cannot access the file because it is being used by another process.
```

**원인:**
- 원본 파일이 잠겨 있음 (정상 동작)

**해결:**
```
? Shadow Copy 자동 사용됨
? 재시작 시 원본 파일 업데이트됨
```

---

## ?? 성능 및 메모리 관리

### Shadow Copy 디스크 사용량

```
예상 디스크 사용량:
- 모듈 1개당: ~10-50 MB
- 버전 3개 보관 시: ~30-150 MB
- 모듈 10개 × 3버전: ~300-1,500 MB
```

### Shadow Copy 정리 (선택 구현)

```csharp
/// <summary>
/// 오래된 Shadow Copy 버전을 정리합니다.
/// </summary>
public void CleanupOldShadowCopies(int keepRecentVersions = 3)
{
    foreach (var moduleDir in Directory.GetDirectories(_shadowCopyDirectory))
    {
        var versions = Directory.GetDirectories(moduleDir)
            .OrderByDescending(d => Directory.GetCreationTime(d))
            .Skip(keepRecentVersions);
        
        foreach (var oldVersion in versions)
        {
            try
            {
                Directory.Delete(oldVersion, recursive: true);
                Debug.WriteLine($"[ModuleLoader] Cleaned up old shadow copy: {oldVersion}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ModuleLoader] Failed to cleanup {oldVersion}: {ex.Message}");
            }
        }
    }
}

// Shell 종료 시 호출
private void MainShellForm_FormClosing(object sender, FormClosingEventArgs e)
{
    if (!e.Cancel)
    {
        // Shadow Copy 정리 (최근 3버전만 유지)
        _moduleLoader.CleanupOldShadowCopies(keepRecentVersions: 3);
    }
}
```

### 메모리 사용량

```
예상 메모리 사용량:
- 모듈 1개당: ~50-200 MB (DevExpress UI 포함)
- LoadFrom (공유): ~50-200 MB
- LoadFile (격리): ~50-200 MB × N개
- AssemblyLoadContext: ~50-200 MB (공유 어셈블리는 공통)

? LoadFrom이 가장 효율적
```

---

## ?? 베스트 프랙티스

### 1. 모듈 개발

```csharp
// ? 올바른 예시
namespace nU3.Modules.OCS.IN.OrderEntry
{
    [nU3ProgramInfo(typeof(OrderEntryControl), "처방 입력", "OCS_IN_ORDER_001")]
    public partial class OrderEntryControl : BaseWorkControl
    {
        private readonly IBizLogicFactory _logicFactory;
        private readonly IEventAggregator _eventBus;
        
        public OrderEntryControl(
            IBizLogicFactory logicFactory,
            IEventAggregator eventBus)
        {
            _logicFactory = logicFactory;
            _eventBus = eventBus;
            InitializeComponent();
        }
    }
}
```

```csharp
// ? 잘못된 예시
namespace MyModule  // ? 네임스페이스 규칙 위반
{
    // ? Attribute 없음
    public partial class MyControl : UserControl  // ? BaseWorkControl 상속 안 함
    {
        public MyControl()  // ? DI 생성자 없음
        {
            InitializeComponent();
        }
    }
}
```

### 2. EventBus 사용

```csharp
// ? 올바른 예시 - 강타입 이벤트
public class PatientSelectedEvent : PubSubEvent<PatientSelectedEventPayload> { }

public class PatientSelectedEventPayload
{
    public PatientInfoDto Patient { get; set; }
    public string Source { get; set; }
}

// 발행
_eventBus.GetEvent<PatientSelectedEvent>()
    .Publish(new PatientSelectedEventPayload 
    { 
        Patient = patient,
        Source = "OrderEntry"
    });

// 구독
_eventBus.GetEvent<PatientSelectedEvent>()
    .Subscribe(OnPatientSelected);
```

```csharp
// ? 잘못된 예시 - 약타입 이벤트
_eventBus.GetEvent<GenericEvent>()
    .Publish(new { Patient = patient });  // ? 익명 타입

_eventBus.GetEvent<GenericEvent>()
    .Subscribe(obj => 
    {
        var payload = obj as PatientInfo;  // ? 타입 불확실
    });
```

### 3. 버전 관리

```csharp
// ? 올바른 예시
// AssemblyInfo.cs
[assembly: AssemblyVersion("1.0.0")]
[assembly: AssemblyFileVersion("1.0.0.20260208")]

// 또는 .csproj
<PropertyGroup>
  <AssemblyVersion>1.0.0</AssemblyVersion>
  <FileVersion>1.0.0.20260208</FileVersion>
</PropertyGroup>
```

```xml
<!-- ? 잘못된 예시 -->
<PropertyGroup>
  <AssemblyVersion>1.0.*</AssemblyVersion>  <!-- ? 와일드카드 사용 -->
  <FileVersion>1.0.0</FileVersion>
</PropertyGroup>
```

---

## ?? 디버깅 및 로깅

### ModuleLoaderService 로그 확인

```csharp
// 로드 성공
[ModuleLoader] Loaded: OCS_IN_ORDER_001 -> v1.0.0 (Shadow: C:\...\Shadow\...)

// 버전 충돌
[ModuleLoader] !!! VERSION CONFLICT DETECTED !!!
[ModuleLoader] Module: OCS_IN_OrderEntry
[ModuleLoader] Current Version: 1.0.0
[ModuleLoader] Requested Version: 1.1.0
[ModuleLoader] Action: Using EXISTING version to prevent type mismatch.

// Shadow Copy 생성
[ModuleLoader] Created shadow copy: Runtime/... -> Cache/Shadow/...

// 파일 잠김
[ModuleLoader] Runtime file locked: OrderEntry.dll. Using shadow copy for hot deploy.
```

### Visual Studio 디버깅

```csharp
// 로드된 Assembly 확인
foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
{
    Debug.WriteLine($"Loaded: {assembly.FullName}");
    Debug.WriteLine($"Location: {assembly.Location}");
}

// 모듈 버전 확인
var version = _moduleLoader.GetLoadedModuleVersion("OCS_IN_OrderEntry");
Debug.WriteLine($"Module Version: {version}");

// 등록된 프로그램 확인
var registry = _moduleLoader.GetProgramRegistry();
foreach (var kvp in registry)
{
    Debug.WriteLine($"ProgID: {kvp.Key} -> Type: {kvp.Value.FullName}");
}
```

---

## ?? 최종 권장사항

### SI 프로젝트에 최적화된 전략

```
? LoadFrom (기본 로드 컨텍스트)
   ├─> DI 완벽 지원
   ├─> EventBus 타입 호환
   └─> 공유 어셈블리 자동 관리

? Shadow Copy (파일 잠금 회피)
   ├─> Hot Deploy 가능
   ├─> 버전별 디렉토리 관리
   └─> 의존성 자동 복사

? 버전 충돌 정책 (타입 일관성)
   ├─> 같은 모듈은 같은 버전 사용
   ├─> 버전 충돌 감지 및 경고
   └─> 사용자에게 재시작 권장
```

### 구현 완료 사항

| 기능 | 상태 | 위치 |
|------|-----|------|
| Shadow Copy 생성 | ? | `ModuleLoaderService.CreateShadowCopy()` |
| 버전 충돌 감지 | ? | `ModuleLoaderService.ReloadModule()` |
| 이벤트 발생 | ? | `ModuleLoaderService.VersionConflict` |
| 사용자 알림 | ? | `nUShell.OnModuleVersionConflict()` |
| Hot Deploy 지원 | ? | `ModuleLoaderService.DeployToRuntime()` |
| 로깅 | ? | `Debug.WriteLine` + `LogManager` |

### 향후 개선 사항 (선택)

- [ ] Shadow Copy 자동 정리 (디스크 공간 관리)
- [ ] 모듈 언로드 기능 (AssemblyLoadContext 도입)
- [ ] 버전 롤백 기능
- [ ] 모듈 의존성 그래프 분석
- [ ] 자동 통합 테스트

---

## ?? 체크리스트

### 개발팀 체크리스트

모듈 개발 시:
- [ ] 네임스페이스 규칙 준수
- [ ] nU3ProgramInfoAttribute 추가
- [ ] BaseWorkControl 상속
- [ ] DI 생성자 구현
- [ ] EventBus 구독/발행
- [ ] ILifecycleAware 구현
- [ ] 버전 관리 (AssemblyVersion)
- [ ] 공유 어셈블리 버전 통일

배포 시:
- [ ] 로컬 빌드 및 테스트
- [ ] 통합 테스트 (Shell 실행)
- [ ] EventBus 통신 확인
- [ ] Deployer로 서버 업로드
- [ ] DB 버전 정보 등록
- [ ] 클라이언트 업데이트 확인

문제 발생 시:
- [ ] 로그 확인 (Debug.WriteLine)
- [ ] Assembly 로드 경로 확인
- [ ] 버전 정보 확인
- [ ] Shadow Copy 디렉토리 확인
- [ ] 공유 어셈블리 버전 확인

---

## ?? 참고 자료

### .NET Assembly Loading

- [Assembly.LoadFrom Method](https://learn.microsoft.com/en-us/dotnet/api/system.reflection.assembly.loadfrom)
- [Assembly.LoadFile Method](https://learn.microsoft.com/en-us/dotnet/api/system.reflection.assembly.loadfile)
- [AssemblyLoadContext Class](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.loader.assemblyloadcontext)
- [Create a .NET Core application with plugins](https://learn.microsoft.com/en-us/dotnet/core/tutorials/creating-app-with-plugin-support)

### Dependency Injection

- [ActivatorUtilities Class](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.activatorutilities)
- [Dependency injection in .NET](https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection)

### 프레임워크 내부 문서

- [nU3ProgramInfoAttribute 가이드](../nU3.Core/DOC_README_PROGRAM_INFO_ATTRIBUTE.md)
- [EventBus 사용 가이드](../nU3.Core/Events/DOC_EventBus_Guide.md)
- [프레임워크 계획](../DOC/DOC_DOC_nU3.Framework%20-%20Plan.md)

---

## ?? 결론

nU3.Framework의 Assembly 로드 전략은 **SI 프로젝트의 특수성**을 고려하여 설계되었습니다:

1. ? **LoadFrom 사용** - DI와 EventBus 완벽 지원
2. ? **Shadow Copy** - Hot Deploy 가능
3. ? **버전 정책** - 타입 일관성 보장
4. ? **사용자 알림** - 명확한 UX

이 전략은 **다수 개발자 환경**에서 **독립적인 모듈 개발**과 **안정적인 통합 실행**을 모두 만족시킵니다.

**추가 질문이나 개선 제안은 GitHub Issues에 등록해 주세요!**

---

**문서 버전:** 1.0  
**최종 수정일:** 2026-02-08  
**작성자:** nU3 Framework Team  
**라이선스:** MIT

---
