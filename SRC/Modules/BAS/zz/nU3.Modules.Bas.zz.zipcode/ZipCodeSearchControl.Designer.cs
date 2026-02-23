namespace nU3.Modules.Bas.zz.zipcode
{
    partial class ZipCodeSearchControl
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        private void InitializeComponent()
        {
            layoutControl1 = new nU3.Core.UI.Controls.nU3LayoutControl();
            radioGroupType = new nU3.Core.UI.Controls.nU3RadioGroup();
            cboSearchCondition = new nU3.Core.UI.Controls.nU3ComboBoxEdit();
            txtSearchTerm = new nU3.Core.UI.Controls.nU3TextEdit();
            cboCity = new nU3.Core.UI.Controls.nU3ComboBoxEdit();
            txtRoadName = new nU3.Core.UI.Controls.nU3TextEdit();
            txtBldNo = new nU3.Core.UI.Controls.nU3TextEdit();
            txtBldNm = new nU3.Core.UI.Controls.nU3TextEdit();
            gridControlResult = new nU3.Core.UI.Controls.nU3GridControl();
            gridViewResult = new nU3.Core.UI.Controls.nU3GridView();
            txtDetailAddress = new nU3.Core.UI.Controls.nU3TextEdit();
            btnSearch = new nU3.Core.UI.Controls.nU3SimpleButton();
            btnVerify = new nU3.Core.UI.Controls.nU3SimpleButton();
            layoutControlGroup1 = new nU3.Core.UI.Controls.nU3LayoutControlGroup();
            layoutControlItemRadio = new nU3.Core.UI.Controls.nU3LayoutControlItem();
            layoutControlGroupLot = new nU3.Core.UI.Controls.nU3LayoutControlGroup();
            layoutControlItemCondition = new nU3.Core.UI.Controls.nU3LayoutControlItem();
            layoutControlItemTerm = new nU3.Core.UI.Controls.nU3LayoutControlItem();
            layoutControlGroupRoad = new nU3.Core.UI.Controls.nU3LayoutControlGroup();
            layoutControlItemCity = new nU3.Core.UI.Controls.nU3LayoutControlItem();
            layoutControlItemRoadName = new nU3.Core.UI.Controls.nU3LayoutControlItem();
            layoutControlItemBldNo = new nU3.Core.UI.Controls.nU3LayoutControlItem();
            layoutControlItemBldNm = new nU3.Core.UI.Controls.nU3LayoutControlItem();
            layoutControlItemGrid = new nU3.Core.UI.Controls.nU3LayoutControlItem();
            layoutControlItemDetail = new nU3.Core.UI.Controls.nU3LayoutControlItem();
            layoutControlItemSearch = new nU3.Core.UI.Controls.nU3LayoutControlItem();
            layoutControlItemVerify = new nU3.Core.UI.Controls.nU3LayoutControlItem();
            emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            ((System.ComponentModel.ISupportInitialize)layoutControl1).BeginInit();
            layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)radioGroupType.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)cboSearchCondition.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)txtSearchTerm.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)cboCity.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)txtRoadName.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)txtBldNo.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)txtBldNm.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)gridControlResult).BeginInit();
            ((System.ComponentModel.ISupportInitialize)gridViewResult).BeginInit();
            ((System.ComponentModel.ISupportInitialize)txtDetailAddress.Properties).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlGroup1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItemRadio).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlGroupLot).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItemCondition).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItemTerm).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlGroupRoad).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItemCity).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItemRoadName).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItemBldNo).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItemBldNm).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItemGrid).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItemDetail).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItemSearch).BeginInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItemVerify).BeginInit();
            ((System.ComponentModel.ISupportInitialize)emptySpaceItem1).BeginInit();
            SuspendLayout();
            // 
            // layoutControl1
            // 
            layoutControl1.Controls.Add(radioGroupType);
            layoutControl1.Controls.Add(cboSearchCondition);
            layoutControl1.Controls.Add(txtSearchTerm);
            layoutControl1.Controls.Add(cboCity);
            layoutControl1.Controls.Add(txtRoadName);
            layoutControl1.Controls.Add(txtBldNo);
            layoutControl1.Controls.Add(txtBldNm);
            layoutControl1.Controls.Add(gridControlResult);
            layoutControl1.Controls.Add(txtDetailAddress);
            layoutControl1.Controls.Add(btnSearch);
            layoutControl1.Controls.Add(btnVerify);
            layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            layoutControl1.Location = new System.Drawing.Point(0, 0);
            layoutControl1.Name = "layoutControl1";
            layoutControl1.Root = layoutControlGroup1;
            layoutControl1.Size = new System.Drawing.Size(1160, 848);
            layoutControl1.TabIndex = 0;
            layoutControl1.Text = "layoutControl1";
            // 
            // radioGroupType
            // 
            radioGroupType.EditValue = "N";
            radioGroupType.Location = new System.Drawing.Point(12, 11);
            radioGroupType.Name = "radioGroupType";
            radioGroupType.Properties.Items.AddRange(new DevExpress.XtraEditors.Controls.RadioGroupItem[] { new DevExpress.XtraEditors.Controls.RadioGroupItem("J", "지번주소"), new DevExpress.XtraEditors.Controls.RadioGroupItem("N", "도로명주소") });
            radioGroupType.Size = new System.Drawing.Size(1136, 53);
            radioGroupType.StyleController = layoutControl1;
            radioGroupType.TabIndex = 4;
            // 
            // cboSearchCondition
            // 
            cboSearchCondition.Location = new System.Drawing.Point(76, 79);
            cboSearchCondition.Name = "cboSearchCondition";
            cboSearchCondition.Size = new System.Drawing.Size(240, 20);
            cboSearchCondition.StyleController = layoutControl1;
            cboSearchCondition.TabIndex = 5;
            // 
            // txtSearchTerm
            // 
            txtSearchTerm.Location = new System.Drawing.Point(372, 79);
            txtSearchTerm.Name = "txtSearchTerm";
            txtSearchTerm.Size = new System.Drawing.Size(764, 20);
            txtSearchTerm.StyleController = layoutControl1;
            txtSearchTerm.TabIndex = 6;
            // 
            // cboCity
            // 
            cboCity.Location = new System.Drawing.Point(76, 125);
            cboCity.Name = "cboCity";
            cboCity.Size = new System.Drawing.Size(165, 20);
            cboCity.StyleController = layoutControl1;
            cboCity.TabIndex = 7;
            // 
            // txtRoadName
            // 
            txtRoadName.Location = new System.Drawing.Point(297, 125);
            txtRoadName.Name = "txtRoadName";
            txtRoadName.Size = new System.Drawing.Size(240, 20);
            txtRoadName.StyleController = layoutControl1;
            txtRoadName.TabIndex = 8;
            // 
            // txtBldNo
            // 
            txtBldNo.Location = new System.Drawing.Point(593, 125);
            txtBldNo.Name = "txtBldNo";
            txtBldNo.Size = new System.Drawing.Size(166, 20);
            txtBldNo.StyleController = layoutControl1;
            txtBldNo.TabIndex = 9;
            // 
            // txtBldNm
            // 
            txtBldNm.Location = new System.Drawing.Point(815, 125);
            txtBldNm.Name = "txtBldNm";
            txtBldNm.Size = new System.Drawing.Size(321, 20);
            txtBldNm.StyleController = layoutControl1;
            txtBldNm.TabIndex = 10;
            // 
            // gridControlResult
            // 
            gridControlResult.Location = new System.Drawing.Point(12, 186);
            gridControlResult.MainView = gridViewResult;
            gridControlResult.Name = "gridControlResult";
            gridControlResult.Size = new System.Drawing.Size(1136, 625);
            gridControlResult.TabIndex = 11;
            gridControlResult.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { gridViewResult });
            // 
            // gridViewResult
            // 
            gridViewResult.GridControl = gridControlResult;
            gridViewResult.Name = "gridViewResult";
            gridViewResult.OptionsBehavior.Editable = false;
            gridViewResult.OptionsView.ShowGroupPanel = false;
            // 
            // txtDetailAddress
            // 
            txtDetailAddress.Location = new System.Drawing.Point(64, 815);
            txtDetailAddress.Name = "txtDetailAddress";
            txtDetailAddress.Size = new System.Drawing.Size(967, 20);
            txtDetailAddress.StyleController = layoutControl1;
            txtDetailAddress.TabIndex = 12;
            // 
            // btnSearch
            // 
            btnSearch.Location = new System.Drawing.Point(1035, 160);
            btnSearch.Name = "btnSearch";
            btnSearch.Size = new System.Drawing.Size(113, 22);
            btnSearch.StyleController = layoutControl1;
            btnSearch.TabIndex = 13;
            btnSearch.Text = "조회";
            // 
            // btnVerify
            // 
            btnVerify.Location = new System.Drawing.Point(1035, 815);
            btnVerify.Name = "btnVerify";
            btnVerify.Size = new System.Drawing.Size(113, 22);
            btnVerify.StyleController = layoutControl1;
            btnVerify.TabIndex = 14;
            btnVerify.Text = "검증";
            // 
            // layoutControlGroup1
            // 
            layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            layoutControlGroup1.GroupBordersVisible = false;
            layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] { layoutControlItemRadio, layoutControlGroupLot, layoutControlGroupRoad, layoutControlItemGrid, layoutControlItemDetail, layoutControlItemSearch, layoutControlItemVerify, emptySpaceItem1 });
            layoutControlGroup1.Name = "Root";
            layoutControlGroup1.Size = new System.Drawing.Size(1160, 848);
            layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItemRadio
            // 
            layoutControlItemRadio.Control = radioGroupType;
            layoutControlItemRadio.Location = new System.Drawing.Point(0, 0);
            layoutControlItemRadio.Name = "layoutControlItemRadio";
            layoutControlItemRadio.Size = new System.Drawing.Size(1140, 57);
            layoutControlItemRadio.TextSize = new System.Drawing.Size(0, 0);
            layoutControlItemRadio.TextVisible = false;
            // 
            // layoutControlGroupLot
            // 
            layoutControlGroupLot.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] { layoutControlItemCondition, layoutControlItemTerm });
            layoutControlGroupLot.Location = new System.Drawing.Point(0, 57);
            layoutControlGroupLot.Name = "layoutControlGroupLot";
            layoutControlGroupLot.Size = new System.Drawing.Size(1140, 46);
            layoutControlGroupLot.Text = "지번주소 검색조건";
            layoutControlGroupLot.TextVisible = false;
            // 
            // layoutControlItemCondition
            // 
            layoutControlItemCondition.Control = cboSearchCondition;
            layoutControlItemCondition.Location = new System.Drawing.Point(0, 0);
            layoutControlItemCondition.Name = "layoutControlItemCondition";
            layoutControlItemCondition.Size = new System.Drawing.Size(296, 24);
            layoutControlItemCondition.Text = "검색조건";
            layoutControlItemCondition.TextSize = new System.Drawing.Size(40, 14);
            // 
            // layoutControlItemTerm
            // 
            layoutControlItemTerm.Control = txtSearchTerm;
            layoutControlItemTerm.Location = new System.Drawing.Point(296, 0);
            layoutControlItemTerm.Name = "layoutControlItemTerm";
            layoutControlItemTerm.Size = new System.Drawing.Size(820, 24);
            layoutControlItemTerm.Text = "검색어";
            layoutControlItemTerm.TextSize = new System.Drawing.Size(40, 14);
            // 
            // layoutControlGroupRoad
            // 
            layoutControlGroupRoad.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] { layoutControlItemCity, layoutControlItemRoadName, layoutControlItemBldNo, layoutControlItemBldNm });
            layoutControlGroupRoad.Location = new System.Drawing.Point(0, 103);
            layoutControlGroupRoad.Name = "layoutControlGroupRoad";
            layoutControlGroupRoad.Size = new System.Drawing.Size(1140, 46);
            layoutControlGroupRoad.Text = "도로명주소 검색조건";
            layoutControlGroupRoad.TextVisible = false;
            // 
            // layoutControlItemCity
            // 
            layoutControlItemCity.Control = cboCity;
            layoutControlItemCity.Location = new System.Drawing.Point(0, 0);
            layoutControlItemCity.Name = "layoutControlItemCity";
            layoutControlItemCity.Size = new System.Drawing.Size(221, 24);
            layoutControlItemCity.Text = "시도";
            layoutControlItemCity.TextSize = new System.Drawing.Size(40, 14);
            // 
            // layoutControlItemRoadName
            // 
            layoutControlItemRoadName.Control = txtRoadName;
            layoutControlItemRoadName.Location = new System.Drawing.Point(221, 0);
            layoutControlItemRoadName.Name = "layoutControlItemRoadName";
            layoutControlItemRoadName.Size = new System.Drawing.Size(296, 24);
            layoutControlItemRoadName.Text = "도로명";
            layoutControlItemRoadName.TextSize = new System.Drawing.Size(40, 14);
            // 
            // layoutControlItemBldNo
            // 
            layoutControlItemBldNo.Control = txtBldNo;
            layoutControlItemBldNo.Location = new System.Drawing.Point(517, 0);
            layoutControlItemBldNo.Name = "layoutControlItemBldNo";
            layoutControlItemBldNo.Size = new System.Drawing.Size(222, 24);
            layoutControlItemBldNo.Text = "건물번호";
            layoutControlItemBldNo.TextSize = new System.Drawing.Size(40, 14);
            // 
            // layoutControlItemBldNm
            // 
            layoutControlItemBldNm.Control = txtBldNm;
            layoutControlItemBldNm.Location = new System.Drawing.Point(739, 0);
            layoutControlItemBldNm.Name = "layoutControlItemBldNm";
            layoutControlItemBldNm.Size = new System.Drawing.Size(377, 24);
            layoutControlItemBldNm.Text = "건물명";
            layoutControlItemBldNm.TextSize = new System.Drawing.Size(40, 14);
            // 
            // layoutControlItemGrid
            // 
            layoutControlItemGrid.Control = gridControlResult;
            layoutControlItemGrid.Location = new System.Drawing.Point(0, 175);
            layoutControlItemGrid.Name = "layoutControlItemGrid";
            layoutControlItemGrid.Size = new System.Drawing.Size(1140, 629);
            layoutControlItemGrid.TextSize = new System.Drawing.Size(0, 0);
            layoutControlItemGrid.TextVisible = false;
            // 
            // layoutControlItemDetail
            // 
            layoutControlItemDetail.Control = txtDetailAddress;
            layoutControlItemDetail.Location = new System.Drawing.Point(0, 804);
            layoutControlItemDetail.Name = "layoutControlItemDetail";
            layoutControlItemDetail.Size = new System.Drawing.Size(1023, 26);
            layoutControlItemDetail.Text = "상세주소";
            layoutControlItemDetail.TextSize = new System.Drawing.Size(40, 14);
            // 
            // layoutControlItemSearch
            // 
            layoutControlItemSearch.Control = btnSearch;
            layoutControlItemSearch.Location = new System.Drawing.Point(1023, 149);
            layoutControlItemSearch.Name = "layoutControlItemSearch";
            layoutControlItemSearch.Size = new System.Drawing.Size(117, 26);
            layoutControlItemSearch.TextSize = new System.Drawing.Size(0, 0);
            layoutControlItemSearch.TextVisible = false;
            // 
            // layoutControlItemVerify
            // 
            layoutControlItemVerify.Control = btnVerify;
            layoutControlItemVerify.Location = new System.Drawing.Point(1023, 804);
            layoutControlItemVerify.Name = "layoutControlItemVerify";
            layoutControlItemVerify.Size = new System.Drawing.Size(117, 26);
            layoutControlItemVerify.TextSize = new System.Drawing.Size(0, 0);
            layoutControlItemVerify.TextVisible = false;
            // 
            // emptySpaceItem1
            // 
            emptySpaceItem1.AllowHotTrack = false;
            emptySpaceItem1.Location = new System.Drawing.Point(0, 149);
            emptySpaceItem1.Name = "emptySpaceItem1";
            emptySpaceItem1.Size = new System.Drawing.Size(1023, 26);
            emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // ZipCodeSearchControl
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(layoutControl1);
            Name = "ZipCodeSearchControl";
            Size = new System.Drawing.Size(1160, 848);
            ((System.ComponentModel.ISupportInitialize)layoutControl1).EndInit();
            layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)radioGroupType.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)cboSearchCondition.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)txtSearchTerm.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)cboCity.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)txtRoadName.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)txtBldNo.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)txtBldNm.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)gridControlResult).EndInit();
            ((System.ComponentModel.ISupportInitialize)gridViewResult).EndInit();
            ((System.ComponentModel.ISupportInitialize)txtDetailAddress.Properties).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlGroup1).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItemRadio).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlGroupLot).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItemCondition).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItemTerm).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlGroupRoad).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItemCity).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItemRoadName).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItemBldNo).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItemBldNm).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItemGrid).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItemDetail).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItemSearch).EndInit();
            ((System.ComponentModel.ISupportInitialize)layoutControlItemVerify).EndInit();
            ((System.ComponentModel.ISupportInitialize)emptySpaceItem1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private nU3.Core.UI.Controls.nU3LayoutControl layoutControl1;
        private nU3.Core.UI.Controls.nU3LayoutControlGroup layoutControlGroup1;
        
        private nU3.Core.UI.Controls.nU3RadioGroup radioGroupType;
        private nU3.Core.UI.Controls.nU3LayoutControlItem layoutControlItemRadio;
        
        private nU3.Core.UI.Controls.nU3ComboBoxEdit cboSearchCondition;
        private nU3.Core.UI.Controls.nU3TextEdit txtSearchTerm;
        private nU3.Core.UI.Controls.nU3LayoutControlGroup layoutControlGroupLot;
        private nU3.Core.UI.Controls.nU3LayoutControlItem layoutControlItemCondition;
        private nU3.Core.UI.Controls.nU3LayoutControlItem layoutControlItemTerm;
        
        private nU3.Core.UI.Controls.nU3ComboBoxEdit cboCity;
        private nU3.Core.UI.Controls.nU3TextEdit txtRoadName;
        private nU3.Core.UI.Controls.nU3TextEdit txtBldNo;
        private nU3.Core.UI.Controls.nU3TextEdit txtBldNm;
        private nU3.Core.UI.Controls.nU3LayoutControlGroup layoutControlGroupRoad;
        private nU3.Core.UI.Controls.nU3LayoutControlItem layoutControlItemCity;
        private nU3.Core.UI.Controls.nU3LayoutControlItem layoutControlItemRoadName;
        private nU3.Core.UI.Controls.nU3LayoutControlItem layoutControlItemBldNo;
        private nU3.Core.UI.Controls.nU3LayoutControlItem layoutControlItemBldNm;
        
        private nU3.Core.UI.Controls.nU3GridControl gridControlResult;
        private nU3.Core.UI.Controls.nU3GridView gridViewResult;
        private nU3.Core.UI.Controls.nU3LayoutControlItem layoutControlItemGrid;
        
        private nU3.Core.UI.Controls.nU3TextEdit txtDetailAddress;
        private nU3.Core.UI.Controls.nU3LayoutControlItem layoutControlItemDetail;
        
        private nU3.Core.UI.Controls.nU3SimpleButton btnSearch;
        private nU3.Core.UI.Controls.nU3LayoutControlItem layoutControlItemSearch;

        private nU3.Core.UI.Controls.nU3SimpleButton btnVerify;
        private nU3.Core.UI.Controls.nU3LayoutControlItem layoutControlItemVerify;

        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
    }
}
