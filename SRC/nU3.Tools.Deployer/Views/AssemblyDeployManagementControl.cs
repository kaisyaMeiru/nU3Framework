using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Extensions.Configuration;
using DevExpress.XtraGrid.Views.Grid; // GridView
using nU3.Connectivity;
using nU3.Core.Repositories;
using nU3.Models;
using nU3.Core.UI;
using nU3.Core.Interfaces; // BaseWorkControl

namespace nU3.Tools.Deployer.Views
{
    /// <summary>
    /// 프레임워크 구성요소(Component: DLL/EXE 등)의 배포를 관리하는 사용자 컨트롤입니다.
    /// </summary>
    public partial class AssemblyDeployManagementControl : BaseWorkControl
    {
        // 주입되는 리포지토리 및 파일전송 서비스
        private IComponentRepository _componentRepo;
        private IFileTransferService? _fileTransferService;

        // 서버 상의 컴포넌트 루트 경로 (기본: "Patch")
        private string _serverComponentPath = nU3.Core.Services.ModuleLoaderService.FRAMEWORKS_DIR;

        // 서버 전송 사용 여부 (설정에 따라 true/false)
        private bool _useServerTransfer = true;

        // 내부 데이터 캐시
        private List<ComponentMstDto> _components = new();
        private List<ComponentEditRow> _editRows = new();

        /// <summary>
        /// ComponentType별 기본 설치경로, 우선순위, 필수 여부 매핑 테이블
        /// </summary>
        private static readonly Dictionary<ComponentType, (string InstallPath, int Priority, bool IsRequired)> ComponentTypeDefaults = new()
        {
            { ComponentType.ScreenModule,    ("Modules",           100, false) },
            { ComponentType.FrameworkCore,   ("",                   10, true)  },
            { ComponentType.SharedLibrary,   ("",                   50, true) },
            { ComponentType.Executable,      ("",                    5, true)  },
            { ComponentType.Configuration,   ("",                  200, true) },
            { ComponentType.Resource,        ("resources",         300, false) },
            { ComponentType.Plugin,          ("plugins",           150, false) },
            { ComponentType.Json,           ("",                   100, true) },
            { ComponentType.Xml,            ("",                   100, true) },
            { ComponentType.Image,          ("resources/images",   100, true) },
            { ComponentType.Other,           ("",                  500, false) },
        };

        /// <summary>
        /// 디자이너 전용 생성자입니다.
        /// </summary>
        public AssemblyDeployManagementControl()
        {
            InitializeComponent();
            Dock = DockStyle.Fill;
        }

        public void Initialize(IComponentRepository componentRepo, IConfiguration configuration, IFileTransferService fileTransferService)
        {
            _componentRepo = componentRepo;
            _fileTransferService = fileTransferService;

            // 서버 연결 설정 로드
            var serverEnabled = true;
            _serverComponentPath = nU3.Core.Services.ModuleLoaderService.FRAMEWORKS_DIR;
            _useServerTransfer = serverEnabled;

            SetupGrids();
            LoadData();
        }

        private void SetupGrids()
        {
            // 메인 그리드
            _gvComponents.Columns.Clear();
            AddGridColumn(_gvComponents, "ComponentId", "아이디", 150);
            AddGridColumn(_gvComponents, "ComponentType", "유형", 100);
            AddGridColumn(_gvComponents, "ComponentName", "이름", 150);
            AddGridColumn(_gvComponents, "FileName", "파일", 150);
            AddGridColumn(_gvComponents, "InstallPath", "경로", 150);
            AddGridColumn(_gvComponents, "Priority", "우선순위", 60);
            
            _gvComponents.OptionsBehavior.Editable = false;
            _gvComponents.OptionsView.ShowGroupPanel = false;
            _gvComponents.SelectionChanged += DgvComponents_SelectionChanged;

            // 버전 그리드
            _gvVersions.Columns.Clear();
            AddGridColumn(_gvVersions, "Version", "버전", 80);
            AddGridColumn(_gvVersions, "FileHash", "해시", 100);
            AddGridColumn(_gvVersions, "FileSize", "크기", 80);
            AddGridColumn(_gvVersions, "IsActive", "활성", 60);
            AddGridColumn(_gvVersions, "RegDate", "등록일", 120);

            _gvVersions.OptionsBehavior.Editable = false;
            _gvVersions.OptionsView.ShowGroupPanel = false;
        }

