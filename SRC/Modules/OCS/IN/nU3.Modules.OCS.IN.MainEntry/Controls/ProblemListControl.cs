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

namespace nU3.Modules.OCS.IN.MainEntry.Controls
{
    public partial class ProblemListControl : UserControl
    {
        public ProblemListControl()
        {
            InitializeComponent();
            LoadDemoData();
        }

        private void LoadDemoData()
        {
            // 데모 데이터 생성
            DataTable dt = new DataTable();
            dt.Columns.Add("ProblemCode", typeof(string));
            dt.Columns.Add("ProblemName", typeof(string));
            dt.Columns.Add("ProblemType", typeof(string));

            // 데모 데이터 추가
            dt.Rows.Add("P001", "상복부 통증", "주관");
            dt.Rows.Add("P002", "오심 및 구토", "주관");
            dt.Rows.Add("P003", "식욕부진", "주관");

            gridControl.DataSource = dt;
        }

        public void LoadProblemList(string inNumber, string ordDate)
        {
            // 실제로는 DB에서 해당 환자의 문제리스트를 조회
            LoadDemoData();
        }

        public bool SaveData(string inNumber, string ordDate)
        {
            // 실제로는 DB에 저장
            // 데모에서는 항상 성공으로 처리
            return true;
        }

        public void Reset()
        {
            // 데이터 초기화
            if (gridView != null && gridControl != null)
            {
                DataTable dt = gridControl.DataSource as DataTable;
                if (dt != null)
                {
                    dt.Clear();
                    gridControl.RefreshDataSource();
                }
            }
        }

        private void btnAddProblem_Click(object sender, EventArgs e)
        {
            // 문제리스트 추가 팝업
            XtraMessageBox.Show("문제리스트 조회 팝업", "문제추가", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnDeleteProblem_Click(object sender, EventArgs e)
        {
            if (gridView != null && gridView.FocusedRowHandle >= 0)
            {
                gridView.DeleteSelectedRows();
            }
        }

        private void gridView_DoubleClick(object sender, EventArgs e)
        {
            // 더블클릭 시 문제리스트 수정
            XtraMessageBox.Show("문제리스트 수정 팝업", "문제수정", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}