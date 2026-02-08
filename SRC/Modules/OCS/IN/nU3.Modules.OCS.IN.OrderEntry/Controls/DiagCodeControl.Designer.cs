namespace nU3.Modules.OCS.IN.OrderEntry.Controls
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
            this.grpDiagCode = new DevExpress.XtraEditors.GroupControl();
            this.pnlButtons = new DevExpress.XtraEditors.PanelControl();
            this.btnDeleteDiag = new DevExpress.XtraEditors.SimpleButton();
            this.btnAddDiag = new DevExpress.XtraEditors.SimpleButton();
            this.gridControl = new DevExpress.XtraGrid.GridControl();
            this.gridView = new DevExpress.XtraGrid.Views.Grid.GridView();
            ((System.ComponentModel.ISupportInitialize)(this.grpDiagCode)).BeginInit();
            this.grpDiagCode.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pnlButtons)).BeginInit();
            this.pnlButtons.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView)).BeginInit();
            this.SuspendLayout();
            // 
            // grpDiagCode
            // 
            this.grpDiagCode.Controls.Add(this.pnlButtons);
            this.grpDiagCode.Controls.Add(this.gridControl);
            this.grpDiagCode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpDiagCode.Location = new System.Drawing.Point(0, 0);
            this.grpDiagCode.Name = "grpDiagCode";
            this.grpDiagCode.Size = new System.Drawing.Size(225, 107);
            this.grpDiagCode.TabIndex = 0;
            this.grpDiagCode.Text = "진단코드";
            // 
            // pnlButtons
            // 
            this.pnlButtons.Controls.Add(this.btnDeleteDiag);
            this.pnlButtons.Controls.Add(this.btnAddDiag);
            this.pnlButtons.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlButtons.Location = new System.Drawing.Point(2, 22);
            this.pnlButtons.Name = "pnlButtons";
            this.pnlButtons.Size = new System.Drawing.Size(221, 30);
            this.pnlButtons.TabIndex = 1;
            // 
            // btnDeleteDiag
            // 
            this.btnDeleteDiag.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            // this.btnDeleteDiag.Image = global::nU3.Modules.OCS.IN.OrderEntry.Properties.Resources.delete;
            this.btnDeleteDiag.Location = new System.Drawing.Point(139, 3);
            this.btnDeleteDiag.Name = "btnDeleteDiag";
            this.btnDeleteDiag.Size = new System.Drawing.Size(75, 23);
            this.btnDeleteDiag.TabIndex = 1;
            this.btnDeleteDiag.Text = "삭제";
            this.btnDeleteDiag.Click += new System.EventHandler(this.btnDeleteDiag_Click);
            // 
            // btnAddDiag
            // 
            this.btnAddDiag.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            // this.btnAddDiag.Image = global::nU3.Modules.OCS.IN.OrderEntry.Properties.Resources.add;
            this.btnAddDiag.Location = new System.Drawing.Point(58, 3);
            this.btnAddDiag.Name = "btnAddDiag";
            this.btnAddDiag.Size = new System.Drawing.Size(75, 23);
            this.btnAddDiag.TabIndex = 0;
            this.btnAddDiag.Text = "추가";
            this.btnAddDiag.Click += new System.EventHandler(this.btnAddDiag_Click);
            // 
            // gridControl
            // 
            this.gridControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridControl.Location = new System.Drawing.Point(2, 52);
            this.gridControl.MainView = this.gridView;
            this.gridControl.Name = "gridControl";
            this.gridControl.Size = new System.Drawing.Size(221, 53);
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
            this.colDiagCode,
            this.colDiagName,
            this.colDiagType,
            this.colMainYn});
            this.gridView.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridView.GridControl = this.gridControl;
            this.gridView.IndicatorWidth = 40;
            this.gridView.Name = "gridView";
            this.gridView.OptionsBehavior.AutoExpandAllGroups = true;
            this.gridView.OptionsBehavior.Editable = true;
            this.gridView.OptionsCustomization.AllowColumnMoving = false;
            this.gridView.OptionsCustomization.AllowFilter = false;
            this.gridView.OptionsNavigation.EnterMoveNextColumn = true;
            this.gridView.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridView.OptionsView.EnableAppearanceEvenRow = true;
            this.gridView.OptionsView.EnableAppearanceOddRow = true;
            this.gridView.OptionsView.ShowAutoFilterRow = true;
            this.gridView.OptionsView.ShowGroupPanel = false;
            this.gridView.DoubleClick += new System.EventHandler(this.gridView_DoubleClick);
            this.gridView.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gridView_CellValueChanged);
            // 
            // colDiagCode
            // 
            this.colDiagCode.Caption = "진단코드";
            this.colDiagCode.FieldName = "DiagCode";
            this.colDiagCode.Name = "colDiagCode";
            this.colDiagCode.Visible = true;
            this.colDiagCode.VisibleIndex = 0;
            this.colDiagCode.Width = 80;
            // 
            // colDiagName
            // 
            this.colDiagName.Caption = "진단명";
            this.colDiagName.FieldName = "DiagName";
            this.colDiagName.Name = "colDiagName";
            this.colDiagName.Visible = true;
            this.colDiagName.VisibleIndex = 1;
            this.colDiagName.Width = 100;
            // 
            // colDiagType
            // 
            this.colDiagType.Caption = "구분";
            this.colDiagType.FieldName = "DiagType";
            this.colDiagType.Name = "colDiagType";
            this.colDiagType.Visible = true;
            this.colDiagType.VisibleIndex = 2;
            this.colDiagType.Width = 50;
            // 
            // colMainYn
            // 
            this.colMainYn.Caption = "주";
            this.colMainYn.FieldName = "MainYn";
            this.colMainYn.Name = "colMainYn";
            this.colMainYn.Visible = true;
            this.colMainYn.VisibleIndex = 3;
            this.colMainYn.Width = 40;
            // 
            // DiagCodeControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.grpDiagCode);
            this.Name = "DiagCodeControl";
            this.Size = new System.Drawing.Size(225, 107);
            ((System.ComponentModel.ISupportInitialize)(this.grpDiagCode)).EndInit();
            this.grpDiagCode.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pnlButtons)).EndInit();
            this.pnlButtons.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControl)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.GroupControl grpDiagCode;
        private DevExpress.XtraEditors.PanelControl pnlButtons;
        private DevExpress.XtraEditors.SimpleButton btnDeleteDiag;
        private DevExpress.XtraEditors.SimpleButton btnAddDiag;
        private DevExpress.XtraGrid.GridControl gridControl;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView;
        private DevExpress.XtraGrid.Columns.GridColumn colDiagCode;
        private DevExpress.XtraGrid.Columns.GridColumn colDiagName;
        private DevExpress.XtraGrid.Columns.GridColumn colDiagType;
        private DevExpress.XtraGrid.Columns.GridColumn colMainYn;
    }
}