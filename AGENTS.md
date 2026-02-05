# nU3.Framework - Agentic Coding Agent Instructions

> **Framework**: nU3.Framework (Medical IS Framework)  
> **Tech Stack**: .NET 8.0, WinForms, DevExpress 23.2.9, ASP.NET Core  
> **Last Updated**: 2026-02-03  
> **Purpose**: Guide for AI coding agents operating in this repository.

---

## Build, Lint, Test Commands

### Build Commands
```bash
# Build entire solution (Release)
dotnet build nU3.Framework.sln --configuration Release

# Build entire solution (Debug)
dotnet build nU3.Framework.sln --configuration Debug

# Build specific project
dotnet build nU3.Core/nU3.Core.csproj

# Restore packages
dotnet restore nU3.Framework.sln

# Clean build
dotnet clean nU3.Framework.sln
```

### Test Commands
```bash
# NOTE: No xUnit/NUnit test projects currently exists (TODO: Add xUnit to Modules/Tests/)
# Only DbTest project exists

# Run all tests (when available)
dotnet test --filter "Category=Unit" --verbosity normal

# Run specific test file (when test projects exist)
# Example: dotnet test Tests/Modules.EMR.IN.Tests/EMR.IN.Worklist.Tests.cs --logger "xUnit"
```

### Run Single Test (When Test Projects Added)
```bash
# Run specific test method (when test framework added)
dotnet test --filter "FullyQualified.TestMethodName"

# Run tests with coverage (when test framework added)
dotnet test /p:Coverlet.Coverlet.DotCover /p:Coverlet.Coverlet.Collector.VSwhere /p:Coverlet.Coverlet.OpenCover
```

### Lint/Analysis Commands
```bash
# Analyze dependencies (Visual Studio)
# Use Visual Studio → Build → Analyze Solution for Code Clones
# Or: Rider → Code → Inspect Code

# Check for compiler warnings
dotnet build --verbosity quiet 2>&1 | findstr /C /warning

# Check for obsolete APIs (when tooling exists)
dotnet list package --deprecated
```

---

## Code Style Guidelines

### Imports & Namespaces
```csharp
// 1. System namespace imports first (sorted alphabetically)
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

// 2. Third-party (DevExpress, etc.)
using DevExpress.XtraEditors;

// 3. nU3 internal namespaces
using nU3.Core;
using nU3.Connectivity;
using nU3.Models;
```

### Formatting & Indentation
```csharp
// 1. Use 4-space indentation (no tabs)
// 2. XML comments use triple slashes /// or block XML
/// <summary>
/// Service description.
/// </summary>

// 3. Region directives for code organization
#region Public Methods
#endregion
```

### Naming Conventions

| Element | Convention | Example |
|---------|-----------|--------|
| **Classes** | PascalCase | `PatientListControl`, `ConnectivityManager` |
| **Interfaces** | I-prefixed (Framework) | `IShellForm`, `IWorkForm` |
| **DTOs** | **Dto** suffix | `PatientInfoDto`, `ComponentVerDto` |
| **Enums** | PascalCase | `ComponentType`, `BloodType` |
| **Methods** | PascalCase | `LoadData()`, `InitializeContext()` |
| **Private fields** | _camelCase prefix | `_instance`, `_httpClient` |
| **Events** | **Event** suffix (PubSub) | `PatientSelectedEvent` |
| **Event Payloads** | **EventPayload** suffix | `PatientSelectedEventPayload` |
| **Exceptions** | **Exception** suffix | `InvalidOperationException`, `AuthenticationException` |

### Types & Generics
```csharp
// 1. Always use nullable reference types for optional values
public string? ServerUrl { get; }
public Task<T> GetAsync<T>(string id);

// 2. Use 'var' for local variables when type is obvious
var fileInfo = new FileInfo(filePath);
var results = new List<DataTable>();

// 3. Use generics for reusable collections
public interface IRepository<T> where T : class;

// 4. Prefer 'List<T>' over arrays unless performance critical
public List<PatientInfoDto> GetPatients();

// 5. Use 'ReadOnlySpan' for string parameters
public void ProcessData(ReadOnlySpan<char> xmlData);
```

### Async/Await Patterns
```csharp
// 1. Use async/await for I/O operations
public async Task LoadDataAsync()
{
    var data = await _dbClient.ExecuteDataTableAsync(sql, parameters);
}

// 2. Always pass CancellationToken
public async Task CancelableOperationAsync(CancellationToken token)
{
    token.ThrowIfCancellationRequested();
    await _fileTransferService.UploadFileAsync(file, path, token);
}

// 3. ConfigureAwait(false) for non-UI async calls
var response = await _httpClient.GetAsyncAsync(url).ConfigureAwait(false);
```

