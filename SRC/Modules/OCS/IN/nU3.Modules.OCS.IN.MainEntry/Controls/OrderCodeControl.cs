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
    public partial class OrderCodeControl : UserControl
    {
        public OrderCodeControl()
        {
            InitializeComponent();
            LoadDemoData();
        }

        public string OrderType { get; set; } = "D"; // 기본값: 약제처방

        private void LoadDemoData()
        {
            // 데모 데이터 생성
            DataTable dt = new DataTable();
            dt.Columns.Add("OrderCode", typeof(string));
            dt.Columns.Add("OrderName", typeof(string));
            dt.Columns.Add("OrderType", typeof(string));
            dt.Columns.Add("Qty", typeof(int));
            dt.Columns.Add("Days", typeof(int));
            dt.Columns.Add("Dose", typeof(string));
            dt.Columns.Add("Route", typeof(string));
            dt.Columns.Add("Frequency", typeof(string));

            // 데모 데이터 추가
            dt.Rows.Add("D001001", "타이레놀정 500mg", "D", 30, 3, "1T", "PO", "TID");
            dt.Rows.Add("D001002", "게보린정", "D", 60, 5, "2T", "PO", "TID");
            dt.Rows.Add("D001003", "스피롤락톤정 25mg", "D", 20, 7, "1T", "PO", "BID");
            dt.Rows.Add("I001001", "생리식염수 500ml", "I", 1, 1, "500ml", "IV", "QD");
            dt.Rows.Add("I001002", "5% 포도당 500ml", "I", 2, 3, "500ml", "IV", "BID");

            gridControl.DataSource = dt;
        }

        public void LoadOrderCode(string inNumber, string ordDate)
        {
            // 실제로는 DB에서 해당 환자의 처방내역을 조회
            LoadDemoData();
        }

        public bool HasOrderCode()
        {
            return gridView?.DataRowCount > 0;
        }

        public bool SaveData(string inNumber, string ordDate)
        {
            // 실제로는 DB에 저장
            // 데모에서는 항상 성공으로 처리
            return true;
        }

        public void DeleteOrder()
        {
            if (gridView != null && gridView.FocusedRowHandle >= 0)
            {
                gridView.DeleteSelectedRows();
            }
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

        private void btnAddOrder_Click(object sender, EventArgs e)
        {
            // 처방 추가 팝업
            XtraMessageBox.Show("처방코드 조회 팝업", "처방추가", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnDeleteOrder_Click(object sender, EventArgs e)
        {
            DeleteOrder();
        }

        private void gridView_DoubleClick(object sender, EventArgs e)
        {
            // 더블클릭 시 처방 수정
            XtraMessageBox.Show("처방수정 팝업", "처방수정", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}