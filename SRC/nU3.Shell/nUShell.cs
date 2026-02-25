using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

using DevExpress.XtraBars;
using DevExpress.XtraTab;
using DevExpress.XtraTab.ViewInfo;
using DevExpress.XtraEditors;

using nU3.Core.Repositories;
using nU3.Core.UI;
using nU3.Core.Interfaces;
using nU3.Core.Events;
using nU3.Core.Services;
using nU3.Core.Logging;
using nU3.Core.Events.Contracts;
using nU3.Core.UI.Helpers;
using nU3.Core.Security;
using nU3.Core.Helpers;
using nU3.Core.Pipes;
using nU3.Core.UI.Interfaces;
using nU3.Core.UI.Services;
using nU3.Core.UI.Components.Controls;
using nU3.Shell.Helpers;
using nU3.Shell.Configuration;
using nU3.Models;

namespace nU3.Shell
{
    /// <summary>
    /// nU3 Framework 메인 셸 폼.
    /// 핵심 로직이 Core 서비스로 분리되어 있으며, UI 제어 및 이벤트 핸들링을 담당합니다.
    /// </summary>
    public partial class nUShell : BaseWorkForm, IBaseWorkComponent, IShellView
    {
        #region Fields & Services

        private readonly IMenuRepository _menuRepo;
        private readonly IEventAggregator _eventAggregator;
        private readonly IServiceProvider _serviceProvider;

        private readonly IWorkContextService _workContextService;
        private readonly IGlobalExceptionService _exceptionService;
        private readonly INavigationService _navigationService;
        private readonly ModuleLoaderService _moduleLoader;

        private readonly Dictionary<string, Type> _openTabs = new Dictionary<string, Type>();
        private bool _initialized;
        private JsonDocument? _appConfig;

        private NamedPipeServer? _pipeServer;
        private NotificationControl? _notificationControl;
        private CrashReporter? _crashReporter;
        private bool _loggingEnabled;
        private bool _serverConnectionEnabled;

        #endregion

        #region Properties
        public override IEventAggregator OwnerEventBus => _eventAggregator;
        public override string OwnerProgramID => "MAIN_SHELL";
        public string? StartupUri { get; set; }
        #endregion

        #region Constructors

        public nUShell()
        {
            InitializeComponent();
            if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime) return;
        }

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public nUShell(
            IMenuRepository menuRepo,
            IEventAggregator eventAggregator,
            IServiceProvider serviceProvider,
            IWorkContextService workContextService,
            IGlobalExceptionService exceptionService,
            INavigationService navigationService,
            ModuleLoaderService moduleLoader)
            : this()
        {
            _menuRepo = menuRepo;
            _eventAggregator = eventAggregator;
            _serviceProvider = serviceProvider;
            _workContextService = workContextService;
            _exceptionService = exceptionService;
            _navigationService = navigationService;
            _moduleLoader = moduleLoader;

            // 서비스 초기화
            _navigationService.Initialize(this);

            // 단계별 초기화 프로세스
            LoadAppConfiguration();
            InitializeLogging();
            InitializeErrorReporting();

            InitializeShellAppearance();
            InitializePipeServer();
            UpdateStatusBar();

            // 폼 이벤트 연결
            this.FormClosing += MainShellForm_FormClosing;
            this.FormClosed += MainShellForm_FormClosed;
            this.Load += MainShellForm_Load;

            if (_moduleLoader != null)
                _moduleLoader.VersionConflict += OnModuleVersionConflict;

            LogManager.Info("메인 셸 시스템 초기화 완료", "Shell");
        }

        #endregion

        #region IShellView 구현 (내비게이션 엔진용)

        public bool IsProgramOpen(string programId) => FindTabByProgId(programId) != null;

        public void ActivateProgram(string programId)
        {
            var page = FindTabByProgId(programId);
            if (page != null)
            {
                xtraTabControlMain.SelectedTabPage = page;
                ActivateTabContent(page);
            }
        }

        public void ShowContent(Control content, string programId, string? displayName)
        {
            var newPage = new XtraTabPage { Text = displayName ?? programId, Tag = programId };
            newPage.Controls.Add(content);
            xtraTabControlMain.TabPages.Add(newPage);
            xtraTabControlMain.SelectedTabPage = newPage;
            _openTabs[programId] = content.GetType();

            ActivateTabContent(newPage);
            UpdateShellTitle(newPage);
        }

        #endregion

        #region Initialization Logic

