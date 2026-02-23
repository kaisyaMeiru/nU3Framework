using System;
using System.Windows.Forms;
using nU3.Core.Attributes;
using nU3.Core.UI;
using nU3.Modules.EMR.Common.Models;

namespace nU3.Modules.EMR.CO.EmrCommon2
{
    [nU3ProgramInfo(typeof(EMR_CO_0002), "공통 모듈 테스트화면2", "EMR_CO_0002", "CHILD")]
    public partial class EMR_CO_0002 : BaseWorkControl
    {


        public EMR_CO_0002()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);            

            // 공통 컨트롤 이벤트 구독
            searchControl.PatientSelected += SearchControl_PatientSelected;
        }

        private void SearchControl_PatientSelected(object? sender, EmrPatientSummaryDto e)
        {
            txtResult.Text = $"[Screen 2 - 간략보기] {e.PatientName} ({e.Gender}/{e.Age})";
        }
    }
}
