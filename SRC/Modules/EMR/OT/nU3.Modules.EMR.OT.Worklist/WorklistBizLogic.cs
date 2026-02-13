using System;
using System.Data;
using System.Threading.Tasks;
using nU3.Core.Interfaces;
using nU3.Core.Logic;

namespace nU3.Modules.EMR.OT.Worklist
{
    /// <summary>
    /// Business Logic for OT Worklist.
    /// Handles DB queries and file operations independent of UI.
    /// </summary>
    public class WorklistBizLogic : BaseBizLogic
    {
        public WorklistBizLogic(IDBAccessService db, IFileTransferService file) : base(db, file)
        {
        }

        public async Task<DataTable> SearchWorklistAsync()
        {

            // Original DB query commented out and replaced with hard-coded sample data for UI/testing
            // // Real business logic (SQL) goes here
            // // In a real app, this might call a stored procedure like 'SP_EMR_OT_WORKLIST_S'
            // string sql = "SELECT * FROM SYS_MODULE_MST"; 
            // 
            // return await _db.ExecuteDataTableAsync(sql);

            var dt = new DataTable();
            dt.Columns.Add("PatientId", typeof(string));
            dt.Columns.Add("PatientName", typeof(string));
            dt.Columns.Add("Procedure", typeof(string));
            dt.Columns.Add("ScheduledTime", typeof(DateTime));
            dt.Columns.Add("Status", typeof(string));

            dt.Rows.Add("P001", "Kim, John", "Appendectomy", DateTime.Today.AddHours(9), "Scheduled");
            dt.Rows.Add("P002", "Lee, Anna", "Cholecystectomy", DateTime.Today.AddHours(10).AddMinutes(30), "InProgress");
            dt.Rows.Add("P003", "Park, Min", "Hernia Repair", DateTime.Today.AddHours(13), "Waiting");

            return await Task.FromResult(dt);
        }

        public async Task<string> GetServerHomeDirAsync()
        {
            return await _file.GetHomeDirectoryAsync();
        }
    }
}
