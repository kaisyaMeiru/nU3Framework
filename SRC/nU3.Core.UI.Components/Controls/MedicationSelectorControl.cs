using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using System.ComponentModel;

namespace nU3.Core.UI.Components.Controls
{
    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(MedicationSelectorControl))]
    public partial class MedicationSelectorControl : XtraUserControl
    {
        private GridControl? _medicationGrid;
        private GridView? _medicationView;
        private GridControl? _selectedMedicationGrid;
        private GridView? _selectedMedicationView;
        private SearchControl? _searchControl;
        private TextEdit? _codeEdit;
        private TextEdit? _nameEdit;
        private TextEdit? _dosageEdit;
        private SpinEdit? _quantityEdit;
        private SimpleButton? _addButton;
        private SimpleButton? _removeButton;
        private SimpleButton? _clearButton;
        private PanelControl? _selectionPanel;
        private LabelControl? _selectedLabel;

        private readonly BindingList<Medication> _medications = new();
        private readonly BindingList<MedicationPrescription> _selectedMedications = new();

        public event EventHandler<MedicationSelectedEventArgs>? MedicationSelected;

        [Category("Data")]
        public IEnumerable<MedicationPrescription> SelectedMedications => _selectedMedications.ToList();

        [Category("Behavior")]
        public int SearchLimit { get; set; } = 100;

        [Category("Behavior")]
        public bool AllowDuplicates { get; set; } = false;

        public MedicationSelectorControl()
        {
            InitializeComponent();
            AttachEventHandlers();
        }

        private void AttachEventHandlers()
        {
            if (_addButton != null)
            {
                _addButton.Click += OnAddManualMedication;
            }

            if (_removeButton != null)
            {
                _removeButton.Click += OnRemoveMedication;
            }

            if (_clearButton != null)
            {
                _clearButton.Click += OnClearMedications;
            }

            if (_medicationView != null)
            {
                _medicationView.DoubleClick += OnMedicationDoubleClick;
            }
        }

        public void LoadMedications(IEnumerable<Medication> medications)
        {
            _medications.Clear();
            foreach (var medication in medications)
            {
                _medications.Add(medication);
            }
        }

        public void SearchMedications(string searchTerm)
        {
            var filtered = _medications
                .Where(m => m.Code.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                           m.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                           m.GenericName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                .Take(SearchLimit)
                .ToList();

            if (_medicationView != null)
            {
                _medicationView.BeginUpdate();
                _medicationView.ClearSelection();
                _medicationView.EndUpdate();

                foreach (var medication in filtered)
                {
                    var rowHandle = _medicationView.LocateByValue("Code", medication.Code);
                    if (rowHandle >= 0)
                    {
                        _medicationView.SelectRow(rowHandle);
                    }
                }
            }
        }

        public void AddMedication(Medication medication, string dosage = "", int quantity = 1)
        {
            if (!AllowDuplicates && _selectedMedications.Any(m => m.Medication.Code == medication.Code))
            {
                return;
            }

            var prescription = new MedicationPrescription
            {
                Medication = medication,
                Dosage = dosage,
                Quantity = quantity
            };

            _selectedMedications.Add(prescription);

            OnMedicationSelected(new MedicationSelectedEventArgs
            {
                Prescription = prescription,
                Action = "Added"
            });
        }

        public void RemoveMedication(MedicationPrescription prescription)
        {
            if (_selectedMedications.Contains(prescription))
            {
                _selectedMedications.Remove(prescription);
                OnMedicationSelected(new MedicationSelectedEventArgs
                {
                    Prescription = prescription,
                    Action = "Removed"
                });
            }
        }

        public void ClearMedications()
        {
            _selectedMedications.Clear();
            OnMedicationSelected(new MedicationSelectedEventArgs
            {
                Prescription = null,
                Action = "Cleared"
            });
        }

        public List<MedicationPrescription> GetSelectedMedications()
        {
            return _selectedMedications.ToList();
        }

        public string GetSelectedMedicationsAsString(string delimiter = ",")
        {
            return string.Join(delimiter, _selectedMedications.Select(m => $"{m.Medication.Code}({m.Quantity})"));
        }

        private void OnAddManualMedication(object? sender, EventArgs e)
        {
            var code = _codeEdit?.Text.Trim();
            var name = _nameEdit?.Text.Trim();
            var dosage = _dosageEdit?.Text.Trim();
            var quantity = _quantityEdit?.Value ?? 1;

            if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(name))
            {
                XtraMessageBox.Show("약물코드와 약물명을 입력하세요.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var medication = new Medication
            {
                Code = code,
                Name = name,
                GenericName = name,
                Category = "직접입력"
            };

            AddMedication(medication, dosage, (int)quantity);

            if (_codeEdit != null) _codeEdit.Text = "";
            if (_nameEdit != null) _nameEdit.Text = "";
            if (_dosageEdit != null) _dosageEdit.Text = "";
            if (_quantityEdit != null) _quantityEdit.Value = 1;
        }

        private void OnRemoveMedication(object? sender, EventArgs e)
        {
            if (_selectedMedicationView != null && _selectedMedicationView.FocusedRowHandle >= 0)
            {
                var prescription = _selectedMedicationView.GetFocusedRow() as MedicationPrescription;
                if (prescription != null)
                {
                    RemoveMedication(prescription);
                }
            }
        }

        private void OnClearMedications(object? sender, EventArgs e)
        {
            ClearMedications();
        }

        private void OnMedicationDoubleClick(object? sender, EventArgs e)
        {
            if (_medicationView != null && _medicationView.FocusedRowHandle >= 0)
            {
                var medication = _medicationView.GetFocusedRow() as Medication;
                if (medication != null)
                {
                    AddMedication(medication);
                }
            }
        }

        protected virtual void OnMedicationSelected(MedicationSelectedEventArgs e)
        {
            MedicationSelected?.Invoke(this, e);
        }
    }
}
