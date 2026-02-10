using nU3.Core.Attributes;
using nU3.Core.UI;
using System;
using System.Windows.Forms;
using System.ComponentModel;

namespace nU3.Modules.ADM.AD.ADM_AD_00001
{
    [nU3ProgramInfo(typeof(ADM_AD_00001), "nU3 Framework POC Seeder", nameof(ADM_AD_00001))]
    public partial class ADM_AD_00001 : BaseWorkControl
    {
        public ADM_AD_00001()
        {
            InitializeComponent();
        }

        protected override void OnScreenActivated()
        {
            base.OnScreenActivated();

            // 화면 활성화 시 초기화 로직
            label1.Text = "nU3 Framework POC Seeder - Ready";
        }
        
    }
}
