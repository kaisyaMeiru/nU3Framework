# /sg:workflow [Phase1] Basic Architecture & Module System

  Workflow Strategy: Systematic
  Persona: Architect (Primary), Backend, Frontend
  Status: **COMPLETED**
  Context: Enterprise WinForms (DevExpress) Framework with Plugin Architecture

  This workflow establishes the Module (Physical) → Program (Logical) → Menu (Visual) hierarchy and the Shadow Copy deployment mechanism.

  ---

  ## Implementation Roadmap (Status Report)

  ### Phase 1.1: Core Contracts & Metadata (The Foundation)
  Objective: Define the standard interfaces and attributes that all business modules must follow.
  Status: **Done**

   - [x] Define `Core.dll` Assembly:
       - [x] Create IModule interface (Lifecycle hooks: Initialize, Dispose).
       - [x] Create IWorkForm interface (Base contract for all MDI children).
       - [x] Implement ScreenInfoAttribute class (as specified in requirements).
       - [x] Define UserSession singleton for global state management.

   - [x] Implement Base Forms (`GMIS.Framework`):
       - [x] BaseWorkControl: Inherits DevExpress.XtraEditors.XtraUserControl + IWorkForm.
       - [x] BasePopupForm: Inherits DevExpress.XtraEditors.XtraForm.
       - [x] Add AuthLevel check logic in BaseWorkControl.OnLoad.

  ### Phase 1.2: Database & Deployment API (The Supply Chain)
  Objective: Set up the schema to manage Modules, Versions, and Menus.
  Status: **Done** (Implemented via `nU3.Data` and SQLite)

   - [x] Database Schema Implementation (`LocalDatabaseManager.cs`):
       - [x] Create SYS_MODULE_MST: Module ID, Name, Fixed Filename.
       - [x] Create SYS_MODULE_VER: Versioning, Hash (SHA256), Path.
       - [x] Create SYS_PROG_MST: Auto-generated from [ScreenInfo] (Maps ProgID ↔ ClassName).
       - [x] Create SYS_MENU: Hierarchy, connecting MenuID to ProgID.

   - [x] Deployment Service (`nU3.Connectivity` & `Bootstrapper`):
       - [x] Local File Simulation implemented for Server Storage.
       - [x] Hashing and Version Checking implemented.

  ### Phase 1.3: Module Management & Loader Engine (The Engine)
  Objective: The logic to compare local vs. server versions, shadow-copy, and load DLLs.
  Status: **Done**

   - [x] Bootstrapper (Launcher):
       - [x] Implement "Version Check": Compare local DB vs Server Storage.
       - [x] Implement "Downloader": Download changed DLLs to `AppData/nU3.Framework/Cache` (Staging).
       - [x] Implement "Runtime Sync": Copy from Cache to `Modules/` folder before Shell execution.

   - [x] Reflection & Registration Service (`nU3.Shell`):
       - [x] Implement AssemblyLoader: Load DLL from current context.
       - [x] Implement MetadataScanner: Scan loaded DLLs for [ScreenInfo] attributes.
       - [x] Implement ProgramRegistry: Map ScreenId (String) → Type (Class).

  ### Phase 1.4: Main Shell UI & Navigation (The Container)
  Objective: The MDI container that hosts the loaded forms.
  Status: **Done**

   - [x] MainForm Construction:
       - [x] Setup RibbonControl (Top) and StatusBar (Bottom).
       - [x] Configure DocumentManager (View: TabbedView) for MDI behavior.
       - [x] Setup NavBarControl (Left) for Navigation Tree.

   - [x] Menu Building Logic:
       - [x] Fetch SYS_MENU data from SQLite.
       - [x] Build recursive Tree Nodes in NavBarControl.
       - [x] Bind Click event to MenuExecutor.

  ### Phase 1.5: Developer Tools (The Enabler)
  Objective: Tool for developers to register and upload their modules.
  Status: **Done**

   - [x] Module Builder/Uploader Tool (`nU3.Tools.Deployer`):
       - [x] Register Modules & Categories.
       - [x] Upload Versions (SHA256 calculation).
       - [x] Menu Editor (Visual Drag & Drop).

  ---

  ## Phase 1 Retrospective
  The core foundation is stable. Modules can be developed independently, registered via the Deployer, and distributed to clients via the Bootstrapper. The use of SQLite for local client configuration allows for robust offline capability and fast menu loading.