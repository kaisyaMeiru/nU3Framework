# Phase 2 Roadmap: Advanced Capabilities & Migration

**Status:** Planned / In-Progress
**Goal:** Transition from a functional framework to a production-ready HIS system.

## 1. High Priority Infrastructure Tasks

### 1.1 Hardware Integration Layer (Connectivity)
**Requirement:** Hospital systems require interaction with Barcode Scanners, Card Readers, and Medical Devices.
*   **Task:** Implement `nU3.Connectivity.Hardware`.
*   **Components:**
    *   `ISerialPortService`: Generic Wrapper for `System.IO.Ports`.
    *   `IBarcodeScanner`: Event-driven scanner interface.
    *   `ICardReader`: Support for IC/RF Card reading.

### 1.2 Advanced UI Components
**Requirement:** High-performance data handling for large datasets (Patient Lists, Lab Results).
*   **Task:** Implement Server Mode for DevExpress GridControl.
*   **Strategy:**
    *   Create `PageableDataSource` implementing `IListServer`.
    *   Integrate with `IDBAccessService` to fetch data in chunks.

### 1.3 Real Server API Implementation
**Requirement:** Replace the file-copy simulation in `ModuleLoader` with real HTTP/FTP downloading.
*   **Task:** Update `nU3.Connectivity`.
*   **Action:**
    *   Implement `HttpFileTransferService : IFileTransferService`.
    *   Update `Bootstrapper` to use the interface instead of `File.Copy`.

## 2. Business Module Migration (Pilot)

### 2.1 EMR (Electronic Medical Record)
*   **Outpatient Registration (OPD):**
    *   Migrate existing logic to `nU3.Modules.EMR.Clinic`.
    *   Utilize `LocalDatabaseManager` for caching frequent codes (Departments, Doctors).
*   **Doctor Workstation:**
    *   Implement "Patient Dashboard" using the new MDI structure.

### 2.2 ADM (Administration)
*   **User Management:**
    *   Create User/Role management screens using the new `nU3.Core` UserSession.

## 3. Deployment & DevOps

### 3.1 CI/CD Integration
*   **Build Pipeline:** Automate the "Build -> Hash -> Upload" process currently done by the `Deployer` tool.
*   **Auto-Update Server:** Setup a static file server (IIS/Nginx) to host the Module DLLs.

---

## Next Steps for Developer

1.  **Review Phase 1 Docs:** Ensure understanding of the `Bootstrapper` -> `Shell` flow.
2.  **Start Hardware Layer:** Begin implementing `ISerialPortService`.
3.  **Migrate One Screen:** Take a simple screen (e.g., "Code Management") and migrate it to a Module to test the full lifecycle.
