using System;
using System.Data.SQLite;
using System.IO;

namespace nU3.Data
{
    public class LocalDatabaseManager
    {
        private readonly string _dbPath;
        private readonly string _connectionString;

        public LocalDatabaseManager(string dbFileName = "nU3_Local.db")
        {
            // Changed back to ApplicationData to ensure all processes share the same DB
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string folder = Path.Combine(appData, "nU3.Framework", "Database");

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            _dbPath = Path.Combine(folder, dbFileName);
            _connectionString = $"Data Source={_dbPath};Version=3;";
            InitializeSchema();
        }

        public void InitializeSchema()
        {
            if (!File.Exists(_dbPath))
            {
                SQLiteConnection.CreateFile(_dbPath);
            }

            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand(conn))
                {
                    // A. Module Master (IS_USE 제거)
                    cmd.CommandText = @"
                        CREATE TABLE IF NOT EXISTS SYS_MODULE_MST (
                            MODULE_ID TEXT PRIMARY KEY,
                            CATEGORY TEXT,
                            SUBSYSTEM TEXT,
                            MODULE_NAME TEXT,
                            FILE_NAME TEXT NOT NULL,
                            REG_DATE TEXT DEFAULT CURRENT_TIMESTAMP
                        );";
                    cmd.ExecuteNonQuery();

                    // Migration: Check if CATEGORY/SUBSYSTEM columns exist
                    cmd.CommandText = "PRAGMA table_info(SYS_MODULE_MST);";
                    bool hasCategory = false;
                    bool hasSubSystem = false;
                    bool hasIsUse = false;
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string colName = reader["name"].ToString();
                            if (colName.Equals("CATEGORY", StringComparison.OrdinalIgnoreCase)) hasCategory = true;
                            if (colName.Equals("SUBSYSTEM", StringComparison.OrdinalIgnoreCase)) hasSubSystem = true;
                            
                        }
                    }

                    if (!hasCategory)
                    {
                        cmd.CommandText = "ALTER TABLE SYS_MODULE_MST ADD COLUMN CATEGORY TEXT;";
                        cmd.ExecuteNonQuery();
                    }
                    if (!hasSubSystem)
                    {
                        cmd.CommandText = "ALTER TABLE SYS_MODULE_MST ADD COLUMN SUBSYSTEM TEXT;";
                        cmd.ExecuteNonQuery();
                    }
                    if (hasIsUse)
                    {
                        RebuildSysModuleMst(conn);
                    }

                    // B. Module Version (IS_ACTIVE/IS_DELETED 제거, IS_DELETED -> DEL_DATE)
                    cmd.CommandText = @"
                        CREATE TABLE IF NOT EXISTS SYS_MODULE_VER (
                            MODULE_ID TEXT,
                            VERSION TEXT,
                            FILE_HASH TEXT,
                            FILE_SIZE INTEGER,
                            STORAGE_PATH TEXT,
                            DEPLOY_DESC TEXT,
                            DEPLOY_DATE TEXT DEFAULT CURRENT_TIMESTAMP,
                            DEPLOYER TEXT,
                            DEL_DATE TEXT,
                            PRIMARY KEY (MODULE_ID, VERSION)
                        );";
                    cmd.ExecuteNonQuery();

                    MigrateSysModuleVer(conn);

                    // C. Program Master (SCREEN_NAME -> PROG_NAME, IS_ACTIVE 추가, PROG_TYPE 추가)
                    cmd.CommandText = @"
                        CREATE TABLE IF NOT EXISTS SYS_PROG_MST (
                            PROG_ID TEXT PRIMARY KEY,
                            MODULE_ID TEXT,
                            CLASS_NAME TEXT,
                            PROG_NAME TEXT,
                            AUTH_LEVEL INTEGER DEFAULT 1,
                            IS_ACTIVE TEXT DEFAULT 'N',
                            PROG_TYPE INTEGER DEFAULT 1,
                            FOREIGN KEY(MODULE_ID) REFERENCES SYS_MODULE_MST(MODULE_ID)
                        );";
                    cmd.ExecuteNonQuery();

                    MigrateSysProgMst(conn);

