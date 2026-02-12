# 백엔드 유지 시 WinForms 연동 가이드 (Migration Plan)

> **프로젝트**: nU3.Framework (WinForms + DevExpress)
> **백엔드**: nU3.Server.Host (ASP.NET Core 8.0 REST API)
> **타겟**: X-Reference/cmcnu_2.0 (투비 x-platform 앱) → WinForms으로 마이그레이션

---

## 목차

1. [개요](#개요)
2. [현재 아키텍처 분석](#현재-아키텍처-분석)
3. [백엔드 서버 구조](#백엔드-서버-구조)
4. [WinForms 클라이언트 연동 구조](#winforms-클라이언트-연동-구조)
5. [데이터 접근 패턴](#데이터-접근-패턴)
6. [연동 전략 및 구현 계획](#연동-전략-및-구현-계획)
7. [인증 및 보안](#인증-및-보안)
8. [성능 최적화](#성능-최적화)
9. [배포 및 운영](#배포-및-운영)
10. [추가 리소스](#추가-리소스)

---

## 개요

### 개념 정의

- **백엔드 서버**: `nU3.Server.Host` (ASP.NET Core 8.0 REST API 서버)
- **WinForms 클라이언트**: `nU3.Shell` (DevExpress WinForms 애플리케이션)
- **공통 인터페이스**: `IDBAccessService` (데이터 접근 추상화)
- **통신 프로토콜**: HTTP/REST + JSON (POST 방식)

### 핵심 원칙

1. **IDBAccessService 추상화**: Repository는 `IDBAccessService`를 통해 데이터 접근을 추상화
2. **HTTP 기반 원격 통신**: WinForms → 백엔드 REST API 호출
3. **서버 DB 직접 접근**: 백엔드는 서버에 연결된 데이터베이스(Oracle/MariaDB/SQLite) 직접 접근
4. **Data Table 전달**: 클라이언트는 DataTable로 결과를 받고 처리

---

## 현재 아키텍처 분석

### 레이어 구조

```
┌─────────────────────────────────────────────────────────────┐
│                     WinForms Presentation Layer              │
│  (nU3.Shell, DevExpress UI 컨트롤, Modules)                 │
└─────────────────────────────────────────────────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────────┐
│                       Business Logic Layer                  │
│  (nU3.Core, BizLogic, EventAggregator)                      │
└─────────────────────────────────────────────────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────────┐
│                       Data Access Layer                      │
│  (nU3.Data, Repository implementations)                     │
│  → IDBAccessService interface                              │
└─────────────────────────────────────────────────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────────┐
│                   Connectivity / Client Layer               │
│  (nU3.Connectivity, HttpDBAccessClient)                     │
│  → HttpClient (REST API calls)                              │
└─────────────────────────────────────────────────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────────┐
│              Backend Server (REST API Host)                  │
│  (nU3.Server.Host, nU3.Server.Connectivity)                │
│  → DBAccessController, ServerDBAccessService                 │
│  → Direct DB connection (Oracle/MariaDB/SQLite)            │
└─────────────────────────────────────────────────────────────┘
```

### 주요 프로젝트

| 프로젝트 | 역할 | 언어/프레임워크 |
|---------|------|----------------|
| `nU3.Shell` | WinForms 앱 진입점, DI 설정 | C# 8.0, .NET 8.0 |
| `nU3.Bootstrapper` | 앱 부트스트래핑, 모듈 동기화 | C# 8.0, .NET 8.0 |
| `nU3.Core` | 비즈니스 로직, 인터페이스 정의 | C# 8.0 |
| `nU3.Data` | Repository 구현 | C# 8.0 |
| `nU3.Connectivity` | HTTP 클라이언트, 원격 DB 접근 추상 | C# 8.0 |
| `nU3.Models` | DTO, 엔티티 모델 | C# 8.0 |
| `nU3.Server.Host` | REST API 서버 (ASP.NET Core 8.0) | C# 8.0, .NET 8.0 |
| `nU3.Server.Connectivity` | 서버 DB 접근 서비스 | C# 8.0, .NET 8.0 |

---

## 백엔드 서버 구조

### 서버 프로젝트 구조

```
nU3.Server.Host/
├── Program.cs                          # 서버 시작점 (DI 등록)
├── Controllers/
│   └── Connectivity/
│       ├── DBAccessController.cs       # DB API 엔드포인트
│       └── FileTransferController.cs   # 파일 전송 API
└── Services/
    └── ServerDBAccessService.cs       # 서버 DB 접근 서비스
```

### 주요 컨트롤러

#### 1. DBAccessController (`/api/v1/db/*`)

| 엔드포인트 | 메서드 | 역할 | 요청 DTO |
|-----------|--------|------|---------|
| `/connect` | POST | DB 연결 테스트 | 없음 |
| `/transaction/begin` | POST | 트랜잭션 시작 | 없음 |
| `/transaction/commit` | POST | 트랜잭션 커밋 | 없음 |
| `/transaction/rollback` | POST | 트랜잭션 롤백 | 없음 |
| `/query/table` | POST | 쿼리 실행 (DataTable 반환) | `QueryRequestDto` |
| `/query/nonquery` | POST | INSERT/UPDATE/DELETE 실행 | `QueryRequestDto` |
| `/query/scalar` | POST | 단일 값 반환 | `QueryRequestDto` |
| `/procedure` | POST | 저장 프로시저 실행 | `ProcedureRequestDto` |

#### 2. FileTransferController (`/api/v1/files/*`)

| 엔드포인트 | 메서드 | 역할 |
|-----------|--------|------|
| `/directory` | GET | 파일 시스템 루트 디렉토리 확인 |
| `/upload` | POST | 파일 업로드 (multipart/form-data) |
| `/download` | GET | 파일 다운로드 (스트리밍) |
| `/list`, `/create`, `/rename`, `/exists`, `/delete` | POST/GET | 파일 시스템 관리 |

#### 3. LogController (`/api/log/*`)

| 엔드포인트 | 메서드 | 역할 |
|-----------|--------|------|
| `/upload` | POST | 로그 업로드 (압축 여부 처리) |
| `/upload-audit` | POST | 감사 로그 업로드 |
| `/info` | GET | 로그 폴더 통계 |

### ServerDBAccessService 구현

**주요 기능:**

- **다중 DB 공급자 지원**: Oracle, MariaDB(MySQL), SQLite
  - `DbConnectionFactories.CreateOracleFactory()`
  - `DbConnectionFactories.CreateMariaDbFactory()`
  - `DbConnectionFactories.CreateSqliteFactory()`

- **ADO.NET 패턴 사용**:
  - `DbConnection`, `DbCommand`, `DbTransaction`, `DbParameter`
  - 파라미터 바인딩: `p.ParameterName = key; p.Value = value;`

- **JSON 파라미터 변환**:
  - 클라이언트에서 JSON으로 전송된 파라미터를 ADO.NET 파라미터로 변환
  - `JsonElement` → `.NET 타입` 변환 로직

- **트랜잭션 관리**:
  - `_transaction` 필드로 현재 트랜잭션 추적
  - Begin/Commit/Rollback 메서드 제공

- **동기/비동기 지원**:
  - 모든 메서드 동기 버전 및 비동기 버전 제공

### Program.cs DI 설정

```csharp
// 1. 서비스 등록
services.AddSingleton<ServerFileTransferService>();
services.AddScoped<ServerDBAccessService>(sp =>
{
    var provider = configuration["ServerSettings:Database:Provider"];
    var connStr = configuration["ServerSettings:Database:ConnectionString"];

    var dbService = provider switch
    {
        "Oracle" => new ServerDBAccessService(connStr, DbConnectionFactories.CreateOracleFactory(connStr)),
        "MariaDB" => new ServerDBAccessService(connStr, DbConnectionFactories.CreateMariaDbFactory(connStr)),
        "SQLite" => new ServerDBAccessService(connStr, DbConnectionFactories.CreateSqliteFactory(connStr)),
        _ => new ServerDBAccessService(connStr, DbConnectionFactories.CreateSqliteFactory(connStr))
    };

    return dbService;
});

// 2. DI 인터페이스 매핑
services.AddScoped<IDBAccessService>(sp => sp.GetRequiredService<ServerDBAccessService>());

// 3. 컨트롤러 매핑
services.AddControllers();

// 4. 미들웨어 파이프라인
app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
```

---

## WinForms 클라이언트 연동 구조

### IDBAccessService 인터페이스

```csharp
public interface IDBAccessService
{
    // DB 연결
    bool Connect();
    Task<bool> ConnectAsync();

    // 트랜잭션
    void BeginTransaction();
    void CommitTransaction();
    void RollbackTransaction();

    // 쿼리 실행 (동기)
    DataTable ExecuteDataTable(string commandText, Dictionary<string, object>? parameters = null);
    DataSet ExecuteDataSet(string commandText, Dictionary<string, object>? parameters = null);
    bool ExecuteNonQuery(string commandText, Dictionary<string, object>? parameters = null);
    object ExecuteScalarValue(string commandText, Dictionary<string, object>? parameters = null);
    bool ExecuteProcedure(string spName, Dictionary<string, object> inputParams, Dictionary<string, object> outputParams);

    // 쿼리 실행 (비동기)
    Task<DataTable> ExecuteDataTableAsync(string commandText, Dictionary<string, object>? parameters = null);
    Task<DataSet> ExecuteDataSetAsync(string commandText, Dictionary<string, object>? parameters = null);
    Task<bool> ExecuteNonQueryAsync(string commandText, Dictionary<string, object>? parameters = null);
    Task<object> ExecuteScalarValueAsync(string commandText, Dictionary<string, object>? parameters = null);
    Task<bool> ExecuteProcedureAsync(string spName, Dictionary<string, object> inputParams, Dictionary<string, object> outputParams);
}
```

### HttpDBAccessClient 구현

**작동 방식:**

1. **요청 생성**: `RemoteExecuteAsync<T>` 메서드가 내부 REST API 호출을 생성
2. **엔드포인트 매핑**: `MapMethodToEndpoint()`에서 메서드 → URL 매핑
3. **요청 데이터 직렬화**: `CreateRequestData()`에서 요청 DTO를 JSON으로 변환
4. **HTTP POST**: `HttpClient.PostAsJsonAsync()` 호출
5. **응답 처리**: JSON 응답을 `.NET 타입`으로 역직렬화
6. **결과 반환**: DataTable/DataSet/객체로 변환하여 클라이언트로 전달

**엔드포인트 매핑:**

```csharp
private string MapMethodToEndpoint(string method)
{
    return method switch
    {
        nameof(Connect) or nameof(ConnectAsync) => "/api/v1/db/connect",
        nameof(BeginTransaction) => "/api/v1/db/transaction/begin",
        nameof(CommitTransaction) => "/api/v1/db/transaction/commit",
        nameof(RollbackTransaction) => "/api/v1/db/transaction/rollback",
        nameof(ExecuteDataTable) or nameof(ExecuteDataTableAsync) => "/api/v1/db/query/table",
        nameof(ExecuteDataSet) or nameof(ExecuteDataSetAsync) => "/api/v1/db/query/table",
        nameof(ExecuteNonQuery) or nameof(ExecuteNonQueryAsync) => "/api/v1/db/query/nonquery",
        nameof(ExecuteScalarValue) or nameof(ExecuteScalarValueAsync) => "/api/v1/db/query/scalar",
        nameof(ExecuteProcedure) or nameof(ExecuteProcedureAsync) => "/api/v1/db/procedure",
        _ => throw new NotSupportedException($"지원하지 않는 메서드: {method}")
    };
}
```

**요청 데이터 변환:**

```csharp
private object CreateRequestData(string method, object[]? args)
{
    return method switch
    {
        // SQL 쿼리 실행
        nameof(ExecuteDataTable) or nameof(ExecuteDataSetAsync) or
        nameof(ExecuteNonQuery) or nameof(ExecuteNonQueryAsync) or
        nameof(ExecuteScalarValue) or nameof(ExecuteScalarValueAsync) =>
            new
            {
                CommandText = args[0]?.ToString() ?? string.Empty,
                Parameters = args.Length > 1 ? args[1] as Dictionary<string, object> : null
            },

        // 저장 프로시저 호출
        nameof(ExecuteProcedure) or nameof(ExecuteProcedureAsync) =>
            new
            {
                SpName = args[0]?.ToString() ?? string.Empty,
                InputParams = args.Length > 1 ? args[1] as Dictionary<string, object> : new Dictionary<string, object>(),
                OutputParams = args.Length > 2 ? args[2] as Dictionary<string, object> : new Dictionary<string, object>()
            },

        _ => new { }
    };
}
```

**DataTable 변환 로직:**

```csharp
private DataTable ConvertToDataTable(List<Dictionary<string, object>>? data)
{
    var dt = new DataTable();

    if (data == null || data.Count == 0)
        return dt;

    // 1열부터 컬럼 추가 (키 순서 유지)
    foreach (var key in data[0].Keys)
    {
        dt.Columns.Add(key);
    }

    // 각 행을 DataRow로 변환
    foreach (var row in data)
    {
        var dataRow = dt.NewRow();
        foreach (var kvp in row)
        {
            if (kvp.Value is JsonElement element)
            {
                // JsonElement → .NET 타입 변환
                switch (element.ValueKind)
                {
                    case JsonValueKind.String: dataRow[kvp.Key] = element.GetString(); break;
                    case JsonValueKind.Number:
                        if (element.TryGetInt64(out long l)) dataRow[kvp.Key] = l;
                        else if (element.TryGetDouble(out double d)) dataRow[kvp.Key] = d;
                        else dataRow[kvp.Key] = element.ToString();
                        break;
                    case JsonValueKind.True: dataRow[kvp.Key] = true; break;
                    case JsonValueKind.False: dataRow[kvp.Key] = false; break;
                    case JsonValueKind.Null: dataRow[kvp.Key] = DBNull.Value; break;
                    default: dataRow[kvp.Key] = element.ToString(); break;
                }
            }
            else
            {
                dataRow[kvp.Key] = kvp.Value ?? DBNull.Value;
            }
        }
        dt.Rows.Add(dataRow);
    }

    return dt;
}
```

### WinForms DI 설정 (Program.cs)

```csharp
// 1. baseUrl 설정
string baseUrl = configuration["ServerConnection:BaseUrl"] ?? "http://localhost:5000";

// 2. HttpClient 싱글톤 등록 (재사용)
services.AddSingleton(sp => new System.Net.Http.HttpClient
{
    BaseAddress = new Uri(baseUrl),
    Timeout = TimeSpan.FromMinutes(10)
});

// 3. HTTP 클라이언트 서비스 등록
services.AddScoped<IDBAccessService>(sp =>
    new HttpDBAccessClient(
        sp.GetRequiredService<System.Net.Http.HttpClient>(),
        baseUrl
    ));

services.AddScoped<IFileTransferService>(sp =>
    new HttpFileTransferClient(
        sp.GetRequiredService<System.Net.Http.HttpClient>(),
        baseUrl
    ));

services.AddScoped<ILogUploadService>(sp =>
    new HttpLogUploadClient(
        sp.GetRequiredService<System.Net.Http.HttpClient>(),
        baseUrl
    ));
```

---

## 데이터 접근 패턴

### Repository 패턴

**구조:**

```
Repository (SQLiteModuleRepository 등)
  → IDBAccessService (interface)
  → HttpDBAccessClient (remote) 또는 ServerDBAccessService (local)
```

**예시 (SQLiteModuleRepository.cs):**

```csharp
public class SQLiteModuleRepository : IModuleRepository
{
    private readonly IDBAccessService _db;

    public SQLiteModuleRepository(IDBAccessService db)
    {
        _db = db;
    }

    public List<ModuleInfoDto> GetAllModules()
    {
        // DataTable로 쿼리 결과 받기
        var dt = _db.ExecuteDataTable(
            "SELECT ModuleId, ModuleName, Version, Description FROM Modules ORDER BY OrderIndex",
            null
        );

        // DataTable → DTO 리스트 변환
        var modules = new List<ModuleInfoDto>();
        foreach (DataRow row in dt.Rows)
        {
            modules.Add(new ModuleInfoDto
            {
                ModuleId = row["ModuleId"].ToString(),
                ModuleName = row["ModuleName"].ToString(),
                Version = row["Version"].ToString(),
                Description = row["Description"]?.ToString()
            });
        }
        return modules;
    }

    public bool UpdateModuleVersion(string moduleId, string newVersion)
    {
        // 비동기 쿼리 실행 (async/await)
        var result = _db.ExecuteNonQuery(
            "UPDATE Modules SET Version = @Version WHERE ModuleId = @ModuleId",
            new Dictionary<string, object>
            {
                { "Version", newVersion },
                { "ModuleId", moduleId }
            }
        );
        return result;
    }
}
```

### 트랜잭션 사용

**Repository 트랜잭션 예시:**

```csharp
public bool UpdateModuleWithDependencies(string moduleId, ModuleUpdateDto update)
{
    try
    {
        // 1. 트랜잭션 시작
        _db.BeginTransaction();

        // 2. 메인 모듈 업데이트
        var result1 = _db.ExecuteNonQuery(
            "UPDATE Modules SET Version = @Version, LastUpdated = @LastUpdated WHERE ModuleId = @ModuleId",
            new Dictionary<string, object>
            {
                { "Version", update.NewVersion },
                { "LastUpdated", DateTime.Now },
                { "ModuleId", moduleId }
            }
        );

        if (!result1)
        {
            // 롤백
            _db.RollbackTransaction();
            return false;
        }

        // 3. 의존성 모듈 버전 업데이트
        var result2 = _db.ExecuteNonQuery(
            "UPDATE Modules SET Version = @Version WHERE ModuleId IN (SELECT ModuleId FROM ModuleDependencies WHERE ParentModuleId = @ParentModuleId)",
            new Dictionary<string, object>
            {
                { "Version", update.NewVersion },
                { "ParentModuleId", moduleId }
            }
        );

        if (!result2)
        {
            // 롤백
            _db.RollbackTransaction();
            return false;
        }

        // 4. 커밋
        _db.CommitTransaction();
        return true;
    }
    catch (Exception ex)
    {
        // 예외 발생 시 롤백
        _db.RollbackTransaction();
        LogManager.Error($"모듈 업데이트 실패: {ex.Message}", "SQLiteModuleRepository", ex);
        throw;
    }
}
```

---

## 연동 전략 및 구현 계획

### 전략 1: X-Reference 백엔드 유지 (권장)

**개요:** X-Reference의 투비 x-platform 백엔드 유지, WinForms 클라이언트와 연동

**장점:**
- 기존 백엔드 유지보수 비용 최소화
- 투비 x-platform 앱과 동일한 DB 데이터 사용
- REST API 형태의 통신이 기존 아키텍처와 호환

**단점:**
- 클라이언트별 DB 접근 권한 관리 필요
- 네트워크 통신에 따른 성능 오버헤드

**구현 단계:**

#### 단계 1: X-Reference 백엔드 REST API 노출

**작업 내용:**
- X-Reference 백엔드(Java/투비)에서 REST API 엔드포인트로 DB 접근 기능 노출
- 현재 WinForms 서버(`nU3.Server.Host`)와 동일한 API 구조로 구현

**기술 요소:**
- Java Spring Boot 또는 투비 x-platform의 Web 서버 활용
- JPA/DAO → REST Controller 변환
- JSON/DTO를 통한 요청/응답 처리

**구현 예시 (Java Spring Boot):**

```java
@RestController
@RequestMapping("/api/v1/db")
public class XReferenceDBController {

    @Autowired
    private JdbcTemplate jdbcTemplate;

    @PostMapping("/query/table")
    public ResponseEntity<?> executeDataTable(@RequestBody QueryRequestDto request) {
        try {
            List<Map<String, Object>> result = jdbcTemplate.queryForList(request.getCommandText());

            // DataTable → List<Map<String, Object>> 변환
            Map<String, Object> response = new HashMap<>();
            response.put("data", result);
            response.put("success", true);

            return ResponseEntity.ok(response);
        } catch (Exception ex) {
            return ResponseEntity.badRequest().body(Map.of("message", ex.getMessage()));
        }
    }

    @PostMapping("/query/nonquery")
    public ResponseEntity<?> executeNonQuery(@RequestBody QueryRequestDto request) {
        try {
            int affectedRows = jdbcTemplate.update(request.getCommandText());

            Map<String, Object> response = new HashMap<>();
            response.put("affectedRows", affectedRows);
            response.put("success", true);

            return ResponseEntity.ok(response);
        } catch (Exception ex) {
            return ResponseEntity.badRequest().body(Map.of("message", ex.getMessage()));
        }
    }

    // ExecuteScalar, ExecuteProcedure 등 구현...
}
```

**DTO 클래스:**

```java
public class QueryRequestDto {
    private String commandText;
    private Map<String, Object> parameters;

    // Getter, Setter
}
```

#### 단계 2: WinForms HttpDBAccessClient 수정

**작업 내용:**
- `HttpDBAccessClient` 수정하여 X-Reference 백엔드 URL 사용
- baseUrl 설정을 appsettings.json에서 관리

**appsettings.json 수정:**

```json
{
  "ServerConnection": {
    "BaseUrl": "http://localhost:8080/api",  // X-Reference 백엔드 URL
    "Timeout": "600"
  }
}
```

**수정 코드:**

```csharp
public class HttpDBAccessClient : DBAccessClientBase
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;
    private readonly JsonSerializerOptions _jsonOptions;

    public HttpDBAccessClient(string baseUrl)
    {
        _baseUrl = baseUrl.TrimEnd('/');
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(_baseUrl),
            Timeout = TimeSpan.FromMinutes(5)
        };
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    public HttpDBAccessClient(HttpClient httpClient, string baseUrl)
    {
        _httpClient = httpClient;
        _baseUrl = baseUrl.TrimEnd('/');
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    private string MapMethodToEndpoint(string method)
    {
        return method switch
        {
            nameof(Connect) or nameof(ConnectAsync) => "/connect",
            // ... 나머지 엔드포인트 매핑 (baseUrl 포함)
            _ => throw new NotSupportedException($"지원하지 않는 메서드: {method}")
        };
    }
}
```

#### 단계 3: Repository 수정 (데이터 소스 변경)

**작업 내용:**
- Repository가 직접 데이터 소스(로컬 SQLite vs 원격 X-Reference)를 인식하지 않도록 추상화 유지
- `IDBAccessService` 구현체만 교체하면 됨

**기존 Repository (변경 불필요):**

```csharp
public class SQLiteModuleRepository : IModuleRepository
{
    private readonly IDBAccessService _db;

    public SQLiteModuleRepository(IDBAccessService db)
    {
        _db = db;
    }

    public List<ModuleInfoDto> GetAllModules()
    {
        var dt = _db.ExecuteDataTable(
            "SELECT ModuleId, ModuleName, Version FROM Modules ORDER BY OrderIndex",
            null
        );

        // DataTable → DTO 변환 (기존과 동일)
        return ConvertToModules(dt);
    }
}
```

**데이터 소스 변경 방법:**

```csharp
// nU3.Shell/Program.cs

// X-Reference 백엔드 사용 시
services.AddScoped<IDBAccessService>(sp =>
    new XReferenceDBAccessClient(
        sp.GetRequiredService<System.Net.Http.HttpClient>(),
        "http://localhost:8080/api"
    ));

// 로컬 서버 사용 시
services.AddScoped<IDBAccessService>(sp =>
    new HttpDBAccessClient(
        sp.GetRequiredService<System.Net.Http.HttpClient>(),
        "http://localhost:5000"
    ));
```

#### 단계 4: 동기화 메커니즘 구현

**작업 내용:**
- WinForms에서 X-Reference 백엔드와 데이터 동기화
- 부트스트래핑 시 모듈 업데이트 체크

**Bootstrapper 동기화 로직:**

```csharp
// nU3.Bootstrapper/Program.cs

public class ModuleSyncService
{
    private readonly IDBAccessService _db;
    private readonly HttpClient _httpClient;

    public ModuleSyncService(IDBAccessService db, HttpClient httpClient)
    {
        _db = db;
        _httpClient = httpClient;
    }

    public async Task<bool> SyncWithServerAsync(string serverUrl)
    {
        try
        {
            // 1. 서버에서 모듈 목록 조회
            var response = await _httpClient.GetAsync($"{serverUrl}/api/modules/list");
            var modules = await response.Content.ReadFromJsonAsync<List<ModuleDto>>();

            // 2. 로컬 모듈 버전과 비교
            var localModules = await _db.ExecuteDataTableAsync(
                "SELECT ModuleId, Version FROM Modules"
            );

            var needsUpdate = modules.Where(m =>
                !localModules.AsEnumerable().Any(row => row["ModuleId"].ToString() == m.ModuleId)
            ).ToList();

            // 3. 업데이트가 필요한 경우 다운로드
            foreach (var module in needsUpdate)
            {
                var fileContent = await _httpClient.GetByteArrayAsync($"{serverUrl}/api/modules/download/{module.ModuleId}");
                await File.WriteAllBytesAsync($"Modules/{module.ModuleId}.dll", fileContent);
            }

            return true;
        }
        catch (Exception ex)
        {
            LogManager.Error($"모듈 동기화 실패: {ex.Message}", "ModuleSyncService", ex);
            return false;
        }
    }
}
```

---

### 전략 2: nU3 백엔드 유지, X-Reference 기능만 마이그레이션

**개요:** WinForms 클라이언트와 nU3 백엔드 유지, X-Reference의 특정 기능만 WinForms로 마이그레이션

**장점:**
- 기존 WinForms 서버와 통신하므로 최소한의 변경
- X-Reference 기능을 중심으로 마이그레이션 진행 가능

**단점:**
- X-Reference의 다른 기능들이 nU3 백엔드로 이식되지 않을 수 있음
- 투비 x-platform 앱의 DB 스키마와 호환성 유지 필요

**구현 단계:**

#### 단계 1: X-Reference DB 스키마 분석

**작업 내용:**
- X-Reference의 주요 테이블 스키마 분석
- nU3 백엔드에 테이블 생성 SQL 로드

**SQL 스키마 예시:**

```sql
-- X-Reference DB 스키마 예시
CREATE TABLE Patients (
    PatientId VARCHAR(50) PRIMARY KEY,
    Name VARCHAR(100),
    Age INT,
    Gender CHAR(1),
    BloodType CHAR(2),
    RegistrationDate DATETIME
);

CREATE TABLE Prescriptions (
    PrescriptionId VARCHAR(50) PRIMARY KEY,
    PatientId VARCHAR(50),
    MedicineName VARCHAR(100),
    Dosage VARCHAR(50),
    Duration INT,
    StartDate DATETIME,
    FOREIGN KEY (PatientId) REFERENCES Patients(PatientId)
);
```

#### 단계 2: nU3 백엔드에 API 엔드포인트 추가

**작업 내용:**
- X-Reference의 주요 기능(환자 관리, 처방 관리 등)에 대한 API 엔드포인트 추가
- `DBAccessController` 확장 또는 새 컨트롤러 추가

**예시 (XReferenceAPIController.cs):**

```csharp
[ApiController]
[Route("api/v1/xreference")]
public class XReferenceAPIController : ControllerBase
{
    private readonly ServerDBAccessService _dbService;

    public XReferenceAPIController(ServerDBAccessService dbService)
    {
        _dbService = dbService;
    }

    // 환자 조회
    [HttpGet("patients/{patientId}")]
    public async Task<IActionResult> GetPatient(string patientId)
    {
        var dt = await _dbService.ExecuteDataTableAsync(
            "SELECT * FROM Patients WHERE PatientId = @PatientId",
            new Dictionary<string, object> { { "PatientId", patientId } }
        );

        if (dt.Rows.Count == 0)
            return NotFound();

        return Ok(ConvertToPatientDto(dt.Rows[0]));
    }

    // 환자 목록 조회
    [HttpGet("patients")]
    public async Task<IActionResult> GetPatients(string? search)
    {
        var sql = "SELECT * FROM Patients";
        var parameters = new Dictionary<string, object>();

        if (!string.IsNullOrEmpty(search))
        {
            sql += " WHERE Name LIKE @Search OR PatientId LIKE @Search";
            parameters.Add("Search", $"%{search}%");
        }

        var dt = await _dbService.ExecuteDataTableAsync(sql, parameters);
        return Ok(ConvertToPatientList(dt));
    }

    // 처방 추가
    [HttpPost("prescriptions")]
    public async Task<IActionResult> AddPrescription(PrescriptionDto dto)
    {
        try
        {
            var result = await _dbService.ExecuteNonQueryAsync(
                "INSERT INTO Prescriptions (PrescriptionId, PatientId, MedicineName, Dosage, Duration, StartDate) " +
                "VALUES (@PrescriptionId, @PatientId, @MedicineName, @Dosage, @Duration, @StartDate)",
                new Dictionary<string, object>
                {
                    { "PrescriptionId", dto.PrescriptionId },
                    { "PatientId", dto.PatientId },
                    { "MedicineName", dto.MedicineName },
                    { "Dosage", dto.Dosage },
                    { "Duration", dto.Duration },
                    { "StartDate", dto.StartDate }
                }
            );

            return Ok(new { Success = true });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // ... 다른 X-Reference 기능 엔드포인트 구현
}
```

#### 단계 3: WinForms 모듈 개발

**작업 내용:**
- X-Reference 기능별 WinForms 화면 개발
- nU3 모듈 패턴 따르기 (BaseWorkControl 상속)

**예시 (PatientListForm.cs):**

```csharp
[nU3ProgramInfo(typeof(PatientListForm), "환자 관리", "MODULE_PATIENT", "PATIENT")]

public class PatientListForm : BaseWorkControl
{
    private readonly IDBAccessService _db;
    private readonly HttpClient _httpClient;

    public override string ScreenId => "PATIENT_LIST";

    public PatientListForm(IDBAccessService db, HttpClient httpClient)
    {
        _db = db;
        _httpClient = httpClient;

        InitializeControl();
    }

    private void InitializeControl()
    {
        // DevExpress GridControl 설정
        var gridControl = new nU3.Core.UI.Controls.GridViewControl();
        gridControl.Dock = DockStyle.Fill;

        // LoadData 버튼
        var btnLoad = new nU3.Core.UI.Controls.ButtonControl();
        btnLoad.Text = "조회";
        btnLoad.Click += async (s, e) => await LoadPatientDataAsync();

        // 추가 버튼
        var btnAdd = new nU3.Core.UI.Controls.ButtonControl();
        btnAdd.Text = "추가";
        btnAdd.Click += async (s, e) => await AddPatientAsync();

        var panel = new Panel
        {
            Dock = DockStyle.Top,
            Padding = new Padding(5)
        };
        panel.Controls.Add(btnLoad);
        panel.Controls.Add(btnAdd);

        Controls.Add(panel);
        Controls.Add(gridControl);
    }

    private async Task LoadPatientDataAsync()
    {
        try
        {
            var dt = await _db.ExecuteDataTableAsync(
                "SELECT * FROM Patients ORDER BY RegistrationDate DESC"
            );

            // GridControl에 데이터 바인딩
            var gridView = new DevExpress.XtraGrid.Views.Grid.GridView();
            gridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[]
            {
                gridView.Columns.Add("PatientId", "환자 ID"),
                gridView.Columns.Add("Name", "이름"),
                gridView.Columns.Add("Age", "나이"),
                gridView.Columns.Add("Gender", "성별"),
                gridView.Columns.Add("BloodType", "혈액형")
            });

            gridView.DataSource = dt;
        }
        catch (Exception ex)
        {
            LogManager.Error($"환자 데이터 로드 실패: {ex.Message}", "PatientListForm", ex);
            XtraMessageBox.Show("데이터 로드 실패", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async Task AddPatientAsync()
    {
        // 새 환자 추가 폼 표시
        var addForm = new AddPatientForm();
        if (addForm.ShowDialog() == DialogResult.OK)
        {
            await _db.ExecuteNonQueryAsync(
                "INSERT INTO Patients (PatientId, Name, Age, Gender, BloodType, RegistrationDate) " +
                "VALUES (@PatientId, @Name, @Age, @Gender, @BloodType, @RegistrationDate)",
                new Dictionary<string, object>
                {
                    { "PatientId", addForm.PatientId },
                    { "Name", addForm.Name },
                    { "Age", addForm.Age },
                    { "Gender", addForm.Gender },
                    { "BloodType", addForm.BloodType },
                    { "RegistrationDate", DateTime.Now }
                }
            );

            await LoadPatientDataAsync();
        }
    }
}
```

#### 단계 4: 데이터 동기화 메커니즘

**작업 내용:**
- WinForms ↔ X-Reference 백엔드 데이터 동기화
- 모듈 업데이트 체크

**동기화 서비스:**

```csharp
// nU3.Bootstrapper/ModuleSyncService.cs

public class XReferenceSyncService
{
    private readonly HttpClient _httpClient;
    private readonly IDBAccessService _db;

    public XReferenceSyncService(HttpClient httpClient, IDBAccessService db)
    {
        _httpClient = httpClient;
        _db = db;
    }

    // WinForms → X-Reference 백엔드 데이터 동기화
    public async Task<bool> SyncToXReferenceAsync(List<PrescriptionDto> prescriptions)
    {
        try
        {
            foreach (var prescription in prescriptions)
            {
                var response = await _httpClient.PostAsJsonAsync(
                    "http://localhost:8080/api/xreference/prescriptions",
                    prescription
                );

                if (!response.IsSuccessStatusCode)
                {
                    return false;
                }
            }
            return true;
        }
        catch (Exception ex)
        {
            LogManager.Error($"X-Reference 데이터 동기화 실패: {ex.Message}", "XReferenceSyncService", ex);
            return false;
        }
    }

    // X-Reference → WinForms 백엔드 데이터 동기화
    public async Task<bool> SyncFromXReferenceAsync()
    {
        try
        {
            // X-Reference에서 최신 데이터 조회
            var response = await _httpClient.GetAsync("http://localhost:8080/api/xreference/prescriptions/sync");
            var latestPrescriptions = await response.Content.ReadFromJsonAsync<List<PrescriptionDto>>();

            // WinForms DB에 업데이트
            foreach (var prescription in latestPrescriptions)
            {
                await _db.ExecuteNonQueryAsync(
                    "INSERT OR REPLACE INTO Prescriptions " +
                    "(PrescriptionId, PatientId, MedicineName, Dosage, Duration, StartDate) " +
                    "VALUES (@PrescriptionId, @PatientId, @MedicineName, @Dosage, @Duration, @StartDate)",
                    new Dictionary<string, object>
                    {
                        { "PrescriptionId", prescription.PrescriptionId },
                        { "PatientId", prescription.PatientId },
                        { "MedicineName", prescription.MedicineName },
                        { "Dosage", prescription.Dosage },
                        { "Duration", prescription.Duration },
                        { "StartDate", prescription.StartDate }
                    }
                );
            }

            return true;
        }
        catch (Exception ex)
        {
            LogManager.Error($"X-Reference 데이터 동기화 실패: {ex.Message}", "XReferenceSyncService", ex);
            return false;
        }
    }
}
```

---

## 인증 및 보안

### 현재 인증 구조

**WinForms 측 인증:**

```csharp
// nU3.Core/Services/AuthenticationService.cs

public class AuthenticationService : IAuthenticationService
{
    private readonly IUserRepository _userRepository;
    private readonly LocalJwtIssuer _jwtIssuer;

    public AuthenticationService(IUserRepository userRepository, LocalJwtIssuer jwtIssuer)
    {
        _userRepository = userRepository;
        _jwtIssuer = jwtIssuer;
    }

    public async Task<string?> AuthenticateAsync(string username, string password)
    {
        var user = await _userRepository.FindByUsernameAsync(username);

        if (user == null || !VerifyPassword(password, user.PasswordHash))
        {
            return null;
        }

        // JWT 토큰 발급
        var token = _jwtIssuer.GenerateToken(user);

        return token;
    }

    public bool ValidateJwtToken(string token)
    {
        return _jwtIssuer.ValidateToken(token);
    }
}
```

**X-Reference 백엔드 인증:**

- 투비 x-platform 기본 인증 구조 사용 가능
- 또는 Spring Security + JWT 구현

### 인증 흐름

```
1. WinForms 로그인
   ↓
2. AuthenticationService.AuthenticateAsync()
   ↓
3. 사용자 유효성 검사 (Repository)
   ↓
4. JWT 토큰 발급
   ↓
5. 토큰을 UserSession에 저장
   ↓
6. 각 API 호출 시 Authorization 헤더에 토큰 포함
```

### 보안 권장 사항

1. **HTTPS 사용**:
   - Production에서는 반드시 HTTPS 사용
   - SSL 인증서 적용

2. **JWT 토큰 보안**:
   - 비밀 키를 코드 내 하드코딩하지 않음 (appsettings.json 또는 환경 변수 사용)
   - 토큰 만료 시간 설정 (예: 30분)

3. **RBAC (Role-Based Access Control)**:
   - 사용자 역할별 API 접근 권한 설정
   - `[Authorize(Roles = "Admin")]` 어트리뷰트 사용

4. **CORS 설정**:
   - 백엔드 CORS 허용 출처 제한

```csharp
// appsettings.json
{
  "CORS": {
    "AllowedOrigins": [
      "http://localhost:8080",
      "https://localhost:44300"
    ]
  }
}

// Program.cs
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowWinForms",
        builder => builder
            .WithOrigins("http://localhost:8080", "https://localhost:44300")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials());
});
```

5. **API 키 또는 OAuth2**:
   - 추가 보안 계층으로 API 키 사용 가능
   - 또는 OAuth2 + JWT 통합

---

## 성능 최적화

### 네트워크 최적화

1. **HTTP/2 사용**:
   - 서버와 클라이언트 모두 HTTP/2 지원 확인
   - 자동 다중 멀티플렉싱

2. **케이스인덱싱 (Keep-Alive)**:
   - HttpClient 싱글톤으로 재사용
   - 타임아웃 설정 적절히 조정

```csharp
// 재사용 가능한 HttpClient 등록
services.AddSingleton<HttpClient>(sp =>
{
    var httpClient = new HttpClient(new HttpClientHandler
    {
        AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
    });
    httpClient.Timeout = TimeSpan.FromMinutes(5);
    return httpClient;
});
```

3. **이미지/파일 업로드 최적화**:
   - 대용량 파일: Chunked upload (파일 분할 업로드)
   - 압축: 파일 전송 전 압축

```csharp
// 파일 압축 업로드
public async Task UploadCompressedFileAsync(string filePath)
{
    var fileBytes = await File.ReadAllBytesAsync(filePath);
    var compressedBytes = await CompressAsync(fileBytes);

    using var content = new ByteArrayContent(compressedBytes);
    content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
    content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
    {
        FileName = Path.GetFileName(filePath) + ".gz"
    };

    var response = await _httpClient.PostAsync("/api/files/upload", content);
    response.EnsureSuccessStatusCode();
}

private async Task<byte[]> CompressAsync(byte[] data)
{
    using var output = new MemoryStream();
    using (var gzip = new GZipStream(output, CompressionMode.Compress))
    {
        await gzip.WriteAsync(data, 0, data.Length);
    }
    return output.ToArray();
}
```

### DB 접근 최적화

1. **쿼리 최적화**:
   - 인덱스 추가
   - 불필요한 컬럼 조회 제한
   - WHERE 절 최적화

2. **트랜잭션 최소화**:
   - 트랜잭션 범위 최소화
   - 배치 처리 (Batch Processing)

3. **캐싱**:
   - 자주 조회되는 데이터 캐싱
   - Redis 또는 In-Memory 캐시 사용

```csharp
// Redis 캐싱 예시
public class CachedPatientRepository : IPatientRepository
{
    private readonly IPatientRepository _repository;
    private readonly IDistributedCache _cache;

    public CachedPatientRepository(IPatientRepository repository, IDistributedCache cache)
    {
        _repository = repository;
        _cache = cache;
    }

    public async Task<PatientDto> GetPatientAsync(string patientId)
    {
        var cacheKey = $"patient_{patientId}";

        // 캐시에서 조회
        var cachedData = await _cache.GetStringAsync(cacheKey);
        if (!string.IsNullOrEmpty(cachedData))
        {
            return JsonSerializer.Deserialize<PatientDto>(cachedData)!;
        }

        // DB에서 조회
        var patient = await _repository.GetPatientAsync(patientId);

        // 캐시에 저장 (5분)
        if (patient != null)
        {
            await _cache.SetStringAsync(cacheKey,
                JsonSerializer.Serialize(patient),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                });
        }

        return patient;
    }
}
```

### WinForms UI 최적화

1. **비동기 데이터 로드**:
   - 데이터 로드를 비동기로 처리
   - Progress Bar로 진행률 표시

2. **Lazy Loading**:
   - 목록 데이터 부분 로드 (페이징)

3. **Background Worker**:
   - 긴 작업을 백그라운드 스레드에서 처리

```csharp
// 비동기 데이터 로드 + Progress Bar
private async Task LoadPatientDataWithProgressAsync()
{
    var progressBar = new ProgressBarControl();
    var progressForm = new ProgressForm { Controls.Add(progressBar) };
    progressForm.Show();

    try
    {
        progressBar.EditValue = 0;
        var totalCount = await GetPatientCountAsync();
        var processedCount = 0;

        var allPatients = new List<PatientDto>();

        for (int i = 0; i < totalCount; i += 100)
        {
            var patients = await GetPatientsAsync(i, 100);
            allPatients.AddRange(patients);

            processedCount += patients.Count;
            progressBar.EditValue = (processedCount * 100) / totalCount;

            // UI 업데이트 위해 Task.Delay
            await Task.Delay(100);
        }

        // Grid에 데이터 바인딩
        BindToGrid(allPatients);
    }
    finally
    {
        progressForm.Close();
    }
}
```

---

## 배포 및 운영

### 개발 환경 설정

**appsettings.json (WinForms):**

```json
{
  "ServerConnection": {
    "BaseUrl": "http://localhost:8080/api",  // X-Reference 백엔드 URL
    "Timeout": "600"
  },
  "Database": {
    "Provider": "Oracle",  // 또는 "MariaDB", "SQLite"
    "ConnectionString": "Data Source=localhost;User Id=user;Password=pass;"
  }
}
```

**appsettings.json (Server):**

```json
{
  "ServerSettings": {
    "Database": {
      "Provider": "Oracle",  // 또는 "MariaDB", "SQLite"
      "ConnectionString": "Data Source=localhost;User Id=user;Password=pass;"
    },
    "FileTransfer": {
      "Home": "C:/Data/Files"
    }
  }
}
```

### 빌드 및 배포

**WinForms 배포:**

1. **Release 빌드**:
   ```bash
   dotnet build nU3.Shell.csproj --configuration Release
   ```

2. **배포 파일 생성**:
   - `bin/Release/net8.0/` 디렉토리 확인
   - 필요한 DLL, 설정 파일 포함

3. **설정 파일 배포**:
   ```bash
   cp appsettings.json <설치경로>/appsettings.json
   ```

**백엔드 배포:**

1. **Docker 컨테이너로 배포** (권장):

```dockerfile
# nU3.Server.Host/Dockerfile

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet build nU3.Server.Host.csproj -c Release -o /app/build

FROM build AS publish
RUN dotnet publish nU3.Server.Host.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "nU3.Server.Host.dll"]
```

```bash
# Docker 빌드 및 실행
docker build -t nU3-server:latest .
docker run -d -p 8080:8080 --name nU3-server nU3-server:latest
```

2. **서비스로 실행** (Linux/Windows):

```bash
# Linux systemd 서비스
sudo systemctl enable nU3-server.service
sudo systemctl start nU3-server.service
```

3. **환경 변수 설정**:
   ```bash
   export Database_ConnectionString="Data Source=localhost;..."
   export Database_Provider="Oracle"
   ```

### 모니터링 및 로깅

**서버 로깅 설정:**

```csharp
// Program.cs

builder.Services.AddLogging(configure => configure
    .AddConsole()
    .AddDebug()
    .AddEventSourceLogger()
    .AddSerilog(new LoggerConfiguration()
        .ReadFrom.Configuration(builder.Configuration)
        .Enrich.FromLogContext()
        .WriteTo.File("logs/nU3-server-.txt", rollingInterval: RollingInterval.Day)
        .CreateLogger()));

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

// Swagger에서 로그 보기
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "nU3 Server API v1");
});
```

**WinForms 로깅:**

```csharp
// 사용 예시
LogManager.Info("앱 시작", "Program");
LogManager.Error("데이터 로드 실패", "PatientRepository", ex);
```

### 데이터베이스 백업

**백업 스크립트 (Oracle):**

```bash
# Oracle 백업
expdp username/password@database DIRECTORY=backup_dir DUMPFILE=backup_%date%.dmp LOGFILE=backup_%date%.log

# MariaDB 백업
mysqldump -u user -p database > backup_%date%.sql

# SQLite 백업
sqlite3 database.db ".backup backup_%date%.db"
```

**알림 설정:**

```csharp
// 백업 완료 알림
public class DatabaseBackupService
{
    private readonly ILogger<DatabaseBackupService> _logger;

    public async Task BackupAsync()
    {
        try
        {
            // 백업 수행
            BackupDatabase();

            // 이메일 알림
            await SendEmailAsync("Database backup completed successfully");
        }
        catch (Exception ex)
        {
            await SendEmailAsync($"Database backup failed: {ex.Message}");
            throw;
        }
    }

    private async Task SendEmailAsync(string message)
    {
        var smtpClient = new SmtpClient("smtp.example.com");
        var mailMessage = new MailMessage("admin@nU3.com", "admin@nU3.com")
        {
            Subject = "Database Backup Notification",
            Body = message
        };
        await smtpClient.SendMailAsync(mailMessage);
    }
}
```

---

## 추가 리소스

### 관련 문서

1. **DevExpress WinForms Documentation**:
   - https://docs.devexpress.com/WindowsForms/

2. **ASP.NET Core 8.0 Documentation**:
   - https://docs.microsoft.com/aspnet/core/

3. **nU3 Framework Architecture**:
   - `DOC 폴더/DOC_ARCHITECTURE.md`
   - `DOC 폴더/DOC_REST_API.md`

### 샘플 코드

1. **Repository 패턴 예시**:
   - `SRC/nU3.Data/Repositories/` 전체

2. **HTTP 클라이언트 예시**:
   - `SRC/nU3.Connectivity/Implementations/HttpDBAccessClient.cs`
   - `SRC/nU3.Connectivity/Implementations/HttpFileTransferClient.cs`

3. **WinForms 모듈 예시**:
   - `SRC/Modules/EMR/OT/` 전체

### 툴 및 라이브러리

| 툰/라이브러리 | 용도 | 링크 |
|-------------|------|------|
| Postman | API 테스트 | https://www.postman.com/ |
| Swagger UI | API 문서 | https://swagger.io/tools/swagger-ui/ |
| Redis | 캐싱 | https://redis.io/ |
| Grafana | 모니터링 | https://grafana.com/ |
| ELK Stack | 로깅 | https://www.elastic.co/elastic-stack |

---

## 요약

### 추천 전략

**X-Reference 백엔드 유지 + WinForms 연동 (전략 1)이 가장 적합합니다.**

이유:
1. 기존 백엔드 유지보수 비용 최소화
2. X-Reference 앱과 동일한 DB 데이터 사용
3. REST API 형태의 통신이 현재 아키텍처와 완벽하게 호환

### 핵심 구현 포인트

1. **IDBAccessService 추상화 유지** - Repository 수정 불필요
2. **HttpDBAccessClient 수정** - X-Reference 백엔드 URL로 변경
3. **X-Reference 백엔드 REST API 노출** - DB 접근 기능 REST로 노출
4. **데이터 동기화 메커니즘** - WinForms ↔ X-Reference 백엔드 동기화
5. **보안 강화** - JWT 인증, HTTPS, RBAC

### 다음 단계

1. **단기 (1-2주)**:
   - X-Reference 백엔드 REST API 개발
   - HttpDBAccessClient 수정 및 테스트

2. **중기 (1-2달)**:
   - WinForms 모듈별 마이그레이션
   - 데이터 동기화 메커니즘 구현

3. **장기 (3개월 이상)**:
   - 성능 최적화
   - 운영 지원
   - 안정화

---

**문서 버전**: 1.0
**작성일**: 2026-02-12
**작성자**: Sisyphus (OpenCode AI Agent)