        private void LoadAppConfiguration()
        {
            try
            {
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
                if (File.Exists(path)) _appConfig = JsonDocument.Parse(File.ReadAllText(path));
            }
            catch (Exception ex)
            {
                LogManager.Error("설정 로드 실패", "Shell", ex);
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
                LogManager.Info("로깅 시스템이 준비되었습니다.", "Shell");
            }
            catch { _loggingEnabled = false; }
        }

        private void InitializeErrorReporting()
        {
            if (_appConfig == null) return;
            try
            {
                var emailSettings = ExtractEmailSettings();
                if (emailSettings != null)
                {
                    _crashReporter = new CrashReporter(this, emailSettings);
                    bool enabled = GetConfigBoolValue(_appConfig.RootElement, "ErrorReporting", "Enabled");
                    bool uploadOnError = GetConfigBoolValue(_appConfig.RootElement, "Logging", "ServerUpload", "UploadOnError");

                    _exceptionService.Initialize(enabled, uploadOnError, emailSettings, _crashReporter);
                    _exceptionService.RegisterGlobalHandlers();

                    // WinForms 전용 UI 스레드 예외 핸들러
                    Application.ThreadException += (s, e) => _exceptionService.HandleException(e.Exception, "UI Thread");

                    _crashReporter.CleanupOldLogs(30);
                    LogManager.Info("에러 리포팅 시스템이 활성화되었습니다.", "Shell");
                }
            }
            catch (Exception ex) { LogManager.Error("에러 리포팅 초기화 실패", "Shell", ex); }
        }

        private void InitializeShellAppearance()
        {
            _notificationControl = new NotificationControl(this.components) { Position = NotificationPosition.BottomRight };
        }

        private void InitializePipeServer()
        {
            try { _pipeServer = new NamedPipeServer(); _pipeServer.Start("nU3_Shell_Pipe"); } catch { }
        }

        #endregion

        #region Main Life Cycle Events

        private void MainShellForm_Load(object sender, EventArgs e)
        {
            if (_initialized) return; _initialized = true;
            LogManager.Info("메인 셸 로딩 프로세스 시작", "Shell");

            try
            {
                ShowSplashMessage("시스템 모듈을 구성하고 있습니다...");
                // Modules 폴더만 스캔하도록 최적화된 로드 (가장 큰 속도 개선 포인트)
                _moduleLoader.LoadAllModules();

                ShowSplashMessage("사용자 메뉴를 생성하고 있습니다...");
                BuildMenu();

                SubscribeToEvents();
                InitializeServerConnection();

                if (LogManager.Instance.Logger != null)
                {
                    LogManager.Instance.Logger.MessageLogged += (s, msg) => this.SafeInvoke(() => UpdateStatusMessage(msg));
                }

                LogManager.LogAction(AuditAction.Login, "Shell", "MainShell", "시스템 로그인 완료");
                if (!string.IsNullOrEmpty(StartupUri)) ProcessStartupUri(StartupUri);
            }
            catch (Exception ex)
            {
                LogManager.Error("셸 로딩 중 오류 발생", "Shell", ex);
                XtraMessageBox.Show("시스템 로드 중 문제가 발생했습니다.\n로그를 확인해 주세요.", "로드 오류");
            }
            finally
            {
                HideSplashMessage();
            }
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
                LogManager.Error("서버 연결 초기화 실패", "Shell", ex);
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

        private void MainShellForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            _pipeServer?.Stop();
            _pipeServer?.Dispose();
        }

        #endregion

        #region UI Event Handlers (Designer Referenced)

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

        #region Tab & Navigation Helpers

        private void OpenProgram(string progId, string? displayName = null, Core.Context.WorkContext? context = null)
        {
            _navigationService.OpenProgramAsync(progId, displayName, context).Forget();
        }

        private void CloseTab(XtraTabPage page)
        {
            var control = page.Controls.Count > 0 ? page.Controls[0] : null;
            if (control is ILifecycleAware la && !la.CanClose()) return;
            if (control is IResourceManager rm) try { rm.ReleaseResources(); } catch { }

            if (page.Tag is string pid) _openTabs.Remove(pid);
            xtraTabControlMain.TabPages.Remove(page);
            page.Dispose();
        }

        private void ActivateTabContent(XtraTabPage page)
        {
            if (page.Controls.Count > 0 && page.Controls[0] is ILifecycleAware la) la.OnActivated();
        }

        private void DeactivateTabContent(XtraTabPage page)
        {
            if (page.Controls.Count > 0 && page.Controls[0] is ILifecycleAware la) la.OnDeactivated();
        }

        private XtraTabPage? FindTabByProgId(string id) =>
            xtraTabControlMain.TabPages.FirstOrDefault(p => string.Equals(p.Tag as string, id, StringComparison.OrdinalIgnoreCase));

        #endregion

