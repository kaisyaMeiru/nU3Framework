//using nU3.Core.Context;
//using nU3.Core.Events;
//using nU3.Core.Interfaces;
//using nU3.Core.Logging;
//using nU3.Core.Repositories;
//using nU3.Core.Services;
//using nU3.Core.UI.Controls.Forms;
//using nU3.Models;
//using System;
//using System.Collections.Generic;
//using System.Windows.Forms;

//namespace nU3.Core.UI.Shell
//{
//    /// <summary>
//    /// 모듈을 호스팅하는 셸 폼의 기본 클래스입니다.
//    /// 로깅, 이벤트 구독, 모듈 로딩 등 공통 기능을 제공합니다.
//    /// 이 클래스를 상속하여 탭 기반, MDI 기반 또는 다른 셸 레이아웃을 구현합니다.
//    /// </summary>
//    public abstract class ShellFormBase : BaseWorkForm, IShellForm
//    {
//        protected readonly IMenuRepository MenuRepo;
//        protected readonly IModuleRepository ModuleRepo;
//        protected readonly IEventAggregator EventAggregator;
//        protected readonly ModuleLoaderService ModuleLoader;
//        protected readonly ShellServiceManager ServiceManager;

//        protected readonly Dictionary<string, Type> OpenModules = new Dictionary<string, Type>();
//        protected bool Initialized;

//        /// <summary>
//        /// 셸이 초기화되었는지 여부를 반환합니다.
//        /// </summary>
//        public bool IsInitialized => Initialized;

//        /// <summary>
//        /// 로깅 등에 사용되는 셸 이름을 반환합니다.
//        /// </summary>
//        public abstract string ShellName { get; }

//        /// <summary>
//        /// 열린 모듈의 개수를 반환합니다.
//        /// </summary>
//        public int OpenModuleCount => OpenModules.Count;

//        /// <summary>
//        /// 모듈이 열렸을 때 발생하는 이벤트입니다.
//        /// </summary>
//        public event EventHandler<ModuleOpenedEventArgs> ModuleOpened;

//        /// <summary>
//        /// 모듈이 닫혔을 때 발생하는 이벤트입니다.
//        /// </summary>
//        public event EventHandler<ModuleClosedEventArgs> ModuleClosed;

//        /// <summary>
//        /// ShellFormBase 생성자
//        /// </summary>
//        protected ShellFormBase(
//            IMenuRepository menuRepo,
//            IModuleRepository moduleRepo,
//            IEventAggregator eventAggregator)
//        {
//            MenuRepo = menuRepo ?? throw new ArgumentNullException(nameof(menuRepo));
//            ModuleRepo = moduleRepo ?? throw new ArgumentNullException(nameof(moduleRepo));
//            EventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));

//            ModuleLoader = new ModuleLoaderService(moduleRepo);
//            ServiceManager = new ShellServiceManager(this, ShellName);

//            this.Load += ShellFormBase_Load;
//            this.FormClosing += ShellFormBase_FormClosing;
//        }

//        /// <summary>
//        /// 셸 서비스 및 구성요소를 초기화합니다.
//        /// 커스텀 초기화가 필요하면 재정의하되 base.InitializeShell()을 먼저 호출하세요.
//        /// </summary>
//        protected virtual void InitializeShell()
//        {
//            ServiceManager.Initialize();
//            ServiceManager.ServerConnectionStatusChanged += OnServerConnectionStatusChanged;

//            SubscribeToEvents();

//            LogManager.Info($"{ShellName} initialized", ShellName);
//        }

//        /// <summary>
//        /// 서버 연결 상태 변경을 처리합니다. (파생 클래스에서 상태 표시줄 업데이트 등 수행)
//        /// </summary>
//        protected virtual void OnServerConnectionStatusChanged(object sender, ServerConnectionStatusEventArgs e)
//        {
//            // 파생 클래스에서 상태 표시줄 등을 업데이트하도록 재정의
//        }

//        /// <summary>
//        /// 프레임워크 이벤트를 구독합니다.
//        /// </summary>
//        protected virtual void SubscribeToEvents()
//        {
//            EventAggregator?.GetEvent<NavigationRequestEvent>()
//                .Subscribe(OnNavigationRequest);

