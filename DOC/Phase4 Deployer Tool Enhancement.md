Deployer Tool Enhancement

  Workflow Strategy: Systematic
  Context: Enhancing the nU3.Tools.Deployer to automate module registration, versioning, and menu configuration
  while enforcing naming conventions.
  Goal: Reduce manual input errors by auto-extracting metadata from DLLs and standardizing the namespace/file naming
  convention.

  ---

  Implementation Roadmap

  Phase 1: Convention Enforcement & Metadata Extraction
  Objective: Automate data entry by parsing the DLL file name and attributes.
   - [ ] Naming Convention Logic:
       - Format: nU3.Modules.{System}.{SubSys}.{Name}.dll
       - Parser: Extract System (EMR), SubSys (IO), Name from filename.
   - [ ] Attribute Enhancement:
       - Update ScreenInfoAttribute to include DllVersion (or read from AssemblyInfo).
       - Update Deployer to read AssemblyFileVersion.

  Phase 2: Enhanced Deployer UI/UX
  Objective: Streamline the "Upload & Register" workflow.
   - [ ] Smart Upload Wizard:
       - Step 1: Select DLL.
       - Step 2: Auto-parse Name/System/Version. Display for confirmation.
       - Step 3: Auto-scan ScreenInfo attributes. Show list of found screens.
       - Step 4: Commit to DB (SYS_MODULE_MST, SYS_MODULE_VER, SYS_PROG_MST).
   - [ ] Validation: Reject DLLs that don't match the nU3.Modules.* pattern.

  Phase 3: Role-Based Menu Management (RBAC)
  Objective: Map menus to specific Roles/AuthLevels.
   - [ ] Database Update:
       - Add AUTH_LEVEL or ROLE_ID to SYS_MENU (or a separate SYS_MENU_ROLE table).
   - [ ] Menu Editor Update:
       - Allow setting "Min Auth Level" or "Allowed Roles" for each menu node.

  ---

  Task Hierarchy

  Epic: Deployer Automation (DEP-AUTO)

  Story: Smart Metadata Extraction (DEP-AUTO-01)
   - [ ] Task A: Implement DllNameParser class.
   - [ ] Task B: Update ScanAndRegisterPrograms to extract Version from Assembly.
   - [ ] Task C: Refactor DeployerForm to use the "Smart Upload" flow.

  Story: Convention Enforcement (DEP-AUTO-02)
   - [ ] Task A: Add validation logic to DeployerForm.
   - [ ] Task B: Show error if Namespace != Filename pattern.

  ---
