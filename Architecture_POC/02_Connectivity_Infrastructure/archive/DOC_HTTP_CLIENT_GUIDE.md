# HTTP Client 구현체 - 사용 가이드

## ?? 개요

`nU3.Connectivity`의 추상 클래스를 **HTTP/REST로 구현**한 구체 클래스들입니다.

---

## ?? 생성된 파일

```
nU3.Connectivity/
├── Implementations/
│   ├── DBAccessClientBase.cs           # 추상 기반 클래스
│   ├── FileTransferClientBase.cs       # 추상 기반 클래스
│   ├── HttpDBAccessClient.cs          # ? HTTP 구현 (NEW)
│   ├── HttpFileTransferClient.cs      # ? HTTP 구현 (NEW)
│   └── HttpLogUploadClient.cs         # ? HTTP 구현 (NEW)
├── IDBAccessService.cs
├── IFileTransferService.cs
└── ILogUploadService.cs               # ? 인터페이스 (NEW)
```

---

## ?? 아키텍처 흐름

### Before (추상 클래스만 존재)

```
┌────────────────────────────────────┐
│  nU3.Connectivity                  │
├────────────────────────────────────┤
│  DBAccessClientBase (abstract)     │
│    - RemoteExecuteAsync<T>(...)   │ ← 구현 없음!
│                                    │
│  어떻게 서버와 통신하지? ?          │
└────────────────────────────────────┘
```

### After (HTTP 구현 추가)

```
┌────────────────────────────────────┐
│  nU3.Shell (WinForms)              │
│         ↓                          │
│  HttpDBAccessClient                │ ← 구체 클래스!
│  HttpFileTransferClient            │ ← 구체 클래스!
│  HttpLogUploadClient               │ ← 구체 클래스!
│         ↓                          │
│  RemoteExecuteAsync<T>(...)        │
│         ↓ HttpClient.PostAsync     │
│         ↓                          │
│  https://localhost:64229/api/...   │
└────────────────────────────────────┘
```

---

## ?? 사용 방법

### 1. HttpDBAccessClient 사용

```csharp
using nU3.Connectivity.Implementations;

// WinForms Form
public class MyForm : Form
{
    private readonly HttpDBAccessClient _dbClient;

    public MyForm()
    {
        // 서버 URL 지정 (Timeout 기본 5분 설정됨)
        _dbClient = new HttpDBAccessClient("https://localhost:64229");
    }

    private async void btnConnect_Click(object sender, EventArgs e)
    {
        try
        {
            // DB 연결 (POST /api/v1/db/connect)
            var connected = await _dbClient.ConnectAsync();
            
            if (connected)
            {
                // 쿼리 실행 (POST /api/v1/db/query/table)
                // 결과는 List<Dictionary<string, object>>로 수신되어 DataTable로 자동 변환됨
                var dt = await _dbClient.ExecuteDataTableAsync(
                    "SELECT * FROM Users WHERE Age > @age",
                    new Dictionary<string, object> { { "@age", 18 } }
                );
                
                dataGridView1.DataSource = dt;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"에러: {ex.Message}");
        }
    }
}
```

### 2?? **HttpFileTransferClient 사용**

```csharp
using nU3.Connectivity.Implementations;

public class FileOperations
{
    private readonly HttpFileTransferClient _fileClient;

    public FileOperations()
    {
        // 서버 URL 지정
        _fileClient = new HttpFileTransferClient("https://localhost:64229");
    }

    public async Task UploadFileAsync(string localFilePath, string serverPath)
    {
        try
        {
            // 로컬 파일 읽기
            var data = await File.ReadAllBytesAsync(localFilePath);
            
            // 서버에 업로드
            var success = await _fileClient.UploadFileAsync(serverPath, data);
            
            if (success)
            {
                Console.WriteLine("파일 업로드 성공!");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"업로드 실패: {ex.Message}");
        }
    }

    public async Task DownloadFileAsync(string serverPath, string localFilePath)
    {
        try
        {
            // 서버에서 다운로드
            var data = await _fileClient.DownloadFileAsync(serverPath);
            
            // 로컬에 저장
            await File.WriteAllBytesAsync(localFilePath, data);
            
            Console.WriteLine("파일 다운로드 성공!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"다운로드 실패: {ex.Message}");
        }
    }

    public async Task<List<string>> GetFileListAsync(string path)
    {
        return await _fileClient.GetFileListAsync(path);
    }
}
```