        private void AddGridColumn(GridView view, string fieldName, string caption, int width)
        {
            var col = view.Columns.AddVisible(fieldName, caption);
            col.Width = width;
        }

        private void BtnRefresh_Click(object? sender, EventArgs e)
        {
            LoadData();
        }

        private void TxtFileName_TextChanged(object sender, EventArgs e) => UpdatePathPreview();
        private void TxtInstallPath_TextChanged(object sender, EventArgs e) => UpdatePathPreview();

        private void CboComponentType_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (_cboComponentType.SelectedItem == null) return;

            if (Enum.TryParse<ComponentType>(_cboComponentType.SelectedItem.ToString(), out var componentType))
            {
                if (ComponentTypeDefaults.TryGetValue(componentType, out var defaults))
                {
                    _txtInstallPath.Text = defaults.InstallPath;
                    HighlightControl(_txtInstallPath);

                    _nudPriority.Value = defaults.Priority;
                    HighlightControl(_nudPriority);

                    _chkRequired.Checked = defaults.IsRequired;
                    HighlightControl(_chkRequired);

                    UpdatePathPreview();
                }
            }
        }

        private async void HighlightControl(Control control)
        {
            var originalBackColor = control.BackColor;
            control.BackColor = System.Drawing.Color.LightYellow;
            await System.Threading.Tasks.Task.Delay(1000);
            if (!control.IsDisposed)
            {
                control.BackColor = originalBackColor;
            }
        }
        
        private void BtnNewComponent_Click(object? sender, EventArgs e)
        {
            ClearDetailFields();
            _cboComponentType.SelectedIndex = 0;
            _txtComponentId.Focus();
        }

