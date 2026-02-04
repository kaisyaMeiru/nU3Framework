using System.ComponentModel;

namespace nU3.Core.UI.Components.Controls
{
    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(NotificationControl))]
    public partial class NotificationControl : Component
    {
        private readonly List<ToastForm> _activeToasts = new();
        private readonly Form? _ownerForm;

        [Category("Behavior")]
        public int MaxVisibleToasts { get; set; } = 5;

        [Category("Behavior")]
        public int DefaultDuration { get; set; } = 5000;

        [Category("Behavior")]
        public int Spacing { get; set; } = 10;

        [Category("Appearance")]
        public NotificationPosition Position { get; set; } = NotificationPosition.TopRight;

        public event EventHandler<NotificationEventArgs>? NotificationClicked;
        public event EventHandler? AllNotificationsCleared;

        public NotificationControl()
        {
            InitializeComponent();
        }

        public NotificationControl(IContainer container)
        {
            container.Add(this);
            InitializeComponent();
        }

        public void Show(string message, string title = "")
        {
            Show(message, title, NotificationType.Info);
        }

        public void Show(string message, string title, NotificationType type)
        {
            Show(message, title, type, DefaultDuration);
        }

        public void Show(string message, string title, NotificationType type, int duration)
        {
            var toast = CreateToast(message, title, type, duration);
            ShowToast(toast);
        }

        public void ShowSuccess(string message, string title = "성공")
        {
            Show(message, title, NotificationType.Success);
        }

        public void ShowWarning(string message, string title = "경고")
        {
            Show(message, title, NotificationType.Warning);
        }

        public void ShowError(string message, string title = "오류")
        {
            Show(message, title, NotificationType.Error);
        }

        public void ShowInfo(string message, string title = "정보")
        {
            Show(message, title, NotificationType.Info);
        }

        public void ClearAll()
        {
            var toasts = _activeToasts.ToList();
            foreach (var toast in toasts)
            {
                toast.Close();
            }
            AllNotificationsCleared?.Invoke(this, EventArgs.Empty);
        }

        private ToastForm CreateToast(string message, string title, NotificationType type, int duration)
        {
            var toast = new ToastForm
            {
                Message = message,
                Title = title,
                Type = type,
                Duration = duration,
                Tag = new ToastData
                {
                    Message = message,
                    Title = title,
                    Type = type,
                    Timestamp = DateTime.Now
                }
            };

            toast.Click += OnToastClick;
            toast.Closed += OnToastClosed;

            return toast;
        }

        private void ShowToast(ToastForm toast)
        {
            var position = CalculatePosition(toast);
            toast.StartPosition = FormStartPosition.Manual;
            toast.Location = position;
            toast.Show();

            _activeToasts.Add(toast);

            if (_activeToasts.Count > MaxVisibleToasts)
            {
                var oldest = _activeToasts[0];
                oldest.Close();
            }
        }

        private System.Drawing.Point CalculatePosition(ToastForm toast)
        {
            var workingArea = Screen.PrimaryScreen?.WorkingArea ?? new System.Drawing.Rectangle(0, 0, 1920, 1080);
            int x, y;

            switch (Position)
            {
                case NotificationPosition.TopLeft:
                    x = workingArea.Left + Spacing;
                    y = workingArea.Top + Spacing + (_activeToasts.Count * (toast.Height + Spacing));
                    break;
                case NotificationPosition.TopRight:
                    x = workingArea.Right - toast.Width - Spacing;
                    y = workingArea.Top + Spacing + (_activeToasts.Count * (toast.Height + Spacing));
                    break;
                case NotificationPosition.BottomLeft:
                    x = workingArea.Left + Spacing;
                    y = workingArea.Bottom - toast.Height - Spacing - (_activeToasts.Count * (toast.Height + Spacing));
                    break;
                case NotificationPosition.BottomRight:
                    x = workingArea.Right - toast.Width - Spacing;
                    y = workingArea.Bottom - toast.Height - Spacing - (_activeToasts.Count * (toast.Height + Spacing));
                    break;
                default:
                    x = workingArea.Right - toast.Width - Spacing;
                    y = workingArea.Top + Spacing + (_activeToasts.Count * (toast.Height + Spacing));
                    break;
            }

            return new System.Drawing.Point(x, y);
        }

        private void OnToastClick(object? sender, EventArgs e)
        {
            if (sender is ToastForm toast && toast.Tag is ToastData data)
            {
                NotificationClicked?.Invoke(this, new NotificationEventArgs
                {
                    Message = data.Message,
                    Title = data.Title,
                    Type = data.Type,
                    Timestamp = data.Timestamp
                });
            }
        }

        private void OnToastClosed(object? sender, EventArgs e)
        {
            if (sender is ToastForm toast)
            {
                _activeToasts.Remove(toast);
                ReorderToasts();
            }
        }

        private void ReorderToasts()
        {
            for (int i = 0; i < _activeToasts.Count; i++)
            {
                var toast = _activeToasts[i];
                var position = CalculatePosition(toast);
                toast.Location = position;
            }
        }
    }
}
