using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SQLite;
using System.Drawing;
using System.Windows.Forms;
using nU3.Data;
using nU3.Core.UI;
using nU3.Core.UI.Controls;
using nU3.Core.UI.Controls.Forms;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraEditors;
using System.Linq;
using nU3.Core.Enums;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace nU3.Tools.Deployer.Views
{
    public partial class SecurityManagementControl : BaseWorkControl
    {
        private readonly LocalDbService _dbManager;
        
        private string _currentTargetType = ""; // "USER" or "ROLE" or "DEPT"
        private string _currentTargetId = "";
        private string _currentTargetName = "";
        private string _currentProgId = "";

        public SecurityManagementControl()
        {
            _dbManager = new LocalDbService();
            // Ensure local SQLite schema exists before any DB operations
            // InitializeSchema logic is now in Server or implicit in LocalDbService if configured.
            // For standalone Deployer, we rely on existing DB or Server creation.
            InitializeComponent();

            if (!IsDesignMode())
            {
                SetupGrids();
                SyncEnumsToDb(); // Sync Enums to DB
                LoadAllData();
            }
        }

        private static bool IsDesignMode()
        {
            return LicenseManager.UsageMode == LicenseUsageMode.Designtime;
        }

        private void SetupGrids()
        {
            // Users Grid
            gvUsers.Columns.Clear();
            AddGridColumn(gvUsers, "UserId", "ID", 100); // Increased for User Input ID
            AddGridColumn(gvUsers, "Username", "이름", 100);
            AddGridColumn(gvUsers, "RoleName", "역할", 80); // Renamed RankName -> RoleName
            AddGridColumn(gvUsers, "DeptNames", "부서", 150);
            AddGridColumn(gvUsers, "Email", "이메일", 150);

            // Roles Grid
            gvRoles.Columns.Clear();
            AddGridColumn(gvRoles, "RoleCode", "역할코드", 80);
            AddGridColumn(gvRoles, "RoleName", "역할명", 120);
            AddGridColumn(gvRoles, "Description", "설명", 200);

            // Depts Grid
            gvDepts.Columns.Clear();
            AddGridColumn(gvDepts, "DeptCode", "부서코드", 80);
            AddGridColumn(gvDepts, "DeptName", "부서명", 120);
            // ParentCode removed as Enum doesn't strictly support hierarchy in standard attributes unless added.

            // Modules Grid
            gvModules.Columns.Clear();
            AddGridColumn(gvModules, "ProgId", "프로그램ID", 100);
            AddGridColumn(gvModules, "ProgName", "프로그램명", 150);
            AddGridColumn(gvModules, "ModuleName", "모듈", 100);
            
            // Allow filter
            gvModules.OptionsView.ShowAutoFilterRow = true;
        }

        private void AddGridColumn(GridView view, string fieldName, string caption, int width)
        {
            var col = view.Columns.AddVisible(fieldName, caption);
            col.Width = width;
        }

        private void SyncEnumsToDb()
        {
            try
            {
                using (var conn = new SQLiteConnection(_dbManager.GetConnectionString()))
                {
                    conn.Open();
                    using (var tx = conn.BeginTransaction())
                    {
                        // Sync Roles
                        foreach (UserRole role in Enum.GetValues(typeof(UserRole)))
                        {
                            string code = role.ToString();
                            string name = GetEnumDisplayName(role);
                            string desc = GetEnumDescription(role);

                            string sql = @"INSERT OR REPLACE INTO SYS_ROLE (ROLE_CODE, ROLE_NAME, DESCRIPTION) VALUES (@c, @n, @d)";
                            using (var cmd = new SQLiteCommand(sql, conn, tx))
                            {
                                cmd.Parameters.AddWithValue("@c", code);
                                cmd.Parameters.AddWithValue("@n", name);
                                cmd.Parameters.AddWithValue("@d", desc);
                                cmd.ExecuteNonQuery();
                            }
                        }

                        // Sync Depts
                        foreach (Department dept in Enum.GetValues(typeof(Department)))
                        {
                            string code = dept.ToString();
                            string name = GetEnumDisplayName(dept);
                            // Sort Order from Enum Order?
                            // For now just insert code/name
                            string sql = @"INSERT OR REPLACE INTO SYS_DEPT (DEPT_CODE, DEPT_NAME) VALUES (@c, @n)";
                            using (var cmd = new SQLiteCommand(sql, conn, tx))
                            {
                                cmd.Parameters.AddWithValue("@c", code);
                                cmd.Parameters.AddWithValue("@n", name);
                                cmd.ExecuteNonQuery();
                            }
                        }

                        tx.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                // Silent fail or log?
                Console.WriteLine($"Enum Sync Failed: {ex.Message}");
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

        private void LoadAllData()
        {
            LoadRoles();
            LoadDepts();
            LoadUsers();
            LoadModules();
        }

        #region Loading Data

        private void LoadUsers()
        {
            try
            {
                var list = new List<SecurityUserDto>();
                using (var conn = new SQLiteConnection(_dbManager.GetConnectionString()))
                {
                    conn.Open();
                    // Changed RANK_CODE -> ROLE_CODE, SYS_RANK -> SYS_ROLE
                    string sql = @"
                        SELECT u.USER_ID, u.USERNAME, u.EMAIL, u.ROLE_CODE, r.ROLE_NAME
                        FROM SYS_USER u
                        LEFT JOIN SYS_ROLE r ON u.ROLE_CODE = r.ROLE_CODE
                        WHERE u.IS_ACTIVE = 'Y'
                        ORDER BY u.USERNAME";

                    using (var cmd = new SQLiteCommand(sql, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var user = new SecurityUserDto
                            {
                                UserId = reader["USER_ID"]?.ToString() ?? "",
                                Username = reader["USERNAME"]?.ToString() ?? "",
                                Email = reader["EMAIL"]?.ToString() ?? "",
                                RoleCode = reader["ROLE_CODE"]?.ToString() ?? "", // Was RankCode
                                RoleName = reader["ROLE_NAME"]?.ToString() ?? ""  // Was RankName
                            };
                            list.Add(user);
                        }
                    }

                    // Load Depts for each user
                    foreach (var user in list)
                    {
                        sql = @"
                            SELECT d.DEPT_NAME 
                            FROM SYS_USER_DEPT ud
                            JOIN SYS_DEPT d ON ud.DEPT_CODE = d.DEPT_CODE
                            WHERE ud.USER_ID = @uid";
                        using (var cmd = new SQLiteCommand(sql, conn))
                        {
                            cmd.Parameters.AddWithValue("@uid", user.UserId);
                            using (var reader = cmd.ExecuteReader())
                            {
                                var depts = new List<string>();
                                while (reader.Read()) depts.Add(reader["DEPT_NAME"].ToString());
                                user.DeptNames = string.Join(", ", depts);
                            }
                        }
                    }
                }
                dgvUsers.DataSource = list;
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"사용자 로드 실패: {ex.Message}");
            }
        }

        private void LoadRoles()
        {
            try
            {
                var list = new List<SecurityRoleDto>();
                using (var conn = new SQLiteConnection(_dbManager.GetConnectionString()))
                {
                    conn.Open();
                    string sql = "SELECT ROLE_CODE, ROLE_NAME, DESCRIPTION FROM SYS_ROLE ORDER BY ROLE_CODE";

                    using (var cmd = new SQLiteCommand(sql, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new SecurityRoleDto
                            {
                                RoleCode = reader["ROLE_CODE"]?.ToString() ?? "",
                                RoleName = reader["ROLE_NAME"]?.ToString() ?? "",
                                Description = reader["DESCRIPTION"]?.ToString() ?? ""
                            });
                        }
                    }
                }
                dgvRoles.DataSource = list;
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"역할 로드 실패: {ex.Message}");
            }
        }

        private void LoadDepts()
        {
            try
            {
                var list = new List<SecurityDeptDto>();
                using (var conn = new SQLiteConnection(_dbManager.GetConnectionString()))
                {
                    conn.Open();
                    string sql = "SELECT DEPT_CODE, DEPT_NAME FROM SYS_DEPT ORDER BY DEPT_NAME";

                    using (var cmd = new SQLiteCommand(sql, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new SecurityDeptDto
                            {
                                DeptCode = reader["DEPT_CODE"]?.ToString() ?? "",
                                DeptName = reader["DEPT_NAME"]?.ToString() ?? ""
                            });
                        }
                    }
                }
                dgvDepts.DataSource = list;
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"부서 로드 실패: {ex.Message}");
            }
        }

        private void LoadModules()
        {
            try
            {
                var list = new List<SecurityProgDto>();
                using (var conn = new SQLiteConnection(_dbManager.GetConnectionString()))
                {
                    conn.Open();
                    string sql = @"
                        SELECT p.PROG_ID, p.PROG_NAME, m.MODULE_NAME
                        FROM SYS_PROG_MST p
                        LEFT JOIN SYS_MODULE_MST m ON p.MODULE_ID = m.MODULE_ID
                        WHERE p.IS_ACTIVE = 'Y'
                        ORDER BY p.PROG_NAME";

                    using (var cmd = new SQLiteCommand(sql, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new SecurityProgDto
                            {
                                ProgId = reader["PROG_ID"]?.ToString() ?? "",
                                ProgName = reader["PROG_NAME"]?.ToString() ?? "",
                                ModuleName = reader["MODULE_NAME"]?.ToString() ?? ""
                            });
                        }
                    }
                }
                dgvModules.DataSource = list;
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"모듈 로드 실패: {ex.Message}");
            }
        }

        #endregion

        #region User Management

        private void BtnAddUser_Click(object sender, EventArgs e)
        {
            var roles = (List<SecurityRoleDto>)dgvRoles.DataSource ?? new List<SecurityRoleDto>();
            var depts = (List<SecurityDeptDto>)dgvDepts.DataSource ?? new List<SecurityDeptDto>();
            
            using var form = new AddUserForm(roles, depts);
            if (form.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (var conn = new SQLiteConnection(_dbManager.GetConnectionString()))
                    {
                        conn.Open();
                        
                        // Check duplicate ID
                        using (var checkCmd = new SQLiteCommand("SELECT COUNT(*) FROM SYS_USER WHERE USER_ID = @id", conn))
                        {
                            checkCmd.Parameters.AddWithValue("@id", form.UserId);
                            if (Convert.ToInt32(checkCmd.ExecuteScalar()) > 0)
                            {
                                XtraMessageBox.Show("이미 존재하는 ID입니다.");
                                return;
                            }
                        }

                        using (var tx = conn.BeginTransaction())
                        {
                            // Insert User
                            string sql = @"
                                INSERT INTO SYS_USER (USER_ID, USERNAME, PASSWORD, EMAIL, ROLE_CODE, IS_ACTIVE)
                                VALUES (@id, @name, @pwd, @email, @role, 'Y')";
                            using (var cmd = new SQLiteCommand(sql, conn, tx))
                            {
                                cmd.Parameters.AddWithValue("@id", form.UserId); // Input ID
                                cmd.Parameters.AddWithValue("@name", form.Username);
                                cmd.Parameters.AddWithValue("@pwd", form.Password);
                                cmd.Parameters.AddWithValue("@email", form.Email);
                                cmd.Parameters.AddWithValue("@role", form.SelectedRoleCode);
                                cmd.ExecuteNonQuery();
                            }

                            // Insert Depts
                            foreach (var deptCode in form.SelectedDeptCodes)
                            {
                                string sqlDept = "INSERT INTO SYS_USER_DEPT (USER_ID, DEPT_CODE) VALUES (@uid, @dcode)";
                                using (var cmd = new SQLiteCommand(sqlDept, conn, tx))
                                {
                                    cmd.Parameters.AddWithValue("@uid", form.UserId);
                                    cmd.Parameters.AddWithValue("@dcode", deptCode);
                                    cmd.ExecuteNonQuery();
                                }
                            }

                            tx.Commit();
                        }
                    }
                    LoadUsers();
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show($"사용자 추가 실패: {ex.Message}");
                }
            }
        }

        private void BtnEditUser_Click(object sender, EventArgs e)
        {
            var user = gvUsers.GetFocusedRow() as SecurityUserDto;
            if (user == null) return;

            var roles = (List<SecurityRoleDto>)dgvRoles.DataSource ?? new List<SecurityRoleDto>();
            var depts = (List<SecurityDeptDto>)dgvDepts.DataSource ?? new List<SecurityDeptDto>();

            // Get current user depts
            List<string> userDeptCodes = new List<string>();
            try
            {
                using (var conn = new SQLiteConnection(_dbManager.GetConnectionString()))
                {
                    conn.Open();
                    string sql = "SELECT DEPT_CODE FROM SYS_USER_DEPT WHERE USER_ID = @uid";
                    using (var cmd = new SQLiteCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@uid", user.UserId);
                        using (var reader = cmd.ExecuteReader())
                        {
                            while(reader.Read()) userDeptCodes.Add(reader["DEPT_CODE"].ToString());
                        }
                    }
                }
            }
            catch { }

            using var form = new EditUserForm(user, roles, depts, userDeptCodes);
            if (form.ShowDialog() == DialogResult.OK)
            {
                 try
                {
                    using (var conn = new SQLiteConnection(_dbManager.GetConnectionString()))
                    {
                        conn.Open();
                        using (var tx = conn.BeginTransaction())
                        {
                            // Update User
                            string sql = @"UPDATE SYS_USER SET EMAIL = @email, ROLE_CODE = @role WHERE USER_ID = @id";
                            using (var cmd = new SQLiteCommand(sql, conn, tx))
                            {
                                cmd.Parameters.AddWithValue("@email", form.Email);
                                cmd.Parameters.AddWithValue("@role", form.SelectedRoleCode);
                                cmd.Parameters.AddWithValue("@id", user.UserId);
                                cmd.ExecuteNonQuery();
                            }

                            // Update Depts (Delete all & Insert)
                            string sqlDel = "DELETE FROM SYS_USER_DEPT WHERE USER_ID = @uid";
                            using (var cmd = new SQLiteCommand(sqlDel, conn, tx))
                            {
                                cmd.Parameters.AddWithValue("@uid", user.UserId);
                                cmd.ExecuteNonQuery();
                            }

                            foreach (var deptCode in form.SelectedDeptCodes)
                            {
                                string sqlDept = "INSERT INTO SYS_USER_DEPT (USER_ID, DEPT_CODE) VALUES (@uid, @dcode)";
                                using (var cmd = new SQLiteCommand(sqlDept, conn, tx))
                                {
                                    cmd.Parameters.AddWithValue("@uid", user.UserId);
                                    cmd.Parameters.AddWithValue("@dcode", deptCode);
                                    cmd.ExecuteNonQuery();
                                }
                            }
                            tx.Commit();
                        }
                    }
                    LoadUsers();
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show($"사용자 수정 실패: {ex.Message}");
                }
            }
        }

        private void BtnDeleteUser_Click(object sender, EventArgs e)
        {
            var user = gvUsers.GetFocusedRow() as SecurityUserDto;
            if (user == null) return;

            if (XtraMessageBox.Show($"사용자 '{user.Username}'을(를) 삭제하시겠습니까?", "삭제", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                try
                {
                    using (var conn = new SQLiteConnection(_dbManager.GetConnectionString()))
                    {
                        conn.Open();
                        string sql = "UPDATE SYS_USER SET IS_ACTIVE = 'N' WHERE USER_ID = @id";
                        using (var cmd = new SQLiteCommand(sql, conn))
                        {
                            cmd.Parameters.AddWithValue("@id", user.UserId);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    LoadUsers();
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show($"사용자 삭제 실패: {ex.Message}");
                }
            }
        }

        private void BtnSeedTestData_Click(object sender, EventArgs e)
        {
            if (XtraMessageBox.Show("테스트 데이터 20건을 생성하시겠습니까?", "확인", MessageBoxButtons.YesNo) != DialogResult.Yes) return;

            try
            {
                var rand = new Random();
                var roles = Enum.GetNames(typeof(UserRole));
                var depts = Enum.GetNames(typeof(Department));

                using (var conn = new SQLiteConnection(_dbManager.GetConnectionString()))
                {
                    conn.Open();
                    using (var tx = conn.BeginTransaction())
                    {
                        for (int i = 1; i <= 20; i++)
                        {
                            string userId = $"TEST_USER_{i:00}";
                            string name = $"테스트유저_{i:00}";
                            string role = roles[rand.Next(roles.Length)];
                            string dept = depts[rand.Next(depts.Length)];

                            // Insert User
                            string sql = @"
                                INSERT OR REPLACE INTO SYS_USER (USER_ID, USERNAME, PASSWORD, EMAIL, ROLE_CODE, IS_ACTIVE)
                                VALUES (@id, @name, '1234', @email, @role, 'Y')";
                            using (var cmd = new SQLiteCommand(sql, conn, tx))
                            {
                                cmd.Parameters.AddWithValue("@id", userId);
                                cmd.Parameters.AddWithValue("@name", name);
                                cmd.Parameters.AddWithValue("@email", $"user{i}@test.com");
                                cmd.Parameters.AddWithValue("@role", role);
                                cmd.ExecuteNonQuery();
                            }

                            // Insert Dept
                            string sqlDept = @"INSERT OR REPLACE INTO SYS_USER_DEPT (USER_ID, DEPT_CODE) VALUES (@uid, @dcode)";
                            using (var cmd = new SQLiteCommand(sqlDept, conn, tx))
                            {
                                cmd.Parameters.AddWithValue("@uid", userId);
                                cmd.Parameters.AddWithValue("@dcode", dept);
                                cmd.ExecuteNonQuery();
                            }
                        }
                        tx.Commit();
                    }
                }
                LoadUsers();
                XtraMessageBox.Show("테스트 데이터 생성 완료");
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"데이터 생성 실패: {ex.Message}");
            }
        }

        #endregion

        #region Permission Logic

        private void gvUsers_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            if (tabTargets.SelectedTabPage != pageUsers) return;

            var user = gvUsers.GetFocusedRow() as SecurityUserDto;
            if (user != null)
            {
                _currentTargetType = "USER";
                _currentTargetId = user.UserId;
                _currentTargetName = $"{user.Username} (사용자)";
                UpdateTargetLabel();
                LoadPermission();
            }
        }

        private void gvRoles_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            if (tabTargets.SelectedTabPage != pageRoles) return;

            var role = gvRoles.GetFocusedRow() as SecurityRoleDto;
            if (role != null)
            {
                _currentTargetType = "ROLE";
                _currentTargetId = role.RoleCode;
                _currentTargetName = $"{role.RoleName} (역할)";
                UpdateTargetLabel();
                LoadPermission();
            }
        }

        private void gvDepts_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            if (tabTargets.SelectedTabPage != pageDepts) return;

            var dept = gvDepts.GetFocusedRow() as SecurityDeptDto;
            if (dept != null)
            {
                _currentTargetType = "DEPT";
                _currentTargetId = dept.DeptCode;
                _currentTargetName = $"{dept.DeptName} (부서)";
                UpdateTargetLabel();
                LoadPermission();
            }
        }

        private void tabTargets_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            // Trigger selection update based on active page
            if (e.Page == pageUsers) gvUsers_FocusedRowChanged(gvUsers, new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs(DevExpress.XtraGrid.GridControl.InvalidRowHandle, gvUsers.FocusedRowHandle));
            else if (e.Page == pageRoles) gvRoles_FocusedRowChanged(gvRoles, new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs(DevExpress.XtraGrid.GridControl.InvalidRowHandle, gvRoles.FocusedRowHandle));
            else if (e.Page == pageDepts) gvDepts_FocusedRowChanged(gvDepts, new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs(DevExpress.XtraGrid.GridControl.InvalidRowHandle, gvDepts.FocusedRowHandle));
        }

        private void gvModules_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            var prog = gvModules.GetFocusedRow() as SecurityProgDto;
            if (prog != null)
            {
                _currentProgId = prog.ProgId;
                LoadPermission();
            }
            else
            {
                _currentProgId = "";
                ClearChecks();
            }
        }

        private void UpdateTargetLabel()
        {
            lblCurrentTarget.Text = $"선택된 대상: {_currentTargetName}";
        }

        private void ClearChecks()
        {
            chkRead.Checked = false;
            chkCreate.Checked = false;
            chkUpdate.Checked = false;
            chkDelete.Checked = false;
            chkPrint.Checked = false;
            chkExport.Checked = false;
            chkApprove.Checked = false;
            chkCancel.Checked = false;
        }

        private void LoadPermission()
        {
            if (string.IsNullOrEmpty(_currentTargetType) || string.IsNullOrEmpty(_currentTargetId) || string.IsNullOrEmpty(_currentProgId))
            {
                ClearChecks();
                return;
            }

            try
            {
                using (var conn = new SQLiteConnection(_dbManager.GetConnectionString()))
                {
                    conn.Open();
                    string sql = @"
                        SELECT CAN_READ, CAN_CREATE, CAN_UPDATE, CAN_DELETE, CAN_PRINT, CAN_EXPORT, CAN_APPROVE, CAN_CANCEL
                        FROM SYS_PERMISSION
                        WHERE TARGET_TYPE = @type AND TARGET_ID = @id AND PROG_ID = @prog";

                    using (var cmd = new SQLiteCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@type", _currentTargetType);
                        cmd.Parameters.AddWithValue("@id", _currentTargetId);
                        cmd.Parameters.AddWithValue("@prog", _currentProgId);
                        
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                chkRead.Checked = Convert.ToInt32(reader["CAN_READ"]) == 1;
                                chkCreate.Checked = Convert.ToInt32(reader["CAN_CREATE"]) == 1;
                                chkUpdate.Checked = Convert.ToInt32(reader["CAN_UPDATE"]) == 1;
                                chkDelete.Checked = Convert.ToInt32(reader["CAN_DELETE"]) == 1;
                                chkPrint.Checked = Convert.ToInt32(reader["CAN_PRINT"]) == 1;
                                chkExport.Checked = Convert.ToInt32(reader["CAN_EXPORT"]) == 1;
                                chkApprove.Checked = Convert.ToInt32(reader["CAN_APPROVE"]) == 1;
                                chkCancel.Checked = Convert.ToInt32(reader["CAN_CANCEL"]) == 1;
                            }
                            else
                            {
                                ClearChecks();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"권한 로드 실패: {ex.Message}");
            }
        }

        private void BtnSavePermission_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_currentTargetType) || string.IsNullOrEmpty(_currentTargetId) || string.IsNullOrEmpty(_currentProgId))
            {
                XtraMessageBox.Show("대상과 프로그램을 선택하세요.");
                return;
            }

            try
            {
                using (var conn = new SQLiteConnection(_dbManager.GetConnectionString()))
                {
                    conn.Open();
                    string sql = @"
                        INSERT INTO SYS_PERMISSION (TARGET_TYPE, TARGET_ID, PROG_ID, CAN_READ, CAN_CREATE, CAN_UPDATE, CAN_DELETE, CAN_PRINT, CAN_EXPORT, CAN_APPROVE, CAN_CANCEL)
                        VALUES (@type, @id, @prog, @r, @c, @u, @d, @p, @e, @a, @x)
                        ON CONFLICT(TARGET_TYPE, TARGET_ID, PROG_ID) DO UPDATE SET
                            CAN_READ = @r,
                            CAN_CREATE = @c,
                            CAN_UPDATE = @u,
                            CAN_DELETE = @d,
                            CAN_PRINT = @p,
                            CAN_EXPORT = @e,
                            CAN_APPROVE = @a,
                            CAN_CANCEL = @x";

                    using (var cmd = new SQLiteCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@type", _currentTargetType);
                        cmd.Parameters.AddWithValue("@id", _currentTargetId);
                        cmd.Parameters.AddWithValue("@prog", _currentProgId);
                        cmd.Parameters.AddWithValue("@r", chkRead.Checked ? 1 : 0);
                        cmd.Parameters.AddWithValue("@c", chkCreate.Checked ? 1 : 0);
                        cmd.Parameters.AddWithValue("@u", chkUpdate.Checked ? 1 : 0);
                        cmd.Parameters.AddWithValue("@d", chkDelete.Checked ? 1 : 0);
                        cmd.Parameters.AddWithValue("@p", chkPrint.Checked ? 1 : 0);
                        cmd.Parameters.AddWithValue("@e", chkExport.Checked ? 1 : 0);
                        cmd.Parameters.AddWithValue("@a", chkApprove.Checked ? 1 : 0);
                        cmd.Parameters.AddWithValue("@x", chkCancel.Checked ? 1 : 0);
                        cmd.ExecuteNonQuery();
                    }
                }
                XtraMessageBox.Show("권한이 저장되었습니다.");
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"권한 저장 실패: {ex.Message}");
            }
        }

        #endregion

        #region DTOs & Forms

        public class SecurityUserDto
        {
            public string UserId { get; set; }
            public string Username { get; set; }
            public string Email { get; set; }
            public string RoleCode { get; set; }
            public string RoleName { get; set; }
            public string DeptNames { get; set; }
        }

        public class SecurityRoleDto
        {
            public string RoleCode { get; set; }
            public string RoleName { get; set; }
            public string Description { get; set; }
            public override string ToString() => RoleName;
        }

        public class SecurityDeptDto
        {
            public string DeptCode { get; set; }
            public string DeptName { get; set; }
            public override string ToString() => DeptName;
        }

        public class SecurityProgDto
        {
            public string ProgId { get; set; }
            public string ProgName { get; set; }
            public string ModuleName { get; set; }
        }

        public class AddUserForm : BaseWorkForm
        {
            private nU3TextEdit txtId = new nU3TextEdit(); // Added Input ID
            private nU3TextEdit txtName = new nU3TextEdit();
            private nU3TextEdit txtPwd = new nU3TextEdit { Properties = { PasswordChar = '*' } };
            private nU3TextEdit txtEmail = new nU3TextEdit();
            private System.Windows.Forms.ComboBox cmbRole = new System.Windows.Forms.ComboBox();
            private CheckedComboBoxEdit cmbDepts = new CheckedComboBoxEdit();
            private nU3SimpleButton btnSave = new nU3SimpleButton { Text = "저장" };
            
            public string UserId => txtId.Text;
            public string Username => txtName.Text;
            public string Password => txtPwd.Text;
            public string Email => txtEmail.Text;
            public string SelectedRoleCode => (cmbRole.SelectedItem as SecurityRoleDto)?.RoleCode ?? "";
            public List<string> SelectedDeptCodes 
            {
                get 
                {
                    var checkedItems = cmbDepts.Properties.Items.Where(i => i.CheckState == CheckState.Checked).ToList();
                    return checkedItems.Select(i => i.Value.ToString()).ToList();
                }
            }

            public AddUserForm(List<SecurityRoleDto> roles, List<SecurityDeptDto> depts)
            {
                Text = "사용자 추가";
                Size = new Size(350, 350);
                StartPosition = FormStartPosition.CenterParent;
                
                var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, Padding = new Padding(10) };
                layout.Controls.Add(new nU3LabelControl { Text = "ID:" }); layout.Controls.Add(txtId);
                layout.Controls.Add(new nU3LabelControl { Text = "이름:" }); layout.Controls.Add(txtName);
                layout.Controls.Add(new nU3LabelControl { Text = "비밀번호:" }); layout.Controls.Add(txtPwd);
                layout.Controls.Add(new nU3LabelControl { Text = "이메일:" }); layout.Controls.Add(txtEmail);
                layout.Controls.Add(new nU3LabelControl { Text = "역할:" }); layout.Controls.Add(cmbRole);
                layout.Controls.Add(new nU3LabelControl { Text = "부서:" }); layout.Controls.Add(cmbDepts);

                cmbRole.DataSource = roles;
                cmbRole.DisplayMember = "RoleName";
                cmbRole.ValueMember = "RoleCode";
                cmbRole.Dock = DockStyle.Fill;
                
                cmbDepts.Properties.DataSource = depts;
                cmbDepts.Properties.DisplayMember = "DeptName";
                cmbDepts.Properties.ValueMember = "DeptCode";
                cmbDepts.Dock = DockStyle.Fill;

                txtId.Dock = DockStyle.Fill; txtName.Dock = DockStyle.Fill; txtPwd.Dock = DockStyle.Fill; txtEmail.Dock = DockStyle.Fill;

                var pnlBtn = new FlowLayoutPanel { Dock = DockStyle.Bottom, FlowDirection = FlowDirection.RightToLeft };
                btnSave.Click += (s, e) => {
                    if(string.IsNullOrWhiteSpace(txtId.Text)) { XtraMessageBox.Show("ID를 입력하세요"); return; }
                    DialogResult = DialogResult.OK; Close(); 
                };
                pnlBtn.Controls.Add(btnSave);

                Controls.Add(layout);
                Controls.Add(pnlBtn);
            }
        }

        public class EditUserForm : BaseWorkForm
        {
            private nU3TextEdit txtEmail = new nU3TextEdit();
            private System.Windows.Forms.ComboBox cmbRole = new System.Windows.Forms.ComboBox();
            private CheckedComboBoxEdit cmbDepts = new CheckedComboBoxEdit();
            private nU3SimpleButton btnSave = new nU3SimpleButton { Text = "저장" };

            public string Email => txtEmail.Text;
            public string SelectedRoleCode => (cmbRole.SelectedItem as SecurityRoleDto)?.RoleCode ?? "";
            public List<string> SelectedDeptCodes 
            {
                get 
                {
                    var checkedItems = cmbDepts.Properties.Items.Where(i => i.CheckState == CheckState.Checked).ToList();
                    return checkedItems.Select(i => i.Value.ToString()).ToList();
                }
            }

            public EditUserForm(SecurityUserDto user, List<SecurityRoleDto> roles, List<SecurityDeptDto> depts, List<string> userDeptCodes)
            {
                Text = "사용자 수정";
                Size = new Size(350, 250);
                StartPosition = FormStartPosition.CenterParent;

                var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, Padding = new Padding(10) };
                layout.Controls.Add(new nU3LabelControl { Text = "이메일:" }); layout.Controls.Add(txtEmail);
                layout.Controls.Add(new nU3LabelControl { Text = "역할:" }); layout.Controls.Add(cmbRole);
                layout.Controls.Add(new nU3LabelControl { Text = "부서:" }); layout.Controls.Add(cmbDepts);

                cmbRole.DataSource = roles;
                cmbRole.DisplayMember = "RoleName";
                cmbRole.ValueMember = "RoleCode";
                cmbRole.Dock = DockStyle.Fill;
                
                cmbDepts.Properties.DataSource = depts;
                cmbDepts.Properties.DisplayMember = "DeptName";
                cmbDepts.Properties.ValueMember = "DeptCode";
                cmbDepts.Dock = DockStyle.Fill;
                
                txtEmail.Dock = DockStyle.Fill;
                txtEmail.Text = user.Email;

                foreach(var r in roles) {
                    if (r.RoleCode == user.RoleCode) { cmbRole.SelectedItem = r; break; }
                }
                
                if (userDeptCodes != null && userDeptCodes.Count > 0)
                {
                    cmbDepts.SetEditValue(string.Join(cmbDepts.Properties.SeparatorChar.ToString(), userDeptCodes));
                }

                var pnlBtn = new FlowLayoutPanel { Dock = DockStyle.Bottom, FlowDirection = FlowDirection.RightToLeft };
                btnSave.Click += (s, e) => { DialogResult = DialogResult.OK; Close(); };
                pnlBtn.Controls.Add(btnSave);

                Controls.Add(layout);
                Controls.Add(pnlBtn);
            }
        }
        #endregion
    }
}