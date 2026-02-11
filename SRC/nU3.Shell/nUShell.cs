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

namespace nU3.Shell
{
    public partial class nUShell : BaseWorkForm
    {
        private readonly IMenuRepository _menuRepo;
        private readonly IModuleRepository _moduleRepo;
        private readonly nU3.Core.Events.IEventAggregator _eventAggregator;
        private readonly ModuleLoaderService _moduleLoader;
        private readonly IServiceProvider _serviceProvider;
        private readonly Dictionary<string, Type> _openTabs = new Dictionary<string, Type>();
        private bool _initialized;
        private CrashReporter _crashReporter;
        private EmailSettings _emailSettings;
        private bool _errorReportingEnabled;
        private bool _loggingEnabled;
        private bool _uploadOnError;
        private bool _serverConnectionEnabled;

        public nUShell()
        {
            InitializeComponent();

            if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime)
                return;
            if (this.DesignMode)
                return;
        }

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public nUShell(
            IMenuRepository menuRepo,
            IModuleRepository moduleRepo,
            nU3.Core.Events.IEventAggregator eventAggregator,
            ModuleLoaderService moduleLoader,
            IServiceProvider serviceProvider)
            : this()
        {
            if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime)
                return;
            if (this.DesignMode)
                return;

            _menuRepo = menuRepo;
            _moduleRepo = moduleRepo;
            _eventAggregator = eventAggregator;
            _moduleLoader = moduleLoader;
            _serviceProvider = serviceProvider;



            InitializeLogging();
            InitializeServerConnection();
            InitializeErrorReporting();
            InitializeShellAppearance();
            UpdateStatusBar();

            this.FormClosing += MainShellForm_FormClosing;
            this.Load += MainShellForm_Load;

            // 모듈 버전 충돌 이벤트 구독
            _moduleLoader.VersionConflict += OnModuleVersionConflict;