        private void LoadData()
        {
            if (_componentRepo == null) return;

            try
            {
                _components = _componentRepo.GetAllComponents();
                _editRows = _components.Select(c => new ComponentEditRow
                {
                    ComponentId = c.ComponentId,
                    ComponentType = c.ComponentType.ToString(),
                    ComponentName = c.ComponentName,
                    FileName = c.FileName,
                    InstallPath = c.InstallPath ?? "",
                    GroupName = c.GroupName ?? "",
                    IsRequired = c.IsRequired ? "Y" : "N",
                    Priority = c.Priority
                }).ToList();

                _dgvComponents.DataSource = null;
                _dgvComponents.DataSource = _editRows;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"데이터 로드 실패: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DgvComponents_SelectionChanged(object? sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            if (_gvComponents.SelectedRowsCount <= 0 || _componentRepo == null || _editRows == null || _editRows.Count == 0) return;
            
            var rowHandle = _gvComponents.GetSelectedRows()[0];
            if (rowHandle < 0 || rowHandle >= _editRows.Count) return;

            var row = _editRows[rowHandle];
            var component = _components.FirstOrDefault(c => c.ComponentId == row.ComponentId);

            if (component != null)
            {
                _cboComponentType.SelectedIndexChanged -= CboComponentType_SelectedIndexChanged;

                _txtComponentId.Text = component.ComponentId;
                _txtComponentName.Text = component.ComponentName;
                _txtFileName.Text = component.FileName;
                _txtInstallPath.Text = component.InstallPath ?? "";
                _txtGroupName.Text = component.GroupName ?? "";
                _txtDescription.Text = component.Description ?? "";
                _txtDependencies.Text = component.Dependencies ?? "";
                _cboComponentType.SelectedItem = component.ComponentType.ToString();
                _nudPriority.Value = component.Priority;
                _chkRequired.Checked = component.IsRequired;
                _chkAutoUpdate.Checked = component.AutoUpdate;

                _cboComponentType.SelectedIndexChanged += CboComponentType_SelectedIndexChanged;

                UpdatePathPreview();

                var versions = _componentRepo.GetVersionHistory(component.ComponentId);
                _dgvVersions.DataSource = versions.Select(v => new
                {
                    v.Version,
                    FileHash = v.FileHash?.Length > 8 ? v.FileHash.Substring(0, 8) + "..." : v.FileHash,
                    FileSize = $"{v.FileSize / 1024.0:N0} KB",
                    v.IsActive,
                    RegDate = v.RegDate.ToString("yyyy-MM-dd HH:mm")
                }).ToList();
            }
        }

        private void UpdatePathPreview()
        {
            if (_lblPathPreview != null)
            {
                var installPath = _txtInstallPath?.Text?.Trim() ?? "";
                var fileName = _txtFileName?.Text?.Trim() ?? "";

                if (!string.IsNullOrEmpty(fileName))
                {
                    var preview = string.IsNullOrEmpty(installPath)
                        ? $"{{실행경로}}\\{fileName}"
                        : $"{{실행경로}}\\{installPath}\\{fileName}";

                    _lblPathPreview.Text = $"최종 경로: {preview}";
                }
                else
                {
                    _lblPathPreview.Text = "최종 경로: {실행경로}\\{파일명}";
                }
            }
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            if (_componentRepo == null) return;

            try
            {
                var component = new ComponentMstDto
                {
                    ComponentId = _txtComponentId.Text.Trim(),
                    ComponentName = _txtComponentName.Text.Trim(),
                    FileName = _txtFileName.Text.Trim(),
                    InstallPath = _txtInstallPath.Text.Trim(),
                    GroupName = _txtGroupName.Text.Trim(),
                    Description = _txtDescription.Text.Trim(),
                    Dependencies = _txtDependencies.Text.Trim(),
                    ComponentType = Enum.TryParse<ComponentType>(_cboComponentType.SelectedItem?.ToString(), out var type)
                        ? type : ComponentType.SharedLibrary,
                    Priority = (int)_nudPriority.Value,
                    IsRequired = _chkRequired.Checked,
                    AutoUpdate = _chkAutoUpdate.Checked,
                    IsActive = "Y"
                };

                if (string.IsNullOrEmpty(component.ComponentId) || string.IsNullOrEmpty(component.FileName))
                {
                    MessageBox.Show("Component ID와 파일명은 필수입니다.", "확인", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                _componentRepo.SaveComponent(component);
                MessageBox.Show("저장되었습니다.", "완료", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"저장 실패: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnDelete_Click(object? sender, EventArgs e)
        {
            if (_componentRepo == null || string.IsNullOrEmpty(_txtComponentId.Text)) return;

            if (MessageBox.Show($"'{_txtComponentId.Text}'을(를) 삭제하시겠습니까?", "삭제 확인",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            try
            {
                _componentRepo.DeleteComponent(_txtComponentId.Text);
                MessageBox.Show("삭제되었습니다.", "완료", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearDetailFields();
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"삭제 실패: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnSmartDeploy_Click(object? sender, EventArgs e)
        {
            if (_componentRepo == null) return;

            using var ofd = new OpenFileDialog
            {
                Filter = "DLL/EXE/JSON 파일 (*.dll;*.exe;*.json;*.xml)|*.dll;*.exe;*.json;*.xml|모든 파일 (*.*)|*.*",
                Title = "배포할 파일 선택",
                Multiselect = true
            };

            if (ofd.ShowDialog() != DialogResult.OK) return;

            await RunDeployAsync(ofd.FileNames);
        }

        private async void BtnBulkDeploy_Click(object? sender, EventArgs e)
        {
            if (_componentRepo == null) return;

            using var fbd = new FolderBrowserDialog { Description = "배포할 파일들이 있는 폴더 선택" };

            if (fbd.ShowDialog() != DialogResult.OK) return;

            try
            {
                var files = Directory.GetFiles(fbd.SelectedPath, "*.*", SearchOption.AllDirectories)
                    .Where(f => f.EndsWith(".dll", StringComparison.OrdinalIgnoreCase)
                                || f.EndsWith(".exe", StringComparison.OrdinalIgnoreCase)
                                || f.EndsWith(".json", StringComparison.OrdinalIgnoreCase)
                                || f.EndsWith(".xml", StringComparison.OrdinalIgnoreCase)
                                || f.EndsWith(".config", StringComparison.OrdinalIgnoreCase)
                                || f.EndsWith(".pdb", StringComparison.OrdinalIgnoreCase)
                                || f.EndsWith(".dll.config", StringComparison.OrdinalIgnoreCase)
                               )
                    .ToArray();

                if (files.Length == 0)
                {
                    MessageBox.Show("배포할 파일이 없습니다.", "확인", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var rootDirectory = fbd.SelectedPath;
                var folderInfo = $"최종 폴더: {rootDirectory}";
                var totalFiles = $"총 파일 수: {files.Length:N0}개";
                var fileInfo = files.Length > 0
                    ? $"\n예시 파일:\n{files[0]}\n{files[Math.Min(1, files.Length - 1)]}"
                    : "";

                var message = $"{folderInfo}\n{totalFiles}{fileInfo}\n\n계속 하시겠습니까?";

                if (MessageBox.Show(message, "폴더 배포 확인",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    return;

                await RunDeployAsync(files, rootDirectory);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"배포 실패: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task RunDeployAsync(string[] filePaths, string? rootDirectory = null)
        {
            if (filePaths.Length == 0)
                return;

            ToggleUiEnabled(false);

            try
            {
                var result = await DeployFilesAsync(filePaths, rootDirectory);

                var msg = result.failed > 0 ? $"{result.success}개 성공, {result.failed}개 실패" : $"{result.success}개 파일 배포 완료!";
                MessageBox.Show(msg, "배포 결과", MessageBoxButtons.OK, result.failed > 0 ? MessageBoxIcon.Warning : MessageBoxIcon.Information);
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"배포 실패: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                ToggleUiEnabled(true);
            }
        }

        private async Task<(int success, int failed)> DeployFilesAsync(string[] filePaths, string? rootDirectory = null)
        {
            int success = 0;
            int failed = 0;

            if (string.IsNullOrEmpty(rootDirectory))
            {
                var directories = filePaths.Select(Path.GetDirectoryName).Distinct().ToList();
                rootDirectory = directories.Count == 1 ? directories.First() : "";
            }

            string homeDirectory = string.Empty;
            if (_useServerTransfer && _fileTransferService != null)
            {
                homeDirectory = await _fileTransferService.GetHomeDirectoryAsync();
                Console.WriteLine($"홈 디렉터리: {homeDirectory}");
            }

            Console.WriteLine($"루트 디렉터리: {rootDirectory}");
            Console.WriteLine($"{filePaths.Length}개 파일을 배포 중...");

            foreach (var filePath in filePaths)
            {
                try
                {
                    await DeployFileAsync(filePath, homeDirectory, rootDirectory);
                    success++;

                    var relativePath = string.IsNullOrEmpty(rootDirectory)
                        ? Path.GetFileName(filePath)
                        : filePath.Substring(rootDirectory.Length).TrimStart('\\', '/');
                    Console.WriteLine($"배포됨: {relativePath}");
                }
                catch (Exception ex)
                {
                    failed++;
                    System.Diagnostics.Debug.WriteLine($"배포 실패: {filePath}: {ex.Message}");
                }
            }

            return (success, failed);
        }

        private async Task DeployFileAsync(string filePath, string homeDirectory, string? rootDirectory = null)
        {
            var fileInfo = new FileInfo(filePath);
            var fileName = fileInfo.Name;
            var isExe = fileName.EndsWith(".exe", StringComparison.OrdinalIgnoreCase);

            string version = "1.0.0.0";
            try
            {
                var assemblyName = System.Reflection.AssemblyName.GetAssemblyName(filePath);
                version = assemblyName.Version?.ToString() ?? "1.0.0.0";
            }
            catch
            {
                var versionInfo = System.Diagnostics.FileVersionInfo.GetVersionInfo(filePath);
                version = versionInfo.FileVersion ?? "1.0.0.0";
            }

            string componentId;
            string relativeSubfolder = "";

            if (!string.IsNullOrEmpty(rootDirectory) && filePath.StartsWith(rootDirectory, StringComparison.OrdinalIgnoreCase))
            {
                var relativePath = filePath.Substring(rootDirectory.Length);

                while (relativePath.Length > 0 && (relativePath[0] == '\\' || relativePath[0] == '/'))
                {
                    relativePath = relativePath.Substring(1);
                }

                relativePath = relativePath.Replace("\\", "/");

                var lastSlashIndex = relativePath.LastIndexOf('/');
                if (lastSlashIndex >= 0)
                {
                    relativeSubfolder = relativePath.Substring(0, lastSlashIndex + 1);
                    componentId = relativePath;
                }
                else
                {
                    componentId = fileName;
                }
            }
            else
            {
                componentId = fileName;
            }

            Console.WriteLine($"배포 대상: {componentId}");

            var componentType = DetermineComponentType(componentId, isExe);
            var defaults = ComponentTypeDefaults[componentType];
            var groupName = DetermineGroupName(componentId);

            var hash = CalculateFileHash(filePath);
            var storagePath = string.Empty;

            if (_useServerTransfer && _fileTransferService != null)
            {
                try
                {
                    var relativePath = fileName;

                    if (!string.IsNullOrEmpty(rootDirectory) && filePath.StartsWith(rootDirectory, StringComparison.OrdinalIgnoreCase))
                    {
                        relativePath = filePath.Substring(rootDirectory.Length);

                        while (relativePath.Length > 0 && (relativePath[0] == '\\' || relativePath[0] == '/'))
                        {
                            relativePath = relativePath.Substring(1);
                        }

                        relativePath = relativePath.Replace("\\", "/");
                    }

                    var fullPath = string.IsNullOrWhiteSpace(defaults.InstallPath)
                        ? relativePath
                        : $"{defaults.InstallPath.TrimEnd('/', '\\')}/{relativePath}";

                    var serverPath = string.IsNullOrWhiteSpace(homeDirectory)
                        ? $"{_serverComponentPath}/{fullPath}"
                        : $"{homeDirectory.TrimEnd('/', '\\')}/{_serverComponentPath}/{fullPath}";

                    Console.WriteLine($"서버에 업로드 중: {serverPath}");
                    var success = await _fileTransferService.UploadFileAsync(filePath, serverPath);

                    if (!success)
                    {
                        throw new InvalidOperationException($"서버 업로드 실패: {serverPath}");
                    }

                    storagePath = serverPath;
                    Console.WriteLine($"업로드 완료: {fileName} (경로: {serverPath})");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"업로드 실패: {filePath}: {ex.Message}");
                    storagePath = filePath;
                }
            }

            var existing = _componentRepo.GetComponent(componentId);
            if (existing == null)
            {
                _componentRepo.SaveComponent(new ComponentMstDto
                {
                    ComponentId = componentId,
                    ComponentName = componentId,
                    FileName = fileName,
                    InstallPath = storagePath,
                    GroupName = groupName,
                    ComponentType = componentType,
                    IsRequired = defaults.IsRequired,
                    AutoUpdate = true,
                    Priority = defaults.Priority,
                    IsActive = "Y"
                });
            }
            else
            {
                existing.FileName = fileName;
                existing.InstallPath = storagePath;
                existing.GroupName = groupName;
                existing.ComponentType = componentType;
                existing.IsRequired = defaults.IsRequired;
                existing.AutoUpdate = true;
                existing.Priority = defaults.Priority;
                existing.IsActive = "Y";
                if (string.IsNullOrWhiteSpace(existing.ComponentName))
                {
                    existing.ComponentName = componentId;
                }

                _componentRepo.SaveComponent(existing);
            }

            _componentRepo.DeactivateOldVersions(componentId);
            _componentRepo.AddVersion(new ComponentVerDto
            {
                ComponentId = componentId,
                Version = version,
                FileHash = hash,
                FileSize = fileInfo.Length,
                StoragePath = storagePath,
                DeployDesc = $"Smart Deploy: {DateTime.Now:yyyy-MM-dd HH:mm:ss}",
                IsActive = "Y"
            });
        }

        private static ComponentType DetermineComponentType(string componentId, bool isExe)
        {
            var fileName = Path.GetFileName(componentId);
            var baseName = Path.GetFileNameWithoutExtension(fileName);

            if (isExe) return ComponentType.Executable;
            if (baseName.StartsWith("nU3.Core", StringComparison.OrdinalIgnoreCase)) return ComponentType.FrameworkCore;
            if (baseName.StartsWith("nU3.Modules", StringComparison.OrdinalIgnoreCase)) return ComponentType.ScreenModule;
            if (baseName.StartsWith("nU3.Plugin", StringComparison.OrdinalIgnoreCase)) return ComponentType.Plugin;
            return ComponentType.SharedLibrary;
        }

        private static string DetermineGroupName(string componentId)
        {
            var fileName = Path.GetFileName(componentId);
            var baseName = Path.GetFileNameWithoutExtension(fileName);

            if (baseName.StartsWith("nU3", StringComparison.OrdinalIgnoreCase)) return "Framework";
            if (baseName.StartsWith("DevExpress", StringComparison.OrdinalIgnoreCase)) return "DevExpress";
            if (baseName.StartsWith("Oracle", StringComparison.OrdinalIgnoreCase)) return "Oracle";
            if (baseName.StartsWith("Microsoft", StringComparison.OrdinalIgnoreCase)) return "Microsoft";
            if (baseName.StartsWith("System", StringComparison.OrdinalIgnoreCase)) return "System";
            return "Other";
        }

        private static string CalculateFileHash(string filePath)
        {
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            using var stream = File.OpenRead(filePath);
            var hash = sha256.ComputeHash(stream);
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }

        private void BtnSyncTest_Click(object? sender, EventArgs e)
        {
            if (_fileTransferService == null)
            {
                MessageBox.Show("서버 연결이 설정되지 않았습니다.", "확인", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                Cursor = Cursors.WaitCursor;

                string homeDir = _fileTransferService.GetHomeDirectory();
                bool success = _fileTransferService.UploadFile("D:\\temp\\filelist.txt", "D:\\temp\\filelist.txt.x");
                success = _fileTransferService.UploadFile("D:\\temp\\filelist.txt.x", "D:\\temp\\filelist.txt.xx");
                _fileTransferService.SetHomeDirectory(true, "D:\\temp\\xxx");

                success = _fileTransferService.UploadFile("D:\\temp\\filelist.txt.x", "D:\\temp\\filelist.txt.xx");

                if (success)
                    MessageBox.Show("Sync 호출 성공! (데드락 없음)" + homeDir, "테스트 완료", MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                    MessageBox.Show("Sync 호출 실패", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Sync 테스트 중 오류: {ex.Message}");
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private async void BtnAsyncTest_Click(object? sender, EventArgs e)
        {
            if (_fileTransferService == null)
            {
                MessageBox.Show("서버 연결이 설정되지 않았습니다.", "확인", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                ToggleUiEnabled(false);
                bool success = await _fileTransferService.CreateDirectoryAsync("D:\\temp\\TestAsyncPath");

                if (success)
                    MessageBox.Show("Async 호출 성공!", "테스트 완료", MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                    MessageBox.Show("Async 호출 실패", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Async 테스트 중 오류: {ex.Message}");
            }
            finally
            {
                ToggleUiEnabled(true);
            }
        }

        private void ToggleUiEnabled(bool enabled)
        {
            if (_panelTop != null)
            {
                _panelTop.Enabled = enabled;
            }
            if (_dgvComponents != null)
            {
                _dgvComponents.Enabled = enabled;
            }
            if (_panelDetail != null)
            {
                _panelDetail.Enabled = enabled;
            }
            if (_dgvVersions != null)
            {
                _dgvVersions.Enabled = enabled;
            }
        }


        private void ClearDetailFields()
        {
            _cboComponentType.SelectedIndexChanged -= CboComponentType_SelectedIndexChanged;

            _txtComponentId.Text = "";
            _txtComponentName.Text = "";
            _txtFileName.Text = "";
            _txtInstallPath.Text = "";
            _txtGroupName.Text = "";
            _txtDescription.Text = "";
            _txtDependencies.Text = "";
            _cboComponentType.SelectedIndex = -1;
            _nudPriority.Value = 100;
            _chkRequired.Checked = false;
            _chkAutoUpdate.Checked = true;
            _dgvVersions.DataSource = null;

            _cboComponentType.SelectedIndexChanged += CboComponentType_SelectedIndexChanged;
        }

        private sealed class ComponentEditRow
        {
            public string ComponentId { get; set; } = "";
            public string ComponentType { get; set; } = "";
            public string ComponentName { get; set; } = "";
            public string FileName { get; set; } = "";
            public string InstallPath { get; set; } = "";
            public string GroupName { get; set; } = "";
            public string IsRequired { get; set; } = "N";
            public int Priority { get; set; }
        }
    }
}