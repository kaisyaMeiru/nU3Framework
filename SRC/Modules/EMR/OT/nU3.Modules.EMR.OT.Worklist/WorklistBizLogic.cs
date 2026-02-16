using System;
using System.Data;
using System.Threading.Tasks;
using nU3.Core.Interfaces;
using nU3.Core.Logic;

namespace nU3.Modules.EMR.OT.Worklist
{    
    public class WorklistBizLogic : BaseBizLogic
    {
        public WorklistBizLogic(IDBAccessService db, IFileTransferService file) : base(db, file)
        {
        }

        public async Task<DataTable> SearchWorklistAsync()
        {
            var dt = new DataTable();
            dt.Columns.Add("PatientId", typeof(string));
            dt.Columns.Add("PatientName", typeof(string));
            dt.Columns.Add("Procedure", typeof(string));
            dt.Columns.Add("ScheduledTime", typeof(DateTime));
            dt.Columns.Add("Status", typeof(string));

            dt.Rows.Add("P001", "김철수", "맹장수술", DateTime.Today.AddHours(9), "예약됨");
            dt.Rows.Add("P002", "이영희", "담낭절제술", DateTime.Today.AddHours(10).AddMinutes(30), "진행중");
            dt.Rows.Add("P003", "박민수", "탈장수술", DateTime.Today.AddHours(13), "대기중");
            dt.Rows.Add("P004", "최수정", "심장수술", DateTime.Today.AddHours(14), "완료");
            dt.Rows.Add("P005", "정우성", "위절제술", DateTime.Today.AddHours(15), "예약됨");
            dt.Rows.Add("P006", "한지민", "간이식", DateTime.Today.AddHours(16), "진행중");
            dt.Rows.Add("P007", "이준기", "신장수술", DateTime.Today.AddHours(17), "대기중");
            dt.Rows.Add("P008", "문채원", "폐수술", DateTime.Today.AddHours(18), "완료");
            dt.Rows.Add("P009", "박보검", "담낭절제술", DateTime.Today.AddHours(19), "예약됨");
            dt.Rows.Add("P010", "김지원", "맹장수술", DateTime.Today.AddHours(20), "진행중");
            dt.Rows.Add("P011", "이동욱", "탈장수술", DateTime.Today.AddHours(21), "대기중");
            dt.Rows.Add("P012", "서강준", "심장수술", DateTime.Today.AddHours(22), "완료");
            dt.Rows.Add("P013", "김유정", "위절제술", DateTime.Today.AddHours(23), "예약됨");
            dt.Rows.Add("P014", "유아인", "간이식", DateTime.Today.AddHours(8), "진행중");
            dt.Rows.Add("P015", "박서준", "신장수술", DateTime.Today.AddHours(7), "대기중");
            dt.Rows.Add("P016", "수지", "폐수술", DateTime.Today.AddHours(6), "완료");
            dt.Rows.Add("P017", "이성경", "담낭절제술", DateTime.Today.AddHours(5), "예약됨");
            dt.Rows.Add("P018", "남주혁", "맹장수술", DateTime.Today.AddHours(4), "진행중");
            dt.Rows.Add("P019", "신세경", "탈장수술", DateTime.Today.AddHours(3), "대기중");
            dt.Rows.Add("P020", "조인성", "심장수술", DateTime.Today.AddHours(2), "완료");

            return await Task.FromResult(dt);
        }

        public async Task<string> GetServerHomeDirAsync()
        {
            return await _file.GetHomeDirectoryAsync();
        }
    }
}
