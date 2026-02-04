# GEMINI.md - Project Context & Guide

## 1. Project Overview

**Project Name:** Hospital Information System (GMIS / EMR) Framework Migration
**Type:** Enterprise Desktop Application (WinForms)
**Status:** **Foundation Implemented (Phase 1 Completed)**
**Domain:** Healthcare / Hospital Information System (HIS)

This project represents a large-scale Hospital Information System (GMIS) and Electronic Medical Record (EMR) system. The current working directory contains both the legacy source code (`AS-IS`) and the active implementation of the next-generation framework (`nU3.Framework`).

The primary goal is to transition from a legacy custom WinForms framework to a modern **.NET 8 + DevExpress WinForms** architecture, focusing on modularity, performance, and stability.

## 2. Directory Structure & Projects

### 📂 Root Directory
*   `AS-IS/`: Legacy source code.
*   `nU3.Framework/SRC/`: **New Framework Source Code**

### 📂 Active Projects (nU3.Framework)
*   **`nU3.Core`**:
    *   Shared Interfaces (`IModule`, `IWorkForm`).
    *   Attributes (`[ScreenInfo]`) for metadata-driven discovery.
    *   Events (`IEventAggregator`, `PubSubEvent`) for decoupled communication.
    *   Security (`UserSession`).
*   **`nU3.Core.UI`**:
    *   Base UI Classes (`BaseWorkControl`, `BaseXtraForm`).
    *   *Designed to inherit from DevExpress controls.*
*   **`nU3.Data`**:
    *   `LocalDatabaseManager`: SQLite implementation for client-side configuration.
    *   Schema: `SYS_MODULE_MST`, `SYS_MODULE_VER`, `SYS_PROG_MST`, `SYS_MENU`.
*   **`nU3.Connectivity`**:
    *   **Connectivity Layer**: Database access (Oracle/SQL) and File Transfer interfaces.
    *   `IDBAccessService`: Standardized async/sync database operations.
*   **`nU3.Bootstrapper`**:
    *   **Launcher Application**.
    *   Checks for module updates (Syncs `ServerStorage` -> `Staging Cache` -> `Runtime Modules`).
    *   **Zero-Lock Updates**: Ensures DLLs are updated before the main Shell locks them.
    *   Launches `nU3.Shell.exe`.
*   **`nU3.Shell`**:
    *   **Main MDI Container**.
    *   Dynamically loads modules from the `Modules/` directory (Recursive).
    *   Builds menus dynamically from `SYS_MENU`.
    *   Uses Dependency Injection (DI) for services.
*   **`nU3.Tools.Deployer`**:
    *   **Admin Tool** for developers.
    *   Register Modules & Categories.
    *   Upload Versions (with Hash calculation).
    *   **Menu Editor**: Visual Drag & Drop menu configuration.
*   **`nU3.Modules.{Category}.{Name}`**:
    *   Example: `nU3.Modules.EMR.Clinic` (Outpatient Registration).
    *   Business logic implementation.

## 3. Technical Stack & Key Technologies

### TO-BE (New Framework)
*   **Framework:** .NET 8 (Windows)
*   **UI:** WinForms (DevExpress)
*   **Data (Client):** System.Data.SQLite
*   **Data (Server):** Oracle (via `nU3.Connectivity`)
*   **Communication:**
    *   **Internal:** `EventAggregator` (Pub/Sub).
    *   **External:** Service Agent Pattern (`nU3.Connectivity`).
*   **Architecture:**
    *   **Category-Based Plugin Architecture:** Modules are organized by folder (`Modules/EMR/`, `Modules/ADM/`).
    *   **Staged Deployment:** Bootstrapper syncs DLLs to a local cache to prevent file locking.
    *   **DI Container:** `Microsoft.Extensions.DependencyInjection`.

## 4. Deployment System (Implemented)

The framework uses a rigorous deployment strategy:

1.  **Registration**: Developer uses `Deployer` tool to register a module (e.g., `MOD_CLINIC`) and assign a Category (`EMR`).
2.  **Upload**: Developer uploads a compiled DLL. The tool:
    *   Calculates SHA-256 Hash.
    *   Scans for `[ScreenInfo]` attributes to register Programs (`PROG_ID`).
    *   Copies file to Server Storage (`ServerStorage/EMR/File.dll`).
3.  **Bootstrapping (Client)**:
    *   `Bootstrapper` runs on client startup.
    *   Checks `SYS_MODULE_VER` for active versions.
    *   **Staging:** Copies updated DLLs to `%AppData%/nU3.Framework/Cache`.
    *   **Installation:** Syncs Cache to `[AppDir]/Modules` before Shell starts.
4.  **Execution**: Shell loads assemblies from the local `Modules` folder.

## 5. Development Conventions

*   **Naming:** `nU3.Modules.{Category}.{Name}`
*   **Attributes:** All Screens must have `[ScreenInfo("Name", "ID", "Category")]`.
*   **Base Classes:** All Screens must inherit `BaseWorkControl`. All Popups must inherit `BaseXtraForm`.

## 6. Critical Considerations

1.  **Dependency Decoupling:** The legacy system likely has tight coupling. The new system enforces isolation via `IEventAggregator` and Interface-based design.
2.  **Hardware Integration:** (Pending - Phase 2) Needs Serial/USB integration layer.
3.  **Performance:** (Pending - Phase 2) GridControl Server Mode implementation.



## Inportant ##
- 주석은 한글로 작성하며 최대한 자세하게 작성한다. 
- 주석은 내용이 틀린 경우 삭제해도 무방하나 기존에 있던 주석을 임의로 삭제하지 않는다.
- 코드주석 처리는 임의로 삭제하지 않는다. 
- Framework 전반에 관련된 문서는 DOC 폴더밑에 DOC_{Category}_{Title}.md 형식으로 작성한다.
- 프로젝트 내에 문서 파일은 프로젝트 파일(.csproj)과 동일한 곳에 생성하며, DOC_{Category}_{Function}_{Detail}.md 로 형식으로 작성한다.
- 코드가 수정이 되면 DOC_ 내용에 맞게 갱신한다.





