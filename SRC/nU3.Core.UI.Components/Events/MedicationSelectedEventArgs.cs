namespace nU3.Core.UI.Components.Controls
{
    public class MedicationSelectedEventArgs : EventArgs
    {
        public MedicationPrescription? Prescription { get; set; }
        public string Action { get; set; } = "";
    }
}
