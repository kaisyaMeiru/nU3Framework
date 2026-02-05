# nU3.Connectivity vs nU3.Server.Connectivity 분리 이유

## ?? 아키텍처 분리 목적

두 프로젝트는 **Client-Server 아키텍처**를 위해 명확하게 분리되어 있습니다.

---

## ?? 프로젝트 역할 비교

| 항목 | nU3.Connectivity | nU3.Server.Connectivity |
|------|------------------|------------------------|
| **위치** | 클라이언트 측 | 서버 측 |
| **역할** | 인터페이스 정의 + 클라이언트 구현 | 서버 구현 |
| **의존성** | nU3.Core, nU3.Models | nU3.Core, **nU3.Connectivity** |
| **실행 환경** | WinForms (nU3.Shell) | ASP.NET Core (nU3.Server.Host) |
| **통신 방식** | HTTP 요청 전송 | HTTP 요청 수신 및 처리 |

---

## ?? 상세 분석

### 1?? **nU3.Connectivity** (공유 계약 + 클라이언트)

#### ?? 파일 구조
```
nU3.Connectivity/
├── IDBAccessService.cs              # 인터페이스 (계약)
├── IFileTransferService.cs          # 인터페이스 (계약)
├── Implementations/
│   ├── DBAccessClientBase.cs        # 클라이언트 구현 (추상 기반)
│   └── FileTransferClientBase.cs    # 클라이언트 구현 (추상 기반)
```

#### ?? 목적
1. **인터페이스 정의** (계약)
   ```csharp
   public interface IDBAccessService
   {
       bool Connect();
       Task<bool> ConnectAsync();
       DataTable ExecuteDataTable(string commandText, ...);
       // ...
   }
   ```

2. **클라이언트 기본 구현**
   ```csharp
   public abstract class DBAccessClientBase : IDBAccessService
   {
       // 원격 호출을 위한 추상 메서드
       protected abstract Task<T> RemoteExecuteAsync<T>(string method, object[] args);
       
       // 인터페이스 구현 - 원격 서버 호출
       public async Task<bool> ConnectAsync()
       {
           return await RemoteExecuteAsync<bool>(nameof(Connect), null);
       }
   }
   ```

#### ?? 특징
- ? **계약(Contract) 정의**: 인터페이스로 API 규격 명시
- ? **클라이언트 추상화**: 원격 호출 로직 캡슐화
- ? **공유 가능**: 클라이언트와 서버가 같은 인터페이스 사용

---

### 2?? **nU3.Server.Connectivity** (서버 구현)

#### ?? 파일 구조
```
nU3.Server.Connectivity/
├── Services/
│   ├── ServerDBAccessService.cs         # 서버 DB 구현
│   └── ServerFileTransferService.cs     # 서버 파일 구현
```

#### ?? 목적
**실제 비즈니스 로직 구현** (서버 측)

```csharp
public class ServerDBAccessService : IDisposable
{
    private DbConnection? _connection;
    private DbTransaction? _transaction;
    
    // 실제 DB 연결 및 쿼리 실행
    public async Task<bool> ConnectAsync()
    {
        if (_connection == null)
        {
            _connection = _connectionFactory();
            _connection.ConnectionString = _connectionString;
        }
        
        if (_connection.State != ConnectionState.Open)
        {
            await _connection.OpenAsync();  // ← 실제 DB 연결
        }
        return true;
    }
    
    public async Task<DataTable> ExecuteDataTableAsync(string commandText, ...)
    {
        using var cmd = CreateCommand(commandText, parameters);
        using var reader = await cmd.ExecuteReaderAsync();  // ← 실제 쿼리 실행
        var dt = new DataTable();
        dt.Load(reader);
        return dt;
    }
}
```

#### ?? 특징
- ? **실제 구현**: DB 연결, 쿼리 실행, 트랜잭션 관리
- ? **서버 전용**: ASP.NET Core에서만 사용
- ? **리소스 관리**: IDisposable로 연결/트랜잭션 정리

---

## ?? 통신 흐름

### 전체 아키텍처