### 3?? **HttpLogUploadClient 사용** ? NEW!

```csharp
using nU3.Connectivity.Implementations;
using nU3.Core.Logging;

public class LogManager
{
    private readonly HttpLogUploadClient _logClient;
    private readonly FileLogger _logger;

    public LogManager()
    {
        _logger = new FileLogger();
        
        // 로그 업로드 클라이언트 생성
        _logClient = new HttpLogUploadClient(
            baseUrl: "https://localhost:64229",
            logCallback: (level, message) => LogCallback(level, message)
        );
        
        // 자동 업로드 활성화 (매일 오전 2시)
        _logClient.EnableAutoUpload(true);
    }

    private void LogCallback(string level, string message)
    {
        switch (level.ToLower())
        {
            case "info":
                _logger.Information(message, "LogUpload");
                break;
            case "warning":
                _logger.Warning(message, "LogUpload");
                break;
            case "error":
                _logger.Error(message, "LogUpload");
                break;
        }
    }

    // 오류 발생 시 즉시 로그 업로드
    public async Task OnErrorAsync(Exception ex)
    {
        _logger.Error($"Error occurred: {ex.Message}", "App", ex);
        await _logger.FlushAsync();
        
        // 현재 로그 즉시 업로드
        await _logClient.UploadCurrentLogImmediatelyAsync();
    }

    // 앱 종료 시 대기 중인 로그 모두 업로드
    public async Task OnShutdownAsync()
    {
        await _logger.FlushAsync();
        await _logClient.UploadAllPendingLogsAsync();
    }
}
```

---

## ?? HttpClient 커스터마이징

### DI (Dependency Injection) 사용

```csharp
// Startup.cs 또는 Program.cs
public void ConfigureServices(IServiceCollection services)
{
    var serverUrl = "https://localhost:64229";

    // HttpClient 등록 (IHttpClientFactory)
    services.AddHttpClient<HttpDBAccessClient>(client =>
    {
        client.BaseAddress = new Uri(serverUrl);
        client.Timeout = TimeSpan.FromMinutes(5);
        client.DefaultRequestHeaders.Add("X-API-Key", "your-api-key");
    });

    services.AddHttpClient<HttpFileTransferClient>(client =>
    {
        client.BaseAddress = new Uri(serverUrl);
        client.Timeout = TimeSpan.FromMinutes(10);
    });

    services.AddHttpClient<HttpLogUploadClient>(client =>
    {
        client.BaseAddress = new Uri(serverUrl);
        client.Timeout = TimeSpan.FromMinutes(5);
    });
}

// 사용
public class MyService
{
    private readonly HttpDBAccessClient _dbClient;
    private readonly HttpLogUploadClient _logClient;

    public MyService(
        HttpDBAccessClient dbClient,
        HttpLogUploadClient logClient)
    {
        _dbClient = dbClient;
        _logClient = logClient;
    }

    public async Task DoSomethingAsync()
    {
        var dt = await _dbClient.ExecuteDataTableAsync("SELECT * FROM Users");
        await _logClient.UploadCurrentLogImmediatelyAsync();
    }
}
```

### 인증 추가

```csharp
public class AuthenticatedHttpClient
{
    public static HttpClient CreateAuthenticatedClient(string baseUrl, string token)
    {
        var client = new HttpClient
        {
            BaseAddress = new Uri(baseUrl)
        };
        client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        return client;
    }
}

// 사용
var httpClient = AuthenticatedHttpClient.CreateAuthenticatedClient(
    "https://localhost:64229", 
    "your-jwt-token");

var dbClient = new HttpDBAccessClient(httpClient, "https://localhost:64229");
var logClient = new HttpLogUploadClient(httpClient, "https://localhost:64229");
```

---

## ?? 메서드 매핑

### HttpLogUploadClient 매핑 ? NEW!

