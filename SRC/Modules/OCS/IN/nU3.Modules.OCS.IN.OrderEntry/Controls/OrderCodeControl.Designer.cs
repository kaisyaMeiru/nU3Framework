namespace nU3.Modules.OCS.IN.OrderEntry.Controls
{
    partial class OrderCodeControl
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
            this.grpOrderCode = new DevExpress.XtraEditors.GroupControl();
            this.pnlButtons = new DevExpress.XtraEditors.PanelControl();
            this.btnDeleteOrder = new DevExpress.XtraEditors.SimpleButton();
            this.btnAddOrder = new DevExpress.XtraEditors.SimpleButton();
            this.gridControl = new DevExpress.XtraGrid.GridControl();
            this.gridView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colOrderCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colOrderName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colOrderType = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colQty = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colDays = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colDose = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colRoute = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colFrequency = new DevExpress.XtraGrid.Columns.GridColumn();
            ((System.ComponentModel.ISupportInitialize)(this.grpOrderCode)).BeginInit();
            this.grpOrderCode.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pnlButtons)).BeginInit();
            this.pnlButtons.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView)).BeginInit();
            this.SuspendLayout();
            // 
            // grpOrderCode
            // 
            this.grpOrderCode.Controls.Add(this.pnlButtons);
            this.grpOrderCode.Controls.Add(this.gridControl);
            this.grpOrderCode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpOrderCode.Location = new System.Drawing.Point(0, 0);
            this.grpOrderCode.Name = "grpOrderCode";
            this.grpOrderCode.Size = new System.Drawing.Size(536, 497);
            this.grpOrderCode.TabIndex = 0;
            this.grpOrderCode.Text = "처방코드";
            // 
            // pnlButtons
            // 
            this.pnlButtons.Controls.Add(this.btnDeleteOrder);
            this.pnlButtons.Controls.Add(this.btnAddOrder);
            this.pnlButtons.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlButtons.Location = new System.Drawing.Point(2, 22);
            this.pnlButtons.Name = "pnlButtons";
            this.pnlButtons.Size = new System.Drawing.Size(532, 30);
            this.pnlButtons.TabIndex = 1;
            // 
            // btnDeleteOrder
            // 
            this.btnDeleteOrder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDeleteOrder.Location = new System.Drawing.Point(450, 3);
            this.btnDeleteOrder.Name = "btnDeleteOrder";
            this.btnDeleteOrder.Size = new System.Drawing.Size(75, 23);
            this.btnDeleteOrder.TabIndex = 1;
            this.btnDeleteOrder.Text = "삭제";
            this.btnDeleteOrder.Click += new System.EventHandler(this.btnDeleteOrder_Click);
            // 
            // btnAddOrder
            // 
            this.btnAddOrder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddOrder.Location = new System.Drawing.Point(369, 3);
            this.btnAddOrder.Name = "btnAddOrder";
            this.btnAddOrder.Size = new System.Drawing.Size(75, 23);
            this.btnAddOrder.TabIndex = 0;
            this.btnAddOrder.Text = "추가";
            this.btnAddOrder.Click += new System.EventHandler(this.btnAddOrder_Click);
            // 
            // gridControl
            // 
            this.gridControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridControl.Location = new System.Drawing.Point(2, 52);
            this.gridControl.MainView = this.gridView;
            this.gridControl.Name = "gridControl";
            this.gridControl.Size = new System.Drawing.Size(532, 443);
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
            this.gridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colOrderCode,
            this.colOrderName,
            this.colOrderType,
            this.colQty,
            this.colDays,
            this.colDose,
            this.colRoute,
            this.colFrequency});
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
            // colOrderCode
            // 
            this.colOrderCode.Caption = "처방코드";
            this.colOrderCode.FieldName = "OrderCode";
            this.colOrderCode.Name = "colOrderCode";
            this.colOrderCode.Visible = true;
            this.colOrderCode.VisibleIndex = 0;
            this.colOrderCode.Width = 100;
            // 
            // colOrderName
            // 
            this.colOrderName.Caption = "처방명";
            this.colOrderName.FieldName = "OrderName";
            this.colOrderName.Name = "colOrderName";
            this.colOrderName.Visible = true;
            this.colOrderName.VisibleIndex = 1;
            this.colOrderName.Width = 200;
            // 
            // colOrderType
            // 
            this.colOrderType.Caption = "구분";
            this.colOrderType.FieldName = "OrderType";
            this.colOrderType.Name = "colOrderType";
            this.colOrderType.Visible = true;
            this.colOrderType.VisibleIndex = 2;
            this.colOrderType.Width = 60;
            // 
            // colQty
            // 
            this.colQty.Caption = "수량";
            this.colQty.FieldName = "Qty";
            this.colQty.Name = "colQty";
            this.colQty.Visible = true;
            this.colQty.VisibleIndex = 3;
            this.colQty.Width = 60;
            // 
            // colDays
            // 
            this.colDays.Caption = "일수";
            this.colDays.FieldName = "Days";
            this.colDays.Name = "colDays";
            this.colDays.Visible = true;
            this.colDays.VisibleIndex = 4;
            this.colDays.Width = 60;
            // 
            // colDose
            // 
            this.colDose.Caption = "1회량";
            this.colDose.FieldName = "Dose";
            this.colDose.Name = "colDose";
            this.colDose.Visible = true;
            this.colDose.VisibleIndex = 5;
            this.colDose.Width = 80;
            // 
            // colRoute
            // 
            this.colRoute.Caption = "투여경로";
            this.colRoute.FieldName = "Route";
            this.colRoute.Name = "colRoute";
            this.colRoute.Visible = true;
            this.colRoute.VisibleIndex = 6;
            this.colRoute.Width = 80;
            // 
            // colFrequency
            // 
            this.colFrequency.Caption = "투여빈도";
            this.colFrequency.FieldName = "Frequency";
            this.colFrequency.Name = "colFrequency";
            this.colFrequency.Visible = true;
            this.colFrequency.VisibleIndex = 7;
            this.colFrequency.Width = 80;
            // 
            // OrderCodeControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.grpOrderCode);
            this.Name = "OrderCodeControl";
            this.Size = new System.Drawing.Size(536, 497);
            ((System.ComponentModel.ISupportInitialize)(this.grpOrderCode)).EndInit();
            this.grpOrderCode.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pnlButtons)).EndInit();
            this.pnlButtons.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControl)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.GroupControl grpOrderCode;
        private DevExpress.XtraEditors.PanelControl pnlButtons;
        private DevExpress.XtraEditors.SimpleButton btnDeleteOrder;
        private DevExpress.XtraEditors.SimpleButton btnAddOrder;
        private DevExpress.XtraGrid.GridControl gridControl;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView;
        private DevExpress.XtraGrid.Columns.GridColumn colOrderCode;
        private DevExpress.XtraGrid.Columns.GridColumn colOrderName;
        private DevExpress.XtraGrid.Columns.GridColumn colOrderType;
        private DevExpress.XtraGrid.Columns.GridColumn colQty;
        private DevExpress.XtraGrid.Columns.GridColumn colDays;
        private DevExpress.XtraGrid.Columns.GridColumn colDose;
        private DevExpress.XtraGrid.Columns.GridColumn colRoute;
        private DevExpress.XtraGrid.Columns.GridColumn colFrequency;
    }
}