using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using nU3.Core.Repositories;
using nU3.Core.UI;
using nU3.Core.Interfaces;
using nU3.Core.Events;
using nU3.Models;
using System.Reflection;
using nU3.Core.Attributes;
using DevExpress.XtraBars;
using DevExpress.XtraTab;
using DevExpress.XtraTab.ViewInfo;
using nU3.Core.Services;
using DevExpress.XtraEditors;
using nU3.Shell.Helpers;
using nU3.Shell.Configuration;
using System.Threading.Tasks;
using nU3.Core.Logging;
using nU3.Core.Events.Contracts;
using nU3.Core.Pipes;
using nU3.Core.UI.Components.Controls;
using nU3.Core.Helpers;
using nU3.Core.UI.Helpers;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.IO;

namespace nU3.Shell
{
    /// <summary>
    /// nU3 Framework 메인 셸 폼
    /// </summary>
    public partial class nUShell : BaseWorkForm, IBaseWorkComponent
    {
        #region IBaseWorkComponent 구현
        public override IEventAggregator OwnerEventBus => _eventAggregator;
        public override string OwnerProgramID => "MAIN_SHELL";
        #endregion

        #region 필드 및 속성
        private readonly IMenuRepository _menuRepo;
        private readonly IModuleRepository _moduleRepo;
        private readonly ISecurityRepository _securityRepo;
        private readonly IUserRepository _userRepo;
        private readonly IEventAggregator _eventAggregator;
        private readonly ModuleLoaderService _moduleLoader;
        private readonly IServiceProvider _serviceProvider;

        private readonly Dictionary<string, Type> _openTabs = new Dictionary<string, Type>();
        private bool _initialized;
        private JsonDocument? _appConfig; // 통합 설정 객체

        private CrashReporter? _crashReporter;
        private EmailSettings? _emailSettings;
        private bool _errorReportingEnabled;
        private bool _loggingEnabled;
        private bool _uploadOnError;
        private bool _serverConnectionEnabled;

        public string? StartupUri { get; set; }
        private NamedPipeServer? _pipeServer;
        private NotificationControl? _notificationControl;
        #endregion

        #region 생성자 및 단계별 초기화

        public nUShell()
        {
            InitializeComponent();
            if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime) return;
        }

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public nUShell(
            IMenuRepository menuRepo,
            IModuleRepository moduleRepo,
            ISecurityRepository securityRepo,
            IUserRepository userRepo,
            IEventAggregator eventAggregator,
            ModuleLoaderService moduleLoader,
            IServiceProvider serviceProvider)
            : this()
        {
            _menuRepo = menuRepo;
            _moduleRepo = moduleRepo;
            _securityRepo = securityRepo;
            _userRepo = userRepo;
            _eventAggregator = eventAggregator;
            _moduleLoader = moduleLoader;
            _serviceProvider = serviceProvider;

            // [초기화 1단계] 설정 로드
            LoadAppConfiguration();

            // [초기화 2단계] 로깅 시스템
            InitializeLogging();

            // [초기화 3단계] 예외 처리
            InitializeErrorReporting();

            // [초기화 4단계] UI 및 기본 서비스
            InitializeShellAppearance();
            InitializePipeServer();
            UpdateStatusBar();

            this.FormClosing += MainShellForm_FormClosing;
            this.Load += MainShellForm_Load;
            _moduleLoader.VersionConflict += OnModuleVersionConflict;

            LogManager.Info("메인 셸 시스템 초기화 완료 (생성자)", "Shell");
        }

