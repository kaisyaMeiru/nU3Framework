using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms;
using nU3.Core.Attributes;
using nU3.Core.Context;
using nU3.Core.Events;
using nU3.Core.Events.Contracts;
using nU3.Core.Interfaces;
using nU3.Core.Logic;
using nU3.Core.UI;
using nU3.Core.UI.Controls;
using nU3.Core.UI;
using nU3.Core.Attributes;
using nU3.Core.Logic;
using nU3.Models;
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

        public $controlclassname$() { InitializeComponent();}


        public override void InitializeContext(WorkContext context)
        {
            base.InitializeContext(context);
            // EventBus is not assigned to the shell.
            if (EventBus == null)
                LogWarning("EventBus is not assigned.");
            else
                EnableEventListening();
        }

        private void EnableEventListening()
        {
            // 구현 필요
        }

        #region Event Publishing
        /// <summary>
        /// Publish patient selected event (send to other modules)
        /// </summary>
        private void PublishPatientSelected(PatientInfoDto patient)
        {
            try
            {
                // Send event to other modules via EventBus
                EventBus?.GetEvent<nU3.Core.Events.Contracts.PatientSelectedEvent>()
                    .Publish(new PatientSelectedEventPayload
                    {
                        Patient = patient,
                        Source = ProgramID
                    });

                LogInfo($"PatientSelectedEvent occurred: {patient.PatientName} ({patient.PatientId})");
                LogAudit("Read", "Patient", patient.PatientId,
                    $"Patient selected and broadcasted to other modules");
            }
            catch (Exception ex)
            {
                LogError($"Error publishing patient selected event", ex);
            }
        }
        #endregion

        #region Lifecycle

        protected override void OnScreenActivated()
        {
            base.OnScreenActivated();
            LogInfo("PatientListControl activated");
        }

        protected override void OnScreenDeactivated()
        {
            base.OnScreenDeactivated();
            LogInfo("PatientListControl deactivated");
        }

        protected override void OnContextInitialized(WorkContext oldContext, WorkContext newContext)
        {
            base.OnContextInitialized(oldContext, newContext);
            LogInfo("Context initialized");
        }

        protected override void OnContextChanged(WorkContext oldContext, WorkContext newContext)
        {
            base.OnContextChanged(oldContext, newContext);
            LogInfo("Context changed");
        }

        protected override void OnReleaseResources()
        {
            base.OnReleaseResources();

            // Release event handlers (if controls exist)
                // if (gridView != null) ... (Removed non-existent controls)

            LogInfo("Resources released");
        }

        #endregion
    }
}
