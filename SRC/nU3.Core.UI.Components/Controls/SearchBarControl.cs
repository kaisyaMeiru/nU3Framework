using DevExpress.XtraEditors;
using nU3.Core.UI;
using System.ComponentModel;

namespace nU3.Core.UI.Components.Controls
{
    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(SearchControl))]
    public partial class SearchBarControl : BaseWorkComponent
    {
        public event EventHandler<SearchEventArgs>? Search;
        public event EventHandler? Cleared;

        [Category("Data")]
        public string? SearchText
        {
            get => _searchEdit?.Text;
            set
            {
                if (_searchEdit != null)
                {
                    _searchEdit.Text = value ?? "";
                }
            }
        }

        [Category("Behavior")]
        public SearchMode Mode { get; set; } = SearchMode.Text;

        [Category("Behavior")]
        public bool AutoSearch { get; set; } = false;

        [Category("Behavior")]
        public int AutoSearchDelay { get; set; } = 500;

        [Category("Appearance")]
        public string Placeholder
        {
            get => _searchEdit?.Properties.NullValuePrompt ?? "";
            set
            {
                if (_searchEdit != null)
                {
                    _searchEdit.Properties.NullValuePrompt = value;
                }
            }
        }

        private System.Windows.Forms.Timer? _autoSearchTimer;

        public SearchBarControl()
        {
            InitializeComponent();
            InitializeAutoSearchTimer();
            AttachEventHandlers();
            
            if (_searchTypeCombo?.Properties.Items.Count > 0)
            {
                _searchTypeCombo.SelectedIndex = 0;
            }
        }

        private void InitializeAutoSearchTimer()
        {
            _autoSearchTimer = new System.Windows.Forms.Timer { Interval = AutoSearchDelay };
            _autoSearchTimer.Tick += OnAutoSearchTick;
        }

        private void AttachEventHandlers()
        {
            if (_searchEdit != null)
            {
                _searchEdit.KeyDown += OnSearchEditKeyDown;
            }

            if (_searchButton != null)
            {
                _searchButton.Click += OnSearchButtonClick;
            }

            if (_clearButton != null)
            {
                _clearButton.Click += OnClearButtonClick;
            }
        }

        public void SetSearchTypes(IEnumerable<string> searchTypes)
        {
            if (_searchTypeCombo == null) return;

            _searchTypeCombo.Properties.Items.Clear();
            foreach (var type in searchTypes)
            {
                _searchTypeCombo.Properties.Items.Add(type);
            }
            if (_searchTypeCombo.Properties.Items.Count > 0)
            {
                _searchTypeCombo.SelectedIndex = 0;
            }
        }

        public void PerformSearch()
        {
            if (_searchEdit == null || _searchTypeCombo == null) return;

            var searchTerm = _searchEdit.Text.Trim();
            var searchType = _searchTypeCombo.SelectedItem?.ToString() ?? "전체";

            Search?.Invoke(this, new SearchEventArgs
            {
                SearchTerm = searchTerm,
                SearchType = searchType
            });
        }

        public void Clear()
        {
            if (_searchEdit != null)
            {
                _searchEdit.Text = "";
            }
            if (_searchTypeCombo != null)
            {
                _searchTypeCombo.SelectedIndex = 0;
            }
            _autoSearchTimer?.Stop();
            Cleared?.Invoke(this, EventArgs.Empty);
        }

        private void OnSearchEditKeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                PerformSearch();
                e.Handled = true;
            }
            else if (AutoSearch)
            {
                _autoSearchTimer?.Stop();
                _autoSearchTimer?.Start();
            }
        }

        private void OnAutoSearchTick(object? sender, EventArgs e)
        {
            _autoSearchTimer?.Stop();
            PerformSearch();
        }

        private void OnSearchButtonClick(object? sender, EventArgs e)
        {
            PerformSearch();
        }

        private void OnClearButtonClick(object? sender, EventArgs e)
        {
            Clear();
        }
    }
}
