using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using nU3.Connectivity;
using nU3.Connectivity.Implementations;
using nU3.Core.Repositories;
using nU3.Models;
using nU3.Tools.Deployer.Models;
using nU3.Tools.Deployer.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace nU3.Tools.Deployer.Views
{
    /// <summary>
    /// 모듈(Assembly/DLL) 배포 관리를 위한 메인 컨트롤입니다.
    /// </summary>
    public partial class ProgramDeployManagementControl : UserControl
    {
        private readonly IModuleRepository? _moduleRepo;
        private IFileTransferService? _fileTransferService;
        private bool _useServerTransfer;
        private string _serverModulePath = "Modules";

        private List<ModuleFileItem> _scannedFiles = new();
        private List<ModuleCompareRow> _compareRows = new();
        private List<ModuleMstDto> _dbMasters = new();
        private List<ModuleVerDto> _dbActiveVersions = new();

        private bool _isRefreshingFilterCombos;

        private readonly Dictionary<string, List<ProgramDto>> _scannedProgramsByModuleId = new(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, string> _moduleValidationErrors = new(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, Dictionary<string, string>> _programValidationErrors = new(StringComparer.OrdinalIgnoreCase);
        private readonly HashSet<string> _expandedModuleIds = new(StringComparer.OrdinalIgnoreCase);
        private List<ScanGridRow> _scanGridRows = new();

        private BindingList<ProgramEditRow> _programEditRows = new();
        private string? _selectedScanModuleId;

        public ProgramDeployManagementControl(IModuleRepository moduleRepo, IConfiguration configuration)
        {
            _moduleRepo = moduleRepo;

            var serverEnabled = configuration.GetValue<bool>("ServerConnection:Enabled", false);
            var baseUrl = configuration.GetValue<string>("ServerConnection:BaseUrl") ?? "https://localhost:64229";
            _serverModulePath = configuration.GetValue<string>("ModuleStorage:ServerPath") ?? "Modules";
            _useServerTransfer = configuration.GetValue<bool>("ModuleStorage:UseServerTransfer", true) && serverEnabled;

            if (_useServerTransfer)
            {
                try
                {
                    _fileTransferService = new HttpFileTransferClient(baseUrl);
                }
                catch
                {
                    _useServerTransfer = false;
                    _fileTransferService = null;
                }
            }

            InitializeComponent();

            if (!IsDesignMode())
            {
                SetupScanGrid();
                SetupProgramEditGrid();
                LoadModuleSettingsToUi();
                InitFilterCombos();
                WireUiEvents();
                ReloadDbAndBind();
            }
        }

        private void WireUiEvents()
        {
            // 체크박스 이벤트를 안전하게 재연결
            chkUpdated.CheckedChanged -= ChkUpdated_CheckedChanged;
            chkUpdated.CheckedChanged += ChkUpdated_CheckedChanged;
        }

        private void ChkUpdated_CheckedChanged(object? sender, EventArgs e)
        {
            // 스캔 데이터가 이미 있다면 필터 적용 갱신
            if (_scannedFiles.Count > 0)
            {
                RebuildScanGridRowsFromScannedFiles();
                RebindScanGrid(preserveScroll: false);
            }
        }

        private bool ShouldShowModuleInScanGrid(ModuleFileItem m)
        {
            if (!chkUpdated.Checked) return true;

            if (string.IsNullOrWhiteSpace(m.ModuleId))
                return true; // ModuleId 없는 DLL은 항상 표시

            var dbVer = _dbActiveVersions.FirstOrDefault(v => string.Equals(v.ModuleId, m.ModuleId, StringComparison.OrdinalIgnoreCase));
            if (dbVer == null)
                return true; // DB에 버전이 없는 경우 표시

            // DB와 스캔 버전이 다르면 표시
            return !string.Equals(dbVer.Version ?? string.Empty, m.Version ?? string.Empty, StringComparison.OrdinalIgnoreCase);
        }

        private void RebuildScanGridRowsFromScannedFiles()
        {
            var orderedModules = _scannedFiles.ToList();
            _scanGridRows = new List<ScanGridRow>(orderedModules.Count);

            foreach (var m in orderedModules)
            {
                if (!ShouldShowModuleInScanGrid(m))
                {
                    // 필터 제외이면 expand 상태도 제거
                    if (!string.IsNullOrWhiteSpace(m.ModuleId))
                        _expandedModuleIds.Remove(m.ModuleId);
                    continue;
                }

                var isExpanded = !string.IsNullOrWhiteSpace(m.ModuleId) && _expandedModuleIds.Contains(m.ModuleId);
                var validationSummary = _moduleValidationErrors.TryGetValue(m.ModuleId ?? string.Empty, out var errors) ? errors : null;
                _scanGridRows.Add(ScanGridRow.FromModule(m, isExpanded, validationSummary));

                if (isExpanded && !string.IsNullOrWhiteSpace(m.ModuleId) && _scannedProgramsByModuleId.TryGetValue(m.ModuleId, out var progs))
                {
                    var programErrors = _programValidationErrors.TryGetValue(m.ModuleId, out var progErrs) ? progErrs : null;

                    for (var i = 0; i < progs.Count; i++)
                    {
                        var progError = programErrors?.TryGetValue(progs[i].ProgId ?? string.Empty, out var err) == true ? err : null;
                        _scanGridRows.Add(ScanGridRow.FromProgram(m.ModuleId, progs[i], i, progError));
                    }
                }
            }
        }

        private void SetupScanGrid()
        {
            dgvModuleFiles.AutoGenerateColumns = false;
            dgvModuleFiles.Columns.Clear();

            dgvModuleFiles.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ColExpand",
                HeaderText = "",
                DataPropertyName = nameof(ScanGridRow.ExpandText),
                Width = 28,
                ReadOnly = true
            });

            dgvModuleFiles.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ColType",
                HeaderText = "유형",
                DataPropertyName = nameof(ScanGridRow.RowType),
                Width = 70,
                ReadOnly = true
            });

            dgvModuleFiles.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ColDllOrClass",
                HeaderText = "DLL (클래스명)",
                DataPropertyName = nameof(ScanGridRow.DllOrClassName),
                Width = 260,
                ReadOnly = true
            });

            dgvModuleFiles.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ColSystem",
                HeaderText = "시스템",
                DataPropertyName = nameof(ScanGridRow.SystemType),
                Width = 60,
                ReadOnly = true
            });

            dgvModuleFiles.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ColSubSystem",
                HeaderText = "서브",
                DataPropertyName = nameof(ScanGridRow.SubSystem),
                Width = 60,
                ReadOnly = true
            });

            dgvModuleFiles.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ColModuleOrScreen",
                HeaderText = "모듈명 (화면명)",
                DataPropertyName = nameof(ScanGridRow.ModuleOrScreenName),
                Width = 240,
                ReadOnly = true
            });

            dgvModuleFiles.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ColVersion",
                HeaderText = "버전",
                DataPropertyName = nameof(ScanGridRow.Version),
                Width = 90,
                ReadOnly = true
            });

            dgvModuleFiles.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ColProgNo",
                HeaderText = "프로그램#",
                DataPropertyName = nameof(ScanGridRow.ProgNoText),
                Width = 60,
                ReadOnly = true
            });

            dgvModuleFiles.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ColAuth",
                HeaderText = "권한",
                DataPropertyName = nameof(ScanGridRow.AuthLevel),
                Width = 50,
                ReadOnly = true
            });

            dgvModuleFiles.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ColModuleId",
                HeaderText = "모듈ID",
                DataPropertyName = nameof(ScanGridRow.ModuleId),
                Width = 200,
                ReadOnly = true
            });

            dgvModuleFiles.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ColProgId",
                HeaderText = "프로그램ID",
                DataPropertyName = nameof(ScanGridRow.ProgId),
                Width = 120,
                ReadOnly = true
            });

            dgvModuleFiles.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ColValidation",
                HeaderText = "검증",
                DataPropertyName = nameof(ScanGridRow.ValidationStatus),
                Width = 100,
                ReadOnly = true
            });

            dgvModuleFiles.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dgvModuleFiles.CellContentClick -= DgvModuleFiles_CellContentClick;
            dgvModuleFiles.CellContentClick += DgvModuleFiles_CellContentClick;
            dgvModuleFiles.CellDoubleClick -= DgvModuleFiles_CellDoubleClick;
            dgvModuleFiles.CellDoubleClick += DgvModuleFiles_CellDoubleClick;
            dgvModuleFiles.RowPrePaint -= DgvModuleFiles_RowPrePaint;
            dgvModuleFiles.RowPrePaint += DgvModuleFiles_RowPrePaint;
            dgvModuleFiles.SelectionChanged -= DgvModuleFiles_SelectionChanged;
            dgvModuleFiles.SelectionChanged += DgvModuleFiles_SelectionChanged;
        }

        private void SetupProgramEditGrid()
        {
            // 프로그램 병합/비교/편집용 그리드 설정
            dgvDbVersions.AutoGenerateColumns = false;
            dgvDbVersions.Columns.Clear();

            dgvDbVersions.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ColPStatus",
                HeaderText = "상태",
                DataPropertyName = nameof(ProgramEditRow.DbStatus),
                Width = 70,
                ReadOnly = true
            });

            dgvDbVersions.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ColPProgId",
                HeaderText = "프로그램ID",
                DataPropertyName = nameof(ProgramEditRow.ProgId),
                Width = 120
            });

            dgvDbVersions.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ColPScreenName",
                HeaderText = "화면명",
                DataPropertyName = nameof(ProgramEditRow.ProgName),
                Width = 200
            });

            dgvDbVersions.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ColPClassName",
                HeaderText = "클래스명",
                DataPropertyName = nameof(ProgramEditRow.ClassName),
                Width = 280,
                ReadOnly = true
            });

            dgvDbVersions.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ColPAuth",
                HeaderText = "권한",
                DataPropertyName = nameof(ProgramEditRow.AuthLevel),
                Width = 60
            });

            dgvDbVersions.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ColPDbDiff",
                HeaderText = "차이",
                DataPropertyName = nameof(ProgramEditRow.DiffText),
                Width = 240,
                ReadOnly = true
            });

            dgvDbVersions.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ColPModuleId",
                HeaderText = "모듈ID",
                DataPropertyName = nameof(ProgramEditRow.ModuleId),
                Width = 140,
                ReadOnly = true
            });

            dgvDbVersions.DataSource = _programEditRows;
        }

        // 디자이너의 일부 버전에서 여전히 참조될 수 있는 핸들러입니다.
        private void splitMasterDetail_Panel2_Paint(object sender, PaintEventArgs e)
        {
        }

        private void DgvModuleFiles_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var row = _scanGridRows[e.RowIndex];

            // 검증 오류가 있으면 상세 메시지 표시
            if (!string.IsNullOrWhiteSpace(row.ValidationErrors))
            {
                MessageBox.Show(
                    $"검증 오류:\n\n{row.ValidationErrors}",
                    row.IsModule ? $"DLL: {row.DllOrClassName}" : $"프로그램: {row.ProgId}",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            // 오류가 없으면 확장/축소 토글
            ToggleScanRowExpandAt(e.RowIndex);
        }

        private void DgvModuleFiles_CellContentClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (dgvModuleFiles.Columns[e.ColumnIndex].Name != "ColExpand") return;
            ToggleScanRowExpandAt(e.RowIndex);
        }

        private void ToggleScanRowExpandAt(int rowIndex)
        {
            if (rowIndex < 0 || rowIndex >= _scanGridRows.Count) return;

            var row = _scanGridRows[rowIndex];
            if (!row.IsModule) return;

            if (_expandedModuleIds.Contains(row.ModuleId))
                CollapseModuleRow(rowIndex, row.ModuleId);
            else
                ExpandModuleRow(rowIndex, row.ModuleId);

            RebindScanGrid(preserveScroll: true);
        }

        private void ExpandModuleRow(int rowIndex, string moduleId)
        {
            if (!_scannedProgramsByModuleId.TryGetValue(moduleId, out var progs))
            {
                _expandedModuleIds.Add(moduleId);
                return;
            }

            _expandedModuleIds.Add(moduleId);

            var programErrors = _programValidationErrors.TryGetValue(moduleId, out var progErrs) ? progErrs : null;
            var insertIndex = rowIndex + 1;
            for (var i = 0; i < progs.Count; i++)
            {
                var progError = programErrors?.TryGetValue(progs[i].ProgId ?? string.Empty, out var err) == true ? err : null;
                _scanGridRows.Insert(insertIndex++, ScanGridRow.FromProgram(moduleId, progs[i], i, progError));
            }
        }

        private void CollapseModuleRow(int rowIndex, string moduleId)
        {
            _expandedModuleIds.Remove(moduleId);

            var i = rowIndex + 1;
            while (i < _scanGridRows.Count && !_scanGridRows[i].IsModule)
            {
                if (string.Equals(_scanGridRows[i].ModuleId, moduleId, StringComparison.OrdinalIgnoreCase))
                    _scanGridRows.RemoveAt(i);
                else
                    i++;
            }
        }

        private void RebindScanGrid(bool preserveScroll)
        {
            var firstRow = preserveScroll ? dgvModuleFiles.FirstDisplayedScrollingRowIndex : -1;
            dgvModuleFiles.DataSource = null;
            dgvModuleFiles.DataSource = _scanGridRows;
            if (preserveScroll && firstRow >= 0 && firstRow < dgvModuleFiles.RowCount)
                dgvModuleFiles.FirstDisplayedScrollingRowIndex = firstRow;
        }

        private void DgvModuleFiles_RowPrePaint(object? sender, DataGridViewRowPrePaintEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= _scanGridRows.Count) return;

            var row = _scanGridRows[e.RowIndex];
            var hasValidationError = !string.IsNullOrWhiteSpace(row.ValidationErrors);

            if (!row.IsModule)
            {
                dgvModuleFiles.Rows[e.RowIndex].DefaultCellStyle.BackColor = hasValidationError
                    ? System.Drawing.Color.FromArgb(255, 240, 240)
                    : System.Drawing.Color.FromArgb(248, 248, 248);
                dgvModuleFiles.Rows[e.RowIndex].DefaultCellStyle.Font = new System.Drawing.Font(dgvModuleFiles.Font, System.Drawing.FontStyle.Regular);
                dgvModuleFiles.Rows[e.RowIndex].Cells["ColDllOrClass"].Style.Padding = new Padding(18, 0, 0, 0);
            }
            else
            {
                dgvModuleFiles.Rows[e.RowIndex].DefaultCellStyle.BackColor = hasValidationError
                    ? System.Drawing.Color.FromArgb(255, 230, 230)
                    : System.Drawing.Color.White;
                dgvModuleFiles.Rows[e.RowIndex].DefaultCellStyle.Font = new System.Drawing.Font(dgvModuleFiles.Font, System.Drawing.FontStyle.Bold);
                dgvModuleFiles.Rows[e.RowIndex].Cells["ColDllOrClass"].Style.Padding = new Padding(0, 0, 0, 0);
            }

            // 검증 컬럼 색상 설정
            if (hasValidationError)
            {
                dgvModuleFiles.Rows[e.RowIndex].Cells["ColValidation"].Style.ForeColor = System.Drawing.Color.Red;
                dgvModuleFiles.Rows[e.RowIndex].Cells["ColValidation"].Style.Font = new System.Drawing.Font(dgvModuleFiles.Font, System.Drawing.FontStyle.Bold);
            }
            else
            {
                dgvModuleFiles.Rows[e.RowIndex].Cells["ColValidation"].Style.ForeColor = System.Drawing.Color.Green;
            }
        }

        private sealed class ScanGridRow
        {
            public bool IsModule { get; init; }
            public string RowType => IsModule ? "DLL" : "PROG";
            public string ExpandText { get; init; } = string.Empty;

            public string DllOrClassName { get; init; } = string.Empty;
            public string SystemType { get; init; } = string.Empty;
            public string SubSystem { get; init; } = string.Empty;
            public string ModuleId { get; init; } = string.Empty;
            public string ModuleOrScreenName { get; init; } = string.Empty;
            public string Version { get; init; } = string.Empty;

            public string ProgNoText { get; init; } = string.Empty;
            public string ProgId { get; init; } = string.Empty;
            public int AuthLevel { get; init; }
            public string ValidationStatus { get; init; } = string.Empty;
            public string ValidationErrors { get; init; } = string.Empty;

            public static ScanGridRow FromModule(ModuleFileItem item, bool isExpanded, string validationSummary = null)
            {
                return new ScanGridRow
                {
                    IsModule = true,
                    ExpandText = item.ProgramCount <= 0 ? string.Empty : (isExpanded ? "-" : "+"),
                    DllOrClassName = item.FileName,
                    SystemType = item.SystemType,
                    SubSystem = item.SubSystem,
                    ModuleId = item.ModuleId,
                    ModuleOrScreenName = item.ModuleName,
                    Version = item.Version,
                    ProgNoText = string.Empty,
                    ProgId = string.Empty,
                    AuthLevel = 0,
                    ValidationStatus = string.IsNullOrWhiteSpace(validationSummary) ? "✓" : "⚠",
                    ValidationErrors = validationSummary ?? string.Empty
                };
            }

            public static ScanGridRow FromProgram(string moduleId, ProgramDto p, int index, string validationErrors = null)
            {
                return new ScanGridRow
                {
                    IsModule = false,
                    ExpandText = string.Empty,
                    ModuleId = moduleId,
                    DllOrClassName = p.ClassName,
                    SystemType = p.SystemType ?? string.Empty,  // DLL에서 읽어온 실제 값
                    SubSystem = p.SubSystem ?? string.Empty,    // DLL에서 읽어온 실제 값
                    ModuleOrScreenName = p.ProgName,
                    ProgNoText = (index + 1).ToString(),
                    ProgId = p.ProgId,
                    AuthLevel = p.AuthLevel,
                    ValidationStatus = string.IsNullOrWhiteSpace(validationErrors) ? "✓" : "⚠",
                    ValidationErrors = validationErrors ?? string.Empty
                };
            }
        }

        private static bool IsDesignMode()
        {
            return System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime;
        }

        private void InitFilterCombos()
        {
            _isRefreshingFilterCombos = true;
            try
            {
                cboFilterCategory.BeginUpdate();
                try
                {
                    cboFilterCategory.Items.Clear();
                    cboFilterCategory.Items.Add("(ALL)");
                    cboFilterCategory.Items.AddRange(new object[] { "EMR", "ADM", "BAS" });
                    cboFilterCategory.SelectedIndex = 0;
                }
                finally
                {
                    cboFilterCategory.EndUpdate();
                }

                cboFilterSubSystem.BeginUpdate();
                try
                {
                    cboFilterSubSystem.Items.Clear();
                    cboFilterSubSystem.Items.Add("(ALL)");
                    cboFilterSubSystem.SelectedIndex = 0;
                }
                finally
                {
                    cboFilterSubSystem.EndUpdate();
                }
            }
            finally
            {
                _isRefreshingFilterCombos = false;
            }
        }

        private void LoadModuleSettingsToUi()
        {
            var settings = JsonSettingsStore.LoadModuleSettings();
            txtModulesRoot.Text = settings.ModulesRootPath;
        }

        private void SaveModulesRootToSettings(string path)
        {
            var settings = JsonSettingsStore.LoadModuleSettings();
            settings.ModulesRootPath = path;
            JsonSettingsStore.SaveModuleSettings(settings);
        }

        private void BtnBrowseModulesRoot_Click(object sender, EventArgs e)
        {
            using var dialog = new FolderBrowserDialog
            {
                Description = "Modules 폴더를 선택하세요",
                ShowNewFolderButton = false
            };

            if (Directory.Exists(txtModulesRoot.Text))
            {
                dialog.SelectedPath = txtModulesRoot.Text;
            }

            if (dialog.ShowDialog() != DialogResult.OK) return;

            txtModulesRoot.Text = dialog.SelectedPath;
            SaveModulesRootToSettings(dialog.SelectedPath);
        }

        private void BtnScanModules_Click(object sender, EventArgs e)
        {
            ScanModulesFromSelectedFolder();
            RefreshFilterSubSystems();
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            ReloadDbAndBind();
            RefreshFilterSubSystems();
        }

        private void ReloadDbAndBind()
        {
            if (_moduleRepo == null) return;

            _dbMasters = _moduleRepo.GetAllModules();
            _dbActiveVersions = _moduleRepo.GetActiveVersions();

            RefreshFilterSubSystems();

            // DB 로드 이후에는 화면 필터 반영
            if (_scannedFiles.Count > 0)
            {
                RebuildScanGridRowsFromScannedFiles();
                RebindScanGrid(preserveScroll: true);
            }
        }

        private void ScanModulesFromSelectedFolder()
        {
            var root = txtModulesRoot.Text?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(root) || !Directory.Exists(root))
            {
                MessageBox.Show("Modules 폴더 경로가 올바르지 않습니다.");
                return;
            }

            var parser = new DllMetadataParser();
            var list = new List<ModuleFileItem>();

            _scannedProgramsByModuleId.Clear();
            _moduleValidationErrors.Clear();
            _programValidationErrors.Clear();

            foreach (var path in Directory.GetFiles(root, "*.dll", SearchOption.AllDirectories))
            {
                try
                {
                    var fi = new FileInfo(path);
                    var item = new ModuleFileItem
                    {
                        FileName = fi.Name,
                        FullPath = fi.FullName,
                        RelativePath = Path.GetRelativePath(root, fi.FullName),
                        SizeBytes = fi.Length
                    };

                    try
                    {
                        var info = parser.Parse(fi.FullName);
                        item.SystemType = info.SystemType;
                        item.SubSystem = info.SubSystem;
                        item.ModuleId = info.ModuleId;
                        item.ModuleName = info.ModuleName;
                        item.Version = info.Version;
                        item.ProgramCount = info.Programs?.Count ?? 0;

                        // 검증 오류 저장
                        if (info.HasValidationErrors)
                        {
                            _moduleValidationErrors[item.ModuleId] = info.GetValidationSummary();
                        }

                        if (!string.IsNullOrWhiteSpace(item.ModuleId) && info.Programs != null)
                        {
                            _scannedProgramsByModuleId[item.ModuleId] = info.Programs;

                            // 프로그램별 검증 오류 저장
                            var progErrs = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                            foreach (var prog in info.Programs)
                            {
                                // 프로그램별 검증 오류는 ValidationErrors에 포함되어 있음
                                var progErrors = info.ValidationErrors
                                    .Where(e => e.StartsWith($"[{prog.ClassName?.Split('.').LastOrDefault() ?? prog.ProgId}]"))
                                    .ToList();

                                if (progErrors.Count > 0)
                                {
                                    progErrs[prog.ProgId] = string.Join("; ", progErrors);
                                }
                            }

                            if (progErrs.Count > 0)
                            {
                                _programValidationErrors[item.ModuleId] = progErrs;
                            }
                        }
                    }
                    catch
                    {
                        // 파싱 실패는 무시하고 기본 파일 정보만 유지
                    }

                    list.Add(item);
                }
                catch
                {
                    // 파일 접근 오류는 무시
                }
            }

            _scannedFiles = list.OrderBy(x => x.RelativePath).ToList();

            RebuildScanGridRowsFromScannedFiles();
            RebindScanGrid(preserveScroll: false);

            // 검증 오류가 있으면 메시지 표시
            var totalErrors = _moduleValidationErrors.Count + _programValidationErrors.Values.Sum(d => d.Count);
            if (totalErrors > 0)
            {
                MessageBox.Show(
                    $"스캔 완료!\n\n" +
                    $"총 {_scannedFiles.Count}개 DLL 발견\n" +
                    $"⚠ {totalErrors}개 검증 오류 발견\n\n" +
                    $"⚠ 표시된 행을 더블클릭하면 상세 오류를 확인할 수 있습니다.",
                    "스캔 완료",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        private void BuildCompareRowsAndBind()
        {
        }

        private void RefreshFilterSubSystems()
        {
            if (_isRefreshingFilterCombos) return;

            _isRefreshingFilterCombos = true;
            try
            {
                var category = SelectedCategory;

                var subs = _dbMasters
                    .Where(m => category == null || string.Equals(m.Category, category, StringComparison.OrdinalIgnoreCase))
                    .Select(m => m.SubSystem)
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .OrderBy(s => s)
                    .ToList();

                var current = cboFilterSubSystem.SelectedItem?.ToString();

                cboFilterSubSystem.BeginUpdate();
                try
                {
                    cboFilterSubSystem.Items.Clear();
                    cboFilterSubSystem.Items.Add("(ALL)");
                    foreach (var s in subs) cboFilterSubSystem.Items.Add(s);

                    if (cboFilterSubSystem.SelectedIndex != 0)
                    {
                        cboFilterSubSystem.SelectedIndex = 0;
                    }

                    if (!string.IsNullOrWhiteSpace(current) && cboFilterSubSystem.Items.Contains(current))
                    {
                        cboFilterSubSystem.SelectedItem = current;
                    }
                }
                finally
                {
                    cboFilterSubSystem.EndUpdate();
                }
            }
            finally
            {
                _isRefreshingFilterCombos = false;
            }
        }

        private string? SelectedCategory
        {
            get
            {
                var text = cboFilterCategory.SelectedItem?.ToString();
                if (string.IsNullOrWhiteSpace(text) || text == "(ALL)") return null;
                return text;
            }
        }

        private string? SelectedSubSystem
        {
            get
            {
                var text = cboFilterSubSystem.SelectedItem?.ToString();
                if (string.IsNullOrWhiteSpace(text) || text == "(ALL)") return null;
                return text;
            }
        }

        private void CboFilter_Changed(object sender, EventArgs e)
        {
            if (_isRefreshingFilterCombos) return;

            RefreshFilterSubSystems();
            ApplyFilterAndBind();
        }

        private void ApplyFilterAndBind()
        {
            // 중앙 마스터 그리드 제거됨 - 바인딩할 내용 없음
        }

        private void DgvDbMaster_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
        }

        private void DgvDbMaster_SelectionChanged(object sender, EventArgs e)
        {
        }

        private void DgvModuleFiles_SelectionChanged(object? sender, EventArgs e)
        {
            if (dgvModuleFiles.SelectedRows.Count <= 0) return;
            if (dgvModuleFiles.SelectedRows[0].DataBoundItem is not ScanGridRow row) return;

            if (string.IsNullOrWhiteSpace(row.ModuleId)) return;

            _selectedScanModuleId = row.ModuleId;
            RefreshCenterForSelectedScanModule(row.ModuleId);
        }

        private void RefreshCenterForSelectedScanModule(string moduleId)
        {
            // 오른쪽 패널: DB에 모듈 정보가 있으면 DB 값, 없으면 스캔 값으로 채움
            var dbMst = _dbMasters.FirstOrDefault(m => string.Equals(m.ModuleId, moduleId, StringComparison.OrdinalIgnoreCase));
            var scan = _scannedFiles.FirstOrDefault(s => string.Equals(s.ModuleId, moduleId, StringComparison.OrdinalIgnoreCase));

            if (dbMst != null)
            {
                cboCategory.Text = dbMst.Category;
                txtSubSystem.Text = dbMst.SubSystem;
                txtModuleId.Text = dbMst.ModuleId;
                txtModuleName.Text = dbMst.ModuleName;
                txtFileName.Text = dbMst.FileName;
            }
            else if (scan != null)
            {
                cboCategory.Text = scan.SystemType;
                txtSubSystem.Text = scan.SubSystem;
                txtModuleId.Text = scan.ModuleId;
                txtModuleName.Text = scan.ModuleName;
                txtFileName.Text = scan.FileName;
            }

            // 오른쪽 패널: DB의 활성 버전 정보 표시
            var dbVer = _dbActiveVersions.FirstOrDefault(v => string.Equals(v.ModuleId, moduleId, StringComparison.OrdinalIgnoreCase));
            if (dbVer != null)
            {
                txtActiveVersion.Text = dbVer.Version;
                txtFileHash.Text = dbVer.FileHash;
                txtFileSize.Text = dbVer.FileSize.ToString();
                txtStoragePath.Text = dbVer.StoragePath;
                txtDeployDesc.Text = dbVer.DeployDesc;
            }
            else
            {
                txtActiveVersion.Text = scan?.Version ?? string.Empty;
                txtFileHash.Text = string.Empty;
                txtFileSize.Text = scan?.SizeBytes.ToString() ?? string.Empty;
                txtStoragePath.Text = string.Empty;
                txtDeployDesc.Text = string.Empty;
            }

            // 가운데 그리드: 프로그램 비교/편집 행 구성
            var progRepo = Program.ServiceProvider.GetRequiredService<IProgramRepository>();
            var dbModulePrograms = progRepo.GetProgramsByModuleId(moduleId);
            var dbByProgId = dbModulePrograms
                .Where(p => !string.IsNullOrWhiteSpace(p.ProgId))
                .GroupBy(p => p.ProgId, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);

            _programEditRows = new BindingList<ProgramEditRow>();

            if (_scannedProgramsByModuleId.TryGetValue(moduleId, out var scannedProgs))
            {
                foreach (var sp in scannedProgs)
                {
                    var progId = sp.ProgId ?? string.Empty;
                    dbByProgId.TryGetValue(progId, out var dbp);

                    var screenName = sp.ProgName ?? string.Empty;
                    var className = sp.ClassName ?? string.Empty;
                    var auth = sp.AuthLevel;

                    var status = dbp == null ? "NEW" : "OK";
                    var diffs = new List<string>();

                    if (dbp != null)
                    {
                        if (!string.Equals(dbp.ModuleId ?? string.Empty, moduleId, StringComparison.OrdinalIgnoreCase)) diffs.Add($"MODULE_ID:{dbp.ModuleId}->{moduleId}");
                        if (!string.Equals(dbp.ProgName ?? string.Empty, screenName, StringComparison.OrdinalIgnoreCase)) diffs.Add("PROG_NAME");
                        if (!string.Equals(dbp.ClassName ?? string.Empty, className, StringComparison.OrdinalIgnoreCase)) diffs.Add("CLASS_NAME");
                        if (dbp.AuthLevel != auth) diffs.Add($"AUTH:{dbp.AuthLevel}->{auth}");

                        if (diffs.Count > 0) status = "UPDATE";
                    }

                    _programEditRows.Add(new ProgramEditRow
                    {
                        ModuleId = moduleId,
                        ProgId = progId,
                        ProgName = screenName,
                        ClassName = className,
                        AuthLevel = auth,
                        DbStatus = status,
                        DiffText = string.Join(", ", diffs)
                    });
                }
            }

            dgvDbVersions.DataSource = _programEditRows;
        }

        private sealed class ProgramEditRow
        {
            public string DbStatus { get; set; } = string.Empty; // NEW / UPDATE / OK
            public string DiffText { get; set; } = string.Empty;
            public string ModuleId { get; set; } = string.Empty;
            public string ProgId { get; set; } = string.Empty;
            public string ProgName { get; set; } = string.Empty;
            public string ClassName { get; set; } = string.Empty;
            public int AuthLevel { get; set; } = 1;
            public string IsActive { get; set; } = "N";
            public int ProgType { get; set; } = 1;
        }

        private void SaveModule()
        {
            if (_moduleRepo == null) return;

            var dto = new ModuleMstDto
            {
                ModuleId = txtModuleId.Text,
                ModuleName = txtModuleName.Text,
                FileName = txtFileName.Text,
                Category = cboCategory.Text,
                SubSystem = txtSubSystem.Text
            };
            _moduleRepo.SaveModule(dto);

            var progRepo = Program.ServiceProvider.GetRequiredService<IProgramRepository>();
            var activeProgIds = new List<string>();
            foreach (var p in _programEditRows)
            {
                if (string.IsNullOrWhiteSpace(p.ProgId)) continue;

                activeProgIds.Add(p.ProgId);
                progRepo.UpsertProgram(new ProgramDto
                {
                    ProgId = p.ProgId,
                    ModuleId = dto.ModuleId,
                    ProgName = p.ProgName,
                    ClassName = p.ClassName,
                    AuthLevel = p.AuthLevel,
                    IsActive = "Y",
                    ProgType = p.ProgType
                });
            }

            if (!string.IsNullOrWhiteSpace(dto.ModuleId))
            {
                progRepo.DeactivateMissingPrograms(dto.ModuleId, activeProgIds);
            }

            ReloadDbAndBind();

            if (!string.IsNullOrWhiteSpace(dto.ModuleId))
            {
                RefreshCenterForSelectedScanModule(dto.ModuleId);
            }
        }

        private void DeleteModule()
        {
            if (_moduleRepo == null) return;
            _moduleRepo.DeleteModule(txtModuleId.Text);
            ReloadDbAndBind();
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            SaveModule();
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            DeleteModule();
        }

        private void BtnSmartUpload_Click(object sender, EventArgs e)
        {
            PerformSmartUpload();
        }

        private void PerformSmartUpload()
        {
            if (_moduleRepo == null) return;

            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "nU3 Module DLLs (*.dll)|*.dll";
                ofd.Title = "모듈 DLL 선택";

                if (ofd.ShowDialog() != DialogResult.OK) return;

                try
                {
                    var parser = new Services.DllMetadataParser();
                    var info = parser.Parse(ofd.FileName);

                    string msg = $"Module: {info.ModuleId}\n" +
                                 $"System: {info.SystemType} / {info.SubSystem}\n" +
                                 $"Version: {info.Version}\n" +
                                 $"Programs Found: {info.Programs.Count}\n\n" +
                                 "등록 및 업로드를 진행하시겠습니까?";

                    if (MessageBox.Show(msg, "스마트 배포", MessageBoxButtons.YesNo, MessageBoxIcon.Information) != DialogResult.Yes)
                        return;

                    var masterDto = new ModuleMstDto
                    {
                        ModuleId = info.ModuleId,
                        Category = info.SystemType,
                        SubSystem = info.SubSystem,
                        ModuleName = info.ModuleName,
                        FileName = info.FileName
                    };
                    _moduleRepo.SaveModule(masterDto);

                    string hash = CalculateFileHash(ofd.FileName);

                    var categoryPath = info.SystemType;
                    if (!string.IsNullOrEmpty(info.SubSystem))
                    {
                        categoryPath = $"{categoryPath}/{info.SubSystem}";
                    }

                    string storagePath;

                    if (!_useServerTransfer || _fileTransferService == null)
                    {
                        throw new InvalidOperationException("서버 전송이 비활성화되어 있습니다. 서버 연결 설정을 확인하세요.");
                    }

                    var homeDirectory = _fileTransferService.GetHomeDirectory();
                    var serverPath = string.IsNullOrWhiteSpace(homeDirectory)
                        ? $"{_serverModulePath}/{categoryPath}/{info.FileName}"
                        : $"{homeDirectory.TrimEnd('/', '\\')}/{_serverModulePath}/{categoryPath}/{info.FileName}";

                    var success = _fileTransferService.UploadFile(ofd.FileName, serverPath);
                    if (!success)
                    {
                        throw new InvalidOperationException($"Server upload failed: {serverPath}");
                    }

                    storagePath = serverPath;

                    var verDto = new ModuleVerDto
                    {
                        ModuleId = info.ModuleId,
                        Version = info.Version,
                        FileHash = hash,
                        FileSize = info.FileSize,
                        StoragePath = storagePath,
                        DeployDesc = $"Smart Upload: {DateTime.Now}",
                        Category = info.SystemType
                    };

                    _moduleRepo.DeactivateOldVersions(info.ModuleId);
                    _moduleRepo.AddVersion(verDto);

                    var progRepo = Program.ServiceProvider.GetRequiredService<nU3.Core.Repositories.IProgramRepository>();
                    var activeProgIds = new List<string>();
                    foreach (var prog in info.Programs)
                    {
                        if (!string.IsNullOrWhiteSpace(prog.ProgId))
                        {
                            activeProgIds.Add(prog.ProgId);
                        }
                        prog.IsActive = "Y";
                        progRepo.UpsertProgram(prog);
                    }

                    if (!string.IsNullOrWhiteSpace(info.ModuleId))
                    {
                        progRepo.DeactivateMissingPrograms(info.ModuleId, activeProgIds);
                    }

                    MessageBox.Show("배포 완료!");
                    ReloadDbAndBind();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"배포 오류: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private static string CalculateFileHash(string filePath)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            using (var stream = File.OpenRead(filePath))
            {
                var hash = sha256.ComputeHash(stream);
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
        }

        private void btnBulkUpload_Click(object sender, EventArgs e)
        {
            if (_moduleRepo == null) return;
            if (_scannedFiles.Count == 0)
            {
                MessageBox.Show("스캔된 모듈이 없습니다.", "일괄 업로드", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (!_useServerTransfer || _fileTransferService == null)
            {
                MessageBox.Show("서버 전송이 비활성화되어 있습니다. 설정을 확인하세요.", "일괄 업로드", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var parser = new Services.DllMetadataParser();
            var progRepo = Program.ServiceProvider.GetRequiredService<IProgramRepository>();

            var successCount = 0;
            var failureCount = 0;
            var failedModules = new List<string>();

            foreach (var module in _scannedFiles)
            {
                try
                {
                    UploadModuleFromScan(module, parser, progRepo);
                    successCount++;
                }
                catch (Exception ex)
                {
                    failureCount++;
                    failedModules.Add(module.FileName);
                    Debug.WriteLine($"Bulk upload failed for {module.FileName}: {ex}");
                }
            }

            ReloadDbAndBind();

            var message = failureCount == 0
                ? $"{successCount}개 모듈 일괄 업로드 완료!"
                : $"{successCount}개 성공, {failureCount}개 실패";

            if (failedModules.Count > 0)
            {
                message += $"\n실패한 모듈: {string.Join(", ", failedModules)}";
            }

            MessageBox.Show(message, "일괄 업로드 결과", MessageBoxButtons.OK, failureCount > 0 ? MessageBoxIcon.Warning : MessageBoxIcon.Information);
        }

        private void UploadModuleFromScan(ModuleFileItem module, Services.DllMetadataParser parser, IProgramRepository progRepo)
        {
            if (string.IsNullOrWhiteSpace(module.FullPath) || !File.Exists(module.FullPath))
            {
                throw new FileNotFoundException("DLL 파일을 찾을 수 없습니다.", module.FullPath);
            }

            var info = parser.Parse(module.FullPath);
            if (string.IsNullOrWhiteSpace(info.ModuleId))
            {
                throw new InvalidOperationException("모듈 ID를 확인할 수 없습니다.");
            }

            var masterDto = new ModuleMstDto
            {
                ModuleId = info.ModuleId,
                Category = info.SystemType,
                SubSystem = info.SubSystem,
                ModuleName = info.ModuleName,
                FileName = info.FileName
            };
            _moduleRepo.SaveModule(masterDto);

            var hash = CalculateFileHash(module.FullPath);
            var categoryPath = !string.IsNullOrWhiteSpace(info.SystemType)
                ? info.SystemType
                : module.SystemType ?? string.Empty;

            if (!string.IsNullOrWhiteSpace(info.SubSystem))
            {
                categoryPath = string.IsNullOrWhiteSpace(categoryPath)
                    ? info.SubSystem
                    : $"{categoryPath}/{info.SubSystem}";
            }

            var relativeFilePath = string.IsNullOrWhiteSpace(categoryPath)
                ? info.FileName
                : $"{categoryPath}/{info.FileName}";

            var homeDirectory = _fileTransferService.GetHomeDirectory();
            var serverPath = string.IsNullOrWhiteSpace(homeDirectory)
                ? $"{_serverModulePath}/{relativeFilePath}"
                : $"{homeDirectory.TrimEnd('/', '\\')}/{_serverModulePath}/{relativeFilePath}";

            if (!_fileTransferService.UploadFile(module.FullPath, serverPath))
            {
                throw new InvalidOperationException($"Server upload failed: {serverPath}");
            }

            var verDto = new ModuleVerDto
            {
                ModuleId = info.ModuleId,
                Version = info.Version,
                FileHash = hash,
                FileSize = info.FileSize,
                StoragePath = serverPath,
                DeployDesc = $"Bulk Upload: {DateTime.Now}",
                Category = info.SystemType
            };

            _moduleRepo.DeactivateOldVersions(info.ModuleId);
            _moduleRepo.AddVersion(verDto);

            var activeProgIds = new List<string>();
            foreach (var prog in info.Programs)
            {
                if (!string.IsNullOrWhiteSpace(prog.ProgId))
                {
                    activeProgIds.Add(prog.ProgId);
                }

                prog.IsActive = "Y";
                prog.ModuleId = info.ModuleId;
                progRepo.UpsertProgram(prog);
            }

            progRepo.DeactivateMissingPrograms(info.ModuleId, activeProgIds);
        }

    }

}
