namespace nU3.Modules.OCS.IN.MainEntry.Controls
{
    partial class ProblemListControl
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
            grpProblemList = new nU3.Core.UI.Controls.nU3GroupControl();
            pnlButtons = new nU3.Core.UI.Controls.nU3PanelControl();
            btnDeleteProblem = new nU3.Core.UI.Controls.nU3SimpleButton();
            btnAddProblem = new nU3.Core.UI.Controls.nU3SimpleButton();
            gridControl = new nU3.Core.UI.Controls.nU3GridControl();
            gridView = new nU3.Core.UI.Controls.nU3GridView();
            colProblemCode = new nU3.Core.UI.Controls.nU3GridColumn();
            colProblemName = new nU3.Core.UI.Controls.nU3GridColumn();
            colProblemType = new nU3.Core.UI.Controls.nU3GridColumn();
            ((System.ComponentModel.ISupportInitialize)grpProblemList).BeginInit();
            grpProblemList.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pnlButtons).BeginInit();
            pnlButtons.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)gridControl).BeginInit();
            ((System.ComponentModel.ISupportInitialize)gridView).BeginInit();
            SuspendLayout();
            // 
            // grpProblemList
            // 
            grpProblemList.Controls.Add(gridControl);
            grpProblemList.Controls.Add(pnlButtons);
            grpProblemList.Dock = DockStyle.Fill;
            grpProblemList.Location = new Point(0, 0);
            grpProblemList.Margin = new Padding(4, 4, 4, 4);
            grpProblemList.Name = "grpProblemList";
            grpProblemList.Size = new Size(501, 194);
            grpProblemList.TabIndex = 0;
            grpProblemList.Text = "문제리스트";
            // 
            // pnlButtons
            // 
            pnlButtons.Controls.Add(btnDeleteProblem);
            pnlButtons.Controls.Add(btnAddProblem);
            pnlButtons.Dock = DockStyle.Top;
            pnlButtons.Location = new Point(2, 23);
            pnlButtons.Margin = new Padding(4, 4, 4, 4);
            pnlButtons.Name = "pnlButtons";
            pnlButtons.Size = new Size(497, 38);
            pnlButtons.TabIndex = 1;
            // 
            // btnDeleteProblem
            // 
            btnDeleteProblem.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnDeleteProblem.AuthId = "";
            btnDeleteProblem.Location = new Point(401, 4);
            btnDeleteProblem.Margin = new Padding(4, 4, 4, 4);
            btnDeleteProblem.Name = "btnDeleteProblem";
            btnDeleteProblem.Size = new Size(88, 29);
            btnDeleteProblem.TabIndex = 1;
            btnDeleteProblem.Text = "삭제";
            btnDeleteProblem.Click += btnDeleteProblem_Click;
            // 
            // btnAddProblem
            // 
            btnAddProblem.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnAddProblem.AuthId = "";
            btnAddProblem.Location = new Point(307, 4);
            btnAddProblem.Margin = new Padding(4, 4, 4, 4);
            btnAddProblem.Name = "btnAddProblem";
            btnAddProblem.Size = new Size(88, 29);
            btnAddProblem.TabIndex = 0;
            btnAddProblem.Text = "추가";
            btnAddProblem.Click += btnAddProblem_Click;
            // 
            // gridControl
            // 
            gridControl.Dock = DockStyle.Fill;
            gridControl.EmbeddedNavigator.Margin = new Padding(4, 4, 4, 4);
            gridControl.Location = new Point(2, 61);
            gridControl.MainView = gridView;
            gridControl.Margin = new Padding(4, 4, 4, 4);
            gridControl.Name = "gridControl";
            gridControl.Size = new Size(497, 131);
            gridControl.TabIndex = 0;
            gridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { gridView });
            // 
            // gridView
            // 
            gridView.Appearance.HeaderPanel.Options.UseTextOptions = true;
            gridView.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            gridView.Appearance.Row.Options.UseTextOptions = true;
            gridView.Appearance.Row.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            gridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] { colProblemCode, colProblemName, colProblemType });
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
            gridView.DoubleClick += gridView_DoubleClick;
            // 
            // colProblemCode
            // 
            colProblemCode.AuthId = "";
            colProblemCode.Caption = "문제코드";
            colProblemCode.FieldName = "ProblemCode";
            colProblemCode.MinWidth = 23;
            colProblemCode.Name = "colProblemCode";
            colProblemCode.ResourceKey = "";
            colProblemCode.Visible = true;
            colProblemCode.VisibleIndex = 0;
            colProblemCode.Width = 93;
            // 
            // colProblemName
            // 
            colProblemName.AuthId = "";
            colProblemName.Caption = "문제명";
            colProblemName.FieldName = "ProblemName";
            colProblemName.MinWidth = 23;
            colProblemName.Name = "colProblemName";
            colProblemName.ResourceKey = "";
            colProblemName.Visible = true;
            colProblemName.VisibleIndex = 1;
            colProblemName.Width = 175;
            // 
            // colProblemType
            // 
            colProblemType.AuthId = "";
            colProblemType.Caption = "구분";
            colProblemType.FieldName = "ProblemType";
            colProblemType.MinWidth = 23;
            colProblemType.Name = "colProblemType";
            colProblemType.ResourceKey = "";
            colProblemType.Visible = true;
            colProblemType.VisibleIndex = 2;
            colProblemType.Width = 70;
            // 
            // ProblemListControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(grpProblemList);
            Margin = new Padding(4, 4, 4, 4);
            Name = "ProblemListControl";
            Size = new Size(501, 194);
            ((System.ComponentModel.ISupportInitialize)grpProblemList).EndInit();
            grpProblemList.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pnlButtons).EndInit();
            pnlButtons.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)gridControl).EndInit();
            ((System.ComponentModel.ISupportInitialize)gridView).EndInit();
            ResumeLayout(false);

        }

        #endregion
        private nU3.Core.UI.Controls.nU3GridControl gridControl;
        private nU3.Core.UI.Controls.nU3GridView gridView;
        private nU3.Core.UI.Controls.nU3GridColumn colProblemCode;
        private nU3.Core.UI.Controls.nU3GridColumn colProblemName;
        private nU3.Core.UI.Controls.nU3GridColumn colProblemType;
        private Core.UI.Controls.nU3GroupControl grpProblemList;
        private Core.UI.Controls.nU3PanelControl pnlButtons;
        private Core.UI.Controls.nU3SimpleButton btnDeleteProblem;
        private Core.UI.Controls.nU3SimpleButton btnAddProblem;
    }
}