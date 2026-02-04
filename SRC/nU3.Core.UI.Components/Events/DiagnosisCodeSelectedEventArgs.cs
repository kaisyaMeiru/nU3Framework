namespace nU3.Core.UI.Components.Events
{
    public class DiagnosisCodeSelectedEventArgs : EventArgs
    {
        public Models.DiagnosisCode? Code { get; set; }
        public string Action { get; set; } = "";
    }
}