//            EventAggregator?.GetEvent<CloseScreenRequestEvent>()
//                .Subscribe(OnCloseScreenRequest);

//            LogManager.Info($"{ShellName} subscribed to events", ShellName);
//        }

//        /// <summary>
//        /// 네비게이션 요청 이벤트 처리기
//        /// </summary>
//        protected virtual void OnNavigationRequest(object payload)
//        {
//            if (payload is not NavigationRequestEventPayload evt)
//                return;

//            try
//            {
//                LogManager.Info($"Navigation requested to {evt.TargetScreenId} from {evt.Source}", ShellName);
//                OpenProgram(evt.TargetScreenId);

//                if (evt.Context != null)
//                {
//                    UpdateModuleContext(evt.TargetScreenId, evt.Context);
//                }
//            }
//            catch (Exception ex)
//            {
//                LogManager.Error($"Navigation request failed: {ex.Message}", ShellName, ex);
//            }
//        }

//        /// <summary>
//        /// 화면 닫기 요청 이벤트 처리기
//        /// </summary>
//        protected virtual void OnCloseScreenRequest(object payload)
//        {
//            if (payload is not CloseScreenRequestEventPayload evt)
//                return;

//            try
//            {
//                CloseProgram(evt.ScreenId, evt.Force);
//            }
//            catch (Exception ex)
//            {
//                LogManager.Error($"Close screen request failed: {ex.Message}", ShellName, ex);
//            }
//        }

//        /// <summary>
//        /// 모든 모듈을 로드합니다.
//        /// </summary>
//        protected virtual void LoadModules()
//        {
//            ModuleLoader.LoadAllModules();
//            LogManager.Info("Modules loaded", ShellName);
//        }

//        /// <summary>
//        /// ProgId로 프로그램/모듈을 엽니다. 파생 클래스에서 UI 레이아웃에 맞게 구현해야 합니다.
//        /// </summary>
//        public abstract void OpenProgram(string progId, string displayName = null);

//        /// <summary>
//        /// ProgId로 프로그램/모듈을 닫습니다. 파생 클래스에서 UI 레이아웃에 맞게 구현해야 합니다.
//        /// </summary>
//        public abstract void CloseProgram(string progId, bool force = false);

//        /// <summary>
//        /// 모듈 컨텍스트를 업데이트합니다. 파생 클래스에서 구현해야 합니다.
//        /// </summary>
//        protected abstract void UpdateModuleContext(string progId, WorkContext context);

//        /// <summary>
//        /// 활성 모듈에 대한 정보를 가져옵니다. 파생 클래스에서 구현해야 합니다.
//        /// </summary>
//        public abstract string GetActiveModuleInfo();

//        /// <summary>
//        /// 상태 메시지를 업데이트합니다. 파생 클래스에서 구현해야 합니다.
//        /// </summary>
//        public abstract void UpdateStatusMessage(string message);

//        /// <summary>
//        /// ProgId에 해당하는 Type을 가져오며 버전 체크 및 자동 업데이트를 수행합니다.
//        /// </summary>
//        protected Type GetProgramType(string progId)
//        {
//            var attr = ModuleLoader.GetProgramAttribute(progId);
//            if (attr != null)
//            {
//                var moduleId = attr.GetModuleId();
//                try
//                {
//                    if (!ModuleLoader.EnsureModuleUpdated(progId, moduleId))
//                    {
//                        LogManager.Warning($"Failed to update module for: {progId}", ShellName);
//                    }
//                }
//                catch (Exception ex)
//                {
//                    LogManager.Warning($"Error checking module version: {ex.Message}", ShellName);
//                }
//            }

//            return ModuleLoader.GetProgramType(progId);
//        }

//        /// <summary>
//        /// Type으로부터 모듈 컨텐츠(Control)를 생성합니다.
//        /// 폼이면 비-TopLevel로 변경하여 컨테이너에 추가합니다.
//        /// </summary>
//        protected Control CreateModuleContent(Type type)
//        {
//            Control content;

//            if (typeof(Form).IsAssignableFrom(type))
//            {
//                var form = (Form)Activator.CreateInstance(type);
//                form.TopLevel = false;
//                form.FormBorderStyle = FormBorderStyle.None;
//                form.Dock = DockStyle.Fill;
//                form.Show();
//                content = form;
//            }
//            else
//            {
//                content = (Control)Activator.CreateInstance(type);

