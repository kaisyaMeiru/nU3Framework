using DevExpress.XtraEditors;
using nU3.Core.UI;
using DevExpress.XtraGrid.Views.Grid;
using System.ComponentModel;

namespace nU3.Core.UI.Components.Controls
{
    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(DiagnosisCodeControl))]
    public partial class DiagnosisCodeControl : BaseWorkComponent
    {
        public event EventHandler<Events.DiagnosisCodeSelectedEventArgs>? DiagnosisCodeSelected;

        [Category("Data")]
        public IEnumerable<Models.DiagnosisCode> SelectedCodes => _selectedCodes?.ToList() ?? new List<Models.DiagnosisCode>();

        [Category("Behavior")]
        public int SearchLimit { get; set; } = 100;

        [Category("Behavior")]
        public bool AllowMultipleSelection { get; set; } = true;

        public DiagnosisCodeControl()
        {
            InitializeComponent();
            
            if (!DesignMode)
            {
                _availableCodes = new BindingList<Models.DiagnosisCode>();
                _selectedCodes = new BindingList<Models.DiagnosisCode>();
                
                if (_codeGrid != null)
                {
                    _codeGrid.DataSource = _availableCodes;
                }
            }
            
            AttachEventHandlers();
            SetupSelectedCodeGridColumns();
        }

        private void AttachEventHandlers()
        {
            if (_addButton != null)
                _addButton.Click += OnAddManualCode;
            
            if (_removeButton != null)
                _removeButton.Click += OnRemoveCode;
            
            if (_clearButton != null)
                _clearButton.Click += OnClearCodes;
            
            if (_codeView != null)
            {
                _codeView.OptionsSelection.MultiSelect = !AllowMultipleSelection;
                _codeView.DoubleClick += OnCodeDoubleClick;
            }
        }

        private void SetupSelectedCodeGridColumns()
        {
            if (_selectedCodeView == null) return;

            _selectedCodeView.Columns.Clear();

            var colCode = new nU3.Core.UI.Controls.nU3GridColumn
            {
                FieldName = "Code",
                Caption = "코드",
                VisibleIndex = 0,
                Width = 100
            };

            var colName = new nU3.Core.UI.Controls.nU3GridColumn
            {
                FieldName = "Name",
                Caption = "진단명",
                VisibleIndex = 1
            };

            var colCategory = new nU3.Core.UI.Controls.nU3GridColumn
            {
                FieldName = "Category",
                Caption = "카테고리",
                VisibleIndex = 2,
                Width = 150
            };

            _selectedCodeView.Columns.AddRange(new[] { colCode, colName, colCategory });
            _selectedCodeGrid!.DataSource = _selectedCodes;
        }

        public void LoadCodes(IEnumerable<Models.DiagnosisCode> codes)
        {
            if (_availableCodes == null) return;
            
            _availableCodes.Clear();
            foreach (var code in codes)
            {
                _availableCodes.Add(code);
            }
        }

        public void SearchCodes(string searchTerm)
        {
            if (_availableCodes == null || _codeView == null) return;
            
            var filtered = _availableCodes
                .Where(c => c.Code.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                           c.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                .Take(SearchLimit)
                .ToList();

            if (_codeView != null)
            {
                _codeView.BeginUpdate();
                _codeView.ClearSelection();
                _codeView.EndUpdate();

                foreach (var code in filtered)
                {
                    var rowHandle = _codeView.LocateByValue("Code", code.Code);
                    if (rowHandle >= 0)
                    {
                        _codeView.SelectRow(rowHandle);
                    }
                }
            }
        }

        public void AddCode(Models.DiagnosisCode code)
        {
            if (_selectedCodes == null) return;
            
            if (_selectedCodes.Any(c => c.Code == code.Code))
            {
                return;
            }

            _selectedCodes.Add(code);
            OnDiagnosisCodeSelected(new Events.DiagnosisCodeSelectedEventArgs
            {
                Code = code,
                Action = "Added"
            });
        }

        public void RemoveCode(Models.DiagnosisCode code)
        {
            if (_selectedCodes == null) return;
            
            var existing = _selectedCodes.FirstOrDefault(c => c.Code == code.Code);
            if (existing != null)
            {
                _selectedCodes.Remove(existing);
                OnDiagnosisCodeSelected(new Events.DiagnosisCodeSelectedEventArgs
                {
                    Code = code,
                    Action = "Removed"
                });
            }
        }

        public void ClearCodes()
        {
            if (_selectedCodes == null) return;
            
            _selectedCodes.Clear();
            OnDiagnosisCodeSelected(new Events.DiagnosisCodeSelectedEventArgs
            {
                Code = null,
                Action = "Cleared"
            });
        }

        public List<Models.DiagnosisCode> GetSelectedCodes()
        {
            return _selectedCodes?.ToList() ?? new List<Models.DiagnosisCode>();
        }

        public string GetSelectedCodesAsString(string delimiter = ",")
        {
            return _selectedCodes != null 
                ? string.Join(delimiter, _selectedCodes.Select(c => c.Code))
                : string.Empty;
        }

        private void OnAddManualCode(object? sender, EventArgs e)
        {
            var code = _codeEdit?.Text.Trim();
            var name = _nameEdit?.Text.Trim();

            if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(name))
            {
                XtraMessageBox.Show("코드와 진단명을 입력하세요.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var diagnosisCode = new Models.DiagnosisCode
            {
                Code = code,
                Name = name,
                Category = "직접입력"
            };

            AddCode(diagnosisCode);

            if (_codeEdit != null) _codeEdit.Text = "";
            if (_nameEdit != null) _nameEdit.Text = "";
        }

        private void OnRemoveCode(object? sender, EventArgs e)
        {
            if (_selectedCodeView != null && _selectedCodeView.FocusedRowHandle >= 0)
            {
                var code = _selectedCodeView.GetFocusedRow() as Models.DiagnosisCode;
                if (code != null)
                {
                    RemoveCode(code);
                }
            }
        }

        private void OnClearCodes(object? sender, EventArgs e)
        {
            ClearCodes();
        }

        private void OnCodeDoubleClick(object? sender, EventArgs e)
        {
            if (_codeView != null && _codeView.FocusedRowHandle >= 0)
            {
                var code = _codeView.GetFocusedRow() as Models.DiagnosisCode;
                if (code != null)
                {
                    AddCode(code);
                }
            }
        }

        protected virtual void OnDiagnosisCodeSelected(Events.DiagnosisCodeSelectedEventArgs e)
        {
            DiagnosisCodeSelected?.Invoke(this, e);
        }
    }
}
