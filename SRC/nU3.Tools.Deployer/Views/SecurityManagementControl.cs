using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using nU3.Core.UI;
using nU3.Core.UI.Controls;
using nU3.Core.UI.Controls.Forms;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraEditors;
using System.Linq;
using nU3.Core.Enums;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using nU3.Core.Repositories;
using nU3.Connectivity;

namespace nU3.Tools.Deployer.Views
{
    public partial class SecurityManagementControl : BaseWorkControl
    {
        private readonly ISecurityRepository _securityRepo;
        private readonly IProgramRepository _progRepo;
        private readonly IModuleRepository _moduleRepo;
        
        private string _currentTargetType = ""; // "USER"(사용자) 또는 "ROLE"(역할) 또는 "DEPT"(부서)
        private string _currentTargetId = "";
        private string _currentTargetName = "";
        private string _currentProgId = "";

        public SecurityManagementControl()
        {
            InitializeComponent();
        }

        public SecurityManagementControl(ISecurityRepository securityRepo, IProgramRepository progRepo, IModuleRepository moduleRepo) : this()
        {
            _securityRepo = securityRepo;
            _progRepo = progRepo;
            _moduleRepo = moduleRepo;

            if (!IsDesignMode())
            {
                SetupGrids();
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
            AddGridColumn(gvUsers, "UserId", "ID", 100); // 사용자 입력 ID를 위해 너비 증가
            AddGridColumn(gvUsers, "Username", "이름", 100);
            AddGridColumn(gvUsers, "RoleName", "역할", 80); // Renamed RankName -> RoleName (직급명 -> 역할명 변경)
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
            // ParentCode 제거됨 (Enum은 표준 속성에서 계층 구조를 엄격히 지원하지 않음)

            // Modules Grid
            gvModules.Columns.Clear();
            AddGridColumn(gvModules, "ProgId", "프로그램ID", 100);
            AddGridColumn(gvModules, "ProgName", "프로그램명", 150);
            AddGridColumn(gvModules, "ModuleName", "모듈", 100);
            
            // 필터 허용
            gvModules.OptionsView.ShowAutoFilterRow = true;
        }

        private void AddGridColumn(GridView view, string fieldName, string caption, int width)
        {
            var col = view.Columns.AddVisible(fieldName, caption);
            col.Width = width;
        }

        private void LoadAllData()
        {
            LoadRoles();
            LoadDepts();
            LoadUsers();
            LoadModules();
        }

        #region Loading Data (데이터 로드)

        private void LoadUsers()
        {
            try
            {
                dgvUsers.DataSource = _securityRepo.GetAllSecurityUsers();
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
                dgvRoles.DataSource = _securityRepo.GetAllRoles();
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
                dgvDepts.DataSource = _securityRepo.GetAllDepartments();
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
                var programs = _progRepo.GetAllPrograms();
                var modules = _moduleRepo.GetAllModules();
                var moduleMap = modules.ToDictionary(m => m.ModuleId, m => m.ModuleName, StringComparer.OrdinalIgnoreCase);

                foreach (var p in programs)
                {
                    if (p.IsActive != "Y") continue;
                    list.Add(new SecurityProgDto
                    {
                        ProgId = p.ProgId,
                        ProgName = p.ProgName,
                        ModuleName = moduleMap.TryGetValue(p.ModuleId ?? "", out var mName) ? mName : p.ModuleId
                    });
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
                    if (_securityRepo.IsUserIdExists(form.UserId))
                    {
                        XtraMessageBox.Show("이미 존재하는 ID입니다.");
                        return;
                    }

                    _securityRepo.AddSecurityUser(new SecurityUserDto 
                    { 
                        UserId = form.UserId,
                        Username = form.Username,
                        Email = form.Email,
                        RoleCode = form.SelectedRoleCode
                    }, form.Password, form.SelectedDeptCodes);

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

            List<string> userDeptCodes = _securityRepo.GetUserDeptCodes(user.UserId);

            using var form = new EditUserForm(user, roles, depts, userDeptCodes);
            if (form.ShowDialog() == DialogResult.OK)
            {
                 try
                {
                    _securityRepo.UpdateSecurityUser(new SecurityUserDto
                    {
                        UserId = user.UserId,
                        Email = form.Email,
                        RoleCode = form.SelectedRoleCode
                    }, form.SelectedDeptCodes);

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
                    _securityRepo.DeleteSecurityUser(user.UserId);
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

                for (int i = 1; i <= 20; i++)
                {
                    string userId = $"TEST_USER_{i:00}";
                    string name = $"테스트유저_{i:00}";
                    string role = roles[rand.Next(roles.Length)];
                    string dept = depts[rand.Next(depts.Length)];

                    var user = new SecurityUserDto
                    {
                        UserId = userId,
                        Username = name,
                        Email = $"user{i}@test.com",
                        RoleCode = role
                    };

                    _securityRepo.AddSecurityUser(user, "1234", new List<string> { dept });
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
            string moduleInfo = string.IsNullOrEmpty(_currentProgId) ? "모듈 미선택" : _currentProgId;
            // You might want to lookup Module Name if possible, but _currentProgId is available.
            // Retrieving name from grid is possible but requires lookup.
            // Let's just show ID for now or try to get name from grid row if possible.
            var progName = "";
             var prog = gvModules.GetFocusedRow() as SecurityProgDto;
             if (prog != null) progName = prog.ProgName;

            lblCurrentTarget.Text = $"모듈: {progName} ({moduleInfo}) | 대상: {_currentTargetName}";
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
                var perm = _securityRepo.GetPermission(_currentTargetType, _currentTargetId, _currentProgId);

                if (perm != null)
                {
                    chkRead.Checked = perm.CanRead;
                    chkCreate.Checked = perm.CanCreate;
                    chkUpdate.Checked = perm.CanUpdate;
                    chkDelete.Checked = perm.CanDelete;
                    chkPrint.Checked = perm.CanPrint;
                    chkExport.Checked = perm.CanExport;
                    chkApprove.Checked = perm.CanApprove;
                    chkCancel.Checked = perm.CanCancel;
                }
                else
                {
                    ClearChecks();
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
                _securityRepo.SavePermission(new SecurityPermissionDto
                {
                    TargetType = _currentTargetType,
                    TargetId = _currentTargetId,
                    ProgId = _currentProgId,
                    CanRead = chkRead.Checked,
                    CanCreate = chkCreate.Checked,
                    CanUpdate = chkUpdate.Checked,
                    CanDelete = chkDelete.Checked,
                    CanPrint = chkPrint.Checked,
                    CanExport = chkExport.Checked,
                    CanApprove = chkApprove.Checked,
                    CanCancel = chkCancel.Checked
                });

                XtraMessageBox.Show("권한이 저장되었습니다.");
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"권한 저장 실패: {ex.Message}");
            }
        }

        #endregion

        #region DTOs & Forms

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