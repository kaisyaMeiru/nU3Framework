namespace nU3.Modules.OCS.IN.OrderEntry.Controls
{
    partial class OtherOrderControl
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
            this.tabControl = new nU3.Core.UI.Controls.nU3XtraTabControl();
            this.tabOrder = new nU3.Core.UI.Controls.nU3XtraTabPage();
            this.pnlOrder = new nU3.Core.UI.Controls.nU3PanelControl();
            this.pnlOrderButtons = new nU3.Core.UI.Controls.nU3PanelControl();
            this.btnDietOrder = new nU3.Core.UI.Controls.nU3SimpleButton();
            this.btnPhysicalOrder = new nU3.Core.UI.Controls.nU3SimpleButton();
            this.btnNurseOrder = new nU3.Core.UI.Controls.nU3SimpleButton();
            this.btnSurgeryOrder = new nU3.Core.UI.Controls.nU3SimpleButton();
            this.btnRadOrder = new nU3.Core.UI.Controls.nU3SimpleButton();
            this.btnLabOrder = new nU3.Core.UI.Controls.nU3SimpleButton();
            this.tabSheet = new nU3.Core.UI.Controls.nU3XtraTabPage();
            this.lblSheet = new nU3.Core.UI.Controls.nU3LabelControl();
            this.tabEtc = new nU3.Core.UI.Controls.nU3XtraTabPage();
            this.lblEtc = new nU3.Core.UI.Controls.nU3LabelControl();
            ((System.ComponentModel.ISupportInitialize)(this.tabControl)).BeginInit();
            this.tabControl.SuspendLayout();
            this.tabOrder.SuspendLayout();
            this.pnlOrder.SuspendLayout();
            this.pnlOrderButtons.SuspendLayout();
            this.tabSheet.SuspendLayout();
            this.tabEtc.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedTabPage = this.tabOrder;
            this.tabControl.Size = new System.Drawing.Size(439, 729);
            this.tabControl.TabIndex = 0;
            this.tabControl.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
            this.tabOrder,
            this.tabSheet,
            this.tabEtc});
            // 
            // tabOrder
            // 
            this.tabOrder.Controls.Add(this.pnlOrder);
            this.tabOrder.Name = "tabOrder";
            this.tabOrder.Size = new System.Drawing.Size(433, 703);
            this.tabOrder.Text = "기타처방";
            // 
            // pnlOrder
            // 
            this.pnlOrder.Controls.Add(this.pnlOrderButtons);
            this.pnlOrder.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlOrder.Location = new System.Drawing.Point(0, 0);
            this.pnlOrder.Name = "pnlOrder";
            this.pnlOrder.Size = new System.Drawing.Size(433, 703);
            this.pnlOrder.TabIndex = 0;
            // 
            // pnlOrderButtons
            // 
            this.pnlOrderButtons.Appearance.BackColor = System.Drawing.SystemColors.Control;
            this.pnlOrderButtons.Appearance.Options.UseBackColor = true;
            this.pnlOrderButtons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlOrderButtons.Location = new System.Drawing.Point(0, 0);
            this.pnlOrderButtons.Name = "pnlOrderButtons";
            this.pnlOrderButtons.Size = new System.Drawing.Size(433, 703);
            this.pnlOrderButtons.TabIndex = 0;
            // 
            // btnDietOrder
            // 
            this.btnDietOrder.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.TopCenter;
            this.btnDietOrder.Location = new System.Drawing.Point(300, 420);
            this.btnDietOrder.Name = "btnDietOrder";
            this.btnDietOrder.Size = new System.Drawing.Size(120, 80);
            this.btnDietOrder.TabIndex = 5;
            this.btnDietOrder.Text = "식이처방";
            this.btnDietOrder.Click += new System.EventHandler(this.btnDietOrder_Click);
            // 
            // btnPhysicalOrder
            // 
            this.btnPhysicalOrder.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.TopCenter;
            this.btnPhysicalOrder.Location = new System.Drawing.Point(174, 420);
            this.btnPhysicalOrder.Name = "btnPhysicalOrder";
            this.btnPhysicalOrder.Size = new System.Drawing.Size(120, 80);
            this.btnPhysicalOrder.TabIndex = 4;
            this.btnPhysicalOrder.Text = "물리치료";
            this.btnPhysicalOrder.Click += new System.EventHandler(this.btnPhysicalOrder_Click);
            // 
            // btnNurseOrder
            // 
            this.btnNurseOrder.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.TopCenter;
            this.btnNurseOrder.Location = new System.Drawing.Point(300, 334);
            this.btnNurseOrder.Name = "btnNurseOrder";
            this.btnNurseOrder.Size = new System.Drawing.Size(120, 80);
            this.btnNurseOrder.TabIndex = 3;
            this.btnNurseOrder.Text = "간호처방";
            this.btnNurseOrder.Click += new System.EventHandler(this.btnNurseOrder_Click);
            // 
            // btnSurgeryOrder
            // 
            this.btnSurgeryOrder.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.TopCenter;
            this.btnSurgeryOrder.Location = new System.Drawing.Point(174, 334);
            this.btnSurgeryOrder.Name = "btnSurgeryOrder";
            this.btnSurgeryOrder.Size = new System.Drawing.Size(120, 80);
            this.btnSurgeryOrder.TabIndex = 2;
            this.btnSurgeryOrder.Text = "수술처방";
            this.btnSurgeryOrder.Click += new System.EventHandler(this.btnSurgeryOrder_Click);
            // 
            // btnRadOrder
            // 
            this.btnRadOrder.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.TopCenter;
            this.btnRadOrder.Location = new System.Drawing.Point(300, 248);
            this.btnRadOrder.Name = "btnRadOrder";
            this.btnRadOrder.Size = new System.Drawing.Size(120, 80);
            this.btnRadOrder.TabIndex = 1;
            this.btnRadOrder.Text = "방사선처방";
            this.btnRadOrder.Click += new System.EventHandler(this.btnRadOrder_Click);
            // 
            // btnLabOrder
            // 
            this.btnLabOrder.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.TopCenter;
            this.btnLabOrder.Location = new System.Drawing.Point(174, 248);
            this.btnLabOrder.Name = "btnLabOrder";
            this.btnLabOrder.Size = new System.Drawing.Size(120, 80);
            this.btnLabOrder.TabIndex = 0;
            this.btnLabOrder.Text = "검사처방";
            this.btnLabOrder.Click += new System.EventHandler(this.btnLabOrder_Click);
            // 
            // tabSheet
            // 
            this.tabSheet.Controls.Add(this.lblSheet);
            this.tabSheet.Name = "tabSheet";
            this.tabSheet.Size = new System.Drawing.Size(433, 703);
            this.tabSheet.Text = "시트";
            // 
            // lblSheet
            // 
            this.lblSheet.Appearance.Font = new System.Drawing.Font("Tahoma", 12F);
            this.lblSheet.Appearance.Options.UseFont = true;
            this.lblSheet.Appearance.Options.UseTextOptions = true;
            this.lblSheet.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.lblSheet.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblSheet.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblSheet.Location = new System.Drawing.Point(0, 0);
            this.lblSheet.Name = "lblSheet";
            this.lblSheet.Size = new System.Drawing.Size(433, 703);
            this.lblSheet.TabIndex = 0;
            this.lblSheet.Text = "시트 관리 기능은 개발 중입니다.";
            // 
            // tabEtc
            // 
            this.tabEtc.Controls.Add(this.lblEtc);
            this.tabEtc.Name = "tabEtc";
            this.tabEtc.Size = new System.Drawing.Size(433, 703);
            this.tabEtc.Text = "기타";
            // 
            // lblEtc
            // 
            this.lblEtc.Appearance.Font = new System.Drawing.Font("Tahoma", 12F);
            this.lblEtc.Appearance.Options.UseFont = true;
            this.lblEtc.Appearance.Options.UseTextOptions = true;
            this.lblEtc.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.lblEtc.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblEtc.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblEtc.Location = new System.Drawing.Point(0, 0);
            this.lblEtc.Name = "lblEtc";
            this.lblEtc.Size = new System.Drawing.Size(433, 703);
            this.lblEtc.TabIndex = 0;
            this.lblEtc.Text = "기타 처방 기능은 개발 중입니다.";
            // 
            // OtherOrderControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabControl);
            this.Name = "OtherOrderControl";
            this.Size = new System.Drawing.Size(439, 729);
            ((System.ComponentModel.ISupportInitialize)(this.tabControl)).EndInit();
            this.tabControl.ResumeLayout(false);
            this.tabOrder.ResumeLayout(false);
            this.pnlOrder.ResumeLayout(false);
            this.pnlOrderButtons.ResumeLayout(false);
            this.tabSheet.ResumeLayout(false);
            this.tabEtc.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private nU3.Core.UI.Controls.nU3XtraTabControl tabControl;
        private nU3.Core.UI.Controls.nU3XtraTabPage tabOrder;
        private nU3.Core.UI.Controls.nU3PanelControl pnlOrder;
        private nU3.Core.UI.Controls.nU3PanelControl pnlOrderButtons;
        private nU3.Core.UI.Controls.nU3SimpleButton btnDietOrder;
        private nU3.Core.UI.Controls.nU3SimpleButton btnPhysicalOrder;
        private nU3.Core.UI.Controls.nU3SimpleButton btnNurseOrder;
        private nU3.Core.UI.Controls.nU3SimpleButton btnSurgeryOrder;
        private nU3.Core.UI.Controls.nU3SimpleButton btnRadOrder;
        private nU3.Core.UI.Controls.nU3SimpleButton btnLabOrder;
        private nU3.Core.UI.Controls.nU3XtraTabPage tabSheet;
        private nU3.Core.UI.Controls.nU3LabelControl lblSheet;
        private nU3.Core.UI.Controls.nU3XtraTabPage tabEtc;
        private nU3.Core.UI.Controls.nU3LabelControl lblEtc;
    }
}