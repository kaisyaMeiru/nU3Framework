  nU3.Bootstrapper Workflow

  The nU3.Bootstrapper is the entry point application responsible for initializing the environment, ensuring the
  database is ready, downloading/updating modules from a central "Server" (simulated) to a safe Staging area,
  installing them to the Runtime area, and finally launching the main Shell application.

  1. Initialization Phase (Program.Main)

   1. Start: Console application starts.
   2. ModuleLoader Instantiation:
       * Creates LocalDatabaseManager:
           * Sets DB Path to %AppData%\nU3.Framework\Database\nU3_Local.db.
           * Initializes Schema (InitializeSchema()) if DB doesn't exist.
       * Initializes SQLiteModuleRepository.
       * Setup Paths:
           * Staging Path: %AppData%\nU3.Framework\Cache (Safe download/update area).
           * Install Path: [ExeDirectory]\Modules (Runtime execution area).
   3. Ensure Database Initialized:
       * Calls loader.EnsureDatabaseInitialized() (proxied to _dbManager.InitializeSchema()).
       * Ensures tables (SYS_MODULE_MST, SYS_MODULE_VER, SYS_PROG_MST, SYS_MENU) exist.
       * Performs migration checks (adding columns like CATEGORY, SUBSYSTEM, AUTH_LEVEL if missing).

  2. Seeding Phase (Seeder.SeedDummyData)

   * Note: This is currently hardcoded for testing/development.
   1. Locate Dummy DLL: Looks for nU3.Modules.ADM.AD.Deployer.dll (or similar dummy module) in the local bin or
      project path.
   2. Calculate Hash: Computes SHA256 hash of the found DLL.
   3. Insert/Update Database:
       * SYS_MODULE_MST: Registers the module.
       * SYS_MODULE_VER: Registers version 1.0.0.0 with hash and path.
       * SYS_PROG_MST: Registers the program ID and class name.
       * SYS_MENU: Inserts default menu items if the table is empty.

  3. Update & Installation Phase (loader.CheckAndLoadModules)

  This is the core logic implemented in CheckAndDownloadModules():

   1. Fetch Module List: Retrieves all modules from SYS_MODULE_MST via SQLiteModuleRepository.
   2. Iterate Modules: For each active module (IS_USE = 'Y'):
       * A. Download to Staging (Update Check):
           * Target: %AppData%\nU3.Framework\Cache\[SubSystem]\[FileName]
           * Check: If file is missing (or in a real scenario, if Hash/Version differs from Server).
           * Action: Copy file from "ServerStorage" (%AppData%\nU3.Framework\ServerStorage) to Staging.
       * B. Install to Runtime (Deployment):
           * Target: [ExeDirectory]\Modules\[SubSystem]\[FileName]
           * Check: If file is missing in Runtime folder OR if Staging was just updated.
           * Action: Copy file from Staging to Runtime.
           * Benefit: Since nU3.Shell.exe is not running yet, the DLLs in [ExeDirectory]\Modules are not locked,
             allowing safe overwrites.

  4. Launch Phase (Program.Main)

   1. Locate Shell: Looks for nU3.Shell.exe in [ExeDirectory].
   2. Launch: Executes nU3.Shell.exe using Process.Start.
   3. Exit: Bootstrapper terminates.

  ---

  Key Architectural Features

   * Two-Stage Deployment:
       * Stage 1 (Download): To %AppData% (User-writable, no locking issues).
       * Stage 2 (Install): To [ExeDir] (Fast local load, standard .NET loading context).
   * Shadow Copying (Manual): Instead of using .NET's built-in Shadow Copy features which can be complex to manage
     for plugins, we implement a "Copy-on-Boot" strategy.
   * Centralized DB: Uses a known path in %AppData% so Bootstrapper (Admin) and Shell (User) share the same
     metadata.

  This workflow ensures that when nU3.Shell starts, it always runs against the latest locally cached version of
  modules without file locking conflicts during the update process.