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
            OrderCodeControl = new nU3.Modules.OCS.IN.MainEntry.Controls.OrderCodeControl();
            pnlPatientInfo = new nU3.Core.UI.Controls.nU3PanelControl();
            PatientInfoControl = new nU3.Core.UI.Components.Controls.PatientInfoControl();
            pnlWaitList = new nU3.Core.UI.Controls.nU3PanelControl();
            PatientListControl = new nU3.Core.UI.Components.Controls.PatientListControl();
            pnlCenter = new nU3.Core.UI.Controls.nU3PanelControl();
            ProblemListControl = new nU3.Modules.OCS.IN.MainEntry.Controls.ProblemListControl();
            splitterDiagProblem = new DevExpress.XtraEditors.SplitterControl();
            pnlDiagProblem = new nU3.Core.UI.Controls.nU3PanelControl();
            DiagCodeControl = new nU3.Modules.OCS.IN.MainEntry.Controls.DiagCodeControl();
            pnlMemo = new nU3.Core.UI.Controls.nU3PanelControl();
            SendMemoControl = new nU3.Modules.OCS.IN.MainEntry.Controls.SendMemoControl();
            pnlRight = new nU3.Core.UI.Controls.nU3PanelControl();
            tabSupport = new nU3.Core.UI.Controls.nU3XtraTabControl();
            OrderPage = new nU3.Core.UI.Controls.nU3XtraTabPage();
            OtherOrderControl = new nU3.Modules.OCS.IN.MainEntry.Controls.OtherOrderControl();
            OtherTabControl = new nU3.Modules.OCS.IN.MainEntry.Controls.OtherTabControl();
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
            pnlMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pnlPatientInfo).BeginInit();
            pnlPatientInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pnlWaitList).BeginInit();
            pnlWaitList.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pnlCenter).BeginInit();
            pnlCenter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pnlDiagProblem).BeginInit();
            pnlDiagProblem.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pnlMemo).BeginInit();
            pnlMemo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pnlRight).BeginInit();
            pnlRight.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)tabSupport).BeginInit();
            tabSupport.SuspendLayout();
            OrderPage.SuspendLayout();
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
            pnlMain.Controls.Add(OrderCodeControl);
            pnlMain.Dock = DockStyle.Fill;
            pnlMain.Location = new Point(500, 56);
            pnlMain.Margin = new Padding(4);
            pnlMain.Name = "pnlMain";
            pnlMain.Size = new Size(579, 705);
            pnlMain.TabIndex = 0;
            // 
            // OrderCodeControl
            // 
            OrderCodeControl.Dock = DockStyle.Fill;
            OrderCodeControl.Location = new Point(2, 2);
            OrderCodeControl.Margin = new Padding(4);
            OrderCodeControl.Name = "OrderCodeControl";
            OrderCodeControl.OrderType = "D";
            OrderCodeControl.Size = new Size(575, 701);
            OrderCodeControl.TabIndex = 0;
            // 
            // pnlPatientInfo
            // 
            pnlPatientInfo.Controls.Add(PatientInfoControl);
            pnlPatientInfo.Dock = DockStyle.Top;
            pnlPatientInfo.Location = new Point(0, 33);
            pnlPatientInfo.Margin = new Padding(4);
            pnlPatientInfo.Name = "pnlPatientInfo";
            pnlPatientInfo.Size = new Size(1495, 23);
            pnlPatientInfo.TabIndex = 1;
            // 
            // PatientInfoControl
            // 
            PatientInfoControl.Dock = DockStyle.Fill;
            PatientInfoControl.EventBus = null;
            PatientInfoControl.EventBusUse = false;
            PatientInfoControl.EventSource = "BaseWorkComponent";
            PatientInfoControl.Location = new Point(2, 2);
            PatientInfoControl.Margin = new Padding(4);
            PatientInfoControl.Name = "PatientInfoControl";
            PatientInfoControl.OwnerEventBus = null;
            PatientInfoControl.OwnerProgramID = null;
            PatientInfoControl.Size = new Size(1491, 19);
            PatientInfoControl.TabIndex = 0;
            // 
            // pnlWaitList
            // 
            pnlWaitList.Controls.Add(PatientListControl);
            pnlWaitList.Dock = DockStyle.Left;
            pnlWaitList.Location = new Point(0, 56);
            pnlWaitList.Margin = new Padding(4);
            pnlWaitList.Name = "pnlWaitList";
            pnlWaitList.Size = new Size(490, 705);
            pnlWaitList.TabIndex = 2;
            // 
            // PatientListControl
            // 
            PatientListControl.Dock = DockStyle.Fill;
            PatientListControl.EventBus = null;
            PatientListControl.EventBusUse = true;
            PatientListControl.EventSource = "BaseWorkComponent";
            PatientListControl.Location = new Point(2, 2);
            PatientListControl.Margin = new Padding(4);
            PatientListControl.Name = "PatientListControl";
            PatientListControl.OwnerEventBus = null;
            PatientListControl.OwnerProgramID = null;
            PatientListControl.Size = new Size(486, 701);
            PatientListControl.TabIndex = 0;
            // 
            // pnlCenter
            // 
            pnlCenter.Controls.Add(ProblemListControl);
            pnlCenter.Controls.Add(splitterDiagProblem);
            pnlCenter.Controls.Add(pnlDiagProblem);
            pnlCenter.Controls.Add(pnlMemo);
            pnlCenter.Dock = DockStyle.Fill;
            pnlCenter.Location = new Point(500, 56);
            pnlCenter.Margin = new Padding(4);
            pnlCenter.Name = "pnlCenter";
            pnlCenter.Size = new Size(579, 705);
            pnlCenter.TabIndex = 3;
            // 
            // ProblemListControl
            // 
            ProblemListControl.Dock = DockStyle.Fill;
            ProblemListControl.Location = new Point(2, 137);
            ProblemListControl.Margin = new Padding(4);
            ProblemListControl.Name = "ProblemListControl";
            ProblemListControl.Size = new Size(575, 440);
            ProblemListControl.TabIndex = 1;
            // 
            // splitterDiagProblem
            // 
            splitterDiagProblem.Dock = DockStyle.Top;
            splitterDiagProblem.Location = new Point(2, 127);
            splitterDiagProblem.Margin = new Padding(4);
            splitterDiagProblem.Name = "splitterDiagProblem";
            splitterDiagProblem.Size = new Size(575, 10);
            splitterDiagProblem.TabIndex = 9;
            splitterDiagProblem.TabStop = false;
            // 
            // pnlDiagProblem
            // 
            pnlDiagProblem.Controls.Add(DiagCodeControl);
            pnlDiagProblem.Dock = DockStyle.Top;
            pnlDiagProblem.Location = new Point(2, 2);
            pnlDiagProblem.Margin = new Padding(4);
            pnlDiagProblem.Name = "pnlDiagProblem";
            pnlDiagProblem.Size = new Size(575, 125);
            pnlDiagProblem.TabIndex = 0;
            // 
            // DiagCodeControl
            // 
            DiagCodeControl.Dock = DockStyle.Fill;
            DiagCodeControl.Location = new Point(2, 2);
            DiagCodeControl.Margin = new Padding(4);
            DiagCodeControl.Name = "DiagCodeControl";
            DiagCodeControl.Size = new Size(571, 121);
            DiagCodeControl.TabIndex = 0;
            // 
            // pnlMemo
            // 
            pnlMemo.Controls.Add(SendMemoControl);
            pnlMemo.Dock = DockStyle.Bottom;
            pnlMemo.Location = new Point(2, 577);
            pnlMemo.Margin = new Padding(4);
            pnlMemo.Name = "pnlMemo";
            pnlMemo.Size = new Size(575, 126);
            pnlMemo.TabIndex = 1;
            // 
            // SendMemoControl
            // 
            SendMemoControl.Dock = DockStyle.Fill;
            SendMemoControl.Location = new Point(2, 2);
            SendMemoControl.Margin = new Padding(4);
            SendMemoControl.Name = "SendMemoControl";
            SendMemoControl.Size = new Size(571, 122);
            SendMemoControl.TabIndex = 0;
            // 
            // pnlRight
            // 
            pnlRight.Controls.Add(tabSupport);
            pnlRight.Controls.Add(OtherTabControl);
            pnlRight.Dock = DockStyle.Right;
            pnlRight.Location = new Point(1089, 56);
            pnlRight.Margin = new Padding(4);
            pnlRight.Name = "pnlRight";
            pnlRight.Size = new Size(406, 705);
            pnlRight.TabIndex = 4;
            // 
            // tabSupport
            // 
            tabSupport.Dock = DockStyle.Fill;
            tabSupport.Location = new Point(2, 2);
            tabSupport.Margin = new Padding(4);
            tabSupport.Name = "tabSupport";
            tabSupport.SelectedTabPage = OrderPage;
            tabSupport.Size = new Size(365, 701);
            tabSupport.TabIndex = 1;
            tabSupport.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] { OrderPage });
            tabSupport.TabStop = false;
            // 
            // OrderPage
            // 
            OrderPage.AuthId = "";
            OrderPage.Controls.Add(OtherOrderControl);
            OrderPage.Margin = new Padding(4);
            OrderPage.Name = "OrderPage";
            OrderPage.Size = new Size(363, 675);
            OrderPage.Text = "기타처방";
            // 
            // OtherOrderControl
            // 
            OtherOrderControl.Dock = DockStyle.Fill;
            OtherOrderControl.Location = new Point(0, 0);
            OtherOrderControl.Margin = new Padding(4);
            OtherOrderControl.Name = "OtherOrderControl";
            OtherOrderControl.RefCodeType = MainEntry.Controls.RefCodeType.REP;
            OtherOrderControl.Size = new Size(363, 675);
            OtherOrderControl.TabIndex = 0;
            // 
            // OtherTabControl
            // 
            OtherTabControl.Dock = DockStyle.Right;
            OtherTabControl.Location = new Point(367, 2);
            OtherTabControl.Margin = new Padding(4);
            OtherTabControl.Name = "OtherTabControl";
            OtherTabControl.RefCodeType = MainEntry.Controls.RefCodeType.REP;
            OtherTabControl.Size = new Size(37, 701);
            OtherTabControl.TabIndex = 1;
            // 
            // pnlBottom
            // 
            pnlBottom.Controls.Add(btnConfirm);
            pnlBottom.Controls.Add(btnHolding);
            pnlBottom.Controls.Add(btnDelete);
            pnlBottom.Controls.Add(btnSender);
            pnlBottom.Dock = DockStyle.Bottom;
            pnlBottom.Location = new Point(0, 761);
            pnlBottom.Margin = new Padding(4);
            pnlBottom.Name = "pnlBottom";
            pnlBottom.Size = new Size(1495, 50);
            pnlBottom.TabIndex = 5;
            // 
            // btnConfirm
            // 
            btnConfirm.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnConfirm.AuthId = "";
            btnConfirm.Location = new Point(1202, 10);
            btnConfirm.Margin = new Padding(4);
            btnConfirm.Name = "btnConfirm";
            btnConfirm.Size = new Size(93, 30);
            btnConfirm.TabIndex = 1;
            btnConfirm.Text = "처방완료";
            btnConfirm.Click += OnMainButtonClick;
            // 
            // btnHolding
            // 
            btnHolding.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnHolding.AuthId = "";
            btnHolding.Location = new Point(1101, 10);
            btnHolding.Margin = new Padding(4);
            btnHolding.Name = "btnHolding";
            btnHolding.Size = new Size(93, 30);
            btnHolding.TabIndex = 2;
            btnHolding.Text = "처방보류";
            btnHolding.Click += OnMainButtonClick;
            // 
            // btnDelete
            // 
            btnDelete.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnDelete.AuthId = "";
            btnDelete.Location = new Point(1303, 10);
            btnDelete.Margin = new Padding(4);
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new Size(93, 30);
            btnDelete.TabIndex = 0;
            btnDelete.Text = "처방삭제";
            btnDelete.Click += OnMainButtonClick;
            // 
            // btnSender
            // 
            btnSender.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnSender.AuthId = "";
            btnSender.Location = new Point(1404, 10);
            btnSender.Margin = new Padding(4);
            btnSender.Name = "btnSender";
            btnSender.Size = new Size(83, 30);
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
            pnlTop.Size = new Size(1495, 33);
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
            pnlTopBar.Size = new Size(1491, 29);
            pnlTopBar.TabIndex = 0;
            // 
            // btnOpHistory
            // 
            btnOpHistory.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnOpHistory.AuthId = "";
            btnOpHistory.Location = new Point(1244, 1);
            btnOpHistory.Margin = new Padding(4);
            btnOpHistory.Name = "btnOpHistory";
            btnOpHistory.Size = new Size(70, 26);
            btnOpHistory.TabIndex = 67;
            btnOpHistory.Text = "수술력";
            btnOpHistory.Click += OnTopMainButtonClick;
            // 
            // btnFamilyHistory
            // 
            btnFamilyHistory.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnFamilyHistory.AuthId = "";
            btnFamilyHistory.Location = new Point(1317, 1);
            btnFamilyHistory.Margin = new Padding(4);
            btnFamilyHistory.Name = "btnFamilyHistory";
            btnFamilyHistory.Size = new Size(70, 26);
            btnFamilyHistory.TabIndex = 68;
            btnFamilyHistory.Text = "가족력";
            btnFamilyHistory.Click += OnTopMainButtonClick;
            // 
            // btnPastHistory
            // 
            btnPastHistory.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnPastHistory.AuthId = "";
            btnPastHistory.Location = new Point(1389, 1);
            btnPastHistory.Margin = new Padding(4);
            btnPastHistory.Name = "btnPastHistory";
            btnPastHistory.Size = new Size(70, 26);
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
            lblOrderType.Location = new Point(296, 7);
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
            lblTitle.Location = new Point(4, 7);
            lblTitle.Margin = new Padding(4);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(53, 13);
            lblTitle.TabIndex = 75;
            lblTitle.Text = "   처방일자";
            // 
            // splitterLeft
            // 
            splitterLeft.Location = new Point(490, 56);
            splitterLeft.Margin = new Padding(4);
            splitterLeft.Name = "splitterLeft";
            splitterLeft.Size = new Size(10, 705);
            splitterLeft.TabIndex = 6;
            splitterLeft.TabStop = false;
            // 
            // splitterRight
            // 
            splitterRight.Dock = DockStyle.Right;
            splitterRight.Location = new Point(1079, 56);
            splitterRight.Margin = new Padding(4);
            splitterRight.Name = "splitterRight";
            splitterRight.Size = new Size(10, 705);
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
            AutoScaleDimensions = new SizeF(7F, 14F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(pnlMain);
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
            Size = new Size(1495, 811);
            ((System.ComponentModel.ISupportInitialize)pnlMain).EndInit();
            pnlMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pnlPatientInfo).EndInit();
            pnlPatientInfo.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pnlWaitList).EndInit();
            pnlWaitList.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pnlCenter).EndInit();
            pnlCenter.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pnlDiagProblem).EndInit();
            pnlDiagProblem.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pnlMemo).EndInit();
            pnlMemo.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pnlRight).EndInit();
            pnlRight.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)tabSupport).EndInit();
            tabSupport.ResumeLayout(false);
            OrderPage.ResumeLayout(false);
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
        private nU3.Core.UI.Components.Controls.PatientInfoControl PatientInfoControl;
        private nU3.Core.UI.Components.Controls.PatientListControl PatientListControl;
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