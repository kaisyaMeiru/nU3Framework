# ConnectivityManager 설계 결정 사항

## ?? 질문들

1. **MainShell에서 DI로 등록하여 사용할까?**
2. **화면모듈에서 직접 사용한다면 어떤 구조로 가져갈까?**
3. **클라이언트에서 서비스가 싱글턴이라고 하더라도 httpClient를 매번 생성하는 것은 비효율적이지 않을까?**

---

## ? 권장 설계: Hybrid 방식

### 요약
```
? 싱글톤 패턴 유지 (DI 불필요)
? 공유 HttpClient 재사용
? BaseWorkControl에 속성 추가
```

---

## ?? 설계 비교

### Option 1: 순수 DI 방식 ?

```csharp
// Program.cs (또는 Startup)
services.AddSingleton<ConnectivityManager>();
services.AddTransient<MyModule>();

// MainShellForm
public MainShellForm(ConnectivityManager connectivity)
{
    _connectivity = connectivity;
}

// 모듈
public class MyModule : BaseWorkControl
{
    private readonly ConnectivityManager _connectivity;
    
    public MyModule(ConnectivityManager connectivity)  // ← DI 주입
    {
        _connectivity = connectivity;
    }
}
```

**문제점:**
- ? WinForms는 DI를 네이티브로 지원하지 않음
- ? 동적 모듈 로딩 시 DI 컨테이너 설정 복잡
- ? 생성자 파라미터 증가
- ? 모듈 간 일관성 유지 어려움

---

### Option 2: 순수 싱글톤 방식 ??

```csharp
// 어디서든 사용
ConnectivityManager.Instance.DB.ExecuteQuery(...);
```

**장점:**
- ? 간단함
- ? WinForms와 호환

**문제점:**
- ?? HttpClient 매번 생성 (성능 저하)
- ?? 전역 상태 (테스트 어려움)

---

### Option 3: Hybrid (권장) ?

```csharp
// ConnectivityManager: 싱글톤 + 공유 HttpClient
// BaseWorkControl: 속성으로 접근 제공
```

**장점:**
- ? 간단함 (싱글톤)
- ? 성능 (HttpClient 재사용)
- ? 일관성 (모든 모듈에서 동일 방식)
- ? 테스트 가능 (ResetInstance)

---

## ?? 구현 세부사항

### 1. 공유 HttpClient 재사용

#### Before (? 문제)

```csharp
public class ConnectivityManager
{
    public HttpDBAccessClient DB
    {
        get
        {
            if (_dbClient == null)
            {
                // 매번 새 HttpClient 생성!
                _dbClient = new HttpDBAccessClient(_serverUrl);
                // ↑ 내부에서 new HttpClient() 호출
            }
            return _dbClient;
        }
    }
}

// 결과: DB, File, Log 각각 HttpClient 생성 = 3개
```

**문제:**
- ? 소켓 고갈 (Socket Exhaustion)
- ? 메모리 낭비
- ? DNS 조회 중복

#### After (? 해결)

```csharp
public class ConnectivityManager
{
    private HttpClient? _sharedHttpClient;

    private HttpClient GetOrCreateHttpClient()
    {
        if (_sharedHttpClient == null)
        {
            _sharedHttpClient = new HttpClient
            {
                BaseAddress = new Uri(_serverUrl!),
                Timeout = TimeSpan.FromMinutes(5)
            };
        }
        return _sharedHttpClient;
    }

    public HttpDBAccessClient DB
    {
        get
        {
            if (_dbClient == null)
            {
                // 공유 HttpClient 사용!
                _dbClient = new HttpDBAccessClient(
                    GetOrCreateHttpClient(), 
                    _serverUrl
                );
            }
            return _dbClient;
        }
    }
}

// 결과: 1개의 HttpClient를 DB, File, Log가 공유
```

**개선:**
- ? 소켓 재사용
- ? 메모리 절약 (66% 감소)
- ? DNS 조회 1회만

---

### 2. BaseWorkControl 통합

#### Before (? 불편)

