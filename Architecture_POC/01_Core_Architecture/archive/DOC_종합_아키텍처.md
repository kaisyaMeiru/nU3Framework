  # Architecture Comprehensive Design Document (nU3.Framework)

  **Version:** 1.0 (Phase 1 Complete)
  **Date:** 2026-01-28
  **Context:** .NET 8, WinForms (DevExpress), Plugin Architecture

  ## 1. System Overview

  The **nU3.Framework** is a modular, metadata-driven WinForms application designed for high-scalability Hospital Information Systems. It decouples the "Shell" (Container) from the "Modules" (Business Logic) using a strict plugin architecture.

  ### Key Architectural Patterns
  *   **Plugin Architecture:** Modules are loaded dynamically at runtime.
  *   **Staged Deployment:** Updates are staged in a cache to verify integrity before being deployed to the runtime folder, preventing DLL locking.
  *   **Metadata-Driven Discovery:** Features are discovered via `[nU3ProgramInfo]` attributes, not hardcoded configuration.
  *   **Local-First Configuration:** Client configuration (Menus, Module Versions) is stored in a local SQLite database for performance and resilience.
  *   **Centralized Connectivity:** Modules interact with infrastructure (DB, API, File) through a unified `ConnectivityManager`.

  ---

  ## 2. Logical Architecture Layers

  ### Layer 1: The Bootstrapper (`nU3.Bootstrapper`)
  The entry point of the application. It runs *before* the main UI.
  *   **Responsibility:** Deployment & Integrity.
  *   **Process:**
    1.  **Connect:** Checks `ServerStorage` (Simulated or HTTP) for new module versions.
    2.  **Compare:** Checks `SYS_MODULE_VER` in local SQLite.
    3.  **Download:** Copies new DLLs to Staging Area (`%AppData%\nU3.Framework\Cache`).
    4.  **Install:** Syncs Staging Area to Runtime Area (`[AppDir]\Modules`).
    5.  **Launch:** Starts `nU3.Shell.exe`.

  ### Layer 2: The Core (`nU3.Core` & `nU3.Data`)
  The shared contract that binding everything together.
  *   **`IBaseWorkControl`**: Core interface for all business screens.
  *   **`nU3ProgramInfoAttribute`**: Metadata attribute for screen discovery and categorization.
  *   **`UserSession`**: Global singleton for Authentication state and JWT management.
  *   **`LocalDatabaseManager`**: Manages the SQLite schema (`SYS_MODULE_MST`, `SYS_MENU`, etc.).
  *   **`EventAggregator`**: Pub/Sub system for decoupled communication.

  ### Layer 3: The Shell (`nU3.Shell` & `nU3.MainShell`)
  The MDI Container.
  *   **Dependency Injection:** Uses `Microsoft.Extensions.DependencyInjection` for service management.
  *   **Dynamic Menu:** Reads `SYS_MENU` from SQLite and builds the DevExpress NavBar.
  *   **Lazy Loading:** Uses `AssemblyLoadContext` to load Module DLLs on demand.

  ### Layer 4: Connectivity (`nU3.Connectivity` & `ConnectivityManager`)
  The bridge to the outside world.
  *   **`ConnectivityManager`**: A singleton in `nU3.Core` that delegates requests to `nU3.Connectivity` implementations.
  *   **`IDBAccessService`**: Unified interface for SQL operations (Dapper/DataTable).
  *   **`IFileTransferService`**: Interface for HTTP/FTP file operations.

  ### Layer 5: Modules (`nU3.Modules.*`)
  Independent business units.
  *   **Structure:** `Modules/[Category]/[SubSystem]/[Name].dll`
  *   **Isolation:** Modules inherit from `BaseWorkControl` and communicate via `EventBus`.

  ---

  ## 3. Data Model (Local SQLite)

  ### 3.1 Module Master (`SYS_MODULE_MST`)
  Tracks installed modules.
  *   `MODULE_ID`: Unique ID (e.g., `MOD_EMR_CLINIC`).
  *   `CATEGORY`: Logical grouping (e.g., `EMR`).
  *   `FILE_NAME`: Physical DLL name.

  ### 3.2 Program Master (`SYS_PROG_MST`)
  Tracks individual screens within modules. Populated by scanning `[nU3ProgramInfo]`.
  *   `PROG_ID`: Logical Screen ID.
  *   `CLASS_NAME`: Full .NET Type Name.
  *   `AUTH_LEVEL`: Required permission level.

  ### 3.3 Menu Master (`SYS_MENU`)
  Hierarchical menu structure.
  *   `MENU_ID`, `PARENT_ID`: Tree structure.
  *   `PROG_ID`: Links a menu leaf to a Program.

  ---

  ## 4. Execution Flow (Run Program)

  1.  **User Click:** User clicks a Node in `NavBarControl`.
  2.  **Lookup:** Shell finds `PROG_ID` associated with the menu.
  3.  **Resolution:** Shell looks up `CLASS_NAME` and `MODULE_ID` in `SYS_PROG_MST`.
  4.  **Load:**
    *   If Module DLL is not loaded, load it via `AssemblyLoadContext`.
  5.  **Instantiate:** Create instance of `CLASS_NAME`.
  6.  **Activate:** 
    *   `BaseWorkControl.OnActivated()` is called.
    *   `ModuleActivatedEvent` is published to the `EventAggregator`.
  7.  **Display:** Add to `DocumentManager` (Tabbed View).

  ---

  ## 5. Security & Isolation

  *   **File Locking:** Solved by `Bootstrapper` Staging strategy.
  *   **Crash Isolation:** (Planned) Unhandled exceptions in a module are caught by the Shell to prevent full crash.
  *   **Versioning:** Strict Hash (SHA256) checking ensures clients only run authorized binaries.
