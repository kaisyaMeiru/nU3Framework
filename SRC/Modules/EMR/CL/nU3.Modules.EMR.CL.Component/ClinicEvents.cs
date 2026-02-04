using System;
using nU3.Core.Events;
using nU3.Models;

namespace nU3.Modules.EMR.CL.Component
{
    // EventBus events and payloads shared across clinic controls

    public class PatientSelectedEvent : PubSubEvent { }

    public class PatientSelectedEventPayload
    {
        public PatientInfoDto? Patient { get; set; }
        public string Source { get; set; } = string.Empty;
    }

    public class VisitInfoUpdatedEvent : PubSubEvent { }

    public class VisitInfoUpdatedEventPayload
    {
        public string VisitId { get; set; } = string.Empty;
        public string PatientId { get; set; } = string.Empty;
        public string VisitType { get; set; } = string.Empty;
        public DateTime? VisitDate { get; set; }
        public string Source { get; set; } = string.Empty;
    }
}