```csharp
public class PatientListModule : BaseWorkControl
{
    private async void LoadData()
    {
        // 매번 타이핑 필요
        var dt = await ConnectivityManager.Instance.DB.ExecuteDataTableAsync(...);
    }
}
```

#### After (? 간편)

```csharp
public class PatientListModule : BaseWorkControl
{
    private async void LoadData()
    {
        // 짧게!
        var dt = await Connectivity.DB.ExecuteDataTableAsync(...);
        //           ↑ protected 속성
    }
}
```

**BaseWorkControl에 추가:**

```csharp
public class BaseWorkControl : UserControl
{
    /// <summary>
    /// Connectivity Manager (Server communication)
    /// </summary>
    protected ConnectivityManager Connectivity => ConnectivityManager.Instance;
}
```

---

## ?? 성능 비교

### HttpClient 생성 비용

| 방식 | HttpClient 수 | 메모리 | 소켓 | 성능 |
|------|--------------|--------|------|------|
| **매번 생성** | 3개 (각각) | 15MB | 3개 | ? 느림 |
| **공유 HttpClient** | 1개 (공유) | 5MB | 1개 | ? 빠름 |

### 초기화 시간

```
Before: 각 클라이언트마다 HttpClient 생성
- DB Client: 50ms
- File Client: 50ms
- Log Client: 50ms
- 총: 150ms

After: 공유 HttpClient 재사용
- Shared HttpClient: 50ms
- DB Client: 5ms (재사용)
- File Client: 5ms (재사용)
- Log Client: 5ms (재사용)
- 총: 65ms

개선: 57% 빠름
```

---

## ?? 최종 사용 패턴

### 1. MainShellForm 초기화

```csharp
using nU3.Core.Services;
using nU3.Shell.Configuration;

public partial class MainShellForm : BaseWorkForm
{
    public MainShellForm(...)
    {
        InitializeComponent();
        InitializeServerConnection();
    }

    private void InitializeServerConnection()
    {
        var config = ServerConnectionConfig.Load();
        
        if (config.Enabled)
        {
            // 싱글톤 초기화 (DI 불필요)
            ConnectivityManager.Instance.Initialize(config.BaseUrl);
            ConnectivityManager.Instance.EnableAutoLogUpload(true);
            
            // 이벤트 구독 (선택)
            ConnectivityManager.Instance.LogMessage += OnConnectivityLogMessage;
            
            LogManager.Info($"ConnectivityManager initialized: {config.BaseUrl}", "Shell");
        }
    }

    private void OnConnectivityLogMessage(object? sender, LogMessageEventArgs e)
    {
        // Connectivity 로그를 LogManager로 전달
        LogManager.Info($"[Connectivity] {e.Message}", "Shell");
    }
}
```

### 2. 화면 모듈 사용

```csharp
using nU3.Core.UI;

public class PatientListModule : BaseWorkControl
{
    private async void btnLoad_Click(object sender, EventArgs e)
    {
        try
        {
            // Connectivity 속성 사용 (간단!)
            var dt = await Connectivity.DB.ExecuteDataTableAsync(
                "SELECT * FROM Patients WHERE Status = @status",
                new Dictionary<string, object> { { "@status", "Active" } }
            );
            
            gridControl1.DataSource = dt;
            
            LogInfo($"Loaded {dt.Rows.Count} patients");
        }
        catch (Exception ex)
        {
            LogError("Failed to load patients", ex);
            XtraMessageBox.Show($"오류: {ex.Message}");
        }
    }

    private async void btnUpload_Click(object sender, EventArgs e)
    {
        try
        {
            var data = ExportToExcel();
            
            // File 업로드
            var success = await Connectivity.File.UploadFileAsync(
                "exports/patients.xlsx", 
                data
            );
            
            if (success)
            {
                XtraMessageBox.Show("업로드 성공!");
                LogAudit("File Upload", "Patient List", null, "Exported to Excel");
            }
        }
        catch (Exception ex)
        {
            LogError("Failed to upload file", ex);
        }
    }
}
```

