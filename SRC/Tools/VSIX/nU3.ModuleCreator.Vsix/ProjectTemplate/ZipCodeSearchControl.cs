// NOTE: This is a template source file consumed by the VSIX wizard.
// It is NOT meant to compile inside the current solution.

using System;
using System.Windows.Forms;
using nU3.Core.UI;
using nU3.Core.Attributes;
using nU3.Core.Logic;
using nU3.Modules.$system$.$subsystem$.$namespace$.Logic;

namespace nU3.Modules.$system$.$subsystem$.$namespace$
{
    [nU3ProgramInfo(typeof($controlclassname$), "$programname$", "$programid$", "CHILD")]
    public partial class $controlclassname$ : BaseWorkControl
    {
        private readonly $bizlogicclassname$ _logic;

        public $controlclassname$(IBizLogicFactory logicFactory)
        {
            InitializeComponent();
            _logic = logicFactory.Create<$bizlogicclassname$>();
        }

        public $controlclassname$() { InitializeComponent(); }
    }
}
