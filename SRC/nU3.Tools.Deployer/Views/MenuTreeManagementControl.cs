using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using nU3.Core.UI;
using nU3.Core.UI.Controls;
using DevExpress.XtraEditors;
using nU3.Connectivity;
using nU3.Core.Repositories;
using nU3.Models;
using System.Linq;

namespace nU3.Tools.Deployer.Views
{
    /// <summary>
    /// 메뉴 트리(Menu Navigation Tree)를 관리하고 편집하는 컨트롤입니다.
    /// 주요 기능:
    /// - 사용자별/부서별 메뉴 트리 관리
    /// - 전체 프로그램 목록의 로드
    /// - SYS_USER_MENU, SYS_DEPT_MENU, SYS_MENU 3단계 폴백 구조
    /// - 새 항목 추가(루트/자식), 편집, 삭제 기능
    /// </summary>
    public partial class MenuTreeManagementControl : BaseWorkControl
    {
        private readonly IDBAccessService _db;
        private readonly IMenuRepository _menuRepo;
        private readonly IProgramRepository _progRepo;
        private readonly IUserRepository _userRepo;
        private readonly IDepartmentRepository _deptRepo;

        private string? _currentUserId;
        private string? _currentDeptCode;

        /// <summary>
        /// Designer 전용 생성자
        /// </summary>
        public MenuTreeManagementControl()
        {
            InitializeComponent();
        }

        public MenuTreeManagementControl(
            IDBAccessService db,
            IMenuRepository menuRepo,
            IProgramRepository progRepo,
            IUserRepository userRepo,
            IDepartmentRepository deptRepo) : this()
        {
            _db = db;
            _menuRepo = menuRepo;
            _progRepo = progRepo;
            _userRepo = userRepo;
            _deptRepo = deptRepo;

            if (!IsDesignMode())
            {
                LoadPrograms();
                LoadUsers();
                WireEvents();
            }
        }

        private static bool IsDesignMode()
        {
            return LicenseManager.UsageMode == LicenseUsageMode.Designtime;
        }

        private void WireEvents()
        {
            lbPrograms.DoubleClick += LbPrograms_DoubleClick;
            cmsTree.Items.Add("루트 추가", null, CmsAddRoot_Click);
            cmsTree.Items.Add("자식 추가", null, CmsAddChild_Click);
            cmsTree.Items.Add("삭제", null, CmsDelete_Click);
        }

        private void TvMenu_AfterSelect(object sender, TreeViewEventArgs e)
        {
            UpdateButtons();
            if (tvMenu.SelectedNode != null)
            {
                var data = tvMenu.SelectedNode.Tag as MenuNodeData;
                if (data != null)
                {
                    numAuthLevel.Value = data.AuthLevel < 1 ? 1 : data.AuthLevel;
                }
            }
        }

        private void NumAuthLevel_ValueChanged(object sender, EventArgs e)
        {
            if (tvMenu.SelectedNode != null)
            {
                var data = tvMenu.SelectedNode.Tag as MenuNodeData;
                if (data != null)
                {
                    data.AuthLevel = (int)numAuthLevel.Value;
                }
            }
        }

        private void CmsAddRoot_Click(object? sender, EventArgs e) => AddRootNode();
        private void CmsAddChild_Click(object? sender, EventArgs e) => AddChildNode();
        private void CmsDelete_Click(object? sender, EventArgs e) => DeleteNode();

        private void BtnAddRoot_Click(object sender, EventArgs e) => AddRootNode();
        private void BtnAddChild_Click(object sender, EventArgs e) => AddChildNode();
        private void BtnDeleteNode_Click(object sender, EventArgs e) => DeleteNode();
        private void BtnSave_Click(object sender, EventArgs e) => SaveMenu();

        private void UpdateButtons()
        {
            bool hasSelection = tvMenu.SelectedNode != null;
            btnAddChild.Enabled = hasSelection;
            btnDeleteNode.Enabled = hasSelection;
            numAuthLevel.Enabled = hasSelection;
        }

