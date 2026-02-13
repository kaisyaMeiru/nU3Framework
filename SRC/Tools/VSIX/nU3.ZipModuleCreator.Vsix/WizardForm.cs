using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace nU3.ModuleCreator.Vsix
{
    public partial class WizardForm : Form
    {
        public string SystemName { get { return txtSystem.Text.Trim(); } }
        public string SubSystem { get { return txtSubSystem.Text.Trim(); } }
        public string ModuleNamespace { get { return txtModuleNamespace.Text.Trim(); } }
        public string ProgramName { get { return txtProgramName.Text.Trim(); } }
        public string ProgramId { get { return txtProgramId.Text.Trim(); } }

        public WizardForm()
        {
            InitializeComponent();
            txtProgramId.Text = "USER_PROGRAM";
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(ProgramId))
            {
                MessageBox.Show("ProgramId는 필수 입력값입니다.");
                return;
            }

            // ProgramId는 클래스/파일명으로 사용됩니다. C# 식별자 최소 검증.
            // 영문자, 숫자, 밑줄 허용; 숫자로 시작 불가능.
            if (!Regex.IsMatch(ProgramId, @"^[A-Za-z_][A-Za-z0-9_]*$"))
            {
                MessageBox.Show("ProgramId는 C# 클래스/파일명으로 사용됩니다. 영문자/숫자/_ 만 사용 가능하며 숫자로 시작할 수 없습니다.");
                return;
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
