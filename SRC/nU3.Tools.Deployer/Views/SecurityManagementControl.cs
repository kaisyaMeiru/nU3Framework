using System.Windows.Forms;

namespace nU3.Tools.Deployer.Views
{
    public partial class SecurityManagementControl : UserControl
    {
        public SecurityManagementControl()
        {
            Dock = DockStyle.Fill;

            var top = new Panel { Dock = DockStyle.Top, Height = 40 };
            var btnAdd = new Button { Text = "추가", Left = 10, Top = 8 };
            var btnDelete = new Button { Text = "삭제", Left = 90, Top = 8 };
            var btnRefresh = new Button { Text = "새로고침", Left = 170, Top = 8 };

            btnAdd.Click += (s, e) => MessageBox.Show("TODO: 사용자 추가");
            btnDelete.Click += (s, e) => MessageBox.Show("TODO: 사용자 삭제");
            btnRefresh.Click += (s, e) => MessageBox.Show("TODO: 사용자 새로고침");

            top.Controls.Add(btnAdd);
            top.Controls.Add(btnDelete);
            top.Controls.Add(btnRefresh);

            var grid = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true };

            Controls.Add(grid);
            Controls.Add(top);
        }
    }
}
