# Client → .NET Core → SQLite Architecture Diagram

## Overview
```
┌─────────────────────────────────────────────────────────────────────────────┐
│                            CLIENT (UI Layer)                                │
│                                                                              │
│  ┌─────────────────┐      ┌─────────────────┐      ┌─────────────────┐    │
│  │   WinForms App  │ ←──→ │  Web Browser    │ ←──→ │  Other Clients  │    │
│  │   (nU3.MainShell)│      │   (Web UI)      │      │ (API Clients)   │    │
│  └─────────────────┘      └─────────────────┘      └─────────────────┘    │
└─────────────────────────────────────────────────────────────────────────────┘
                                      ↓
┌─────────────────────────────────────────────────────────────────────────────┐
│                        .NET CORE (Application Layer)                       │
│                                                                              │
│  ┌──────────────────────────────────────────────────────────────────────┐  │
│  │                     ASP.NET Core API                                  │  │
│  │                                                                       │  │
│  │  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐               │  │
│  │  │ Controllers  │  │   Middleware │  │  Services    │               │  │
│  │  │ (REST API)   │  │  (Auth/Logs)  │  │  (BL Logic)  │               │  │
│  │  └──────────────┘  └──────────────┘  └──────────────┘               │  │
│  └──────────────────────────────────────────────────────────────────────┘  │
│                                                                              │
│  ┌──────────────────────────────────────────────────────────────────────┐  │
│  │                    nU3.Bootstrapper (Dependency Injection)            │  │
│  │                  ┌──────────────────────────────────────────────┐   │  │
│  │                  │   IServiceProvider (Container)                │   │  │
│  │                  └──────────────────────────────────────────────┘   │  │
│  └──────────────────────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────────────────────┘
                                      ↓
┌─────────────────────────────────────────────────────────────────────────────┐
│                           DATA ACCESS LAYER                                 │
│                                                                              │
│  ┌──────────────────────────────────────────────────────────────────────┐  │
│  │                      nU3.Data (EF Core)                               │  │
│  │                                                                       │  │
│  │  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐               │  │
│  │  │ DbContext    │  │  Repositories │  │   Migrations │               │  │
│  │  │ (SQLite)     │  │  (Async)      │  │   (Schema)   │               │  │
│  │  └──────────────┘  └──────────────┘  └──────────────┘               │  │
│  └──────────────────────────────────────────────────────────────────────┘  │
│                                                                              │
│  ┌──────────────────────────────────────────────────────────────────────┐  │
│  │                    nU3.Connectivity (HTTP)                            │  │
│  │                   (External APIs, File Storage)                       │  │
│  └──────────────────────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────────────────────┘
                                      ↓
┌─────────────────────────────────────────────────────────────────────────────┐
│                           SQLITE DATABASE                                   │
│                                                                              │
│  ┌──────────────────────────────────────────────────────────────────────┐  │
│  │                    SQLite File (.db)                                  │  │
│  │                                                                       │  │
│  │  Tables:                                                              │  │
│  │  • Patients, Components, Orders, Logs...                              │  │
│  │  • indexed columns for performance                                     │  │
│  │  • constraints for data integrity                                     │  │
│  └──────────────────────────────────────────────────────────────────────┘  │
│                                                                              │
│  Files:                                                                     │
│  • nU3.db (Main Database)                                                   │
│  • nU3.db-shm (Shared Memory)                                               │
│  • nU3.db-wal (Write-Ahead Log)                                             │
└─────────────────────────────────────────────────────────────────────────────┘
```

## Data Flow Example: Patient Registration

```
┌──────────────┐
│   Client UI  │  1. User clicks "Register Patient"
│              │
└──────┬───────┘
       │ HTTP POST /api/patients
       ↓
┌──────────────────────────────┐
│  ASP.NET Core Controller     │  2. Validates request
│                              │     Creates PatientDto
└──────┬───────────────────────┘
       │
       ↓
┌──────────────────────────────┐
│  Business Service (BL)       │  3. Processes business logic
│                              │     Generates patient ID
└──────┬───────────────────────┘
       │
       ↓
┌──────────────────────────────┐
│  Repository Pattern          │  4. Uses DbContext
│  (IRepository<Patient>)      │
└──────┬───────────────────────┘
       │
       ↓
┌──────────────────────────────┐
│  Entity Framework Core       │  5. Maps DTO → Entity
│  (SQLite)                    │
└──────┬───────────────────────┘
       │
       ↓
┌──────────────────────────────┐
│  SQLite Database             │  6. INSERT INTO Patients
│  (nU3.db)                     │     ✓ Execution
└──────────────────────────────┘
       │
       ↓
┌──────────────┐
│   Client UI  │  7. Returns response: { "id": 12345, ... }
│              │     Updates UI with result
└──────────────┘
```

