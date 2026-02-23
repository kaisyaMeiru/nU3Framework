using System;
using System.Windows.Forms;
using nU3.Core.Attributes;
using nU3.Core.UI;
using nU3.Modules.EMR.Common.Models;

namespace nU3.Modules.EMR.CO.EmrCommon
{
    [nU3ProgramInfo(typeof(EMR_CO_0001), "공통 모듈 테스트화면1", "EMR_CO_0001", "CHILD")]
    public partial class EMR_CO_0001 : BaseWorkControl
    {


        public EMR_CO_0001()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // 공통 컨트롤 이벤트 구독
            searchControl.PatientSelected += SearchControl_PatientSelected;
        }

        private void SearchControl_PatientSelected(object? sender, PatientSummaryDto e)
        {
            string message = $"[Screen 1] 환자 선택 이벤트 수신:\r\n" +
                             $"- 이름: {e.PatientName}\r\n" +
                             $"- ID: {e.PatientId}\r\n" +
                             $"- 나이: {e.Age}\r\n" +
                             $"- 진료과: {e.DepartmentCode}";

            memoLogs.Text += message + "\r\n\r\n";
        }
    }
}
