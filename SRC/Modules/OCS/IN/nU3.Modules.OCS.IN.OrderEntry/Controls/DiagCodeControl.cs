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
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraEditors.Repository;
using DevExpress.Utils;
using nU3.Core.UI;

namespace nU3.Modules.OCS.IN.OrderEntry.Controls
{
    public partial class DiagCodeControl : DevExpress.XtraEditors.XtraUserControl
    {
        public DiagCodeControl()
        {
            InitializeComponent();
            // Load 이벤트에서 그리드 초기화를 수행하도록 변경
            this.Load += DiagCodeControl_Load;
        }

        private void DiagCodeControl_Load(object sender, EventArgs e)
        {
            InitializeGrid();
            LoadDemoData();
        }

        private void InitializeGrid()
        {
            // 그리드 뷰 초기화 - null 체크 추가
            if (gridView == null) return;

            gridView.OptionsSelection.MultiSelect = false;
            gridView.OptionsBehavior.Editable = true;

            // MainYn 컬럼을 체크박스로 설정
            GridColumn mainYnColumn = gridView.Columns["MainYn"];
            if (mainYnColumn != null)
            {
                // RepositoryItemCheckEdit 생성 및 설정
                RepositoryItemCheckEdit checkEdit = new RepositoryItemCheckEdit();

                mainYnColumn.ColumnEdit = checkEdit;
            }
        }

