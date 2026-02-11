using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraTab;
using nU3.Core.UI;

namespace nU3.Modules.OCS.IN.MainEntry.Controls
{
    public partial class OtherOrderControl : UserControl
    {
        public OtherOrderControl()
        {
            InitializeComponent();
            
            // 탭 헤더 숨기기
            if (tabControl != null)
            {
                tabControl.ShowTabHeader = DevExpress.Utils.DefaultBoolean.False;
                
                // 탭 변경 이벤트 구독 (외부에서 탭이 변경되었을 때 이벤트 발생)
                tabControl.SelectedPageChanged += TabControl_SelectedPageChanged;
            }
        }

        public RefCodeType RefCodeType { get; set; } = RefCodeType.REP;

        /// <summary>
        /// 탭이 변경되었을 때 발생하는 이벤트
        /// </summary>
        public event EventHandler<RefCodeType> TabChanged;

        private void TabControl_SelectedPageChanged(object sender, TabPageChangedEventArgs e)
        {
            // 현재 선택된 탭에 따라 RefCodeType 업데이트
            if (e.Page == tabOrder)
            {
                RefCodeType = RefCodeType.REP;
                TabChanged?.Invoke(this, RefCodeType.REP);
            }
            else if (e.Page == tabSheet)
            {
                RefCodeType = RefCodeType.SHT;
                TabChanged?.Invoke(this, RefCodeType.SHT);
            }
            else if (e.Page == tabEtc)
            {
                RefCodeType = RefCodeType.ETC;
                TabChanged?.Invoke(this, RefCodeType.ETC);
            }
        }

        private void btnLabOrder_Click(object sender, EventArgs e)
        {
            XtraMessageBox.Show("검사처방 팝업", "검사처방", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnRadOrder_Click(object sender, EventArgs e)
        {
            XtraMessageBox.Show("방사선처방 팝업", "방사선처방", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnSurgeryOrder_Click(object sender, EventArgs e)
        {
            XtraMessageBox.Show("수술처방 팝업", "수술처방", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnNurseOrder_Click(object sender, EventArgs e)
        {
            XtraMessageBox.Show("간호처방 팝업", "간호처방", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnPhysicalOrder_Click(object sender, EventArgs e)
        {
            XtraMessageBox.Show("물리치료 팝업", "물리치료", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnDietOrder_Click(object sender, EventArgs e)
        {
            XtraMessageBox.Show("식이처방 팝업", "식이처방", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public void LoadOtherOrder(string patiNumber, string deptID, string doctorID)
        {
            // 실제로는 DB에서 해당 환자의 기타처방내역을 조회
            // 데모에서는 아무 작업하지 않음
        }

        public void Reset()
        {
            // 데이터 초기화
        }

        /// <summary>
        /// 외부에서 탭을 변경합니다 (OtherTabControl에서 호출)
        /// </summary>
        public void SetRefCodeSelect(RefCodeType refCodeType)
        {
            RefCodeType = refCodeType;
            
            if (tabControl == null) return;

            // 탭 선택에 따른 처리 (이벤트가 발생하지 않도록 일시적으로 구독 해제)
            tabControl.SelectedPageChanged -= TabControl_SelectedPageChanged;
            
            try
            {
                switch (refCodeType)
                {
                    case RefCodeType.REP:
                        tabControl.SelectedTabPage = tabOrder;
                        break;
                    case RefCodeType.SHT:
                        tabControl.SelectedTabPage = tabSheet;
                        break;
                    case RefCodeType.ETC:
                        tabControl.SelectedTabPage = tabEtc;
                        break;
                }
            }
            finally
            {
                // 이벤트 다시 구독
                tabControl.SelectedPageChanged += TabControl_SelectedPageChanged;
            }
        }
    }
}