namespace nU3.ModuleCreator.Vsix
{
    partial class WizardForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.txtSystem = new System.Windows.Forms.TextBox();
            this.txtSubSystem = new System.Windows.Forms.TextBox();
            this.txtModuleNamespace = new System.Windows.Forms.TextBox();
            this.txtProgramName = new System.Windows.Forms.TextBox();
            this.txtProgramId = new System.Windows.Forms.TextBox();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblSystem = new System.Windows.Forms.Label();
            this.lblSubSystem = new System.Windows.Forms.Label();
            this.lblModuleNamespace = new System.Windows.Forms.Label();
            this.lblProgramName = new System.Windows.Forms.Label();
            this.lblProgramId = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // txtSystem
            // 
            this.txtSystem.Location = new System.Drawing.Point(120, 15);
            this.txtSystem.Name = "txtSystem";
            this.txtSystem.Size = new System.Drawing.Size(252, 21);
            this.txtSystem.TabIndex = 1;
            // 
            // txtSubSystem
            // 
            this.txtSubSystem.Location = new System.Drawing.Point(120, 42);
            this.txtSubSystem.Name = "txtSubSystem";
            this.txtSubSystem.Size = new System.Drawing.Size(252, 21);
            this.txtSubSystem.TabIndex = 3;
            // 
            // txtModuleNamespace
            // 
            this.txtModuleNamespace.Location = new System.Drawing.Point(120, 69);
            this.txtModuleNamespace.Name = "txtModuleNamespace";
            this.txtModuleNamespace.Size = new System.Drawing.Size(252, 21);
            this.txtModuleNamespace.TabIndex = 5;
            // 
            // txtProgramName
            // 
            this.txtProgramName.Location = new System.Drawing.Point(120, 96);
            this.txtProgramName.Name = "txtProgramName";
            this.txtProgramName.Size = new System.Drawing.Size(252, 21);
            this.txtProgramName.TabIndex = 7;
            // 
            // txtProgramId
            // 
            this.txtProgramId.Location = new System.Drawing.Point(120, 123);
            this.txtProgramId.Name = "txtProgramId";
            this.txtProgramId.Size = new System.Drawing.Size(252, 21);
            this.txtProgramId.TabIndex = 9;
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(216, 160);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 10;
            this.btnOk.Text = "확인";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(297, 160);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 11;
            this.btnCancel.Text = "취소";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // lblSystem
            // 
            this.lblSystem.AutoSize = true;
            this.lblSystem.Location = new System.Drawing.Point(12, 18);
            this.lblSystem.Name = "lblSystem";
            this.lblSystem.Size = new System.Drawing.Size(46, 12);
            this.lblSystem.TabIndex = 0;
            this.lblSystem.Text = "시스템";
            // 
            // lblSubSystem
            // 
            this.lblSubSystem.AutoSize = true;
            this.lblSubSystem.Location = new System.Drawing.Point(12, 45);
            this.lblSubSystem.Name = "lblSubSystem";
            this.lblSubSystem.Size = new System.Drawing.Size(68, 12);
            this.lblSubSystem.TabIndex = 2;
            this.lblSubSystem.Text = "서브 시스템";
            // 
            // lblModuleNamespace
            // 
            this.lblModuleNamespace.AutoSize = true;
            this.lblModuleNamespace.Location = new System.Drawing.Point(12, 72);
            this.lblModuleNamespace.Name = "lblModuleNamespace";
            this.lblModuleNamespace.Size = new System.Drawing.Size(73, 12);
            this.lblModuleNamespace.TabIndex = 4;
            this.lblModuleNamespace.Text = "모듈 네임스페이스";
            // 
            // lblProgramName
            // 
            this.lblProgramName.AutoSize = true;
            this.lblProgramName.Location = new System.Drawing.Point(12, 99);
            this.lblProgramName.Name = "lblProgramName";
            this.lblProgramName.Size = new System.Drawing.Size(89, 12);
            this.lblProgramName.TabIndex = 6;
            this.lblProgramName.Text = "프로그램 명";
            // 
            // lblProgramId
            // 
            this.lblProgramId.AutoSize = true;
            this.lblProgramId.Location = new System.Drawing.Point(12, 126);
            this.lblProgramId.Name = "lblProgramId";
            this.lblProgramId.Size = new System.Drawing.Size(66, 12);
            this.lblProgramId.TabIndex = 8;
            this.lblProgramId.Text = "프로그램 ID";
            // 
            // WizardForm
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(384, 201);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.txtProgramId);
            this.Controls.Add(this.lblProgramId);
            this.Controls.Add(this.txtProgramName);
            this.Controls.Add(this.lblProgramName);
            this.Controls.Add(this.txtModuleNamespace);
            this.Controls.Add(this.lblModuleNamespace);
            this.Controls.Add(this.txtSubSystem);
            this.Controls.Add(this.lblSubSystem);
            this.Controls.Add(this.txtSystem);
            this.Controls.Add(this.lblSystem);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "WizardForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "nU3 모듈 생성 마법사";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.TextBox txtSystem;
        private System.Windows.Forms.TextBox txtSubSystem;
        private System.Windows.Forms.TextBox txtModuleNamespace;
        private System.Windows.Forms.TextBox txtProgramName;
        private System.Windows.Forms.TextBox txtProgramId;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lblSystem;
        private System.Windows.Forms.Label lblSubSystem;
        private System.Windows.Forms.Label lblModuleNamespace;
        private System.Windows.Forms.Label lblProgramName;
        private System.Windows.Forms.Label lblProgramId;
        
        #endregion
    }
}
