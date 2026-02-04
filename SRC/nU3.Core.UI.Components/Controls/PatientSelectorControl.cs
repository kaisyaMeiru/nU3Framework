using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using nU3.Core.UI.Components.Events;
using nU3.Models;
using System.ComponentModel;

namespace nU3.Core.UI.Components.Controls
{
    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(PatientSelectorControl))]
    public partial class PatientSelectorControl : XtraUserControl
    {
        private GridControl? _gridControl;
        private GridView? _gridView;
        private TextEdit? _searchEdit;
        private SimpleButton? _searchButton;
        private SimpleButton? _clearButton;
        private SimpleButton? _selectButton;
        private PanelControl? _headerPanel;
        private PanelControl? _buttonPanel;

        private readonly BindingList<PatientInfoDto> _patients = new();

        public event EventHandler<PatientSelectedEventArgs>? PatientSelected;

        [Category("Data")]
        public string? SelectedPatientId
        {
            get => _selectedPatient?.PatientId;
        }

        [Category("Behavior")]
        public int SearchLimit { get; set; } = 100;

        [Category("Behavior")]
        public bool AutoSearch { get; set; } = true;

        private PatientInfoDto? _selectedPatient;

        public PatientSelectorControl()
        {
            InitializeComponent();
            AttachEventHandlers();
        }

        private void AttachEventHandlers()
        {
            if (_searchButton != null)
            {
                _searchButton.Click += OnSearch;
            }

            if (_clearButton != null)
            {
                _clearButton.Click += OnClear;
            }

            if (_selectButton != null)
            {
                _selectButton.Click += OnSelect;
            }

            if (_gridView != null)
            {
                _gridView.DoubleClick += OnGridDoubleClick;
                _gridView.CustomColumnDisplayText += OnGridViewCustomColumnDisplayText;
            }
        }

        private void OnGridViewCustomColumnDisplayText(object? sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            if (e.Column.FieldName == "Gender" && e.ListSourceRowIndex >= 0)
            {
                if (e.Value is int genderValue)
                {
                    e.DisplayText = genderValue switch
                    {
                        1 => "남",
                        2 => "여",
                        _ => "알수없음"
                    };
                }
            }
        }

        public void LoadPatients(string searchTerm = "", int offset = 0)
        {
            // TODO: 구현 필요 - 현재는 단순히 그리드 데이터만 로드
        // 실제 구현에서는 API 호출 등을 통해 데이터를 가져옵니다
            var filtered = _patients.Where(p =>
                p.PatientName.Contains(searchTerm) ||
                p.PatientId.Contains(searchTerm)).ToList();

            if (_gridView != null)
            {
                _gridView.BeginDataUpdate();
                _gridView.ClearSelection();
                _gridView.EndDataUpdate();

                foreach (var patient in filtered)
                {
                    var rowHandle = _gridView.LocateByValue("PatientId", patient.PatientId);
                    if (rowHandle >= 0)
                    {
                        _gridView.FocusedRowHandle = rowHandle;
                    }
                }
            }
        }

        public void SetPatients(IEnumerable<PatientInfoDto> patients)
        {
            _patients.Clear();
            foreach (var patient in patients)
            {
                _patients.Add(patient);
            }
        }

        public PatientInfoDto? GetSelectedPatient()
        {
            return _selectedPatient;
        }

        public void ClearSelection()
        {
            _selectedPatient = null;
            if (_gridView != null)
            {
                _gridView.ClearSelection();
            }
        }

        // OnSearchPatients 메서드 제거 (사용하지 않음)

        private void OnSearch(object? sender, EventArgs e)
        {
            LoadPatients(_searchEdit?.Text ?? "");
        }

        private void OnClear(object? sender, EventArgs e)
        {
            if (_searchEdit != null)
            {
                _searchEdit.Text = "";
            }
            _patients.Clear();
            ClearSelection();
        }

        private void OnSelect(object? sender, EventArgs e)
        {
            if (_gridView != null && _gridView.FocusedRowHandle >= 0)
            {
                _selectedPatient = _gridView.GetFocusedRow() as PatientInfoDto;
                PatientSelected?.Invoke(this, new PatientSelectedEventArgs
                {
                    Patient = _selectedPatient,
                    Source = "PatientSelector"
                });
            }
        }

        private void OnGridDoubleClick(object? sender, EventArgs e)
        {
            OnSelect(sender, e);
        }
    }
}
