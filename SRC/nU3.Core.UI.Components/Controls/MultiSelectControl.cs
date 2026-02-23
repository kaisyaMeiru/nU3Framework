using DevExpress.XtraEditors;
using nU3.Core.UI;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using System.ComponentModel;

namespace nU3.Core.UI.Components.Controls
{
    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(MultiSelectControl))]
    public partial class MultiSelectControl : BaseWorkComponent
    {
        private nU3.Core.UI.Controls.nU3GridControl? _gridControl;
        private nU3.Core.UI.Controls.nU3GridView? _gridView;
        private nU3.Core.UI.Controls.nU3SearchControl? _searchControl;
        private nU3.Core.UI.Controls.nU3PanelControl? _headerPanel;
        private nU3.Core.UI.Controls.nU3PanelControl? _selectedPanel;
        private nU3.Core.UI.Controls.nU3LabelControl? _selectedLabel;

        private readonly BindingList<SelectableItem> _allItems = new();
        private readonly BindingList<SelectableItem> _selectedItems = new();

        public event EventHandler? SelectionChanged;

        [Category("Data")]
        public IEnumerable<string> SelectedItems
        {
            get => _selectedItems.Where(i => i.IsSelected).Select(i => i.DisplayText);
        }

        [Category("Data")]
        public string? Delimiter { get; set; } = ",";

        [Category("Behavior")]
        public int ChipMaxWidth { get; set; } = 200;

        [Category("Behavior")]
        public bool AllowDuplicates { get; set; } = false;

        public MultiSelectControl()
        {
            InitializeComponent();
            AttachEventHandlers();
        }

        private void AttachEventHandlers()
        {
            if (_gridView != null)
            {
                _gridView.Click += OnGridClick;
                _gridView.CellValueChanged += OnCellValueChanged;
            }
        }
        public void LoadItems(IEnumerable<string> items)
        {
            _allItems.Clear();
            _selectedItems.Clear();

            foreach (var item in items)
            {
                var selectableItem = new SelectableItem
                {
                    DisplayText = item,
                    Value = item,
                    IsSelected = false
                };
                _allItems.Add(selectableItem);
            }
        }

        public void LoadItems(IEnumerable<KeyValuePair<string, string>> items)
        {
            _allItems.Clear();
            _selectedItems.Clear();

            foreach (var kvp in items)
            {
                var selectableItem = new SelectableItem
                {
                    DisplayText = kvp.Value,
                    Value = kvp.Key,
                    IsSelected = false
                };
                _allItems.Add(selectableItem);
            }
        }

        public void SelectItems(IEnumerable<string> values)
        {
            foreach (var item in _allItems)
            {
                if (values.Contains(item.Value))
                {
                    item.IsSelected = true;
                }
            }
            UpdateSelectedChips();
            OnSelectionChanged();
        }

        public void ClearSelection()
        {
            foreach (var item in _allItems)
            {
                item.IsSelected = false;
            }
            UpdateSelectedChips();
            OnSelectionChanged();
        }

        public string GetSelectedValuesAsString()
        {
            return string.Join(Delimiter, SelectedItems);
        }

        private void UpdateSelectedChips()
        {
            if (_selectedPanel == null) return;

            _selectedPanel.Controls.Clear();

            int x = 10;
            int y = 10;
            int maxHeight = 0;

            foreach (var item in _allItems.Where(i => i.IsSelected))
            {
                var chip = CreateChip(item);
                if (chip == null) continue;

                chip.Width = Math.Min(chip.PreferredSize.Width, ChipMaxWidth);

                if (x + chip.Width > _selectedPanel.Width - 10)
                {
                    x = 10;
                    y += maxHeight + 5;
                    maxHeight = 0;
                }

                chip.Location = new System.Drawing.Point(x, y);
                chip.Tag = item;
                chip.Click += OnChipClick;

                _selectedPanel.Controls.Add(chip);

                x += chip.Width + 5;
                maxHeight = Math.Max(maxHeight, chip.Height);
            }
        }

        private nU3.Core.UI.Controls.nU3SimpleButton? CreateChip(SelectableItem item)
        {
            var chip = new nU3.Core.UI.Controls.nU3SimpleButton
            {
                Text = item.DisplayText,
                ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Default,
                AllowFocus = false,
                Appearance = { FontSizeDelta = -1 }
            };

            var textSize = TextRenderer.MeasureText(item.DisplayText, chip.Appearance.GetFont());
            chip.Width = textSize.Width + 30;

            return chip;
        }

        private void OnGridClick(object? sender, EventArgs e)
        {
            if (_gridView == null) return;

            var hitInfo = _gridView.CalcHitInfo(_gridView.GridControl.PointToClient(Control.MousePosition));
            if (hitInfo.InColumn && hitInfo.Column.FieldName == "IsSelected")
            {
                UpdateSelectedChips();
                OnSelectionChanged();
            }
        }

        private void OnCellValueChanged(object? sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (e.Column.FieldName == "IsSelected")
            {
                UpdateSelectedChips();
                OnSelectionChanged();
            }
        }

        private void OnChipClick(object? sender, EventArgs e)
        {
            if (sender is nU3.Core.UI.Controls.nU3SimpleButton chip && chip.Tag is SelectableItem item)
            {
                item.IsSelected = false;
                _selectedPanel?.Controls.Remove(chip);
                chip.Dispose();
                OnSelectionChanged();
            }
        }

        protected virtual void OnSelectionChanged()
        {
            SelectionChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
