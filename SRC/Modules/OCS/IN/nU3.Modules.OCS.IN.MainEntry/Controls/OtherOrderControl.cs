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
    public partial class OtherOrderControl : BaseWorkControl
    {
        public OtherOrderControl()
        {
            InitializeComponent();
        }

        public RefCodeType RefCodeType { get; set; } = RefCodeType.REP;

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

        public void SetRefCodeSelect(RefCodeType refCodeType)
        {
            RefCodeType = refCodeType;
            
            // 탭 선택에 따른 처리
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
    }
}