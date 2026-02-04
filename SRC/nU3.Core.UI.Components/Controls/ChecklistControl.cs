#nullable enable
using System.ComponentModel;
using System.Linq;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraLayout;
using nU3.Core.UI.Components.Events;

namespace nU3.Core.UI.Components.Controls
{
    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(ChecklistControl))]
    public partial class ChecklistControl : XtraUserControl
    {
        private readonly BindingList<ChecklistItem> _items = new();

        public event EventHandler<ChecklistChangedEventArgs>? CheckStateChanged;
        public event EventHandler? AllChecked;
        public event EventHandler? AllUnchecked;

        [Category("Data")]
        public IEnumerable<ChecklistItem> Items => _items.ToList();

        [Category("Data")]
        public IEnumerable<ChecklistItem> CheckedItems => _items.Where(i => i.IsChecked).ToList();

        [Category("Data")]
        public string CheckedItemsAsString
        {
            get => string.Join(", ", _items.Where(i => i.IsChecked).Select(i => i.Text));
        }

        [Category("Behavior")]
        public ChecklistStyle Style { get; set; } = ChecklistStyle.VerticalList;

        [Category("Behavior")]
        public bool ShowButtons { get; set; } = true;

        [Category("Appearance")]
        public int ItemSpacing { get; set; } = 5;

        [Category("Appearance")]
        public string? Title { get; set; }

        public ChecklistControl()
        {
            InitializeComponent();
        }

        public void AddItem(string text, string? value = null, bool isChecked = false)
        {
            var item = new ChecklistItem
            {
                Text = text,
                Value = value ?? text,
                IsChecked = isChecked
            };

            _items.Add(item);
            AddItemControl(item);
        }

        public void AddItems(IEnumerable<string> items)
        {
            foreach (var item in items)
            {
                AddItem(item);
            }
        }

        public void AddItems(IEnumerable<KeyValuePair<string, string>> items)
        {
            foreach (var kvp in items)
            {
                AddItem(kvp.Value, kvp.Key);
            }
        }

        public void RemoveItem(string value)
        {
            var item = _items.FirstOrDefault(i => i.Value == value);
            if (item != null)
            {
                _items.Remove(item);
                RefreshLayout();
            }
        }

        public void ClearItems()
        {
            _items.Clear();
            RefreshLayout();
        }

        public void CheckAll()
        {
            foreach (var item in _items)
            {
                item.IsChecked = true;
            }
            OnAllChecked();
            OnCheckStateChanged();
        }

        public void UncheckAll()
        {
            foreach (var item in _items)
            {
                item.IsChecked = false;
            }
            OnAllUnchecked();
            OnCheckStateChanged();
        }

        public void CheckByValue(string value)
        {
            var item = _items.FirstOrDefault(i => i.Value == value);
            if (item != null)
            {
                item.IsChecked = true;
                OnCheckStateChanged();
            }
        }

        public void UncheckByValue(string value)
        {
            var item = _items.FirstOrDefault(i => i.Value == value);
            if (item != null)
            {
                item.IsChecked = false;
                OnCheckStateChanged();
            }
        }

        public bool IsChecked(string value)
        {
            var item = _items.FirstOrDefault(i => i.Value == value);
            return item?.IsChecked ?? false;
        }

        public List<string> GetCheckedValues()
        {
            return _items.Where(i => i.IsChecked).Select(i => i.Value).ToList();
        }

        private void AddItemControl(ChecklistItem item)
        {
            if (_layoutControl == null) return;

            var layoutGroup = _layoutControl.Root;

            if (Style == ChecklistStyle.VerticalList)
            {
                var layoutItem = layoutGroup.AddItem();
                layoutItem.Control = CreateCheckBox(item);
                layoutItem.TextVisible = false;
            }
            else if (Style == ChecklistStyle.Grid)
            {
                var layoutItem = layoutGroup.AddItem();
                layoutItem.Control = CreateCheckBox(item);
                layoutItem.TextVisible = false;
            }
        }

        private CheckEdit CreateCheckBox(ChecklistItem item)
        {
            var checkEdit = new CheckEdit
            {
                Text = item.Text,
                Checked = item.IsChecked,
                Tag = item
            };

            checkEdit.CheckedChanged += new EventHandler(this.OnCheckBoxChecked);

            return checkEdit;
        }

        private void OnCheckBoxChecked(object? sender, EventArgs e)
        {
            var checkEdit = sender as CheckEdit;
            if (checkEdit != null)
            {
                var item = checkEdit.Tag as ChecklistItem;
                if (item != null)
                {
                    item.IsChecked = checkEdit.Checked;
                    OnCheckStateChanged();
                }
            }
        }

        private void RefreshLayout()
        {
            if (_layoutControl == null) return;

            _layoutControl.Root.Clear();

            foreach (var item in _items)
            {
                AddItemControl(item);
            }

            _layoutControl.PerformLayout();
        }

        private void OnCheckAll(object? sender, EventArgs e)
        {
            CheckAll();
        }

        private void OnUncheckAll(object? sender, EventArgs e)
        {
            UncheckAll();
        }

        private void OnClear(object? sender, EventArgs e)
        {
            ClearItems();
        }

        protected virtual void OnCheckStateChanged()
        {
            CheckStateChanged?.Invoke(this, new ChecklistChangedEventArgs
            {
                CheckedCount = _items.Count(i => i.IsChecked),
                TotalCount = _items.Count,
                CheckedItems = _items.Where(i => i.IsChecked).ToList()
            });
        }

        protected virtual void OnAllChecked()
        {
            AllChecked?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnAllUnchecked()
        {
            AllUnchecked?.Invoke(this, EventArgs.Empty);
        }
    }

    public class ChecklistItem : INotifyPropertyChanged
    {
        private bool _isChecked;
        private string? _text;
        private string? _value;

        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                if (_isChecked != value)
                {
                    _isChecked = value;
                    OnPropertyChanged(nameof(IsChecked));
                }
            }
        }

        public string? Text
        {
            get => _text;
            set
            {
                if (_text != value)
                {
                    _text = value;
                    OnPropertyChanged(nameof(Text));
                }
            }
        }

        public string? Value
        {
            get => _value;
            set
            {
                if (_value != value)
                {
                    _value = value;
                    OnPropertyChanged(nameof(Value));
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public enum ChecklistStyle
    {
        VerticalList,
        Grid
    }
}
