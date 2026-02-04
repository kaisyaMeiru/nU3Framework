namespace nU3.Core.UI.Components.Controls
{
    public class MedicationPrescription
    {
        public Medication Medication { get; set; } = new();
        public string Dosage { get; set; } = "";
        public int Quantity { get; set; } = 1;
        public string? Instructions { get; set; }
    }
}
