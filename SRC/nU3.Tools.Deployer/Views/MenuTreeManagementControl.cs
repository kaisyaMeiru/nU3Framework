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
    /// - 전체 프로그램 목록의 로드
    /// - SYS_MENU 테이블의 메뉴 트리 로드/저장/삭제
    /// - 새 항목 추가(루트/자식), 편집, 삭제 기능
    /// </summary>
    public partial class MenuTreeManagementControl : BaseWorkControl
    {
        private readonly IDBAccessService _db;
        private readonly IMenuRepository _menuRepo;
        private readonly IProgramRepository _progRepo;

        /// <summary>
        /// Designer 전용 생성자
        /// </summary>
        public MenuTreeManagementControl()
        {
            InitializeComponent();
        }

        public MenuTreeManagementControl(IDBAccessService db, IMenuRepository menuRepo, IProgramRepository progRepo) : this()
        {
            _db = db;
            _menuRepo = menuRepo;
            _progRepo = progRepo;

            if (!IsDesignMode())
            {
                LoadPrograms();
                LoadMenuTree();
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

        private void LoadMenuTree()
        {
            tvMenu.Nodes.Clear();
            var allMenus = _menuRepo.GetAllMenus();
            AddTreeNodes(allMenus, null, tvMenu.Nodes);
            tvMenu.ExpandAll();
        }

        private void BtnRefreshPrograms_Click(object sender, EventArgs e)
        {
            LoadPrograms();
            LoadMenuTree();

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
                // 선택된 노드가 있는지 확인
                if (tvMenu.SelectedNode != null)
                {
                    var selectedData = tvMenu.SelectedNode.Tag as MenuNodeData;
                    
                    // ★ 중요: 선택된 항목이 이미 '프로그램'일 경우 하위에 추가하는 것을 차단
                    if (selectedData != null && !string.IsNullOrEmpty(selectedData.ProgId))
                    {
                        return; // 아무 동작도 하지 않음
                    }
                }

                // 프로그램 정보를 담은 새 메뉴 노드 데이터 생성
                var data = new MenuNodeData 
                { 
                    MenuId = Guid.NewGuid().ToString().Substring(0, 8), 
                    ProgId = item.ProgId, 
                    AuthLevel = 1 
                };
                
                // 트리 노드 생성 (프로그램 연결 노드는 파란색으로 표시)
                var newNode = new TreeNode(item.ProgName) 
                { 
                    Tag = data, 
                    ForeColor = Color.Blue 
                };

                if (tvMenu.SelectedNode != null)
                {
                    // 폴더 노드일 경우 자식으로 추가
                    tvMenu.SelectedNode.Nodes.Add(newNode);
                    tvMenu.SelectedNode.Expand();
                }
                else
                {
                    // 선택된 노드가 없으면 최상위 루트로 추가
                    tvMenu.Nodes.Add(newNode);
                }
            }
        }

        private void SaveMenu()
        {
            try
            {
                //_db.BeginTransaction();
                
                _menuRepo.DeleteAllMenus();
                SaveNodesRecursive(tvMenu.Nodes, null);
                
                //_db.CommitTransaction();
                XtraMessageBox.Show("메뉴가 저장되었습니다!", "완료", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                //_db.RollbackTransaction();
                XtraMessageBox.Show($"메뉴 저장 실패: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SaveNodesRecursive(TreeNodeCollection nodes, string? parentId)
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

                _menuRepo.AddMenu(dto);

                SaveNodesRecursive(node.Nodes, data.MenuId);
                sort += 10;
            }
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
