 /sg:task Deployment Tool Implementation & End-to-End Verification

  Strategy: Systematic
  Context: Moving from PoC (Dummy Module) to a Functional Deployment System.
  Goal: Create a WinForms tool for developers/admins to upload DLLs, register them in the DB, configure menus, and
  verify the client (Shell) downloads and runs them correctly.

  ---

  Task Hierarchy

  Epic: Deployment & Configuration System (DEP)

  Story: Module Registration & Upload Tool (DEP-01)
  Objective: A UI tool for developers to register new modules and upload versions.
   - [ ] Task DEP-01-A: Create nU3.Tools.Deployer Project (WinForms).
   - [ ] Task DEP-01-B: Implement "Module Master Management" UI (CRUD SYS_MODULE_MST).
   - [ ] Task DEP-01-C: Implement "Version Upload" UI.
       - Select DLL file.
       - Auto-calculate Hash/Size.
       - Copy to "Server Storage" (Simulated folder).
       - Insert into SYS_MODULE_VER.

  Story: Menu Configuration Manager (DEP-02)
  Objective: A UI to construct the menu tree from registered programs.
   - [ ] Task DEP-02-A: Implement "Program Scanner".
       - Load a DLL via Reflection.
       - Scan [ScreenInfo] attributes.
       - Upsert into SYS_PROG_MST.
   - [ ] Task DEP-02-B: Implement "Menu Tree Editor".
       - Drag & drop interface to build SYS_MENU hierarchy.
       - Map Menu Nodes to PROG_ID.

  Story: End-to-End Verification (DEP-03)
  Objective: Prove the system works with a new module without touching the Shell code.
   - [ ] Task DEP-03-A: Create a new test module nU3.Modules.Clinic (Real-world scenario).
   - [ ] Task DEP-03-B: Use Deployer tool to upload Clinic.dll.
   - [ ] Task DEP-03-C: Use Deployer tool to create a menu "Clinic > Outpatient".
   - [ ] Task DEP-03-D: Launch Bootstrapper -> Shell and verify "Clinic" menu appears and opens.

  ---

  Execution Plan

  I will start with DEP-01 (The Deployer Tool).

  Step 1: Create nU3.Tools.Deployer project.
  Step 2: Implement the Module Master CRUD UI.