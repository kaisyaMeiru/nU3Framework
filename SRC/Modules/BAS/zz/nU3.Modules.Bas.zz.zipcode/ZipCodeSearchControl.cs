using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using nU3.Core.UI; // BaseWorkControl
using nU3.Core.Attributes; // nU3ProgramInfo
using nU3.Core.Logic; // IBizLogicFactory
using nU3.Modules.Bas.zz.zipcode.Logic;
using nU3.Modules.Bas.zz.zipcode.Models;
using Microsoft.Extensions.DependencyInjection;

namespace nU3.Modules.Bas.zz.zipcode
{    
    [nU3ProgramInfo(typeof(ZipCodeSearchControl), "우편번호검색", "ZIPCODE_SEARCH", "CHILD")]
    public partial class ZipCodeSearchControl : BaseWorkControl
    {
        private readonly ZipCodeBizLogic _logic;
        
        // Remove ScreenId override as it is not in BaseWorkControl. ProgramID is handled by attribute.

        [ActivatorUtilitiesConstructor]
        public ZipCodeSearchControl(IBizLogicFactory logicFactory)
        {
            InitializeComponent();
            
            // IBizLogicFactory를 통해 비즈니스 로직 인스턴스를 생성
            _logic = logicFactory.Create<ZipCodeBizLogic>();
            
            // Event Handlers
            this.Load += ZipCodeSearchControl_Load;
            this.btnSearch.Click += BtnSearch_Click;
            this.btnVerify.Click += BtnVerify_Click;
            this.radioGroupType.SelectedIndexChanged += RadioGroupType_SelectedIndexChanged;
        }

        public ZipCodeSearchControl() // For Designer
        {
            InitializeComponent();
        }

        private class ComboItem
        {
            public string Text { get; set; }
            public string Value { get; set; }
            public override string ToString() => Text;
        }

        private void ZipCodeSearchControl_Load(object sender, EventArgs e)
        {
            // Init Search Condition Combo
            cboSearchCondition.Properties.Items.Add(new ComboItem { Text = "우편번호", Value = "zipcode" });
            cboSearchCondition.Properties.Items.Add(new ComboItem { Text = "동명/아파트/건물", Value = "combination" });
            cboSearchCondition.Properties.Items.Add(new ComboItem { Text = "주소", Value = "address" });
            cboSearchCondition.SelectedIndex = 1; // Default combination

            // Init Road Search Combo (City)
            cboCity.Properties.Items.AddRange(new string[] { "서울", "부산", "대구", "인천", "광주", "대전", "울산", "세종", "경기", "강원", "충북", "충남", "전북", "전남", "경북", "경남", "제주" });
            
            // Default Mode
            UpdateSearchMode();
        }

        private void RadioGroupType_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateSearchMode();
        }

        private void UpdateSearchMode()
        {
            bool isRoad = (string)radioGroupType.EditValue == "N";
            layoutControlGroupLot.Visibility = isRoad ? DevExpress.XtraLayout.Utils.LayoutVisibility.Never : DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
            layoutControlGroupRoad.Visibility = isRoad ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            
            gridControlResult.DataSource = null;
        }

        private async void BtnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                bool isRoad = (string)radioGroupType.EditValue == "N";

                if (isRoad)
                {
                    // Road Search
                    var condition = new RoadSearchConditionDto
                    {
                        City = cboCity.Text,
                        RoadNm = txtRoadName.Text,
                        BldNo = txtBldNo.Text,
                        BldNm = txtBldNm.Text,
                        // CityCntyArea handling..
                    };
                    
                    if (string.IsNullOrEmpty(condition.City))
                    {
                        XtraMessageBox.Show("시도를 선택해주세요.", "알림");
                        return;
                    }
                    if (string.IsNullOrEmpty(condition.RoadNm))
                    {
                        XtraMessageBox.Show("도로명을 입력해주세요.", "알림");
                        return;
                    }

                    var rslt = await _logic.SearchRoadAddressAsync(new SearchConditionDto() { SearchCondition = "zipcode", SearchTerm = txtRoadName.Text.Trim() });
                    gridControlResult.DataSource = rslt;

                    var results = await _logic.SearchRoadAddressAsync2(condition);
                    gridControlResult.DataSource = results;
                }
                else
                {
                    // Lot Search
                    var condition = new SearchConditionDto
                    {
                        SearchCondition = (cboSearchCondition.SelectedItem as ComboItem)?.Value ?? "combination",
                        SearchTerm = txtSearchTerm.Text
                    };

                    if (string.IsNullOrEmpty(condition.SearchTerm) || condition.SearchTerm.Length < 2)
                    {
                        XtraMessageBox.Show("검색어는 2글자 이상 입력해주세요.", "알림");
                        return;
                    }

                    var results = await _logic.SearchLotAddressAsync(condition);
                    gridControlResult.DataSource = results;
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"검색 실패: {ex.Message}", "에러");
            }
        }

        private async void BtnVerify_Click(object sender, EventArgs e)
        {
            // Verify Logic
            // Get selected row
            var view = gridControlResult.MainView as DevExpress.XtraGrid.Views.Grid.GridView;
            if (view == null || view.FocusedRowHandle < 0) return;

            var selected = view.GetFocusedRow();
            if (selected == null) return;
            
            string zip = "", addr1 = "";
            
            if (selected is ZipCodeDto lot)
            {
                zip = lot.ZipCode;
                addr1 = lot.Address;
            }
            else if (selected is RoadAddressDto road)
            {
                zip = road.ZipCode;
                addr1 = $"{road.City} {road.CityCntyArea} {road.RoadNm} {road.BldNo}"; // Construct address
            }
            
            var addr2 = txtDetailAddress.Text;

            if (string.IsNullOrEmpty(addr2))
            {
                 if (XtraMessageBox.Show("상세주소가 없습니다. 진행하시겠습니까?", "확인", MessageBoxButtons.YesNo) != DialogResult.Yes)
                    return;
            }

            try 
            {
                var dto = new VerificationDto
                {
                    ZipCode = zip,
                    Address = addr1,
                    AddressDetail = addr2,
                    SearchMode = (string)radioGroupType.EditValue
                };

                var result = await _logic.VerifyAddressAsync(dto);
                
                // Show Result or Close logic
                XtraMessageBox.Show("주소 검증 완료.", "알림");
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"검증 실패: {ex.Message}", "에러");
            }
        }
    }
}
