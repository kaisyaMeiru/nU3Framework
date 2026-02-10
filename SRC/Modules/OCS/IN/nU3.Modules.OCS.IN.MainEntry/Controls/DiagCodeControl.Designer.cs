using System;
using System.ComponentModel;
using System.Windows.Forms;
using nU3.Core.UI.Controls;
using DevExpress.XtraGrid.Columns;

namespace nU3.Modules.OCS.IN.MainEntry.Controls
{
    partial class DiagCodeControl
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
            grpDiagCode = new nU3GroupControl();
            gridControl = new nU3GridControl();
            gridView = new nU3GridView();
            colDiagCode = new nU3GridColumn();
            colDiagName = new nU3GridColumn();
            colDiagType = new nU3GridColumn();
            colMainYn = new nU3GridColumn();
            pnlButtons = new nU3PanelControl();
            btnDeleteDiag = new nU3SimpleButton();
            btnAddDiag = new nU3SimpleButton();
            ((ISupportInitialize)grpDiagCode).BeginInit();
            grpDiagCode.SuspendLayout();
            ((ISupportInitialize)gridControl).BeginInit();
            ((ISupportInitialize)gridView).BeginInit();
            ((ISupportInitialize)pnlButtons).BeginInit();
            pnlButtons.SuspendLayout();
            SuspendLayout();
            // 
            // grpDiagCode
            // 
            grpDiagCode.Controls.Add(gridControl);
            grpDiagCode.Controls.Add(pnlButtons);
            grpDiagCode.Dock = DockStyle.Fill;
            grpDiagCode.Location = new Point(0, 0);
            grpDiagCode.Margin = new Padding(4);
            grpDiagCode.Name = "grpDiagCode";
            grpDiagCode.Size = new Size(1305, 852);
            grpDiagCode.TabIndex = 0;
            grpDiagCode.Text = "진단코드";
            // 
            // gridControl
            // 
            gridControl.Dock = DockStyle.Fill;
            gridControl.EmbeddedNavigator.Margin = new Padding(4);
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
            gridView.Columns.AddRange(new GridColumn[] { colDiagCode, colDiagName, colDiagType, colMainYn });
            gridView.DetailHeight = 437;
            gridView.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            gridView.GridControl = gridControl;
            gridView.IndicatorWidth = 47;
            gridView.Name = "gridView";
            gridView.OptionsBehavior.AutoExpandAllGroups = true;
            gridView.OptionsCustomization.AllowColumnMoving = false;
            gridView.OptionsCustomization.AllowFilter = false;
            gridView.OptionsEditForm.PopupEditFormWidth = 933;
            gridView.OptionsNavigation.EnterMoveNextColumn = true;
            gridView.OptionsSelection.EnableAppearanceFocusedCell = false;
            gridView.OptionsView.EnableAppearanceEvenRow = true;
            gridView.OptionsView.EnableAppearanceOddRow = true;
            gridView.OptionsView.ShowAutoFilterRow = true;
            gridView.OptionsView.ShowGroupPanel = false;
            gridView.CellValueChanged += gridView_CellValueChanged;
            gridView.DoubleClick += gridView_DoubleClick;
            // 
            // colDiagCode
            // 
            colDiagCode.AuthId = "";
            colDiagCode.Caption = "진단코드";
            colDiagCode.FieldName = "DiagCode";
            colDiagCode.MinWidth = 23;
            colDiagCode.Name = "colDiagCode";
            colDiagCode.ResourceKey = "";
            colDiagCode.Visible = true;
            colDiagCode.VisibleIndex = 0;
            colDiagCode.Width = 93;
            // 
            // colDiagName
            // 
            colDiagName.AuthId = "";
            colDiagName.Caption = "진단명";
            colDiagName.FieldName = "DiagName";
            colDiagName.MinWidth = 23;
            colDiagName.Name = "colDiagName";
            colDiagName.ResourceKey = "";
            colDiagName.Visible = true;
            colDiagName.VisibleIndex = 1;
            colDiagName.Width = 117;
            // 
            // colDiagType
            // 
            colDiagType.AuthId = "";
            colDiagType.Caption = "구분";
            colDiagType.FieldName = "DiagType";
            colDiagType.MinWidth = 23;
            colDiagType.Name = "colDiagType";
            colDiagType.ResourceKey = "";
            colDiagType.Visible = true;
            colDiagType.VisibleIndex = 2;
            colDiagType.Width = 58;
            // 
            // colMainYn
            // 
            colMainYn.AuthId = "";
            colMainYn.Caption = "주";
            colMainYn.FieldName = "MainYn";
            colMainYn.MinWidth = 23;
            colMainYn.Name = "colMainYn";
            colMainYn.ResourceKey = "";
            colMainYn.Visible = true;
            colMainYn.VisibleIndex = 3;
            colMainYn.Width = 47;
            // 
            // pnlButtons
            // 
            pnlButtons.Controls.Add(btnDeleteDiag);
            pnlButtons.Controls.Add(btnAddDiag);
            pnlButtons.Dock = DockStyle.Top;
            pnlButtons.Location = new Point(2, 23);
            pnlButtons.Margin = new Padding(4);
            pnlButtons.Name = "pnlButtons";
            pnlButtons.Size = new Size(1301, 38);
            pnlButtons.TabIndex = 1;
            // 
            // btnDeleteDiag
            // 
            btnDeleteDiag.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnDeleteDiag.AuthId = "";
            btnDeleteDiag.Location = new Point(1205, 4);
            btnDeleteDiag.Margin = new Padding(4);
            btnDeleteDiag.Name = "btnDeleteDiag";
            btnDeleteDiag.Size = new Size(88, 29);
            btnDeleteDiag.TabIndex = 1;
            btnDeleteDiag.Text = "삭제";
            btnDeleteDiag.Click += btnDeleteDiag_Click;
            // 
            // btnAddDiag
            // 
            btnAddDiag.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnAddDiag.AuthId = "";
            btnAddDiag.Location = new Point(1111, 4);
            btnAddDiag.Margin = new Padding(4);
            btnAddDiag.Name = "btnAddDiag";
            btnAddDiag.Size = new Size(88, 29);
            btnAddDiag.TabIndex = 0;
            btnAddDiag.Text = "추가";
            btnAddDiag.Click += btnAddDiag_Click;
            // 
            // DiagCodeControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(grpDiagCode);
            Margin = new Padding(4);
            Name = "DiagCodeControl";
            Size = new Size(1305, 852);
            ((ISupportInitialize)grpDiagCode).EndInit();
            grpDiagCode.ResumeLayout(false);
            ((ISupportInitialize)gridControl).EndInit();
            ((ISupportInitialize)gridView).EndInit();
            ((ISupportInitialize)pnlButtons).EndInit();
            pnlButtons.ResumeLayout(false);
            ResumeLayout(false);

        }

        #endregion

        private nU3GroupControl grpDiagCode;
        private nU3PanelControl pnlButtons;
        private nU3SimpleButton btnDeleteDiag;
        private nU3SimpleButton btnAddDiag;
        private nU3GridControl gridControl;
        private nU3GridView gridView;
        private nU3GridColumn colDiagCode;
        private nU3GridColumn colDiagName;
        private nU3GridColumn colDiagType;
        private nU3GridColumn colMainYn;
    }
}