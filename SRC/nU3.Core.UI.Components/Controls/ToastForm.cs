using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraLayout;

namespace nU3.Core.UI.Components.Controls
{
    public class ToastForm : DevExpress.XtraEditors.XtraForm
    {
        private System.Windows.Forms.Timer? _closeTimer;

        public string? Title { get; set; }
        public string? Message { get; set; }
        public NotificationType Type { get; set; }
        public int Duration { get; set; }

        public ToastForm()
        {
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            ShowInTaskbar = false;
            TopMost = true;
            Size = new System.Drawing.Size(450, 250);
            StartPosition = FormStartPosition.Manual;
            BackColor = GetBackgroundColor(NotificationType.Info);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            var layoutControl = new LayoutControl
            {
                Dock = DockStyle.Fill,
                Parent = this
            };

            var layoutGroup = new LayoutControlGroup();
            layoutControl.Root.AddItem(layoutGroup);

            var titleItem = layoutGroup.AddItem();
            var titleEdit = new TextEdit
            {
                Text = Title ?? "",
                Properties = { ReadOnly = true, BorderStyle = BorderStyles.NoBorder }
            };
            titleEdit.Properties.Appearance.ForeColor = GetTitleColor(Type);
            titleEdit.Properties.Appearance.Font = new System.Drawing.Font(titleEdit.Properties.Appearance.Font ?? new System.Drawing.Font("Segoe UI", 9F), System.Drawing.FontStyle.Bold);
            titleItem.Control = titleEdit;
            titleItem.TextVisible = false;

            var messageItem = layoutGroup.AddItem();
            var memoEdit = new MemoEdit
            {
                Text = Message ?? "",
                Properties = { ReadOnly = true, BorderStyle = BorderStyles.NoBorder }
            };
            memoEdit.Properties.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            memoEdit.Height = 100;
            messageItem.Control = memoEdit;
            messageItem.TextVisible = false;

            BackColor = GetBackgroundColor(Type);

            if (Duration > 0)
            {
                _closeTimer = new System.Windows.Forms.Timer { Interval = Duration };
                _closeTimer.Tick += OnCloseTimerTick;
                _closeTimer.Start();
            }
        }

        private System.Drawing.Color GetBackgroundColor(NotificationType type)
        {
            return type switch
            {
                NotificationType.Success => System.Drawing.Color.FromArgb(46, 204, 113),
                NotificationType.Warning => System.Drawing.Color.FromArgb(241, 196, 15),
                NotificationType.Error => System.Drawing.Color.FromArgb(231, 76, 60),
                _ => System.Drawing.Color.FromArgb(52, 152, 219)
            };
        }

        private System.Drawing.Color GetTitleColor(NotificationType type)
        {
            return System.Drawing.Color.White;
        }

        private void OnCloseTimerTick(object? sender, EventArgs e)
        {
            Close();
        }

        private void InitializeComponent()
        {

        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _closeTimer?.Stop();
                _closeTimer?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
