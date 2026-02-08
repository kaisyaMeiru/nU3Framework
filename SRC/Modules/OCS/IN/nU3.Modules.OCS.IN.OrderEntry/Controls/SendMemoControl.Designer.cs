namespace nU3.Modules.OCS.IN.OrderEntry.Controls
{
    partial class SendMemoControl
    {
        /// <summary> 
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 구성 요소 디자이너에서 생성한 코드

        /// <summary> 
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
        /// </summary>
        private void InitializeComponent()
        {
            this.grpSendMemo = new DevExpress.XtraEditors.GroupControl();
            this.pnlButtons = new DevExpress.XtraEditors.PanelControl();
            this.btnMemoResv = new DevExpress.XtraEditors.SimpleButton();
            this.btnSave = new DevExpress.XtraEditors.SimpleButton();
            this.btnClear = new DevExpress.XtraEditors.SimpleButton();
            this.memoEdit = new DevExpress.XtraEditors.MemoEdit();
            ((System.ComponentModel.ISupportInitialize)(this.grpSendMemo)).BeginInit();
            this.grpSendMemo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pnlButtons)).BeginInit();
            this.pnlButtons.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.memoEdit.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // grpSendMemo
            // 
            this.grpSendMemo.Controls.Add(this.pnlButtons);
            this.grpSendMemo.Controls.Add(this.memoEdit);
            this.grpSendMemo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpSendMemo.Location = new System.Drawing.Point(0, 0);
            this.grpSendMemo.Name = "grpSendMemo";
            this.grpSendMemo.Size = new System.Drawing.Size(536, 108);
            this.grpSendMemo.TabIndex = 0;
            this.grpSendMemo.Text = "전달메모";
            // 
            // pnlButtons
            // 
            this.pnlButtons.Controls.Add(this.btnMemoResv);
            this.pnlButtons.Controls.Add(this.btnSave);
            this.pnlButtons.Controls.Add(this.btnClear);
            this.pnlButtons.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlButtons.Location = new System.Drawing.Point(2, 22);
            this.pnlButtons.Name = "pnlButtons";
            this.pnlButtons.Size = new System.Drawing.Size(532, 30);
            this.pnlButtons.TabIndex = 1;
            // 
            // btnMemoResv
            // 
            this.btnMemoResv.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            // this.btnMemoResv.Image = global::nU3.Modules.OCS.IN.OrderEntry.Properties.Resources.reservation;
            this.btnMemoResv.Location = new System.Drawing.Point(450, 3);
            this.btnMemoResv.Name = "btnMemoResv";
            this.btnMemoResv.Size = new System.Drawing.Size(75, 23);
            this.btnMemoResv.TabIndex = 2;
            this.btnMemoResv.Text = "예약문구";
            this.btnMemoResv.Click += new System.EventHandler(this.btnMemoResv_Click);
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            // this.btnSave.Image = global::nU3.Modules.OCS.IN.OrderEntry.Properties.Resources.save;
            this.btnSave.Location = new System.Drawing.Point(369, 3);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 1;
            this.btnSave.Text = "저장";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnClear
            // 
            this.btnClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            // this.btnClear.Image = global::nU3.Modules.OCS.IN.OrderEntry.Properties.Resources.clear;
            this.btnClear.Location = new System.Drawing.Point(288, 3);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(75, 23);
            this.btnClear.TabIndex = 0;
            this.btnClear.Text = "지우기";
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // memoEdit
            // 
            this.memoEdit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.memoEdit.Location = new System.Drawing.Point(2, 52);
            this.memoEdit.Name = "memoEdit";
            this.memoEdit.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 9F);
            this.memoEdit.Properties.Appearance.Options.UseFont = true;
            this.memoEdit.Properties.MaxLength = 2000;
            this.memoEdit.Size = new System.Drawing.Size(532, 54);
            this.memoEdit.TabIndex = 0;
            // 
            // SendMemoControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.grpSendMemo);
            this.Name = "SendMemoControl";
            this.Size = new System.Drawing.Size(536, 108);
            ((System.ComponentModel.ISupportInitialize)(this.grpSendMemo)).EndInit();
            this.grpSendMemo.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pnlButtons)).EndInit();
            this.pnlButtons.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.memoEdit.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.GroupControl grpSendMemo;
        private DevExpress.XtraEditors.PanelControl pnlButtons;
        private DevExpress.XtraEditors.SimpleButton btnMemoResv;
        private DevExpress.XtraEditors.SimpleButton btnSave;
        private DevExpress.XtraEditors.SimpleButton btnClear;
        private DevExpress.XtraEditors.MemoEdit memoEdit;
    }
}