### Error Handling
```csharp
// 1. Never throw on non-fatal paths
public void LogInfo(string message, string category)
{
    try
    {
        LogManager.Info(message, category);
    }
    catch { }  // Silent fail
}

// 2. Always log exceptions with context
catch (Exception ex)
{777777
    LogManager.Error($"Operation failed: {ex.Message}", category, ex);
    throw; // Re-throw for critical errors
}

// 3. Use custom exceptions for business logic
public class AuthenticationException : Exception
{
    public AuthenticationException(string message) : base(message) { }
}
```

### Logging Guidelines
```csharp
// 1. Use context-aware logging
LogManager.Info($"Patient selected: {patient.PatientName}", "EMR_IN");

// 2. Use audit logging for sensitive operations
LogAudit(AuditAction.Read, "Patient", patientId, "Viewed patient details");

// 3. Log progress for long operations
LogInfo($"Deploying {files.Length} files...", "Deployer");
```

---

## Module & Architecture Guidelines

### Plugin Development
```csharp
// 1. Always include [nU3ProgramInfo] attribute
[nU3ProgramInfo(
    typeof(MyModuleScreen),
    "Module Display Name",       // Program Name (used in UI)
    "MODULE_ID"             // Unique ID
    "CHILD"               // Form Type: CHILD/POPUP/SDI
)]

public class MyModuleScreen : BaseWorkControl
{
    public override string ScreenId => "MY_SCREEN_001";
    public override string ProgramTitle => "Module Display Name";
}
```

### Event-Driven Communication
```csharp
// Module-to-Module (Decoupled)
// Publish events for state changes
EventBus?.GetEvent<PatientSelectedEvent>()
    .Publish(new PatientSelectedEventPayload { Patient = patient, Source = ScreenId });

// Subscribe to relevant events
protected override void OnScreenActivated()
{
    EventBus?.GetEvent<PatientSelectedEvent>()
        .Subscribe(OnPatientSelected);
}

// Avoid circular event dependencies (check Source)
private void OnPatientSelected(object payload)
{
    if (payload is PatientSelectedEventPayload evt && evt.Source == ScreenId)
        return; // Ignore own events
}
```

### Dependency Injection
```csharp
// Use DI for services (Microsoft.Extensions.DependencyInjection)
// Avoid direct instantiation in business logic

// Constructor injection for services
public class MyBusinessService
{
    private readonly IComponentRepository _repo;
    private readonly ILogger _logger;
    
    public MyBusinessService(
        IComponentRepository repo,
        ILogger logger)
    {
        _repo = repo;
        _logger = logger;
    }
}

// Use [ActivatorUtilitiesConstructor] for view constructors
public MyView(IService service) : BaseWorkControl
{
    [ActivatorUtilitiesConstructor]
    public MyView(IService service) : this()
    {
        _service = service; // Injected
    }
}
```

### Database Access
```csharp
// Always use repository pattern
// Use async/await for DB operations

// Example: Repository
public interface IPatientRepository
{
    Task<PatientInfoDto> GetPatientAsync(string patientId);
    Task SavePatientAsync(PatientInfoDto patient);
}

// Avoid raw SQL in UI components - use services
// Use parameterized queries to prevent SQL injection
```

### UI Development (WinForms + DevExpress)
```csharp
// Inherit from BaseWorkControl, not Control directly
public partial class PatientListControl : BaseWorkControl
{
    // Override lifecycle methods
    protected override void OnScreenActivated() { }
    protected override void OnScreenDeactivated() { }
    
    // Use permission checks
    protected bool CanUpdate => HasPermission(p => p.CanUpdate);
}
}

// Use DevExpress controls via wrappers
// nU3GridControl, nU3TextEdit, etc.

// Resource management is automatic in BaseWorkControl
// Register disposable resources
RegisterDisposable(_httpClient);
```

### Component Deployment
```csharp
// 1. ComponentId must be unique (include extension if needed)
// 2. Always verify file hash before overwriting
// 3. Use StoragePath that preserves folder structure

// Component naming convention
// Core: "nU3.Core", "DevExpress.XtraEditors" → Root folder
// Modules: "Modules\EMR\IN\nU3.Modules.EMR.IN.Worklist.dll"
// Plugins: "plugins\PluginName\PluginName.dll"
// Resources: "resources\images\logo.png"
```