| 클라이언트 메서드 | HTTP 메서드 | API 엔드포인트 |
|------------------|-------------|----------------|
| `UploadLogFileAsync(...)` | POST | `/api/log/upload` |
| `UploadAuditLogAsync(...)` | POST | `/api/log/upload-audit` |
| `UploadAllPendingLogsAsync()` | POST (multiple) | `/api/log/upload` |
| `UploadCurrentLogImmediatelyAsync()` | POST | `/api/log/upload` |

### HttpDBAccessClient 매핑

| 클라이언트 메서드 | HTTP 메서드 | API 엔드포인트 |
|------------------|-------------|----------------|
| `ConnectAsync()` | POST | `/api/v1/db/connect` |
| `BeginTransaction()` | POST | `/api/v1/db/transaction/begin` |
| `CommitTransaction()` | POST | `/api/v1/db/transaction/commit` |
| `RollbackTransaction()` | POST | `/api/v1/db/transaction/rollback` |
| `ExecuteDataTableAsync(...)` | POST | `/api/v1/db/query/table` |
| `ExecuteDataSetAsync(...)` | POST | `/api/v1/db/query/table` |
| `ExecuteNonQueryAsync(...)` | POST | `/api/v1/db/query/nonquery` |
| `ExecuteScalarValueAsync(...)` | POST | `/api/v1/db/query/scalar` |
| `ExecuteProcedureAsync(...)` | POST | `/api/v1/db/procedure` |

> **참고**: `ExecuteDataSetAsync`는 서버에서 단일 테이블을 반환하더라도 클라이언트에서 `DataSet` 구조로 래핑하여 제공합니다.

### HttpFileTransferClient 매핑

| 클라이언트 메서드 | HTTP 메서드 | API 엔드포인트 |
|------------------|-------------|----------------|
| `GetHomeDirectoryAsync()` | GET | `/api/v1/files/directory` |
| `SetHomeDirectoryAsync(...)` | POST | `/api/v1/files/directory/config` |
| `GetFileListAsync(...)` | GET | `/api/v1/files/list?path=...` |
| `GetSubDirectoryListAsync(...)` | GET | `/api/v1/files/subdirectories?path=...` |
| `CreateDirectoryAsync(...)` | POST | `/api/v1/files/directory/create?path=...` |
| `DeleteDirectoryAsync(...)` | DELETE | `/api/v1/files/directory?path=...` |
| `UploadFileAsync(...)` | POST | `/api/v1/files/upload` (multipart/form-data) |
| `DownloadFileAsync(...)` | GET | `/api/v1/files/download?serverPath=...` |
| `ExistFileAsync(...)` | GET | `/api/v1/files/exists?path=...` |
| `DeleteFileAsync(...)` | DELETE | `/api/v1/files?path=...` |
| `GetFileSizeAsync(...)` | GET | `/api/v1/files/size?path=...` |

---

## ?? 테스트 예시

```csharp
[Fact]
public async Task Should_Connect_To_Database()
{
    // Arrange
    var client = new HttpDBAccessClient("https://localhost:64229");

    // Act
    var connected = await client.ConnectAsync();

    // Assert
    Assert.True(connected);
}

[Fact]
public async Task Should_Upload_Log_File()
{
    // Arrange
    var client = new HttpLogUploadClient("https://localhost:64229");
    var testLogFile = "test.log";
    File.WriteAllText(testLogFile, "Test log content");

    // Act
    var success = await client.UploadLogFileAsync(testLogFile);

    // Assert
    Assert.True(success);

    // Cleanup
    File.Delete(testLogFile);
}

[Fact]
public async Task Should_Execute_Query_And_Return_DataTable()
{
    // Arrange
    var client = new HttpDBAccessClient("https://localhost:64229");
    await client.ConnectAsync();

    // Act
    var dt = await client.ExecuteDataTableAsync("SELECT * FROM Users");

    // Assert
    Assert.NotNull(dt);
    Assert.True(dt.Rows.Count > 0);
}

[Fact]
public async Task Should_Upload_File_Successfully()
{
    // Arrange
    var client = new HttpFileTransferClient("https://localhost:64229");
    var data = System.Text.Encoding.UTF8.GetBytes("test content");

    // Act
    var success = await client.UploadFileAsync("test.txt", data);

    // Assert
    Assert.True(success);
}
```

