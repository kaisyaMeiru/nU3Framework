using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Windows.Forms;
using nU3.Data;
using nU3.Core.UI;

namespace nU3.Tools.Deployer.Views
{
    /// <summary>
    /// 메뉴 트리(네비게이션 메뉴)를 편집하고 로컬 DB에 저장할 수 있는 에디터 컨트롤입니다.
    /// 주요 기능:
    /// - 등록된 프로그램 목록 로드
    /// - SYS_MENU 테이블 기반의 메뉴 트리 로드/편집/저장
    /// - 노드 추가(루트/자식), 삭제, 인증 레벨 편집
    /// </summary>
    public partial class MenuTreeManagementControl : UserControl
    {
        private readonly LocalDatabaseManager _dbManager;

        public MenuTreeManagementControl()
        {
            _dbManager = new LocalDatabaseManager();
            this.Text = "메뉴 구성 편집기";
            this.Size = new System.Drawing.Size(1000, 700);

            InitializeComponent();

            if (!IsDesignMode())
            {
                LoadPrograms();
                LoadMenuTree();
            }
        }

        /// <summary>
        /// 디자이너 모드 여부 확인 (디자인타임에는 DB 접근 등을 건너뜁니다)
        /// </summary>
        private static bool IsDesignMode()
        {
            return LicenseManager.UsageMode == LicenseUsageMode.Designtime;
        }

        /// <summary>
        /// 트리에서 노드 선택이 변경되었을 때 버튼 상태와 인증 레벨 컨트롤을 갱신합니다.
        /// </summary>
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

        /// <summary>
        /// 인증 레벨 값 변경 시 선택된 노드의 데이터에 반영합니다.
        /// </summary>
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

        private void CmsAddRoot_Click(object sender, EventArgs e)
        {
            AddRootNode();
        }

        private void CmsAddChild_Click(object sender, EventArgs e)
        {
            AddChildNode();
        }

        private void CmsDelete_Click(object sender, EventArgs e)
        {
            DeleteNode();
        }

        private void BtnAddRoot_Click(object sender, EventArgs e)
        {
            AddRootNode();
        }

        private void BtnAddChild_Click(object sender, EventArgs e)
        {
            AddChildNode();
        }

        private void BtnDeleteNode_Click(object sender, EventArgs e)
        {
            DeleteNode();
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            SaveMenu();
        }

        /// <summary>
        /// 트리 선택 상태에 따라 버튼/컨트롤의 활성화 상태를 갱신합니다.
        /// </summary>
        private void UpdateButtons()
        {
            bool hasSelection = tvMenu.SelectedNode != null;
            btnAddChild.Enabled = hasSelection;
            btnDeleteNode.Enabled = hasSelection;
            numAuthLevel.Enabled = hasSelection;
        }