//                if (content is BaseWorkControl workControl)
//                {
//                    workControl.EventBus = EventAggregator;
//                }

//                if (content is IWorkContextProvider contextProvider)
//                {
//                    var context = CreateWorkContext();
//                    contextProvider.InitializeContext(context);
//                }

//                content.Dock = DockStyle.Fill;
//            }

//            return content;
//        }

//        /// <summary>
//        /// 현재 사용자에 대한 WorkContext를 생성합니다.
//        /// </summary>
//        protected WorkContext CreateWorkContext()
//        {
//            var context = new WorkContext();

//            try
//            {
//                var currentUser = Security.UserSession.Current;
//                if (currentUser != null)
//                {
//                    context.CurrentUser = new UserInfoDto
//                    {
//                        UserId = currentUser.UserId,
//                        UserName = currentUser.UserName,
//                        AuthLevel = currentUser.AuthLevel
//                    };

//                    context.Permissions = CreatePermissionsForUser(currentUser.AuthLevel);
//                }
//            }
//            catch (Exception ex)
//            {
//                LogManager.Warning($"Failed to set user context: {ex.Message}", ShellName);
//            }

//            return context;
//        }

//        /// <summary>
//        /// 사용자 권한 레벨에 기반한 모듈 권한을 생성합니다.
//        /// </summary>
//        protected virtual ModulePermissions CreatePermissionsForUser(int authLevel)
//        {
//            var permissions = new ModulePermissions();

//            if (authLevel == 0)
//            {
//                permissions.GrantAll();
//            }
//            else if (authLevel >= 1 && authLevel <= 9)
//            {
//                permissions.CanRead = true;
//                permissions.CanCreate = true;
//                permissions.CanUpdate = true;
//                permissions.CanPrint = true;
//                permissions.CanExport = true;
//            }
//            else if (authLevel == 10)
//            {
//                permissions.CanRead = true;
//                permissions.CanCreate = true;
//                permissions.CanUpdate = true;
//                permissions.CanDelete = true;
//                permissions.CanPrint = true;
//                permissions.CanApprove = true;
//            }
//            else if (authLevel == 11)
//            {
//                permissions.CanRead = true;
//                permissions.CanCreate = true;
//                permissions.CanUpdate = true;
//                permissions.CanPrint = true;
//            }
//            else
//            {
//                permissions.CanRead = true;
//            }

//            return permissions;
//        }

//        /// <summary>
//        /// 열린 모든 모듈에 컨텍스트 변경을 브로드캐스트합니다.
//        /// </summary>
//        public virtual void BroadcastContextChange(WorkContext context)
//        {
//            try
//            {
//                EventAggregator?.GetEvent<WorkContextChangedEvent>()
//                    .Publish(new WorkContextChangedEventPayload
//                    {
//                        NewContext = context,
//                        Source = ShellName
//                    });

//                LogManager.Info("Context change broadcasted to all modules", ShellName);
//            }
//            catch (Exception ex)
//            {
//                LogManager.Error($"Error broadcasting context change: {ex.Message}", ShellName, ex);
//            }
//        }

//        /// <summary>
//        /// 환자를 선택하고 열린 모듈에 브로드캐스트합니다.
//        /// </summary>
//        public virtual void SelectPatient(PatientInfoDto patient)
//        {
//            if (patient == null)
//                return;

//            try
//            {
//                EventAggregator?.GetEvent<PatientSelectedEvent>()
//                    .Publish(new PatientSelectedEventPayload
//                    {
//                        Patient = patient,
//                        Source = ShellName
//                    });

//                var context = CreateWorkContext();
//                context.CurrentPatient = patient;
//                BroadcastContextChange(context);

//                LogManager.Info($"Patient selected and broadcasted: {patient.PatientName} ({patient.PatientId})", ShellName);
//            }
//            catch (Exception ex)
//            {
//                LogManager.Error($"Error selecting patient: {ex.Message}", ShellName, ex);
//            }
//        }