        private void LoadPrograms()
        {
            lbPrograms.Items.Clear();
            lbPrograms.DisplayMember = "DisplayInfo";

            var programs = _progRepo.GetAllPrograms();
            foreach (var p in programs)
            {
                if (p.IsActive != "Y") continue;

                var item = new ProgramItem();
                item.ProgId = p.ProgId;
                item.ProgName = p.ProgName;
                item.ModuleId = p.ModuleId;
                item.IsActive = p.IsActive;
                item.ProgType = p.ProgType;
                lbPrograms.Items.Add(item);
            }
        }

        private async void LoadUsers()
        {
            lbUsers.Items.Clear();
            lbUsers.DisplayMember = "DisplayText";

            var users = await _userRepo.GetAllUsersAsync();
            foreach (var user in users)
            {
                var item = new UserItem
                {
                    UserId = user.UserId,
                    UserName = user.UserName
                };
                lbUsers.Items.Add(item);
            }
        }

        private async void LbUsers_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (lbUsers.SelectedItem is not UserItem userItem) return;

            _currentUserId = userItem.UserId;
            _currentDeptCode = null;

            lbDepts.Items.Clear();
            tvMenu.Nodes.Clear();
            btnSave.Enabled = false;

            var depts = await _deptRepo.GetDepartmentsByUserIdAsync(_currentUserId);
            lbDepts.DisplayMember = "DisplayText";

            foreach (var dept in depts)
            {
                lbDepts.Items.Add(dept);
            }
        }

        private void LbDepts_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (lbDepts.SelectedItem is not DepartmentDto deptDto) return;