        /// <summary>
        /// DB에서 활성화된 프로그램 목록을 로드하여 리스트박스에 표시합니다.
        /// 표시 형식: [ModuleId] ProgName (ProgId)
        /// </summary>
        private void LoadPrograms()
        {
            lbPrograms.Items.Clear();
            lbPrograms.DisplayMember = "DisplayInfo";

            using (var conn = new SQLiteConnection(_dbManager.GetConnectionString()))
            {
                conn.Open();
                string sql = "SELECT PROG_ID, PROG_NAME, MODULE_ID, IS_ACTIVE, PROG_TYPE FROM SYS_PROG_MST WHERE IFNULL(IS_ACTIVE,'Y') = 'Y' ORDER BY PROG_NAME";
                using (var cmd = new SQLiteCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
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
        }

        /// <summary>
        /// 로컬 DB의 SYS_MENU 테이블을 읽어 TreeView에 노드들을 구성합니다.
        /// </summary>
        private void LoadMenuTree()
        {
            tvMenu.Nodes.Clear();
            using (var conn = new SQLiteConnection(_dbManager.GetConnectionString()))
            {
                conn.Open();
                string sql = "SELECT * FROM SYS_MENU ORDER BY PARENT_ID, SORT_ORD";
                using (var da = new SQLiteDataAdapter(sql, conn))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    AddTreeNodes(dt, null, tvMenu.Nodes);
                }
            }
            tvMenu.ExpandAll();
        }

        /// <summary>
        /// DataTable의 메뉴 레코드를 재귀적으로 순회하여 TreeNode를 구성하고 nodes 컬렉션에 추가합니다.
        /// 프로그램과 연결된 노드는 파란색으로 표시합니다.
        /// </summary>
        private void AddTreeNodes(DataTable dt, string parentId, TreeNodeCollection nodes)
        {
            string filter = parentId == null ? "PARENT_ID IS NULL" : $"PARENT_ID = '{parentId}'";
            foreach (DataRow row in dt.Select(filter, "SORT_ORD"))
            {
                string id = row["MENU_ID"].ToString();
                string name = row["MENU_NAME"].ToString();
                string progId = row["PROG_ID"].ToString();
                int auth = row["AUTH_LEVEL"] == DBNull.Value ? 1 : Convert.ToInt32(row["AUTH_LEVEL"]);

                var data = new MenuNodeData();
                data.MenuId = id;
                data.ProgId = progId;
                data.AuthLevel = auth;

                var node = new TreeNode(name);
                node.Tag = data;

                // 프로그램이 연결된 노드는 파란색으로 표시
                if (!string.IsNullOrEmpty(progId))
                    node.ForeColor = Color.Blue;

                nodes.Add(node);
                AddTreeNodes(dt, id, node.Nodes);
            }
        }

        /// <summary>
        /// 루트 노드를 추가합니다. 사용자에게 이름을 입력받아 트리에 추가합니다.
        /// </summary>
        private void AddRootNode()
        {
            string name = Prompt.ShowDialog("루트 메뉴 이름을 입력하세요:", "루트 추가");
            if (!string.IsNullOrEmpty(name))
            {
                var data = new MenuNodeData();
                data.MenuId = Guid.NewGuid().ToString().Substring(0, 8);
                data.ProgId = null;
                data.AuthLevel = 1;

                var node = new TreeNode(name);
                node.Tag = data;

                tvMenu.Nodes.Add(node);
            }
        }

        /// <summary>
        /// 선택된 노드의 자식 노드를 추가합니다. 이름은 사용자 입력으로 받습니다.
        /// </summary>
        private void AddChildNode()
        {
            if (tvMenu.SelectedNode == null) return;
            string name = Prompt.ShowDialog("자식 메뉴 이름을 입력하세요:", "자식 추가");
            if (!string.IsNullOrEmpty(name))
            {
                var data = new MenuNodeData();
                data.MenuId = Guid.NewGuid().ToString().Substring(0, 8);
                data.ProgId = null;
                data.AuthLevel = 1;

                var node = new TreeNode(name);
                node.Tag = data;

                tvMenu.SelectedNode.Nodes.Add(node);
                tvMenu.SelectedNode.Expand();
            }
        }

        /// <summary>
        /// 프로그램 리스트에서 항목을 더블클릭하면 선택된 트리 노드의 자식으로 추가됩니다. 프로그램 노드는 파란색으로 표시됩니다.
        /// </summary>
        private void LbPrograms_DoubleClick(object sender, EventArgs e)
        {
            if (lbPrograms.SelectedItem is ProgramItem prog && tvMenu.SelectedNode != null)
            {
                var data = new MenuNodeData();
                data.MenuId = Guid.NewGuid().ToString().Substring(0, 8);
                data.ProgId = prog.ProgId;
                data.AuthLevel = 1;

                var node = new TreeNode(prog.ProgName);
                node.Tag = data;
                node.ForeColor = Color.Blue;

                tvMenu.SelectedNode.Nodes.Add(node);
                tvMenu.SelectedNode.Expand();
            }
        }

        /// <summary>
        /// 현재 선택된 노드를 삭제합니다. 사용자에게 확인을 요청합니다.
        /// </summary>
        private void DeleteNode()
        {
            if (tvMenu.SelectedNode != null)
            {
                if (MessageBox.Show("이 메뉴를 정말로 삭제하시겠습니까?", "확인", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    tvMenu.SelectedNode.Remove();
            }
        }

        /// <summary>
        /// TreeView의 현재 구성을 SYS_MENU 테이블에 저장합니다.
        /// 기존 레코드는 모두 삭제한 후 재삽입합니다. 트랜잭션으로 안전하게 처리합니다.
        /// </summary>
        private void SaveMenu()
        {
            using (var conn = new SQLiteConnection(_dbManager.GetConnectionString()))
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        new SQLiteCommand("DELETE FROM SYS_MENU", conn).ExecuteNonQuery();

                        SaveNodesRecursive(tvMenu.Nodes, null, conn);

                        trans.Commit();
                        MessageBox.Show("메뉴 구성이 저장되었습니다!", "완료", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        MessageBox.Show($"메뉴 저장 실패: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        /// <summary>
        /// 트리 노드 컬렉션을 재귀적으로 순회하며 DB에 INSERT 쿼리를 실행합니다.
        /// 각 노드의 정렬 순서는 sort 변수로 제어되며, 자식 그룹 내에서 10 단위로 증가합니다.
        /// </summary>
        private void SaveNodesRecursive(TreeNodeCollection nodes, string parentId, SQLiteConnection conn)
        {
            int sort = 10;
            foreach (TreeNode node in nodes)
            {
                var data = node.Tag as MenuNodeData;
                string sql = @"INSERT INTO SYS_MENU (MENU_ID, PARENT_ID, MENU_NAME, PROG_ID, SORT_ORD, AUTH_LEVEL) 
                               VALUES (@id, @pid, @name, @prog, @sort, @auth)";

                using (var cmd = new SQLiteCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", data.MenuId);
                    cmd.Parameters.AddWithValue("@pid", (object)parentId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@name", node.Text);
                    cmd.Parameters.AddWithValue("@prog", (object)data.ProgId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@sort", sort);
                    cmd.Parameters.AddWithValue("@auth", data.AuthLevel);
                    cmd.ExecuteNonQuery();
                }

                SaveNodesRecursive(node.Nodes, data.MenuId, conn);
                sort += 10;
            }
        }
    }

    // 도우미(Helper) 클래스들
    class ProgramItem
    {
        public string ProgId { get; set; }
        public string ProgName { get; set; }
        public string ModuleId { get; set; }
        public string IsActive { get; set; }
        public int ProgType { get; set; }
        public string DisplayInfo => $"[{ModuleId}] {ProgName} ({ProgId})";
    }

    class MenuNodeData
    {
        public string MenuId { get; set; }
        public string ProgId { get; set; }
        public int AuthLevel { get; set; }
    }

    public static class Prompt
    {
        /// <summary>
        /// 간단한 입력 대화상자를 표시하고 사용자가 입력한 문자열을 반환합니다.
        /// 취소하거나 빈 값이면 빈 문자열을 반환합니다.
        /// </summary>
        public static string ShowDialog(string text, string caption)
        {
            Form prompt = new Form()
            {
                Width = 500,
                Height = 150,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = caption,
                StartPosition = FormStartPosition.CenterScreen
            };
            Label textLabel = new Label() { Left = 50, Top = 20, Text = text };
            TextBox textBox = new TextBox() { Left = 50, Top = 50, Width = 400 };
            Button confirmation = new Button() { Text = "확인", Left = 350, Width = 100, Top = 70, DialogResult = DialogResult.OK };
            confirmation.Click += (sender, e) => { prompt.Close(); };
            prompt.Controls.Add(textBox);
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(textLabel);
            prompt.AcceptButton = confirmation;

            return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : "";
        }
    }
}