---

## ?? 보안 고려사항

### 1. HTTPS 사용

```csharp
// ? 올바른 방법
var client = new HttpDBAccessClient("https://localhost:64229");
var logClient = new HttpLogUploadClient("https://localhost:64229");

// ? 프로덕션에서 HTTP 사용 금지
var client = new HttpDBAccessClient("http://localhost:64228");
```

### 2. 인증서 검증

```csharp
// 개발 환경에서만 인증서 검증 무시
#if DEBUG
var handler = new HttpClientHandler
{
    ServerCertificateCustomValidationCallback = 
        HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
};
var httpClient = new HttpClient(handler);
var client = new HttpDBAccessClient(httpClient, "https://localhost:64229");
#endif
```

### 3. 타임아웃 설정

```csharp
var httpClient = new HttpClient
{
    Timeout = TimeSpan.FromSeconds(30) // 적절한 타임아웃 설정
};
var client = new HttpDBAccessClient(httpClient, "https://server");
```

---

## ?? 에러 처리

```csharp
public async Task<DataTable> GetUsersWithErrorHandling()
{
    try
    {
        var dt = await _dbClient.ExecuteDataTableAsync("SELECT * FROM Users");
        return dt;
    }
    catch (HttpRequestException ex)
    {
        // HTTP 통신 에러
        LogError($"HTTP Error: {ex.Message}");
        
        // 로그 업로드
        await _logClient.UploadCurrentLogImmediatelyAsync();
        throw;
    }
    catch (InvalidOperationException ex)
    {
        // 원격 실행 에러
        LogError($"Remote Execution Error: {ex.Message}");
        throw;
    }
    catch (Exception ex)
    {
        // 기타 에러
        LogError($"Unexpected Error: {ex.Message}");
        throw;
    }
}
```

---

## ?? 결론

### ? 문제 해결

**Before:**
```csharp
// 추상 메서드만 있고 구현 없음
protected abstract Task<T> RemoteExecuteAsync<T>(...); // ?
```

**After:**
```csharp
// HTTP 구현 완료!
protected override async Task<T> RemoteExecuteAsync<T>(...)
{
    var response = await _httpClient.PostAsJsonAsync(endpoint, data);
    return await response.Content.ReadFromJsonAsync<T>();
}
```

### ?? 체크리스트

- [x] `HttpDBAccessClient` 생성
- [x] `HttpFileTransferClient` 생성
- [x] `HttpLogUploadClient` 생성 ? NEW!
- [x] `ILogUploadService` 인터페이스 생성 ? NEW!
- [x] 서버 URL 구성 가능
- [x] 모든 API 엔드포인트 매핑
- [x] DataTable/DataSet 변환
- [x] 파일 업로드/다운로드 (multipart/form-data)
- [x] 로그 업로드 (자동/수동) ? NEW!
- [x] 에러 처리
- [x] IDisposable 구현

### ?? 이제 할 수 있는 것

```csharp
// 1. DB 접근
var dbClient = new HttpDBAccessClient("https://localhost:64229");
var dt = await dbClient.ExecuteDataTableAsync("SELECT * FROM Users");

// 2. 파일 전송
var fileClient = new HttpFileTransferClient("https://localhost:64229");
await fileClient.UploadFileAsync("file.txt", data);

// 3. 로그 업로드 ? NEW!
var logClient = new HttpLogUploadClient("https://localhost:64229");
await logClient.UploadAllPendingLogsAsync();
logClient.EnableAutoUpload(true); // 매일 오전 2시 자동 업로드

// 4. WinForms에서 사용
var client = new HttpDBAccessClient(_serverUrl);
dataGridView1.DataSource = await client.ExecuteDataTableAsync(query);
```

**완벽하게 동작합니다!** ?

---

**다음 단계:** nU3.Shell에서 `HttpDBAccessClient`, `HttpFileTransferClient`, `HttpLogUploadClient`를 사용하여 서버와 통신하세요!
