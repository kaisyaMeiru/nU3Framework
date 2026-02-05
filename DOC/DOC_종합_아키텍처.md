  # Architecture Comprehensive Design Document (nU3.Framework)

  **Version:** 1.0 (Phase 1 Complete)
  **Date:** 2026-01-28
  **Context:** .NET 8, WinForms (DevExpress), Plugin Architecture

  ## 1. System Overview

  The **nU3.Framework** is a modular, metadata-driven WinForms application designed for high-scalability Hospital Information Systems. It decouples the "Shell" (Container) from the "Modules" (Business Logic) using a strict plugin architecture.

  ### Key Architectural Patterns
  *   **Plugin Architecture:** Modules are loaded dynamically at runtime.
  *   **Staged Deployment:** Updates are staged in a cache to verify integrity before being deployed to the runtime folder, preventing DLL locking.
  *   **Metadata-Driven Discovery:** Features are discovered via `[ScreenInfo]` attributes, not hardcoded configuration.
  *   **Local-First Configuration:** Client configuration (Menus, Module Versions) is stored in a local SQLite database for performance and resilience.

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
  *   **`IModule`:** Interface for Module Lifecycle (Init/Dispose).
  *   **`IWorkForm`:** Interface for all business screens.
  *   **`UserSession`:** Global singleton for Authentication state.
  *   **`LocalDatabaseManager`:** Manages the SQLite schema (`SYS_MODULE_MST`, `SYS_MENU`, etc.).

  ### Layer 3: The Shell (`nU3.Shell`)
  The MDI Container.
  *   **Dependency Injection:** Uses `Microsoft.Extensions.DependencyInjection` to provide services (`IDBAccessService`, `ILogger`).
  *   **Dynamic Menu:** Reads `SYS_MENU` from SQLite and builds the DevExpress NavBar.
  *   **Lazy Loading:** Does not load Module DLLs until a menu item is clicked.

  ### Layer 4: Connectivity (`nU3.Connectivity`)
  The bridge to the outside world.
  *   **`IDBAccessService`:** Unified interface for Oracle/SQL operations. Supports Async/Sync.
  *   **`IFileTransferService`:** Interface for FTP/HTTP file operations.

  ### Layer 5: Modules (`nU3.Modules.*`)
  Independent business units.
  *   **Structure:** `Modules\[Category]\[SubSystem]\[Name].dll`
  *   **Isolation:** Modules do not reference each other directly. They communicate via `EventAggregator`.

  ---

  ## 3. Data Model (Local SQLite)

  ### 3.1 Module Master (`SYS_MODULE_MST`)
  Tracks installed modules.
  *   `MODULE_ID`: Unique ID (e.g., `MOD_EMR_CLINIC`).
  *   `CATEGORY`: Logical grouping (e.g., `EMR`).
  *   `FILE_NAME`: Physical DLL name.

  ### 3.2 Program Master (`SYS_PROG_MST`)
  Tracks individual screens within modules. Populated by scanning `[ScreenInfo]`.
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
    *   Scan for `IModule` and call `Initialize()`.
  5.  **Instantiate:** Create instance of `CLASS_NAME` using DI Container.
  6.  **Display:** Add to `DocumentManager` (Tabbed View).

  ---

  ## 5. Security & Isolation

  *   **File Locking:** Solved by `Bootstrapper` Staging strategy.
  *   **Crash Isolation:** (Planned) Unhandled exceptions in a module are caught by the Shell to prevent full crash.
  *   **Versioning:** Strict Hash (SHA256) checking ensures clients only run authorized binaries.
