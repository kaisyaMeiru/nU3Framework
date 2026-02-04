using nU3.Core.Security;
using nU3.Core.UI;
using nU3.Core.UI.Controls.Forms;
using System;
using System.Windows.Forms;

namespace nU3.Shell.Forms
{
    public partial class LoginForm : nU3Form
    {
        private Button btnLogin2;

        public LoginForm()
        {
            InitializeComponent();
            this.Text = "nU3 System Login";
            this.Size = new System.Drawing.Size(300, 200);
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            string id = txtId.Text;
            string pwd = txtPwd.Text;

            // TODO: 실제 API(IAuthService)로 교체 필요
            if (Authenticate(id, pwd))
            {
                UserSession.Current.SetSession(id, "Administrator", "IT", 9);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Invalid Credentials (Try admin/1234)");
                // 실패 시 다이얼로그 닫기 방지
                this.DialogResult = DialogResult.None;
            }
        }

        private bool Authenticate(string id, string pwd)
        {
            // 더미 로직
            return id == "admin" && pwd == "1234";
        }

      
    }
}