                    cmd.CommandText = @"
                        CREATE TABLE IF NOT EXISTS SYS_MENU (
                            MENU_ID TEXT PRIMARY KEY,
                            PARENT_ID TEXT,
                            MENU_NAME TEXT,
                            PROG_ID TEXT,
                            ICON_RES TEXT,
                            SORT_ORD INTEGER,
                            SHORTCUT TEXT,
                            AUTH_LEVEL INTEGER DEFAULT 1,
                            FOREIGN KEY(PROG_ID) REFERENCES SYS_PROG_MST(PROG_ID)
                        );";
                    cmd.ExecuteNonQuery();

                    // Migration: Check for AUTH_LEVEL
                    cmd.CommandText = "PRAGMA table_info(SYS_MENU);";
                    bool hasAuth = false;
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (reader["name"].ToString().Equals("AUTH_LEVEL", StringComparison.OrdinalIgnoreCase))
                            {
                                hasAuth = true;
                                break;
                            }
                        }
                    }

                    if (!hasAuth)
                    {
                        cmd.CommandText = "ALTER TABLE SYS_MENU ADD COLUMN AUTH_LEVEL INTEGER DEFAULT 1;";
                        cmd.ExecuteNonQuery();
                    }

                    // D. Component Master (Framework DLL, 공용 라이브러리, EXE 관리)
                    cmd.CommandText = @"
                        CREATE TABLE IF NOT EXISTS SYS_COMPONENT_MST (
                            COMPONENT_ID TEXT PRIMARY KEY,
                            COMPONENT_TYPE INTEGER NOT NULL DEFAULT 0,
                            COMPONENT_NAME TEXT NOT NULL,
                            FILE_NAME TEXT NOT NULL,
                            INSTALL_PATH TEXT,
                            GROUP_NAME TEXT,
                            IS_REQUIRED INTEGER DEFAULT 0,
                            AUTO_UPDATE INTEGER DEFAULT 1,
                            DESCRIPTION TEXT,
                            PRIORITY INTEGER DEFAULT 100,
                            DEPENDENCIES TEXT,
                            REG_DATE TEXT DEFAULT CURRENT_TIMESTAMP,
                            MOD_DATE TEXT,
                            IS_ACTIVE TEXT DEFAULT 'Y'
                        );";
                    cmd.ExecuteNonQuery();

                    // E. Component Version
                    cmd.CommandText = @"
                        CREATE TABLE IF NOT EXISTS SYS_COMPONENT_VER (
                            COMPONENT_ID TEXT,
                            VERSION TEXT,
                            FILE_HASH TEXT,
                            FILE_SIZE INTEGER,
                            STORAGE_PATH TEXT,
                            MIN_FRAMEWORK_VER TEXT,
                            MAX_FRAMEWORK_VER TEXT,
                            DEPLOY_DESC TEXT,
                            RELEASE_NOTE_URL TEXT,
                            REG_DATE TEXT DEFAULT CURRENT_TIMESTAMP,
                            DEL_DATE TEXT,
                            IS_ACTIVE TEXT DEFAULT 'Y',
                            PRIMARY KEY (COMPONENT_ID, VERSION),
                            FOREIGN KEY(COMPONENT_ID) REFERENCES SYS_COMPONENT_MST(COMPONENT_ID)
                        );";
                    cmd.ExecuteNonQuery();

                    // F. Client Component (클라이언트 설치 현황 - 로컬 전용)
                    cmd.CommandText = @"
                        CREATE TABLE IF NOT EXISTS SYS_CLIENT_COMPONENT (
                            COMPONENT_ID TEXT PRIMARY KEY,
                            INSTALLED_VERSION TEXT,
                            INSTALLED_PATH TEXT,
                            INSTALLED_DATE TEXT,
                            FILE_HASH TEXT
                        );";
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private static bool ColumnExists(SQLiteConnection conn, string tableName, string columnName)
        {
            using var cmd = new SQLiteCommand($"PRAGMA table_info({tableName});", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                if (reader["name"].ToString().Equals(columnName, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }

        private static void RebuildSysModuleMst(SQLiteConnection conn)
        {
            using var tx = conn.BeginTransaction();
            using var cmd = new SQLiteCommand(conn);

            cmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS SYS_MODULE_MST__NEW (
                    MODULE_ID TEXT PRIMARY KEY,
                    CATEGORY TEXT,
                    SUBSYSTEM TEXT,
                    MODULE_NAME TEXT,
                    FILE_NAME TEXT NOT NULL,
                    REG_DATE TEXT DEFAULT CURRENT_TIMESTAMP
                );";
            cmd.ExecuteNonQuery();

            cmd.CommandText = @"
                INSERT INTO SYS_MODULE_MST__NEW (MODULE_ID, CATEGORY, SUBSYSTEM, MODULE_NAME, FILE_NAME, REG_DATE)
                SELECT MODULE_ID, CATEGORY, SUBSYSTEM, MODULE_NAME, FILE_NAME, REG_DATE
                FROM SYS_MODULE_MST;";
            cmd.ExecuteNonQuery();

            cmd.CommandText = "DROP TABLE SYS_MODULE_MST;";
            cmd.ExecuteNonQuery();

            cmd.CommandText = "ALTER TABLE SYS_MODULE_MST__NEW RENAME TO SYS_MODULE_MST;";
            cmd.ExecuteNonQuery();

            tx.Commit();
        }

        private static void MigrateSysModuleVer(SQLiteConnection conn)
        {
            bool hasIsActive = ColumnExists(conn, "SYS_MODULE_VER", "IS_ACTIVE");
            bool hasIsDeleted = ColumnExists(conn, "SYS_MODULE_VER", "IS_DELETED");
            bool hasDelDate = ColumnExists(conn, "SYS_MODULE_VER", "DEL_DATE");

            if (!hasIsActive && !hasIsDeleted)
            {
                if (!hasDelDate)
                {
                    using var cmdAdd = new SQLiteCommand("ALTER TABLE SYS_MODULE_VER ADD COLUMN DEL_DATE TEXT;", conn);
                    cmdAdd.ExecuteNonQuery();
                }
                return;
            }

            using var tx = conn.BeginTransaction();
            using var cmd = new SQLiteCommand(conn);

            cmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS SYS_MODULE_VER__NEW (
                    MODULE_ID TEXT,
                    VERSION TEXT,
                    FILE_HASH TEXT,
                    FILE_SIZE INTEGER,
                    STORAGE_PATH TEXT,
                    DEPLOY_DESC TEXT,
                    DEPLOY_DATE TEXT DEFAULT CURRENT_TIMESTAMP,
                    DEPLOYER TEXT,
                    DEL_DATE TEXT,
                    PRIMARY KEY (MODULE_ID, VERSION)
                );";
            cmd.ExecuteNonQuery();

            if (hasIsDeleted)
            {
                cmd.CommandText = @"
                    INSERT INTO SYS_MODULE_VER__NEW (MODULE_ID, VERSION, FILE_HASH, FILE_SIZE, STORAGE_PATH, DEPLOY_DESC, DEPLOY_DATE, DEPLOYER, DEL_DATE)
                    SELECT MODULE_ID, VERSION, FILE_HASH, FILE_SIZE, STORAGE_PATH, DEPLOY_DESC, DEPLOY_DATE, DEPLOYER,
                           CASE WHEN IFNULL(IS_DELETED,'N') = 'Y' THEN CURRENT_TIMESTAMP ELSE NULL END
                    FROM SYS_MODULE_VER;";
            }
            else
            {
                cmd.CommandText = @"
                    INSERT INTO SYS_MODULE_VER__NEW (MODULE_ID, VERSION, FILE_HASH, FILE_SIZE, STORAGE_PATH, DEPLOY_DESC, DEPLOY_DATE, DEPLOYER, DEL_DATE)
                    SELECT MODULE_ID, VERSION, FILE_HASH, FILE_SIZE, STORAGE_PATH, DEPLOY_DESC, DEPLOY_DATE, DEPLOYER, NULL
                    FROM SYS_MODULE_VER;";
            }
            cmd.ExecuteNonQuery();

            cmd.CommandText = "DROP TABLE SYS_MODULE_VER;";
            cmd.ExecuteNonQuery();

            cmd.CommandText = "ALTER TABLE SYS_MODULE_VER__NEW RENAME TO SYS_MODULE_VER;";
            cmd.ExecuteNonQuery();

            tx.Commit();
        }

        private static void MigrateSysProgMst(SQLiteConnection conn)
        {
            bool hasScreenName = ColumnExists(conn, "SYS_PROG_MST", "SCREEN_NAME");
            bool hasProgName = ColumnExists(conn, "SYS_PROG_MST", "PROG_NAME");
            bool hasIsActive = ColumnExists(conn, "SYS_PROG_MST", "IS_ACTIVE");
            bool hasProgType = ColumnExists(conn, "SYS_PROG_MST", "PROG_TYPE");

            if (!hasScreenName && hasProgName)
            {
                if (!hasIsActive)
                {
                    using var cmdAdd = new SQLiteCommand("ALTER TABLE SYS_PROG_MST ADD COLUMN IS_ACTIVE TEXT DEFAULT 'N';", conn);
                    cmdAdd.ExecuteNonQuery();
                }
                if (!hasProgType)
                {
                    using var cmdAdd = new SQLiteCommand("ALTER TABLE SYS_PROG_MST ADD COLUMN PROG_TYPE INTEGER DEFAULT 1;", conn);
                    cmdAdd.ExecuteNonQuery();
                }
                return;
            }

            using var tx = conn.BeginTransaction();
            using var cmd = new SQLiteCommand(conn);

            cmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS SYS_PROG_MST__NEW (
                    PROG_ID TEXT PRIMARY KEY,
                    MODULE_ID TEXT,
                    CLASS_NAME TEXT,
                    PROG_NAME TEXT,
                    AUTH_LEVEL INTEGER DEFAULT 1,
                    IS_ACTIVE TEXT DEFAULT 'N',
                    PROG_TYPE INTEGER DEFAULT 1,
                    FOREIGN KEY(MODULE_ID) REFERENCES SYS_MODULE_MST(MODULE_ID)
                );";
            cmd.ExecuteNonQuery();

            if (hasScreenName)
            {
                cmd.CommandText = @"
                    INSERT INTO SYS_PROG_MST__NEW (PROG_ID, MODULE_ID, CLASS_NAME, PROG_NAME, AUTH_LEVEL, IS_ACTIVE, PROG_TYPE)
                    SELECT PROG_ID, MODULE_ID, CLASS_NAME, SCREEN_NAME, AUTH_LEVEL,
                           'N',
                           1
                    FROM SYS_PROG_MST;";
            }
            else if (hasProgName)
            {
                cmd.CommandText = @"
                    INSERT INTO SYS_PROG_MST__NEW (PROG_ID, MODULE_ID, CLASS_NAME, PROG_NAME, AUTH_LEVEL, IS_ACTIVE, PROG_TYPE)
                    SELECT PROG_ID, MODULE_ID, CLASS_NAME, PROG_NAME, AUTH_LEVEL,
                           COALESCE(IS_ACTIVE,'N'),
                           COALESCE(PROG_TYPE,1)
                    FROM SYS_PROG_MST;";
            }
            else
            {
                cmd.CommandText = @"
                    INSERT INTO SYS_PROG_MST__NEW (PROG_ID, MODULE_ID, CLASS_NAME, PROG_NAME, AUTH_LEVEL, IS_ACTIVE, PROG_TYPE)
                    SELECT PROG_ID, MODULE_ID, CLASS_NAME, NULL, AUTH_LEVEL,
                           'N',
                           1
                    FROM SYS_PROG_MST;";
            }
            cmd.ExecuteNonQuery();

            cmd.CommandText = "DROP TABLE SYS_PROG_MST;";
            cmd.ExecuteNonQuery();

            cmd.CommandText = "ALTER TABLE SYS_PROG_MST__NEW RENAME TO SYS_PROG_MST;";
            cmd.ExecuteNonQuery();

            tx.Commit();
        }

        public string GetConnectionString()
        {
            return _connectionString;
        }
    }
}
