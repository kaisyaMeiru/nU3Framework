using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using nU3.Connectivity;
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
using nU3.Core.UI;
using nU3.Core.UI.Controls;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Columns;
using System.Drawing;
using nU3.Core.Interfaces;

namespace nU3.Tools.Deployer.Views
{
    /// <summary>
    /// 모듈(Assembly/DLL) 배포 관리를 위한 메인 컨트롤입니다.
    /// </summary>
    public partial class ProgramDeployManagementControl : BaseWorkControl
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
        private string? _selectedScanProgId;   // 좌측에서 선택된 프로그램 ID

        private List<DbGridRow> _dbGridRows = new();
        private readonly HashSet<string> _expandedDbModuleIds = new(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Designer 전용 생성자입니다.
        /// </summary>
        public ProgramDeployManagementControl()
        {
            InitializeComponent();
        }

        public ProgramDeployManagementControl(IModuleRepository moduleRepo, IConfiguration configuration, IFileTransferService fileTransferService)
        {
            _moduleRepo = moduleRepo;
            _fileTransferService = fileTransferService;

            var serverEnabled = true;
            // 코드상으로 고정: 서버 경로는 항상 'Modules'
            _serverModulePath = nU3.Core.Services.ModuleLoaderService.MODULES_DIR;
            _useServerTransfer = true && serverEnabled;

            InitializeComponent();

            if (!IsDesignMode())
            {
                SetupScanGrid();
                SetupDbGrid();
                LoadModuleSettingsToUi();
                InitFilterCombos();
                WireUiEvents();
                ReloadDbAndBind();
            }
        }

        private void WireUiEvents()
        {
            chkUpdated.CheckedChanged -= ChkUpdated_CheckedChanged;
            chkUpdated.CheckedChanged += ChkUpdated_CheckedChanged;
        }

        private void ChkUpdated_CheckedChanged(object? sender, EventArgs e)
        {
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
                return true;

            var dbVer = _dbActiveVersions.FirstOrDefault(v => string.Equals(v.ModuleId, m.ModuleId, StringComparison.OrdinalIgnoreCase));
            if (dbVer == null)
                return true;

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
                    if (!string.IsNullOrWhiteSpace(m.ModuleId))
                        _expandedModuleIds.Remove(m.ModuleId);
                    continue;
                }

                var isExpanded = !string.IsNullOrWhiteSpace(m.ModuleId) && _expandedModuleIds.Contains(m.ModuleId);
                var validationSummary = _moduleValidationErrors.TryGetValue(m.ModuleId ?? string.Empty, out var errors) ? errors : null;
                var dbVer = _dbActiveVersions.FirstOrDefault(v => string.Equals(v.ModuleId, m.ModuleId, StringComparison.OrdinalIgnoreCase));

                _scanGridRows.Add(ScanGridRow.FromModule(m, isExpanded, validationSummary, dbVer));

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
            gvModuleFiles.Columns.Clear();

            // 공통
            var colToggle = gvModuleFiles.Columns.AddVisible("Toggle", "");
            colToggle.Width = 24;
            colToggle.OptionsColumn.FixedWidth = true;

            var colCat = gvModuleFiles.Columns.AddVisible("Category", "시스템");
            colCat.Width = 52;

            var colSub = gvModuleFiles.Columns.AddVisible("SubSystem", "서브");
            colSub.Width = 52;

            // 모듈 전용
            var colDll = gvModuleFiles.Columns.AddVisible("DllFileName", "DLL 파일명");
            colDll.Width = 200;

            var colMN = gvModuleFiles.Columns.AddVisible("ModuleName", "모듈명");
            colMN.Width = 180;

            var colVer = gvModuleFiles.Columns.AddVisible("Version", "파일버전");
            colVer.Width = 76;

            var colDbV = gvModuleFiles.Columns.AddVisible("DbVersion", "DB버전");
            colDbV.Width = 76;

            var colPCnt = gvModuleFiles.Columns.AddVisible("ProgCount", "화면수");
            colPCnt.Width = 46;
            colPCnt.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            colPCnt.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;

            var colSync = gvModuleFiles.Columns.AddVisible("SyncMark", "동기상태");
            colSync.Width = 72;
            colSync.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;

            // 프로그램 전용
            var colNo = gvModuleFiles.Columns.AddVisible("ProgNo", "#");
            colNo.Width = 28;
            colNo.OptionsColumn.FixedWidth = true;
            colNo.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;

            var colPId = gvModuleFiles.Columns.AddVisible("ProgId", "프로그램 ID");
            colPId.Width = 140;

            var colPN = gvModuleFiles.Columns.AddVisible("ProgName", "화면명");
            colPN.Width = 180;

            var colCls = gvModuleFiles.Columns.AddVisible("ClassName", "클래스명");
            colCls.Width = 200;

            var colAuth = gvModuleFiles.Columns.AddVisible("AuthLevelText", "권한");
            colAuth.Width = 52;
            colAuth.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;

            // 검증 (공통, 마지막)
            var colValid = gvModuleFiles.Columns.AddVisible("ValidMark", "검증");
            colValid.Width = 36;
            colValid.OptionsColumn.FixedWidth = true;
            colValid.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;

            // ModuleId (참조용)
            var colMid = gvModuleFiles.Columns.AddVisible("ModuleId", "모듈 ID");
            colMid.Width = 220;

            gvModuleFiles.OptionsBehavior.Editable = false;
            gvModuleFiles.OptionsView.ShowGroupPanel = false;
            gvModuleFiles.OptionsView.EnableAppearanceEvenRow = false;
            gvModuleFiles.OptionsView.EnableAppearanceOddRow = false;
            gvModuleFiles.OptionsView.RowAutoHeight = false;
            gvModuleFiles.RowHeight = 20;

            gvModuleFiles.RowCellClick += GvModuleFiles_RowCellClick;
            gvModuleFiles.DoubleClick += GvModuleFiles_DoubleClick;
            gvModuleFiles.CustomDrawCell += GvModuleFiles_CustomDrawCell;
            gvModuleFiles.RowStyle += GvModuleFiles_RowStyle;
            gvModuleFiles.SelectionChanged += GvModuleFiles_SelectionChanged;
        }