        private void LoadAppConfiguration()
        {
            try
            {
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
                if (File.Exists(path)) _appConfig = JsonDocument.Parse(File.ReadAllText(path));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"설정 로드 실패: {ex.Message}");
            }
        }

        private void InitializeLogging()
        {
            if (_appConfig == null) return;
            try
            {
                if (!_appConfig.RootElement.TryGetProperty("Logging", out var lc)) return;
                if (!lc.TryGetProperty("Enabled", out var e) || !e.GetBoolean()) return;

                _loggingEnabled = true;
                LogManager.Instance.Initialize(
                    logDirectory: GetConfigValue(lc, "FileLogging", "LogDirectory"),
                    auditDirectory: GetConfigValue(lc, "AuditLogging", "AuditDirectory"),
                    fileTransferService: null,
                    enableAutoUpload: GetConfigBoolValue(lc, "ServerUpload", "AutoUpload")
                );
                _uploadOnError = GetConfigBoolValue(lc, "ServerUpload", "UploadOnError");
                LogManager.Info("로깅 시스템이 준비되었습니다.", "Shell");
            }
            catch { _loggingEnabled = false; }
        }

        private void InitializeErrorReporting()
        {
            if (_appConfig == null) return;
            try
            {
                _emailSettings = ExtractEmailSettings();
                if (_emailSettings != null)
                {
                    _errorReportingEnabled = true;
                    _crashReporter = new CrashReporter(this, _emailSettings);

                    Application.ThreadException += (s, e) => HandleUnhandledException(e.Exception, "UI Thread");
                    AppDomain.CurrentDomain.UnhandledException += (s, e) => { if (e.ExceptionObject is Exception ex) HandleUnhandledException(ex, "AppDomain"); };
                    TaskScheduler.UnobservedTaskException += (s, e) => { HandleUnhandledException(e.Exception, "Task"); e.SetObserved(); };

                    _crashReporter.CleanupOldLogs(30);
                    LogManager.Info("에러 리포팅 시스템 활성화됨", "Shell");
                }
            }
            catch (Exception ex) { LogManager.Error("에러 리포팅 초기화 실패", "Shell", ex); }
        }

        private EmailSettings? ExtractEmailSettings()
        {
            try
            {
                if (!_appConfig!.RootElement.TryGetProperty("ErrorReporting", out var er)) return null;
                if (!er.TryGetProperty("Enabled", out var e) || !e.GetBoolean()) return null;
                var ec = er.GetProperty("Email");
                return new EmailSettings
                {
                    SmtpServer = ec.GetProperty("SmtpServer").GetString() ?? "",
                    SmtpPort = ec.GetProperty("SmtpPort").GetInt32(),
                    EnableSsl = ec.GetProperty("EnableSsl").GetBoolean(),
                    Username = ec.GetProperty("Username").GetString(),
                    Password = ec.GetProperty("Password").GetString(),
                    FromEmail = ec.GetProperty("FromEmail").GetString(),
                    FromName = ec.GetProperty("FromName").GetString(),
                    ToEmail = ec.GetProperty("ToEmail").GetString()
                };
            }
            catch { return null; }
        }
        #endregion

        #region 메인 라이프사이클 (Load/Closing)

        private void MainShellForm_Load(object sender, EventArgs e)
        {
            if (_initialized) return; _initialized = true;
            LogManager.Info("메인 셸 로딩 시작", "Shell");

            ShowSplashMessage("모듈 데이터를 로드하고 있습니다...");
            _moduleLoader.LoadAllModules();

            ShowSplashMessage("사용자 메뉴를 구성하고 있습니다...");
            BuildMenu();
            SubscribeToEvents();

            InitializeServerConnection();
            HideSplashMessage();

            if (LogManager.Instance.Logger != null)
            {
                LogManager.Instance.Logger.MessageLogged += (s, msg) => this.SafeInvoke(() => UpdateStatusMessage(msg));
            }

            LogManager.LogAction(AuditAction.Login, "Shell", "MainShell", "시스템 로그인 완료");
            if (!string.IsNullOrEmpty(StartupUri)) ProcessStartupUri(StartupUri);
        }

        private void InitializeServerConnection()
        {
            try
            {
                var config = ServerConnectionConfig.Load();
                if (!config.Enabled)
                {
                    barStaticItemServer.Caption = "🔴 서버: 비활성";
                    _serverConnectionEnabled = false;
                    return;
                }

                ConnectivityManager.Instance.Initialize(config.BaseUrl, true);
                ConnectivityManager.Instance.LogMessage += (s, e) => LogManager.Info(e.Message, "Connectivity");

                Task.Run(async () =>
                {
                    try
                    {
                        var connected = await ConnectivityManager.Instance.TestConnectionAsync();
                        this.SafeInvoke(() =>
                        {
                            barStaticItemServer.Caption = connected ? $"🟢 {config.BaseUrl}" : $"🟡 {config.BaseUrl} (응답 없음)";
                            _serverConnectionEnabled = connected;
                        });
                    }
                    catch (Exception ex)
                    {
                        this.SafeInvoke(() =>
                        {
                            barStaticItemServer.Caption = $"🔴 {config.BaseUrl} (오류)";
                            LogManager.Error($"서버 연결 실패: {ex.Message}", "Shell");
                        });
                    }
                });
            }
            catch (Exception ex)
            {
                LogManager.Error($"서버 초기화 실패: {ex.Message}", "Shell");
                barStaticItemServer.Caption = "🔴 서버: 오류";
            }
        }

        private void MainShellForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                if (XtraMessageBox.Show("프로그램을 종료하시겠습니까?", "종료 확인", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                {
                    e.Cancel = true; return;
                }
            }
            if (!e.Cancel)
            {
                LogManager.LogAction(AuditAction.Logout, "Shell", "MainShell", "시스템 로그아웃");
                if (_loggingEnabled) LogManager.Instance.Shutdown();
            }
        }

        #endregion

        #region 디자이너 이벤트 핸들러 복구

        private void XtraTabControlMain_SelectedPageChanged(object sender, TabPageChangedEventArgs e)
        {
            if (e.PrevPage != null) DeactivateTabContent(e.PrevPage);
            if (e.Page != null)
            {
                ActivateTabContent(e.Page);
                UpdateShellTitle(e.Page);
                UpdateStatusMessage($"'{e.Page.Text}' 활성화됨");
            }
            else this.Text = "nU3 Healthcare Information System";
        }

        private void XtraTabControlMain_CloseButtonClick(object sender, EventArgs e)
        {
            if (e is ClosePageButtonEventArgs a && a.Page is XtraTabPage p) CloseTab(p);
        }

        private void BarStaticItemLogMessage_ItemDoubleClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                if (LogManager.Instance.Logger is FileLogger fl && File.Exists(fl.GetLogFilePath()))
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo { FileName = fl.GetLogFilePath(), UseShellExecute = true });
            }
            catch { }
        }

        private void TimerStatusUpdate_Tick(object sender, EventArgs e) => UpdateStatusBar();

        #endregion

        #region 화면 및 메뉴 관리 로직

        private void BuildMenu()
        {
            var user = nU3.Core.Security.UserSession.Current;
            var manager = barManager1; if (manager == null) return;
            manager.BeginUpdate();
            try
            {
                foreach (var item in manager.Items.Cast<BarItem>().Where(i => i != barStaticItemUser && i != barStaticItemTime && i != barStaticItemServer && i != barStaticItemVersion).ToList())
                    manager.Items.Remove(item);
                barMainMenu.ItemLinks.Clear();

                if (string.IsNullOrWhiteSpace(user.SelectedDeptCode))
                {
                    AddEmptyMenuNotice(manager, "시스템 (부서 미선택)", "로그인 시 부서를 선택해야 합니다.");
                    return;
                }

                var allMenus = _menuRepo.GetMenusByUserAndDept(user.UserId, user.SelectedDeptCode);
                if (allMenus.Count == 0) allMenus = _menuRepo.GetAllMenus();

                var filteredMenus = allMenus.Where(m => m.AuthLevel <= user.AuthLevel).OrderBy(m => m.SortOrd).ToList();
                foreach (var m in filteredMenus.Where(m => m.ParentId == null))
                {
                    var sub = CreateBarSubMenu(manager, m.MenuName);
                    BuildBarMenuRecursive(sub, m.MenuId, filteredMenus, user.AuthLevel, manager);
                    if (!string.IsNullOrEmpty(m.ProgId)) sub.AddItem(CreateBarButtonItem(manager, m.MenuName, (s, e) => OpenProgram(m.ProgId, m.MenuName)));
                    if (sub.ItemLinks.Count > 0) barMainMenu.AddItem(sub);
                }
            }
            finally { AddSystemMenu(manager); manager.EndUpdate(); }
        }

        private void OpenProgram(string progId, string? displayName = null)
        {
            var type = _moduleLoader.GetProgramType(progId);
            if (type == null)
            {
                XtraMessageBox.Show($"프로그램 '{progId}'을(를) 로드할 수 없습니다.", "실행 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var page = FindTabByProgId(progId);
            if (page != null) { xtraTabControlMain.SelectedTabPage = page; ActivateTabContent(page); return; }

            try
            {
                var content = CreateProgramContent(type, progId);
                var newPage = new XtraTabPage { Text = displayName ?? progId, Tag = progId };
                newPage.Controls.Add(content); xtraTabControlMain.TabPages.Add(newPage); xtraTabControlMain.SelectedTabPage = newPage;
                _openTabs[progId] = type;
                ActivateTabContent(newPage);
            }
            catch (Exception ex)
            {
                LogManager.Error($"화면 실행 실패: {progId}", "Shell", ex);
                XtraMessageBox.Show($"실행 오류: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private Control CreateProgramContent(Type t, string id)
        {
            var c = (Control)Microsoft.Extensions.DependencyInjection.ActivatorUtilities.CreateInstance(_serviceProvider, t);
            if (typeof(Form).IsAssignableFrom(t))
            {
                var f = (Form)c; f.TopLevel = false; f.FormBorderStyle = FormBorderStyle.None; f.Dock = DockStyle.Fill; f.Show();
            }
            else
            {
                if (c is BaseWorkControl wc) wc.EventBus = _eventAggregator;
                if (c is IWorkContextProvider cp) cp.InitializeContext(CreateWorkContext(id));
                c.Dock = DockStyle.Fill;
            }
            return c;
        }

        #endregion

        #region 유틸리티 및 헬퍼

        private void SafeInvoke(Action action) => nU3.Core.UI.Helpers.UIHelper.SafeInvoke(this, action);

        private void UpdateStatusBar()
        {
            var u = nU3.Core.Security.UserSession.Current;
            if (u != null) barStaticItemUser.Caption = $"👤 {u.UserId} (Lv {u.AuthLevel})";
            barStaticItemTime.Caption = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        private void UpdateStatusMessage(string message) => barStaticItemLogMessage.Caption = message;

        private void ShowSplashMessage(string msg) { UpdateStatusMessage(msg); Application.DoEvents(); }

        private void HideSplashMessage() => UpdateStatusBar();

        private BarSubItem CreateBarSubMenu(BarManager m, string t) => new BarSubItem(m, t) { ItemAppearance = { Normal = { Font = new System.Drawing.Font("Segoe UI", 9F), Options = { UseFont = true } } } };

        private BarButtonItem CreateBarButtonItem(BarManager m, string t, ItemClickEventHandler c) { var b = new BarButtonItem(m, t); b.ItemClick += c; return b; }

        private XtraTabPage? FindTabByProgId(string id) => xtraTabControlMain.TabPages.FirstOrDefault(p => string.Equals(p.Tag as string, id, StringComparison.OrdinalIgnoreCase));

        private void UpdateShellTitle(XtraTabPage page)
        {
            string baseTitle = "nU3 Healthcare Information System";
            if (page.Controls.Count > 0 && page.Controls[0] is BaseWorkControl wc)
            {
                this.Text = $"{baseTitle} - [{wc.ProgramID}] {wc.ProgramTitle} v{wc.GetType().Assembly.GetName().Version}";
            }
            else this.Text = $"{baseTitle} - {page.Text}";
        }

        private string? GetConfigValue(JsonElement p, string s, string k) { try { return p.GetProperty(s).GetProperty(k).GetString(); } catch { return null; } }

        private bool GetConfigBoolValue(JsonElement p, string s, string k) { try { return p.GetProperty(s).GetProperty(k).GetBoolean(); } catch { return false; } }

        #endregion

        #region 기타 내부 연동 로직

        private void SubscribeToEvents()
        {
            _eventAggregator.GetEvent<NavigationRequestEvent>().Subscribe(p => { if (p is NavigationRequestEventPayload e) OpenProgram(e.TargetScreenId); });
            _eventAggregator.GetEvent<CloseScreenRequestEvent>().Subscribe(p => { if (p is CloseScreenRequestEventPayload e) { var pg = FindTabByProgId(e.ScreenId); if (pg != null) CloseTab(pg); } });
            _eventAggregator.GetEvent<ModuleActivatedEvent>().Subscribe(p => { if (p is ModuleActivatedEventPayload e) SafeInvoke(() => this.Text = $"nU3 HIS - [{e.ProgId}] v{e.Version}"); });
            _eventAggregator.GetEvent<Core.Events.Contracts.PatientSelectedEvent>().Subscribe(p => { if (p is PatientSelectedEventPayload e) UpdateStatusMessage($"환자 선택: {e.Patient.PatientName}"); });
        }

        private void HandleUnhandledException(Exception ex, string src)
        {
            try
            {
                LogManager.Critical($"미처리 예외 ({src}): {ex.Message}", "Error", ex);
                if (_loggingEnabled && _uploadOnError && _serverConnectionEnabled)
                {
                    Task.Run(async () => await ConnectivityManager.Instance.Log.UploadCurrentLogImmediatelyAsync()).Wait(TimeSpan.FromSeconds(3));
                }
                if (_errorReportingEnabled && _crashReporter != null) _crashReporter.ReportCrashAsync(ex, $"출처: {src}").Wait(TimeSpan.FromSeconds(5));
                XtraMessageBox.Show($"시스템에 예상치 못한 오류가 발생했습니다.\n\n{ex.Message}", "치명적 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch { }
        }

        private void OnModuleVersionConflict(object sender, ModuleVersionConflictEventArgs e)
        {
            SafeInvoke(() =>
            {
                if (XtraMessageBox.Show($"⚠️ 모듈 버전 불일치 감지 (로드:v{e.CurrentVersion}, 요청:v{e.RequestedVersion})\n\n재시작하시겠습니까?", "버전 충돌", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    Application.Restart(); Environment.Exit(0);
                }
            });
        }

        private void BuildBarMenuRecursive(BarSubItem parent, string parentId, List<MenuDto> all, int auth, BarManager m)
        {
            foreach (var child in all.Where(x => x.ParentId == parentId && x.AuthLevel <= auth).OrderBy(x => x.SortOrd))
            {
                if (!string.IsNullOrEmpty(child.ProgId)) parent.AddItem(CreateBarButtonItem(m, child.MenuName, (s, e) => OpenProgram(child.ProgId, child.MenuName)));
                else { var group = CreateBarSubMenu(m, child.MenuName); parent.AddItem(group); BuildBarMenuRecursive(group, child.MenuId, all, auth, m); }
            }
        }

        private nU3.Core.Context.WorkContext CreateWorkContext(string id)
        {
            var ctx = new nU3.Core.Context.WorkContext { CurrentUser = GetCurrentUserWithRole() };
            if (ctx.CurrentUser != null) ctx.Permissions = CreatePermissions(ctx.CurrentUser.UserId, ctx.CurrentUser.AuthLevel, ctx.CurrentUser.RoleCode, id);
            return ctx;
        }

        private UserInfoDto? GetCurrentUserWithRole()
        {
            var cur = nU3.Core.Security.UserSession.Current; if (cur == null) return null;
            var u = _userRepo.GetUserById(cur.UserId);
            return new UserInfoDto { UserId = cur.UserId, UserName = cur.UserName, AuthLevel = cur.AuthLevel, RoleCode = u?.RoleCode ?? "" };
        }

        private nU3.Core.Context.ModulePermissions CreatePermissions(string uid, int lv, string rc, string pid)
        {
            if (lv == 0) { var p = new nU3.Core.Context.ModulePermissions(); p.GrantAll(); return p; }
            try
            {
                var d = _securityRepo.GetEffectivePermission(uid, rc, pid);
                if (d != null) return new nU3.Core.Context.ModulePermissions { CanRead = d.CanRead, CanCreate = d.CanCreate, CanUpdate = d.CanUpdate, CanDelete = d.CanDelete, CanPrint = d.CanPrint, CanExport = d.CanExport, CanApprove = d.CanApprove, CanCancel = d.CanCancel };
            }
            catch { }
            return new nU3.Core.Context.ModulePermissions { CanRead = true };
        }

        private void CloseTab(XtraTabPage page)
        {
            var control = page.Controls.Count > 0 ? page.Controls[0] : null;
            if (control is ILifecycleAware la && !la.CanClose()) return;
            if (control is IResourceManager rm) try { rm.ReleaseResources(); } catch { }
            if (page.Tag is string pid) _openTabs.Remove(pid);
            xtraTabControlMain.TabPages.Remove(page); page.Dispose();
        }

        private void AddSystemMenu(BarManager m)
        {
            var s = CreateBarSubMenu(m, "시스템");
            s.AddItem(CreateBarButtonItem(m, "메뉴 새로고침", (x, y) => BuildMenu()));
            s.AddItem(CreateBarButtonItem(m, "모든 탭 닫기", (x, y) => { while (xtraTabControlMain.TabPages.Count > 0) CloseTab(xtraTabControlMain.TabPages[0]); }));
            s.AddItem(CreateBarButtonItem(m, "종료", (x, y) => this.Close()));
            barMainMenu.AddItem(s);
        }

        private void AddEmptyMenuNotice(BarManager manager, string rootText, string notice)
        {
            var root = new BarSubItem(manager, rootText);
            var item = new BarButtonItem(manager, notice);
            item.ItemClick += (s, e) => XtraMessageBox.Show(notice, "알림");
            root.AddItem(item); barMainMenu.AddItem(root);
        }

        private void ActivateTabContent(XtraTabPage p) { if (p.Controls.Count > 0 && p.Controls[0] is ILifecycleAware la) la.OnActivated(); }

        private void DeactivateTabContent(XtraTabPage p) { if (p.Controls.Count > 0 && p.Controls[0] is ILifecycleAware la) la.OnDeactivated(); }

        private void MainShellForm_FormClosed(object sender, FormClosedEventArgs e) { _pipeServer?.Stop(); _pipeServer?.Dispose(); }

        private void InitializePipeServer() { try { _pipeServer = new NamedPipeServer(); _pipeServer.Start("nU3_Shell_Pipe"); } catch { } }

        private void InitializeShellAppearance() { _notificationControl = new NotificationControl(this.components) { Position = NotificationPosition.BottomRight }; }

        private void ProcessStartupUri(string uri) { try { var u = new Uri(uri); var query = u.Query.TrimStart('?').Split('&').Select(x => x.Split('=')).ToDictionary(x => x[0], x => x[1]); if (query.ContainsKey("programid")) OpenProgram(query["programid"]); } catch { } }

        #endregion
    }
}
