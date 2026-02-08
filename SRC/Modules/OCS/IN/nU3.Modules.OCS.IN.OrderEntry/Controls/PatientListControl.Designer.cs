namespace nU3.Modules.OCS.IN.OrderEntry.Controls
{
    partial class PatientListControl
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
            this.grpPatientList = new DevExpress.XtraEditors.GroupControl();
            this.pnlButtons = new DevExpress.XtraEditors.PanelControl();
            this.btnRefresh = new DevExpress.XtraEditors.SimpleButton();
            this.gridControl = new DevExpress.XtraGrid.GridControl();
            this.gridView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colInNumber = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colPatiNumber = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colPatiName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colGender = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colAge = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colDeptName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colDoctorName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colRoomNo = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colAdmDate = new DevExpress.XtraGrid.Columns.GridColumn();
            ((System.ComponentModel.ISupportInitialize)(this.grpPatientList)).BeginInit();
            this.grpPatientList.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pnlButtons)).BeginInit();
            this.pnlButtons.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView)).BeginInit();
            this.SuspendLayout();
            // 
            // grpPatientList
            // 
            this.grpPatientList.Controls.Add(this.pnlButtons);
            this.grpPatientList.Controls.Add(this.gridControl);
            this.grpPatientList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpPatientList.Location = new System.Drawing.Point(0, 0);
            this.grpPatientList.Name = "grpPatientList";
            this.grpPatientList.Size = new System.Drawing.Size(275, 754);
            this.grpPatientList.TabIndex = 0;
            this.grpPatientList.Text = "입원환자 리스트";
            // 
            // pnlButtons
            // 
            this.pnlButtons.Controls.Add(this.btnRefresh);
            this.pnlButtons.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlButtons.Location = new System.Drawing.Point(2, 22);
            this.pnlButtons.Name = "pnlButtons";
            this.pnlButtons.Size = new System.Drawing.Size(271, 30);
            this.pnlButtons.TabIndex = 1;
            // 
            // btnRefresh
            // 
            this.btnRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            // this.btnRefresh.Image = global::nU3.Modules.OCS.IN.OrderEntry.Properties.Resources.refresh;
            this.btnRefresh.Location = new System.Drawing.Point(189, 3);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(75, 23);
            this.btnRefresh.TabIndex = 0;
            this.btnRefresh.Text = "새로고침";
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // gridControl
            // 
            this.gridControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridControl.Location = new System.Drawing.Point(2, 52);
            this.gridControl.MainView = this.gridView;
            this.gridControl.Name = "gridControl";
            this.gridControl.Size = new System.Drawing.Size(271, 700);
            this.gridControl.TabIndex = 0;
            this.gridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView});
            // 
            // gridView
            // 
            this.gridView.Appearance.HeaderPanel.Options.UseTextOptions = true;
            this.gridView.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridView.Appearance.Row.Options.UseTextOptions = true;
            this.gridView.Appearance.Row.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            this.gridView.Appearance.ViewCaption.Options.UseTextOptions = true;
            this.gridView.Appearance.ViewCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colInNumber,
            this.colPatiNumber,
            this.colPatiName,
            this.colGender,
            this.colAge,
            this.colDeptName,
            this.colDoctorName,
            this.colRoomNo,
            this.colAdmDate});
            this.gridView.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridView.GridControl = this.gridControl;
            this.gridView.IndicatorWidth = 40;
            this.gridView.Name = "gridView";
            this.gridView.OptionsBehavior.AutoExpandAllGroups = true;
            this.gridView.OptionsBehavior.Editable = false;
            this.gridView.OptionsCustomization.AllowColumnMoving = false;
            this.gridView.OptionsCustomization.AllowFilter = false;
            this.gridView.OptionsNavigation.EnterMoveNextColumn = true;
            this.gridView.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridView.OptionsView.EnableAppearanceEvenRow = true;
            this.gridView.OptionsView.EnableAppearanceOddRow = true;
            this.gridView.OptionsView.ShowAutoFilterRow = true;
            this.gridView.OptionsView.ShowGroupPanel = false;
            this.gridView.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.gridView_FocusedRowChanged);
            this.gridView.DoubleClick += new System.EventHandler(this.gridView_DoubleClick);
            // 
            // colInNumber
            // 
            this.colInNumber.Caption = "입원번호";
            this.colInNumber.FieldName = "InNumber";
            this.colInNumber.Name = "colInNumber";
            this.colInNumber.Visible = true;
            this.colInNumber.VisibleIndex = 0;
            this.colInNumber.Width = 80;
            // 
            // colPatiNumber
            // 
            this.colPatiNumber.Caption = "환자번호";
            this.colPatiNumber.FieldName = "PatiNumber";
            this.colPatiNumber.Name = "colPatiNumber";
            this.colPatiNumber.Visible = true;
            this.colPatiNumber.VisibleIndex = 1;
            this.colPatiNumber.Width = 80;
            // 
            // colPatiName
            // 
            this.colPatiName.Caption = "환자명";
            this.colPatiName.FieldName = "PatiName";
            this.colPatiName.Name = "colPatiName";
            this.colPatiName.Visible = true;
            this.colPatiName.VisibleIndex = 2;
            this.colPatiName.Width = 60;
            // 
            // colGender
            // 
            this.colGender.Caption = "성별";
            this.colGender.FieldName = "Gender";
            this.colGender.Name = "colGender";
            this.colGender.Visible = true;
            this.colGender.VisibleIndex = 3;
            this.colGender.Width = 40;
            // 
            // colAge
            // 
            this.colAge.Caption = "나이";
            this.colAge.FieldName = "Age";
            this.colAge.Name = "colAge";
            this.colAge.Visible = true;
            this.colAge.VisibleIndex = 4;
            this.colAge.Width = 50;
            // 
            // colDeptName
            // 
            this.colDeptName.Caption = "진료과";
            this.colDeptName.FieldName = "DeptName";
            this.colDeptName.Name = "colDeptName";
            this.colDeptName.Visible = true;
            this.colDeptName.VisibleIndex = 5;
            this.colDeptName.Width = 70;
            // 
            // colDoctorName
            // 
            this.colDoctorName.Caption = "담당의";
            this.colDoctorName.FieldName = "DoctorName";
            this.colDoctorName.Name = "colDoctorName";
            this.colDoctorName.Visible = true;
            this.colDoctorName.VisibleIndex = 6;
            this.colDoctorName.Width = 70;
            // 
            // colRoomNo
            // 
            this.colRoomNo.Caption = "병실";
            this.colRoomNo.FieldName = "RoomNo";
            this.colRoomNo.Name = "colRoomNo";
            this.colRoomNo.Visible = true;
            this.colRoomNo.VisibleIndex = 7;
            this.colRoomNo.Width = 50;
            // 
            // colAdmDate
            // 
            this.colAdmDate.Caption = "입원일";
            this.colAdmDate.DisplayFormat.FormatString = "yyyy-MM-dd";
            this.colAdmDate.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.colAdmDate.FieldName = "AdmDate";
            this.colAdmDate.Name = "colAdmDate";
            this.colAdmDate.Visible = true;
            this.colAdmDate.VisibleIndex = 8;
            this.colAdmDate.Width = 80;
            // 
            // PatientListControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.grpPatientList);
            this.Name = "PatientListControl";
            this.Size = new System.Drawing.Size(275, 754);
            ((System.ComponentModel.ISupportInitialize)(this.grpPatientList)).EndInit();
            this.grpPatientList.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pnlButtons)).EndInit();
            this.pnlButtons.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControl)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.GroupControl grpPatientList;
        private DevExpress.XtraEditors.PanelControl pnlButtons;
        private DevExpress.XtraEditors.SimpleButton btnRefresh;
        private DevExpress.XtraGrid.GridControl gridControl;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView;
        private DevExpress.XtraGrid.Columns.GridColumn colInNumber;
        private DevExpress.XtraGrid.Columns.GridColumn colPatiNumber;
        private DevExpress.XtraGrid.Columns.GridColumn colPatiName;
        private DevExpress.XtraGrid.Columns.GridColumn colGender;
        private DevExpress.XtraGrid.Columns.GridColumn colAge;
        private DevExpress.XtraGrid.Columns.GridColumn colDeptName;
        private DevExpress.XtraGrid.Columns.GridColumn colDoctorName;
        private DevExpress.XtraGrid.Columns.GridColumn colRoomNo;
        private DevExpress.XtraGrid.Columns.GridColumn colAdmDate;
    }
}