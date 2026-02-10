# Visual Architecture Documentation
## nU3.Framework System Architecture

**Version:** 1.0
**Date:** February 2026
**Format:** Mermaid Diagrams (renderable in GitHub, VS Code, and Mermaid Live Editor)

---

## 📋 Table of Contents

1. [System Context Diagram](#system-context-diagram)
2. [Container Diagram](#container-diagram)
3. [Component Diagram](#component-diagram)
4. [Sequence Diagrams](#sequence-diagrams)
5. [Deployment Diagram](#deployment-diagram)
6. [Data Flow Diagrams](#data-flow-diagrams)
7. [Module Loading Flow](#module-loading-flow)
8. [Update Deployment Flow](#update-deployment-flow)

---

## System Context Diagram

Shows the external entities that interact with the nU3.Framework system.

```mermaid
graph TB
    subgraph "Medical Systems"
        direction LR
        HL7[HL7 V2.x Gateway]
        FHIR[FHIR Server]
        DICOM[DICOM PACS]
        LIS[LIS Laboratory]
        INSURANCE[Insurance EDI]
    end

    subgraph "nU3.Framework"
        direction TB
        Shell[nU3.Shell]
        Core[nU3.Core]
        Connectivity[nU3.Connectivity]
        Data[(Oracle DB)]
        LocalDB[(SQLite)]
    end

    subgraph "User"
        Doctor[Doctor]
        Nurse[Nurse]
        Admin[System Admin]
    end

    Doctor -->|Orders| Shell
    Nurse -->|Charts| Shell
    Admin -->|Config| Shell

    Shell -->|User Actions| Core
    Core -->|Data Requests| Data
    Core -->|Config| LocalDB

    Connectivity -->|SQL Queries| Data
    Connectivity -->|File Transfer| LIS
    Connectivity -->|Log Upload| INSURANCE

    HL7 -->|ADT Messages| Connectivity
    FHIR -->|Resources| Connectivity
    DICOM -->|Images| Connectivity
```

**Legend:**
- **Solid arrows:** Direct communication
- **Dashed arrows:** API/Web Service calls
- **Database:** SQLite (local) / Oracle (server)

---

## Container Diagram

Shows high-level architectural containers and their relationships.

```mermaid
graph TB
    subgraph "nU3.Framework Platform"
        direction TB

        subgraph "Client Layer"
            Client[nU3.Client<br/>(WinForms Application)]
        end

        subgraph "Infrastructure Layer"
            Bootstrapper[nU3.Bootstrapper<br/>(Deployment & Loading)]
            Connectivity[nU3.Connectivity<br/>(HTTP Clients)]
        end

        subgraph "Core Layer"
            Core[nU3.Core<br/>(Base Classes & Interfaces)]
            Security[nU3.Security<br/>(JWT & RBAC)]
        end

        subgraph "Application Layer"
            Shell[nU3.Shell<br/>(MDI Container)]
            Modules[nU3.Modules.<br/>(Business Modules)]
        end

        subgraph "Data Layer"
            LocalDB[(SQLite<br/>Configuration)]
            OracleDB[(Oracle<br/>Business Data)]
        end

        Client --> Bootstrapper
        Client --> Shell
        Client --> Connectivity

        Bootstrapper --> Core
        Connectivity --> Core
        Connectivity --> OracleDB

        Core --> Shell
        Shell --> Modules

        Core --> LocalDB
        Core --> Security
        Shell --> Security
    end

    style Client fill:#e1f5ff
    style Shell fill:#fff4e1
    style Modules fill:#ffe1e1
    style OracleDB fill:#e1ffe1
```

---

## Component Diagram

Detailed component view showing internal architecture.

```mermaid
classDiagram
    class BaseWorkControl {
        <<interface>>
        +string ScreenId
        +string ScreenTitle
        +OnActivated()
        +OnDeactivated()
        +OnBeforeClose()
        +RegisterDisposable(IDisposable)
        +ReleaseResources()
        +HasPermission(permission)
    }

    class nU3ProgramInfo {
        +Type ControlType
        +string DisplayTitle
        +string ProgramId
        +string ModuleCategory
    }

    class EventAggregator {
        +Publish~T~(payload)
        +Subscribe~T~(Action~T~)
        +Unsubscribe~T~(Action~T~)
    }

    class UserSession {
        +Authenticate(username, password)
        +ValidateToken(token)
        +GetCurrentUser() User
        +GetPermissions() List~Permission~
    }

    class ConnectivityManager {
        +GetDbClient() HttpDBAccessClient
        +GetFileClient() HttpFileTransferClient
        +GetLogClient() HttpLogUploadClient
    }

    class ModuleLoader {
        +LoadModule(moduleId) Assembly
        +UnloadModule(moduleId)
        +GetLoadedModules() List~ModuleInfo~
    }

    BaseWorkControl <|-- BaseWorkForm
    BaseWorkControl <|-- PatientListControl

    nU3ProgramInfo --> BaseWorkControl : defines

    EventAggregator ..> EventAggregator : event bus

    UserSession --> EventAggregator : publish authentication events
    UserSession --> ConnectivityManager : orchestrates

    ConnectivityManager --> ConnectivityManager : manages multiple HTTP clients
    ModuleLoader --> EventAggregator : notify module load/unload
    ModuleLoader --> BaseWorkControl : instantiate modules
```

---

## Sequence Diagrams

### Module Loading Sequence

Shows how a user navigates to a module and it's loaded dynamically.

```mermaid
sequenceDiagram
    participant User as 👤 User
    participant Shell as nU3.Shell
    participant Menu as Menu System
    participant Loader as ModuleLoader
    participant DI as DI Container
    participant Module as Module Instance
    participant DB as SQLite

    User->>Shell: Click menu item
    Shell->>Menu: GetMenuItem(menuId)
    Menu-->>Shell: Return PROG_ID

    Shell->>DB: Query SYS_MENU for PROG_ID
    DB-->>Shell: Return module metadata

    Shell->>DB: Query SYS_MODULE_VER for version
    DB-->>Shell: Return version and file hash

    alt Module not loaded
        Shell->>Loader: LoadModule(moduleId)
        Loader->>DI: CreateModule(assembly)
        DI-->>Module: Return BaseWorkControl instance
        Module->>Module: OnActivated()
        Module-->>Shell: ModuleActivatedEvent
        Shell->>User: Show module in MDI tab
    else Module already loaded
        Shell->>Module: Show existing instance
        Module->>Module: OnActivated()
    end

    Note over Module: Business logic execution...
```

### Event-Driven Communication

Shows inter-module communication using the Event Aggregator.

```mermaid
sequenceDiagram
    participant ModA as Module A<br/>(Patient List)
    participant EventBus as EventAggregator
    participant ModB as Module B<br/>(Lab Orders)
    participant ModC as Module C<br/>(Radiology)
    participant ModD as Module D<br/>(Pharmacy)

    ModA->>EventBus: Publish PatientSelectedEvent{patient}
    EventBus->>EventBus: Route to subscribers

    EventBus->>ModB: OnPatientSelected(patient)
    ModB->>ModB: Load lab orders for patient

    EventBus->>ModC: OnPatientSelected(patient)
    ModC->>ModC: Load radiology images for patient

    EventBus->>ModD: OnPatientSelected(patient)
    ModD->>ModD: Check medication interactions

    Note over ModB,ModD: All modules react<br/>to patient selection automatically
```

### Update Deployment Flow

Shows how the framework updates modules without restart.

```mermaid
sequenceDiagram
    participant User as 👤 User
    participant Shell as nU3.Shell
    participant Bootstrap as Bootstrapper
    participant Server as Server Repository
    participant Cache as Staging Cache
    participant SQLite as SQLite DB

    User->>Shell: Application starts
    Shell->>Bootstrap: Check for updates

    Bootstrap->>Server: GET /api/modules/versions
    Server-->>Bootstrap: Return latest versions

    Bootstrap->>SQLite: Query SYS_MODULE_VER
    SQLite-->>Bootstrap: Return local versions

    loop For each module
        Bootstrap->>Bootstrap: Compare versions
        alt New version available
            Bootstrap->>Server: Download new DLL
            Server-->>Bootstrap: Return DLL file

            Bootstrap->>Cache: Save to Staging\
            Note right of Cache: Verify SHA256 hash

            Bootstrap->>SQLite: Update version to new version
            Bootstrap->>Shell: Notify update complete
        end
    end

    Shell->>User: "Updates applied successfully"
    Note over Shell: Modules will load new<br/>version on next use
```

---

## Deployment Diagram

Shows physical deployment architecture.

```mermaid
graph TB
    subgraph "🏥 Hospital Network"
        direction LR

        subgraph "Workstations (Multiple)"
            WS1[WinForms Workstation 1]
            WS2[WinForms Workstation 2]
            WS3[WinForms Workstation 3]
        end

        subgraph "File Server"
            FS[Shared File Server<br/>(Modules, Configs)]
        end

        subgraph "Application Server"
            AppServer[nU3.Server.Host<br/>(ASP.NET Core API)]
        end

        subgraph "Database Server"
            OracleDB[(Oracle Database)]
            SQLite[(SQLite Local Config)]
        end

        subgraph "Archive Server"
            Archive[Archive Server<br/>(Module Archives)]
        end
    end

    WS1 --> FS
    WS2 --> FS
    WS3 --> FS

    WS1 -.->|HTTPS| AppServer
    WS2 -.->|HTTPS| AppServer
    WS3 -.->|HTTPS| AppServer

    AppServer --> OracleDB
    AppServer --> Archive
```

---

## Data Flow Diagrams

### Authentication Flow

Shows how a user logs in and gets authenticated.

```mermaid
sequenceDiagram
    participant UI as UI Layer
    participant Auth as UserSession
    participant DB as Oracle DB
    participant JWT as JWT Service
    participant TokenStore as Token Store

    UI->>Auth: Authenticate(username, password)

    Auth->>DB: SELECT * FROM USERS WHERE USERNAME=?

    alt User exists
        DB-->>Auth: Return user data
        Auth->>Auth: Verify password hash
        Auth->>JWT: GenerateToken(user)
        JWT-->>Auth: Return JWT token

        Auth->>TokenStore: Store token (refresh token)
        Auth->>Auth: Set session cookie

        Auth-->>UI: Return JWT + user info
    else User not found
        DB-->>Auth: Empty result
        Auth-->>UI: throw AuthenticationException
    end
```

### CRUD Operation Flow

Shows standard data access pattern.

```mermaid
sequenceDiagram
    participant UI as UI Layer<br/>(BaseWorkControl)
    participant Service as Service Agent<br/>(IPatientServiceAgent)
    participant DAL as Data Access Layer<br/>(HttpDBAccessClient)
    participant Server as Server API
    participant DB as Oracle DB

    UI->>UI: User clicks "Save" button

    UI->>Service: GetPatientByIdAsync(id)
    Service->>DAL: GET /api/patients/{id}
    DAL->>Server: HTTP GET
    Server->>DB: SELECT * FROM PATIENTS WHERE ID=?
    DB-->>Server: Return patient data
    Server-->>DAL: JSON response
    DAL-->>Service: PagedResultDto<PatientListDto>
    Service-->>UI: Return patient details

    UI->>Service: UpdatePatientAsync(patientDto)
    Service->>DAL: POST /api/patients/{id}
    DAL->>Server: HTTP POST
    Server->>DB: UPDATE PATIENTS SET ...
    DB-->>Server: Affected rows
    Server-->>DAL: HTTP 200 OK
    DAL-->>Service: BaseResponseDto
    Service-->>UI: Success response
```

---

## Module Loading Flow

Detailed view of how the module loader discovers and loads modules.

```mermaid
graph TD
    Start[Start: Application Launch] --> Scan[Scan for nU3ProgramInfo Attributes]

    Scan --> Found[Attributes Found]

    Found --> Filter{Filter by Category}

    Filter -->|Match| Load[Load Assembly via AssemblyLoadContext]
    Filter -->|No Match| Skip[Skip Module]

    Load --> Verify{Verify Integrity}
    Verify -->|SHA256 Mismatch| Reject[Reject Module<br/>Corrupt file]
    Verify -->|OK| Cache[Cache Assembly]

    Cache --> Resolve{Resolve Dependencies}
    Resolve -->|Dependencies OK| Register[Register with DI Container]
    Resolve -->|Missing Deps| Fail[Load Failed<br/>Missing dependency]

    Register --> Instantiate[Instantiate Module]
    Instantiate --> Lifecycle[Call OnActivated]
    Lifecycle --> Complete[Complete: Module Ready]
```

---

## Update Deployment Flow

Detailed view of how modules are updated.

```mermaid
graph TD
    Start[Start: Update Check] --> Query[Query Server for Latest Versions]

    Query --> Compare{Version Check}

    Compare -->|Up to Date| Done[Module Up to Date]

    Compare -->|New Version| Download[Download New DLL]
    Download --> Save[Save to Staging Cache]
    Save --> Verify{Verify SHA256 Hash}

    Verify -->|Mismatch| Error[Error: File Corrupt<br/>Re-download]
    Verify -->|OK| Swap[Swap with Running Version]

    Swap --> UpdateDB[Update SQLite Version Table]
    UpdateDB --> Notify[Notify Application]
    Notify --> Check{Check Dependencies}
    Check -->|OK| Success[Update Complete]
    Check -->|Fail| Rollback[Rollback Staging]
    Rollback --> Error

    Done --> End[End]
    Success --> End
    Error --> End
```

---

## Component Relationships

### Dependency Graph

Shows module dependencies.

```mermaid
graph LR
    subgraph "Core Infrastructure"
        Core[Core]
        Shell[Shell]
    end

    subgraph "Business Modules"
        Mod1[Patient Management]
        Mod2[Lab Orders]
        Mod3[Radiology]
        Mod4[Pharmacy]
        Mod5[EMR Documentation]
    end

    Core --> Mod1
    Core --> Mod2
    Core --> Mod3
    Core --> Mod4
    Core --> Mod5

    Mod1 --> Mod2
    Mod2 --> Mod3
    Mod3 --> Mod4

    Shell -.->|Event Bus| Mod1
    Shell -.->|Event Bus| Mod2
    Shell -.->|Event Bus| Mod3

    style Core fill:#ff9999
    style Shell fill:#99ccff
    style Mod1 fill:#99ff99
    style Mod2 fill:#99ff99
    style Mod3 fill:#99ff99
    style Mod4 fill:#99ff99
    style Mod5 fill:#99ff99
```

---

## Architecture Layers

### 5-Layer Architecture Diagram

```mermaid
graph TB
    subgraph "Layer 5: Data Layer"
        DataLayer[(Oracle Database)]
    end

    subgraph "Layer 4: Connectivity Layer"
        Connect[nU3.Connectivity<br/>HTTP Clients<br/>Connection Pooling]
    end

    subgraph "Layer 3: Shell Layer"
        Shell[nU3.Shell<br/>MDI Container<br/>Module Loading<br/>Event Aggregator]
    end

    subgraph "Layer 2: Core Layer"
        Core[nU3.Core<br/>BaseWorkControl<br/>nU3ProgramInfo<br/>EventAggregator]
    end

    subgraph "Layer 1: Bootstrapper Layer"
        Boot[nU3.Bootstrapper<br/>Deployment<br/>Assembly Loading<br/>Version Control]
    end

    Boot --> Core
    Core --> Shell
    Shell --> Connect
    Connect --> DataLayer
```

---

## Medical Standards Integration

### HL7 ADT Message Flow

```mermaid
sequenceDiagram
    participant HIS as Hospital Information System
    participant HL7[HL7 Gateway]
    participant Server as Server API
    participant DB as Oracle DB
    participant Patient as Patient Module

    HIS->>HL7: ADT^A01 (Patient Admission)
    HL7->>HL7: Parse message

    HL7->>Server: POST /api/hl7/adt
    Server->>Server: Validate message structure
    Server->>DB: INSERT INTO PATIENTS (...)
    DB-->>Server: Success
    Server-->>HL7: HTTP 200 OK
    HL7-->>HIS: ACK (Acknowledge)

    Server->>Patient: Publish PatientCreatedEvent
    Patient->>Patient: Load patient data
    Patient-->>Server: Patient loaded
```

### FHIR Resource Mapping

```mermaid
graph LR
    subgraph "External Systems"
        FHIR[FHIR API]
        DICOM[DICOM Server]
    end

    subgraph "nU3 Framework"
        Adapter[Resource Adapter<br/>Service]
        Core[Core Layer]
    end

    subgraph "Local System"
        Oracle[(Oracle DB)]
        Modules[Business Modules]
    end

    FHIR -->|GET /Patient| Adapter
    Adapter -->|Map to Entity| Core
    Core --> Oracle

    DICOM -->|GET images| Adapter
    Adapter -->|Extract metadata| Core
    Core --> Modules
```

---

## Security Architecture

### JWT Token Flow

```mermaid
sequenceDiagram
    participant UI as UI Layer
    participant Auth as Auth Service
    participant TokenGen as Token Generator
    participant DB as Database

    UI->>Auth: POST /api/auth/login
    Auth->>DB: Verify credentials
    DB-->>Auth: User data

    Auth->>TokenGen: CreateToken(user)
    TokenGen->>TokenGen: Sign with private key
    TokenGen-->>Auth: JWT token + refresh token

    Auth-->>UI: Return tokens
    UI->>UI: Store JWT in memory

    Note over UI: Make API calls

    UI->>API: GET /api/patients<br/>Header: Authorization: Bearer <JWT>
    API->>Auth: ValidateToken(token)
    Auth->>TokenGen: Verify signature
    TokenGen-->>Auth: Valid
    Auth-->>API: User claims
    API-->>UI: Data
```

### RBAC Permission Check

```mermaid
graph TB
    Start[Start: User Action] --> GetToken[Get JWT Token]

    GetToken --> Decode[Decode JWT Payload]
    Decode --> Extract[Extract permissions]
    Extract --> Map[Map permissions to roles]

    Map --> Check{Permission Check}

    Check -->|Has Permission| Allow[Allow Access]
    Check -->|No Permission| Deny[Deny Access]
    Check -->|No Token| RequireLogin[Require Login]

    Allow --> Execute[Execute Action]
    Deny --> Throw[Throw UnauthorizedException]
    RequireLogin --> Redirect[Redirect to Login]

    Execute --> Audit[Audit Log Entry]
    Throw --> Audit

    Audit --> End[End]
    Redirect --> End
```

---

## Appendix: Mermaid Diagram Best Practices

### Rendering Options

1. **GitHub:**
   - Automatically renders Mermaid diagrams
   - No special syntax needed

2. **VS Code:**
   - Install "Markdown Preview Mermaid Support" extension
   - Press `Ctrl+Shift+V` to preview

3. **Online:**
   - [Mermaid Live Editor](https://mermaid.live/)
   - Copy-paste diagrams to render

### Diagram Types Used

| Type | Purpose | Examples |
|------|---------|----------|
| **graph TB/LR** | High-level architecture | System Context, Deployment |
| **sequenceDiagram** | Workflow interactions | Module Loading, Event Flow |
| **classDiagram** | Class relationships | Component Structure |
| **stateDiagram** | State transitions | Login flow, Update deployment |
| **ERD** | Database schema | Module structure |

---

**Document Version:** 1.0
**Last Updated:** February 2026
**Next Review:** April 2026 (Phase 2 completion)
