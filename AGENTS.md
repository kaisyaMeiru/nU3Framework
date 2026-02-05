# nU3.Framework - Agentic Coding Agent Instructions

> **Framework**: nU3.Framework (Medical IS Framework)  
> **Tech Stack**: .NET 8.0, WinForms, DevExpress 23.2.9, ASP.NET Core  
> **Purpose**: Guide for AI coding agents operating in this repository.

---

## Build, Lint, Test Commands

### Build Commands
```bash
# Build entire solution
dotnet build nU3.Framework.sln --configuration Release
dotnet build nU3.Framework.sln --configuration Debug

# Build specific project
dotnet build nU3.Core/nU3.Core.csproj

# Restore packages / Clean
dotnet restore nU3.Framework.sln
dotnet clean nU3.Framework.sln
```

### Test Commands
```bash
# NOTE: Only DbTest project exists (no xUnit/NUnit yet)

# Run DbTest project
dotnet build Tools/DbTest/DbTest.csproj

# Run specific test (when test framework added)
dotnet test --filter "FullyQualified.TestMethodName"
dotnet test --filter "Category=Unit" --verbosity normal
```

### Lint/Analysis Commands
```bash
# Check for compiler warnings
dotnet build --verbosity quiet 2>&1 | findstr /C /warning

# Check for obsolete packages
dotnet list package --deprecated

# Use Visual Studio → Build → Analyze Solution for Code Clones
```

---

## Code Style Guidelines

### Imports & Namespaces
```csharp
// 1. System imports (alphabetical)
using System;
using System.Threading.Tasks;

// 2. Third-party (DevExpress)
using DevExpress.XtraEditors;

// 3. nU3 internal
using nU3.Core;
using nU3.Models;
```

### Naming Conventions
| Element | Convention | Example |
|---------|-----------|--------|
| Classes | PascalCase | `PatientListControl` |
| Interfaces | I-prefixed | `IShellForm`, `IWorkForm` |
| DTOs | **Dto** suffix | `PatientInfoDto` |
| Enums | PascalCase | `ComponentType` |
| Methods | PascalCase | `LoadData()` |
| Private fields | _camelCase | `_instance` |
| Events | **Event** suffix | `PatientSelectedEvent` |
| Event Payloads | **EventPayload** suffix | `PatientSelectedEventPayload` |
| Exceptions | **Exception** suffix | `AuthenticationException` |

### Formatting & Types
```csharp
// 4-space indentation, no tabs
// XML comments: /// <summary>
// Use regions: #region Public Methods

// Nullable reference types
public string? ServerUrl { get; }
public Task<T> GetAsync<T>(string id);

// Use 'var' when type obvious, List<T> over arrays
var results = new List<PatientInfoDto>();

// Async/await patterns
public async Task LoadDataAsync(CancellationToken token = default)
{
    token.ThrowIfCancellationRequested();
    var data = await _dbClient.ExecuteDataTableAsync(sql, token).ConfigureAwait(false);
}
```

### Error Handling & Logging
```csharp
// Never throw on non-fatal paths
try { LogManager.Info(message, category); } catch { }

// Always log exceptions with context
catch (Exception ex)
{
    LogManager.Error($"Operation failed: {ex.Message}", category, ex);
    throw; // Re-throw critical errors
}
```

---

## Module & Architecture Guidelines

### Plugin Development
```csharp
[nU3ProgramInfo(typeof(MyModuleScreen), "Display Name", "MODULE_ID", "CHILD")]

public class MyModuleScreen : BaseWorkControl
{
    public override string ScreenId => "MY_SCREEN_001";
    protected override void OnScreenActivated() { }
    protected bool CanUpdate => HasPermission(p => p.CanUpdate);
}
```

### Event-Driven Communication
```csharp
// Publish events
EventBus?.GetEvent<PatientSelectedEvent>()
    .Publish(new PatientSelectedEventPayload { Patient = patient, Source = ScreenId });

// Subscribe (avoid circular dependencies)
protected override void OnScreenActivated()
{
    EventBus?.GetEvent<PatientSelectedEvent>().Subscribe(OnPatientSelected);
}

private void OnPatientSelected(object payload)
{
    if (payload is PatientSelectedEventPayload evt && evt.Source == ScreenId)
        return; // Ignore own events
}
```

