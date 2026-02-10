namespace nU3.Modules.OCS.IN.MainEntry.Controls
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
            grpOrderCode = new nU3.Core.UI.Controls.nU3GroupControl();
            pnlButtons = new nU3.Core.UI.Controls.nU3PanelControl();
            btnDeleteOrder = new nU3.Core.UI.Controls.nU3SimpleButton();
            btnAddOrder = new nU3.Core.UI.Controls.nU3SimpleButton();
            gridControl = new nU3.Core.UI.Controls.nU3GridControl();
            gridView = new nU3.Core.UI.Controls.nU3GridView();
            colOrderCode = new nU3.Core.UI.Controls.nU3GridColumn();
            colOrderName = new nU3.Core.UI.Controls.nU3GridColumn();
            colOrderType = new nU3.Core.UI.Controls.nU3GridColumn();
            colQty = new nU3.Core.UI.Controls.nU3GridColumn();
            colDays = new nU3.Core.UI.Controls.nU3GridColumn();
            colDose = new nU3.Core.UI.Controls.nU3GridColumn();
            colRoute = new nU3.Core.UI.Controls.nU3GridColumn();
            colFrequency = new nU3.Core.UI.Controls.nU3GridColumn();
            ((System.ComponentModel.ISupportInitialize)grpOrderCode).BeginInit();
            grpOrderCode.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pnlButtons).BeginInit();
            pnlButtons.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)gridControl).BeginInit();
            ((System.ComponentModel.ISupportInitialize)gridView).BeginInit();
            SuspendLayout();
            // 
            // grpOrderCode
            // 
            grpOrderCode.Controls.Add(pnlButtons);
            grpOrderCode.Controls.Add(gridControl);
            grpOrderCode.Dock = DockStyle.Fill;
            grpOrderCode.Location = new Point(0, 0);
            grpOrderCode.Margin = new Padding(4);
            grpOrderCode.Name = "grpOrderCode";
            grpOrderCode.Size = new Size(1406, 750);
            grpOrderCode.TabIndex = 0;
            grpOrderCode.Text = "처방코드";
            // 
            // pnlButtons
            // 
            pnlButtons.Controls.Add(btnDeleteOrder);
            pnlButtons.Controls.Add(btnAddOrder);
            pnlButtons.Dock = DockStyle.Top;
            pnlButtons.Location = new Point(2, 23);
            pnlButtons.Margin = new Padding(4);
            pnlButtons.Name = "pnlButtons";
            pnlButtons.Size = new Size(1402, 38);
            pnlButtons.TabIndex = 1;
            // 
            // btnDeleteOrder
            // 
            btnDeleteOrder.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnDeleteOrder.AuthId = "";
            btnDeleteOrder.Location = new Point(1306, 4);
            btnDeleteOrder.Margin = new Padding(4);
            btnDeleteOrder.Name = "btnDeleteOrder";
            btnDeleteOrder.Size = new Size(88, 29);
            btnDeleteOrder.TabIndex = 1;
            btnDeleteOrder.Text = "삭제";
            btnDeleteOrder.Click += btnDeleteOrder_Click;
            // 
            // btnAddOrder
            // 
            btnAddOrder.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnAddOrder.AuthId = "";
            btnAddOrder.Location = new Point(1211, 4);
            btnAddOrder.Margin = new Padding(4);
            btnAddOrder.Name = "btnAddOrder";
            btnAddOrder.Size = new Size(88, 29);
            btnAddOrder.TabIndex = 0;
            btnAddOrder.Text = "추가";
            btnAddOrder.Click += btnAddOrder_Click;
            // 
            // gridControl
            // 
            gridControl.Dock = DockStyle.Fill;
            gridControl.EmbeddedNavigator.Margin = new Padding(4);
            gridControl.Location = new Point(2, 23);
            gridControl.MainView = gridView;
            gridControl.Margin = new Padding(4);
            gridControl.Name = "gridControl";
            gridControl.Size = new Size(1402, 725);
            gridControl.TabIndex = 0;
            gridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { gridView });
            // 
            // gridView
            // 
            gridView.Appearance.HeaderPanel.Options.UseTextOptions = true;
            gridView.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            gridView.Appearance.Row.Options.UseTextOptions = true;
            gridView.Appearance.Row.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            gridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] { colOrderCode, colOrderName, colOrderType, colQty, colDays, colDose, colRoute, colFrequency });
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
            // colOrderCode
            // 
            colOrderCode.AuthId = "";
            colOrderCode.Caption = "처방코드";
            colOrderCode.FieldName = "OrderCode";
            colOrderCode.MinWidth = 23;
            colOrderCode.Name = "colOrderCode";
            colOrderCode.ResourceKey = "";
            colOrderCode.Visible = true;
            colOrderCode.VisibleIndex = 0;
            colOrderCode.Width = 117;
            // 
            // colOrderName
            // 
            colOrderName.AuthId = "";
            colOrderName.Caption = "처방명";
            colOrderName.FieldName = "OrderName";
            colOrderName.MinWidth = 23;
            colOrderName.Name = "colOrderName";
            colOrderName.ResourceKey = "";
            colOrderName.Visible = true;
            colOrderName.VisibleIndex = 1;
            colOrderName.Width = 233;
            // 
            // colOrderType
            // 
            colOrderType.AuthId = "";
            colOrderType.Caption = "구분";
            colOrderType.FieldName = "OrderType";
            colOrderType.MinWidth = 23;
            colOrderType.Name = "colOrderType";
            colOrderType.ResourceKey = "";
            colOrderType.Visible = true;
            colOrderType.VisibleIndex = 2;
            colOrderType.Width = 70;
            // 
            // colQty
            // 
            colQty.AuthId = "";
            colQty.Caption = "수량";
            colQty.FieldName = "Qty";
            colQty.MinWidth = 23;
            colQty.Name = "colQty";
            colQty.ResourceKey = "";
            colQty.Visible = true;
            colQty.VisibleIndex = 3;
            colQty.Width = 70;
            // 
            // colDays
            // 
            colDays.AuthId = "";
            colDays.Caption = "일수";
            colDays.FieldName = "Days";
            colDays.MinWidth = 23;
            colDays.Name = "colDays";
            colDays.ResourceKey = "";
            colDays.Visible = true;
            colDays.VisibleIndex = 4;
            colDays.Width = 70;
            // 
            // colDose
            // 
            colDose.AuthId = "";
            colDose.Caption = "1회량";
            colDose.FieldName = "Dose";
            colDose.MinWidth = 23;
            colDose.Name = "colDose";
            colDose.ResourceKey = "";
            colDose.Visible = true;
            colDose.VisibleIndex = 5;
            colDose.Width = 93;
            // 
            // colRoute
            // 
            colRoute.AuthId = "";
            colRoute.Caption = "투여경로";
            colRoute.FieldName = "Route";
            colRoute.MinWidth = 23;
            colRoute.Name = "colRoute";
            colRoute.ResourceKey = "";
            colRoute.Visible = true;
            colRoute.VisibleIndex = 6;
            colRoute.Width = 93;
            // 
            // colFrequency
            // 
            colFrequency.AuthId = "";
            colFrequency.Caption = "투여빈도";
            colFrequency.FieldName = "Frequency";
            colFrequency.MinWidth = 23;
            colFrequency.Name = "colFrequency";
            colFrequency.ResourceKey = "";
            colFrequency.Visible = true;
            colFrequency.VisibleIndex = 7;
            colFrequency.Width = 93;
            // 
            // OrderCodeControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(grpOrderCode);
            Margin = new Padding(4);
            Name = "OrderCodeControl";
            Size = new Size(1406, 750);
            ((System.ComponentModel.ISupportInitialize)grpOrderCode).EndInit();
            grpOrderCode.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pnlButtons).EndInit();
            pnlButtons.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)gridControl).EndInit();
            ((System.ComponentModel.ISupportInitialize)gridView).EndInit();
            ResumeLayout(false);

        }

        #endregion
        private nU3.Core.UI.Controls.nU3GridControl gridControl;
        private nU3.Core.UI.Controls.nU3GridView gridView;
        private nU3.Core.UI.Controls.nU3GridColumn colOrderCode;
        private nU3.Core.UI.Controls.nU3GridColumn colOrderName;
        private nU3.Core.UI.Controls.nU3GridColumn colOrderType;
        private nU3.Core.UI.Controls.nU3GridColumn colQty;
        private nU3.Core.UI.Controls.nU3GridColumn colDays;
        private nU3.Core.UI.Controls.nU3GridColumn colDose;
        private nU3.Core.UI.Controls.nU3GridColumn colRoute;
        private nU3.Core.UI.Controls.nU3GridColumn colFrequency;
        private Core.UI.Controls.nU3GroupControl grpOrderCode;
        private Core.UI.Controls.nU3PanelControl pnlButtons;
        private Core.UI.Controls.nU3SimpleButton btnDeleteOrder;
        private Core.UI.Controls.nU3SimpleButton btnAddOrder;
    }
}