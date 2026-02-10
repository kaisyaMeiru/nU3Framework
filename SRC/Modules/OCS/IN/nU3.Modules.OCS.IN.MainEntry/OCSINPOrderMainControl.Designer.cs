namespace nU3.Modules.OCS.IN.MainEntry
{
    partial class OCSINPOrderMainControl
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
            pnlMain = new nU3.Core.UI.Controls.nU3PanelControl();
            pnlPatientInfo = new nU3.Core.UI.Controls.nU3PanelControl();
            pnlWaitList = new nU3.Core.UI.Controls.nU3PanelControl();
            pnlCenter = new nU3.Core.UI.Controls.nU3PanelControl();
            splitterDiagProblem = new DevExpress.XtraEditors.SplitterControl();
            pnlDiagProblem = new nU3.Core.UI.Controls.nU3PanelControl();
            pnlMemo = new nU3.Core.UI.Controls.nU3PanelControl();
            pnlRight = new nU3.Core.UI.Controls.nU3PanelControl();
            tabSupport = new nU3.Core.UI.Controls.nU3XtraTabControl();
            OrderPage = new nU3.Core.UI.Controls.nU3XtraTabPage();
            pnlBottom = new nU3.Core.UI.Controls.nU3PanelControl();
            btnConfirm = new nU3.Core.UI.Controls.nU3SimpleButton();
            btnHolding = new nU3.Core.UI.Controls.nU3SimpleButton();
            btnDelete = new nU3.Core.UI.Controls.nU3SimpleButton();
            btnSender = new nU3.Core.UI.Controls.nU3SimpleButton();
            pnlTop = new nU3.Core.UI.Controls.nU3PanelControl();
            pnlTopBar = new nU3.Core.UI.Controls.nU3PanelControl();
            btnOpHistory = new nU3.Core.UI.Controls.nU3SimpleButton();
            btnFamilyHistory = new nU3.Core.UI.Controls.nU3SimpleButton();
            btnPastHistory = new nU3.Core.UI.Controls.nU3SimpleButton();
            dtpOrdDate = new nU3.Core.UI.Controls.nU3DateEdit();
            cboOrderType = new nU3.Core.UI.Controls.nU3ComboBoxEdit();
            lblOrderType = new nU3.Core.UI.Controls.nU3LabelControl();
            lblTitle = new nU3.Core.UI.Controls.nU3LabelControl();
            splitterLeft = new DevExpress.XtraEditors.SplitterControl();
            splitterRight = new DevExpress.XtraEditors.SplitterControl();
            splitterCenter = new DevExpress.XtraEditors.SplitterControl();
            ((System.ComponentModel.ISupportInitialize)pnlMain).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pnlPatientInfo).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pnlWaitList).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pnlCenter).BeginInit();
            pnlCenter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pnlDiagProblem).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pnlMemo).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pnlRight).BeginInit();
            pnlRight.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)tabSupport).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pnlBottom).BeginInit();
            pnlBottom.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pnlTop).BeginInit();
            pnlTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pnlTopBar).BeginInit();
            pnlTopBar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dtpOrdDate.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dtpOrdDate.Properties.CalendarTimeProperties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)cboOrderType.Properties).BeginInit();
            SuspendLayout();
            // 
            // pnlMain
            // 
            pnlMain.Dock = DockStyle.Fill;
            pnlMain.Location = new Point(331, 120);
            pnlMain.Margin = new Padding(4);
            pnlMain.Name = "pnlMain";
            pnlMain.Size = new Size(508, 576);
            pnlMain.TabIndex = 0;
            // 
            // pnlPatientInfo
            // 
            pnlPatientInfo.Dock = DockStyle.Top;
            pnlPatientInfo.Location = new Point(0, 35);
            pnlPatientInfo.Margin = new Padding(4);
            pnlPatientInfo.Name = "pnlPatientInfo";
            pnlPatientInfo.Size = new Size(1389, 85);
            pnlPatientInfo.TabIndex = 1;
            // 
            // pnlWaitList
            // 
            pnlWaitList.Dock = DockStyle.Left;
            pnlWaitList.Location = new Point(0, 120);
            pnlWaitList.Margin = new Padding(4);
            pnlWaitList.Name = "pnlWaitList";
            pnlWaitList.Size = new Size(300, 797);
            pnlWaitList.TabIndex = 2;
            // 
            // pnlCenter
            // 
            pnlCenter.Controls.Add(splitterDiagProblem);
            pnlCenter.Controls.Add(pnlDiagProblem);
            pnlCenter.Controls.Add(pnlMemo);
            pnlCenter.Dock = DockStyle.Fill;
            pnlCenter.Location = new Point(310, 120);
            pnlCenter.Margin = new Padding(4);
            pnlCenter.Name = "pnlCenter";
            pnlCenter.Size = new Size(522, 797);
            pnlCenter.TabIndex = 3;
            // 
            // splitterDiagProblem
            // 
            splitterDiagProblem.Dock = DockStyle.Top;
            splitterDiagProblem.Location = new Point(2, 136);
            splitterDiagProblem.Margin = new Padding(4);
            splitterDiagProblem.Name = "splitterDiagProblem";
            splitterDiagProblem.Size = new Size(518, 10);
            splitterDiagProblem.TabIndex = 9;
            splitterDiagProblem.TabStop = false;
            // 
            // pnlDiagProblem
            // 
            pnlDiagProblem.Dock = DockStyle.Top;
            pnlDiagProblem.Location = new Point(2, 2);
            pnlDiagProblem.Margin = new Padding(4);
            pnlDiagProblem.Name = "pnlDiagProblem";
            pnlDiagProblem.Size = new Size(518, 134);
            pnlDiagProblem.TabIndex = 0;
            // 
            // pnlMemo
            // 
            pnlMemo.Dock = DockStyle.Bottom;
            pnlMemo.Location = new Point(2, 660);
            pnlMemo.Margin = new Padding(4);
            pnlMemo.Name = "pnlMemo";
            pnlMemo.Size = new Size(518, 135);
            pnlMemo.TabIndex = 1;
            // 
            // pnlRight
            // 
            pnlRight.Controls.Add(tabSupport);
            pnlRight.Dock = DockStyle.Right;
            pnlRight.Location = new Point(842, 120);
            pnlRight.Margin = new Padding(4);
            pnlRight.Name = "pnlRight";
            pnlRight.Size = new Size(547, 797);
            pnlRight.TabIndex = 4;
            // 
            // tabSupport
            // 
            tabSupport.Dock = DockStyle.Fill;
            tabSupport.Location = new Point(2, 2);
            tabSupport.Margin = new Padding(4);
            tabSupport.Name = "tabSupport";
            tabSupport.SelectedTabPage = OrderPage;
            tabSupport.Size = new Size(543, 793);
            tabSupport.TabIndex = 1;
            tabSupport.TabStop = false;
            // 
            // OrderPage
            // 
            OrderPage.AuthId = "";
            OrderPage.Margin = new Padding(4);
            OrderPage.Name = "OrderPage";
            OrderPage.Size = new Size(541, 791);
            OrderPage.Text = "기타처방";
            // 
            // pnlBottom
            // 
            pnlBottom.Controls.Add(btnConfirm);
            pnlBottom.Controls.Add(btnHolding);
            pnlBottom.Controls.Add(btnDelete);
            pnlBottom.Controls.Add(btnSender);
            pnlBottom.Dock = DockStyle.Bottom;
            pnlBottom.Location = new Point(0, 917);
            pnlBottom.Margin = new Padding(4);
            pnlBottom.Name = "pnlBottom";
            pnlBottom.Size = new Size(1389, 54);
            pnlBottom.TabIndex = 5;
            // 
            // btnConfirm
            // 
            btnConfirm.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnConfirm.AuthId = "";
            btnConfirm.Location = new Point(1096, 11);
            btnConfirm.Margin = new Padding(4);
            btnConfirm.Name = "btnConfirm";
            btnConfirm.Size = new Size(93, 32);
            btnConfirm.TabIndex = 1;
            btnConfirm.Text = "처방완료";
            btnConfirm.Click += OnMainButtonClick;
            // 
            // btnHolding
            // 
            btnHolding.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnHolding.AuthId = "";
            btnHolding.Location = new Point(995, 11);
            btnHolding.Margin = new Padding(4);
            btnHolding.Name = "btnHolding";
            btnHolding.Size = new Size(93, 32);
            btnHolding.TabIndex = 2;
            btnHolding.Text = "처방보류";
            btnHolding.Click += OnMainButtonClick;
            // 
            // btnDelete
            // 
            btnDelete.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnDelete.AuthId = "";
            btnDelete.Location = new Point(1197, 11);
            btnDelete.Margin = new Padding(4);
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new Size(93, 32);
            btnDelete.TabIndex = 0;
            btnDelete.Text = "처방삭제";
            btnDelete.Click += OnMainButtonClick;
            // 
            // btnSender
            // 
            btnSender.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnSender.AuthId = "";
            btnSender.Location = new Point(1298, 11);
            btnSender.Margin = new Padding(4);
            btnSender.Name = "btnSender";
            btnSender.Size = new Size(83, 32);
            btnSender.TabIndex = 0;
            btnSender.Text = "전자서명";
            btnSender.Click += OnMainButtonClick;
            // 
            // pnlTop
            // 
            pnlTop.Controls.Add(pnlTopBar);
            pnlTop.Dock = DockStyle.Top;
            pnlTop.Location = new Point(0, 0);
            pnlTop.Margin = new Padding(4);
            pnlTop.Name = "pnlTop";
            pnlTop.Size = new Size(1389, 35);
            pnlTop.TabIndex = 6;
            // 
            // pnlTopBar
            // 
            pnlTopBar.Appearance.BackColor = Color.FromArgb(224, 224, 224);
            pnlTopBar.Appearance.BorderColor = Color.FromArgb(197, 197, 197);
            pnlTopBar.Appearance.Options.UseBackColor = true;
            pnlTopBar.Appearance.Options.UseBorderColor = true;
            pnlTopBar.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            pnlTopBar.Controls.Add(btnOpHistory);
            pnlTopBar.Controls.Add(btnFamilyHistory);
            pnlTopBar.Controls.Add(btnPastHistory);
            pnlTopBar.Controls.Add(dtpOrdDate);
            pnlTopBar.Controls.Add(cboOrderType);
            pnlTopBar.Controls.Add(lblOrderType);
            pnlTopBar.Controls.Add(lblTitle);
            pnlTopBar.Dock = DockStyle.Fill;
            pnlTopBar.Location = new Point(2, 2);
            pnlTopBar.Margin = new Padding(4);
            pnlTopBar.Name = "pnlTopBar";
            pnlTopBar.Size = new Size(1385, 31);
            pnlTopBar.TabIndex = 0;
            // 
            // btnOpHistory
            // 
            btnOpHistory.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnOpHistory.AuthId = "";
            btnOpHistory.Location = new Point(1138, 1);
            btnOpHistory.Margin = new Padding(4);
            btnOpHistory.Name = "btnOpHistory";
            btnOpHistory.Size = new Size(70, 28);
            btnOpHistory.TabIndex = 67;
            btnOpHistory.Text = "수술력";
            btnOpHistory.Click += OnTopMainButtonClick;
            // 
            // btnFamilyHistory
            // 
            btnFamilyHistory.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnFamilyHistory.AuthId = "";
            btnFamilyHistory.Location = new Point(1211, 1);
            btnFamilyHistory.Margin = new Padding(4);
            btnFamilyHistory.Name = "btnFamilyHistory";
            btnFamilyHistory.Size = new Size(70, 28);
            btnFamilyHistory.TabIndex = 68;
            btnFamilyHistory.Text = "가족력";
            btnFamilyHistory.Click += OnTopMainButtonClick;
            // 
            // btnPastHistory
            // 
            btnPastHistory.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnPastHistory.AuthId = "";
            btnPastHistory.Location = new Point(1283, 1);
            btnPastHistory.Margin = new Padding(4);
            btnPastHistory.Name = "btnPastHistory";
            btnPastHistory.Size = new Size(70, 28);
            btnPastHistory.TabIndex = 69;
            btnPastHistory.Text = "과거력";
            btnPastHistory.Click += OnTopMainButtonClick;
            // 
            // dtpOrdDate
            // 
            dtpOrdDate.EditValue = new DateTime(2024, 2, 7, 0, 0, 0, 0);
            dtpOrdDate.Location = new Point(93, 5);
            dtpOrdDate.Margin = new Padding(4);
            dtpOrdDate.Name = "dtpOrdDate";
            dtpOrdDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton() });
            dtpOrdDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
            dtpOrdDate.Properties.Mask.EditMask = "yyyy-MM-dd";
            dtpOrdDate.Size = new Size(196, 20);
            dtpOrdDate.TabIndex = 65;
            // 
            // cboOrderType
            // 
            cboOrderType.IsRequired = false;
            cboOrderType.Location = new Point(393, 5);
            cboOrderType.Margin = new Padding(4);
            cboOrderType.Name = "cboOrderType";
            cboOrderType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton() });
            cboOrderType.Properties.Items.AddRange(new object[] { "약제처방", "주사처방", "외래처방", "검사처방", "방사선처방", "물리치료", "간호처방", "수술처방" });
            cboOrderType.Size = new Size(197, 20);
            cboOrderType.TabIndex = 71;
            // 
            // lblOrderType
            // 
            lblOrderType.Appearance.Font = new Font("Tahoma", 8.25F, FontStyle.Bold);
            lblOrderType.Appearance.ForeColor = Color.FromArgb(0, 0, 64);
            lblOrderType.Appearance.Options.UseFont = true;
            lblOrderType.Appearance.Options.UseForeColor = true;
            lblOrderType.IsRequiredMarker = false;
            lblOrderType.Location = new Point(296, 8);
            lblOrderType.Margin = new Padding(4);
            lblOrderType.Name = "lblOrderType";
            lblOrderType.Size = new Size(53, 13);
            lblOrderType.TabIndex = 75;
            lblOrderType.Text = "   처방타입";
            // 
            // lblTitle
            // 
            lblTitle.Appearance.Font = new Font("Tahoma", 8.25F, FontStyle.Bold);
            lblTitle.Appearance.ForeColor = Color.FromArgb(0, 0, 64);
            lblTitle.Appearance.Options.UseFont = true;
            lblTitle.Appearance.Options.UseForeColor = true;
            lblTitle.IsRequiredMarker = false;
            lblTitle.Location = new Point(4, 8);
            lblTitle.Margin = new Padding(4);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(53, 13);
            lblTitle.TabIndex = 75;
            lblTitle.Text = "   처방일자";
            // 
            // splitterLeft
            // 
            splitterLeft.Location = new Point(300, 120);
            splitterLeft.Margin = new Padding(4);
            splitterLeft.Name = "splitterLeft";
            splitterLeft.Size = new Size(10, 797);
            splitterLeft.TabIndex = 6;
            splitterLeft.TabStop = false;
            // 
            // splitterRight
            // 
            splitterRight.Dock = DockStyle.Right;
            splitterRight.Location = new Point(832, 120);
            splitterRight.Margin = new Padding(4);
            splitterRight.Name = "splitterRight";
            splitterRight.Size = new Size(10, 797);
            splitterRight.TabIndex = 7;
            splitterRight.TabStop = false;
            // 
            // splitterCenter
            // 
            splitterCenter.Dock = DockStyle.Right;
            splitterCenter.Location = new Point(1396, 35);
            splitterCenter.Margin = new Padding(4);
            splitterCenter.Name = "splitterCenter";
            splitterCenter.Size = new Size(10, 661);
            splitterCenter.TabIndex = 8;
            splitterCenter.TabStop = false;
            // 
            // OCSINPOrderMainControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(pnlCenter);
            Controls.Add(splitterRight);
            Controls.Add(pnlRight);
            Controls.Add(splitterLeft);
            Controls.Add(pnlWaitList);
            Controls.Add(pnlPatientInfo);
            Controls.Add(pnlTop);
            Controls.Add(pnlBottom);
            Margin = new Padding(4);
            Name = "OCSINPOrderMainControl";
            Size = new Size(1389, 971);
            ((System.ComponentModel.ISupportInitialize)pnlMain).EndInit();
            ((System.ComponentModel.ISupportInitialize)pnlPatientInfo).EndInit();
            ((System.ComponentModel.ISupportInitialize)pnlWaitList).EndInit();
            ((System.ComponentModel.ISupportInitialize)pnlCenter).EndInit();
            pnlCenter.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pnlDiagProblem).EndInit();
            ((System.ComponentModel.ISupportInitialize)pnlMemo).EndInit();
            ((System.ComponentModel.ISupportInitialize)pnlRight).EndInit();
            pnlRight.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)tabSupport).EndInit();
            ((System.ComponentModel.ISupportInitialize)pnlBottom).EndInit();
            pnlBottom.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pnlTop).EndInit();
            pnlTop.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pnlTopBar).EndInit();
            pnlTopBar.ResumeLayout(false);
            pnlTopBar.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dtpOrdDate.Properties.CalendarTimeProperties).EndInit();
            ((System.ComponentModel.ISupportInitialize)dtpOrdDate.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)cboOrderType.Properties).EndInit();
            ResumeLayout(false);

        }

        #endregion

        private nU3.Core.UI.Controls.nU3PanelControl pnlMain;
        private nU3.Core.UI.Controls.nU3PanelControl pnlPatientInfo;
        private nU3.Core.UI.Controls.nU3PanelControl pnlWaitList;
        private nU3.Core.UI.Controls.nU3PanelControl pnlCenter;
        private nU3.Core.UI.Controls.nU3PanelControl pnlRight;
        private nU3.Core.UI.Controls.nU3PanelControl pnlBottom;
        private nU3.Core.UI.Controls.nU3PanelControl pnlTop;
        private nU3.Core.UI.Controls.nU3PanelControl pnlTopBar;
        private nU3.Core.UI.Controls.nU3PanelControl pnlDiagProblem;
        private nU3.Core.UI.Controls.nU3PanelControl pnlMemo;
        private nU3.Modules.OCS.IN.MainEntry.Controls.PatientInfoControl PatientInfoControl;
        private nU3.Modules.OCS.IN.MainEntry.Controls.PatientListControl PatientListControl;
        private nU3.Modules.OCS.IN.MainEntry.Controls.OrderCodeControl OrderCodeControl;
        private nU3.Modules.OCS.IN.MainEntry.Controls.DiagCodeControl DiagCodeControl;
        private nU3.Modules.OCS.IN.MainEntry.Controls.ProblemListControl ProblemListControl;
        private nU3.Modules.OCS.IN.MainEntry.Controls.OtherOrderControl OtherOrderControl;
        private nU3.Modules.OCS.IN.MainEntry.Controls.SendMemoControl SendMemoControl;
        private nU3.Modules.OCS.IN.MainEntry.Controls.OtherTabControl OtherTabControl;
        private nU3.Core.UI.Controls.nU3XtraTabControl tabSupport;
        private nU3.Core.UI.Controls.nU3XtraTabPage OrderPage;
        private nU3.Core.UI.Controls.nU3SimpleButton btnConfirm;
        private nU3.Core.UI.Controls.nU3SimpleButton btnHolding;
        private nU3.Core.UI.Controls.nU3SimpleButton btnDelete;
        private nU3.Core.UI.Controls.nU3SimpleButton btnSender;
        private nU3.Core.UI.Controls.nU3SimpleButton btnOpHistory;
        private nU3.Core.UI.Controls.nU3SimpleButton btnFamilyHistory;
        private nU3.Core.UI.Controls.nU3SimpleButton btnPastHistory;
        private nU3.Core.UI.Controls.nU3ComboBoxEdit cboOrderType;
        private nU3.Core.UI.Controls.nU3DateEdit dtpOrdDate;
        private nU3.Core.UI.Controls.nU3LabelControl lblTitle;
        private nU3.Core.UI.Controls.nU3LabelControl lblOrderType;
        private DevExpress.XtraEditors.SplitterControl splitterLeft;
        private DevExpress.XtraEditors.SplitterControl splitterRight;
        private DevExpress.XtraEditors.SplitterControl splitterCenter;
        private DevExpress.XtraEditors.SplitterControl splitterDiagProblem;
    }
}