```
┌─────────────────────────────────────────────────────────────────┐
│                     Client Side (WinForms)                       │
├─────────────────────────────────────────────────────────────────┤
│                                                                  │
│  nU3.Shell                                                       │
│     ↓                                                            │
│  DBAccessClient (extends DBAccessClientBase)                    │
│     ↓                                                            │
│  [nU3.Connectivity]                                              │
│     IDBAccessService (interface)                                 │
│     DBAccessClientBase (abstract)                                │
│         ↓ HTTP Request                                           │
│         RemoteExecuteAsync("Connect", ...)                       │
│                                                                  │
└─────────────────────────────────────────────────────────────────┘
                             ↓ HTTP/REST
┌─────────────────────────────────────────────────────────────────┐
│                      Server Side (ASP.NET Core)                  │
├─────────────────────────────────────────────────────────────────┤
│                                                                  │
│  nU3.Server.Host                                                 │
│     ↓                                                            │
│  DBAccessController                                              │
│     ↓                                                            │
│  [nU3.Server.Connectivity]                                       │
│     ServerDBAccessService                                        │
│         ↓ Actual DB Call                                         │
│         Oracle Database                                          │
│                                                                  │
└─────────────────────────────────────────────────────────────────┘
```

---

## ?? 코드 비교

### Client Side (nU3.Connectivity)

```csharp
// DBAccessClientBase.cs
public abstract class DBAccessClientBase : IDBAccessService
{
    // 원격 호출 추상 메서드
    protected abstract Task<T> RemoteExecuteAsync<T>(string method, object[] args);
    
    // Connect 구현 - 원격 서버 호출
    public async Task<bool> ConnectAsync()
    {
        return await RemoteExecuteAsync<bool>(nameof(Connect), null);
        // ↑ HTTP POST https://server/api/v1/db/connect 호출
    }
    
    // Query 구현 - 원격 서버 호출
    public async Task<DataTable> ExecuteDataTableAsync(string cmd, Dictionary<string, object>? params)
    {
        return await RemoteExecuteAsync<DataTable>(
            nameof(ExecuteDataTable), 
            new object[] { cmd, params });
        // ↑ HTTP POST https://server/api/v1/db/query/table 호출
    }
}
```

### Server Side (nU3.Server.Connectivity)

```csharp
// ServerDBAccessService.cs
public class ServerDBAccessService : IDisposable
{
    private DbConnection? _connection;
    
    // Connect 구현 - 실제 DB 연결
    public async Task<bool> ConnectAsync()
    {
        if (_connection == null)
        {
            _connection = _connectionFactory();
            _connection.ConnectionString = _connectionString;
        }
        
        if (_connection.State != ConnectionState.Open)
        {
            await _connection.OpenAsync();  // ← 실제 Oracle DB 연결
        }
        return true;
    }
    
    // Query 구현 - 실제 쿼리 실행
    public async Task<DataTable> ExecuteDataTableAsync(string cmd, Dictionary<string, object>? params)
    {
        using var command = CreateCommand(cmd, params);
        using var reader = await command.ExecuteReaderAsync();  // ← 실제 쿼리 실행
        var dt = new DataTable();
        dt.Load(reader);
        return dt;
    }
}
```

---

## ?? 분리의 장점

### 1. **관심사의 분리 (Separation of Concerns)**

```
클라이언트 관심사:
- 원격 서버와 통신
- 응답 처리
- 에러 핸들링

서버 관심사:
- 실제 비즈니스 로직
- DB 연결 관리
- 트랜잭션 관리
- 보안
```

### 2. **재사용성**

```csharp
// nU3.Connectivity는 여러 클라이언트에서 사용 가능
- nU3.Shell (WinForms)
- 향후 nU3.Mobile (Xamarin)
- 향후 nU3.Web (Blazor WASM)

// 모두 같은 인터페이스 사용!
```

### 3. **테스트 용이성**

```csharp
// 클라이언트 테스트 - Mock 서버
public class MockDBAccessClient : DBAccessClientBase
{
    protected override Task<T> RemoteExecuteAsync<T>(string method, object[] args)
    {
        // Mock 응답 반환
        return Task.FromResult(default(T));
    }
}

// 서버 테스트 - Mock DB Connection
var mockConnection = new MockDbConnection();
var service = new ServerDBAccessService("connStr", () => mockConnection);
```

### 4. **독립적인 배포**

```
클라이언트 업데이트:
- nU3.Shell 재배포
- nU3.Connectivity 포함

서버 업데이트:
- nU3.Server.Host 재배포
- nU3.Server.Connectivity 포함

인터페이스 변경 시에만 양쪽 모두 업데이트 필요!
```

