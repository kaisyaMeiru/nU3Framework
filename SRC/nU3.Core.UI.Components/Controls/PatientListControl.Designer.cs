using System;
using System.ComponentModel;
using System.Windows.Forms;
using nU3.Core.UI.Controls;
using DevExpress.XtraGrid.Columns;

namespace nU3.Core.UI.Components.Controls
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
            grpPatientList = new nU3GroupControl();
            gridControl = new nU3GridControl();
            gridView = new nU3GridView();
            colInNumber = new nU3GridColumn();
            colPatiNumber = new nU3GridColumn();
            colPatiName = new nU3GridColumn();
            colGender = new nU3GridColumn();
            colAge = new nU3GridColumn();
            colDeptName = new nU3GridColumn();
            colDoctorName = new nU3GridColumn();
            colRoomNo = new nU3GridColumn();
            colAdmDate = new nU3GridColumn();
            pnlButtons = new nU3PanelControl();
            btnRefresh = new nU3SimpleButton();
            ((ISupportInitialize)grpPatientList).BeginInit();
            grpPatientList.SuspendLayout();
            ((ISupportInitialize)gridControl).BeginInit();
            ((ISupportInitialize)gridView).BeginInit();
            ((ISupportInitialize)pnlButtons).BeginInit();
            pnlButtons.SuspendLayout();
            SuspendLayout();
            // 
            // grpPatientList
            // 
            grpPatientList.Controls.Add(gridControl);
            grpPatientList.Controls.Add(pnlButtons);
            grpPatientList.Dock = DockStyle.Fill;
            grpPatientList.Location = new Point(0, 0);
            grpPatientList.Margin = new Padding(4);
            grpPatientList.Name = "grpPatientList";
            grpPatientList.Size = new Size(1305, 852);
            grpPatientList.TabIndex = 0;
            grpPatientList.Text = "입원환자 리스트";
            // 
            // gridControl
            // 
            gridControl.Dock = DockStyle.Fill;
            gridControl.Location = new Point(2, 61);
            gridControl.MainView = gridView;
            gridControl.Margin = new Padding(4);
            gridControl.Name = "gridControl";
            gridControl.Size = new Size(1301, 789);
            gridControl.TabIndex = 0;
            gridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { gridView });
            // 
            // gridView
            // 
            gridView.Columns.AddRange(new GridColumn[] { colInNumber, colPatiNumber, colPatiName, colGender, colAge, colDeptName, colDoctorName, colRoomNo, colAdmDate });
            gridView.DetailHeight = 437;
            gridView.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            gridView.GridControl = gridControl;
            gridView.IndicatorWidth = 47;
            gridView.Name = "gridView";
            gridView.OptionsBehavior.AutoExpandAllGroups = true;
            gridView.OptionsBehavior.Editable = false;
            gridView.OptionsCustomization.AllowColumnMoving = false;
            gridView.OptionsCustomization.AllowFilter = false;
            gridView.OptionsEditForm.PopupEditFormWidth = 933;
            gridView.OptionsNavigation.EnterMoveNextColumn = true;
            gridView.OptionsSelection.EnableAppearanceFocusedCell = false;
            gridView.OptionsView.EnableAppearanceEvenRow = true;
            gridView.OptionsView.EnableAppearanceOddRow = true;
            gridView.OptionsView.ShowAutoFilterRow = true;
            gridView.OptionsView.ShowGroupPanel = false;
            gridView.FocusedRowChanged += gridView_FocusedRowChanged;
            gridView.DoubleClick += gridView_DoubleClick;
            gridView.CustomColumnDisplayText += gridView_CustomColumnDisplayText;
            // 
            // colInNumber
            // 
            colInNumber.AuthId = "";
            colInNumber.Name = "colInNumber";
            colInNumber.ResourceKey = "";
            // 
            // colPatiNumber
            // 
            colPatiNumber.AuthId = "";
            colPatiNumber.Caption = "환자번호";
            colPatiNumber.FieldName = "PatientId";
            colPatiNumber.MinWidth = 23;
            colPatiNumber.Name = "colPatiNumber";
            colPatiNumber.ResourceKey = "";
            colPatiNumber.Visible = true;
            colPatiNumber.VisibleIndex = 0;
            colPatiNumber.Width = 93;
            // 
            // colPatiName
            // 
            colPatiName.AuthId = "";
            colPatiName.Caption = "환자명";
            colPatiName.FieldName = "PatientName";
            colPatiName.MinWidth = 23;
            colPatiName.Name = "colPatiName";
            colPatiName.ResourceKey = "";
            colPatiName.Visible = true;
            colPatiName.VisibleIndex = 1;
            colPatiName.Width = 70;
            // 
            // colGender
            // 
            colGender.AuthId = "";
            colGender.Caption = "성별";
            colGender.FieldName = "Gender";
            colGender.MinWidth = 23;
            colGender.Name = "colGender";
            colGender.ResourceKey = "";
            colGender.Visible = true;
            colGender.VisibleIndex = 2;
            colGender.Width = 47;
            // 
            // colAge
            // 
            colAge.AuthId = "";
            colAge.Caption = "나이";
            colAge.FieldName = "Age";
            colAge.MinWidth = 23;
            colAge.Name = "colAge";
            colAge.ResourceKey = "";
            colAge.Visible = true;
            colAge.VisibleIndex = 3;
            colAge.Width = 58;
            // 
            // colDeptName
            // 
            colDeptName.AuthId = "";
            colDeptName.Caption = "진료과";
            colDeptName.FieldName = "DeptName";
            colDeptName.MinWidth = 23;
            colDeptName.Name = "colDeptName";
            colDeptName.ResourceKey = "";
            colDeptName.Visible = true;
            colDeptName.VisibleIndex = 4;
            colDeptName.Width = 82;
            // 
            // colDoctorName
            // 
            colDoctorName.AuthId = "";
            colDoctorName.Caption = "담당의";
            colDoctorName.FieldName = "DoctorName";
            colDoctorName.MinWidth = 23;
            colDoctorName.Name = "colDoctorName";
            colDoctorName.ResourceKey = "";
            colDoctorName.Visible = true;
            colDoctorName.VisibleIndex = 5;
            colDoctorName.Width = 82;
            // 
            // colRoomNo
            // 
            colRoomNo.AuthId = "";
            colRoomNo.Caption = "병실";
            colRoomNo.FieldName = "RoomNo";
            colRoomNo.MinWidth = 23;
            colRoomNo.Name = "colRoomNo";
            colRoomNo.ResourceKey = "";
            colRoomNo.Visible = true;
            colRoomNo.VisibleIndex = 6;
            colRoomNo.Width = 58;
            // 
            // colAdmDate
            // 
            colAdmDate.AuthId = "";
            colAdmDate.Caption = "입원일";
            colAdmDate.DisplayFormat.FormatString = "yyyy-MM-dd";
            colAdmDate.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            colAdmDate.FieldName = "AdmDate";
            colAdmDate.MinWidth = 23;
            colAdmDate.Name = "colAdmDate";
            colAdmDate.ResourceKey = "";
            colAdmDate.Visible = true;
            colAdmDate.VisibleIndex = 7;
            colAdmDate.Width = 93;
            // 
            // pnlButtons
            // 
            pnlButtons.Controls.Add(btnRefresh);
            pnlButtons.Dock = DockStyle.Top;
            pnlButtons.Location = new Point(2, 23);
            pnlButtons.Margin = new Padding(4);
            pnlButtons.Name = "pnlButtons";
            pnlButtons.Size = new Size(1301, 38);
            pnlButtons.TabIndex = 1;
            // 
            // btnRefresh
            // 
            btnRefresh.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnRefresh.AuthId = "";
            btnRefresh.Location = new Point(1205, 4);
            btnRefresh.Margin = new Padding(4);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new Size(88, 29);
            btnRefresh.TabIndex = 0;
            btnRefresh.Text = "새로고침";
            btnRefresh.Click += btnRefresh_Click;
            // 
            // PatientListControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(grpPatientList);
            Margin = new Padding(4);
            Name = "PatientListControl";
            Size = new Size(1305, 852);
            ((ISupportInitialize)grpPatientList).EndInit();
            grpPatientList.ResumeLayout(false);
            ((ISupportInitialize)gridControl).EndInit();
            ((ISupportInitialize)gridView).EndInit();
            ((ISupportInitialize)pnlButtons).EndInit();
            pnlButtons.ResumeLayout(false);
            ResumeLayout(false);

        }

        #endregion

        private nU3GroupControl grpPatientList;
        private nU3PanelControl pnlButtons;
        private nU3SimpleButton btnRefresh;
        private nU3GridControl gridControl;
        private nU3GridView gridView;
        private nU3GridColumn colInNumber;
        private nU3GridColumn colPatiNumber;
        private nU3GridColumn colPatiName;
        private nU3GridColumn colGender;
        private nU3GridColumn colAge;
        private nU3GridColumn colDeptName;
        private nU3GridColumn colDoctorName;
        private nU3GridColumn colRoomNo;
        private nU3GridColumn colAdmDate;
    }
}