        private void LoadDemoData()
        {
            try
            {
                if (gridControl == null)
                {
                    return;
                }

                // 데모 데이터 생성
                DataTable dt = new DataTable();
                dt.Columns.Add("DiagCode", typeof(string));
                dt.Columns.Add("DiagName", typeof(string));
                dt.Columns.Add("DiagType", typeof(string));
                dt.Columns.Add("MainYn", typeof(string));

                // 데모 데이터 추가
                dt.Rows.Add("A01", "급성 위염", "주", "Y");
                dt.Rows.Add("K21", "위식도 역류질환", "부", "N");
                dt.Rows.Add("I10", "본태성 고혈압", "부", "N");

                gridControl.DataSource = dt;
                gridControl.RefreshDataSource();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"데모 데이터 로드 중 오류가 발생했습니다: {ex.Message}", "오류",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void LoadDiagCode(string inNumber, string ordDate)
        {
            // 실제로는 DB에서 해당 환자의 진단내역을 조회
            // 데모에서는 inNumber에 따라 다른 데이터를 표시
            LoadDemoData();

            // TODO: 실제 구현시에는 아래와 같이 DB 조회 로직으로 대체
            /*
            string query = @"
                SELECT DiagCode, DiagName, DiagType, MainYn
                FROM Diagnosis
                WHERE InNumber = @InNumber AND OrdDate = @OrdDate
                ORDER BY MainYn DESC, DiagCode";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@InNumber", inNumber),
                new SqlParameter("@OrdDate", ordDate)
            };

            DataTable dt = DatabaseHelper.ExecuteDataTable(query, parameters);
            gridControl.DataSource = dt;
            */
        }

        public bool HasDiagCode()
        {
            if (gridView == null) return false;

            // 주진단(Y)이 있는지 확인
            for (int i = 0; i < gridView.DataRowCount; i++)
            {
                int rowHandle = gridView.GetRowHandle(i);
                if (rowHandle >= 0)
                {
                    object mainYn = gridView.GetRowCellValue(rowHandle, "MainYn");
                    if (mainYn != null && mainYn.ToString() == "Y")
                    {
                        return true;
                    }
                }
            }

            // 주진단이 없으면 일단 진단코드가 하나라도 있는지 확인
            return gridView.DataRowCount > 0;
        }

        public bool SaveData(string inNumber, string ordDate)
        {
            try
            {
                // 실제로는 DB에 저장
                // 데모에서는 항상 성공으로 처리
                if (gridView == null || gridView.DataRowCount == 0)
                {
                    return false;
                }

                // TODO: 실제 구현시에는 아래와 같이 DB 저장 로직으로 대체
                /*
                // 기존 데이터 삭제
                string deleteQuery = "DELETE FROM Diagnosis WHERE InNumber = @InNumber AND OrdDate = @OrdDate";
                SqlParameter[] deleteParams = new SqlParameter[]
                {
                    new SqlParameter("@InNumber", inNumber),
                    new SqlParameter("@OrdDate", ordDate)
                };
                DatabaseHelper.ExecuteNonQuery(deleteQuery, deleteParams);

                // 새로운 데이터 저장
                foreach (DataRow row in GetGridData())
                {
                    string insertQuery = @"
                        INSERT INTO Diagnosis (InNumber, OrdDate, DiagCode, DiagName, DiagType, MainYn)
                        VALUES (@InNumber, @OrdDate, @DiagCode, @DiagName, @DiagType, @MainYn)";

                    SqlParameter[] insertParams = new SqlParameter[]
                    {
                        new SqlParameter("@InNumber", inNumber),
                        new SqlParameter("@OrdDate", ordDate),
                        new SqlParameter("@DiagCode", row["DiagCode"]),
                        new SqlParameter("@DiagName", row["DiagName"]),
                        new SqlParameter("@DiagType", row["DiagType"]),
                        new SqlParameter("@MainYn", row["MainYn"])
                    };

                    DatabaseHelper.ExecuteNonQuery(insertQuery, insertParams);
                }
                */

                return true;
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"진단코드 저장 중 오류가 발생했습니다: {ex.Message}", "오류",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private DataTable GetGridData()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("DiagCode", typeof(string));
            dt.Columns.Add("DiagName", typeof(string));
            dt.Columns.Add("DiagType", typeof(string));
            dt.Columns.Add("MainYn", typeof(string));

            for (int i = 0; i < gridView.DataRowCount; i++)
            {
                int rowHandle = gridView.GetRowHandle(i);
                if (rowHandle >= 0)
                {
                    DataRow row = gridView.GetDataRow(rowHandle);
                    if (row != null)
                    {
                        dt.ImportRow(row);
                    }
                }
            }

            return dt;
        }

        public void Reset()
        {
            try
            {
                // 데이터 초기화
                if (gridControl != null)
                {
                    DataTable dt = gridControl.DataSource as DataTable;
                    if (dt != null)
                    {
                        dt.Clear();
                        gridControl.RefreshDataSource();
                    }
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"데이터 초기화 중 오류가 발생했습니다: {ex.Message}", "오류",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAddDiag_Click(object sender, EventArgs e)
        {
            try
            {
                // 진단코드 추가 팝업 - 간단한 폼으로 변경
                using (var frm = new XtraForm())
                {
                    frm.Text = "진단코드 추가";
                    frm.Size = new Size(400, 300);
                    frm.StartPosition = FormStartPosition.CenterParent;
                    frm.MinimizeBox = false;
                    frm.MaximizeBox = false;

                    // 패널 기반 레이아웃
                    var panel = new Panel()
                    {
                        Dock = DockStyle.Fill,
                        Padding = new Padding(10)
                    };

                    var lblDiagCode = new Label() { Text = "진단코드:", Location = new Point(10, 20), Width = 80 };
                    var txtDiagCode = new TextEdit() { Location = new Point(100, 20), Width = 200 };

                    var lblDiagName = new Label() { Text = "진단명:", Location = new Point(10, 50), Width = 80 };
                    var txtDiagName = new TextEdit() { Location = new Point(100, 50), Width = 200 };

                    var lblDiagType = new Label() { Text = "구분:", Location = new Point(10, 80), Width = 80 };
                    var cmbDiagType = new ComboBoxEdit() { Location = new Point(100, 80), Width = 100 };
                    cmbDiagType.Properties.Items.AddRange(new object[] { "주", "부" });
                    cmbDiagType.SelectedIndex = 0;

                    var lblMainYn = new Label() { Text = "주진단:", Location = new Point(10, 110), Width = 80 };
                    var chkMainYn = new CheckEdit() { Location = new Point(100, 110), Width = 100 };

                    var btnOK = new SimpleButton() { Text = "확인", DialogResult = DialogResult.OK, Location = new Point(100, 150), Width = 80 };
                    var btnCancel = new SimpleButton() { Text = "취소", DialogResult = DialogResult.Cancel, Location = new Point(190, 150), Width = 80 };

                    panel.Controls.AddRange(new Control[] {
                        lblDiagCode, txtDiagCode,
                        lblDiagName, txtDiagName,
                        lblDiagType, cmbDiagType,
                        lblMainYn, chkMainYn,
                        btnOK, btnCancel
                    });

                    frm.Controls.Add(panel);
                    frm.AcceptButton = btnOK;
                    frm.CancelButton = btnCancel;

                    if (frm.ShowDialog() == DialogResult.OK)
                    {
                        if (!string.IsNullOrEmpty(txtDiagCode.Text) && !string.IsNullOrEmpty(txtDiagName.Text))
                        {
                            AddDiagCode(txtDiagCode.Text, txtDiagName.Text,
                                cmbDiagType.SelectedItem?.ToString() ?? "주",
                                chkMainYn.Checked ? "Y" : "N");
                        }
                        else
                        {
                            XtraMessageBox.Show("진단코드와 진단명을 입력해주세요.", "입력오류",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"진단코드 추가 중 오류가 발생했습니다: {ex.Message}", "오류",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AddDiagCode(string diagCode, string diagName, string diagType, string mainYn)
        {
            try
            {
                if (gridControl == null)
                {
                    XtraMessageBox.Show("그리드 컨트롤이 초기화되지 않았습니다.", "오류",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                DataTable dt = gridControl.DataSource as DataTable;
                if (dt == null)
                {
                    dt = new DataTable();
                    dt.Columns.Add("DiagCode", typeof(string));
                    dt.Columns.Add("DiagName", typeof(string));
                    dt.Columns.Add("DiagType", typeof(string));
                    dt.Columns.Add("MainYn", typeof(string));
                    gridControl.DataSource = dt;
                }

                // 중복된 진단코드가 있는지 확인
                foreach (DataRow row in dt.Rows)
                {
                    if (row["DiagCode"].ToString() == diagCode)
                    {
                        XtraMessageBox.Show("이미 존재하는 진단코드입니다.", "중복오류",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }

                // 주진단 체크 시 기존 주진단 해제
                if (mainYn == "Y")
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        row["MainYn"] = "N";
                    }
                }

                dt.Rows.Add(diagCode, diagName, diagType, mainYn);

                // 그리드 새로고침
                gridControl.RefreshDataSource();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"진단코드 추가 중 오류가 발생했습니다: {ex.Message}", "오류",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDeleteDiag_Click(object sender, EventArgs e)
        {
            try
            {
                if (gridView == null)
                {
                    XtraMessageBox.Show("그리드 뷰가 초기화되지 않았습니다.", "오류",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (gridView.FocusedRowHandle < 0)
                {
                    XtraMessageBox.Show("삭제할 진단코드를 선택해주세요.", "선택오류",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // 삭제 확인
                DataRow row = gridView.GetDataRow(gridView.FocusedRowHandle);
                if (row != null)
                {
                    string diagCode = row["DiagCode"].ToString();
                    string diagName = row["DiagName"].ToString();

                    DialogResult result = XtraMessageBox.Show(
                        $"다음 진단코드를 삭제하시겠습니까?\n\n진단코드: {diagCode}\n진단명: {diagName}",
                        "삭제 확인",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        gridView.DeleteSelectedRows();
                        gridControl.RefreshDataSource();
                    }
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"진단코드 삭제 중 오류가 발생했습니다: {ex.Message}", "오류",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void gridView_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                // 더블클릭 시 진단 수정
                if (gridView == null || gridView.FocusedRowHandle < 0)
                {
                    return;
                }

                DataRow row = gridView.GetDataRow(gridView.FocusedRowHandle);
                if (row == null)
                {
                    return;
                }

                string currentDiagCode = row["DiagCode"].ToString();
                string currentDiagName = row["DiagName"].ToString();
                string currentDiagType = row["DiagType"].ToString();
                string currentMainYn = row["MainYn"].ToString();

                // 수정 폼 표시
                using (var frm = new XtraForm())
                {
                    frm.Text = "진단코드 수정";
                    frm.Size = new Size(400, 300);
                    frm.StartPosition = FormStartPosition.CenterParent;
                    frm.MinimizeBox = false;
                    frm.MaximizeBox = false;

                    var panel = new Panel()
                    {
                        Dock = DockStyle.Fill,
                        Padding = new Padding(10)
                    };

                    var lblDiagCode = new Label() { Text = "진단코드:", Location = new Point(10, 20), Width = 80 };
                    var txtDiagCode = new TextEdit() { Location = new Point(100, 20), Width = 200, Text = currentDiagCode, ReadOnly = true };

                    var lblDiagName = new Label() { Text = "진단명:", Location = new Point(10, 50), Width = 80 };
                    var txtDiagName = new TextEdit() { Location = new Point(100, 50), Width = 200, Text = currentDiagName };

                    var lblDiagType = new Label() { Text = "구분:", Location = new Point(10, 80), Width = 80 };
                    var cmbDiagType = new ComboBoxEdit() { Location = new Point(100, 80), Width = 100 };
                    cmbDiagType.Properties.Items.AddRange(new object[] { "주", "부" });
                    cmbDiagType.SelectedItem = currentDiagType;

                    var lblMainYn = new Label() { Text = "주진단:", Location = new Point(10, 110), Width = 80 };
                    var chkMainYn = new CheckEdit() { Location = new Point(100, 110), Width = 100, Checked = currentMainYn == "Y" };

                    var btnOK = new SimpleButton() { Text = "수정", DialogResult = DialogResult.OK, Location = new Point(100, 150), Width = 80 };
                    var btnCancel = new SimpleButton() { Text = "취소", DialogResult = DialogResult.Cancel, Location = new Point(190, 150), Width = 80 };

                    panel.Controls.AddRange(new Control[] {
                        lblDiagCode, txtDiagCode,
                        lblDiagName, txtDiagName,
                        lblDiagType, cmbDiagType,
                        lblMainYn, chkMainYn,
                        btnOK, btnCancel
                    });

                    frm.Controls.Add(panel);
                    frm.AcceptButton = btnOK;
                    frm.CancelButton = btnCancel;

                    if (frm.ShowDialog() == DialogResult.OK)
                    {
                        if (!string.IsNullOrEmpty(txtDiagName.Text))
                        {
                            // 진단명만 수정 가능하도록 함
                            row["DiagName"] = txtDiagName.Text;
                            row["DiagType"] = cmbDiagType.SelectedItem?.ToString() ?? "주";
                            row["MainYn"] = chkMainYn.Checked ? "Y" : "N";

                            // 주진단 체크 시 기존 주진단 해제
                            if (chkMainYn.Checked)
                            {
                                for (int i = 0; i < gridView.DataRowCount; i++)
                                {
                                    if (i != gridView.FocusedRowHandle)
                                    {
                                        gridView.SetRowCellValue(i, "MainYn", "N");
                                    }
                                }
                            }

                            gridControl.RefreshDataSource();
                        }
                        else
                        {
                            XtraMessageBox.Show("진단명을 입력해주세요.", "입력오류",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"진단코드 수정 중 오류가 발생했습니다: {ex.Message}", "오류",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void gridView_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                if (gridView == null) return;

                // 주진단 변경 시 처리
                if (e.Column.FieldName == "MainYn")
                {
                    object newValue = gridView.GetRowCellValue(e.RowHandle, "MainYn");

                    // 체크된 경우에만 다른 행들의 주진단 해제
                    if (newValue != null && newValue.ToString() == "Y")
                    {
                        // 주진단은 하나만 선택 가능
                        for (int i = 0; i < gridView.DataRowCount; i++)
                        {
                            if (i != e.RowHandle && gridView.GetRowHandle(i) >= 0)
                            {
                                gridView.SetRowCellValue(i, "MainYn", "N");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"셀 값 변경 중 오류가 발생했습니다: {ex.Message}", "오류",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
