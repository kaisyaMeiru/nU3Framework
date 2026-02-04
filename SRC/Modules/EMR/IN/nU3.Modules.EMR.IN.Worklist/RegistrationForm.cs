using System;
using System.Windows.Forms;
using nU3.Core.Attributes;
using nU3.Core.UI;

namespace nU3.Modules.EMR.IN.Worklist
{
    [nU3ProgramInfo(typeof(RegistrationForm), "Outpatient Registration", "EMR_IN_00004", AuthLevel = 1)]
    public partial class RegistrationForm : BaseWorkControl
    {
        public RegistrationForm()
        {
            InitializeComponent();
        }
    }
}
