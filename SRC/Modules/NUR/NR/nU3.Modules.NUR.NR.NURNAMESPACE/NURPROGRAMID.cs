// NOTE: This is a template source file consumed by the VSIX wizard.
// It is NOT meant to compile inside the current solution.

using DevExpress.XtraEditors;
using nU3.Core.Attributes;
using nU3.Core.Events;
using nU3.Core.Logic;
using nU3.Core.UI;
using nU3.Modules.NUR.NR.NURNAMESPACE.Logic;
using System;
using System.Windows.Forms;

namespace nU3.Modules.NUR.NR.NURNAMESPACE
{
    [nU3ProgramInfo(typeof(NURPROGRAMID), "NURPROGRAM", "NURPROGRAMID", "CHILD")]
    public partial class NURPROGRAMID : BaseWorkControl
    {
        private readonly NURPROGRAMIDBizLogic _logic;

        public NURPROGRAMID(IBizLogicFactory logicFactory)
        {
            InitializeComponent();
            _logic = logicFactory.Create<NURPROGRAMIDBizLogic>();                     
        }

        public NURPROGRAMID() { InitializeComponent(); }

        private void patientListControl1_PatientSelected(object sender, nU3.Core.UI.Components.Events.PatientSelectedEventArgs e)
        {
            if(e.Patient == null)
            {
                return;
            }

            this.patientInfoControl1.SetPatientInfo(e.Patient);

            this.Context.CurrentPatient = e.Patient;
            this.Context.CurrentExam = null;
            
            //this.EventBus.GetEvent<NavigationRequestEvent>().Publish(new NavigationRequestEventPayload() { Context = Context.Clone(), Source = this.ProgramID, TargetScreenId = "EMR_IN_00002" });

        }

    }
}