---

## Important Notes for Agents

### 1. Medical Domain Specifics
```csharp
// Medical Data Handling
// - Always validate patient IDs (HIPAA)
// - Use DateTimeOffset for timestamps
// - Never log sensitive patient data to console/file
// - Use secure storage for encryption keys

// Medical Constants
// Gender: 0=Unknown, 1=Male, 2=Female, 3=Other
// BloodType: 0=Unknown, 1=A+, 2=A-, 3=B+, 4=B-, 5=O+, 6=O-
```

### 2. Thread Safety
```csharp
// Use ConcurrentDictionary for thread-safe caches
private readonly ConcurrentDictionary<int, HttpClient> _httpClientPool;

// Use lock for non-thread-safe operations
private readonly object _lock = new object();
lock (_lock) { ... }
```

### 3. Disposable Pattern
```csharp
// Always call RegisterDisposable() for disposable resources
public class MyControl : BaseWorkControl
{
    private Timer _timer;
    
    public MyControl()
    {
        _timer = new Timer();
        RegisterDisposable(_timer);  // Auto-cleanup
    }
}
```

### 4. DevExpress Specifics
```csharp
// Use XtraTabbedMdiManager for MDI forms
// Use XtraGridControl with GridView for tables
// Use LayoutControl for responsive layouts

// Avoid direct form show from business logic
// Use navigation pattern instead
```

### 5. Common Pitfalls to Avoid
```csharp
// ❌ DO: Use string magic numbers
// ✅ DO: Use constants with meaningful names
const int DEFAULT_TIMEOUT_SECONDS = 30;

// ❌ DO: Hardcode paths
// ✅ DO: Use Path.Combine() and Environment variables
string configPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "config");

// ❌ DO: Use Thread.Sleep()
// ✅ DO: await Task.Delay() or CancellationToken

// ❌ DO: Catch and ignore exceptions silently
// ✅ DO: Log and rethrow with context

// ❌ DO: Implement Singleton as static property
// ✅ DO: Use DI Container for lifecycle management
```

### 6. Testing Considerations
```csharp
// When writing tests for this project:
// - Mock repositories (IComponentRepository, ILogger, etc.)
// - Mock HTTP clients (HttpClient, IDBAccessService)
// - Test async methods (with ConfigureAwait(false))
// - Use in-memory database for unit tests (SQLite in-memory mode)
// - Verify all exceptions are logged appropriately
```

---

## Quick Reference

### Project Structure
```
SRC/
├── nU3.Core/         # Business logic, interfaces, services
├── nU3.Core.UI/       # Base UI classes, controls
├── nU3.Data/          # Data access layer
├── nU3.Models/        # DTOs, entities, enums
├── nU3.Connectivity/     # External APIs, HTTP clients
├── nU3.Tools.Deployer/# Deployment tool
└── Modules/              # Business modules (EMR, ADM, OT, etc.)
```

### Key Dependencies
- DevExpress WinForms v23.2.9
- Microsoft.Extensions.DependencyInjection v8.0.0
- System.Data.SqlClient (for local SQLite)
- System.Net.Http.Json (for REST APIs)


## Inportant ##
- 주석은 한글로 작성하며 최대한 자세하게 작성한다. 
- 주석은 내용이 틀린 경우 삭제해도 무방하나 기존에 있던 주석을 임의로 삭제하지 않는다.
- 코드주석 처리는 임의로 삭제하지 않는다. 
- Framework 전반에 관련된 문서는 DOC 폴더밑에 DOC_{Category}_{Title}.md 형식으로 작성한다.
- 프로젝트 내에 문서 파일은 프로젝트 파일(.csproj)과 동일한 곳에 생성하며, DOC_{Category}_{Function}_{Detail}.md 로 형식으로 작성한다.
- 코드가 수정이 되면 DOC_ 내용에 맞게 갱신한다.
- 모든 코드 및 문서는 UTF8로 인코딩하여 저장할것.
- DevExpress Winform Demo : c:\Project2_OPERATION\05.Framework\Reference\Example_CS\
- DevExpress Winform Document : c:\Project2_OPERATION\05.Framework\Reference\DevExpress.WindowsForms.v23.2\
-


---

**Generated**: 2026-02-03  
**Version**: 1.0  
**Framework**: nU3.Framework v1.0
