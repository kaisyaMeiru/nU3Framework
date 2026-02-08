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
using nU3.Core.UI;

namespace nU3.Modules.OCS.IN.OrderEntry.Controls
{
    public partial class PatientInfoControl : DevExpress.XtraEditors.XtraUserControl
    {
        public PatientInfoControl()
        {
            InitializeComponent();
        }

        public void LoadPatientInfo(string inNumber)
        {
            // 데모 데이터 - 실제로는 DB에서 조회
            if (!string.IsNullOrEmpty(inNumber))
            {
                if (txtPatId != null) txtPatId.Text = "P001234";
                if (txtPatName != null) txtPatName.Text = "홍길동";
                if (txtGender != null) txtGender.Text = "남";
                if (txtAge != null) txtAge.Text = "45세";
                if (txtDeptName != null) txtDeptName.Text = "내과";
                if (txtDoctorName != null) txtDoctorName.Text = "김의사";
                if (txtRoomNo != null) txtRoomNo.Text = "301";
                if (txtInDate != null) txtInDate.Text = "2024-02-01";
                if (txtDiagnosis != null) txtDiagnosis.Text = "급성 위염";
            }
            else
            {
                ClearPatientInfo();
            }
        }

        public void ClearPatientInfo()
        {
            if (txtPatId != null) txtPatId.Text = string.Empty;
            if (txtPatName != null) txtPatName.Text = string.Empty;
            if (txtGender != null) txtGender.Text = string.Empty;
            if (txtAge != null) txtAge.Text = string.Empty;
            if (txtDeptName != null) txtDeptName.Text = string.Empty;
            if (txtDoctorName != null) txtDoctorName.Text = string.Empty;
            if (txtRoomNo != null) txtRoomNo.Text = string.Empty;
            if (txtInDate != null) txtInDate.Text = string.Empty;
            if (txtDiagnosis != null) txtDiagnosis.Text = string.Empty;
        }

        public void Reset()
        {
            ClearPatientInfo();
        }

        private void btnAlergy_Click(object sender, EventArgs e)
        {
            XtraMessageBox.Show("알레르기 정보 팝업", "환자정보", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnAlert_Click(object sender, EventArgs e)
        {
            XtraMessageBox.Show("주의사항 정보 팝업", "환자정보", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnMedi_Click(object sender, EventArgs e)
        {
            XtraMessageBox.Show("투약정보 팝업", "환자정보", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnVital_Click(object sender, EventArgs e)
        {
            XtraMessageBox.Show("생체징후 정보 팝업", "환자정보", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}