        // ── CustomDrawCell: 프로그램 행 Toggle 컬럼에 세로선 들여쓰기 ───────
        private void GvModuleFiles_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            var row = (sender as GridView)?.GetRow(e.RowHandle) as ScanGridRow;
            if (row == null || row.IsModule) return;

            if (e.Column.FieldName == "Toggle")
            {
                e.DefaultDraw();
                using var pen = new Pen(Color.FromArgb(180, 180, 180), 1);
                var cx = e.Bounds.Left + e.Bounds.Width / 2;
                e.Graphics.DrawLine(pen, cx, e.Bounds.Top, cx, e.Bounds.Bottom);
                e.Handled = true;
            }
        }

        private void GvModuleFiles_DoubleClick(object sender, EventArgs e)
        {
            var view = sender as GridView;
            if (view == null) return;

            var pt = view.GridControl.PointToClient(Control.MousePosition);
            var hitInfo = view.CalcHitInfo(pt);

            if (!hitInfo.InRow) return;

            var row = view.GetRow(hitInfo.RowHandle) as ScanGridRow;
            if (row == null) return;

            if (!string.IsNullOrWhiteSpace(row.ValidationErrors))
            {
                MessageBox.Show(
                    $"검증 오류:\n\n{row.ValidationErrors}",
                    row.IsModule ? $"DLL: {row.DllFileName}" : $"프로그램: {row.ProgId}",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            ToggleScanRowExpandAt(hitInfo.RowHandle);
        }

        private void GvModuleFiles_RowCellClick(object sender, RowCellClickEventArgs e)
        {
            if (e.Column.FieldName == "ExpandText")
            {
                ToggleScanRowExpandAt(e.RowHandle);
            }
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

            // Toggle 아이콘 갱신
            var item = _scannedFiles.FirstOrDefault(f => string.Equals(f.ModuleId, moduleId, StringComparison.OrdinalIgnoreCase));
            var dbVer = _dbActiveVersions.FirstOrDefault(v => string.Equals(v.ModuleId, moduleId, StringComparison.OrdinalIgnoreCase));
            if (item != null)
            {
                var valSummary = _moduleValidationErrors.TryGetValue(moduleId, out var vs) ? vs : null;
                _scanGridRows[rowIndex] = ScanGridRow.FromModule(item, isExpanded: true, valSummary, dbVer);
            }

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

            // Toggle 아이콘 갱신
            var item = _scannedFiles.FirstOrDefault(f => string.Equals(f.ModuleId, moduleId, StringComparison.OrdinalIgnoreCase));
            var dbVer = _dbActiveVersions.FirstOrDefault(v => string.Equals(v.ModuleId, moduleId, StringComparison.OrdinalIgnoreCase));
            if (item != null)
            {
                var valSummary = _moduleValidationErrors.TryGetValue(moduleId, out var vs) ? vs : null;
                _scanGridRows[rowIndex] = ScanGridRow.FromModule(item, isExpanded: false, valSummary, dbVer);
            }

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
            var topRowIndex = preserveScroll ? gvModuleFiles.TopRowIndex : 0;
            var focusedRowHandle = gvModuleFiles.FocusedRowHandle;

            dgvModuleFiles.DataSource = null;
            dgvModuleFiles.DataSource = _scanGridRows;
            gvModuleFiles.RefreshData();

            if (preserveScroll)
            {
                gvModuleFiles.TopRowIndex = topRowIndex;
                if (focusedRowHandle >= 0 && focusedRowHandle < gvModuleFiles.DataRowCount)
                    gvModuleFiles.FocusedRowHandle = focusedRowHandle;
            }
        }

        private void GvModuleFiles_RowStyle(object sender, RowStyleEventArgs e)
        {
            if (e.RowHandle < 0) return;
            var view = sender as GridView;
            var row = view?.GetRow(e.RowHandle) as ScanGridRow;
            if (row == null) return;

            var hasValidationError = !string.IsNullOrWhiteSpace(row.ValidationErrors);

            // ── 기본 배경 ─────────────────────────────────────────────────
            if (row.IsModule)
            {
                if (row.HasValidationError)
                {
                    e.Appearance.BackColor = Color.FromArgb(255, 220, 220);
                    e.Appearance.ForeColor = Color.FromArgb(150, 0, 0);
                }
                else if (row.IsNewModule)
                {
                    e.Appearance.BackColor = Color.FromArgb(230, 250, 230);
                    e.Appearance.ForeColor = Color.FromArgb(0, 100, 0);
                }
                else if (row.NeedsUpdate)
                {
                    e.Appearance.BackColor = Color.FromArgb(255, 245, 220);
                    e.Appearance.ForeColor = Color.FromArgb(130, 80, 0);
                }
                else
                {
                    e.Appearance.BackColor = Color.FromArgb(235, 242, 250);
                    e.Appearance.ForeColor = Color.FromArgb(30, 60, 100);
                }
                e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
            }
            else
            {
                e.Appearance.BackColor = hasValidationError
                    ? Color.FromArgb(255, 235, 235)
                    : Color.FromArgb(250, 250, 255);
                e.Appearance.ForeColor = hasValidationError
                    ? Color.FromArgb(150, 0, 0)
                    : Color.FromArgb(30, 30, 80);
            }

            // ── 선택 하이라이트 (붉은색 계통) ─────────────────────────────
            var isSelectedModule = !string.IsNullOrWhiteSpace(_selectedScanModuleId) &&
                                   string.Equals(row.ModuleId, _selectedScanModuleId, StringComparison.OrdinalIgnoreCase);
            var isSelectedProg   = !row.IsModule &&
                                   !string.IsNullOrWhiteSpace(_selectedScanProgId) &&
                                   string.Equals(row.ProgId, _selectedScanProgId, StringComparison.OrdinalIgnoreCase);

            if (isSelectedProg)
            {
                // 프로그램 선택: 선명한 로즈
                e.Appearance.BackColor = Color.FromArgb(255, 180, 180);
                e.Appearance.ForeColor = Color.FromArgb(120, 0, 0);
                e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
            }
            else if (isSelectedModule && row.IsModule)
            {
                // 모듈 선택: 진한 크림즌
                e.Appearance.BackColor = Color.FromArgb(200, 50, 50);
                e.Appearance.ForeColor = Color.White;
            }
            else if (isSelectedModule && !row.IsModule)
            {
                // 선택 모듈 소속 프로그램: 연한 살구
                e.Appearance.BackColor = Color.FromArgb(255, 220, 210);
                e.Appearance.ForeColor = Color.FromArgb(100, 30, 20);
            }
        }

        private sealed class ScanGridRow
        {
            public bool IsModule { get; init; }

            // ── 공통 ────────────────────────────────────────────────────────
            public string Toggle { get; init; } = string.Empty; // ▶/▼/○ (모듈), │ (프로그램)
            public string ModuleId { get; init; } = string.Empty;
            public string Category { get; init; } = string.Empty;
            public string SubSystem { get; init; } = string.Empty;

            // ── 모듈 전용 컬럼 ──────────────────────────────────────────────
            public string DllFileName { get; init; } = string.Empty; // DLL 파일명
            public string ModuleName { get; init; } = string.Empty; // 모듈명
            public string Version { get; init; } = string.Empty; // 파일 버전
            public string DbVersion { get; init; } = string.Empty; // DB 배포 버전
            public string ProgCount { get; init; } = string.Empty; // 화면 수 (N개)
            public string SyncMark { get; init; } = string.Empty; // ✓ 동일 / ↑ 업데이트 필요 / ★ 신규

            // ── 프로그램 전용 컬럼 ──────────────────────────────────────────
            public string ProgNo { get; init; } = string.Empty; // 순번
            public string ProgId { get; init; } = string.Empty; // 프로그램 ID
            public string ProgName { get; init; } = string.Empty; // 화면명
            public string ClassName { get; init; } = string.Empty; // 클래스명
            public string AuthLevelText { get; init; } = string.Empty; // 권한 텍스트

            // ── 검증 (모듬/프로그램 공통) ────────────────────────────────────
            public string ValidMark { get; init; } = string.Empty; // ✓ 정상 / ⚠ 오류
            public string ValidationErrors { get; init; } = string.Empty; // 오류 상세 (더블클릭용)

            // ── 스캔 상태 플래그 (스타일링용) ───────────────────────────────
            public bool HasValidationError => !string.IsNullOrWhiteSpace(ValidationErrors);
            public bool IsNewModule { get; init; } // DB에 없는 신규
            public bool NeedsUpdate { get; init; } // DB와 버전 다름

            public static ScanGridRow FromModule(
                ModuleFileItem item, bool isExpanded, string? validationSummary,
                ModuleVerDto? dbVer)
            {
                var isNew = dbVer == null && !string.IsNullOrWhiteSpace(item.ModuleId);
                var needsUpdate = dbVer != null &&
                                  !string.Equals(dbVer.Version ?? string.Empty,
                                                 item.Version ?? string.Empty,
                                                 StringComparison.OrdinalIgnoreCase);

                var syncMark = isNew ? "★ 신규"
                             : needsUpdate ? "↑ 업데이트"
                             : string.IsNullOrWhiteSpace(item.ModuleId) ? "?"
                             : "✓";

                return new ScanGridRow
                {
                    IsModule = true,
                    Toggle = item.ProgramCount <= 0 ? "○" : (isExpanded ? "▼" : "▶"),
                    ModuleId = item.ModuleId ?? string.Empty,
                    Category = item.SystemType ?? string.Empty,
                    SubSystem = item.SubSystem ?? string.Empty,
                    DllFileName = item.FileName ?? string.Empty,
                    ModuleName = item.ModuleName ?? string.Empty,
                    Version = item.Version ?? string.Empty,
                    DbVersion = dbVer?.Version ?? "(미등록)",
                    ProgCount = item.ProgramCount > 0 ? $"{item.ProgramCount}개" : "-",
                    SyncMark = syncMark,
                    ValidMark = string.IsNullOrWhiteSpace(validationSummary) ? "✓" : "⚠",
                    ValidationErrors = validationSummary ?? string.Empty,
                    IsNewModule = isNew,
                    NeedsUpdate = needsUpdate,
                    // 프로그램 전용 컬럼은 비움
                    ProgNo = string.Empty,
                    ProgId = string.Empty,
                    ProgName = string.Empty,
                    ClassName = string.Empty,
                    AuthLevelText = string.Empty,
                };
            }

            public static ScanGridRow FromProgram(string moduleId, ProgramDto p, int index, string? validationErrors)
            {
                var authText = p.AuthLevel switch
                {
                    1 => "일반",
                    2 => "관리자",
                    9 => "슈퍼",
                    _ => p.AuthLevel.ToString()
                };

                return new ScanGridRow
                {
                    IsModule = false,
                    Toggle = string.Empty,
                    ModuleId = moduleId,
                    Category = p.SystemType ?? string.Empty,
                    SubSystem = p.SubSystem ?? string.Empty,
                    // 모듈 전용 컬럼은 비움
                    DllFileName = string.Empty,
                    ModuleName = string.Empty,
                    Version = string.Empty,
                    DbVersion = string.Empty,
                    ProgCount = string.Empty,
                    SyncMark = string.Empty,
                    // 프로그램 전용 컬럼
                    ProgNo = (index + 1).ToString(),
                    ProgId = p.ProgId ?? string.Empty,
                    ProgName = p.ProgName ?? string.Empty,
                    ClassName = p.ClassName ?? string.Empty,
                    AuthLevelText = authText,
                    ValidMark = string.IsNullOrWhiteSpace(validationErrors) ? "✓" : "⚠",
                    ValidationErrors = validationErrors ?? string.Empty,
                };
            }
        }

        private static bool IsDesignMode()
        {
            return LicenseManager.UsageMode == LicenseUsageMode.Designtime;
        }

        private void InitFilterCombos()
        {
            _isRefreshingFilterCombos = true;
            try
            {
                cboFilterCategory.Properties.Items.Clear();
                cboFilterCategory.Properties.Items.Add("(ALL)");
                cboFilterCategory.Properties.Items.AddRange(new object[] { "ADM", "EMR", "OCS" });
                cboFilterCategory.SelectedIndex = 0;

                cboFilterSubSystem.Properties.Items.Clear();
                cboFilterSubSystem.Properties.Items.Add("(ALL)");
                cboFilterSubSystem.SelectedIndex = 0;
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
            LoadDbGrid();

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

                        if (info.HasValidationErrors)
                        {
                            _moduleValidationErrors[item.ModuleId] = info.GetValidationSummary();
                        }

                        if (!string.IsNullOrWhiteSpace(item.ModuleId) && info.Programs != null)
                        {
                            _scannedProgramsByModuleId[item.ModuleId] = info.Programs;

                            var progErrs = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                            foreach (var prog in info.Programs)
                            {
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
                    }

                    list.Add(item);
                }
                catch
                {
                }
            }

            _scannedFiles = list.OrderBy(x => x.RelativePath).ToList();

            RebuildScanGridRowsFromScannedFiles();
            RebindScanGrid(preserveScroll: false);

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

                cboFilterSubSystem.Properties.Items.Clear();
                cboFilterSubSystem.Properties.Items.Add("(ALL)");
                foreach (var s in subs) cboFilterSubSystem.Properties.Items.Add(s);

                if (cboFilterSubSystem.SelectedIndex != 0)
                {
                    cboFilterSubSystem.SelectedIndex = 0;
                }

                if (!string.IsNullOrWhiteSpace(current) && cboFilterSubSystem.Properties.Items.Contains(current))
                {
                    cboFilterSubSystem.SelectedItem = current;
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
            LoadDbGrid();
        }

        private void GvModuleFiles_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            var view = sender as GridView;
            if (view == null) return;

            if (view.SelectedRowsCount <= 0) return;

            var rowHandle = view.GetSelectedRows()[0];
            var row = view.GetRow(rowHandle) as ScanGridRow;
            if (row == null) return;

            if (string.IsNullOrWhiteSpace(row.ModuleId)) return;

            _selectedScanModuleId = row.ModuleId;
            _selectedScanProgId   = row.IsModule ? null : row.ProgId;

            RefreshCenterForSelectedScanModule(row.ModuleId);

            if (row.IsModule)
                ScrollDbGridToModule(row.ModuleId);
            else
                ScrollDbGridToProgram(row.ModuleId, row.ProgId);
        }

        private void RefreshCenterForSelectedScanModule(string moduleId)
        {
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

            // _programEditRows는 저장(SaveModule) 시에만 사용 - dgvDbVersions와 무관
            var progRepo = Program.ServiceProvider.GetRequiredService<IProgramRepository>();
            var dbModuleProgs = progRepo.GetProgramsByModuleId(moduleId);
            var dbByProgId = dbModuleProgs
                .Where(p => !string.IsNullOrWhiteSpace(p.ProgId))
                .GroupBy(p => p.ProgId, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);

            _programEditRows = new BindingList<ProgramEditRow>();

            if (_scannedProgramsByModuleId.TryGetValue(moduleId, out var scannedProgs))
            {
                foreach (var sp in scannedProgs)
                {
                    var progId = sp.ProgId ?? string.Empty;
                    var screenName = sp.ProgName ?? string.Empty;
                    var className = sp.ClassName ?? string.Empty;
                    var auth = sp.AuthLevel;

                    dbByProgId.TryGetValue(progId, out var dbp);
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

            // DB 그리드(dgvDbVersions)는 DbGridRow 목록을 유지 - 덮어쓰지 않음
            // 선택된 모듈로만 스크롤/하이라이트
            gvDbVersions.RefreshData();
        }

        private sealed class ProgramEditRow
        {
            public string DbStatus { get; set; } = string.Empty;
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

        // ── DB 그리드 내부 row 모델 ──────────────────────────────────────────
        // 모듈 행과 프로그램 행이 별도 컬럼을 사용하도록 필드를 분리
        private sealed class DbGridRow
        {
            public bool IsModule { get; init; }

            // ── 공통 ──────────────────────────────────────────────────────
            // [+/-] 토글 텍스트 (모듈 행에만 표시)
            public string Toggle { get; init; } = string.Empty;
            public string ModuleId { get; init; } = string.Empty;
            public string Category { get; init; } = string.Empty;
            public string SubSystem { get; init; } = string.Empty;

            // ── 모듈 전용 컬럼 ────────────────────────────────────────────
            public string DllFileName { get; init; } = string.Empty;  // DLL 파일명
            public string ModuleName { get; init; } = string.Empty;  // 모듈명
            public string Version { get; init; } = string.Empty;  // 배포 버전
            public string ProgCount { get; init; } = string.Empty;  // 프로그램 수 (N개)

            // ── 프로그램 전용 컬럼 ────────────────────────────────────────
            public string ProgNo { get; init; } = string.Empty;  // 순번
            public string ProgId { get; init; } = string.Empty;  // 프로그램 ID
            public string ProgName { get; init; } = string.Empty;  // 화면명
            public string ClassName { get; init; } = string.Empty;  // 클래스명
            public string AuthLevelText { get; init; } = string.Empty;  // 권한 (1=일반, 2=관리자...)
            public string ActiveMark { get; init; } = string.Empty;  // ● 활성 / ○ 비활성

            public static DbGridRow FromModule(ModuleMstDto m, ModuleVerDto? ver, bool isExpanded, int progCount)
            {
                return new DbGridRow
                {
                    IsModule = true,
                    Toggle = progCount <= 0 ? "○" : (isExpanded ? "▼" : "▶"),
                    ModuleId = m.ModuleId ?? string.Empty,
                    Category = m.Category ?? string.Empty,
                    SubSystem = m.SubSystem ?? string.Empty,
                    DllFileName = m.FileName ?? string.Empty,
                    ModuleName = m.ModuleName ?? string.Empty,
                    Version = ver?.Version ?? "(미배포)",
                    ProgCount = progCount > 0 ? $"{progCount}개" : "-",
                    // 프로그램 전용 컬럼은 비움
                    ProgNo = string.Empty,
                    ProgId = string.Empty,
                    ProgName = string.Empty,
                    ClassName = string.Empty,
                    AuthLevelText = string.Empty,
                    ActiveMark = string.Empty,
                };
            }

            public static DbGridRow FromProgram(ProgramDto p, int index)
            {
                var authText = p.AuthLevel switch
                {
                    1 => "일반",
                    2 => "관리자",
                    9 => "슈퍼",
                    _ => p.AuthLevel.ToString()
                };

                return new DbGridRow
                {
                    IsModule = false,
                    Toggle = string.Empty,
                    ModuleId = p.ModuleId ?? string.Empty,
                    Category = p.SystemType ?? string.Empty,
                    SubSystem = p.SubSystem ?? string.Empty,
                    // 모듈 전용 컬럼은 비움
                    DllFileName = string.Empty,
                    ModuleName = string.Empty,
                    Version = string.Empty,
                    ProgCount = string.Empty,
                    // 프로그램 전용 컬럼
                    ProgNo = (index + 1).ToString(),
                    ProgId = p.ProgId ?? string.Empty,
                    ProgName = p.ProgName ?? string.Empty,
                    ClassName = p.ClassName ?? string.Empty,
                    AuthLevelText = authText,
                    ActiveMark = p.IsActive == "Y" ? "●" : "○",
                };
            }
        }

        // ── DB 그리드 컬럼 구성 ─────────────────────────────────────────────
        // 모듈 행 : Toggle | ModuleId | Category | SubSystem | DllFileName | ModuleName | Version | ProgCount
        // 프로그램행: (들여쓰기) ProgNo | ProgId | ProgName | ClassName | AuthLevelText | ActiveMark
        // → 한 그리드에서 행 타입에 따라 필요한 컬럼만 값이 채워지고 나머지는 빈 문자열
        private void SetupDbGrid()
        {
            gvDbVersions.Columns.Clear();

            // 공통
            var colToggle = gvDbVersions.Columns.AddVisible("Toggle", "");
            colToggle.Width = 24;
            colToggle.OptionsColumn.FixedWidth = true;

            var colCategory = gvDbVersions.Columns.AddVisible("Category", "시스템");
            colCategory.Width = 52;

            var colSub = gvDbVersions.Columns.AddVisible("SubSystem", "서브");
            colSub.Width = 52;

            // 모듈 전용
            var colDll = gvDbVersions.Columns.AddVisible("DllFileName", "DLL 파일명");
            colDll.Width = 200;

            var colMName = gvDbVersions.Columns.AddVisible("ModuleName", "모듈명");
            colMName.Width = 180;

            var colVer = gvDbVersions.Columns.AddVisible("Version", "배포버전");
            colVer.Width = 80;

            var colPCnt = gvDbVersions.Columns.AddVisible("ProgCount", "화면수");
            colPCnt.Width = 46;
            colPCnt.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            colPCnt.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;

            // 프로그램 전용
            var colNo = gvDbVersions.Columns.AddVisible("ProgNo", "#");
            colNo.Width = 28;
            colNo.OptionsColumn.FixedWidth = true;
            colNo.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;

            var colProgId = gvDbVersions.Columns.AddVisible("ProgId", "프로그램 ID");
            colProgId.Width = 140;

            var colProgName = gvDbVersions.Columns.AddVisible("ProgName", "화면명");
            colProgName.Width = 180;

            var colClass = gvDbVersions.Columns.AddVisible("ClassName", "클래스명");
            colClass.Width = 200;

            var colAuth = gvDbVersions.Columns.AddVisible("AuthLevelText", "권한");
            colAuth.Width = 52;
            colAuth.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;

            var colActive = gvDbVersions.Columns.AddVisible("ActiveMark", "활성");
            colActive.Width = 36;
            colActive.OptionsColumn.FixedWidth = true;
            colActive.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;

            // ModuleId 는 마지막에 (참조용, 너비 최소)
            var colMid = gvDbVersions.Columns.AddVisible("ModuleId", "모듈 ID");
            colMid.Width = 220;

            gvDbVersions.OptionsBehavior.Editable = false;
            gvDbVersions.OptionsView.ShowGroupPanel = false;
            gvDbVersions.OptionsView.EnableAppearanceEvenRow = false;
            gvDbVersions.OptionsView.EnableAppearanceOddRow = false;
            gvDbVersions.OptionsView.RowAutoHeight = false;
            gvDbVersions.RowHeight = 20;

            gvDbVersions.RowCellClick += GvDbVersions_RowCellClick;
            gvDbVersions.CustomDrawCell += GvDbVersions_CustomDrawCell;
            gvDbVersions.RowStyle += GvDbVersions_RowStyle;
        }

        // ── CustomDrawCell: 프로그램 행 들여쓰기 시각화 ─────────────────────
        private void GvDbVersions_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            var row = (sender as GridView)?.GetRow(e.RowHandle) as DbGridRow;
            if (row == null || row.IsModule) return;

            // 프로그램 행의 Toggle 컬럼을 회색 세로선으로 표현
            if (e.Column.FieldName == "Toggle")
            {
                e.DefaultDraw();
                using var pen = new Pen(Color.FromArgb(180, 180, 180), 1);
                var cx = e.Bounds.Left + e.Bounds.Width / 2;
                e.Graphics.DrawLine(pen, cx, e.Bounds.Top, cx, e.Bounds.Bottom);
                e.Handled = true;
            }
        }

        // ── DB 그리드 행 생성 ───────────────────────────────────────────────
        private void LoadDbGrid()
        {
            if (_moduleRepo == null) return;

            var progRepo = Program.ServiceProvider.GetRequiredService<IProgramRepository>();
            var allPrograms = progRepo.GetAllPrograms();

            var progsByModule = allPrograms
                .Where(p => !string.IsNullOrWhiteSpace(p.ModuleId))
                .GroupBy(p => p.ModuleId!, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => g.OrderBy(p => p.ProgId).ToList(), StringComparer.OrdinalIgnoreCase);

            var verByModule = _dbActiveVersions
                .GroupBy(v => v.ModuleId, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);

            var category = SelectedCategory;
            var subSystem = SelectedSubSystem;

            var filteredModules = _dbMasters
                .Where(m => category == null || string.Equals(m.Category, category, StringComparison.OrdinalIgnoreCase))
                .Where(m => subSystem == null || string.Equals(m.SubSystem, subSystem, StringComparison.OrdinalIgnoreCase))
                .OrderBy(m => m.Category)
                .ThenBy(m => m.SubSystem)
                .ThenBy(m => m.ModuleId)
                .ToList();

            _dbGridRows = new List<DbGridRow>(filteredModules.Count * 4);

            foreach (var m in filteredModules)
            {
                verByModule.TryGetValue(m.ModuleId, out var ver);
                progsByModule.TryGetValue(m.ModuleId, out var progs);
                var progCount = progs?.Count ?? 0;
                var isExpanded = _expandedDbModuleIds.Contains(m.ModuleId);

                _dbGridRows.Add(DbGridRow.FromModule(m, ver, isExpanded, progCount));

                if (isExpanded && progs != null)
                {
                    for (var i = 0; i < progs.Count; i++)
                        _dbGridRows.Add(DbGridRow.FromProgram(progs[i], i));
                }
            }

            RebindDbGrid(preserveScroll: false);
        }

        private void RebindDbGrid(bool preserveScroll)
        {
            var topRow = preserveScroll ? gvDbVersions.TopRowIndex : 0;
            var focusedRow = preserveScroll ? gvDbVersions.FocusedRowHandle : -1;

            dgvDbVersions.DataSource = null;
            dgvDbVersions.DataSource = _dbGridRows;
            gvDbVersions.RefreshData();

            if (preserveScroll)
            {
                gvDbVersions.TopRowIndex = topRow;
                if (focusedRow >= 0 && focusedRow < gvDbVersions.DataRowCount)
                    gvDbVersions.FocusedRowHandle = focusedRow;
            }
        }

        // ── DB 그리드 확장/축소 ─────────────────────────────────────────────
        private void GvDbVersions_RowCellClick(object sender, RowCellClickEventArgs e)
        {
            if (e.Column.FieldName == "Toggle")
                ToggleDbRowExpandAt(e.RowHandle);
        }

        private void ToggleDbRowExpandAt(int rowIndex)
        {
            if (rowIndex < 0 || rowIndex >= _dbGridRows.Count) return;
            var row = _dbGridRows[rowIndex];
            if (!row.IsModule) return;

            if (_expandedDbModuleIds.Contains(row.ModuleId))
                CollapseDbModuleRow(rowIndex, row.ModuleId);
            else
                ExpandDbModuleRow(rowIndex, row.ModuleId);

            RebindDbGrid(preserveScroll: true);
        }

        private void ExpandDbModuleRow(int rowIndex, string moduleId)
        {
            var progRepo = Program.ServiceProvider.GetRequiredService<IProgramRepository>();
            var progs = progRepo.GetProgramsByModuleId(moduleId)
                                   .OrderBy(p => p.ProgId)
                                   .ToList();

            _expandedDbModuleIds.Add(moduleId);

            _dbGridRows[rowIndex] = DbGridRow.FromModule(
                _dbMasters.First(m => string.Equals(m.ModuleId, moduleId, StringComparison.OrdinalIgnoreCase)),
                _dbActiveVersions.FirstOrDefault(v => string.Equals(v.ModuleId, moduleId, StringComparison.OrdinalIgnoreCase)),
                isExpanded: true,
                progCount: progs.Count);

            var insertIndex = rowIndex + 1;
            for (var i = 0; i < progs.Count; i++)
                _dbGridRows.Insert(insertIndex++, DbGridRow.FromProgram(progs[i], i));
        }

        private void CollapseDbModuleRow(int rowIndex, string moduleId)
        {
            _expandedDbModuleIds.Remove(moduleId);

            var progCount = 0;
            var i = rowIndex + 1;
            while (i < _dbGridRows.Count && !_dbGridRows[i].IsModule)
            {
                if (string.Equals(_dbGridRows[i].ModuleId, moduleId, StringComparison.OrdinalIgnoreCase))
                { _dbGridRows.RemoveAt(i); progCount++; }
                else
                    i++;
            }

            _dbGridRows[rowIndex] = DbGridRow.FromModule(
                _dbMasters.First(m => string.Equals(m.ModuleId, moduleId, StringComparison.OrdinalIgnoreCase)),
                _dbActiveVersions.FirstOrDefault(v => string.Equals(v.ModuleId, moduleId, StringComparison.OrdinalIgnoreCase)),
                isExpanded: false,
                progCount: progCount);
        }

        // ── DB 그리드 행 스타일 ─────────────────────────────────────────────
        private void GvDbVersions_RowStyle(object sender, RowStyleEventArgs e)
        {
            if (e.RowHandle < 0) return;
            var row = (sender as GridView)?.GetRow(e.RowHandle) as DbGridRow;
            if (row == null) return;

            // ── 기본 배경 ─────────────────────────────────────────────────
            if (row.IsModule)
            {
                e.Appearance.BackColor = Color.FromArgb(235, 242, 250);
                e.Appearance.ForeColor = Color.FromArgb(30, 60, 100);
                e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
            }
            else
            {
                e.Appearance.BackColor = row.ActiveMark == "●"
                    ? Color.FromArgb(250, 250, 255)
                    : Color.FromArgb(242, 242, 242);
                e.Appearance.ForeColor = row.ActiveMark == "●"
                    ? Color.FromArgb(30, 30, 80)
                    : Color.FromArgb(150, 150, 150);
            }

            // ── 선택 하이라이트 (붉은색 계통) ─────────────────────────────
            var isSelectedModule = !string.IsNullOrWhiteSpace(_selectedScanModuleId) &&
                                   string.Equals(row.ModuleId, _selectedScanModuleId, StringComparison.OrdinalIgnoreCase);
            var isSelectedProg   = !row.IsModule &&
                                   !string.IsNullOrWhiteSpace(_selectedScanProgId) &&
                                   string.Equals(row.ProgId, _selectedScanProgId, StringComparison.OrdinalIgnoreCase);

            if (isSelectedProg)
            {
                // 프로그램 선택: 선명한 로즈
                e.Appearance.BackColor = Color.FromArgb(255, 180, 180);
                e.Appearance.ForeColor = Color.FromArgb(120, 0, 0);
                e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
            }
            else if (isSelectedModule && row.IsModule)
            {
                // 모듈 선택: 진한 크림즌
                e.Appearance.BackColor = Color.FromArgb(200, 50, 50);
                e.Appearance.ForeColor = Color.White;
            }
            else if (isSelectedModule && !row.IsModule)
            {
                // 선택 모듈 소속 프로그램: 연한 살구
                e.Appearance.BackColor = Color.FromArgb(255, 220, 210);
                e.Appearance.ForeColor = Color.FromArgb(100, 30, 20);
            }
        }

        // ── 좌측 스캔그리드 선택 시 DB 그리드 해당 모듈로 스크롤 ──────────
        private void ScrollDbGridToModule(string moduleId)
        {
            for (var i = 0; i < _dbGridRows.Count; i++)
            {
                if (_dbGridRows[i].IsModule &&
                    string.Equals(_dbGridRows[i].ModuleId, moduleId, StringComparison.OrdinalIgnoreCase))
                {
                    gvDbVersions.FocusedRowHandle = i;
                    gvDbVersions.TopRowIndex = Math.Max(0, i - 2);
                    gvDbVersions.RefreshData();
                    return;
                }
            }
        }

        // ── 좌측에서 프로그램 선택 시 DB 그리드에서 해당 프로그램으로 스크롤 ─
        private void ScrollDbGridToProgram(string moduleId, string progId)
        {
            if (string.IsNullOrWhiteSpace(progId)) return;

            // 1. 해당 모듈이 DB 그리드에 펼쳐져 있는지 확인, 없으면 먼저 펼침
            var moduleRowIndex = -1;
            for (var i = 0; i < _dbGridRows.Count; i++)
            {
                if (_dbGridRows[i].IsModule &&
                    string.Equals(_dbGridRows[i].ModuleId, moduleId, StringComparison.OrdinalIgnoreCase))
                {
                    moduleRowIndex = i;
                    break;
                }
            }

            if (moduleRowIndex < 0) return;

            if (!_expandedDbModuleIds.Contains(moduleId))
            {
                ExpandDbModuleRow(moduleRowIndex, moduleId);
                RebindDbGrid(preserveScroll: false);
            }

            // 2. 프로그램 행 찾아 스크롤
            for (var i = 0; i < _dbGridRows.Count; i++)
            {
                if (!_dbGridRows[i].IsModule &&
                    string.Equals(_dbGridRows[i].ModuleId, moduleId, StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(_dbGridRows[i].ProgId, progId, StringComparison.OrdinalIgnoreCase))
                {
                    gvDbVersions.FocusedRowHandle = i;
                    gvDbVersions.TopRowIndex = Math.Max(0, i - 3);
                    gvDbVersions.RefreshData();
                    return;
                }
            }
        }
    }

}