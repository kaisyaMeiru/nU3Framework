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
            this.grpProblemList = new nU3.Core.UI.Controls.nU3GroupControl();
            this.pnlButtons = new nU3.Core.UI.Controls.nU3PanelControl();
            this.btnDeleteProblem = new nU3.Core.UI.Controls.nU3SimpleButton();
            this.btnAddProblem = new nU3.Core.UI.Controls.nU3SimpleButton();
            this.gridControl = new nU3.Core.UI.Controls.nU3GridControl();
            this.gridView = new nU3.Core.UI.Controls.nU3GridView();
            this.colProblemCode = new nU3.Core.UI.Controls.nU3GridColumn();
            this.colProblemName = new nU3.Core.UI.Controls.nU3GridColumn();
            this.colProblemType = new nU3.Core.UI.Controls.nU3GridColumn();
            ((System.ComponentModel.ISupportInitialize)(this.grpProblemList)).BeginInit();
            this.grpProblemList.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pnlButtons)).BeginInit();
            this.pnlButtons.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView)).BeginInit();
            this.SuspendLayout();
            // 
            // grpProblemList
            // 
            this.grpProblemList.Controls.Add(this.pnlButtons);
            this.grpProblemList.Controls.Add(this.gridControl);
            this.grpProblemList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpProblemList.Location = new System.Drawing.Point(0, 0);
            this.grpProblemList.Name = "grpProblemList";
            this.grpProblemList.Size = new System.Drawing.Size(305, 107);
            this.grpProblemList.TabIndex = 0;
            this.grpProblemList.Text = "문제리스트";
            // 
            // pnlButtons
            // 
            this.pnlButtons.Controls.Add(this.btnDeleteProblem);
            this.pnlButtons.Controls.Add(this.btnAddProblem);
            this.pnlButtons.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlButtons.Location = new System.Drawing.Point(2, 22);
            this.pnlButtons.Name = "pnlButtons";
            this.pnlButtons.Size = new System.Drawing.Size(301, 30);
            this.pnlButtons.TabIndex = 1;
            // 
            // btnDeleteProblem
            // 
            this.btnDeleteProblem.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDeleteProblem.Location = new System.Drawing.Point(219, 3);
            this.btnDeleteProblem.Name = "btnDeleteProblem";
            this.btnDeleteProblem.Size = new System.Drawing.Size(75, 23);
            this.btnDeleteProblem.TabIndex = 1;
            this.btnDeleteProblem.Text = "삭제";
            this.btnDeleteProblem.Click += new System.EventHandler(this.btnDeleteProblem_Click);
            // 
            // btnAddProblem
            // 
            this.btnAddProblem.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddProblem.Location = new System.Drawing.Point(138, 3);
            this.btnAddProblem.Name = "btnAddProblem";
            this.btnAddProblem.Size = new System.Drawing.Size(75, 23);
            this.btnAddProblem.TabIndex = 0;
            this.btnAddProblem.Text = "추가";
            this.btnAddProblem.Click += new System.EventHandler(this.btnAddProblem_Click);
            // 
            // gridControl
            // 
            this.gridControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridControl.Location = new System.Drawing.Point(2, 52);
            this.gridControl.MainView = this.gridView;
            this.gridControl.Name = "gridControl";
            this.gridControl.Size = new System.Drawing.Size(301, 53);
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
            this.gridView.Columns.AddRange(new nU3.Core.UI.Controls.nU3GridColumn[] {
            this.colProblemCode,
            this.colProblemName,
            this.colProblemType});
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
            this.gridView.DoubleClick += new System.EventHandler(this.gridView_DoubleClick);
            // 
            // colProblemCode
            // 
            this.colProblemCode.Caption = "문제코드";
            this.colProblemCode.FieldName = "ProblemCode";
            this.colProblemCode.Name = "colProblemCode";
            this.colProblemCode.Visible = true;
            this.colProblemCode.VisibleIndex = 0;
            this.colProblemCode.Width = 80;
            // 
            // colProblemName
            // 
            this.colProblemName.Caption = "문제명";
            this.colProblemName.FieldName = "ProblemName";
            this.colProblemName.Name = "colProblemName";
            this.colProblemName.Visible = true;
            this.colProblemName.VisibleIndex = 1;
            this.colProblemName.Width = 150;
            // 
            // colProblemType
            // 
            this.colProblemType.Caption = "구분";
            this.colProblemType.FieldName = "ProblemType";
            this.colProblemType.Name = "colProblemType";
            this.colProblemType.Visible = true;
            this.colProblemType.VisibleIndex = 2;
            this.colProblemType.Width = 60;
            // 
            // ProblemListControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.grpProblemList);
            this.Name = "ProblemListControl";
            this.Size = new System.Drawing.Size(305, 107);
            ((System.ComponentModel.ISupportInitialize)(this.grpProblemList)).EndInit();
            this.grpProblemList.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pnlButtons)).EndInit();
            this.pnlButtons.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControl)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.GroupControl grpProblemList;
        private DevExpress.XtraEditors.PanelControl pnlButtons;
        private DevExpress.XtraEditors.SimpleButton btnDeleteProblem;
        private DevExpress.XtraEditors.SimpleButton btnAddProblem;
        private nU3.Core.UI.Controls.nU3GridControl gridControl;
        private nU3.Core.UI.Controls.nU3GridView gridView;
        private nU3.Core.UI.Controls.nU3GridColumn colProblemCode;
        private nU3.Core.UI.Controls.nU3GridColumn colProblemName;
        private nU3.Core.UI.Controls.nU3GridColumn colProblemType;
    }
}