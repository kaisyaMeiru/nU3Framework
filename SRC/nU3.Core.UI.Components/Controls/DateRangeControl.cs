using DevExpress.XtraEditors;
using nU3.Core.UI;
using DevExpress.XtraEditors.Controls;
using nU3.Models;
using System.ComponentModel;

namespace nU3.Core.UI.Components.Controls
{
    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(DateRangeControl))]
    public partial class DateRangeControl : BaseWorkComponent
    {
        public event EventHandler? DateRangeChanged;

        private bool _suppressDateRangeChanged;

        [Category("Data")]
        public DateTime? StartDate
        {
            get => _startDateEdit?.DateTime;
            set
            {
                if (_startDateEdit != null)
                {
                    _startDateEdit.DateTime = value ?? DateTime.Today;
                    OnDateRangeChanged();
                }
            }
        }

        [Category("Data")]
        public DateTime? EndDate
        {
            get => _endDateEdit?.DateTime;
            set
            {
                if (_endDateEdit != null)
                {
                    _endDateEdit.DateTime = value ?? DateTime.Today;
                    OnDateRangeChanged();
                }
            }
        }

        [Category("Behavior")]
        public bool AllowNull { get; set; } = false;

        [Category("Behavior")]
        public bool ShowQuickButtons { get; set; } = true;

        public DateRangeControl()
        {
            InitializeComponent();
            AttachEventHandlers();
            SetQuickButtonsVisibility();
            SetDateRange(DateTime.Today.AddDays(-30), DateTime.Today);
        }

        private void AttachEventHandlers()
        {
            if (_startDateEdit != null)
                _startDateEdit.EditValueChanged += OnDateChanged;
            
            if (_endDateEdit != null)
                _endDateEdit.EditValueChanged += OnDateChanged;
            
            if (_todayButton != null)
                _todayButton.Click += OnToday;
            
            if (_thisWeekButton != null)
                _thisWeekButton.Click += OnThisWeek;
            
            if (_thisMonthButton != null)
                _thisMonthButton.Click += OnThisMonth;
            
            if (_clearButton != null)
                _clearButton.Click += OnClear;
        }

        private void SetQuickButtonsVisibility()
        {
            if (_todayButton != null)
                _todayButton.Visible = ShowQuickButtons;
            
            if (_thisWeekButton != null)
                _thisWeekButton.Visible = ShowQuickButtons;
            
            if (_thisMonthButton != null)
                _thisMonthButton.Visible = ShowQuickButtons;
            
            if (_clearButton != null)
                _clearButton.Visible = ShowQuickButtons;
        }

        public void SetDateRange(DateTime? start, DateTime? end)
        {
            bool changed = false;
            try
            {
                _suppressDateRangeChanged = true;
                if (_startDateEdit != null && start.HasValue && _startDateEdit.DateTime != start.Value)
                {
                    _startDateEdit.DateTime = start.Value;
                    changed = true;
                }
                if (_endDateEdit != null && end.HasValue && _endDateEdit.DateTime != end.Value)
                {
                    _endDateEdit.DateTime = end.Value;
                    changed = true;
                }
            }
            finally
            {
                _suppressDateRangeChanged = false;
            }

            if (changed)
                OnDateRangeChanged();
        }

        public nU3.Models.DateRange GetDateRange()
        {
            return new nU3.Models.DateRange
            {
                StartDate = _startDateEdit?.DateTime,
                EndDate = _endDateEdit?.DateTime
            };
        }

        public void Clear()
        {
            if (AllowNull)
            {
                if (_startDateEdit != null)
                {
                    _startDateEdit.EditValue = null;
                }
                if (_endDateEdit != null)
                {
                    _endDateEdit.EditValue = null;
                }
            }
            else
            {
                SetDateRange(DateTime.Today, DateTime.Today);
            }
            OnDateRangeChanged();
        }

        private void OnDateChanged(object? sender, EventArgs e)
        {
            OnDateRangeChanged();
        }

        private void OnToday(object? sender, EventArgs e)
        {
            SetDateRange(DateTime.Today, DateTime.Today);
        }

        private void OnThisWeek(object? sender, EventArgs e)
        {
            var today = DateTime.Today;
            var dayOfWeek = (int)today.DayOfWeek;
            var monday = today.AddDays(-(dayOfWeek == 0 ? 6 : dayOfWeek - 1));
            var sunday = monday.AddDays(6);
            SetDateRange(monday, sunday);
        }

        private void OnThisMonth(object? sender, EventArgs e)
        {
            var today = DateTime.Today;
            var firstDay = new DateTime(today.Year, today.Month, 1);
            var lastDay = firstDay.AddMonths(1).AddDays(-1);
            SetDateRange(firstDay, lastDay);
        }

        private void OnClear(object? sender, EventArgs e)
        {
            Clear();
        }

        protected virtual void OnDateRangeChanged()
        {
            if (_suppressDateRangeChanged) return;
            DateRangeChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
