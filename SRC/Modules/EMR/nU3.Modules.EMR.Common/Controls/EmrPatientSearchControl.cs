using System;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using nU3.Core.UI;
using nU3.Modules.EMR.Common.Models;

namespace nU3.Modules.EMR.Common.Controls
{
    public partial class EmrPatientSearchControl : BaseWorkComponent
    {
        public event EventHandler<EmrPatientSummaryDto>? PatientSelected;

        public EmrPatientSearchControl()
        {
            InitializeComponent();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            PerformSearch();
        }

        private void txtKeyword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                PerformSearch();
            }
        }

        private void PerformSearch()
        {
            var keyword = txtKeyword.Text.Trim();
            if (string.IsNullOrEmpty(keyword))
            {
                XtraMessageBox.Show("검색어를 입력하세요.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Dummy Search Logic for Example
            var dummyPatient = new EmrPatientSummaryDto
            {
                PatientId = "12345678",
                PatientName = "홍길동",
                Gender = "Male",
                BirthDate = new DateTime(1980, 5, 20),
                BloodType = "A+",
                DepartmentCode = Constants.EmrConstants.Departments.InternalMedicine,
                WardCode = "101"
            };

            XtraMessageBox.Show($"[EMR Common] 환자 검색됨: {dummyPatient}", "검색 결과");

            // Event Trigger
            PatientSelected?.Invoke(this, dummyPatient);
        }
    }
}