### Dependency Injection
```csharp
// Constructor injection for services
public class MyService
{
    private readonly IComponentRepository _repo;
    public MyService(IComponentRepository repo) => _repo = repo;
}

// Use [ActivatorUtilitiesConstructor] for views
public MyView(IService service) : BaseWorkControl
{
    [ActivatorUtilitiesConstructor]
    public MyView(IService service) : this() { _service = service; }
}
```

### Database & UI Development
```csharp
// Always use repository pattern with async/await
// Use parameterized queries to prevent SQL injection

// Use nU3 controls (wrapped DevExpress, namespace: nU3.Core.UI.Controls)
// Inherit from BaseWorkControl, use RegisterDisposable() for resources
RegisterDisposable(_httpClient);
```

---

## Important Notes

### Medical Domain
- Validate patient IDs (HIPAA), use DateTimeOffset for timestamps
- Never log sensitive patient data, use secure storage for keys
- Gender: 0=Unknown, 1=Male, 2=Female, 3=Other
- BloodType: 0=Unknown, 1=A+, 2=A-, 3=B+, 4=B-, 5=O+, 6=O-

### DevExpress Specifics
- Reference Demo: `c:\Project2_OPERATION\05.Framework\Reference\Example_CS\`
- Reference Docs: `c:\Project2_OPERATION\05.Framework\Reference\DevExpress.WindowsForms.v23.2\`
- Use nU3 controls (same API as DevExpress, just namespace/prefix change)
- Separate designer code for WinForms designer compatibility, no lambdas in designer code

### Common Pitfalls
```csharp
// ❌ Magic numbers, hardcoded paths, Thread.Sleep(), silent catches, static singletons
// ✅ Constants, Path.Combine(), await Task.Delay(), log+rethrow, DI container
```

### Thread Safety & Disposables
```csharp
private readonly ConcurrentDictionary<int, HttpClient> _httpClientPool;
private readonly object _lock = new object();

public MyControl()
{
    _timer = new Timer();
    RegisterDisposable(_timer);  // Auto-cleanup in BaseWorkControl
}
```

---

## Project Structure & Dependencies

```
SRC/
├── nU3.Core/         # Business logic, interfaces, services
├── nU3.Core.UI/       # Base UI classes, controls
├── nU3.Core.UI.Components/  # nU3 wrapped controls
├── nU3.Data/          # Data access layer
├── nU3.Models/        # DTOs, entities, enums
├── nU3.Connectivity/  # External APIs, HTTP clients
├── nU3.Bootstrapper/  # DI setup
├── nU3.Shell/         # Shell interface
├── nU3.MainShell/     # Main shell implementation
├── nU3.Tools.Deployer/ # Deployment tool
├── Servers/           # Server projects (Host, Connectivity)
├── Modules/           # Business modules (EMR, ADM, OT, etc.)
└── Tools/             # Utilities (DbTest)
```

**Key Dependencies**: DevExpress WinForms v23.2.9, Microsoft.Extensions.DependencyInjection v8.0.0, System.Data.SqlClient, System.Net.Http.Json

---

## Critical Guidelines (Korean)

### 코딩 규칙
- **주석**: 한글로 작성하며 최대한 자세하게 작성한다. 내용이 틀린 경우에만 삭제하며, 기존 주석은 임의로 삭제하지 않는다
- **코드주석 처리**: 임의로 삭제하지 않는다
- **문서 작성**:
  - Framework 전반 문서: `DOC 폴더/DOC_{Category}_{Title}.md`
  - 프로젝트 내 문서: `.csproj와 동일 위치/DOC_{Category}_{Function}_{Detail}.md`
  - 코드 수정 시 DOC 내용을 갱신한다
- **인코딩**: 모든 코드 및 문서는 UTF8로 저장한다

### DevExpress 컨트롤 사용
- 이 프로젝트에서 사용되는 컨트롤은 nU3 컨트롤을 사용해야 한다 (네임스페이스: `nU3.Core.UI.Controls`)
- DevExpress 컨트롤을 상속받아 랩핑했으므로 동일한 API로 작동한다
- DevExpress 컨트롤로 작성 후 네임스페이스와 접두어를 일괄 변경하여 변환 가능하다

### 화면 개발
- 윈폼 디자이너에서 디자인 가능하도록 디자인 코드를 분리해야 한다
- 디자인 코드에는 람다식이 들어가면 안된다

---

**Last Updated**: 2026-02-05