## Layer Responsibilities

### 1. Client Layer
- **WinForms** (nU3.MainShell): Desktop application
- **Web** (ASP.NET Core MVC): Browser-based UI
- **Responsibility**: User interaction, data visualization, API calls

### 2. Application Layer (.NET Core)
- **Controllers**: HTTP endpoint definitions
- **Middleware**: Authentication, logging, error handling
- **Services**: Business logic implementation
- **Repository Pattern**: Data access abstraction
- **Dependency Injection**: Service management

### 3. Data Access Layer
- **DbContext (SQLite)**: ORM, schema management, entity tracking
- **Repositories**: Data access methods (CRUD)
- **Migrations**: Database schema versioning
- **External APIs**: File storage, external service calls

### 4. Data Layer (SQLite)
- **File-based**: Single database file (nU3.db)
- **Embedded**: Runs inside .NET process (no separate server)
- **ACID**: Transaction support
- **Schema**: Tables, indexes, constraints

## Key Technologies

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                         Technology Stack                                     │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                              │
│  Client UI                                                                  │
│  ├── WinForms (Desktop)                                                     │
│  ├── ASP.NET Core MVC (Web)                                                 │
│  └── HttpClient (API Clients)                                               │
│                                                                              │
│  .NET Core API                                                              │
│  ├── ASP.NET Core Web API (Controllers)                                     │
│  ├── Middleware Pipeline                                                    │
│  ├── Dependency Injection (Microsoft.Extensions.DependencyInjection)       │
│  └── Validation (Data Annotations)                                          │
│                                                                              │
│  Data Access                                                                │
│  ├── Entity Framework Core (ORM)                                            │
│  ├── SQLite Provider (Microsoft.EntityFrameworkCore.Sqlite)                │
│  ├── Async/Await Pattern                                                    │
│  └── Repository Pattern                                                     │
│                                                                              │
│  Database                                                                    │
│  ├── SQLite (.db file)                                                     │
│  ├── JSON File Storage (Optional)                                           │
│  └── Transaction Management                                                │
└─────────────────────────────────────────────────────────────────────────────┘
```

## Performance Characteristics

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                      Performance & Scalability                               │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                              │
│  Client → .NET Core API                                                      │
│  ┌──────────────────────────────────────────────────────────────────────┐  │
│  │ • Fast HTTP/HTTPS communication                                      │  │
│  │ • ASP.NET Core Kestrel high-performance server                       │  │
│  │ • Connection pooling (HttpClient)                                    │  │
│  │ • JSON serialization (System.Text.Json)                              │  │
│  └──────────────────────────────────────────────────────────────────────┘  │
│                                                                              │
│  .NET Core → SQLite                                                         │
│  ┌──────────────────────────────────────────────────────────────────────┐  │
│  │ • Entity Framework Core optimized queries                             │  │
│  │ • Asynchronous I/O (non-blocking)                                     │  │
│  │ • Connection reuse (DbContext)                                        │  │
│  │ • Lazy loading/Explicit loading for related entities                  │  │
│  └──────────────────────────────────────────────────────────────────────┘  │
│                                                                              │
│  SQLite Performance                                                         │
│  ┌──────────────────────────────────────────────────────────────────────┐  │
│  │ • Single-process database (no external server)                        │  │
│  │ • File I/O caching (OS-level)                                         │  │
│  │ • WAL mode for concurrent reads                                       │  │
│  │ • Indexes on frequently queried columns                               │  │
│  └──────────────────────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────────────────────┘
```

## Security Considerations

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                           Security Architecture                               │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                              │
│  1. Authentication (ASP.NET Core Identity)                                  │
│     ├── JWT Tokens                                                          │
│     ├── Cookie Authentication                                                │
│     └── Role-Based Access Control (RBAC)                                     │
│                                                                              │
│  2. Authorization (Middleware)                                               │
│     ├── Policy-based authorization                                           │
│     ├── [Authorize] attributes                                               │
│     └── Resource-based claims                                               │
│                                                                              │
│  3. Data Protection                                                         │
│     ├── Encrypt sensitive fields in SQLite (App.config)                     │
│     ├── Parameterized queries (SQL Injection prevention)                    │
│     ├── HTTPS/TLS for client communication                                  │
│     └── Secure secrets storage (User Secrets/Environment variables)         │
│                                                                              │
│  4. Audit Logging                                                            │
│     ├── Log all CRUD operations                                             │
│     ├── Track user actions                                                  │
│     └── HIPAA compliance for patient data                                  │
└─────────────────────────────────────────────────────────────────────────────┘
```