            _currentDeptCode = deptDto.DeptCode;
            LoadMenuTree(_currentUserId, _currentDeptCode);
            btnSave.Enabled = true;
        }

        private void LoadMenuTree(string? userId, string? deptCode)
        {
            tvMenu.Nodes.Clear();

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(deptCode))
                return;

            List<MenuDto> allMenus;

            var userMenus = _menuRepo.GetMenusByUserAndDept(userId, deptCode);
            if (userMenus.Count > 0)
            {
                allMenus = userMenus;
            }
            else
            {
                var deptMenus = _menuRepo.GetMenusByDeptCode(deptCode);
                if (deptMenus.Count > 0)
                {
                    allMenus = deptMenus;
                }
                else
                {
                    allMenus = _menuRepo.GetAllMenus();
                }
            }

            AddTreeNodes(allMenus, null, tvMenu.Nodes);
            tvMenu.ExpandAll();
        }

        private void BtnRefreshAll_Click(object sender, EventArgs e)
        {
            LoadUsers();
            LoadPrograms();
            lbDepts.Items.Clear();
            tvMenu.Nodes.Clear();
            _currentUserId = null;
            _currentDeptCode = null;
            btnSave.Enabled = false;
        }

        private void BtnRefreshPrograms_Click(object sender, EventArgs e)
        {
            BtnRefreshAll_Click(sender, e);
        }

        private void AddTreeNodes(List<MenuDto> allMenus, string? parentId, TreeNodeCollection nodes)
        {
            var children = allMenus
                .Where(m => m.ParentId == parentId)
                .OrderBy(m => m.SortOrd)
                .ToList();

            foreach (var menu in children)
            {
                var data = new MenuNodeData
                {
                    MenuId = menu.MenuId,
                    ProgId = menu.ProgId,
                    AuthLevel = menu.AuthLevel
                };
                var node = new TreeNode(menu.MenuName) { Tag = data };

                if (!string.IsNullOrEmpty(menu.ProgId)) node.ForeColor = Color.Blue;

                nodes.Add(node);
                AddTreeNodes(allMenus, menu.MenuId, node.Nodes);
            }
        }

        private void AddRootNode()
        {
            string name = XtraInputBox.Show("루트 메뉴 이름을 입력하세요:", "루트 추가", "");
            if (!string.IsNullOrEmpty(name))
            {
                var data = new MenuNodeData { MenuId = Guid.NewGuid().ToString().Substring(0, 8), ProgId = null, AuthLevel = 1 };
                tvMenu.Nodes.Add(new TreeNode(name) { Tag = data });
            }
        }

        private void AddChildNode()
        {
            if (tvMenu.SelectedNode == null) return;
            string name = XtraInputBox.Show("자식 메뉴 이름을 입력하세요:", "자식 추가", "");
            if (!string.IsNullOrEmpty(name))
            {
                var data = new MenuNodeData { MenuId = Guid.NewGuid().ToString().Substring(0, 8), ProgId = null, AuthLevel = 1 };
                tvMenu.SelectedNode.Nodes.Add(new TreeNode(name) { Tag = data });
                tvMenu.SelectedNode.Expand();
            }
        }

        private void DeleteNode()
        {
            if (tvMenu.SelectedNode != null)
            {
                if (XtraMessageBox.Show("이 메뉴를 삭제하시겠습니까?", "확인", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    tvMenu.SelectedNode.Remove();
            }
        }

        private void LbPrograms_DoubleClick(object? sender, EventArgs e)
        {
            if (lbPrograms.SelectedItem is ProgramItem item)
            {
                if (tvMenu.SelectedNode != null)
                {
                    var selectedData = tvMenu.SelectedNode.Tag as MenuNodeData;

                    if (selectedData != null && !string.IsNullOrEmpty(selectedData.ProgId))
                    {
                        return;
                    }
                }

                var data = new MenuNodeData
                {
                    MenuId = Guid.NewGuid().ToString().Substring(0, 8),
                    ProgId = item.ProgId,
                    AuthLevel = 1
                };

                var newNode = new TreeNode(item.ProgName)
                {
                    Tag = data,
                    ForeColor = Color.Blue
                };

                if (tvMenu.SelectedNode != null)
                {
                    tvMenu.SelectedNode.Nodes.Add(newNode);
                    tvMenu.SelectedNode.Expand();
                }
                else
                {
                    tvMenu.Nodes.Add(newNode);
                }
            }
        }

        private void SaveMenu()
        {
            if (string.IsNullOrEmpty(_currentUserId) || string.IsNullOrEmpty(_currentDeptCode))
            {
                XtraMessageBox.Show("사용자와 부서를 먼저 선택해주세요.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                _menuRepo.DeleteMenusByUserAndDept(_currentUserId, _currentDeptCode);
                SaveNodesRecursive(tvMenu.Nodes, null, _currentUserId, _currentDeptCode);

                XtraMessageBox.Show($"메뉴가 저장되었습니다!\n사용자: {_currentUserId}, 부서: {_currentDeptCode}", "완료", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"메뉴 저장 실패: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SaveNodesRecursive(TreeNodeCollection nodes, string? parentId, string userId, string deptCode)
        {
            int sort = 10;
            foreach (TreeNode node in nodes)
            {
                var data = node.Tag as MenuNodeData;
                if (data == null) continue;

                var dto = new MenuDto
                {
                    MenuId = data.MenuId ?? string.Empty,
                    ParentId = string.IsNullOrEmpty(parentId) ? null : parentId,
                    MenuName = node.Text,
                    ProgId = string.IsNullOrEmpty(data.ProgId) ? null : data.ProgId,
                    SortOrd = sort,
                    AuthLevel = data.AuthLevel
                };

                _menuRepo.AddMenuForUser(userId, deptCode, dto);

                SaveNodesRecursive(node.Nodes, data.MenuId, userId, deptCode);
                sort += 10;
            }
        }

        class UserItem
        {
            public string UserId { get; set; } = string.Empty;
            public string UserName { get; set; } = string.Empty;
            public string DisplayText => $"[{UserId}] {UserName}";
            public override string ToString() => DisplayText;
        }

        class ProgramItem
        {
            public string? ProgId { get; set; }
            public string? ProgName { get; set; }
            public string? ModuleId { get; set; }
            public string? IsActive { get; set; }
            public int ProgType { get; set; }
            public string DisplayInfo => $"[{ModuleId}] {ProgName} ({ProgId})";

            public override string ToString() => DisplayInfo;
        }

        class MenuNodeData
        {
            public string? MenuId { get; set; }
            public string? ProgId { get; set; }
            public int AuthLevel { get; set; }
        }
    }
}