//        /// <summary>
//        /// 검사를 선택하고 열린 모듈에 브로드캐스트합니다.
//        /// </summary>
//        public virtual void SelectExam(ExamOrderDto exam, PatientInfoDto patient)
//        {
//            if (exam == null)
//                return;

//            try
//            {
//                EventAggregator?.GetEvent<ExamSelectedEvent>()
//                    .Publish(new ExamSelectedEventPayload
//                    {
//                        Exam = exam,
//                        Patient = patient,
//                        Source = ShellName
//                    });

//                var context = CreateWorkContext();
//                context.CurrentPatient = patient;
//                context.CurrentExam = exam;
//                BroadcastContextChange(context);

//                LogManager.Info($"Exam selected and broadcasted: {exam.ExamOrderId}", ShellName);
//            }
//            catch (Exception ex)
//            {
//                LogManager.Error($"Error selecting exam: {ex.Message}", ShellName, ex);
//            }
//        }

//        /// <summary>
//        /// 모듈이 열렸을 때 ModuleOpened 이벤트를 발생시킵니다.
//        /// </summary>
//        protected void OnModuleOpened(string progId, string displayName)
//        {
//            ModuleOpened?.Invoke(this, new ModuleOpenedEventArgs
//            {
//                ProgId = progId,
//                DisplayName = displayName
//            });
//        }

//        /// <summary>
//        /// 모듈이 닫혔을 때 ModuleClosed 이벤트를 발생시킵니다.
//        /// </summary>
//        protected void OnModuleClosed(string progId)
//        {
//            ModuleClosed?.Invoke(this, new ModuleClosedEventArgs
//            {
//                ProgId = progId
//            });
//        }

//        /// <summary>
//        /// 셸이 로드될 때 호출됩니다. 기본 로직을 사용하려면 재정의 시 base.OnShellLoad()를 호출하세요.
//        /// </summary>
//        protected virtual void OnShellLoad()
//        {
//            if (Initialized)
//                return;

//            Initialized = true;

//            LogManager.Info($"{ShellName} loading", ShellName);

//            LoadModules();
//            BuildMenu();

//            LogManager.Info($"{ShellName} loaded successfully", ShellName);
//            LogManager.LogAction(AuditAction.Login, ShellName, this.GetType().Name, $"User logged in at {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
//        }

//        /// <summary>
//        /// 메뉴 구조를 구성합니다. 파생 클래스에서 구현해야 합니다.
//        /// </summary>
//        protected abstract void BuildMenu();

//        /// <summary>
//        /// 셸이 닫힐 때 호출됩니다. 기본 동작을 원하면 재정의 시 base.OnShellClosing(e)를 호출하세요.
//        /// </summary>
//        protected virtual void OnShellClosing(FormClosingEventArgs e)
//        {
//            if (e.CloseReason == CloseReason.UserClosing)
//            {
//                if (!ConfirmCloseAllModules())
//                {
//                    e.Cancel = true;
//                    return;
//                }

//                var result = MessageBox.Show(
//                    "프로그램을 종료하시겠습니까?",
//                    "종료 확인",
//                    MessageBoxButtons.YesNo,
//                    MessageBoxIcon.Question);

//                if (result != DialogResult.Yes)
//                {
//                    e.Cancel = true;
//                    return;
//                }

//                LogManager.LogAction(AuditAction.Logout, ShellName, this.GetType().Name, $"User logged out at {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
//                LogManager.Info("Application closing", ShellName);
//            }

//            if (!e.Cancel)
//            {
//                ServiceManager.Shutdown();
//            }
//        }

//        /// <summary>
//        /// 모든 모듈을 닫을 것인지 확인합니다. 파생 클래스에서 구현해야 합니다.
//        /// </summary>
//        protected abstract bool ConfirmCloseAllModules();

//        private void ShellFormBase_Load(object sender, EventArgs e)
//        {
//            InitializeShell();
//            OnShellLoad();
//        }

//        private void ShellFormBase_FormClosing(object sender, FormClosingEventArgs e)
//        {
//            OnShellClosing(e);
//        }

//        protected override void Dispose(bool disposing)
//        {
//            if (disposing)
//            {
//                ServiceManager?.Dispose();
//            }
//            base.Dispose(disposing);
//        }
//    }
//}
