using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using nU3.Data;
using nU3.Core.UI;
using nU3.Core.UI.Controls;
using DevExpress.XtraEditors;

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
        private readonly LocalDbService _dbManager;

        /// <summary>
        /// Designer 전용 생성자
        /// </summary>
        public MenuTreeManagementControl()
        {
            _dbManager = new LocalDbService();
            InitializeComponent();

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

            string sql = "SELECT PROG_ID, PROG_NAME, MODULE_ID, IS_ACTIVE, PROG_TYPE FROM SYS_PROG_MST WHERE IFNULL(IS_ACTIVE,'Y') = 'Y' ORDER BY PROG_NAME";
            using (var dt = _dbManager.ExecuteDataTable(sql))
            {
                foreach (DataRow reader in dt.Rows)
                {
                    var item = new ProgramItem();
                    item.ProgId = reader["PROG_ID"].ToString();
                    item.ProgName = reader["PROG_NAME"]?.ToString();
                    item.ModuleId = reader["MODULE_ID"].ToString();
                    item.IsActive = reader["IS_ACTIVE"] == DBNull.Value ? "Y" : reader["IS_ACTIVE"].ToString();
                    item.ProgType = reader["PROG_TYPE"] == DBNull.Value ? 1 : Convert.ToInt32(reader["PROG_TYPE"]);
                    lbPrograms.Items.Add(item);
                }
            }
        }

        private void LoadMenuTree()
        {
            tvMenu.Nodes.Clear();
            string sql = "SELECT * FROM SYS_MENU ORDER BY PARENT_ID, SORT_ORD";
            using (var dt = _dbManager.ExecuteDataTable(sql))
            {
                AddTreeNodes(dt, null, tvMenu.Nodes);
            }
            tvMenu.ExpandAll();
        }

        private void AddTreeNodes(DataTable dt, string? parentId, TreeNodeCollection nodes)
        {
            string filter = parentId == null ? "PARENT_ID IS NULL" : $"PARENT_ID = '{parentId}'";
            foreach (DataRow row in dt.Select(filter, "SORT_ORD"))
            {
                string id = row["MENU_ID"].ToString() ?? string.Empty;
                string name = row["MENU_NAME"].ToString() ?? string.Empty;
                string progId = row["PROG_ID"].ToString() ?? string.Empty;
                int auth = row["AUTH_LEVEL"] == DBNull.Value ? 1 : Convert.ToInt32(row["AUTH_LEVEL"]);

                var data = new MenuNodeData { MenuId = id, ProgId = progId, AuthLevel = auth };
                var node = new TreeNode(name) { Tag = data };

                if (!string.IsNullOrEmpty(progId)) node.ForeColor = Color.Blue;

                nodes.Add(node);
                AddTreeNodes(dt, id, node.Nodes);
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
                _dbManager.BeginTransaction();
                _dbManager.ExecuteNonQuery("DELETE FROM SYS_MENU");
                SaveNodesRecursive(tvMenu.Nodes, null);
                _dbManager.CommitTransaction();
                XtraMessageBox.Show("메뉴가 저장되었습니다!", "완료", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                _dbManager.RollbackTransaction();
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

                string sql = @"INSERT INTO SYS_MENU (MENU_ID, PARENT_ID, MENU_NAME, PROG_ID, SORT_ORD, AUTH_LEVEL)
                               VALUES (@id, @pid, @name, @prog, @sort, @auth)";

                var parameters = new Dictionary<string, object>
                {
                    { "@id", data.MenuId },
                    { "@pid", (object?)parentId ?? DBNull.Value },
                    { "@name", node.Text },
                    { "@prog", (object?)data.ProgId ?? DBNull.Value },
                    { "@sort", sort },
                    { "@auth", data.AuthLevel }
                };

                _dbManager.ExecuteNonQuery(sql, parameters);

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
