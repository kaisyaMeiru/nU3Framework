using nU3.Models;

namespace nU3.Core.UI.Components.Events
{
    public class PatientSelectedEventArgs : EventArgs
    {
        public PatientInfoDto? Patient { get; set; }
        public string Source { get; set; } = "";
    }

    public class PatientSearchingEventArgs : EventArgs
    {
        public string SearchTerm { get; set; } = "";
        public int Offset { get; set; }
        public int Limit { get; set; }
    }

    public class VisitRecordedEventArgs : EventArgs
    {
        public string VisitId { get; set; } = string.Empty;
        public string PatientId { get; set; } = string.Empty;
        public string PatientName { get; set; } = string.Empty;
        public string VisitType { get; set; } = string.Empty;
        public DateTime? VisitDate { get; set; }
    }

    public class ChecklistChangedEventArgs : EventArgs
    {
        public int CheckedCount { get; set; }
        public int TotalCount { get; set; }
        public IEnumerable<object>? CheckedItems { get; set; }
    }
}
