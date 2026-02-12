# WinForms 화면별 DAO 전략 (Single-DLL Strategy)

> **목표**: X-Reference 백엔드 유지, WinForms 단일 DLL = 단일 화면, 최소 변경의 효율적인 DAO 계층 구축

---

## 목차

1. [전체 아키텍처](#전체-아키텍처)
2. [핵심 전략](#핵심-전략)
3. [DAO 계층 구조](#dao-계층-구조)
4. [데이터 매핑 전략](#데이터-매핑-전략)
5. [트랜잭션 관리](#트랜잭션-관리)
6. [구현 예시](#구현-예시)
7. [단계별 구현 계획](#단계별-구현-계획)

---

## 전체 아키텍처

### 시스템 아키텍처

```
┌─────────────────────────────────────────────────────────────┐
│                     WinForms UI Layer                       │
│  ┌─────────────────────────────────────────────────────┐   │
│  │  Modules/<Domain>/<SubDomain>                        │   │
│  │  └── <Module>.csproj (단일 DLL = 단일 화면)           │   │
│  │      ├── MainControl.cs (BaseWorkControl 상속)       │   │
│  │      ├── BizLogic.cs (비즈니스 로직)                 │   │
│  │      └── UI Controls (.Designer.cs)                  │   │
│  └─────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────────┐
│                    Data Access Layer (NEW)                  │
│  ┌─────────────────────────────────────────────────────┐   │
│  │  IDao<T> (Generic DAO Interface)                     │   │
│  │  └── GetAsync<TDto>(), SaveAsync<TDto>(), ...       │   │
│  └─────────────────────────────────────────────────────┘   │
│  ┌─────────────────────────────────────────────────────┐   │
│  │  SqlMapper.cs (Dapper 기반 매핑)                      │   │
│  │  └── Query<T>(), Execute<T>(), ...                    │   │
│  └─────────────────────────────────────────────────────┘   │
│  ┌─────────────────────────────────────────────────────┐   │
│  │  SqlBuilder.cs (SQL 빌더 & 매개변수)                 │   │
│  │  └── BuildInClause(), BuildWhere(), ...              │   │
│  └─────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────────┐
│                 X-Reference 백엔드 (유지)                    │
│  ┌─────────────────────────────────────────────────────┐   │
│  │  nU3.Server.Host (ASP.NET Core REST API)              │   │
│  │  └── /api/v1/db/*                                    │   │
│  └─────────────────────────────────────────────────────┘   │
│  ┌─────────────────────────────────────────────────────┐   │
│  │  ServerDBAccessService (DB 접근 서비스)              │   │
│  │  └── Oracle/MariaDB/SQLite 직접 접근                 │   │
│  └─────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────┘
```

### 계층별 역할

| 계층 | 역할 | 변경 여부 |
|------|------|----------|
| **UI Layer** | 화면 구현 (BaseWorkControl 상속) | 유지 |
| **BizLogic Layer** | 비즈니스 로직 (이벤트 처리, 데이터 변환) | 유지 |
| **Data Access Layer** | DAO 인터페이스, 매핑, 트랜잭션 | **신규** |
| **Backend Layer** | X-Reference 백엔드 REST API | **유지** |

---

## 핵심 전략

### 전략 1: Generic DAO 인터페이스 도입

**목적**: 화면별 동일한 데이터 접근 패턴 적용

```csharp
// IDao<T> - Generic DAO 인터페이스
public interface IDao<TDto>
    where TDto : class
{
    // 조회
    Task<IEnumerable<TDto>> GetAllAsync();
    Task<TDto?> GetByIdAsync(string id);
    Task<IEnumerable<TDto>> GetByConditionAsync(Expression<Func<TDto, bool>> condition);
    Task<PagedResult<TDto>> GetPagedAsync(int page, int pageSize, string? orderBy = null);

    // 저장
    Task<int> SaveAsync(TDto dto);
    Task<int> SaveManyAsync(IEnumerable<TDto> dtos);

    // 삭제
    Task<int> DeleteAsync(string id);
    Task<int> DeleteManyAsync(IEnumerable<string> ids);

    // 실행 (SQL 직접)
    Task<T> ExecuteScalarAsync<T>(string sql, object? parameters = null);
    Task<int> ExecuteNonQueryAsync(string sql, object? parameters = null);
    Task<IEnumerable<T>> QueryAsync<T>(string sql, object? parameters = null);
}
```

**장점:**
- 화면별로 다른 Repository 구현체를 만들 필요 없음
- 인터페이스 명시화로 유지보수 용이
- Dapper와 자연스러운 통합

---

### 전략 2: Dapper 기반 매핑 라이브러리

**목적**: DataTable 매핑 대신 강타입 DTO 매핑

```csharp
// SqlMapper.cs
public static class SqlMapper
{
    private static readonly IDbConnection _connection;

    static SqlMapper()
    {
        // LocalDbService 또는 ServerDBAccessService에서 Connection 가져오기
        // IDBAccessService에 Connection 노출 방식 확장
    }

    public static async Task<IEnumerable<TDto>> QueryAsync<TDto>(string sql, object? parameters = null)
        where TDto : class, new()
    {
        using var command = CreateCommand(sql, parameters);
        using var reader = await command.ExecuteReaderAsync();
        return reader.MapToList<TDto>();
    }

    public static async Task<TDto?> QueryFirstOrDefaultAsync<TDto>(string sql, object? parameters = null)
        where TDto : class, new()
    {
        var result = await QueryAsync<TDto>(sql, parameters);
        return result.FirstOrDefault();
    }

    public static async Task<int> ExecuteNonQueryAsync(string sql, object? parameters = null)
    {
        using var command = CreateCommand(sql, parameters);
        return await command.ExecuteNonQueryAsync();
    }

    public static async Task<T> ExecuteScalarAsync<T>(string sql, object? parameters = null)
    {
        using var command = CreateCommand(sql, parameters);
        var result = await command.ExecuteScalarAsync();
        return result == DBNull.Value ? default : (T)result;
    }

    private static IDbCommand CreateCommand(string sql, object? parameters)
    {
        var command = _connection.CreateCommand();
        command.CommandText = sql;

        if (parameters != null)
        {
            foreach (var prop in parameters.GetType().GetProperties())
            {
                var value = prop.GetValue(parameters);
                command.Parameters.AddWithValue($"@{prop.Name}", value ?? DBNull.Value);
            }
        }

        return command;
    }
}
```

**장점:**
- 코드 간소화 (DataTable → DTO 매핑 로직 제거)
- 타입 안전성 보장
- 성능 향상 (직접 매핑 vs DataTable 로드)

---

### 전략 3: SQL Builder 패턴

**목적**: 동적 쿼리 빌딩 및 매개변수 바인딩

```csharp
// SqlBuilder.cs
public static class SqlBuilder
{
    // IN 절 빌더
    public static string BuildInClause(string columnName, IEnumerable<string> values)
    {
        if (!values.Any()) return "1=0";

        var paramNames = values.Select((v, i) => $"@p{i}")
                              .ToArray();

        return $"{columnName} IN ({string.Join(", ", paramNames)})";
    }

    // WHERE 절 빌더
    public static string BuildWhereClause(Dictionary<string, object> conditions, bool addWhere = true)
    {
        if (!conditions.Any()) return "";

        var conditionsList = conditions.Select(kvp =>
        {
            var op = IsNumeric(kvp.Value) ? "=" : "LIKE";
            return $"{kvp.Key} {op} @{kvp.Key}";
        });

        return addWhere ? "WHERE " + string.Join(" AND ", conditionsList)
                        : string.Join(" AND ", conditionsList);
    }

    // ORDER BY 빌더
    public static string BuildOrderBy(string column, bool ascending = true)
    {
        return $"ORDER BY {column} {(ascending ? "ASC" : "DESC")}";
    }

    // 페이징 빌더
    public static string BuildPaging(int page, int pageSize, string? orderBy = null)
    {
        var offset = (page - 1) * pageSize;
        return "OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
    }
}
```

**사용 예시:**

```csharp
// IN 절 사용
var ids = new[] { "A001", "A002", "A003" };
var sql = $"SELECT * FROM Patients WHERE {SqlBuilder.BuildInClause("PatientId", ids)}";

// 동적 WHERE 절 사용
var conditions = new Dictionary<string, object>
{
    { "PatientId", "P%" },
    { "IsActive", true }
};
var sql = $"SELECT * FROM Patients {SqlBuilder.BuildWhereClause(conditions)}";
```

---

### 전략 4: Repository → DAO 마이그레이션 계획

**기존 Repository 구조 (변경 예정):**

```
nU3.Data/Repositories/
├── SQLiteModuleRepository.cs          (기존)
├── SQLiteComponentRepository.cs       (기존)
├── PatientRepository.cs               (신규 - Generic DAO 사용)
├── OrderRepository.cs                 (신규 - Generic DAO 사용)
└── PrescriptionRepository.cs          (신규 - Generic DAO 사용)
```

**변경 방향:**

1. **기존 Repository 유지 (시스템 설정용)**
   - `SQLiteModuleRepository`: 시스템 설정용 (Module, Component, Program)
   - `SQLiteComponentRepository`: 시스템 설정용
   - `SQLiteProgramRepository`: 시스템 설정용

2. **비즈니스 로직 Repository → DAO로 리팩토링**
   - `PatientRepository` → `PatientDao<TPatientDto>`
   - `OrderRepository` → `OrderDao<TOrderDto>`
   - `PrescriptionRepository` → `PrescriptionDao<TPrescriptionDto>`

---

## DAO 계층 구조

### 패키지 구조

```
nU3.Data/
├── Dao/                              # 신규 DAO 패키지
│   ├── IDao.cs                       # Generic DAO 인터페이스
│   ├── DaoBase.cs                    # 기본 구현
│   ├── SqlMapper.cs                  # Dapper 기반 매핑
│   ├── SqlBuilder.cs                 # SQL 빌더
│   └── Mappers/                      # DTO 매핑 클래스
│       ├── PatientMapper.cs
│       ├── OrderMapper.cs
│       └── PrescriptionMapper.cs
├── Repositories/                     # 기존 Repository (유지)
│   ├── SQLiteModuleRepository.cs
│   ├── SQLiteComponentRepository.cs
│   └── ...
└── Infrastructure/
    ├── UnitOfWork.cs                 # 트랜잭션 관리
    └── DbContext.cs                  # DB 컨텍스트 (선택적)
```

### IDao<T> 인터페이스 상세

```csharp
namespace nU3.Data.Dao
{
    /// <summary>
    /// Generic DAO 인터페이스
    /// </summary>
    /// <typeparam name="TDto">DTO 타입</typeparam>
    public interface IDao<TDto>
        where TDto : class
    {
        // 기본 CRUD
        Task<IEnumerable<TDto>> GetAllAsync();
        Task<TDto?> GetByIdAsync(string id);
        Task<int> SaveAsync(TDto dto);
        Task<int> DeleteAsync(string id);

        // 조회
        Task<IEnumerable<TDto>> GetByConditionAsync(Expression<Func<TDto, bool>> condition);
        Task<PagedResult<TDto>> GetPagedAsync(int page, int pageSize, string? orderBy = null);
        Task<IEnumerable<TDto>> GetByParentIdAsync(string parentId);

        // 다중 작업
        Task<int> SaveManyAsync(IEnumerable<TDto> dtos);
        Task<int> DeleteManyAsync(IEnumerable<string> ids);

        // 직접 SQL 실행
        Task<IEnumerable<TDto>> QueryAsync(string sql, object? parameters = null);
        Task<int> ExecuteNonQueryAsync(string sql, object? parameters = null);
        Task<T> ExecuteScalarAsync<T>(string sql, object? parameters = null);

        // 트랜잭션
        Task<IDaoTransaction> BeginTransactionAsync();
    }

    /// <summary>
    /// DAO 트랜잭션
    /// </summary>
    public interface IDaoTransaction : IDisposable
    {
        Task<int> CommitAsync();
        Task<int> RollbackAsync();
    }

    /// <summary>
    /// 페이징 결과
    /// </summary>
    public class PagedResult<TDto>
    {
        public IEnumerable<TDto> Items { get; set; } = Enumerable.Empty<TDto>();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    }
}
```

---

## 데이터 매핑 전략

### 매핑 라이브러리 구현

```csharp
// SqlMapper.cs (확장)
public static class SqlMapperExtensions
{
    public static IEnumerable<TDto> MapToList<TDto>(this IDataReader reader)
        where TDto : class, new()
    {
        var result = new List<TDto>();
        var properties = typeof(TDto).GetProperties();

        while (reader.Read())
        {
            var dto = new TDto();
            foreach (var prop in properties)
            {
                var value = reader[prop.Name];
                if (value != DBNull.Value)
                {
                    prop.SetValue(dto, ConvertValue(value, prop.PropertyType));
                }
            }
            result.Add(dto);
        }

        return result;
    }

    private static object? ConvertValue(object value, Type targetType)
    {
        if (value == null) return null;

        // 기본 타입 변환
        if (targetType.IsValueType)
        {
            if (targetType == typeof(string))
                return value.ToString();
            if (targetType == typeof(int) || targetType == typeof(int?))
                return Convert.ToInt32(value);
            if (targetType == typeof(long) || targetType == typeof(long?))
                return Convert.ToInt64(value);
            if (targetType == typeof(double) || targetType == typeof(double?))
                return Convert.ToDouble(value);
            if (targetType == typeof(decimal) || targetType == typeof(decimal?))
                return Convert.ToDecimal(value);
            if (targetType == typeof(bool) || targetType == typeof(bool?))
                return Convert.ToBoolean(value);
            if (targetType == typeof(DateTime) || targetType == typeof(DateTime?))
                return Convert.ToDateTime(value);
        }

        return value;
    }
}
```

### DTO 예시

```csharp
// PatientDto.cs
public class PatientDto
{
    public string PatientId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string BloodType { get; set; } = string.Empty;
    public DateTime RegistrationDate { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
}

// PrescriptionDto.cs
public class PrescriptionDto
{
    public string PrescriptionId { get; set; } = string.Empty;
    public string PatientId { get; set; } = string.Empty;
    public string MedicineName { get; set; } = string.Empty;
    public string Dosage { get; set; } = string.Empty;
    public int Duration { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
```

---

## 트랜잭션 관리

### UnitOfWork 패턴

```csharp
// UnitOfWork.cs
public class UnitOfWork : IDaoTransaction
{
    private readonly IDBAccessService _db;
    private bool _disposed = false;

    public UnitOfWork(IDBAccessService db)
    {
        _db = db;
    }

    public Task<int> CommitAsync()
    {
        _db.CommitTransaction();
        return Task.FromResult(1);
    }

    public Task<int> RollbackAsync()
    {
        _db.RollbackTransaction();
        return Task.FromResult(1);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                // 리소스 정리
            }
            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
```

### 사용 예시

```csharp
// BizLogic에서 사용
public class PatientBizLogic
{
    private readonly IPatientDao _patientDao;

    public async Task<bool> SavePatientAsync(PatientDto patient)
    {
        using var transaction = await _patientDao.BeginTransactionAsync();

        try
        {
            // 1. 환자 저장
            await _patientDao.SaveAsync(patient);

            // 2. 처방 저장
            foreach (var prescription in patient.Prescriptions)
            {
                prescription.PatientId = patient.PatientId;
                await _prescriptionDao.SaveAsync(prescription);
            }

            await transaction.CommitAsync();
            return true;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}
```

---

## 구현 예시

### 예시 1: PatientDao 구현

```csharp
// PatientDao.cs
public class PatientDao : IDao<PatientDto>
{
    private readonly IDBAccessService _db;

    public PatientDao(IDBAccessService db)
    {
        _db = db;
    }

    public async Task<IEnumerable<PatientDto>> GetAllAsync()
    {
        const string sql = @"
            SELECT PatientId, Name, Age, Gender, BloodType, RegistrationDate,
                   Email, PhoneNumber
            FROM Patients
            ORDER BY RegistrationDate DESC";

        using var dt = await _db.ExecuteDataTableAsync(sql);

        return dt.Rows
            .Cast<DataRow>()
            .Select(row => MapToPatient(row))
            .ToList();
    }

    public async Task<PatientDto?> GetByIdAsync(string patientId)
    {
        const string sql = @"
            SELECT PatientId, Name, Age, Gender, BloodType, RegistrationDate,
                   Email, PhoneNumber
            FROM Patients
            WHERE PatientId = @PatientId";

        using var dt = await _db.ExecuteDataTableAsync(sql,
            new Dictionary<string, object> { { "PatientId", patientId } });

        return dt.Rows.Count == 0 ? null : MapToPatient(dt.Rows[0]);
    }

    public async Task<int> SaveAsync(PatientDto patient)
    {
        const string sql = @"
            INSERT INTO Patients (PatientId, Name, Age, Gender, BloodType,
                                  RegistrationDate, Email, PhoneNumber)
            VALUES (@PatientId, @Name, @Age, @Gender, @BloodType,
                    @RegistrationDate, @Email, @PhoneNumber)
            ON CONFLICT(PatientId) DO UPDATE SET
                Name = @Name,
                Age = @Age,
                Gender = @Gender,
                BloodType = @BloodType,
                Email = @Email,
                PhoneNumber = @PhoneNumber";

        return await _db.ExecuteNonQueryAsync(sql, ToParameters(patient));
    }

    public async Task<int> DeleteAsync(string patientId)
    {
        const string sql = "DELETE FROM Patients WHERE PatientId = @PatientId";
        return await _db.ExecuteNonQueryAsync(sql,
            new Dictionary<string, object> { { "PatientId", patientId } });
    }

    public async Task<IEnumerable<PatientDto>> GetByConditionAsync(Expression<Func<PatientDto, bool>> condition)
    {
        // Dynamic LINQ 사용
        var expression = condition.ToString();
        var sql = $"SELECT * FROM Patients WHERE {expression}";
        return await QueryAsync(sql);
    }

    public async Task<PagedResult<PatientDto>> GetPagedAsync(int page, int pageSize, string? orderBy = null)
    {
        var offset = (page - 1) * pageSize;
        var orderByClause = orderBy ?? "RegistrationDate DESC";
        var sql = $@"
            SELECT * FROM Patients
            ORDER BY {orderByClause}
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

        var countSql = "SELECT COUNT(*) FROM Patients";

        var dt = await _db.ExecuteDataTableAsync(sql,
            new Dictionary<string, object>
            {
                { "Offset", offset },
                { "PageSize", pageSize }
            });

        using var countDt = await _db.ExecuteDataTableAsync(countSql);

        return new PagedResult<PatientDto>
        {
            Items = dt.Rows.Cast<DataRow>().Select(MapToPatient),
            TotalCount = countDt.Rows[0][0] != DBNull.Value
                ? Convert.ToInt32(countDt.Rows[0][0])
                : 0,
            Page = page,
            PageSize = pageSize
        };
    }

    public Task<IEnumerable<PatientDto>> GetByParentIdAsync(string parentId)
    {
        throw new NotImplementedException();
    }

    public Task<int> SaveManyAsync(IEnumerable<PatientDto> dtos)
    {
        throw new NotImplementedException();
    }

    public Task<int> DeleteManyAsync(IEnumerable<string> ids)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<PatientDto>> QueryAsync(string sql, object? parameters = null)
    {
        throw new NotImplementedException();
    }

    public Task<int> ExecuteNonQueryAsync(string sql, object? parameters = null)
    {
        throw new NotImplementedException();
    }

    public Task<T> ExecuteScalarAsync<T>(string sql, object? parameters = null)
    {
        throw new NotImplementedException();
    }

    public Task<IDaoTransaction> BeginTransactionAsync()
    {
        _db.BeginTransaction();
        return Task.FromResult<IDaoTransaction>(new DaoTransaction(_db));
    }

    // Private Helpers
    private PatientDto MapToPatient(DataRow row)
    {
        return new PatientDto
        {
            PatientId = row["PatientId"]?.ToString() ?? string.Empty,
            Name = row["Name"]?.ToString() ?? string.Empty,
            Age = row["Age"] != DBNull.Value ? Convert.ToInt32(row["Age"]) : 0,
            Gender = row["Gender"]?.ToString() ?? string.Empty,
            BloodType = row["BloodType"]?.ToString() ?? string.Empty,
            RegistrationDate = row["RegistrationDate"] != DBNull.Value
                ? Convert.ToDateTime(row["RegistrationDate"])
                : DateTime.Now,
            Email = row["Email"]?.ToString(),
            PhoneNumber = row["PhoneNumber"]?.ToString()
        };
    }

    private Dictionary<string, object> ToParameters(PatientDto patient)
    {
        return new Dictionary<string, object>
        {
            { "PatientId", patient.PatientId },
            { "Name", patient.Name },
            { "Age", patient.Age },
            { "Gender", patient.Gender },
            { "BloodType", patient.BloodType },
            { "RegistrationDate", patient.RegistrationDate },
            { "Email", patient.Email },
            { "PhoneNumber", patient.PhoneNumber }
        };
    }

    private class DaoTransaction : IDaoTransaction
    {
        private readonly IDBAccessService _db;

        public DaoTransaction(IDBAccessService db)
        {
            _db = db;
        }

        public Task<int> CommitAsync()
        {
            _db.CommitTransaction();
            return Task.FromResult(1);
        }

        public Task<int> RollbackAsync()
        {
            _db.RollbackTransaction();
            return Task.FromResult(1);
        }

        public void Dispose()
        {
            // 트랜잭션은 자동 커밋/롤백 처리
        }
    }
}
```

### 예시 2: BizLogic에서 DAO 사용

```csharp
// PatientBizLogic.cs
public class PatientBizLogic
{
    private readonly IPatientDao _patientDao;
    private readonly IPrescriptionDao _prescriptionDao;

    public PatientBizLogic(IPatientDao patientDao, IPrescriptionDao prescriptionDao)
    {
        _patientDao = patientDao;
        _prescriptionDao = prescriptionDao;
    }

    public async Task<PatientDto?> GetPatientAsync(string patientId)
    {
        return await _patientDao.GetByIdAsync(patientId);
    }

    public async Task<IEnumerable<PatientDto>> GetPatientsAsync(string? searchTerm = null)
    {
        if (string.IsNullOrEmpty(searchTerm))
        {
            return await _patientDao.GetAllAsync();
        }

        var conditions = new Dictionary<string, object>
        {
            { "Name", $"{searchTerm}%" },
            { "PatientId", $"{searchTerm}%" }
        };

        return await _patientDao.GetByConditionAsync(ConstructExpression<PatientDto>(
            p => p.Name.StartsWith(searchTerm) || p.PatientId.StartsWith(searchTerm)));
    }

    public async Task<PagedResult<PatientDto>> GetPagedPatientsAsync(int page, int pageSize, string? orderBy = null)
    {
        return await _patientDao.GetPagedAsync(page, pageSize, orderBy);
    }

    public async Task<bool> SavePatientAsync(PatientDto patient)
    {
        using var transaction = await _patientDao.BeginTransactionAsync();

        try
        {
            // 환자 저장
            var result = await _patientDao.SaveAsync(patient);
            if (result == 0) return false;

            // 처방 저장 (환자가 여러 처방을 가지는 경우)
            foreach (var prescription in patient.Prescriptions ?? Enumerable.Empty<PrescriptionDto>())
            {
                prescription.PatientId = patient.PatientId;
                await _prescriptionDao.SaveAsync(prescription);
            }

            await transaction.CommitAsync();
            return true;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<bool> DeletePatientAsync(string patientId)
    {
        using var transaction = await _patientDao.BeginTransactionAsync();

        try
        {
            // 처방 삭제 먼저
            await _prescriptionDao.DeleteByPatientIdAsync(patientId);

            // 환자 삭제
            return await _patientDao.DeleteAsync(patientId) > 0;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    private static Expression<Func<T, bool>> ConstructExpression<T>(Func<T, bool> condition)
    {
        return condition;
    }
}
```

### 예시 3: UI에서 BizLogic 사용

```csharp
// PatientListControl.cs
public class PatientListControl : BaseWorkControl
{
    private readonly IPatientBizLogic _patientBizLogic;
    private readonly IPrescriptionBizLogic _prescriptionBizLogic;

    private DataTable _patientGrid;

    public PatientListControl(
        IPatientBizLogic patientBizLogic,
        IPrescriptionBizLogic prescriptionBizLogic)
    {
        _patientBizLogic = patientBizLogic;
        _prescriptionBizLogic = prescriptionBizLogic;

        InitializeControl();
    }

    private void InitializeControl()
    {
        var gridView = new DevExpress.XtraGrid.Views.Grid.GridView();
        gridView.Columns.Add("PatientId", "환자 ID");
        gridView.Columns.Add("Name", "이름");
        gridView.Columns.Add("Age", "나이");
        gridView.Columns.Add("Gender", "성별");
        gridView.Columns.Add("RegistrationDate", "등록일");

        _patientGrid = new DataTable();
        _patientGrid.Columns.Add("PatientId", typeof(string));
        _patientGrid.Columns.Add("Name", typeof(string));
        _patientGrid.Columns.Add("Age", typeof(int));
        _patientGrid.Columns.Add("Gender", typeof(string));
        _patientGrid.Columns.Add("RegistrationDate", typeof(DateTime));

        gridView.DataSource = _patientGrid;
    }

    protected override async void OnScreenActivated()
    {
        await LoadPatientDataAsync();
    }

    private async Task LoadPatientDataAsync()
    {
        try
        {
            var patients = await _patientBizLogic.GetPatientsAsync();
            LoadPatientGrid(patients);
        }
        catch (Exception ex)
        {
            LogManager.Error($"환자 데이터 로드 실패: {ex.Message}", "PatientListControl", ex);
            XtraMessageBox.Show("데이터 로드 실패", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void LoadPatientGrid(IEnumerable<PatientDto> patients)
    {
        _patientGrid.Clear();

        foreach (var patient in patients)
        {
            var row = _patientGrid.NewRow();
            row["PatientId"] = patient.PatientId;
            row["Name"] = patient.Name;
            row["Age"] = patient.Age;
            row["Gender"] = patient.Gender;
            row["RegistrationDate"] = patient.RegistrationDate.ToString("yyyy-MM-dd");
            _patientGrid.Rows.Add(row);
        }
    }

    private async Task SearchPatientAsync()
    {
        var searchTerm = GetSearchTerm();

        try
        {
            var patients = await _patientBizLogic.GetPatientsAsync(searchTerm);
            LoadPatientGrid(patients);
        }
        catch (Exception ex)
        {
            LogManager.Error($"검색 실패: {ex.Message}", "PatientListControl", ex);
            XtraMessageBox.Show("검색 실패", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private string GetSearchTerm()
    {
        // 검색 입력창에서 값을 가져옴
        // ...
        return "";
    }
}
```

---

## 단계별 구현 계획

### Phase 1: 기반 구축 (1-2주)

1. **DAO 인터페이스 정의**
   - `IDao.cs` - Generic DAO 인터페이스
   - `IDaoTransaction.cs` - 트랜잭션 인터페이스
   - `PagedResult.cs` - 페이징 결과 클래스

2. **SqlMapper 구현**
   - `SqlMapper.cs` - Dapper 기반 매핑
   - `SqlMapperExtensions.cs` - 확장 메서드

3. **SqlBuilder 구현**
   - `SqlBuilder.cs` - SQL 빌더
   - `InClauseBuilder.cs` - IN 절 빌더

4. **Unit of Work 구현**
   - `UnitOfWork.cs` - 트랜잭션 관리

**테스트:**
```csharp
// 단위 테스트
[Test]
public async Task PatientDao_GetByIdAsync_ReturnsPatient()
{
    var mockDb = new Mock<IDBAccessService>();
    var patientDao = new PatientDao(mockDb.Object);

    var expectedPatient = new PatientDto
    {
        PatientId = "P001",
        Name = "홍길동",
        Age = 30
    };

    // Mock 설정
    mockDb.Setup(x => x.ExecuteDataTableAsync(
        It.IsAny<string>(),
        It.IsAny<Dictionary<string, object>>()))
        .ReturnsAsync(CreateDataTable(expectedPatient));

    var result = await patientDao.GetByIdAsync("P001");

    Assert.IsNotNull(result);
    Assert.AreEqual(expectedPatient.PatientId, result.PatientId);
}
```

---

### Phase 2: 기존 Repository 리팩토링 (2-3주)

1. **시스템 설정 Repository 유지**
   - `SQLiteModuleRepository.cs` (유지)
   - `SQLiteComponentRepository.cs` (유지)
   - `SQLiteProgramRepository.cs` (유지)

2. **비즈니스 Repository → DAO 마이그레이션**
   - `PatientRepository` → `PatientDao`
   - `OrderRepository` → `OrderDao`
   - `PrescriptionRepository` → `PrescriptionDao`
   - `DepartmentRepository` → `DepartmentDao`

3. **Mapper 클래스 작성**
   - `PatientMapper.cs`
   - `OrderMapper.cs`
   - `PrescriptionMapper.cs`
   - ...

**전략:**
- 기존 Repository는 유지하되, 내부적으로 DAO 사용
- 단계적으로 DAO로 전환

---

### Phase 3: BizLogic 계층 개선 (1-2주)

1. **BizLogic 리팩토링**
   - BizLogic에서 Repository 대신 DAO 사용
   - 트랜잭션 관리 개선

2. **UI 레이어 연동**
   - WinForms 화면에서 BizLogic 사용 유지
   - 내부적으로는 DAO 호출

---

### Phase 4: 테스트 및 최적화 (1주)

1. **단위 테스트 작성**
   - 각 DAO 메서드 테스트
   - 트랜잭션 테스트
   - 에지 케이스 테스트

2. **통합 테스트**
   - 전체 플로우 테스트
   - 트랜잭션 롤백 테스트

3. **성능 최적화**
   - 인덱스 확인
   - 쿼리 최적화
   - 캐싱 전략

---

## 백엔드 연동 전략

### ServerDBAccessService 확장

```csharp
// ServerDBAccessService에 Connection 노출 (선택적)
public class ServerDBAccessService : IDBAccessService
{
    // ... 기존 코드 ...

    // Connection 노출 (Dapper 사용 시)
    public IDbConnection GetConnection()
    {
        return _connection;
    }

    // ConnectionPool 관리
    private readonly ConcurrentDictionary<string, IDbConnection> _connectionPool
        = new ConcurrentDictionary<string, IDbConnection>();

    public IDbConnection GetPooledConnection()
    {
        var connStr = _connectionString;
        return _connectionPool.GetOrAdd(connStr, _ => CreateConnection());
    }

    private IDbConnection CreateConnection()
    {
        return _connectionFactory();
    }
}
```

### LocalDbService 확장

```csharp
// LocalDbService에 Connection 노출
public class LocalDbService : IDBAccessService
{
    // ... 기존 코드 ...

    public SQLiteConnection GetConnection()
    {
        if (_connection == null || _connection.State != ConnectionState.Open)
            Connect();

        return _connection;
    }
}
```

---

## 백엔드 REST API 연동

### HttpDBAccessClient에서 Dapper 사용

```csharp
// HttpDBAccessClient.cs (확장)
public class HttpDBAccessClient : DBAccessClientBase
{
    // ... 기존 코드 ...

    public async Task<IEnumerable<TDto>> QueryAsync<TDto>(string sql, object? parameters = null)
        where TDto : class, new()
    {
        var requestData = new { CommandText = sql, Parameters = parameters };
        var response = await _httpClient.PostAsJsonAsync("/api/v1/db/query", requestData);
        var result = await response.Content.ReadFromJsonAsync<List<Dictionary<string, object>>>(_jsonOptions);

        return result?.Select(MapToDto<TDto>).ToList() ?? new List<TDto>();
    }

    public async Task<int> ExecuteNonQueryAsync(string sql, object? parameters = null)
    {
        var requestData = new { CommandText = sql, Parameters = parameters };
        var response = await _httpClient.PostAsJsonAsync("/api/v1/db/query", requestData);
        return await response.Content.ReadFromJsonAsync<int>(_jsonOptions) ?? 0;
    }

    public async Task<TDto?> QueryFirstOrDefaultAsync<TDto>(string sql, object? parameters = null)
        where TDto : class, new()
    {
        var dt = await ExecuteDataTableAsync(sql, parameters);
        return dt.Rows.Count == 0 ? null : MapToDto<TDto>(dt.Rows[0]);
    }

    private TDto MapToDto<TDto>(DataRow row) where TDto : class, new()
    {
        var dto = new TDto();
        var properties = typeof(TDto).GetProperties();

        foreach (var prop in properties)
        {
            var value = row[prop.Name];
            if (value != DBNull.Value)
            {
                prop.SetValue(dto, ConvertValue(value, prop.PropertyType));
            }
        }

        return dto;
    }
}
```

---

## 결론

### 추천 전략 요약

1. **Generic DAO 도입**
   - IDao<T> 인터페이스 사용
   - 화면별 다른 Repository 구현 제거
   - 강타입 데이터 접근

2. **Dapper 기반 매핑**
   - SqlMapper.cs로 데이터 매핑 자동화
   - DataTable 매핑 로직 제거
   - 타입 안전성 보장

3. **SQL Builder 패턴**
   - SqlBuilder.cs로 동적 쿼리 빌딩
   - IN 절, WHERE 절, ORDER BY, 페이징 자동 생성
   - 매개변수 바인딩 자동화

4. **트랜잭션 관리**
   - UnitOfWork 패턴 사용
   - 여러 DAO를 한 트랜잭션으로 묶기
   - 롤백 처리 자동화

5. **백엔드 유지**
   - X-Reference 백엔드 REST API 유지
   - ServerDBAccessService + HttpDBAccessClient
   - Server에서는 Dapper, Client에서는 기존 방식 유지

### 이점

- **코드 간소화**: DataTable 매핑 코드 제거
- **유지보수성**: Generic DAO로 일관성 확보
- **성능**: 강타입 매핑으로 성능 향상
- **안정성**: 트랜잭션 관리 개선
- **확장성**: 쉬운 추가 및 수정

---

**문서 버전**: 1.0
**작성일**: 2026-02-12
**작성자**: Sisyphus (OpenCode AI Agent)
