namespace nU3.Modules.OCS.IN.MainEntry.Controls
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
            pnlMain = new DevExpress.XtraEditors.PanelControl();
            btnEtc = new DevExpress.XtraEditors.SimpleButton();
            btnSheet = new DevExpress.XtraEditors.SimpleButton();
            btnRep = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)pnlMain).BeginInit();
            pnlMain.SuspendLayout();
            SuspendLayout();
            // 
            // pnlMain
            // 
            pnlMain.Appearance.BackColor = SystemColors.Control;
            pnlMain.Appearance.Options.UseBackColor = true;
            pnlMain.Controls.Add(btnEtc);
            pnlMain.Controls.Add(btnSheet);
            pnlMain.Controls.Add(btnRep);
            pnlMain.Dock = DockStyle.Fill;
            pnlMain.Location = new Point(0, 0);
            pnlMain.Margin = new Padding(4);
            pnlMain.Name = "pnlMain";
            pnlMain.Size = new Size(1406, 750);
            pnlMain.TabIndex = 0;
            // 
            // btnEtc
            // 
            btnEtc.Appearance.Options.UseTextOptions = true;
            btnEtc.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            btnEtc.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            btnEtc.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
            btnEtc.Location = new Point(2, 82);
            btnEtc.Margin = new Padding(4);
            btnEtc.Name = "btnEtc";
            btnEtc.Size = new Size(23, 75);
            btnEtc.TabIndex = 2;
            btnEtc.Text = "기\r\n\r\n\r\n타";
            btnEtc.Click += btnEtc_Click;
            // 
            // btnSheet
            // 
            btnSheet.Appearance.Options.UseTextOptions = true;
            btnSheet.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            btnSheet.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            btnSheet.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
            btnSheet.Location = new Point(2, 42);
            btnSheet.Margin = new Padding(4);
            btnSheet.Name = "btnSheet";
            btnSheet.Size = new Size(23, 75);
            btnSheet.TabIndex = 1;
            btnSheet.Text = "시\r\n\r\n\r\n트";
            btnSheet.Click += btnSheet_Click;
            // 
            // btnRep
            // 
            btnRep.Appearance.ForeColor = Color.Blue;
            btnRep.Appearance.Options.UseForeColor = true;
            btnRep.Appearance.Options.UseTextOptions = true;
            btnRep.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            btnRep.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            btnRep.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
            btnRep.Location = new Point(2, 2);
            btnRep.Margin = new Padding(4);
            btnRep.Name = "btnRep";
            btnRep.Size = new Size(23, 75);
            btnRep.TabIndex = 0;
            btnRep.Text = "처\r\n\r\n\r\n방";
            btnRep.Click += btnRep_Click;
            // 
            // OtherTabControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(pnlMain);
            Margin = new Padding(4);
            Name = "OtherTabControl";
            Size = new Size(1406, 750);
            ((System.ComponentModel.ISupportInitialize)pnlMain).EndInit();
            pnlMain.ResumeLayout(false);
            ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.PanelControl pnlMain;
        private DevExpress.XtraEditors.SimpleButton btnEtc;
        private DevExpress.XtraEditors.SimpleButton btnSheet;
        private DevExpress.XtraEditors.SimpleButton btnRep;
    }
}