            LogManager.Info("메인 셸 초기화됨", "Shell");
        }

        private void InitializeServerConnection()
        {
            try
            {
                var config = ServerConnectionConfig.Load();

                if (!config.Enabled)
                {
                    LogManager.Info("구성에서 서버 연결이 비활성화되어 있습니다.", "Shell");
                    barStaticItemServer.Caption = "🔴 서버: 비활성화";
                    _serverConnectionEnabled = false;
                    return;
                }

                LogManager.Info($"서버 연결 초기화: {config.BaseUrl}", "Shell");

                // ConnectivityManager 초기화
                ConnectivityManager.Instance.Initialize(
                    config.BaseUrl,
                    enableLogCompression: true  // 압축 활성화 (90% 대역폭 절약)
                );

                // 로그 메시지 이벤트 구독
                ConnectivityManager.Instance.LogMessage += OnConnectivityLogMessage;

                
                // 연결 테스트 (비동기)
                Task.Run(async () =>
                {
                    try
                    {
                        var connected = await ConnectivityManager.Instance.TestConnectionAsync();

                        this.Invoke((System.Windows.Forms.MethodInvoker)delegate
                        {
                            if (connected)
                            {
                                barStaticItemServer.Caption = $"🟢 {config.BaseUrl}";
                                LogManager.Info($"서버 연결 성공: {config.BaseUrl}", "Shell");
                                _serverConnectionEnabled = true;
                            }
                            else
                            {
                                barStaticItemServer.Caption = $"🟡 {config.BaseUrl} (응답 없음)";
                                LogManager.Warning($"서버 연결 실패: {config.BaseUrl}", "Shell");
                                _serverConnectionEnabled = false;
                            }
                        });
                    }
                    catch (Exception ex)
                    {
                        this.Invoke((System.Windows.Forms.MethodInvoker)delegate
                        {
                            barStaticItemServer.Caption = $"🔴 {config.BaseUrl} (오류)";
                            LogManager.Error($"서버 연결 오류: {ex.Message}", "Shell", ex);
                            _serverConnectionEnabled = false;
                        });
                    }
                });
            }
            catch (Exception ex)
            {
                LogManager.Error($"서버 연결 초기화 실패: {ex.Message}", "Shell", ex);
                barStaticItemServer.Caption = "🔴 서버: 오류";
                _serverConnectionEnabled = false;
            }
        }

        private void OnConnectivityLogMessage(object? sender, LogMessageEventArgs e)
        {
            // Connectivity 클라이언트의 로그 메시지를 LogManager로 전달
            try
            {
                switch (e.Level.ToLower())
                {
                    case "info":
                        LogManager.Info(e.Message, "Connectivity");
                        break;
                    case "warning":
                        LogManager.Warning(e.Message, "Connectivity");
                        break;
                    case "error":
                        LogManager.Error(e.Message, "Connectivity");
                        break;
                    default:
                        LogManager.Debug(e.Message, "Connectivity");
                        break;
                }
            }
            catch
            {
                // Ignore logging errors
            }
        }

        private void InitializeLogging()
        {
            try
            {
                var config = LoadConfiguration();
                if (config == null)
                    return;

                if (!config.RootElement.TryGetProperty("Logging", out var loggingConfig))
                    return;

                if (!loggingConfig.TryGetProperty("Enabled", out var enabled) || !enabled.GetBoolean())
                    return;

                _loggingEnabled = true;

                // LogManager 초기화 (서버 연결 전)
                LogManager.Instance.Initialize(
                    logDirectory: GetConfigValue(loggingConfig, "FileLogging", "LogDirectory"),
                    auditDirectory: GetConfigValue(loggingConfig, "AuditLogging", "AuditDirectory"),
                    fileTransferService: null, // ConnectivityManager로 대체됨
                    enableAutoUpload: GetConfigBoolValue(loggingConfig, "ServerUpload", "AutoUpload")
                );

                _uploadOnError = GetConfigBoolValue(loggingConfig, "ServerUpload", "UploadOnError");

                LogManager.Info("로그 시스템 초기화 성공", "Shell");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to initialize logging: {ex.Message}");
                _loggingEnabled = false;
            }
        }

        private System.Text.Json.JsonDocument LoadConfiguration()
        {
            try
            {
                var configFile = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
                if (!System.IO.File.Exists(configFile))
                    return null;

                var json = System.IO.File.ReadAllText(configFile);
                return System.Text.Json.JsonDocument.Parse(json);
            }
            catch
            {
                return null;
            }
        }

        private string GetConfigValue(System.Text.Json.JsonElement parent, string section, string key)
        {
            try
            {
                if (parent.TryGetProperty(section, out var sectionElement))
                {
                    if (sectionElement.TryGetProperty(key, out var valueElement))
                    {
                        var value = valueElement.GetString();
                        return string.IsNullOrWhiteSpace(value) ? null : value;
                    }
                }
            }
            catch { }
            return null;
        }

        private bool GetConfigBoolValue(System.Text.Json.JsonElement parent, string section, string key)
        {
            try
            {
                if (parent.TryGetProperty(section, out var sectionElement))
                {
                    if (sectionElement.TryGetProperty(key, out var valueElement))
                    {
                        return valueElement.GetBoolean();
                    }
                }
            }
            catch { }
            return false;
        }

        private void InitializeErrorReporting()
        {
            try
            {
                _emailSettings = LoadEmailSettings();
                _errorReportingEnabled = _emailSettings != null;

                if (_errorReportingEnabled)
                {
                    _crashReporter = new CrashReporter(this, _emailSettings);

                    Application.ThreadException += Application_ThreadException;
                    AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
                    TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

                    _crashReporter.CleanupOldLogs(30);

                    LogManager.Info("오류 보고(크래시리포트) 초기화됨", "Shell");
                }
            }
            catch (Exception ex)
            {
                LogManager.Error("오류 보고 초기화 실패", "Shell", ex);
                _errorReportingEnabled = false;
            }
        }

        private EmailSettings LoadEmailSettings()
        {
            try
            {
                var config = LoadConfiguration();
                if (config == null)
                    return null;

                if (!config.RootElement.TryGetProperty("ErrorReporting", out var errorReporting))
                    return null;

                if (!errorReporting.TryGetProperty("Enabled", out var enabled) || !enabled.GetBoolean())
                    return null;

                if (!errorReporting.TryGetProperty("Email", out var emailConfig))
                    return null;

                return new EmailSettings
                {
                    SmtpServer = emailConfig.GetProperty("SmtpServer").GetString(),
                    SmtpPort = emailConfig.GetProperty("SmtpPort").GetInt32(),
                    EnableSsl = emailConfig.GetProperty("EnableSsl").GetBoolean(),
                    Username = emailConfig.GetProperty("Username").GetString(),
                    Password = emailConfig.GetProperty("Password").GetString(),
                    FromEmail = emailConfig.GetProperty("FromEmail").GetString(),
                    FromName = emailConfig.GetProperty("FromName").GetString(),
                    ToEmail = emailConfig.GetProperty("ToEmail").GetString()
                };
            }
            catch
            {
                return null;
            }
        }

        private void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            HandleUnhandledException(e.Exception, "UI Thread Exception");
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception exception)
            {
                HandleUnhandledException(exception, "AppDomain Unhandled Exception");
            }
        }

        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            HandleUnhandledException(e.Exception, "Task Unobserved Exception");
            e.SetObserved();
        }

        private void HandleUnhandledException(Exception exception, string source)
        {
            try
            {
                var additionalInfo = $"Source: {source}\nActive Tab: {GetActiveTabInfo()}\nOpen Tabs Count: {xtraTabControlMain.TabPages.Count}";

                // 로그 기록
                LogManager.Critical($"처리되지 않은 예외 - {source}", "Error", exception);
                LogManager.Critical($"추가 정보: {additionalInfo}", "Error");

                // 서버에 로그 업로드 (ConnectivityManager 사용)
                if (_loggingEnabled && _uploadOnError && _serverConnectionEnabled && ConnectivityManager.Instance.IsInitialized)
                {
                    Task.Run(async () =>
                    {
                        try
                        {
                            // 즉시 업로드
                            await ConnectivityManager.Instance.Log.UploadCurrentLogImmediatelyAsync();

                            LogManager.Info("오류 로그를 서버에 업로드함", "Shell");
                        }
                        catch (Exception ex)
                        {
                            LogManager.Warning($"오류 로그 업로드 실패: {ex.Message}", "Shell");
                        }
                    }).Wait(TimeSpan.FromSeconds(5));
                }

                // 크래시 리포트 전송
                if (_errorReportingEnabled && _crashReporter != null)
                {
                    var task = _crashReporter.ReportCrashAsync(exception, additionalInfo);
                    task.Wait(TimeSpan.FromSeconds(10));
                }

                var result = XtraMessageBox.Show(
                    $"예상치 않은 오류가 발생했습니다.\n\n" +
                    $"오류: {exception.Message}\n\n" +
                    $"{(_errorReportingEnabled ? "에러 리포트가 관리자에게 자동으로 전송되었습니다.\n\n" : "")}" +
                    $"프로그램을 종료하시겠습니까?",
                    "오류 발생",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Error);

                if (result == DialogResult.Yes)
                {
                    LogManager.Info("사용자가 오류 후 종료를 선택함", "Shell");
                    Environment.Exit(1);
                }
            }
            catch
            {
                Environment.Exit(1);
            }
        }

        private string GetActiveTabInfo()
        {
            try
            {
                var selectedTab = xtraTabControlMain.SelectedTabPage;
                if (selectedTab != null)
                {
                    var progId = selectedTab.Tag as string;
                    return $"{selectedTab.Text} ({progId ?? "알 수 없음"})";
                }
                return "활성 탭 없음";
            }
            catch
            {
                return "알 수 없음";
            }
        }

        private void InitializeShellAppearance()
        {
            xtraTabControlMain.AppearancePage.Header.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            xtraTabControlMain.AppearancePage.Header.Options.UseFont = true;
            xtraTabControlMain.AppearancePage.HeaderActive.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            xtraTabControlMain.AppearancePage.HeaderActive.Options.UseFont = true;

            barStaticItemVersion.Caption = $"v{Assembly.GetExecutingAssembly().GetName().Version}";
        }

        private void MainShellForm_Load(object sender, EventArgs e)
        {
            if (_initialized)
                return;

            if (IsDesignMode())
                return;

            _initialized = true;

            LogManager.Info("메인 셸 로딩 시작", "Shell");

            ShowSplashMessage("모듈 로딩 중...");
            _moduleLoader.LoadAllModules();
            LogManager.Info("모듈 로딩 완료", "Shell");

            ShowSplashMessage("메뉴 구성 중...");
            BuildMenu();
            LogManager.Info("메뉴 구성 완료", "Shell");

            HideSplashMessage();

            // EventBus 구독 (모듈 간 통신)
            SubscribeToEvents();

            // LogManager 메시지 구독
            if (LogManager.Instance.Logger != null)
            {
                LogManager.Instance.Logger.MessageLogged += (s, msg) => {
                    this.Invoke(new Action(() => UpdateStatusMessage(msg)));
                };
            }

            LogManager.Info("메인 셸 로딩 완료", "Shell");

            // 로그인 오딧
            LogManager.LogAction(AuditAction.Login, "Shell", "MainShellForm", $"사용자 로그인: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        }

        private void SubscribeToEvents()
        {
            // 네비게이션 요청 구독
            _eventAggregator?.GetEvent<NavigationRequestEvent>()
                .Subscribe(OnNavigationRequest);

            // 화면 닫기 요청 구독
            _eventAggregator?.GetEvent<CloseScreenRequestEvent>()
                .Subscribe(OnCloseScreenRequest);

            // 모듈 활성화 이벤트 구독 (제목 표시줄 업데이트용)
            _eventAggregator?.GetEvent<ModuleActivatedEvent>()
                .Subscribe(OnModuleActivated);

            // 신규 제네릭 환자 선택 이벤트 구독 (강타입 계약 사용)
            _eventAggregator?.GetEvent<Core.Events.Contracts.PatientSelectedEvent>()
                .Subscribe(OnPatientSelectedGeneric);

            LogManager.Info("메인 셸이 이벤트를 구독함", "Shell");
        }

        private void OnModuleActivated(object payload)
        {
            if (payload is not ModuleActivatedEventPayload evt)
                return;

            // UI 스레드에서 실행
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => OnModuleActivated(payload)));
                return;
            }

            var defaultTitle = "nU3 Healthcare Information System";
            
            // 모듈에서 전달받은 데이터를 사용하여 제목 업데이트
            // DB 조회 없이 현재 실행 중인 모듈의 실제 정보를 반영함
            var version = evt.Version ?? "Unknown";
            this.Text = $"{defaultTitle} - [{evt.ProgId}] v{version}";
            
            LogManager.Debug($"Shell Title 업데이트됨: {evt.ProgId} (v{version})", "Shell");
        }

        private void OnPatientSelectedGeneric(PatientSelectedEventPayload context)
        {
            // UI 스레드에서 상태 표시줄 업데이트
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => OnPatientSelectedGeneric(context)));
                return;
            }

            UpdateStatusMessage($"[계약 기반 데이터 흐름] 환자 선택: {context.Patient.PatientName} ({context.Patient.PatientId})");
            LogManager.Info($"제네릭 이벤트로 글로벌 컨텍스트 업데이트: {context.Patient.PatientId}", "Shell");
        }

        /// <summary>
        /// 모듈 버전 충돌 이벤트 처리
        /// </summary>
        private void OnModuleVersionConflict(object sender, nU3.Core.Services.ModuleVersionConflictEventArgs e)
        {
            // UI 스레드에서 실행
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => OnModuleVersionConflict(sender, e)));
                return;
            }

            // 로그 기록
            LogManager.Warning(
                $"모듈 버전 충돌 감지 - Module: {e.ModuleId}, " +
                $"Current: v{e.CurrentVersion}, Requested: v{e.RequestedVersion}", 
                "Shell");

            // 사용자에게 알림
            var result = XtraMessageBox.Show(
                $"⚠️ 모듈 버전 불일치 감지\n\n" +
                $"모듈: {e.ModuleId}\n" +
                $"현재 로드된 버전: v{e.CurrentVersion}\n" +
                $"요청된 버전: v{e.RequestedVersion}\n\n" +
                $"타입 불일치를 방지하기 위해 현재 버전을 계속 사용합니다.\n\n" +
                $"권장 사항:\n" +
                $"- 이 모듈의 모든 인스턴스(탭)를 닫으세요.\n" +
                $"- 프로그램을 재시작하세요.\n\n" +
                $"프로그램을 지금 재시작하시겠습니까?",
                "모듈 버전 충돌",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                LogManager.Info("사용자가 버전 충돌 해결을 위해 재시작 선택", "Shell");
                Application.Restart();
                Environment.Exit(0);
            }
        }

        private void OnNavigationRequest(object payload)
        {
            if (payload is not NavigationRequestEventPayload evt)
                return;

            try
            {
                LogManager.Info($"네비게이션 요청: {evt.TargetScreenId} (출처: {evt.Source})", "Shell");

                // 프로그램 열기
                OpenProgram(evt.TargetScreenId);

                // 컨텍스트가 있으면 전달
                if (evt.Context != null)
                {
                    var tabPage = FindTabByProgId(evt.TargetScreenId);
                    if (tabPage?.Controls.Count > 0)
                    {
                        var control = tabPage.Controls[0];

                        if (control is IWorkContextProvider contextProvider)
                        {
                            contextProvider.UpdateContext(evt.Context);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.Error($"네비게이션 요청 실패: {ex.Message}", "Shell", ex);
            }
        }

        private void OnCloseScreenRequest(object payload)
        {
            if (payload is not CloseScreenRequestEventPayload evt)
                return;

            try
            {
                var tabPage = FindTabByProgId(evt.ScreenId);
                if (tabPage != null)
                {
                    if (evt.Force)
                    {
                        // 강제 닫기
                        CloseTab(tabPage);
                    }
                    else
                    {
                        // 확인 후 닫기
                        var control = tabPage.Controls.Count > 0 ? tabPage.Controls[0] : null;
                        if (control is ILifecycleAware lifecycleAware)
                        {
                            if (lifecycleAware.CanClose())
                            {
                                CloseTab(tabPage);
                            }
                        }
                        else
                        {
                            CloseTab(tabPage);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.Error($"화면 닫기 요청 실패: {ex.Message}", "Shell", ex);
            }
        }

        private XtraTabPage FindTabByProgId(string progId)
        {
            foreach (XtraTabPage page in xtraTabControlMain.TabPages)
            {
                if (string.Equals(page.Tag as string, progId, StringComparison.OrdinalIgnoreCase))
                {
                    return page;
                }
            }
            return null;
        }

        /// <summary>
        /// 모든 열린 모듈에 컨텍스트 변경 브로드캐스트
        /// </summary>
        public void BroadcastContextChange(nU3.Core.Context.WorkContext newContext)
        {
            try
            {
                // 이벤트 발행
                _eventAggregator?.GetEvent<WorkContextChangedEvent>()
                    .Publish(new WorkContextChangedEventPayload
                    {
                        NewContext = newContext,
                        Source = "MainShell"
                    });

                // 모든 열린 탭에 직접 업데이트
                foreach (XtraTabPage page in xtraTabControlMain.TabPages)
                {
                    if (page.Controls.Count > 0)
                    {
                        var control = page.Controls[0];

                        if (control is IWorkContextProvider contextProvider)
                        {
                            contextProvider.UpdateContext(newContext);
                        }
                    }
                }

                LogManager.Info("컨텍스트 변경이 모든 모듈에 브로드캐스트됨", "Shell");
            }
            catch (Exception ex)
            {
                LogManager.Error($"컨텍스트 변경 브로드캐스트 중 오류: {ex.Message}", "Shell", ex);
            }
        }

        /// <summary>
        /// 환자 선택 이벤트 발행 (MainShell에서 환자 선택 시)
        /// </summary>
        public void SelectPatient(PatientInfoDto patient)
        {
            if (patient == null)
                return;

            try
            {
                // 이벤트 발행
                _eventAggregator?.GetEvent<nU3.Core.Events.PatientSelectedEvent>()
                    .Publish(new PatientSelectedEventPayload
                    {
                        Patient = patient,
                        Source = "MainShell"
                    });

                // 컨텍스트 생성 및 브로드캐스트
                var context = new nU3.Core.Context.WorkContext();
                context.CurrentPatient = patient;

                var currentUser = nU3.Core.Security.UserSession.Current;
                if (currentUser != null)
                {
                    context.CurrentUser = new UserInfoDto
                    {
                        UserId = currentUser.UserId,
                        UserName = currentUser.UserName,
                        AuthLevel = currentUser.AuthLevel
                    };
                    context.Permissions = CreatePermissionsForUser(currentUser.AuthLevel);
                }

                BroadcastContextChange(context);

                LogManager.Info($"환자 선택 및 브로드캐스트: {patient.PatientName} ({patient.PatientId})", "Shell");
            }
            catch (Exception ex)
            {
                LogManager.Error($"환자 선택 중 오류: {ex.Message}", "Shell", ex);
            }
        }

        /// <summary>
        /// 검사 선택 이벤트 발행
        /// </summary>
        public void SelectExam(ExamOrderDto exam, PatientInfoDto patient)
        {
            if (exam == null)
                return;

            try
            {
                // 이벤트 발행
                _eventAggregator?.GetEvent<ExamSelectedEvent>()
                    .Publish(new ExamSelectedEventPayload
                    {
                        Exam = exam,
                        Patient = patient,
                        Source = "MainShell"
                    });

                // 컨텍스트 생성 및 브로드캐스트
                var context = new nU3.Core.Context.WorkContext();
                context.CurrentPatient = patient;
                context.CurrentExam = exam;

                var currentUser = nU3.Core.Security.UserSession.Current;
                if (currentUser != null)
                {
                    context.CurrentUser = new UserInfoDto
                    {
                        UserId = currentUser.UserId,
                        UserName = currentUser.UserName,
                        AuthLevel = currentUser.AuthLevel
                    };
                    context.Permissions = CreatePermissionsForUser(currentUser.AuthLevel);
                }

                BroadcastContextChange(context);

                LogManager.Info($"검사 선택 및 브로드캐스트: {exam.ExamOrderId}", "Shell");
            }
            catch (Exception ex)
            {
                LogManager.Error($"검사 선택 오류: {ex.Message}", "Shell", ex);
            }
        }

        private nU3.Core.Context.ModulePermissions CreatePermissionsForUser(int authLevel)
        {
            var permissions = new nU3.Core.Context.ModulePermissions();

            if (authLevel == 0)
            {
                permissions.GrantAll();
            }
            else if (authLevel >= 1 && authLevel <= 9)
            {
                permissions.CanRead = true;
                permissions.CanCreate = true;
                permissions.CanUpdate = true;
                permissions.CanPrint = true;
                permissions.CanExport = true;
            }
            else if (authLevel == 10)
            {
                permissions.CanRead = true;
                permissions.CanCreate = true;
                permissions.CanUpdate = true;
                permissions.CanDelete = true;
                permissions.CanPrint = true;
                permissions.CanApprove = true;
            }
            else if (authLevel == 11)
            {
                permissions.CanRead = true;
                permissions.CanCreate = true;
                permissions.CanUpdate = true;
                permissions.CanPrint = true;
            }
            else
            {
                permissions.CanRead = true;
            }

            return permissions;
        }

        private void AddSystemMenu(BarManager manager)
        {
            var systemMenu = new BarSubItem(manager, "시스템");
            systemMenu.ItemAppearance.Normal.Font = new System.Drawing.Font("Segoe UI", 9F);
            systemMenu.ItemAppearance.Normal.Options.UseFont = true;

            var btnRefreshMenu = new BarButtonItem(manager, "메뉴 새로고침");
            btnRefreshMenu.ItemClick += (s, e) => BuildMenu();
            systemMenu.AddItem(btnRefreshMenu);

            var btnCloseAllTabs = new BarButtonItem(manager, "모든 탭 닫기");
            btnCloseAllTabs.ItemClick += (s, e) => CloseAllTabs();
            systemMenu.AddItem(btnCloseAllTabs);

            // 서버 연결 메뉴
            var btnServerConnection = new BarButtonItem(manager, "서버 연결 상태");
            btnServerConnection.ItemClick += (s, e) => ShowServerConnectionStatus();
            systemMenu.AddItem(btnServerConnection);

            var btnTestConnection = new BarButtonItem(manager, "서버 연결 테스트 (전체)");
            btnTestConnection.ItemClick += (s, e) => TestServerConnection();
            systemMenu.AddItem(btnTestConnection);

            // 개별 서비스 테스트 서브메뉴
            var testSubMenu = new BarSubItem(manager, "개별 서비스 테스트");
            testSubMenu.ItemAppearance.Normal.Font = new System.Drawing.Font("Segoe UI", 9F);

            var btnTestDB = new BarButtonItem(manager, "Database 연결 테스트");
            btnTestDB.ItemClick += async (s, e) => await TestDatabaseConnection();
            testSubMenu.AddItem(btnTestDB);

            var btnTestFile = new BarButtonItem(manager, "File Transfer 연결 테스트");
            btnTestFile.ItemClick += async (s, e) => await TestFileConnection();
            testSubMenu.AddItem(btnTestFile);

            var btnTestLog = new BarButtonItem(manager, "Log Upload 연결 테스트");
            btnTestLog.ItemClick += async (s, e) => await TestLogConnection();
            testSubMenu.AddItem(btnTestLog);

            systemMenu.AddItem(testSubMenu);

            var btnErrorReport = new BarButtonItem(manager, "에러 리포팅 설정");
            btnErrorReport.ItemClick += (s, e) => ShowErrorReportingSettings();
            systemMenu.AddItem(btnErrorReport);

            var btnTestCrash = new BarButtonItem(manager, "크래시 리포트 테스트");
            btnTestCrash.ItemClick += (s, e) => TestCrashReport();
            systemMenu.AddItem(btnTestCrash);

            var btnAbout = new BarButtonItem(manager, "정보");
            btnAbout.ItemClick += (s, e) => ShowAboutDialog();
            systemMenu.AddItem(btnAbout);

            var btnExit = new BarButtonItem(manager, "종료");
            btnExit.ItemClick += (s, e) => this.Close();
            systemMenu.AddItem(btnExit);

            barMainMenu.AddItem(systemMenu);
        }

        private void ShowServerConnectionStatus()
        {
            var status = ConnectivityManager.Instance.IsInitialized ? "초기화됨" : "초기화 안 됨";
            var serverUrl = ConnectivityManager.Instance.ServerUrl ?? "없음";
            var compression = ConnectivityManager.Instance.EnableLogCompression ? "활성화" : "비활성화";
            var connected = _serverConnectionEnabled ? "연결됨" : "연결 안 됨";

            var message = $"서버 연결 상태\n\n" +
                         $"상태: {status}\n" +
                         $"서버 URL: {serverUrl}\n" +
                         $"연결: {connected}\n" +
                         $"로그 압축: {compression}\n\n" +
                         $"설정은 appsettings.json 파일에서 변경할 수 있습니다.";

            XtraMessageBox.Show(message, "서버 연결 상태", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private async void TestServerConnection()
        {
            if (!ConnectivityManager.Instance.IsInitialized)
            {
                XtraMessageBox.Show(
                    "서버 연결이 초기화되지 않았습니다.\n\nappsettings.json에서 ServerConnection.Enabled를 true로 설정하세요.",
                    "연결 안 됨",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

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

                // 전체 테스트 실행
                var result = await ConnectivityManager.Instance.TestAllConnectionsAsync();

                progressForm.Close();
                progressForm.Dispose();

                if (result.AllConnected)
                {
                    barStaticItemServer.Caption = $"🟢 {ConnectivityManager.Instance.ServerUrl}";
                    _serverConnectionEnabled = true;

                    XtraMessageBox.Show(
                        $"서버 연결 성공!\n\n" +
                        $"서버: {ConnectivityManager.Instance.ServerUrl}\n\n" +
                        $"✅ Database: 연결됨\n" +
                        $"✅ File Transfer: 연결됨\n" +
                        $"✅ Log Upload: 연결됨",
                        "연결 성공",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                else
                {
                    barStaticItemServer.Caption = $"🟡 {ConnectivityManager.Instance.ServerUrl} (일부 실패)";
                    _serverConnectionEnabled = result.DBConnected || result.FileConnected; // 하나라도 연결되면 활성화

                    var statusMessage = new System.Text.StringBuilder();
                    statusMessage.AppendLine($"서버: {ConnectivityManager.Instance.ServerUrl}");
                    statusMessage.AppendLine();
                    statusMessage.AppendLine($"{(result.DBConnected ? "✅" : "❌")} Database: {(result.DBConnected ? "연결됨" : $"실패 - {result.DBError}")}");
                    statusMessage.AppendLine($"{(result.FileConnected ? "✅" : "❌")} File Transfer: {(result.FileConnected ? "연결됨" : $"실패 - {result.FileError}")}");
                    statusMessage.AppendLine($"{(result.LogConnected ? "✅" : "❌")} Log Upload: {(result.LogConnected ? "연결됨" : $"실패 - {result.LogError}")}");

                    if (!string.IsNullOrEmpty(result.GeneralError))
                    {
                        statusMessage.AppendLine();
                        statusMessage.AppendLine($"일반 오류: {result.GeneralError}");
                    }

                    XtraMessageBox.Show(
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

                barStaticItemServer.Caption = $"🔴 Error";
                _serverConnectionEnabled = false;

                XtraMessageBox.Show(
                    $"연결 테스트 중 오류 발생!\n\n{ex.Message}",
                    "오류",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private async Task TestDatabaseConnection()
        {
            if (!ConnectivityManager.Instance.IsInitialized)
            {
                XtraMessageBox.Show("서버 연결이 초기화되지 않았습니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var connected = await ConnectivityManager.Instance.TestDBConnectionAsync();

                if (connected)
                {
                    XtraMessageBox.Show(
                        $"Database 연결 성공!\n\n서버: {ConnectivityManager.Instance.ServerUrl}",
                        "연결 성공",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                else
                {
                    XtraMessageBox.Show(
                        $"Database 연결 실패!\n\n서버: {ConnectivityManager.Instance.ServerUrl}\n\n데이터베이스 서비스가 실행 중인지 확인하세요.",
                        "연결 실패",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    $"Database 테스트 중 오류!\n\n{ex.Message}",
                    "오류",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private async Task TestFileConnection()
        {
            if (!ConnectivityManager.Instance.IsInitialized)
            {
                XtraMessageBox.Show("서버 연결이 초기화되지 않았습니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var connected = await ConnectivityManager.Instance.TestFileConnectionAsync();

                if (connected)
                {
                    XtraMessageBox.Show(
                        $"File Transfer 연결 성공!\n\n서버: {ConnectivityManager.Instance.ServerUrl}",
                        "연결 성공",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                else
                {
                    XtraMessageBox.Show(
                        $"File Transfer 연결 실패!\n\n서버: {ConnectivityManager.Instance.ServerUrl}\n\n파일 전송 서비스가 실행 중인지 확인하세요.",
                        "연결 실패",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    $"File Transfer 테스트 중 오류!\n\n{ex.Message}",
                    "오류",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private async Task TestLogConnection()
        {
            if (!ConnectivityManager.Instance.IsInitialized)
            {
                XtraMessageBox.Show("서버 연결이 초기화되지 않았습니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var connected = await ConnectivityManager.Instance.TestLogConnectionAsync();

                if (connected)
                {
                    XtraMessageBox.Show(
                        $"Log Upload 연결 성공!\n\n서버: {ConnectivityManager.Instance.ServerUrl}\n\n테스트 로그 파일이 업로드되었습니다.",
                        "연결 성공",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                else
                {
                    XtraMessageBox.Show(
                        $"Log Upload 연결 실패!\n\n서버: {ConnectivityManager.Instance.ServerUrl}\n\n로그 업로드 서비스가 실행 중인지 확인하세요.",
                        "연결 실패",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    $"Log Upload 테스트 중 오류!\n\n{ex.Message}",
                    "오류",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void MainShellForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                if (!ConfirmCloseAllTabs())
                {
                    e.Cancel = true;
                    return;
                }

                var result = XtraMessageBox.Show(
                    "프로그램을 종료하시겠습니까?",
                    "종료 확인",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result != DialogResult.Yes)
                {
                    e.Cancel = true;
                    return;
                }

                LogManager.LogAction(AuditAction.Logout, "Shell", "MainShellForm", $"User logged out at {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                LogManager.Info("Application closing", "Shell");
            }

            if (!e.Cancel)
            {
                return;


                // 종료 처리
                try
                {
                    // 에러 리포팅 이벤트 구독 해제
                    if (_errorReportingEnabled)
                    {
                        Application.ThreadException -= Application_ThreadException;
                        AppDomain.CurrentDomain.UnhandledException -= CurrentDomain_UnhandledException;
                        TaskScheduler.UnobservedTaskException -= TaskScheduler_UnobservedTaskException;
                    }

                    // 서버 연결된 경우 대기 중인 로그 업로드
                    if (_serverConnectionEnabled && ConnectivityManager.Instance.IsInitialized)
                    {
                        try
                        {
                            LogManager.Info("종료 전 대기 중인 로그 서버로 업로드 중", "Shell");

                            var uploadTask = Task.Run(async () =>
                            {
                                // 대기 중인 모든 로그 업로드
                                await ConnectivityManager.Instance.Log.UploadAllPendingLogsAsync();
                            });

                            // 최대 10초 대기
                            if (!uploadTask.Wait(TimeSpan.FromSeconds(10)))
                            {
                                LogManager.Warning("종료 중 로그 업로드 타임아웃", "Shell");
                            }
                            else
                            {
                                LogManager.Info("대기 중인 로그 업로드 완료", "Shell");
                            }
                        }
                        catch (Exception ex)
                        {
                            LogManager.Warning($"대기 중인 로그 업로드 실패: {ex.Message}", "Shell");
                        }

                        // ConnectivityManager 이벤트 구독 해제
                        ConnectivityManager.Instance.LogMessage -= OnConnectivityLogMessage;

                        // ConnectivityManager 정리
                        ConnectivityManager.Instance.Dispose();
                        LogManager.Info("ConnectivityManager 정리됨", "Shell");
                    }

                    // LogManager 종료
                    if (_loggingEnabled)
                    {
                        LogManager.Instance.Shutdown();
                        LogManager.Info("LogManager 종료 완료", "Shell");
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error during shutdown: {ex.Message}");
                }
            }
        }

        private static bool IsDesignMode()
        {
            return System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime;
        }

        private void TimerStatusUpdate_Tick(object sender, EventArgs e)
        {
            UpdateStatusBar();
        }

        private void UpdateStatusBar()
        {
            var user = nU3.Core.Security.UserSession.Current;
            if (user != null)
            {
                barStaticItemUser.Caption = $"👤 {user.UserId} (Level {user.AuthLevel})";
            }

            barStaticItemTime.Caption = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        private void BuildMenu()
        {
            var user = nU3.Core.Security.UserSession.Current;
            var manager = barManager1;
            if (manager == null)
                return;

            manager.BeginUpdate();
            try
            {
                var itemsToRemove = manager.Items.Cast<BarItem>()
                    .Where(i => i != barStaticItemUser &&
                               i != barStaticItemTime &&
                               i != barStaticItemServer &&
                               i != barStaticItemDatabase &&
                               i != barStaticItemVersion)
                    .ToList();

                foreach (var item in itemsToRemove)
                {
                    manager.Items.Remove(item);
                }

                barMainMenu.ItemLinks.Clear();

                if (string.IsNullOrWhiteSpace(user.SelectedDeptCode))
                {
                    var root = new BarSubItem(manager, "System (No Department)");
                    var item = new BarButtonItem(manager, "Please select a department.");
                    item.ItemClick += (s, e) => XtraMessageBox.Show("Please select a department during login.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    root.AddItem(item);
                    barMainMenu.AddItem(root);
                    return;
                }

                List<MenuDto> allMenus;

                // 1단계: 사용자+부서 커스텀 메뉴
                allMenus = _menuRepo.GetMenusByUserAndDept(user.UserId, user.SelectedDeptCode);

                // 2단계: 부서 템플릿 메뉴 (커스텀 메뉴가 없으면)
                if (allMenus.Count == 0)
                {
                    allMenus = _menuRepo.GetMenusByDeptCode(user.SelectedDeptCode);
                }

                // 3단계: 글로벌 템플릿 메뉴 (부서 템플릿도 없으면)
                if (allMenus.Count == 0)
                {
                    allMenus = _menuRepo.GetAllMenus();
                }

                // AuthLevel로 필터링
                var filteredMenus = allMenus
                    .Where(m => m.AuthLevel <= user.AuthLevel)
                    .OrderBy(m => m.SortOrd)
                    .ToList();

                if (filteredMenus.Count == 0)
                {
                    var root = new BarSubItem(manager, "System (Empty)");
                    var item = new BarButtonItem(manager, "No Menus Available");
                    item.ItemClick += (s, e) => XtraMessageBox.Show("No menus configured for your role or department.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    root.AddItem(item);
                    barMainMenu.AddItem(root);
                    return;
                }

                var rootMenus = filteredMenus
                    .Where(m => m.ParentId == null)
                    .ToList();

                foreach (var m in rootMenus)
                {
                    var subItem = new BarSubItem(manager, m.MenuName);
                    subItem.ItemAppearance.Normal.Font = new System.Drawing.Font("Segoe UI", 9F);
                    subItem.ItemAppearance.Normal.Options.UseFont = true;

                    BuildBarMenuRecursive(subItem, m.MenuId, filteredMenus, user.AuthLevel, manager);

                    if (!string.IsNullOrEmpty(m.ProgId))
                    {
                        var prog = new BarButtonItem(manager, m.MenuName);
                        prog.ItemClick += (s, e) => OpenProgram(m.ProgId, m.MenuName);
                        subItem.AddItem(prog);
                    }

                    if (subItem.ItemLinks.Count > 0)
                        barMainMenu.AddItem(subItem);
                }

            }
            finally
            {
                AddSystemMenu(manager);

                manager.EndUpdate();
            }
        }

        private void BuildBarMenuRecursive(BarSubItem parent, string parentId, List<MenuDto> allMenus, int userAuth, BarManager manager)
        {
            var children = allMenus
                .Where(m => m.ParentId == parentId && m.AuthLevel <= userAuth)
                .OrderBy(m => m.SortOrd)
                .ToList();

            foreach (var child in children)
            {
                if (!string.IsNullOrEmpty(child.ProgId))
                {
                    var item = new BarButtonItem(manager, child.MenuName);
                    item.ItemClick += (s, e) => OpenProgram(child.ProgId, child.MenuName);
                    parent.AddItem(item);
                }
                else
                {
                    var group = new BarSubItem(manager, child.MenuName);
                    parent.AddItem(group);
                    BuildBarMenuRecursive(group, child.MenuId, allMenus, userAuth, manager);
                }
            }
        }

        private void OpenProgram(string progId, string displayName = null)
        {
            LogManager.Info($"프로그램 열기 요청: {progId}", "Shell");

            // ModuleLoaderService가 내부적으로 DB 확인, 다운로드, 로드를 모두 수행함
            Type type = _moduleLoader.GetProgramType(progId);
            
            if (type == null)
            {
                LogManager.Warning($"프로그램을 로드할 수 없음: {progId}", "Shell");
                XtraMessageBox.Show(
                    $"프로그램 '{progId}'을(를) 실행할 수 없습니다.\n" +
                    $"모듈이 서버에 없거나 다운로드에 실패했을 수 있습니다.", 
                    "실행 오류", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Warning);
                return;
            }

            foreach (XtraTabPage page in xtraTabControlMain.TabPages)
            {
                if (string.Equals(page.Tag as string, progId, StringComparison.OrdinalIgnoreCase))
                {
                    xtraTabControlMain.SelectedTabPage = page;
                    ActivateTabContent(page);
                    LogManager.Info($"이미 열려 있어 활성화함: {progId}", "Shell");
                    return;
                }
            }

            try
            {
                Control content = CreateProgramContent(type);
                if (content == null)
                    return;

                var tabPage = new XtraTabPage
                {
                    Text = displayName ?? progId,
                    Tag = progId
                };

                tabPage.Controls.Add(content);
                xtraTabControlMain.TabPages.Add(tabPage);
                xtraTabControlMain.SelectedTabPage = tabPage;

                _openTabs[progId] = type;

                ActivateTabContent(tabPage);

                UpdateStatusMessage($"'{displayName ?? progId}' 열림");

                LogManager.Info($"프로그램 열기 성공: {progId}", "Shell");
                LogManager.LogAction(AuditAction.Execute, "Shell", progId, $"프로그램 실행: {displayName ?? progId}");
            }
            catch (Exception ex)
            {
                LogManager.Error($"프로그램 실행 오류 {progId}: {ex.Message}", "Shell", ex);
                XtraMessageBox.Show($"프로그램 '{progId}' 실행 중 오류가 발생했습니다.\n\n{ex.Message}", "실행 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private Control CreateProgramContent(Type type)
        {
            Control content;

            // DI를 지원하기 위해 ActivatorUtilities 사용
            if (typeof(Form).IsAssignableFrom(type))
            {
                var form = (Form)Microsoft.Extensions.DependencyInjection.ActivatorUtilities.CreateInstance(_serviceProvider, type);
                form.TopLevel = false;
                form.FormBorderStyle = FormBorderStyle.None;
                form.Dock = DockStyle.Fill;
                form.Show();
                content = form;
            }
            else
            {
                content = (Control)Microsoft.Extensions.DependencyInjection.ActivatorUtilities.CreateInstance(_serviceProvider, type);

                if (content is BaseWorkControl workControl)
                {
                    workControl.EventBus = _eventAggregator;
                }

                if (content is IWorkContextProvider contextProvider)
                {
                    var context = CreateWorkContext();
                    contextProvider.InitializeContext(context);
                }

                content.Dock = DockStyle.Fill;
            }

            return content;
        }

        private nU3.Core.Context.WorkContext CreateWorkContext()
        {
            var context = new nU3.Core.Context.WorkContext();

            try
            {
                var currentUser = nU3.Core.Security.UserSession.Current;
                if (currentUser != null)
                {
                    context.CurrentUser = new UserInfoDto
                    {
                        UserId = currentUser.UserId,
                        UserName = currentUser.UserName,
                        AuthLevel = currentUser.AuthLevel
                    };

                    context.Permissions = CreatePermissionsForUser(currentUser.AuthLevel);
                }
            }
            catch (Exception ex)
            {
                LogManager.Warning($"사용자 컨텍스트 설정 실패: {ex.Message}", "Shell");
            }

            return context;
        }

        private void ActivateTabContent(XtraTabPage page)
        {
            if (page.Controls.Count > 0)
            {
                var control = page.Controls[0];

                if (control is ILifecycleAware lifecycleAware)
                {
                    lifecycleAware.OnActivated();
                }
            }
        }

        private void DeactivateTabContent(XtraTabPage page)
        {
            if (page.Controls.Count > 0)
            {
                var control = page.Controls[0];

                if (control is ILifecycleAware lifecycleAware)
                {
                    lifecycleAware.OnDeactivated();
                }
            }
        }

        private void XtraTabControlMain_CloseButtonClick(object sender, EventArgs e)
        {
            var args = e as ClosePageButtonEventArgs;
            if (args?.Page is XtraTabPage page)
            {
                CloseTab(page);
            }
        }

        private void XtraTabControlMain_SelectedPageChanged(object sender, TabPageChangedEventArgs e)
        {
            if (e.PrevPage != null)
            {
                DeactivateTabContent(e.PrevPage);
            }

            if (e.Page != null)
            {
                ActivateTabContent(e.Page);
                UpdateStatusMessage($"'{e.Page.Text}' 활성화됨");
            }
            else
            {
                // 모든 탭이 닫힌 경우 제목 초기화
                this.Text = "nU3 Healthcare Information System";
            }
        }


        private void CloseTab(XtraTabPage page)
        {
            var progId = page.Tag as string;
            var control = page.Controls.Count > 0 ? page.Controls[0] : null;

            if (control is ILifecycleAware lifecycleAware)
            {
                if (!lifecycleAware.CanClose())
                {
                    LogManager.Info($"탭 닫기 취소됨: {progId}", "Shell");
                    return;
                }
            }

            if (control is IResourceManager resourceManager)
            {
                try
                {
                    resourceManager.ReleaseResources();
                    LogManager.Info($"리소스 해제 완료: {progId}", "Shell");
                }
                catch (Exception ex)
                {
                    LogManager.Warning($"리소스 해제 중 오류: {progId} - {ex.Message}", "Shell");
                }
            }

            if (progId != null)
            {
                _openTabs.Remove(progId);
                LogManager.Info($"탭 닫힘: {progId}", "Shell");
            }

            xtraTabControlMain.TabPages.Remove(page);
            page.Dispose();
        }

        private bool ConfirmCloseAllTabs()
        {
            foreach (XtraTabPage page in xtraTabControlMain.TabPages)
            {
                if (page.Controls.Count > 0)
                {
                    var control = page.Controls[0];

                    if (control is ILifecycleAware lifecycleAware)
                    {
                        if (!lifecycleAware.CanClose())
                        {
                            xtraTabControlMain.SelectedTabPage = page;
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        private void CloseAllTabs()
        {
            if (!ConfirmCloseAllTabs())
            {
                XtraMessageBox.Show("일부 탭을 닫을 수 없습니다.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            while (xtraTabControlMain.TabPages.Count > 0)
            {
                var page = xtraTabControlMain.TabPages[0] as XtraTabPage;
                if (page != null)
                {
                    CloseTab(page);
                }
            }

            _openTabs.Clear();
            UpdateStatusMessage("모든 탭 닫힘");
        }

        private void ShowErrorReportingSettings()
        {
            var status = _errorReportingEnabled ? "활성화됨" : "비활성화됨";
            var toEmail = _emailSettings?.ToEmail ?? "설정되지 않음";

            var message = $"에러 리포팅 상태\n\n" +
                         $"상태: {status}\n" +
                         $"수신자: {toEmail}\n\n" +
                         $"설정은 appsettings.json 파일에서 변경할 수 있습니다.";

            XtraMessageBox.Show(message, "에러 리포팅 설정", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private async void TestCrashReport()
        {
            var result = XtraMessageBox.Show(
                "크래시 리포트 테스트를 실행하시겠습니까?\n\n" +
                "스크린샷이 캡처되고 에러 리포트가 이메일로 전송됩니다.\n" +
                "(실제 예외는 발생하지 않습니다)",
                "테스트 확인",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
                return;

            try
            {
                var testException = new InvalidOperationException(
                    "This is a test exception for crash reporting system validation.");

                var additionalInfo = $"Test Mode\n" +
                                   $"Active Tab: {GetActiveTabInfo()}\n" +
                                   $"Open Tabs Count: {xtraTabControlMain.TabPages.Count}";

                if (_errorReportingEnabled && _crashReporter != null)
                {
                    var success = await _crashReporter.ReportCrashAsync(testException, additionalInfo);

                    if (success)
                    {
                        XtraMessageBox.Show("테스트 크래시 리포트가 성공적으로 전송되었습니다.\n\n이메일을 확인하세요.", "테스트 성공", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        XtraMessageBox.Show("테스트 크래시 리포트 전송에 실패했습니다.\n\nappsettings.json의 이메일 설정을 확인하세요.", "테스트 실패", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    XtraMessageBox.Show("에러 리포팅이 비활성화되어 있습니다.\n\nappsettings.json에서 ErrorReporting.Enabled를 true로 설정하세요.", "비활성화됨", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"테스트 실행 중 오류가 발생했습니다.\n\n{ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ShowAboutDialog()
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            var message = $"nU3 PH Information System\n\n" +
                         $"Version: {version}\n" +
                         $"Build Date: {GetBuildDate()}\n\n" +
                         $"© 2026 nU3 Framework";

            XtraMessageBox.Show(message, "정보", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private string GetBuildDate()
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            var buildDate = new DateTime(2026, 2, 8).AddDays(version.Build).AddSeconds(version.Revision * 2);
            return buildDate.ToString("yyyy-MM-dd");
        }

        private void UpdateStatusMessage(string message)
        {
            barStaticItemLogMessage.Caption = message;
        }

        private void BarStaticItemLogMessage_ItemDoubleClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                // 현재 로그 파일 경로 가져오기
                if (LogManager.Instance.Logger is FileLogger fileLogger)
                {
                    string logPath = fileLogger.GetLogFilePath();
                    if (System.IO.File.Exists(logPath))
                    {
                        LogManager.Info($"로그 파일 열기: {logPath}", "Shell");
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = logPath,
                            UseShellExecute = true
                        });
                    }
                    else
                    {
                        XtraMessageBox.Show("현재 로그 파일이 아직 생성되지 않았거나 찾을 수 없습니다.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.Error("로그 파일 열기 실패", "Shell", ex);
                XtraMessageBox.Show($"로그 파일을 열 수 없습니다: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ShowSplashMessage(string message)
        {
            UpdateStatusMessage(message);
            Application.DoEvents();
        }

        private void HideSplashMessage()
        {
            UpdateStatusBar();
        }
    }
}
