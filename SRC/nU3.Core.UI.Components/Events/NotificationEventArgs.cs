namespace nU3.Core.UI.Components.Controls
{
    public class NotificationEventArgs : EventArgs
    {
        public string? Message { get; set; }
        public string? Title { get; set; }
        public NotificationType Type { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
