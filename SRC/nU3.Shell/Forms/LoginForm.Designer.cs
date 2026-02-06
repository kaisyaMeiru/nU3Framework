using nU3.Core.UI.Controls.Forms;
using System.Windows.Forms;

namespace nU3.Shell.Forms
{
    partial class LoginForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            lblId = new System.Windows.Forms.Label();
            lblPwd = new System.Windows.Forms.Label();
            lblDept = new System.Windows.Forms.Label();
            btnLogin = new nU3.Core.UI.Controls.nU3SimpleButton();
            btnCancel = new nU3.Core.UI.Controls.nU3SimpleButton();
            txtId = new nU3.Core.UI.Controls.nU3TextEdit();
            txtPwd = new nU3.Core.UI.Controls.nU3TextEdit();
            cboDept = new nU3.Core.UI.Controls.nU3ComboBoxEdit();
            ((System.ComponentModel.ISupportInitialize)txtId.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)txtPwd.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)cboDept.Properties).BeginInit();
            SuspendLayout();
            // 
            // lblId
            // 
            lblId.Location = new System.Drawing.Point(29, 44);
            lblId.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblId.Name = "lblId";
            lblId.Size = new System.Drawing.Size(71, 33);
            lblId.TabIndex = 0;
            lblId.Text = "ID:";
            // 
            // lblPwd
            // 
            lblPwd.Location = new System.Drawing.Point(29, 102);
            lblPwd.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblPwd.Name = "lblPwd";
            lblPwd.Size = new System.Drawing.Size(71, 33);
            lblPwd.TabIndex = 2;
            lblPwd.Text = "PWD:";
            // 
            // lblDept
            // 
            lblDept.Location = new System.Drawing.Point(29, 160);
            lblDept.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblDept.Name = "lblDept";
            lblDept.Size = new System.Drawing.Size(71, 33);
            lblDept.TabIndex = 10;
            lblDept.Text = "ºÎ¼­:";
            // 
            // btnLogin
            // 
            btnLogin.Location = new System.Drawing.Point(114, 217);
            btnLogin.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            btnLogin.Name = "btnLogin";
            btnLogin.Size = new System.Drawing.Size(107, 36);
            btnLogin.TabIndex = 6;
            btnLogin.Text = "Login";
            btnLogin.Click += BtnLogin_Click;
            // 
            // btnCancel
            // 
            btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            btnCancel.Location = new System.Drawing.Point(230, 217);
            btnCancel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new System.Drawing.Size(107, 36);
            btnCancel.TabIndex = 7;
            btnCancel.Text = "Exit";
            // 
            // txtId
            // 
            txtId.EditValue = "admin";
            txtId.Location = new System.Drawing.Point(114, 46);
            txtId.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            txtId.Name = "txtId";
            txtId.Properties.AutoHeight = false;
            txtId.Size = new System.Drawing.Size(214, 31);
            txtId.TabIndex = 8;
            // 
            // txtPwd
            // 
            txtPwd.EditValue = "1234";
            txtPwd.Location = new System.Drawing.Point(114, 104);
            txtPwd.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            txtPwd.Name = "txtPwd";
            txtPwd.Properties.AutoHeight = false;
            txtPwd.Properties.PasswordChar = '*';
            txtPwd.Size = new System.Drawing.Size(214, 31);
            txtPwd.TabIndex = 9;
            // 
            // cboDept (nU3ComboBoxEdit)
            // 
            cboDept.Location = new System.Drawing.Point(114, 162);
            cboDept.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            cboDept.Name = "cboDept";
            cboDept.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            cboDept.Size = new System.Drawing.Size(214, 24);
            cboDept.TabIndex = 11;
            // 
            // LoginForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(144F, 144F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            AutoSize = true;
            ClientSize = new System.Drawing.Size(429, 280);
            Controls.Add(cboDept);
            Controls.Add(txtPwd);
            Controls.Add(txtId);
            Controls.Add(btnCancel);
            Controls.Add(btnLogin);
            Controls.Add(lblId);
            Controls.Add(lblPwd);
            Controls.Add(lblDept);
            Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            MaximumSize = new System.Drawing.Size(431, 341);
            MinimumSize = new System.Drawing.Size(431, 341);
            Name = "LoginForm";
            Text = "nU3 Framework";
            ((System.ComponentModel.ISupportInitialize)txtId.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)txtPwd.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)cboDept.Properties).EndInit();
            ResumeLayout(false);
        }
        private System.Windows.Forms.Label lblId;
        private System.Windows.Forms.Label lblPwd;
        private System.Windows.Forms.Label lblDept;
        private DevExpress.XtraBars.TabFormControl tabFormControl1;
        private DevExpress.XtraBars.TabFormPage tabFormPage1;
        private DevExpress.XtraBars.TabFormContentContainer tabFormContentContainer1;
        private Core.UI.Controls.nU3SimpleButton btnLogin;
        private Core.UI.Controls.nU3SimpleButton btnCancel;
        private Core.UI.Controls.nU3TextEdit txtId;
        private Core.UI.Controls.nU3TextEdit txtPwd;
        private Core.UI.Controls.nU3ComboBoxEdit cboDept;
    }
}