### 5. **명확한 계약(Contract)**

```csharp
// IDBAccessService 인터페이스가 계약 역할
// 클라이언트와 서버 모두 이 계약을 준수
// API 버전 관리 가능

public interface IDBAccessService  // ← API Contract
{
    Task<bool> ConnectAsync();     // ← 모두가 구현해야 함
    Task<DataTable> ExecuteDataTableAsync(string cmd, ...);
}
```

---

## ?? 실제 사용 예시

### Client Side (nU3.Shell)

```csharp
// WinForms 애플리케이션
public class MyForm : Form
{
    private readonly DBAccessClient _dbClient;
    
    public MyForm()
    {
        _dbClient = new DBAccessClient("https://localhost:64229");
    }
    
    private async void btnConnect_Click(object sender, EventArgs e)
    {
        var connected = await _dbClient.ConnectAsync();
        // ↑ HTTP 요청 전송 → 서버 호출
        
        if (connected)
        {
            var dt = await _dbClient.ExecuteDataTableAsync("SELECT * FROM Users");
            // ↑ HTTP 요청 전송 → 서버에서 쿼리 실행 → 결과 반환
            
            dataGridView1.DataSource = dt;
        }
    }
}
```

### Server Side (nU3.Server.Host)

```csharp
// ASP.NET Core Controller
[ApiController]
[Route("api/v1/db")]
public class DBAccessController : ControllerBase
{
    private readonly ServerDBAccessService _dbService;
    
    public DBAccessController(ServerDBAccessService dbService)
    {
        _dbService = dbService;  // DI로 주입
    }
    
    [HttpPost("connect")]
    public async Task<IActionResult> Connect()
    {
        var result = await _dbService.ConnectAsync();
        // ↑ 실제 Oracle DB 연결
        return Ok(result);
    }
    
    [HttpPost("query/table")]
    public async Task<IActionResult> ExecuteDataTable([FromBody] QueryDto request)
    {
        var dt = await _dbService.ExecuteDataTableAsync(request.CommandText, request.Parameters);
        // ↑ 실제 Oracle 쿼리 실행
        return Ok(ConvertToJson(dt));
    }
}
```

---

## ?? 의존성 관계

```
nU3.Shell (WinForms)
    ↓ depends on
nU3.Connectivity (Client Interface + Client Implementation)
    ↑ referenced by (인터페이스만)
nU3.Server.Connectivity (Server Implementation)
    ↓ used by
nU3.Server.Host (ASP.NET Core)
```

**중요:** `nU3.Server.Connectivity`는 `nU3.Connectivity`를 참조하여 인터페이스를 알지만, 클라이언트 구현은 사용하지 않음!

---

## ?? 결론

### ? **분리 이유 요약**

1. **아키텍처 분리**: Client-Server 구조
2. **책임 분리**: 클라이언트는 통신, 서버는 비즈니스 로직
3. **재사용성**: 여러 클라이언트에서 동일 인터페이스 사용
4. **테스트 용이성**: 각각 독립적으로 테스트 가능
5. **유지보수성**: 클라이언트와 서버를 독립적으로 수정 가능

### ?? **프로젝트 역할**

| 프로젝트 | 역할 | 포함 내용 |
|---------|------|-----------|
| **nU3.Connectivity** | 계약 + 클라이언트 | 인터페이스, 클라이언트 기반 클래스 |
| **nU3.Server.Connectivity** | 서버 구현 | 실제 DB/파일 처리 로직 |

### ?? **이점**

```
클라이언트 개발자:
→ nU3.Connectivity의 인터페이스만 알면 됨
→ 서버 구현 세부사항 몰라도 됨

서버 개발자:
→ 인터페이스 계약만 지키면 됨
→ 클라이언트 구현 신경 안 써도 됨

결과:
→ 독립적인 개발 가능
→ 병렬 개발 가능
→ 명확한 책임 분리
```

**이것이 전형적인 Clean Architecture / Onion Architecture 패턴입니다!** ??

---

**최종 답변:** 두 프로젝트는 Client-Server 아키텍처를 위해 의도적으로 분리되었으며, 각각 클라이언트 측 추상화와 서버 측 구현을 담당합니다.
