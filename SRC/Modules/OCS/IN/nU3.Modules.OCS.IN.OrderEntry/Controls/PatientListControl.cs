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
using DevExpress.XtraGrid.Views.Grid;
using nU3.Core.UI;

namespace nU3.Modules.OCS.IN.OrderEntry.Controls
{
    public partial class PatientListControl : DevExpress.XtraEditors.XtraUserControl
    {
        public PatientListControl()
        {
            InitializeComponent();
            this.Load += PatientListControl_Load;
        }

        private void PatientListControl_Load(object sender, EventArgs e)
        {
            if (!DesignMode)
            {
                LoadDemoData();
            }
        }

        private void LoadDemoData()
        {
            // ✅ 디자인 타임 체크
            if (DesignMode || LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                return;
            
            // ✅ gridControl null 체크
            if (gridControl == null)
                return;
            
            try
            {
                // 데모 데이터 생성
                DataTable dt = new DataTable();
                dt.Columns.Add("InNumber", typeof(string));
                dt.Columns.Add("PatiNumber", typeof(string));
                dt.Columns.Add("PatiName", typeof(string));
                dt.Columns.Add("Gender", typeof(string));
                dt.Columns.Add("Age", typeof(int));
                dt.Columns.Add("DeptName", typeof(string));
                dt.Columns.Add("DoctorName", typeof(string));
                dt.Columns.Add("RoomNo", typeof(string));
                dt.Columns.Add("AdmDate", typeof(DateTime));
                dt.Columns.Add("DoctorID", typeof(string));
                dt.Columns.Add("DeptID", typeof(string));

                // 데모 데이터 추가
                dt.Rows.Add("I2024001", "P001234", "홍길동", "남", 45, "내과", "김의사", "301", new DateTime(2024, 2, 1), "D001", "INT");
                dt.Rows.Add("I2024002", "P001235", "김철수", "남", 32, "외과", "이의사", "302", new DateTime(2024, 2, 2), "D002", "SUR");
                dt.Rows.Add("I2024003", "P001236", "이영희", "여", 28, "산부인과", "박의사", "303", new DateTime(2024, 2, 3), "D003", "OBS");
                dt.Rows.Add("I2024004", "P001237", "박민준", "남", 56, "내과", "최의사", "304", new DateTime(2024, 2, 4), "D001", "INT");
                dt.Rows.Add("I2024005", "P001238", "정수현", "여", 41, "정형외과", "정의사", "305", new DateTime(2024, 2, 5), "D004", "ORT");

                gridControl.DataSource = dt;
            }
            catch (Exception ex)
            {
                // 디자인 타임에서는 예외 무시
                System.Diagnostics.Debug.WriteLine($"LoadDemoData Error: {ex.Message}");
            }
        }

        public string SelectedInNumber { get; private set; }
        public string SelectedPatiNumber { get; private set; }
        public string SelectedPatiName { get; private set; }
        public string SelectedGender { get; private set; }
        public int SelectedAge { get; private set; }
        public string SelectedDeptName { get; private set; }
        public string SelectedDoctorName { get; private set; }
        public string SelectedRoomNo { get; private set; }
        public DateTime SelectedAdmDate { get; private set; }
        public string SelectedDoctorID { get; private set; }
        public string SelectedDeptID { get; private set; }

        private void gridView_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            // ✅ 디자인 타임 체크
            if (DesignMode || LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                return;
            
            // ✅ null 체크
            if (gridView == null || gridView.FocusedRowHandle < 0)
                return;
            
            try
            {
                DataRow row = gridView.GetDataRow(gridView.FocusedRowHandle);
                if (row != null)
                {
                    SelectedInNumber = row["InNumber"].ToString();
                    SelectedPatiNumber = row["PatiNumber"].ToString();
                    SelectedPatiName = row["PatiName"].ToString();
                    SelectedGender = row["Gender"].ToString();
                    SelectedAge = Convert.ToInt32(row["Age"]);
                    SelectedDeptName = row["DeptName"].ToString();
                    SelectedDoctorName = row["DoctorName"].ToString();
                    SelectedRoomNo = row["RoomNo"].ToString();
                    SelectedAdmDate = Convert.ToDateTime(row["AdmDate"]);
                    SelectedDoctorID = row["DoctorID"].ToString();
                    SelectedDeptID = row["DeptID"].ToString();

                    // 선택된 환자 정보를 이벤트로 전달
                    OnPatientSelected?.Invoke(this, EventArgs.Empty);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"gridView_FocusedRowChanged Error: {ex.Message}");
            }
        }

        private void gridView_DoubleClick(object sender, EventArgs e)
        {
            // ✅ 디자인 타임 체크
            if (DesignMode || LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                return;
            
            // ✅ null 체크
            if (gridView == null || gridView.FocusedRowHandle < 0)
                return;
            
            try
            {
                // 더블클릭 시 환자 선택
                OnPatientSelected?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"gridView_DoubleClick Error: {ex.Message}");
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            // ✅ 디자인 타임 체크
            if (DesignMode || LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                return;
            
            // 데이터 새로고침
            LoadDemoData();
        }

        public event EventHandler OnPatientSelected;
    }
}