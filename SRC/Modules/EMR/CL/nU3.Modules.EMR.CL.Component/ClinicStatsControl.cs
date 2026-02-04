using System;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;

namespace nU3.Modules.EMR.CL.Component
{
    public partial class ClinicStatsControl : XtraUserControl
    {
        private readonly System.Data.DataTable _statsTable = new System.Data.DataTable();

        public ClinicStatsControl()
        {
            InitializeComponent();
            InitializeStatsTable();
            LoadStatsData();
        }

        private void InitializeStatsTable()
        {
            _statsTable.Columns.Add("Date", typeof(string));
            _statsTable.Columns.Add("PatientCount", typeof(int));
            _statsTable.Columns.Add("VisitType", typeof(string));
            _statsTable.Columns.Add("Status", typeof(string));
        }

        private void LoadStatsData()
        {
            _statsTable.Clear();

            for (int i = 1; i <= 30; i++)
            {
                var date = DateTime.Today.AddDays(-i);
                var patientCount = new System.Random(i).Next(20, 50);
                var visitType = i % 2 == 0 ? "외래" : "응급";

                _statsTable.Rows.Add(
                    date.ToString("yyyy-MM-dd"),
                    patientCount,
                    visitType,
                    "완료"
                );
            }

            if (_gridStats != null)
            {
                _gridStats.DataSource = _statsTable;
                _gridStats.RefreshDataSource();
                _gridViewStats?.BestFitColumns();
            }
        }

        #region 공용 메서드

        public void UpdateStatus(string message, System.Drawing.Color color)
        {
            if (_lblStatus == null) return;

            _lblStatus.Text = $"상태: {message}";
            _lblStatus.Appearance.ForeColor = color;
        }

        public void AddLogMessage(string message)
        {
            if (_txtEventLog == null) return;

            try
            {
                var timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
                var logMessage = $"[{timestamp}] {message}\r\n";

                _txtEventLog.Text += logMessage;

                _txtEventLog.SelectionStart = _txtEventLog.Text.Length;
                _txtEventLog.ScrollToCaret();

                var lines = _txtEventLog.Lines;
                if (lines.Length > 500)
                {
                    var newText = string.Join("\r\n", lines, lines.Length - 500, 500);
                    _txtEventLog.Text = newText;
                }
            }
            catch
            {
            }
        }

        #endregion
    }
}
