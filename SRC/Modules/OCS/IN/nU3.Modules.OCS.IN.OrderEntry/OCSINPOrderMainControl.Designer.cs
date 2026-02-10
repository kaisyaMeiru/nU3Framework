namespace nU3.Modules.OCS.IN.OrderEntry
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
            this.components = new System.ComponentModel.Container();
            
            // 메인 패널
            this.pnlMain = new nU3.Core.UI.Controls.nU3PanelControl();
            this.pnlPatientInfo = new nU3.Core.UI.Controls.nU3PanelControl();
            this.pnlWaitList = new nU3.Core.UI.Controls.nU3PanelControl();
            this.pnlCenter = new nU3.Core.UI.Controls.nU3PanelControl();
            this.pnlRight = new nU3.Core.UI.Controls.nU3PanelControl();
            this.pnlBottom = new nU3.Core.UI.Controls.nU3PanelControl();
            
            // 상단 패널
            this.pnlTop = new nU3.Core.UI.Controls.nU3PanelControl();
            this.pnlTopBar = new nU3.Core.UI.Controls.nU3PanelControl();
            
            // 하단 패널
            this.pnlDiagProblem = new nU3.Core.UI.Controls.nU3PanelControl();
            this.pnlMemo = new nU3.Core.UI.Controls.nU3PanelControl();
            
            // 컨트롤들
            this.PatientInfoControl = new nU3.Modules.OCS.IN.OrderEntry.Controls.PatientInfoControl();
            this.PatientListControl = new nU3.Modules.OCS.IN.OrderEntry.Controls.PatientListControl();
            this.OrderCodeControl = new nU3.Modules.OCS.IN.OrderEntry.Controls.OrderCodeControl();
            this.DiagCodeControl = new nU3.Modules.OCS.IN.OrderEntry.Controls.DiagCodeControl();
            this.ProblemListControl = new nU3.Modules.OCS.IN.OrderEntry.Controls.ProblemListControl();
            this.OtherOrderControl = new nU3.Modules.OCS.IN.OrderEntry.Controls.OtherOrderControl();
            this.SendMemoControl = new nU3.Modules.OCS.IN.OrderEntry.Controls.SendMemoControl();
            this.OtherTabControl = new nU3.Modules.OCS.IN.OrderEntry.Controls.OtherTabControl();
            
            // 탭 컨트롤
            this.tabSupport = new nU3.Core.UI.Controls.nU3XtraTabControl();
            this.OrderPage = new nU3.Core.UI.Controls.nU3XtraTabPage();
            
            // 버튼들
            this.btnConfirm = new nU3.Core.UI.Controls.nU3SimpleButton();
            this.btnHolding = new nU3.Core.UI.Controls.nU3SimpleButton();
            this.btnDelete = new nU3.Core.UI.Controls.nU3SimpleButton();
            this.btnSender = new nU3.Core.UI.Controls.nU3SimpleButton();
            
            // 상단 버튼들
            this.btnOpHistory = new nU3.Core.UI.Controls.nU3SimpleButton();
            this.btnFamilyHistory = new nU3.Core.UI.Controls.nU3SimpleButton();
            this.btnPastHistory = new nU3.Core.UI.Controls.nU3SimpleButton();
            
            // 기타 컨트롤들
            this.cboOrderType = new nU3.Core.UI.Controls.nU3ComboBoxEdit();
            this.dtpOrdDate = new nU3.Core.UI.Controls.nU3DateEdit();
            this.lblTitle = new nU3.Core.UI.Controls.nU3LabelControl();
            this.lblOrderType = new nU3.Core.UI.Controls.nU3LabelControl();
            
            // 스플리터
            this.splitterLeft = new DevExpress.XtraEditors.SplitterControl();
            this.splitterRight = new DevExpress.XtraEditors.SplitterControl();
            this.splitterCenter = new DevExpress.XtraEditors.SplitterControl();
            this.splitterDiagProblem = new DevExpress.XtraEditors.SplitterControl();
            
            ((System.ComponentModel.ISupportInitialize)(this.pnlMain)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnlPatientInfo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnlWaitList)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnlCenter)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnlRight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnlBottom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnlTop)).BeginInit();
            this.pnlTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pnlTopBar)).BeginInit();
            this.pnlTopBar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pnlDiagProblem)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnlMemo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tabSupport)).BeginInit();
            this.tabSupport.SuspendLayout();
            this.OrderPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboOrderType.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtpOrdDate.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlMain
            // 
            this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMain.Location = new System.Drawing.Point(0, 0);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Size = new System.Drawing.Size(1280, 865);
            this.pnlMain.TabIndex = 0;
            
            // 
            // pnlPatientInfo
            // 
            this.pnlPatientInfo.Controls.Add(this.PatientInfoControl);
            this.pnlPatientInfo.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlPatientInfo.Location = new System.Drawing.Point(0, 0);
            this.pnlPatientInfo.Name = "pnlPatientInfo";
            this.pnlPatientInfo.Size = new System.Drawing.Size(1280, 68);
            this.pnlPatientInfo.TabIndex = 1;
            
            // 
            // pnlWaitList
            // 
            this.pnlWaitList.Controls.Add(this.PatientListControl);
            this.pnlWaitList.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlWaitList.Location = new System.Drawing.Point(0, 68);
            this.pnlWaitList.Name = "pnlWaitList";
            this.pnlWaitList.Size = new System.Drawing.Size(275, 754);
            this.pnlWaitList.TabIndex = 2;
            
            // 
            // pnlCenter
            // 
            this.pnlCenter.Controls.Add(this.OrderCodeControl);
            this.pnlCenter.Controls.Add(this.pnlDiagProblem);
            this.pnlCenter.Controls.Add(this.splitterDiagProblem);
            this.pnlCenter.Controls.Add(this.pnlMemo);
            this.pnlCenter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlCenter.Location = new System.Drawing.Point(275, 68);
            this.pnlCenter.Name = "pnlCenter";
            this.pnlCenter.Size = new System.Drawing.Size(536, 754);
            this.pnlCenter.TabIndex = 3;
            
            // 
            // pnlRight
            // 
            this.pnlRight.Controls.Add(this.tabSupport);
            this.pnlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnlRight.Location = new System.Drawing.Point(811, 68);
            this.pnlRight.Name = "pnlRight";
            this.pnlRight.Size = new System.Drawing.Size(469, 754);
            this.pnlRight.TabIndex = 4;
            
            // 
            // pnlBottom
            // 
            this.pnlBottom.Controls.Add(this.btnConfirm);
            this.pnlBottom.Controls.Add(this.btnHolding);
            this.pnlBottom.Controls.Add(this.btnDelete);
            this.pnlBottom.Controls.Add(this.btnSender);
            this.pnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlBottom.Location = new System.Drawing.Point(0, 822);
            this.pnlBottom.Name = "pnlBottom";
            this.pnlBottom.Size = new System.Drawing.Size(1280, 43);
            this.pnlBottom.TabIndex = 5;
            
            // 
            // pnlTop
            // 
            this.pnlTop.Controls.Add(this.pnlTopBar);
            this.pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTop.Location = new System.Drawing.Point(0, 68);
            this.pnlTop.Name = "pnlTop";
            this.pnlTop.Size = new System.Drawing.Size(1280, 28);
            this.pnlTop.TabIndex = 6;
            
            // 
            // pnlTopBar
            // 
            this.pnlTopBar.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.pnlTopBar.Appearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(197)))), ((int)(((byte)(197)))), ((int)(((byte)(197)))));
            this.pnlTopBar.Appearance.Options.UseBackColor = true;
            this.pnlTopBar.Appearance.Options.UseBorderColor = true;
            this.pnlTopBar.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.pnlTopBar.Controls.Add(this.btnOpHistory);
            this.pnlTopBar.Controls.Add(this.btnFamilyHistory);
            this.pnlTopBar.Controls.Add(this.btnPastHistory);
            this.pnlTopBar.Controls.Add(this.dtpOrdDate);
            this.pnlTopBar.Controls.Add(this.cboOrderType);
            this.pnlTopBar.Controls.Add(this.lblOrderType);
            this.pnlTopBar.Controls.Add(this.lblTitle);
            this.pnlTopBar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlTopBar.Location = new System.Drawing.Point(0, 0);
            this.pnlTopBar.Name = "pnlTopBar";
            this.pnlTopBar.Size = new System.Drawing.Size(1280, 28);
            this.pnlTopBar.TabIndex = 0;
            
            // 
            // pnlDiagProblem
            // 
            this.pnlDiagProblem.Controls.Add(this.DiagCodeControl);
            this.pnlDiagProblem.Controls.Add(this.ProblemListControl);
            this.pnlDiagProblem.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlDiagProblem.Location = new System.Drawing.Point(0, 0);
            this.pnlDiagProblem.Name = "pnlDiagProblem";
            this.pnlDiagProblem.Size = new System.Drawing.Size(536, 107);
            this.pnlDiagProblem.TabIndex = 0;
            
            // 
            // pnlMemo
            // 
            this.pnlMemo.Controls.Add(this.SendMemoControl);
            this.pnlMemo.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlMemo.Location = new System.Drawing.Point(0, 648);
            this.pnlMemo.Name = "pnlMemo";
            this.pnlMemo.Size = new System.Drawing.Size(536, 108);
            this.pnlMemo.TabIndex = 1;
            
            // 
            // btnConfirm
            // 
            this.btnConfirm.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnConfirm.Location = new System.Drawing.Point(377, 4);
            this.btnConfirm.Name = "btnConfirm";
            this.btnConfirm.Size = new System.Drawing.Size(80, 26);
            this.btnConfirm.TabIndex = 1;
            this.btnConfirm.Text = "처방완료";
            this.btnConfirm.Click += new System.EventHandler(this.OnMainButtonClick);
            
            // 
            // btnHolding
            // 
            this.btnHolding.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnHolding.Location = new System.Drawing.Point(297, 4);
            this.btnHolding.Name = "btnHolding";
            this.btnHolding.Size = new System.Drawing.Size(80, 26);
            this.btnHolding.TabIndex = 2;
            this.btnHolding.Text = "처방보류";
            this.btnHolding.Click += new System.EventHandler(this.OnMainButtonClick);
            
            // 
            // btnDelete
            // 
            this.btnDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDelete.Location = new System.Drawing.Point(457, 4);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(80, 26);
            this.btnDelete.TabIndex = 0;
            this.btnDelete.Text = "처방삭제";
            this.btnDelete.Click += new System.EventHandler(this.OnMainButtonClick);
            
            // 
            // btnSender
            // 
            this.btnSender.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSender.Location = new System.Drawing.Point(537, 4);
            this.btnSender.Name = "btnSender";
            this.btnSender.Size = new System.Drawing.Size(80, 26);
            this.btnSender.TabIndex = 0;
            this.btnSender.Text = "전자서명";
            this.btnSender.Click += new System.EventHandler(this.OnMainButtonClick);
            
            // 
            // btnOpHistory
            // 
            this.btnOpHistory.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            // this.btnOpHistory.Appearance.ImageOptions.Image = global::nU3.Modules.OCS.IN.OrderEntry.Properties.Resources.op;
            this.btnOpHistory.Location = new System.Drawing.Point(1069, 3);
            this.btnOpHistory.Name = "btnOpHistory";
            this.btnOpHistory.Size = new System.Drawing.Size(60, 22);
            this.btnOpHistory.TabIndex = 67;
            this.btnOpHistory.Text = "수술력";
            this.btnOpHistory.Click += new System.EventHandler(this.OnTopMainButtonClick);
            
            // 
            // btnFamilyHistory
            // 
            this.btnFamilyHistory.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            // this.btnFamilyHistory.Appearance.ImageOptions.Image = global::nU3.Modules.OCS.IN.OrderEntry.Properties.Resources.family;
            this.btnFamilyHistory.Location = new System.Drawing.Point(1131, 3);
            this.btnFamilyHistory.Name = "btnFamilyHistory";
            this.btnFamilyHistory.Size = new System.Drawing.Size(60, 22);
            this.btnFamilyHistory.TabIndex = 68;
            this.btnFamilyHistory.Text = "가족력";
            this.btnFamilyHistory.Click += new System.EventHandler(this.OnTopMainButtonClick);
            
            // 
            // btnPastHistory
            // 
            this.btnPastHistory.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            // this.btnPastHistory.Appearance.ImageOptions.Image = global::nU3.Modules.OCS.IN.OrderEntry.Properties.Resources.past;
            this.btnPastHistory.Location = new System.Drawing.Point(1193, 3);
            this.btnPastHistory.Name = "btnPastHistory";
            this.btnPastHistory.Size = new System.Drawing.Size(60, 22);
            this.btnPastHistory.TabIndex = 69;
            this.btnPastHistory.Text = "과거력";
            this.btnPastHistory.Click += new System.EventHandler(this.OnTopMainButtonClick);
            
            // 
            // cboOrderType
            // 
            this.cboOrderType.Location = new System.Drawing.Point(337, 4);
            this.cboOrderType.Name = "cboOrderType";
            this.cboOrderType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.cboOrderType.Properties.Items.AddRange(new object[] {
            "약제처방",
            "주사처방",
            "외래처방",
            "검사처방",
            "방사선처방",
            "물리치료",
            "간호처방",
            "수술처방"});
            this.cboOrderType.Size = new System.Drawing.Size(169, 20);
            this.cboOrderType.TabIndex = 71;
            
            // 
            // dtpOrdDate
            // 
            this.dtpOrdDate.EditValue = new System.DateTime(2024, 2, 7, 0, 0, 0, 0);
            this.dtpOrdDate.Location = new System.Drawing.Point(80, 4);
            this.dtpOrdDate.Name = "dtpOrdDate";
            this.dtpOrdDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.dtpOrdDate.Properties.Mask.EditMask = "yyyy-MM-dd";
            this.dtpOrdDate.Size = new System.Drawing.Size(168, 21);
            this.dtpOrdDate.TabIndex = 65;
            
            // 
            // lblTitle
            // 
            this.lblTitle.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.lblTitle.Location = new System.Drawing.Point(3, 6);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(103, 18);
            this.lblTitle.TabIndex = 75;
            this.lblTitle.Text = "   처방일자";
            
            // 
            // lblOrderType
            // 
            this.lblOrderType.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblOrderType.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.lblOrderType.Location = new System.Drawing.Point(254, 6);
            this.lblOrderType.Name = "lblOrderType";
            this.lblOrderType.Size = new System.Drawing.Size(93, 18);
            this.lblOrderType.TabIndex = 75;
            this.lblOrderType.Text = "   처방타입";
            
            // 
            // tabSupport
            // 
            this.tabSupport.Controls.Add(this.OrderPage);
            this.tabSupport.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabSupport.Location = new System.Drawing.Point(0, 0);
            this.tabSupport.Name = "tabSupport";
            this.tabSupport.SelectedTabPage = this.OrderPage;
            this.tabSupport.Size = new System.Drawing.Size(469, 754);
            this.tabSupport.TabIndex = 1;
            this.tabSupport.TabStop = false;
            
            // 
            // OrderPage
            // 
            this.OrderPage.Controls.Add(this.OtherOrderControl);
            this.OrderPage.Name = "OrderPage";
            this.OrderPage.Size = new System.Drawing.Size(463, 729);
            this.OrderPage.Text = "기타처방";
            
            // 
            // splitterLeft
            // 
            this.splitterLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.splitterLeft.Location = new System.Drawing.Point(275, 68);
            this.splitterLeft.Name = "splitterLeft";
            this.splitterLeft.Size = new System.Drawing.Size(6, 754);
            this.splitterLeft.TabIndex = 6;
            
            // 
            // splitterRight
            // 
            this.splitterRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.splitterRight.Location = new System.Drawing.Point(805, 68);
            this.splitterRight.Name = "splitterRight";
            this.splitterRight.Size = new System.Drawing.Size(6, 754);
            this.splitterRight.TabIndex = 7;
            
            // 
            // splitterCenter
            // 
            this.splitterCenter.Dock = System.Windows.Forms.DockStyle.Right;
            this.splitterCenter.Location = new System.Drawing.Point(520, 68);
            this.splitterCenter.Name = "splitterCenter";
            this.splitterCenter.Size = new System.Drawing.Size(6, 754);
            this.splitterCenter.TabIndex = 8;
            
            // 
            // splitterDiagProblem
            // 
            this.splitterDiagProblem.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitterDiagProblem.Location = new System.Drawing.Point(0, 107);
            this.splitterDiagProblem.Name = "splitterDiagProblem";
            this.splitterDiagProblem.Size = new System.Drawing.Size(536, 6);
            this.splitterDiagProblem.TabIndex = 9;
            
            // 
            // OCSINPOrderMainControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnlMain);
            this.Controls.Add(this.pnlPatientInfo);
            this.Controls.Add(this.pnlWaitList);
            this.Controls.Add(this.splitterLeft);
            this.Controls.Add(this.pnlCenter);
            this.Controls.Add(this.splitterRight);
            this.Controls.Add(this.pnlRight);
            this.Controls.Add(this.splitterCenter);
            this.Controls.Add(this.pnlTop);
            this.Controls.Add(this.pnlBottom);
            this.Name = "OCSINPOrderMainControl";
            this.Size = new System.Drawing.Size(1280, 865);
            ((System.ComponentModel.ISupportInitialize)(this.pnlMain)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnlPatientInfo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnlWaitList)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnlCenter)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnlRight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnlBottom)).EndInit();
            this.pnlTop.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pnlTop)).EndInit();
            this.pnlTopBar.ResumeLayout(false);
            this.pnlTopBar.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pnlTopBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnlDiagProblem)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnlMemo)).EndInit();
            this.tabSupport.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.tabSupport)).EndInit();
            this.OrderPage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cboOrderType.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtpOrdDate.Properties)).EndInit();
            this.ResumeLayout(false);
            
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
        private nU3.Modules.OCS.IN.OrderEntry.Controls.PatientInfoControl PatientInfoControl;
        private nU3.Modules.OCS.IN.OrderEntry.Controls.PatientListControl PatientListControl;
        private nU3.Modules.OCS.IN.OrderEntry.Controls.OrderCodeControl OrderCodeControl;
        private nU3.Modules.OCS.IN.OrderEntry.Controls.DiagCodeControl DiagCodeControl;
        private nU3.Modules.OCS.IN.OrderEntry.Controls.ProblemListControl ProblemListControl;
        private nU3.Modules.OCS.IN.OrderEntry.Controls.OtherOrderControl OtherOrderControl;
        private nU3.Modules.OCS.IN.OrderEntry.Controls.SendMemoControl SendMemoControl;
        private nU3.Modules.OCS.IN.OrderEntry.Controls.OtherTabControl OtherTabControl;
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