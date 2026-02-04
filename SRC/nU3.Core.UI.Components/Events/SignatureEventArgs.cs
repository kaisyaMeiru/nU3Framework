namespace nU3.Core.UI.Components.Controls
{
    public class SignatureEventArgs : EventArgs
    {
        public byte[] SignatureData { get; set; } = Array.Empty<byte>();
        public string? Comment { get; set; }
        public string? SignerName { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