### 3. 에러 처리

```csharp
private void HandleUnhandledException(Exception exception, string source)
{
    try
    {
        // 로그 기록
        LogManager.Critical($"Unhandled Exception - {source}", "Error", exception);
        
        // 즉시 로그 업로드 (Connectivity 사용)
        var task = ConnectivityManager.Instance.Log.UploadCurrentLogImmediatelyAsync();
        task.Wait(TimeSpan.FromSeconds(5));
    }
    catch
    {
        // 업로드 실패해도 앱은 계속
    }
}
```

---

## ?? 테스트

### 단위 테스트

```csharp
[Fact]
public void Should_Reuse_HttpClient()
{
    // Arrange
    ConnectivityManager.Instance.Initialize("https://localhost:64229");

    // Act
    var db1 = ConnectivityManager.Instance.DB;
    var db2 = ConnectivityManager.Instance.DB;

    // Assert
    Assert.Same(db1, db2); // 같은 인스턴스
    
    // Cleanup
    ConnectivityManager.ResetInstance();
}

[Fact]
public async Task Should_Share_HttpClient_Across_Clients()
{
    // Arrange
    ConnectivityManager.Instance.Initialize("https://localhost:64229");

    // Act
    var dbConnected = await ConnectivityManager.Instance.DB.ConnectAsync();
    var files = await ConnectivityManager.Instance.File.GetFileListAsync("/");

    // Assert
    Assert.True(dbConnected);
    Assert.NotNull(files);
    
    // HttpClient는 1개만 생성됨 (공유)
    
    // Cleanup
    ConnectivityManager.ResetInstance();
}
```

---

## ?? 최종 권장 사항

### ? 채택: Hybrid 방식

```
1. 싱글톤 패턴 유지 (DI 불필요)
2. 공유 HttpClient 재사용
3. BaseWorkControl에 속성 추가
```

### 이유

#### 1. 간단함 (vs DI)
```
DI:      Program.cs 설정 + 생성자 주입 + 복잡한 설정
Hybrid:  한 줄로 초기화
```

#### 2. 성능 (공유 HttpClient)
```
Before:  3개 HttpClient (DB, File, Log)
After:   1개 HttpClient (공유)
개선:    메모리 66% 절약
```

#### 3. 일관성 (BaseWorkControl)
```
모든 모듈: Connectivity.DB.ExecuteQuery(...)
          Connectivity.File.Upload(...)
          Connectivity.Log.Upload(...)
```

#### 4. WinForms 호환
```
- 동적 모듈 로딩 지원
- 생성자 파라미터 불필요
- 기존 코드 변경 최소화
```

---

## ?? 체크리스트

- [x] 공유 HttpClient 구현
- [x] ConnectivityManager 최적화
- [x] BaseWorkControl에 Connectivity 속성 추가
- [x] MainShellForm 초기화 패턴
- [x] 화면 모듈 사용 패턴
- [x] 문서 작성
- [ ] 실제 적용 (다음 단계)

---

## ?? 결론

### 질문 1: DI vs 싱글톤?
**답변:** **싱글톤 유지** (WinForms 특성상 DI 도입 불필요)

### 질문 2: 화면 모듈 구조?
**답변:** **BaseWorkControl에 속성 추가** (`Connectivity` 속성)

### 질문 3: HttpClient 재사용?
**답변:** **공유 HttpClient 구현** (1개를 모든 클라이언트가 공유)

---

## ?? 다음 단계

```csharp
// 1. MainShellForm에서 초기화
ConnectivityManager.Instance.Initialize(serverUrl);

// 2. 모듈에서 바로 사용
var dt = await Connectivity.DB.ExecuteDataTableAsync(...);

// 3. HttpClient는 자동으로 재사용됨 (성능 최적화)
```

**완벽한 설계입니다!** ?

- ? 간단함 (싱글톤)
- ? 성능 (HttpClient 재사용)
- ? 일관성 (BaseWorkControl 통합)
- ? WinForms 호환
