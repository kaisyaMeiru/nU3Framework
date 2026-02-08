namespace nU3.Modules.OCS.IN.OrderEntry.Controls
{
    partial class OtherTabControl
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
            this.pnlMain = new DevExpress.XtraEditors.PanelControl();
            this.btnEtc = new DevExpress.XtraEditors.SimpleButton();
            this.btnSheet = new DevExpress.XtraEditors.SimpleButton();
            this.btnRep = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.pnlMain)).BeginInit();
            this.pnlMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlMain
            // 
            this.pnlMain.Appearance.BackColor = System.Drawing.SystemColors.Control;
            this.pnlMain.Appearance.Options.UseBackColor = true;
            this.pnlMain.Controls.Add(this.btnEtc);
            this.pnlMain.Controls.Add(this.btnSheet);
            this.pnlMain.Controls.Add(this.btnRep);
            this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMain.Location = new System.Drawing.Point(0, 0);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Size = new System.Drawing.Size(24, 754);
            this.pnlMain.TabIndex = 0;
            // 
            // btnEtc
            // 
            this.btnEtc.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
            this.btnEtc.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.btnEtc.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.btnEtc.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
            this.btnEtc.Location = new System.Drawing.Point(2, 66);
            this.btnEtc.Name = "btnEtc";
            this.btnEtc.Size = new System.Drawing.Size(20, 60);
            this.btnEtc.TabIndex = 2;
            this.btnEtc.Text = "기\r\n\r\n\r\n타";
            this.btnEtc.Click += new System.EventHandler(this.btnEtc_Click);
            // 
            // btnSheet
            // 
            this.btnSheet.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSheet.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.btnSheet.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.btnSheet.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
            this.btnSheet.Location = new System.Drawing.Point(2, 34);
            this.btnSheet.Name = "btnSheet";
            this.btnSheet.Size = new System.Drawing.Size(20, 60);
            this.btnSheet.TabIndex =1;
            this.btnSheet.Text = "시\r\n\r\n\r\n트";
            this.btnSheet.Click += new System.EventHandler(this.btnSheet_Click);
            // 
            // btnRep
            // 
            this.btnRep.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
            this.btnRep.Appearance.ForeColor = System.Drawing.Color.Blue;
            this.btnRep.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.btnRep.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.btnRep.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
            this.btnRep.Location = new System.Drawing.Point(2, 2);
            this.btnRep.Name = "btnRep";
            this.btnRep.Size = new System.Drawing.Size(20, 60);
            this.btnRep.TabIndex = 0;
            this.btnRep.Text = "처\r\n\r\n\r\n방";
            this.btnRep.Click += new System.EventHandler(this.btnRep_Click);
            // 
            // OtherTabControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnlMain);
            this.Name = "OtherTabControl";
            this.Size = new System.Drawing.Size(24, 754);
            ((System.ComponentModel.ISupportInitialize)(this.pnlMain)).EndInit();
            this.pnlMain.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.PanelControl pnlMain;
        private DevExpress.XtraEditors.SimpleButton btnEtc;
        private DevExpress.XtraEditors.SimpleButton btnSheet;
        private DevExpress.XtraEditors.SimpleButton btnRep;
    }
}