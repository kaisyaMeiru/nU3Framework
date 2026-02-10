namespace nU3.Modules.OCS.IN.MainEntry.Controls
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
            tabControl = new nU3.Core.UI.Controls.nU3XtraTabControl();
            tabOrder = new nU3.Core.UI.Controls.nU3XtraTabPage();
            pnlOrder = new nU3.Core.UI.Controls.nU3PanelControl();
            pnlOrderButtons = new nU3.Core.UI.Controls.nU3PanelControl();
            tabSheet = new nU3.Core.UI.Controls.nU3XtraTabPage();
            lblSheet = new nU3.Core.UI.Controls.nU3LabelControl();
            tabEtc = new nU3.Core.UI.Controls.nU3XtraTabPage();
            lblEtc = new nU3.Core.UI.Controls.nU3LabelControl();
            btnDietOrder = new nU3.Core.UI.Controls.nU3SimpleButton();
            btnPhysicalOrder = new nU3.Core.UI.Controls.nU3SimpleButton();
            btnNurseOrder = new nU3.Core.UI.Controls.nU3SimpleButton();
            btnSurgeryOrder = new nU3.Core.UI.Controls.nU3SimpleButton();
            btnRadOrder = new nU3.Core.UI.Controls.nU3SimpleButton();
            btnLabOrder = new nU3.Core.UI.Controls.nU3SimpleButton();
            ((System.ComponentModel.ISupportInitialize)tabControl).BeginInit();
            tabControl.SuspendLayout();
            tabOrder.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pnlOrder).BeginInit();
            pnlOrder.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pnlOrderButtons).BeginInit();
            tabSheet.SuspendLayout();
            tabEtc.SuspendLayout();
            SuspendLayout();
            // 
            // tabControl
            // 
            tabControl.Dock = DockStyle.Fill;
            tabControl.Location = new Point(0, 0);
            tabControl.Margin = new Padding(4);
            tabControl.Name = "tabControl";
            tabControl.SelectedTabPage = tabOrder;
            tabControl.Size = new Size(1305, 852);
            tabControl.TabIndex = 0;
            tabControl.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] { tabOrder, tabSheet, tabEtc });
            // 
            // tabOrder
            // 
            tabOrder.AuthId = "";
            tabOrder.Controls.Add(pnlOrder);
            tabOrder.Margin = new Padding(4);
            tabOrder.Name = "tabOrder";
            tabOrder.Size = new Size(1303, 826);
            tabOrder.Text = "기타처방";
            // 
            // pnlOrder
            // 
            pnlOrder.Controls.Add(pnlOrderButtons);
            pnlOrder.Dock = DockStyle.Fill;
            pnlOrder.Location = new Point(0, 0);
            pnlOrder.Margin = new Padding(4);
            pnlOrder.Name = "pnlOrder";
            pnlOrder.Size = new Size(1303, 826);
            pnlOrder.TabIndex = 0;
            // 
            // pnlOrderButtons
            // 
            pnlOrderButtons.Appearance.BackColor = SystemColors.Control;
            pnlOrderButtons.Appearance.Options.UseBackColor = true;
            pnlOrderButtons.Dock = DockStyle.Fill;
            pnlOrderButtons.Location = new Point(2, 2);
            pnlOrderButtons.Margin = new Padding(4);
            pnlOrderButtons.Name = "pnlOrderButtons";
            pnlOrderButtons.Size = new Size(1299, 822);
            pnlOrderButtons.TabIndex = 0;
            // 
            // tabSheet
            // 
            tabSheet.AuthId = "";
            tabSheet.Controls.Add(lblSheet);
            tabSheet.Margin = new Padding(4);
            tabSheet.Name = "tabSheet";
            tabSheet.Size = new Size(1303, 826);
            tabSheet.Text = "시트";
            // 
            // lblSheet
            // 
            lblSheet.Appearance.Font = new Font("Tahoma", 12F);
            lblSheet.Appearance.Options.UseFont = true;
            lblSheet.Appearance.Options.UseTextOptions = true;
            lblSheet.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            lblSheet.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            lblSheet.Dock = DockStyle.Fill;
            lblSheet.IsRequiredMarker = false;
            lblSheet.Location = new Point(0, 0);
            lblSheet.Margin = new Padding(4);
            lblSheet.Name = "lblSheet";
            lblSheet.Size = new Size(1303, 826);
            lblSheet.TabIndex = 0;
            lblSheet.Text = "시트 관리 기능은 개발 중입니다.";
            // 
            // tabEtc
            // 
            tabEtc.AuthId = "";
            tabEtc.Controls.Add(lblEtc);
            tabEtc.Margin = new Padding(4);
            tabEtc.Name = "tabEtc";
            tabEtc.Size = new Size(1303, 826);
            tabEtc.Text = "기타";
            // 
            // lblEtc
            // 
            lblEtc.Appearance.Font = new Font("Tahoma", 12F);
            lblEtc.Appearance.Options.UseFont = true;
            lblEtc.Appearance.Options.UseTextOptions = true;
            lblEtc.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            lblEtc.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            lblEtc.Dock = DockStyle.Fill;
            lblEtc.IsRequiredMarker = false;
            lblEtc.Location = new Point(0, 0);
            lblEtc.Margin = new Padding(4);
            lblEtc.Name = "lblEtc";
            lblEtc.Size = new Size(1303, 826);
            lblEtc.TabIndex = 0;
            lblEtc.Text = "기타 처방 기능은 개발 중입니다.";
            // 
            // btnDietOrder
            // 
            btnDietOrder.AuthId = "";
            btnDietOrder.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.TopCenter;
            btnDietOrder.Location = new Point(300, 420);
            btnDietOrder.Name = "btnDietOrder";
            btnDietOrder.Size = new Size(120, 80);
            btnDietOrder.TabIndex = 5;
            btnDietOrder.Text = "식이처방";
            btnDietOrder.Click += btnDietOrder_Click;
            // 
            // btnPhysicalOrder
            // 
            btnPhysicalOrder.AuthId = "";
            btnPhysicalOrder.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.TopCenter;
            btnPhysicalOrder.Location = new Point(174, 420);
            btnPhysicalOrder.Name = "btnPhysicalOrder";
            btnPhysicalOrder.Size = new Size(120, 80);
            btnPhysicalOrder.TabIndex = 4;
            btnPhysicalOrder.Text = "물리치료";
            btnPhysicalOrder.Click += btnPhysicalOrder_Click;
            // 
            // btnNurseOrder
            // 
            btnNurseOrder.AuthId = "";
            btnNurseOrder.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.TopCenter;
            btnNurseOrder.Location = new Point(300, 334);
            btnNurseOrder.Name = "btnNurseOrder";
            btnNurseOrder.Size = new Size(120, 80);
            btnNurseOrder.TabIndex = 3;
            btnNurseOrder.Text = "간호처방";
            btnNurseOrder.Click += btnNurseOrder_Click;
            // 
            // btnSurgeryOrder
            // 
            btnSurgeryOrder.AuthId = "";
            btnSurgeryOrder.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.TopCenter;
            btnSurgeryOrder.Location = new Point(174, 334);
            btnSurgeryOrder.Name = "btnSurgeryOrder";
            btnSurgeryOrder.Size = new Size(120, 80);
            btnSurgeryOrder.TabIndex = 2;
            btnSurgeryOrder.Text = "수술처방";
            btnSurgeryOrder.Click += btnSurgeryOrder_Click;
            // 
            // btnRadOrder
            // 
            btnRadOrder.AuthId = "";
            btnRadOrder.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.TopCenter;
            btnRadOrder.Location = new Point(300, 248);
            btnRadOrder.Name = "btnRadOrder";
            btnRadOrder.Size = new Size(120, 80);
            btnRadOrder.TabIndex = 1;
            btnRadOrder.Text = "방사선처방";
            btnRadOrder.Click += btnRadOrder_Click;
            // 
            // btnLabOrder
            // 
            btnLabOrder.AuthId = "";
            btnLabOrder.ImageOptions.Location = DevExpress.XtraEditors.ImageLocation.TopCenter;
            btnLabOrder.Location = new Point(174, 248);
            btnLabOrder.Name = "btnLabOrder";
            btnLabOrder.Size = new Size(120, 80);
            btnLabOrder.TabIndex = 0;
            btnLabOrder.Text = "검사처방";
            btnLabOrder.Click += btnLabOrder_Click;
            // 
            // OtherOrderControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(tabControl);
            Margin = new Padding(4);
            Name = "OtherOrderControl";
            Size = new Size(1305, 852);
            ((System.ComponentModel.ISupportInitialize)tabControl).EndInit();
            tabControl.ResumeLayout(false);
            tabOrder.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pnlOrder).EndInit();
            pnlOrder.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pnlOrderButtons).EndInit();
            tabSheet.ResumeLayout(false);
            tabEtc.ResumeLayout(false);
            ResumeLayout(false);

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