using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Extensions.Configuration;
using nU3.Connectivity;
using nU3.Core.Repositories;
using nU3.Models;
using nU3.Core.UI;
using nU3.Core.Interfaces;

namespace nU3.Tools.Deployer.Views
{
    public partial class DomainCommonDeployControl : BaseWorkControl
    {
        private IComponentRepository _componentRepo;
        private IFileTransferService _fileTransferService;
        private IConfiguration _configuration;
        private string _serverComponentPath;

        public DomainCommonDeployControl()
        {
            InitializeComponent();
        }

        public void Initialize(IComponentRepository componentRepo, IConfiguration configuration, IFileTransferService fileTransferService)
        {
            _componentRepo = componentRepo;
            _configuration = configuration;
            _fileTransferService = fileTransferService;

            _serverComponentPath = nU3.Core.Services.ModuleLoaderService.FRAMEWORKS_DIR; // e.g. "Patch"

            SetupGrid();
            LoadData();
        }

        private void SetupGrid()
        {
            gvComponents.Columns.Clear();
            
            var colId = gvComponents.Columns.AddField("ComponentId");
            colId.Caption = "Component ID";
            colId.Visible = true;

            var colDomain = gvComponents.Columns.AddField("Domain");
            colDomain.Caption = "Domain";
            colDomain.Visible = true;

            var colFile = gvComponents.Columns.AddField("FileName");
            colFile.Caption = "File Name";
            colFile.Visible = true;

            var colPath = gvComponents.Columns.AddField("InstallPath");
            colPath.Caption = "Install Path";
            colPath.Visible = true;

            var colDate = gvComponents.Columns.AddField("RegDate");
            colDate.Caption = "Registered Date";
            colDate.Visible = true;
            colDate.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            colDate.DisplayFormat.FormatString = "yyyy-MM-dd HH:mm";
        }

        private void LoadData()
        {
            if (_componentRepo == null) return;
            
            // Fetch all components and filter for those that look like Domain Common modules
            // Logic: ComponentType = ScreenModule OR InstallPath starts with "Modules/"
            var components = _componentRepo.GetAllComponents()
                .Where(c => c.InstallPath != null && c.InstallPath.StartsWith("Modules/", StringComparison.OrdinalIgnoreCase))
                .Select(c => new
                {
                    c.ComponentId,
                    Domain = c.GroupName,
                    c.FileName,
                    c.InstallPath,
                    RegDate = DateTime.Now // Placeholder as MST doesn't have RegDate, but Version has.
                })
                .ToList();

            gcComponents.DataSource = components;
        }

        private void btnSelectFile_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = "Domain Common DLL (*.Common.dll)|*.Common.dll|All Files (*.*)|*.*";
                ofd.Title = "Select Domain Common DLL";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    txtSelectedFile.Text = ofd.FileName;
                    
                    // Auto-detect domain if possible (e.g., nU3.Modules.EMR.Common.dll -> EMR)
                    var fileName = Path.GetFileName(ofd.FileName);
                    var parts = fileName.Split('.');
                    if (parts.Length >= 4 && parts[2].Length == 3) // Simple heuristic
                    {
                        string possibleDomain = parts[2].ToUpper();
                        if (cboDomain.Properties.Items.Contains(possibleDomain))
                        {
                            cboDomain.Text = possibleDomain;
                        }
                    }
                }
            }
        }

        private async void btnDeploy_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtSelectedFile.Text))
            {
                MessageBox.Show("Please select a file.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(cboDomain.Text))
            {
                MessageBox.Show("Please select a domain.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string filePath = txtSelectedFile.Text;
            string domain = cboDomain.Text.Trim().ToUpper();
            string fileName = Path.GetFileName(filePath);
            string componentId = fileName; // Use filename as ID for simplicity
            
            // CRITICAL: Enforce standard path
            string installPath = $"Modules/{domain}/{fileName}";
            
            // Server Storage Path
            string serverPath = $"{_serverComponentPath}/{installPath}";

            try
            {
                // 1. Upload File
                if (_fileTransferService != null)
                {
                    bool success = await _fileTransferService.UploadFileAsync(filePath, serverPath);
                    if (!success) throw new Exception("File upload failed.");
                }

                // 2. Register/Update Component Master (SYS_COMPONENT_MST)
                var component = new ComponentMstDto
                {
                    ComponentId = componentId,
                    ComponentName = componentId,
                    ComponentType = ComponentType.ScreenModule, // Force ScreenModule typ
                    FileName = fileName,
                    InstallPath = installPath, // Force correct path
                    GroupName = domain,
                    Description = $"{domain} Domain Common Module",
                    IsRequired = false,
                    AutoUpdate = true,
                    Priority = 100,
                    IsActive = "Y"
                };

                _componentRepo.SaveComponent(component);

                // 3. Register Version (SYS_COMPONENT_VER)
                string version = System.Diagnostics.FileVersionInfo.GetVersionInfo(filePath).FileVersion ?? "1.0.0.0";
                string hash = CalculateFileHash(filePath);
                long size = new FileInfo(filePath).Length;

                _componentRepo.AddVersion(new ComponentVerDto
                {
                    ComponentId = componentId,
                    Version = version,
                    FileHash = hash,
                    FileSize = size,
                    StoragePath = serverPath,
                    DeployDesc = $"Domain Common Deploy: {domain}",
                    IsActive = "Y"
                });

                MessageBox.Show($"Successfully deployed to {installPath}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Deployment failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string CalculateFileHash(string filePath)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            using (var stream = File.OpenRead(filePath))
            {
                var hash = sha256.ComputeHash(stream);
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadData();
        }
    }
}
