using System;
using System.Data;
using System.IO;
using nU3.Connectivity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using nU3.Core.Enums;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using nU3.Core.Interfaces;

namespace nU3.Server.Connectivity.Services
{
    /// <summary>
    /// SQLite 스키마 초기화 서비스
    ///
    /// 역할:
    /// - 구성에서 SQLite가 사용되도록 설정된 경우 데이터베이스 파일 경로를 확인하고 필요한 디렉토리를 생성합니다.
    /// - 기본 테이블을 생성하고 초기 데이터를 시드합니다.
    /// - 주로 서버 스타트업 시 호출되어 로컬 SQLite 파일 기반의 POC 또는 로컬 모드 환경을 준비합니다.
    ///
    /// 주의사항:
    /// - 이 서비스는 SQLite 전용이며, 구성에서 Provider가 'Sqlite' 또는 'SQLite'로 설정되어 있을 때만 동작합니다.    
    /// </summary>
    public class SqliteSchemaService
    {
        private readonly IDBAccessService _db;
        private readonly ILogger<SqliteSchemaService> _logger;
        private readonly IConfiguration _config;

        public SqliteSchemaService(IDBAccessService db, ILogger<SqliteSchemaService> logger, IConfiguration config)
        {
            _db = db;
            _logger = logger;
            _config = config;
        }