        #region Menu Management

        private void BuildMenu()
        {
            LogManager.Info("[BuildMenu] 메뉴 구성 시작", "Shell");
            var user = UserSession.Current;
            var manager = barManager1;

            if (manager == null || barMainMenu == null)
            {
                LogManager.Error("[BuildMenu] 필수 UI 컴포넌트(BarManager/MainMenu)가 초기화되지 않았습니다.", "Shell");
                return;
            }

            manager.BeginUpdate();
            try
            {
                barMainMenu.ItemLinks.Clear();

                if (user == null || !user.IsLoggedIn)
                {
                    LogManager.Warning("[BuildMenu] 유효한 사용자 세션이 없습니다.", "Shell");
                    return;
                }

                LogManager.Info($"[BuildMenu] 사용자: {user.UserId}, 권한: {user.AuthLevel}, 부서: {user.SelectedDeptCode}", "Shell");

                // 1. 데이터 취득
                if (string.IsNullOrWhiteSpace(user.SelectedDeptCode))
                {
                    LogManager.Info("[BuildMenu] 부서 미선택 - 안내 메뉴 표시", "Shell");
                    AddEmptyMenuNotice(manager, "시스템 (부서 미선택)", "로그인 시 부서를 선택해야 메뉴가 활성화됩니다.");
                    return;
                }

                var allMenus = _menuRepo.GetMenusByUserAndDept(user.UserId, user.SelectedDeptCode);
                if (allMenus == null || allMenus.Count == 0)
                {
                    LogManager.Info("[BuildMenu] 사용자 전용 메뉴가 없어 전체 메뉴를 로드합니다.", "Shell");
                    allMenus = _menuRepo.GetAllMenus();
                }

                if (allMenus == null || allMenus.Count == 0)
                {
                    LogManager.Warning("[BuildMenu] 표시할 메뉴 데이터가 데이터베이스에 존재하지 않습니다.", "Shell");
                    return;
                }

                // 2. 권한 필터링 (레벨 0은 관리자로 간주하여 모두 허용)
                var filteredMenus = allMenus                    
                    .OrderBy(m => m.SortOrd)
                    .ToList();

                LogManager.Info($"[BuildMenu] 로드된 {allMenus.Count}개 중 {filteredMenus.Count}개 메뉴가 사용자 권한에 부합합니다.", "Shell");

                // 3. 트리 구성 (루트 메뉴 검색)
                var roots = filteredMenus.Where(m => string.IsNullOrWhiteSpace(m.ParentId)).ToList();
                LogManager.Info($"[BuildMenu] {roots.Count}개의 최상위 메뉴를 처리합니다.", "Shell");

                foreach (var menuDto in roots)
                {
                    var subMenu = CreateBarSubMenu(manager, menuDto.MenuName);

                    // 재귀적으로 하위 구성 (ID 매칭 강화)
                    BuildBarMenuRecursive(subMenu, menuDto.MenuId, filteredMenus, user.AuthLevel, manager);

                    // 하위 메뉴가 있거나 루트 자체가 실행 가능한 경우에만 추가
                    bool hasProgram = !string.IsNullOrWhiteSpace(menuDto.ProgId);
                    bool hasChildren = subMenu.ItemLinks.Count > 0;

                    if (hasProgram || hasChildren)
                    {
                        if (hasProgram)
                        {
                            subMenu.AddItem(CreateBarButtonItem(manager, menuDto.MenuName, (s, e) => OpenProgram(menuDto.ProgId!, menuDto.MenuName)));
                        }

                        barMainMenu.ItemLinks.Add(subMenu);
                        LogManager.Debug($"[BuildMenu] 상단 메뉴 추가: {menuDto.MenuName}", "Shell");
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.Error("[BuildMenu] 메뉴 빌드 중 치명적 오류", "Shell", ex);
            }
            finally
            {
                AddSystemCommonMenu(manager);
                manager.EndUpdate();
                barMainMenu.Visible = true;
                LogManager.Info("[BuildMenu] 메뉴 빌드 프로세스 완료", "Shell");
            }
        }

        private void BuildBarMenuRecursive(BarSubItem parent, string parentId, List<MenuDto> all, int auth, BarManager m)
        {
            if (string.IsNullOrWhiteSpace(parentId)) return;

            // ID 비교 시 Trim 및 대소문자 무시 적용으로 매칭 성공률 극대화
            var children = all.Where(x => string.Equals(x.ParentId?.Trim(), parentId.Trim(), StringComparison.OrdinalIgnoreCase))
                              .OrderBy(x => x.SortOrd);

            foreach (var child in children)
            {
                if (!string.IsNullOrWhiteSpace(child.ProgId))
                {
                    parent.AddItem(CreateBarButtonItem(m, child.MenuName, (s, e) => OpenProgram(child.ProgId!, child.MenuName)));
                }
                else
                {
                    var group = CreateBarSubMenu(m, child.MenuName);
                    BuildBarMenuRecursive(group, child.MenuId, all, auth, m);

                    // 내용이 있는 그룹만 부모에 추가
                    if (group.ItemLinks.Count > 0)
                    {
                        parent.AddItem(group);
                    }
                }
            }
        }

        private void AddSystemCommonMenu(BarManager m)
        {
            if (barMainMenu == null) return;

            var s = CreateBarSubMenu(m, "시스템");
            s.AddItem(CreateBarButtonItem(m, "메뉴 새로고침", (x, y) => BuildMenu()));
            s.AddItem(CreateBarButtonItem(m, "모든 탭 닫기", (x, y) => { while (xtraTabControlMain.TabPages.Count > 0) CloseTab(xtraTabControlMain.TabPages[0]); }));

            // [추가] 개발자용 테스트 메뉴 (개발 환경인 경우에만 노출)
            if (IsDevelopmentMode())
            {
                var devMenu = CreateBarSubMenu(m, "개발자 도구");
                devMenu.AddItem(CreateBarButtonItem(m, "서버 연결 상세 테스트", (x, y) => RunServerConnectionTest()));
                devMenu.AddItem(CreateBarButtonItem(m, "로컬 로그 폴더 열기", (x, y) => OpenLogFolder()));
                devMenu.AddItem(CreateBarButtonItem(m, "모듈 전체 재검색/로드", (x, y) => _moduleLoader.LoadAllModules()));
                s.AddItem(devMenu);
            }

            s.AddItem(CreateBarButtonItem(m, "로그아웃", (x, y) => this.Close()));

            barMainMenu.ItemLinks.Add(s);
        }

        private bool IsDevelopmentMode()
        {
            try
            {
                if (_appConfig != null && _appConfig.RootElement.TryGetProperty("Environment", out var env))
                {
                    var mode = env.GetProperty("Mode").GetString();
                    return string.Equals(mode, "Development", StringComparison.OrdinalIgnoreCase);
                }
            }
            catch { }
            return false;
        }

        private async void RunServerConnectionTest()
        {
            ShowSplashMessage("서버 연결 테스트 중...");
            try
            {
                var result = await ConnectivityManager.Instance.TestAllConnectionsAsync();

                string msg = $"[서버 연결 테스트 결과]\n\n" +
                             $"전체 성공: {result.AllConnected}\n" +
                             $"DB 연결: {(result.DBConnected ? "🔵" : "❌")}\n" +
                             $"파일 서버: {(result.FileConnected ? "🔵" : "❌")}\n" +
                             $"로그 서버: {(result.LogConnected ? "🔵" : "❌")}\n\n" +
                             $"테스트 시각: {result.TestTime:yyyy-MM-dd HH:mm:ss}";

                XtraMessageBox.Show(msg, "테스트 결과", MessageBoxButtons.OK, result.AllConnected ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"테스트 중 오류 발생: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                HideSplashMessage();
            }
        }

        private void OpenLogFolder()
        {
            try
            {
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
                if (LogManager.Instance.Logger is FileLogger fl)
                {
                    var filePath = fl.GetLogFilePath();
                    if (!string.IsNullOrEmpty(filePath)) path = Path.GetDirectoryName(filePath) ?? path;
                }

                if (Directory.Exists(path))
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo { FileName = path, UseShellExecute = true });
                }
                else
                {
                    XtraMessageBox.Show("로그 폴더를 찾을 수 없습니다.", "알림");
                }
            }
            catch (Exception ex) { LogManager.Error("로그 폴더 열기 실패", "Shell", ex); }
        }

        private void AddEmptyMenuNotice(BarManager manager, string rootText, string notice)
        {
            if (barMainMenu == null) return;

            var root = CreateBarSubMenu(manager, rootText);
            var item = CreateBarButtonItem(manager, notice, (s, e) => XtraMessageBox.Show(notice, "알림"));
            root.AddItem(item);
            barMainMenu.ItemLinks.Add(root);
        }

        #endregion

        #region Event Handling & Subscriptions

        private void SubscribeToEvents()
        {
            _eventAggregator.GetEvent<NavigationRequestEvent>().Subscribe(p => { if (p is NavigationRequestEventPayload e) OpenProgram(e.TargetScreenId, null, e.Context); });
            _eventAggregator.GetEvent<CloseScreenRequestEvent>().Subscribe(p => { if (p is CloseScreenRequestEventPayload e) { var pg = FindTabByProgId(e.ScreenId); if (pg != null) CloseTab(pg); } });
            _eventAggregator.GetEvent<ModuleActivatedEvent>().Subscribe(p => { if (p is ModuleActivatedEventPayload e) SafeInvoke(() => this.Text = $"nU3 HIS - [{e.ProgId}] v{e.Version}"); });
            _eventAggregator.GetEvent<Core.Events.Contracts.PatientSelectedEvent>().Subscribe(p => { if (p is PatientSelectedEventPayload e) UpdateStatusMessage($"환자 선택: {e.Patient.PatientName}"); });
        }

        private void OnModuleVersionConflict(object sender, ModuleVersionConflictEventArgs e)
        {
            this.SafeInvoke(() =>
            {
                if (XtraMessageBox.Show($"⚠️ 모듈 버전 불일치 감지 (로드:v{e.CurrentVersion}, 요청:v{e.RequestedVersion})\n\n재시작하시겠습니까?", "버전 충돌", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    Application.Restart(); Environment.Exit(0);
                }
            });
        }

        #endregion

        #region UI Utilities & Helpers

        private void SafeInvoke(Action action) => UIHelper.SafeInvoke(this, action);

        private void UpdateStatusBar()
        {
            var u = UserSession.Current;
            if (u != null) barStaticItemUser.Caption = $"👤 {u.UserId} (Lv {u.AuthLevel})";
            barStaticItemTime.Caption = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        private void UpdateStatusMessage(string message) => barStaticItemLogMessage.Caption = message;

        private void ShowSplashMessage(string msg) { UpdateStatusMessage(msg); Application.DoEvents(); }

        private void HideSplashMessage() => UpdateStatusBar();

        private BarSubItem CreateBarSubMenu(BarManager m, string t) => new BarSubItem(m, t) { ItemAppearance = { Normal = { Font = new System.Drawing.Font("Segoe UI", 9F), Options = { UseFont = true } } } };

        private BarButtonItem CreateBarButtonItem(BarManager m, string t, ItemClickEventHandler c) { var b = new BarButtonItem(m, t); b.ItemClick += c; return b; }

        private void UpdateShellTitle(XtraTabPage page)
        {
            string baseTitle = "nU3 Healthcare Information System";
            if (page.Controls.Count > 0 && page.Controls[0] is BaseWorkControl wc)
                this.Text = $"{baseTitle} - [{wc.ProgramID}] {wc.ProgramTitle} v{wc.GetType().Assembly.GetName().Version}";
            else this.Text = $"{baseTitle} - {page.Text}";
        }

        private void ProcessStartupUri(string uri)
        {
            try { var u = new Uri(uri); var query = u.Query.TrimStart('?').Split('&').Select(x => x.Split('=')).ToDictionary(x => x[0], x => x[1]); if (query.ContainsKey("programid")) OpenProgram(query["programid"]); } catch { }
        }

        private string? GetConfigValue(JsonElement p, string s, string k) { try { return p.GetProperty(s).GetProperty(k).GetString(); } catch { return null; } }

        private bool GetConfigBoolValue(JsonElement p, string s, string k) { try { return p.GetProperty(s).GetProperty(k).GetBoolean(); } catch { return false; } }

        private bool GetConfigBoolValue(JsonElement p, string s1, string s2, string k) { try { return p.GetProperty(s1).GetProperty(s2).GetProperty(k).GetBoolean(); } catch { return false; } }

        private nU3.Models.EmailSettings? ExtractEmailSettings()
        {
            if (_appConfig == null) return null;
            try
            {
                if (!_appConfig.RootElement.TryGetProperty("ErrorReporting", out var er)) return null;
                if (!er.TryGetProperty("Email", out var ec)) return null;
                return new nU3.Models.EmailSettings
                {
                    SmtpServer = ec.GetProperty("SmtpServer").GetString() ?? "smtp.gmail.com",
                    SmtpPort = ec.GetProperty("SmtpPort").GetInt32(),
                    EnableSsl = ec.GetProperty("EnableSsl").GetBoolean(),
                    Username = ec.GetProperty("Username").GetString(),
                    Password = ec.GetProperty("Password").GetString(),
                    FromEmail = ec.GetProperty("FromEmail").GetString(),
                    FromName = ec.GetProperty("FromName").GetString() ?? "nU3 Framework",
                    ToEmail = ec.GetProperty("ToEmail").GetString(),
                    TimeoutMs = ec.TryGetProperty("TimeoutMs", out var t) ? t.GetInt32() : 30000
                };
            }
            catch { return null; }
        }

        #endregion
    }
}
