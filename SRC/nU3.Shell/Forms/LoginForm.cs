using nU3.Core.UI;
using nU3.Core.UI.Controls.Forms;
using System;
using System.Windows.Forms;
using nU3.Core.Enums;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using nU3.Core.Security;
using nU3.Connectivity;
using nU3.Core.Interfaces;
using System.Threading.Tasks;

namespace nU3.Shell.Forms
{
    public partial class LoginForm : nU3Form
    {
        private readonly IAuthenticationService _authService;

        public LoginForm(IAuthenticationService authService)
        {
            _authService = authService;
            InitializeComponent();
            this.Text = "nU3 System Login";
            this.Size = new System.Drawing.Size(429, 294);

            // 초기 로그인 화면에는 부서 목록을 표시하지 않습니다.
            try { if (this.Controls.Contains(cboDept)) cboDept.Visible = false; if (this.Controls.Contains(lblDept)) lblDept.Visible = false; }
            catch { }
        }

        private async void BtnLogin_Click(object sender, EventArgs e)
        {
            string id = txtId.Text?.ToString() ?? string.Empty;
            string pwd = txtPwd.Text?.ToString() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(id)) { MessageBox.Show("아이디를 입력하세요.", "로그인", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }

            // 서버 IDP를 통한 인증
            var authResult = await _authService.AuthenticateAsync(id, pwd);

            if (!authResult.Success) { MessageBox.Show(authResult.ErrorMessage ?? "인증 실패", "로그인 실패", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            try {
                string tokenString = authResult.Token!;
                nU3.Core.Security.UserSession.Current.SetJwt(tokenString);

                string? selectedDept = nU3.Core.Security.UserSession.Current.SetJwtAndEnsureDepartment(tokenString, available => {
                    var map = new Dictionary<string, string>();
                    foreach (Department d in Enum.GetValues(typeof(Department))) {
                        var display = typeof(Department).GetMember(d.ToString()).FirstOrDefault()?.GetCustomAttributes(typeof(DisplayAttribute), false).OfType<DisplayAttribute>().FirstOrDefault()?.Name ?? d.ToString();
                        map[((int)d).ToString()] = display;
                    }
                    using var dlg = new DeptSelectionDialog(available.Select(code => (code, map.ContainsKey(code) ? map[code] : code)).ToList()) { Owner = this, Height = this.Height };
                    return dlg.ShowDialog(this) == DialogResult.OK ? dlg.SelectedDeptCode : null;
                });

                if (string.IsNullOrWhiteSpace(selectedDept)) { MessageBox.Show("부서를 선택하지 않았습니다.", "로그인 취소", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

                nU3.Core.Security.UserSession.Current.SetAvailableDepartments(authResult.DeptCodes!);
                nU3.Core.Security.UserSession.Current.SelectDepartment(selectedDept);
                var authLevel = (authResult.Roles ?? Array.Empty<string>()).Any(r => r == "0") ? 9 : 1;
                nU3.Core.Security.UserSession.Current.SetSession(id, id, selectedDept, authLevel);

                this.DialogResult = DialogResult.OK;
                this.Close();
            } catch (Exception ex) { MessageBox.Show($"로그인 오류: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private class DeptItem
        {
            public Department Dept { get; set; }
            public string Text { get; set; } = string.Empty;
            public override string ToString() => Text;
        }

        // nU3 컨트롤 기반 부서 선택 대화상자
        private class DeptSelectionDialog : nU3Form
        {
            private ComboBox cbo;
            private Button btnOk;
            private Button btnCancel;
            private readonly List<(string code, string display)> _items;

            public string SelectedDeptCode { get; private set; }

            public DeptSelectionDialog(List<(string code, string display)> items)
            {
                _items = items ?? new List<(string, string)>();

                Text = "부서 선택";
                StartPosition = FormStartPosition.CenterParent;
                FormBorderStyle = FormBorderStyle.FixedDialog;
                MaximizeBox = false;
                MinimizeBox = false;
                ClientSize = new System.Drawing.Size(420, 140);

                // nU3 스타일의 라벨, 콤보, 버튼을 사용해 일관된 UI 제공
                cbo = new ComboBox { Left = 12, Top = 16, Width = 396, DropDownStyle = ComboBoxStyle.DropDownList };
                foreach (var it in _items)
                {
                    cbo.Items.Add(new ComboItem { Code = it.code, Display = it.display });
                }
                if (cbo.Items.Count > 0) cbo.SelectedIndex = 0;

                btnOk = new Button { Text = "확인", Left = 228, Top = 72, Width = 88 };
                btnCancel = new Button { Text = "취소", Left = 324, Top = 72, Width = 88 };

                btnOk.Click += (s, e) =>
                {
                    if (cbo.SelectedItem is ComboItem ci)
                    {
                        SelectedDeptCode = ci.Code;
                        DialogResult = DialogResult.OK;
                        Close();
                    }
                    else
                    {
                        MessageBox.Show("부서를 선택하세요.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                };

                btnCancel.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };

                Controls.Add(cbo);
                Controls.Add(btnOk);
                Controls.Add(btnCancel);
            }

            private class ComboItem
            {
                public string Code { get; set; }
                public string Display { get; set; }
                public override string ToString() => Display;
            }
        }
    }
}