        /// <summary>
        /// SQLite 스키마를 검증하고 필요하면 테이블을 생성한 뒤 초기 데이터를 시드합니다.
        /// 구성 경로:
        /// - ServerSettings:Database:Provider              : 데이터베이스 공급자
        /// - ServerSettings:Database:Connections:Sqlite    : SQLite 연결 문자열
        /// - ServerSettings:Database:DbDirectory           : 상대 경로일 경우 기준이 되는 디렉토리
        /// </summary>
        public void Initialize()
        {
            try
            {                                         
                // 1) 구성에서 DB 공급자 확인
                var provider = _config.GetValue<string>("ServerSettings:Database:Provider");
                if (!string.Equals(provider, "Sqlite", StringComparison.OrdinalIgnoreCase) &&
                    !string.Equals(provider, "SQLite", StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogInformation($"Current Provider is {provider}. Skipping SQLite initialization.");
                    return;
                }

                _logger.LogInformation("Starting SQLite schema verification and initialization...");

                // 2) 연결 문자열 획득
                var connStr = _config.GetValue<string>("ServerSettings:Database:Connections:Sqlite");
                if (string.IsNullOrEmpty(connStr))
                {
                    _logger.LogWarning("Sqlite connection string is missing in ServerSettings:Database:Connections.");
                    return;
                }

                // 3) 데이터베이스 파일 경로 계산 (DbDirectory와 결합하여 절대 경로로 변환)
                var dbDir = _config.GetValue<string>("ServerSettings:Database:DbDirectory") ?? "Server_Database";

                var builder = new System.Data.Common.DbConnectionStringBuilder { ConnectionString = connStr };

                if (builder.TryGetValue("Data Source", out var pathObj) && pathObj is string dbPath)
                {
                    // 상대 경로일 경우 ServerSettings:Database:DbDirectory 또는 AppContext.BaseDirectory를 기준으로 절대 경로 생성
                    if (!Path.IsPathRooted(dbPath))
                    {
                        var basePath = Path.IsPathRooted(dbDir)
                            ? dbDir
                            : Path.Combine(AppContext.BaseDirectory, dbDir);

                        dbPath = Path.GetFullPath(Path.Combine(basePath, dbPath));
                        builder["Data Source"] = dbPath;

                        // 참고: 여기서는 실제 _db의 연결 문자열을 직접 수정하지 않습니다.
                        // 이 서비스의 목적은 파일/디렉토리 생성 보장에 집중합니다.
                    }

                    var dir = Path.GetDirectoryName(dbPath);
                    if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                        _logger.LogInformation($"Created database directory: {dir}");
                    }

                    _logger.LogInformation($"Resolved Database file path: {dbPath}");
                }

                // 4) DB 연결 및 테이블 생성/시드
                if (_db.Connect())
                {
                    CreateTables();
                    _logger.LogInformation("Tables created or verified.");

                    SeedInitialData();
                    _logger.LogInformation("Initial seed data checked.");

                    SeedTestData();
                    _logger.LogInformation("Test data checked.");

                    _logger.LogInformation("SQLite schema initialization completed successfully.");
                }
                else
                {
                    _logger.LogError("Failed to connect to SQLite database for initialization.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initializing SQLite schema.");
            }
        }

        /// <summary>
        /// 기본 데이터(역할, 관리자 계정 등)를 확인하고 없으면 삽입합니다.
        /// 실패해도 전체 흐름을 중단하지 않도록 예외를 잡아서 경고 로그를 남깁니다.
        /// </summary>
        private void SeedInitialData()
        {
            try
            {
                SyncEnumsToDb();

                var userCount = Convert.ToInt32(_db.ExecuteScalarValue("SELECT COUNT(*) FROM SYS_USER"));
                if (userCount == 0)
                {
                    _db.ExecuteNonQuery(@"
                        INSERT INTO SYS_USER (USER_ID, USERNAME, PASSWORD, EMAIL, ROLE_CODE, IS_ACTIVE) 
                        VALUES ('admin', '관리자', '1234', 'admin@nu3.com',  '0', 'Y')");
                    _logger.LogInformation("Default admin user seeded.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Failed to seed data: {ex.Message}");
            }
        }

        private void SyncEnumsToDb()
        {
            try
            {
                // Sync Roles
                foreach (UserRole role in Enum.GetValues(typeof(UserRole)))
                {
                    int code = (int)role;
                    string name = GetEnumDisplayName(role);
                    string desc = GetEnumDescription(role);

                    string sql = @"INSERT OR REPLACE INTO SYS_ROLE (ROLE_CODE, ROLE_NAME, DESCRIPTION) VALUES (@c, @n, @d)";
                    _db.ExecuteNonQuery(sql, new Dictionary<string, object> { { "@c", code.ToString() }, { "@n", name }, { "@d", desc } });
                }

                // Sync Depts (Department enum 기반)
                foreach (Department dept in Enum.GetValues(typeof(Department)))
                {
                    int deptCode = (int)dept;
                    string deptCodeStr = deptCode.ToString();
                    string deptName = GetEnumDisplayName(dept);
                    string deptNameEng = dept.ToString();
                    string description = GetEnumDescription(dept);
                    int displayOrder = GetEnumDisplayOrder(dept);

                    string sql = @"
                        INSERT OR REPLACE INTO SYS_DEPT 
                        (DEPT_CODE, DEPT_NAME, DEPT_NAME_ENG, DESCRIPTION, DISPLAY_ORDER, IS_ACTIVE, CREATED_DATE) 
                        VALUES (@code, @name, @nameEng, @desc, @order, 'Y', CURRENT_TIMESTAMP)";
                    
                    _db.ExecuteNonQuery(sql, new Dictionary<string, object> 
                    { 
                        { "@code", deptCodeStr }, 
                        { "@name", deptName },
                        { "@nameEng", deptNameEng },
                        { "@desc", description },
                        { "@order", displayOrder }
                    });
                }
                
                _logger.LogInformation("Enums synced to SYS_ROLE and SYS_DEPT.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to sync Enums to DB.");
            }
        }

        private string GetEnumDisplayName(Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attr = field?.GetCustomAttribute<DisplayAttribute>();
            return attr?.Name ?? value.ToString();
        }

        private string GetEnumDescription(Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attr = field?.GetCustomAttribute<DisplayAttribute>();
            return attr?.Description ?? "";
        }

        private int GetEnumDisplayOrder(Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attr = field?.GetCustomAttribute<DisplayAttribute>();
            return attr?.Order ?? 0;
        }

        /// <summary>
        /// 애플리케이션이 필요로 하는 모든 테이블을 생성합니다.
        /// 기존 테이블이 존재하면 영향을 주지 않습니다.
        /// </summary>
        private void CreateTables()
        {
            // 모듈 마스터
            ExecuteSql(@"
                CREATE TABLE IF NOT EXISTS SYS_MODULE_MST (
                    MODULE_ID TEXT PRIMARY KEY,
                    CATEGORY TEXT,
                    SUBSYSTEM TEXT,
                    MODULE_NAME TEXT,
                    FILE_NAME TEXT NOT NULL,
                    REG_DATE TEXT DEFAULT CURRENT_TIMESTAMP
                );");

            // 모듈 버전
            ExecuteSql(@"
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
                );");

            // 프로그램 마스터
            ExecuteSql(@"
                CREATE TABLE IF NOT EXISTS SYS_PROG_MST (
                    PROG_ID TEXT PRIMARY KEY,
                    MODULE_ID TEXT,
                    CLASS_NAME TEXT,
                    PROG_NAME TEXT,
                    AUTH_LEVEL INTEGER DEFAULT 1,
                    IS_ACTIVE TEXT DEFAULT 'N',
                    PROG_TYPE INTEGER DEFAULT 1
                );");

            // 메뉴 마스터
            ExecuteSql(@"
                CREATE TABLE IF NOT EXISTS SYS_MENU (
                    MENU_ID TEXT PRIMARY KEY,
                    PARENT_ID TEXT,
                    MENU_NAME TEXT,
                    PROG_ID TEXT,
                    ICON_RES TEXT,
                    SORT_ORD INTEGER,
                    SHORTCUT TEXT,
                    AUTH_LEVEL INTEGER DEFAULT 1,
                    CHECK (PARENT_ID IS NULL OR PARENT_ID != ''),
                    CHECK (PROG_ID IS NULL OR PROG_ID != '')
                );");

            // 컴포넌트 마스터
            ExecuteSql(@"
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
                );");

            // 컴포넌트 버전
            ExecuteSql(@"
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
                    PRIMARY KEY (COMPONENT_ID, VERSION)
                );");

            // 클라이언트 컴포넌트
            ExecuteSql(@"
                CREATE TABLE IF NOT EXISTS SYS_CLIENT_COMPONENT (
                    COMPONENT_ID TEXT PRIMARY KEY,
                    INSTALLED_VERSION TEXT,
                    INSTALLED_PATH TEXT,
                    INSTALLED_DATE TEXT,
                    FILE_HASH TEXT
                );");

            // 시스템 사용자
            ExecuteSql(@"
                CREATE TABLE IF NOT EXISTS SYS_USER (
                    USER_ID TEXT PRIMARY KEY,
                    USERNAME TEXT NOT NULL,
                    PASSWORD TEXT,
                    EMAIL TEXT,
                    PERMISSION_LEVEL TEXT DEFAULT '1',
                    CREATED_DATE TEXT DEFAULT CURRENT_TIMESTAMP,
                    LAST_LOGIN TEXT,
                    IS_ACTIVE TEXT DEFAULT 'Y',
                    ROLE_CODE TEXT
                );");

            // 시스템 역할
            ExecuteSql(@"
                CREATE TABLE IF NOT EXISTS SYS_ROLE (
                    ROLE_CODE TEXT PRIMARY KEY,
                    ROLE_NAME TEXT NOT NULL,
                    DESCRIPTION TEXT
                );");

            // 시스템 부서 (Department enum 기반)
            ExecuteSql(@"
                CREATE TABLE IF NOT EXISTS SYS_DEPT (
                    DEPT_CODE VARCHAR(10) PRIMARY KEY,
                    DEPT_NAME VARCHAR(100) NOT NULL,
                    DEPT_NAME_ENG VARCHAR(100),
                    DESCRIPTION VARCHAR(200),
                    DISPLAY_ORDER INTEGER DEFAULT 0,
                    PARENT_DEPT VARCHAR(10),
                    IS_ACTIVE VARCHAR(1) DEFAULT 'Y',
                    CREATED_DATE DATETIME DEFAULT CURRENT_TIMESTAMP,
                    MODIFIED_DATE DATETIME
                );");
            
            ExecuteSql("CREATE INDEX IF NOT EXISTS IDX_DEPT_ACTIVE ON SYS_DEPT(IS_ACTIVE);");
            ExecuteSql("CREATE INDEX IF NOT EXISTS IDX_DEPT_ORDER ON SYS_DEPT(DISPLAY_ORDER);");

            // 사용자-부서 매핑
            ExecuteSql(@"
                CREATE TABLE IF NOT EXISTS SYS_USER_DEPT (
                    USER_ID VARCHAR(50) NOT NULL,
                    DEPT_CODE VARCHAR(10) NOT NULL,
                    IS_PRIMARY VARCHAR(1) DEFAULT 'N',
                    CREATED_DATE DATETIME DEFAULT CURRENT_TIMESTAMP,
                    PRIMARY KEY (USER_ID, DEPT_CODE)
                );");
            
            ExecuteSql("CREATE INDEX IF NOT EXISTS IDX_USER_DEPT_USER ON SYS_USER_DEPT(USER_ID);");
            ExecuteSql("CREATE INDEX IF NOT EXISTS IDX_USER_DEPT_DEPT ON SYS_USER_DEPT(DEPT_CODE);");

            // 시스템 권한
            ExecuteSql(@"
                CREATE TABLE IF NOT EXISTS SYS_PERMISSION (
                    PERM_ID INTEGER PRIMARY KEY AUTOINCREMENT,
                    TARGET_TYPE TEXT NOT NULL,
                    TARGET_ID TEXT NOT NULL,
                    PROG_ID TEXT NOT NULL,
                    CAN_READ INTEGER DEFAULT 1,
                    CAN_CREATE INTEGER DEFAULT 0,
                    CAN_UPDATE INTEGER DEFAULT 0,
                    CAN_DELETE INTEGER DEFAULT 0,
                    CAN_PRINT INTEGER DEFAULT 0,
                    CAN_EXPORT INTEGER DEFAULT 0,
                    CAN_APPROVE INTEGER DEFAULT 0,
                    CAN_CANCEL INTEGER DEFAULT 0,
                    CUSTOM_JSON TEXT,
                    UNIQUE(TARGET_TYPE, TARGET_ID, PROG_ID)
                );");

            // 부서별 메뉴 템플릿
            ExecuteSql(@"
                CREATE TABLE IF NOT EXISTS SYS_DEPT_MENU (
                    MENU_ID VARCHAR(50) NOT NULL,
                    DEPT_CODE VARCHAR(10) NOT NULL,
                    PARENT_ID VARCHAR(50),
                    MENU_NAME VARCHAR(100) NOT NULL,
                    PROG_ID VARCHAR(50),
                    SORT_ORD INTEGER DEFAULT 0,
                    AUTH_LEVEL INTEGER DEFAULT 1,
                    CREATED_DATE DATETIME DEFAULT CURRENT_TIMESTAMP,
                    MODIFIED_DATE DATETIME,
                    PRIMARY KEY (MENU_ID, DEPT_CODE)
                );");
            
            ExecuteSql("CREATE INDEX IF NOT EXISTS IDX_DEPT_MENU_DEPT ON SYS_DEPT_MENU(DEPT_CODE);");
            ExecuteSql("CREATE INDEX IF NOT EXISTS IDX_DEPT_MENU_PARENT ON SYS_DEPT_MENU(PARENT_ID);");
            ExecuteSql("CREATE INDEX IF NOT EXISTS IDX_DEPT_MENU_SORT ON SYS_DEPT_MENU(DEPT_CODE, SORT_ORD);");

            // 사용자별 커스텀 메뉴
            ExecuteSql(@"
                CREATE TABLE IF NOT EXISTS SYS_USER_MENU (
                    MENU_ID VARCHAR(50) NOT NULL,
                    USER_ID VARCHAR(50) NOT NULL,
                    DEPT_CODE VARCHAR(10) NOT NULL,
                    PARENT_ID VARCHAR(50),
                    MENU_NAME VARCHAR(100) NOT NULL,
                    PROG_ID VARCHAR(50),
                    SORT_ORD INTEGER DEFAULT 0,
                    AUTH_LEVEL INTEGER DEFAULT 1,
                    CREATED_DATE DATETIME DEFAULT CURRENT_TIMESTAMP,
                    MODIFIED_DATE DATETIME,
                    PRIMARY KEY (MENU_ID, USER_ID, DEPT_CODE)
                );");
            
            ExecuteSql("CREATE INDEX IF NOT EXISTS IDX_USER_MENU_USER_DEPT ON SYS_USER_MENU(USER_ID, DEPT_CODE);");
            ExecuteSql("CREATE INDEX IF NOT EXISTS IDX_USER_MENU_PARENT ON SYS_USER_MENU(PARENT_ID);");
            ExecuteSql("CREATE INDEX IF NOT EXISTS IDX_USER_MENU_SORT ON SYS_USER_MENU(USER_ID, DEPT_CODE, SORT_ORD);");
        }

        private void SeedTestData()
        {
#if DEBUG
            try
            {
                var userCount = Convert.ToInt32(_db.ExecuteScalarValue("SELECT COUNT(*) FROM SYS_USER WHERE USER_ID IN ('user01', 'user02', 'user03')"));
                if (userCount > 0)
                {
                    _logger.LogInformation("Test users already exist. Skipping test data seeding.");
                    return;
                }

                _logger.LogInformation("Seeding test data for user/department menu system...");

                SeedTestUsers();
                SeedUserDepartmentMappings();
                SeedDepartmentMenuTemplates();
                SeedUserCustomMenus();

                _logger.LogInformation("Test data seeding completed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Failed to seed test data: {ex.Message}");
            }
#endif
        }

        private void SeedTestUsers()
        {
            var users = new[]
            {
                new { UserId = "user01", UserName = "홍길동", Password = "1234", Email = "hong@test.com",
                      RoleCode = ((int)UserRole.Doctor).ToString() },
                new { UserId = "user02", UserName = "김철수", Password = "1234", Email = "kim@test.com",
                      RoleCode = ((int)UserRole.Nurse).ToString() },
                new { UserId = "user03", UserName = "이영희", Password = "1234", Email = "lee@test.com",
                      RoleCode = ((int)UserRole.Doctor).ToString() }
            };

            foreach (var user in users)
            {
                _db.ExecuteNonQuery(@"
                    INSERT OR IGNORE INTO SYS_USER (USER_ID, USERNAME, PASSWORD, EMAIL, ROLE_CODE, IS_ACTIVE) 
                    VALUES (@id, @name, @pwd, @email, @role, 'Y')",
                    new Dictionary<string, object>
                    {
                        { "@id", user.UserId },
                        { "@name", user.UserName },
                        { "@pwd", user.Password },
                        { "@email", user.Email },
                        { "@role", user.RoleCode }
                    });
            }

            _logger.LogInformation("Test users seeded: user01, user02, user03");
        }

        private void SeedUserDepartmentMappings()
        {
            var mappings = new[]
            {
                new { UserId = "user01", DeptCode = "1", IsPrimary = "Y" },
                new { UserId = "user01", DeptCode = "2", IsPrimary = "N" },
                new { UserId = "user02", DeptCode = "3", IsPrimary = "Y" },
                new { UserId = "user03", DeptCode = "1", IsPrimary = "Y" },
                new { UserId = "user03", DeptCode = "4", IsPrimary = "N" },
                new { UserId = "user03", DeptCode = "5", IsPrimary = "N" },
                new { UserId = "admin", DeptCode = "1", IsPrimary = "Y" },
                new { UserId = "admin", DeptCode = "2", IsPrimary = "N" },
                new { UserId = "admin", DeptCode = "3", IsPrimary = "N" }
            };

            foreach (var mapping in mappings)
            {
                _db.ExecuteNonQuery(@"
                    INSERT OR IGNORE INTO SYS_USER_DEPT (USER_ID, DEPT_CODE, IS_PRIMARY) 
                    VALUES (@userId, @deptCode, @isPrimary)",
                    new Dictionary<string, object>
                    {
                        { "@userId", mapping.UserId },
                        { "@deptCode", mapping.DeptCode },
                        { "@isPrimary", mapping.IsPrimary }
                    });
            }

            _logger.LogInformation("User-Department mappings seeded (9 mappings)");
        }

        private void SeedDepartmentMenuTemplates()
        {
            var deptMenus = new[]
            {
                new { MenuId = "DEPT1_M01", DeptCode = "1", ParentId = (string?)null, MenuName = "내과 환자관리", ProgId = (string?)null, SortOrd = 10, AuthLevel = 1 },
                new { MenuId = "DEPT1_M02", DeptCode = "1", ParentId = (string?)"DEPT1_M01", MenuName = "외래 환자", ProgId = (string?)"EMR_001", SortOrd = 10, AuthLevel = 1 },
                new { MenuId = "DEPT1_M03", DeptCode = "1", ParentId = (string?)"DEPT1_M01", MenuName = "입원 환자", ProgId = (string?)"EMR_002", SortOrd = 20, AuthLevel = 1 },
                new { MenuId = "DEPT1_M04", DeptCode = "1", ParentId = (string?)null, MenuName = "내과 검사", ProgId = (string?)null, SortOrd = 20, AuthLevel = 1 },
                new { MenuId = "DEPT1_M05", DeptCode = "1", ParentId = (string?)"DEPT1_M04", MenuName = "내시경 검사", ProgId = (string?)"EMR_005", SortOrd = 10, AuthLevel = 2 },
                
                new { MenuId = "DEPT2_M01", DeptCode = "2", ParentId = (string?)null, MenuName = "외과 수술관리", ProgId = (string?)null, SortOrd = 10, AuthLevel = 1 },
                new { MenuId = "DEPT2_M02", DeptCode = "2", ParentId = (string?)"DEPT2_M01", MenuName = "수술 예약", ProgId = (string?)"OT_001", SortOrd = 10, AuthLevel = 1 },
                new { MenuId = "DEPT2_M03", DeptCode = "2", ParentId = (string?)"DEPT2_M01", MenuName = "수술 기록", ProgId = (string?)"OT_002", SortOrd = 20, AuthLevel = 1 },
                new { MenuId = "DEPT2_M04", DeptCode = "2", ParentId = (string?)null, MenuName = "외과 입원관리", ProgId = (string?)null, SortOrd = 20, AuthLevel = 1 },
                new { MenuId = "DEPT2_M05", DeptCode = "2", ParentId = (string?)"DEPT2_M04", MenuName = "병상 배정", ProgId = (string?)"ADM_003", SortOrd = 10, AuthLevel = 1 },
                
                new { MenuId = "DEPT3_M01", DeptCode = "3", ParentId = (string?)null, MenuName = "소아 진료", ProgId = (string?)null, SortOrd = 10, AuthLevel = 1 },
                new { MenuId = "DEPT3_M02", DeptCode = "3", ParentId = (string?)"DEPT3_M01", MenuName = "예방접종", ProgId = (string?)"EMR_010", SortOrd = 10, AuthLevel = 1 },
                new { MenuId = "DEPT3_M03", DeptCode = "3", ParentId = (string?)"DEPT3_M01", MenuName = "성장 발달", ProgId = (string?)"EMR_011", SortOrd = 20, AuthLevel = 1 }
            };

            foreach (var menu in deptMenus)
            {
                _db.ExecuteNonQuery(@"
                    INSERT OR IGNORE INTO SYS_DEPT_MENU 
                    (MENU_ID, DEPT_CODE, PARENT_ID, MENU_NAME, PROG_ID, SORT_ORD, AUTH_LEVEL) 
                    VALUES (@menuId, @deptCode, @parentId, @menuName, @progId, @sortOrd, @authLevel)",
                    new Dictionary<string, object>
                    {
                        { "@menuId", menu.MenuId },
                        { "@deptCode", menu.DeptCode },
                        { "@parentId", menu.ParentId ?? (object)DBNull.Value },
                        { "@menuName", menu.MenuName },
                        { "@progId", menu.ProgId ?? (object)DBNull.Value },
                        { "@sortOrd", menu.SortOrd },
                        { "@authLevel", menu.AuthLevel }
                    });
            }

            _logger.LogInformation("Department menu templates seeded (13 menus for 3 departments)");
        }

        private void SeedUserCustomMenus()
        {
            var userMenus = new[]
            {
                new { MenuId = "USER01_D1_M01", UserId = "user01", DeptCode = "1", ParentId = (string?)null, MenuName = "내 즐겨찾기", ProgId = (string?)null, SortOrd = 10, AuthLevel = 1 },
                new { MenuId = "USER01_D1_M02", UserId = "user01", DeptCode = "1", ParentId = (string?)"USER01_D1_M01", MenuName = "빠른 외래", ProgId = (string?)"EMR_001", SortOrd = 10, AuthLevel = 1 },
                new { MenuId = "USER01_D1_M03", UserId = "user01", DeptCode = "1", ParentId = (string?)"USER01_D1_M01", MenuName = "빠른 처방", ProgId = (string?)"EMR_004", SortOrd = 20, AuthLevel = 1 },
                new { MenuId = "USER01_D1_M04", UserId = "user01", DeptCode = "1", ParentId = (string?)null, MenuName = "내과 환자관리", ProgId = (string?)null, SortOrd = 20, AuthLevel = 1 },
                new { MenuId = "USER01_D1_M05", UserId = "user01", DeptCode = "1", ParentId = (string?)"USER01_D1_M04", MenuName = "외래 환자", ProgId = (string?)"EMR_001", SortOrd = 10, AuthLevel = 1 }
            };

            foreach (var menu in userMenus)
            {
                _db.ExecuteNonQuery(@"
                    INSERT OR IGNORE INTO SYS_USER_MENU 
                    (MENU_ID, USER_ID, DEPT_CODE, PARENT_ID, MENU_NAME, PROG_ID, SORT_ORD, AUTH_LEVEL) 
                    VALUES (@menuId, @userId, @deptCode, @parentId, @menuName, @progId, @sortOrd, @authLevel)",
                    new Dictionary<string, object>
                    {
                        { "@menuId", menu.MenuId },
                        { "@userId", menu.UserId },
                        { "@deptCode", menu.DeptCode },
                        { "@parentId", menu.ParentId ?? (object)DBNull.Value },
                        { "@menuName", menu.MenuName },
                        { "@progId", menu.ProgId ?? (object)DBNull.Value },
                        { "@sortOrd", menu.SortOrd },
                        { "@authLevel", menu.AuthLevel }
                    });
            }

            _logger.LogInformation("User custom menus seeded (5 menus for user01+내과)");
        }

        private void ExecuteSql(string sql)
        {
            _db.ExecuteNonQuery(sql);
        }
    }
}
