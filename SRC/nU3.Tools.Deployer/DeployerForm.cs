using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using nU3.Core.Repositories;
using nU3.Core.Services;
using nU3.Core.UI;
using DevExpress.XtraTab;
using DevExpress.XtraBars;

namespace nU3.Tools.Deployer
{
    public partial class DeployerForm : BaseWorkForm
    {
        private readonly IModuleRepository _moduleRepo;
        private readonly IComponentRepository _componentRepo;
        private readonly IConfiguration _configuration;
        private string _serverStoragePath;
        private string? _serverBaseUrl;
        private bool _serverEnabled;

        private XtraTabControl tabMain;

        /// <summary>
        /// Designer 전용 생성자입니다.
        /// </summary>
        public DeployerForm()
        {
            InitializeComponent();
        }

        public DeployerForm(IModuleRepository moduleRepo, IComponentRepository componentRepo, IConfiguration configuration)
        {
            _moduleRepo = moduleRepo;
            _componentRepo = componentRepo;
            _configuration = configuration;

            InitializeComponent();

            this.Text = "nU3 도구 - 관리자용";
            this.Size = new System.Drawing.Size(1280, 800);
            this.StartPosition = FormStartPosition.CenterScreen;

            _serverStoragePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "nU3.Framework", "ServerStorage");
            if (!Directory.Exists(_serverStoragePath)) Directory.CreateDirectory(_serverStoragePath);

            // Updated to use BarButtonItem
            bbiTestServer.ItemClick += ToolStripButtonTestServer_Click;
            this.Load += DeployerForm_Load;

            InitializeServerConnectionStatus();

            if (!IsDesignMode())
            {
                BuildUi();
            }
        }

        private void DeployerForm_Load(object? sender, EventArgs e)
        {
            if (_serverEnabled)
            {
                _ = StartServerConnectionTestAsync();
            }
        }

        private void ToolStripButtonTestServer_Click(object? sender, ItemClickEventArgs e)
        {
            _ = StartServerConnectionTestAsync();
        }

        private void InitializeServerConnectionStatus()
        {
            _serverEnabled = _configuration.GetValue<bool>("ServerConnection:Enabled", false);
            _serverBaseUrl = _configuration.GetValue<string>("ServerConnection:BaseUrl") ?? "https://localhost:64229";

            bbiTestServer.Enabled = _serverEnabled;

            if (!_serverEnabled)
            {
                bsiStatus.Caption = "🟡 서버 비활성";
                return;
            }

            bsiStatus.Caption = $"🟡 {_serverBaseUrl} (테스트 대기)";
        }

        private async Task StartServerConnectionTestAsync()
        {
            if (!_serverEnabled || string.IsNullOrWhiteSpace(_serverBaseUrl))
            {
                bsiStatus.Caption = "🔴 서버 URL이 설정되지 않았습니다.";
                return;
            }

            bsiStatus.Caption = $"🟡 {_serverBaseUrl} (테스트 중...)";

            var progressForm = new Form
            {
                Width = 400,
                Height = 150,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterScreen,
                Text = "서버 연결 테스트 중...",
                ControlBox = false
            };
            var label = new Label
            {
                Text = "서버 연결을 테스트하고 있습니다...\n각 서비스를 순차적으로 확인합니다.",
                AutoSize = false,
                Width = 360,
                Height = 80,
                Location = new System.Drawing.Point(20, 30),
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            };
            progressForm.Controls.Add(label);

            try
            {
                progressForm.Show();
                Application.DoEvents();

                ConnectivityManager.Instance.Initialize(_serverBaseUrl);
                var result = await ConnectivityManager.Instance.TestAllConnectionsAsync();

                progressForm.Close();
                progressForm.Dispose();

                if (result.AllConnected)
                {
                    bsiStatus.Caption = $"🟢 {_serverBaseUrl}";
                    MessageBox.Show(
                        this,
                        $"서버 연결 성공!\n\n" +
                        $"서버: {_serverBaseUrl}\n\n" +
                        $"✅ 데이터베이스: 연결됨\n" +
                        $"✅ 파일 전송: 연결됨\n" +
                        $"✅ 로그 업로드: 연결됨",
                        "연결 성공",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                else
                {
                    bsiStatus.Caption = $"🟡 {_serverBaseUrl} (일부 실패)";

                    var statusMessage = new System.Text.StringBuilder();
                    statusMessage.AppendLine($"서버: {_serverBaseUrl}");
                    statusMessage.AppendLine();
                    statusMessage.AppendLine($"{(result.DBConnected ? "✅" : "❌")} 데이터베이스: {(result.DBConnected ? "연결됨" : $"실패 - {result.DBError}")}");
                    statusMessage.AppendLine($"{(result.FileConnected ? "✅" : "❌")} 파일 전송: {(result.FileConnected ? "연결됨" : $"실패 - {result.FileError}")}");
                    statusMessage.AppendLine($"{(result.LogConnected ? "✅" : "❌")} 로그 업로드: {(result.LogConnected ? "연결됨" : $"실패 - {result.LogError}")}");

                    if (!string.IsNullOrEmpty(result.GeneralError))
                    {
                        statusMessage.AppendLine();
                        statusMessage.AppendLine($"일반 오류: {result.GeneralError}");
                    }

                    MessageBox.Show(
                        this,
                        statusMessage.ToString(),
                        "연결 테스트 결과",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                progressForm?.Close();
                progressForm?.Dispose();

                bsiStatus.Caption = $"🔴 {_serverBaseUrl} (오류)";

                MessageBox.Show(
                    this,
                    $"연결 테스트 중 오류 발생!\n\n{ex.Message}",
                    "오류",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private static bool IsDesignMode()
        {
            return System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime;
        }

        private void BuildUi()
        {
            tabMain = new XtraTabControl { Dock = DockStyle.Fill };

            // 1. 화면 모듈 배포
            var tabDeploy = new XtraTabPage { Text = "프로그램모듈 배포" };
            var deployControl = new Views.ProgramDeployManagementControl(_moduleRepo, _configuration);
            deployControl.Dock = DockStyle.Fill;
            tabDeploy.Controls.Add(deployControl);

            // 2. Framework 컴포넌트 배포 
            var tabComponent = new XtraTabPage { Text = "프레임워크모듈 배포" };
            var componentControl = new Views.AssemblyDeployManagementControl();            
            componentControl.Initialize(_componentRepo, _configuration);
            componentControl.Dock = DockStyle.Fill;
            tabComponent.Controls.Add(componentControl);
            
            // 3. 메뉴트리 관리
            var tabMenu = new XtraTabPage { Text = "메뉴 관리" };
            var menuControl = new Views.MenuTreeManagementControl();
            menuControl.Dock = DockStyle.Fill;
            tabMenu.Controls.Add(menuControl);

            // 4. 사용자/권한 관리
            var tabSecurity = new XtraTabPage { Text = "권한 관리" };
            var securityControl = new Views.SecurityManagementControl();
            securityControl.Dock = DockStyle.Fill;
            tabSecurity.Controls.Add(securityControl);
            
            tabMain.TabPages.Add(tabDeploy);
            tabMain.TabPages.Add(tabComponent);
            tabMain.TabPages.Add(tabMenu);
            tabMain.TabPages.Add(tabSecurity);

            Controls.Add(tabMain);
        }
    }
}