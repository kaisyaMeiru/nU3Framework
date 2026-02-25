using DevExpress.XtraEditors;
using nU3.Core.Attributes;
using nU3.Core.Context;
using nU3.Core.Events;
using nU3.Core.Helpers;
using nU3.Core.UI.Helpers;
using nU3.Core.Interfaces;
using nU3.Core.Services;
using nU3.Core.UI.Controls;
using nU3.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraBars.Docking;
using System.Linq;
using nU3.Core.Logging;
using System.Xml.Linq;
using System.Text;

namespace nU3.Core.UI
{
    /// <summary>
    /// nU3 Framework의 모든 작업 화면(UserControl)을 위한 기본 클래스입니다.
    /// 이 클래스는 다음 사항들을 담당합니다:
    /// - 화면 생명주기 관리(활성화/비활성화, 리소스 해제)
    /// - 작업 컨텍스트(사용자, 권한 등) 주입 및 업데이트
    /// - 통합 레이아웃 번들(Persistence) 관리 (Dock, Bar, Ribbon, Grid, Splitter, MainLayout)
    /// - 공통 UI 헬퍼 및 로깅 유틸리티
    /// 화면을 개발할 때 이 클래스를 상속받아 필요한 훅(메서드)을 오버라이드하면 됩니다.
    /// </summary>
    public class BaseWorkControl : XtraUserControl, IBaseWorkControl, IBaseWorkControlExpand, IDisposable
    {
        // 내부 상태 필드
        private WorkContext _workContext;
        private bool _isActivated;
        private bool _isDisposed;
        private readonly List<IDisposable> _disposables;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly object _lock = new object();
        private string? _programId;
        private string? _programTitle;

        // 레이아웃 저장 기본 경로 (%AppData%\nU3\Layouts)
        private static readonly string LayoutBaseDir =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "nU3", "Layouts");

        private const string LayoutVersion = "1.0";

        #region 화면 식별자(Program ID) 구현

        /// <summary>
        /// 화면의 고유 식별자(Program ID)를 가져옵니다.
        /// <para>클래스에 지정된 nU3ProgramInfoAttribute가 있으면 그 값을 사용하고, 없으면 타입 이름을 사용합니다.</para>
        /// </summary>
        public virtual string ProgramID
        {
            get
            {
                if (_programId == null)
                {
                    var attr = this.GetType().GetCustomAttribute<nU3ProgramInfoAttribute>();
                    _programId = attr?.ProgramId ?? this.GetType().Name;
                }

                return _programId;
            }

            protected set
            {
                _programId = value;
            }
        }

        /// <summary>
        /// 화면의 표시 제목(Program Title)을 가져옵니다.
        /// <para>클래스에 지정된 nU3ProgramInfoAttribute가 있으면 그 값을 사용하고, 없으면 타입 이름을 사용합니다.</para>
        /// </summary>
        public virtual string ProgramTitle
        {
            get
            {
                if (_programTitle == null)
                {
                    var attr = this.GetType().GetCustomAttribute<nU3ProgramInfoAttribute>();
                    _programTitle = attr?.ProgramName ?? this.GetType().Name;
                }

                return _programTitle;
            }

            protected set
            {
                _programTitle = value;
            }
        }

        #endregion

        #region 속성 및 생성자

        /// <summary>
        /// 전역 이벤트 버스입니다. 화면 간 통신, 이벤트 발행/구독에 사용됩니다.
        /// 프레임워크가 인스턴스를 주입합니다.
        /// </summary>
        public nU3.Core.Events.IEventAggregator EventBus { get; set; }

        /// <summary>
        /// 현재 화면의 작업 컨텍스트(사용자, 환자 정보, 권한 등)를 반환합니다.
        /// </summary>
        public WorkContext Context
        {
            get
            {
                return _workContext;
            }
        }

        /// <summary>
        /// 연결성(Connectivity) 매니저 인스턴스입니다. 디자인타임에는 null을 반환합니다.
        /// </summary>
        protected ConnectivityManager? Connectivity
        {
            get
            {
                if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime)
                {
                    return null;
                }

                return ConnectivityManager.Instance;
            }
        }

        /// <summary>
        /// 표준 로그 기록을 위한 로거입니다. 디자인타임에는 null을 반환합니다.
        /// </summary>
        protected ILogger? Logger
        {
            get
            {
                if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime)
                {
                    return null;
                }

                return LogManager.Instance.Logger;
            }
        }

        /// <summary>
        /// 감사 로그 전용 로거입니다. 디자인타임에는 null을 반환합니다.
        /// </summary>
        protected IAuditLogger? AuditLogger
        {
            get
            {
                if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime)
                {
                    return null;
                }

                return LogManager.Instance.AuditLogger;
            }
        }

        /// <summary>
        /// 이 화면 인스턴스에서 사용하는 비동기 취소 토큰입니다.
        /// </summary>
        protected CancellationToken CancellationToken
        {
            get
            {
                return _cancellationTokenSource.Token;
            }
        }

        /// <summary>
        /// 현재 화면의 활성화 여부입니다.
        /// </summary>
        public bool IsActivated
        {
            get
            {
                return _isActivated;
            }
        }

        /// <summary>
        /// 메인 레이아웃 컨트롤입니다. (프레임워크 스캐폴딩에 의해 기본 생성될 수 있습니다)
        /// 화면에서 레이아웃을 구성할 때 이 컨트롤을 사용하세요.
        /// </summary>
        public nU3LayoutControl MainLayoutControl;

        /// <summary>
        /// 레이아웃 컨트롤의 루트 그룹입니다.
        /// </summary>
        public DevExpress.XtraLayout.LayoutControlGroup Root;

        /// <summary>
        /// 기본 생성자입니다.
        /// <para>디자인타임 환경에서는 초기화 로직을 실행하지 않습니다.</para>
        /// </summary>
        public BaseWorkControl()
        {
            if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime)
            {
                return;
            }

            // 폼/컨트롤 초기화 지점(필요한 경우 오버라이드하여 레이아웃을 구성)
            LayoutInit();

            this.Dock = DockStyle.None;

            _disposables = new List<IDisposable>();
            _cancellationTokenSource = new CancellationTokenSource();
            _workContext = new WorkContext();
        }

        // 과거에 사용하던 InitializeComponent 샘플 구현이 주석으로 남아있습니다.
        // LayoutInit 메서드를 오버라이드하거나 이 클래스의 파생에서 컨트롤을 초기화하세요.
        private void LayoutInit()
        {
            //MainLayoutControl = new nU3LayoutControl();
            //Root = new DevExpress.XtraLayout.LayoutControlGroup();
            //((System.ComponentModel.ISupportInitialize)MainLayoutControl).BeginInit();
            //((System.ComponentModel.ISupportInitialize)Root).BeginInit();
            //SuspendLayout();
            //// 
            //// MainLayoutControl
            //// 
            //MainLayoutControl.Dock = DockStyle.Fill;
            //MainLayoutControl.Location = new System.Drawing.Point(0, 0);
            //MainLayoutControl.Name = "MainLayoutControl";
            //MainLayoutControl.Root = Root;
            //MainLayoutControl.Size = new System.Drawing.Size(708, 500);
            //MainLayoutControl.TabIndex = 0;
            //MainLayoutControl.Text = "nU3LayoutControl1";
            //// 
            //// Root
            //// 
            //Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            //Root.GroupBordersVisible = false;
            //Root.Name = "Root";
            //Root.Size = new System.Drawing.Size(708, 500);
            //Root.TextVisible = false;
            //// 
            //// BaseWorkControl
            //// 
            //Controls.Add(MainLayoutControl);
            //Name = "BaseWorkControl";
            //Size = new System.Drawing.Size(708, 500);
            //((System.ComponentModel.ISupportInitialize)MainLayoutControl).EndInit();
            //((System.ComponentModel.ISupportInitialize)Root).EndInit();
            //ResumeLayout(false);
        }

        #endregion

        #region 컨텍스트 관리 (IWorkContextProvider)

        /// <summary>
        /// 화면 로딩 시 프레임워크에 의해 외부에서 컨텍스트가 주입됩니다.
        /// <para>주입 시 기존 컨텍스트의 복사본을 보존하고, OnContextInitialized 훅을 호출합니다.</para>
        /// </summary>
        /// <param name="context">주입될 <see cref="WorkContext"/> 인스턴스</param>
        public virtual void InitializeContext(WorkContext context)
        {
            if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime)
            {
                return;
            }

            if (context == null)
            {
                return;
            }

            var oldContext = _workContext;
            _workContext = context.Clone();

            OnContextInitialized(oldContext, _workContext);

            LogInfo($"컨텍스트 초기화: 사용자={_workContext.CurrentUser?.UserId}");

            // 컨텍스트 변경 후 저장된 레이아웃을 복원합니다.
            LoadLayout();
        }

        /// <summary>
        /// 런타임 중 글로벌 컨텍스트가 변경될 때 호출됩니다. 주로 사용자 변경, 권한 변경 등 발생 시 사용됩니다.
        /// </summary>
        /// <param name="context">새로운 <see cref="WorkContext"/> 인스턴스</param>
        public virtual void UpdateContext(WorkContext context)
        {
            if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime)
            {
                return;
            }

            if (context == null)
            {
                return;
            }

            var oldContext = _workContext;
            _workContext = context.Clone();

            OnContextChanged(oldContext, _workContext);

            LogInfo("컨텍스트 업데이트 완료");
        }

        /// <summary>
        /// 현재 컨텍스트의 복사본을 반환합니다. 디자인타임일 경우 새 <see cref="WorkContext"/>를 반환합니다.
        /// </summary>
        /// <returns>복사된 <see cref="WorkContext"/> 인스턴스</returns>
        public WorkContext GetContext()
        {
            if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime)
            {
                return new WorkContext();
            }

            return _workContext?.Clone() ?? new WorkContext();
        }

        /// <summary>
        /// 컨텍스트 초기화 후 호출되는 훅입니다. 화면에서 필요한 추가 초기화를 구현하세요.
        /// </summary>
        /// <remarks>기본 구현은 아무 동작도 하지 않습니다.</remarks>
        /// <param name="old">이전 컨텍스트(복사본)</param>
        /// <param name="new">새 컨텍스트(복사본)</param>
        protected virtual void OnContextInitialized(WorkContext? old, WorkContext @new)
        {
        }

        /// <summary>
        /// 런타임 중 컨텍스트 변경 시 호출되는 훅입니다.
        /// </summary>
        /// <param name="old">이전 컨텍스트(복사본)</param>
        /// <param name="new">새 컨텍스트(복사본)</param>
        protected virtual void OnContextChanged(WorkContext? old, WorkContext @new)
        {
        }

        #endregion

        #region 생명주기 및 리소스 관리

        /// <summary>
        /// 화면이 활성화될 때 프레임워크에서 호출합니다.
        /// 이 메서드는 내부적으로 <see cref="OnScreenActivated"/>를 호출하고 모듈 활성화 이벤트를 발행합니다.
        /// </summary>
        public virtual void OnActivated()
        {
            if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime)
            {
                return;
            }

            if (_isActivated)
            {
                return;
            }

            _isActivated = true;

            try
            {
                OnScreenActivated();

                var attr = this.GetType().GetCustomAttribute<nU3ProgramInfoAttribute>();

                var eventPayload = new ModuleActivatedEventPayload
                {
                    ProgId = this.ProgramID,
                    ProgramName = this.ProgramTitle,
                    ModuleId = attr?.GetModuleId(),
                    Version = this.GetType().Assembly.GetName().Version?.ToString()
                };

                EventBus?.GetEvent<ModuleActivatedEvent>().Publish(eventPayload);
            }
            catch (Exception ex)
            {
                LogError("활성화 오류", ex);
            }
        }

        /// <summary>
        /// 화면이 비활성화될 때 프레임워크에서 호출합니다.
        /// 기본 동작은 OnScreenDeactivated 훅을 호출하고 레이아웃을 저장하는 것입니다.
        /// </summary>
        public virtual void OnDeactivated()
        {
            if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime)
            {
                return;
            }

            if (!_isActivated)
            {
                return;
            }

            _isActivated = false;

            try
            {
                OnScreenDeactivated();
                SaveLayout();
            }
            catch (Exception ex)
            {
                LogError("비활성화 오류", ex);
            }
        }

        /// <summary>
        /// 화면이 닫혀도 되는지 여부를 반환합니다.
        /// 디자인타임이면 true를 반환하며, 기본적으로 <see cref="OnBeforeClose"/>의 값을 반환합니다.
        /// </summary>
        /// <returns>닫을 수 있으면 true</returns>
        public virtual bool CanClose()
        {
            if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime)
            {
                return true;
            }

            return OnBeforeClose();
        }

        /// <summary>
        /// 화면이 활성화될 때 호출되는 훅입니다. 화면에서 필요한 초기화 동작을 구현하세요.
        /// </summary>
        protected virtual void OnScreenActivated()
        {
        }

        /// <summary>
        /// 화면이 비활성화될 때 호출되는 훅입니다. 화면에서 필요한 정리 동작을 구현하세요.
        /// </summary>
        protected virtual void OnScreenDeactivated()
        {
        }

        /// <summary>
        /// 화면을 닫기 전에 호출되는 훅입니다. 닫아도 되는지 여부를 반환합니다.
        /// 기본 구현은 true를 반환합니다.
        /// </summary>
        /// <returns>닫아도 되면 true</returns>
        protected virtual bool OnBeforeClose()
        {
            return true;
        }

        /// <summary>
        /// 화면에서 사용한 리소스를 해제합니다. (구독 해제, IDisposable 객체 Dispose 등)
        /// 프레임워크가 화면 종료 시 호출합니다.
        /// </summary>
        public virtual void ReleaseResources()
        {
            if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime)
            {
                return;
            }

            if (_isDisposed)
            {
                return;
            }

            try
            {
                _cancellationTokenSource?.Cancel();

                // 사용자 레이아웃 및 컴포넌트 상태를 저장합니다.
                SaveLayout();

                // 화면별 추가 리소스 해제 로직
                OnReleaseResources();

                // 등록된 IDisposable 항목들을 안전히 해제합니다.
                lock (_lock)
                {
                    foreach (var d in _disposables)
                    {
                        try
                        {
                            d?.Dispose();
                        }
                        catch
                        {
                            // Dispose 중 예외는 무시합니다. (로깅만으로 충분)
                        }
                    }

                    _disposables.Clear();
                }

                LogInfo($"리소스 해제 완료: {ProgramTitle}");
            }
            catch (Exception ex)
            {
                LogError("리소스 해제 중 오류 발생", ex);
            }
        }

        /// <summary>
        /// 파생 클래스에서 리소스 해제 시 추가로 처리할 로직을 구현하세요.
        /// </summary>
        protected virtual void OnReleaseResources()
        {
        }

        /// <summary>
        /// IDisposable 객체를 등록하여 <see cref="ReleaseResources"/> 호출 시 자동으로 Dispose 되도록 합니다.
        /// </summary>
        /// <param name="disposable">등록할 IDisposable 객체</param>
        protected void RegisterDisposable(IDisposable disposable)
        {
            if (disposable == null)
            {
                return;
            }

            lock (_lock)
            {
                if (!_disposables.Contains(disposable))
                {
                    _disposables.Add(disposable);
                }
            }
        }

        /// <summary>
        /// 이전에 등록한 IDisposable 객체를 목록에서 제거합니다.
        /// </summary>
        /// <param name="disposable">제거할 IDisposable 객체</param>
        protected void UnregisterDisposable(IDisposable disposable)
        {
            if (disposable == null)
            {
                return;
            }

            lock (_lock)
            {
                _disposables.Remove(disposable);
            }
        }

        /// <summary>
        /// 화면의 데이터를 비동기로 갱신합니다. 기본 구현은 아무 동작도 하지 않습니다.
        /// </summary>
        /// <returns>완료된 Task</returns>
        public virtual async Task RefreshDataAsync()
        {
            await Task.CompletedTask;
        }

        /// <summary>
        /// 이벤트 버스에 특정 이벤트 타입을 구독합니다.
        /// </summary>
        /// <typeparam name="TEvent">구독할 이벤트 타입 (PubSubEvent 계열)</typeparam>
        /// <typeparam name="TPayload">이벤트 페이로드 타입</typeparam>
        /// <param name="action">이벤트 수신 시 처리할 액션</param>
        protected void SubscribeEvent<TEvent, TPayload>(Action<TPayload> action)
            where TEvent : nU3.Core.Events.PubSubEvent<TPayload>, new()
        {
            if (EventBus == null)
            {
                return;
            }

            EventBus.GetEvent<TEvent>().Subscribe(action);
        }

        #endregion

        #region UI 및 로그 헬퍼 (전량 복구)

        /// <summary>
        /// Invoke/BeginInvoke를 안전하게 처리합니다. UI 스레드에서 실행되어야 하는 동작을 안전히 실행할 때 사용합니다.
        /// </summary>
        /// <param name="action">UI 스레드에서 실행할 액션</param>
        protected void SafeInvoke(Action action)
        {
            nU3.Core.UI.Helpers.UIHelper.SafeInvoke(this, action);
        }

        /// <summary>
        /// 사용자에게 정보 메시지를 표시합니다. (MessageBox 스타일)
        /// </summary>
        /// <param name="message">표시할 메시지</param>
        /// <param name="title">창 제목. 기본값은 "정보"</param>
        protected void ShowInfo(string message, string title = "정보")
        {
            this.SafeInvoke(() =>
            {
                XtraMessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
            });
        }

        /// <summary>
        /// 사용자에게 경고 메시지를 표시합니다.
        /// </summary>
        /// <param name="message">표시할 메시지</param>
        /// <param name="title">창 제목. 기본값은 "경고"</param>
        protected void ShowWarning(string message, string title = "경고")
        {
            this.SafeInvoke(() =>
            {
                XtraMessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            });
        }

        /// <summary>
        /// 사용자에게 오류 메시지를 표시하고 필요시 예외를 로그합니다.
        /// </summary>
        /// <param name="message">표시할 메시지</param>
        /// <param name="title">창 제목. 기본값은 "오류"</param>
        /// <param name="ex">추가로 로깅할 예외. null 허용</param>
        protected void ShowError(string message, string title = "오류", Exception? ex = null)
        {
            this.SafeInvoke(() =>
            {
                XtraMessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);

                if (ex != null)
                {
                    LogError(message, ex);
                }
            });
        }

        /// <summary>
        /// 예/아니오 확인 대화상자를 띄웁니다.
        /// </summary>
        /// <param name="message">질문 텍스트</param>
        /// <param name="title">대화상자 제목</param>
        /// <returns>사용자가 선택한 DialogResult</returns>
        protected DialogResult ShowQuestion(string message, string title = "확인")
        {
            return XtraMessageBox.Show(message, title, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        }

        /// <summary>
        /// 정보 로그를 남깁니다. 로그 카테고리는 화면의 ProgramID를 사용합니다.
        /// </summary>
        /// <param name="message">로그 메시지</param>
        protected void LogInfo(string message)
        {
            LogManager.Info(message, ProgramID ?? this.GetType().Name);
        }

        /// <summary>
        /// 경고 로그를 남깁니다. 로그 카테고리는 화면의 ProgramID를 사용합니다.
        /// </summary>
        /// <param name="message">로그 메시지</param>
        protected void LogWarning(string message)
        {
            LogManager.Warning(message, ProgramID ?? this.GetType().Name);
        }

        /// <summary>
        /// 오류 로그를 남깁니다. 예외 정보를 함께 기록할 수 있습니다.
        /// </summary>
        /// <param name="message">로그 메시지</param>
        /// <param name="exception">관련 예외. null 허용</param>
        protected void LogError(string message, Exception? exception = null)
        {
            LogManager.Error(message, ProgramID ?? this.GetType().Name, exception);
        }

        /// <summary>
        /// 감사 로그를 기록합니다.
        /// </summary>
        /// <param name="action">실행된 액션 이름</param>
        /// <param name="entityType">엔티티 타입(선택)</param>
        /// <param name="entityId">엔티티 식별자(선택)</param>
        /// <param name="additionalInfo">추가 정보(선택)</param>
        protected void LogAudit(string action, string? entityType = null, string? entityId = null, string? additionalInfo = null)
        {
            LogManager.LogAction(action, entityType ?? this.GetType().Namespace, entityId ?? ProgramID ?? this.GetType().Name, additionalInfo);
        }

        /// <summary>
        /// 조회(Read) 로그를 기록합니다.
        /// </summary>
        /// <param name="entityType">엔티티 타입 또는 모듈</param>
        /// <param name="entityId">엔티티 식별자 또는 화면</param>
        /// <param name="module">모듈 이름(선택)</param>
        /// <param name="screen">화면 이름(선택)</param>
        protected void LogRead(string entityType, string entityId, string? module = null, string? screen = null)
        {
            LogManager.LogAction("Read", module ?? entityType, screen ?? entityId, $"조회: {entityId}");
        }

        /// <summary>
        /// 현재 컨텍스트의 권한을 검사하는 헬퍼입니다.
        /// </summary>
        /// <param name="check">권한 검사 델리게이트</param>
        /// <returns>권한 검사 결과</returns>
        protected bool HasPermission(Func<ModulePermissions, bool> check)
        {
            return _workContext?.Permissions != null && check(_workContext.Permissions);
        }

        /// <summary>읽기 권한 여부</summary>
        protected bool CanRead => HasPermission(p => p.CanRead);

        /// <summary>생성 권한 여부</summary>
        protected bool CanCreate => HasPermission(p => p.CanCreate);

        /// <summary>수정 권한 여부</summary>
        protected bool CanUpdate => HasPermission(p => p.CanUpdate);

        /// <summary>삭제 권한 여부</summary>
        protected bool CanDelete => HasPermission(p => p.CanDelete);

        /// <summary>인쇄 권한 여부</summary>
        protected bool CanPrint => HasPermission(p => p.CanPrint);

        /// <summary>내보내기 권한 여부</summary>
        protected bool CanExport => HasPermission(p => p.CanExport);

        #endregion

        #region 통합 레이아웃 번들 관리 (Persistence)

        /// <summary>
        /// 화면 및 관련 컴포넌트의 레이아웃을 저장합니다.
        /// 저장 대상: DockManager, BarManager, Ribbon, MainLayout, Grid (각 Grid의 MainView 레이아웃), Splitter 위치 등
        /// 저장 위치는 사용자의 AppData 폴더 아래 nU3\Layouts\{UserId}\ 입니다.
        /// </summary>
        public virtual void SaveLayout()
        {
            if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime)
            {
                return;
            }

            try
            {
                var bundlePath = GetLayoutFilePath("bundle");
                var dir = Path.GetDirectoryName(bundlePath);

                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                var root = new XElement("nU3LayoutBundle", 
                    new XAttribute("ProgramID", ProgramID),
                    new XAttribute("Version", LayoutVersion),
                    new XAttribute("SavedAt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));

                // 1. 주요 컴포넌트 레이아웃 저장 (Dock, Bars, Ribbon)
                AppendAllComponentsToBundle<nU3DockManager>(root, "Dock");
                AppendAllComponentsToBundle<nU3BarManager>(root, "Bars");
                AppendAllComponentsToBundle<nU3RibbonControl>(root, "Ribbon");

                // 2. 메인 레이아웃 컨트롤 저장
                if (MainLayoutControl != null)
                {
                    SaveControlLayout(root, MainLayoutControl, "MainLayout");
                }

                // 3. 화면 내 모든 Grid 및 TreeList, PivotGrid 등 저장
                SaveGenericControls(root);

                // 4. Splitter 위치 저장
                SaveSplitterPositions(root);

                root.Save(bundlePath);

                LogInfo($"통합 레이아웃 저장 성공: {ProgramID}");
            }
            catch (Exception ex)
            {
                LogError("레이아웃 저장 실패", ex);
            }
        }

        private void SaveGenericControls(XElement root)
        {
            // 1. 모든 GridControl 탐색 (nU3GridControl 포함 DX 표준 GridControl 전체)
            var grids = nU3.Core.UI.Helpers.UIHelper.FindAllControls<DevExpress.XtraGrid.GridControl>(this);
            foreach (var grid in grids)
            {
                if (grid.MainView != null)
                {
                    SaveControlLayout(root, grid.MainView, "Grid", GetControlPath(grid));
                }
            }

            // 2. TreeList 및 PivotGrid 탐색
            var treeLists = nU3.Core.UI.Helpers.UIHelper.FindAllControls<DevExpress.XtraTreeList.TreeList>(this);
            foreach (var tree in treeLists)
            {
                if (!string.IsNullOrEmpty(tree.Name))
                {
                    SaveControlLayout(root, tree, "TreeList", GetControlPath(tree));
                }
            }

            var pivots = nU3.Core.UI.Helpers.UIHelper.FindAllControls<DevExpress.XtraPivotGrid.PivotGridControl>(this);
            foreach (var pivot in pivots)
            {
                if (!string.IsNullOrEmpty(pivot.Name))
                {
                    SaveControlLayout(root, pivot, "PivotGrid", GetControlPath(pivot));
                }
            }

            // 3. XtraTabControl (현재 선택된 탭 인덱스)
            var tabs = nU3.Core.UI.Helpers.UIHelper.FindAllControls<DevExpress.XtraTab.XtraTabControl>(this);
            foreach (var tab in tabs)
            {
                if (!string.IsNullOrEmpty(tab.Name))
                {
                    var tabNode = new XElement("Control",
                        new XAttribute("Type", "Tab"),
                        new XAttribute("Name", GetControlPath(tab)),
                        new XAttribute("SelectedIndex", tab.SelectedTabPageIndex));
                    root.Add(tabNode);
                }
            }

            // 4. 도킹된 패널 크기 저장 (SplitterControl에 의해 변경된 너비/높이 대응)
            var containers = nU3.Core.UI.Helpers.UIHelper.FindAllControls<Control>(this)
                .Where(c => c is PanelControl || c is GroupControl || c is UserControl)
                .Where(c => c.Dock != DockStyle.Fill && c.Dock != DockStyle.None);

            foreach (var container in containers)
            {
                var sizeNode = new XElement("Control",
                    new XAttribute("Type", "DockSize"),
                    new XAttribute("Name", GetControlPath(container)),
                    new XAttribute("Width", container.Width),
                    new XAttribute("Height", container.Height));
                root.Add(sizeNode);
            }

            // 5. 추가 LayoutControl
            var layouts = nU3.Core.UI.Helpers.UIHelper.FindAllControls<nU3LayoutControl>(this);
            foreach (var layout in layouts)
            {
                if (layout == MainLayoutControl) continue;
                if (!string.IsNullOrEmpty(layout.Name))
                {
                    SaveControlLayout(root, layout, "Layout", GetControlPath(layout));
                }
            }
        }

        private void SaveSplitterPositions(XElement root)
        {
            // SplitContainerControl (표준 DX SplitContainer)
            var splitContainers = nU3.Core.UI.Helpers.UIHelper.FindAllControls<SplitContainerControl>(this);
            foreach (var split in splitContainers)
            {
                if (string.IsNullOrEmpty(split.Name)) continue;

                var splitterNode = new XElement("Control",
                    new XAttribute("Type", "SplitContainer"),
                    new XAttribute("Name", GetControlPath(split)),
                    new XAttribute("Position", split.SplitterPosition));

                root.Add(splitterNode);
            }
        }

        /// <summary>
        /// 컨트롤의 계층 구조를 포함한 고유 경로를 생성합니다.
        /// 이름이 없는 컨트롤은 타입명과 HashCode를 조합하여 식별합니다.
        /// </summary>
        private string GetControlPath(Control control)
        {
            var path = new List<string>();
            Control? current = control;
            while (current != null && current != this)
            {
                string name = string.IsNullOrEmpty(current.Name) 
                    ? $"{current.GetType().Name}_{current.GetHashCode()}" 
                    : current.Name;
                path.Insert(0, name);
                current = current.Parent;
            }
            return string.Join("/", path);
        }

        private void SaveControlLayout(XElement root, object control, string type, string? name = null)
        {
            try
            {
                using var ms = new MemoryStream();
                var method = control.GetType().GetMethod("SaveLayoutToStream", new[] { typeof(Stream) });
                if (method == null) return;

                method.Invoke(control, new object[] { ms });

                if (ms.Length > 0)
                {
                    var data = Encoding.UTF8.GetString(ms.ToArray());
                    var node = new XElement("Control",
                        new XAttribute("Type", type),
                        name != null ? new XAttribute("Name", name) : null,
                        new XCData(data));

                    root.Add(node);
                }
            }
            catch (Exception ex)
            {
                LogWarning($"컨트롤 레이아웃 저장 오류 ({type}:{name}): {ex.Message}");
            }
        }

        /// <summary>
        /// 저장된 레이아웃을 복원합니다. 저장된 파일이 없으면 아무 동작도 하지 않습니다.
        /// </summary>
        public virtual void LoadLayout()
        {
            if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime)
            {
                return;
            }

            try
            {
                var bundlePath = GetLayoutFilePath("bundle");

                if (!File.Exists(bundlePath))
                {
                    return;
                }

                var root = XElement.Load(bundlePath);

                // 1. 주요 컴포넌트 복원
                RestoreAllComponentsFromBundle<nU3DockManager>(root, "Dock");
                RestoreAllComponentsFromBundle<nU3BarManager>(root, "Bars");
                RestoreAllComponentsFromBundle<nU3RibbonControl>(root, "Ribbon");

                // 2. 메인 레이아웃 복원
                var layoutNode = root.Elements("Control").FirstOrDefault(x => x.Attribute("Type")?.Value == "MainLayout");
                if (MainLayoutControl != null && layoutNode != null)
                {
                    RestoreControlLayout(MainLayoutControl, layoutNode.Value);
                }

                // 3. Grid, TreeList, PivotGrid 등 복원
                RestoreGenericControls(root);

                // 4. Splitter 위치 복원
                RestoreSplitterPositions(root);

                LogInfo($"통합 레이아웃 복원 성공: {ProgramID}");
            }
            catch (Exception ex)
            {
                LogError("레이아웃 복원 실패", ex);
            }
        }

        private void RestoreGenericControls(XElement root)
        {
            // 1. Grid
            var grids = nU3.Core.UI.Helpers.UIHelper.FindAllControls<DevExpress.XtraGrid.GridControl>(this);
            foreach (var grid in grids)
            {
                if (grid.MainView == null) continue;

                var path = GetControlPath(grid);
                var node = root.Elements("Control")
                               .FirstOrDefault(x => x.Attribute("Type")?.Value == "Grid"
                                                 && x.Attribute("Name")?.Value == path);

                if (node != null) RestoreControlLayout(grid.MainView, node.Value);
            }

            // 2. TreeList & PivotGrid
            var treeLists = nU3.Core.UI.Helpers.UIHelper.FindAllControls<DevExpress.XtraTreeList.TreeList>(this);
            foreach (var tree in treeLists)
            {
                var path = GetControlPath(tree);
                var node = root.Elements("Control")
                               .FirstOrDefault(x => x.Attribute("Type")?.Value == "TreeList"
                                                 && x.Attribute("Name")?.Value == path);

                if (node != null) RestoreControlLayout(tree, node.Value);
            }

            var pivots = nU3.Core.UI.Helpers.UIHelper.FindAllControls<DevExpress.XtraPivotGrid.PivotGridControl>(this);
            foreach (var pivot in pivots)
            {
                var path = GetControlPath(pivot);
                var node = root.Elements("Control")
                               .FirstOrDefault(x => x.Attribute("Type")?.Value == "PivotGrid"
                                                 && x.Attribute("Name")?.Value == path);

                if (node != null) RestoreControlLayout(pivot, node.Value);
            }

            // 3. TabControl
            var tabs = nU3.Core.UI.Helpers.UIHelper.FindAllControls<DevExpress.XtraTab.XtraTabControl>(this);
            foreach (var tab in tabs)
            {
                var path = GetControlPath(tab);
                var node = root.Elements("Control")
                               .FirstOrDefault(x => x.Attribute("Type")?.Value == "Tab"
                                                 && x.Attribute("Name")?.Value == path);

                if (node != null)
                {
                    var indexAttr = node.Attribute("SelectedIndex");
                    if (indexAttr != null && int.TryParse(indexAttr.Value, out int index))
                    {
                        tab.SelectedTabPageIndex = index;
                    }
                }
            }

            // 4. 패널 크기 복원
            var sizeNodes = root.Elements("Control").Where(x => x.Attribute("Type")?.Value == "DockSize");
            foreach (var node in sizeNodes)
            {
                var path = node.Attribute("Name")?.Value;
                if (string.IsNullOrEmpty(path)) continue;

                var target = FindControlByPath(this, path);
                if (target != null)
                {
                    if (int.TryParse(node.Attribute("Width")?.Value, out int w)) target.Width = w;
                    if (int.TryParse(node.Attribute("Height")?.Value, out int h)) target.Height = h;
                }
            }

            // 5. 추가 LayoutControl
            var layouts = nU3.Core.UI.Helpers.UIHelper.FindAllControls<nU3LayoutControl>(this);
            foreach (var layout in layouts)
            {
                if (layout == MainLayoutControl) continue;
                var path = GetControlPath(layout);
                var node = root.Elements("Control")
                               .FirstOrDefault(x => x.Attribute("Type")?.Value == "Layout"
                                                 && x.Attribute("Name")?.Value == path);

                if (node != null) RestoreControlLayout(layout, node.Value);
            }
        }

        private void RestoreSplitterPositions(XElement root)
        {
            var splitContainers = nU3.Core.UI.Helpers.UIHelper.FindAllControls<SplitContainerControl>(this);
            foreach (var split in splitContainers)
            {
                var path = GetControlPath(split);
                var node = root.Elements("Control")
                               .FirstOrDefault(x => x.Attribute("Type")?.Value == "SplitContainer"
                                                 && x.Attribute("Name")?.Value == path);

                if (node != null)
                {
                    var posAttr = node.Attribute("Position");
                    if (posAttr != null && int.TryParse(posAttr.Value, out int pos))
                    {
                        split.SplitterPosition = pos;
                    }
                }
            }
        }

        /// <summary>
        /// 경로 문자열을 기반으로 컨트롤을 찾습니다.
        /// </summary>
        private Control? FindControlByPath(Control parent, string path)
        {
            var parts = path.Split('/');
            Control? current = parent;

            foreach (var part in parts)
            {
                current = current?.Controls.Cast<Control>().FirstOrDefault(c => 
                    (string.IsNullOrEmpty(c.Name) ? $"{c.GetType().Name}_{c.GetHashCode()}" : c.Name) == part);
                
                if (current == null) break;
            }

            return current;
        }

        private void RestoreControlLayout(object control, string xmlData)
        {
            try
            {
                using var ms = new MemoryStream(Encoding.UTF8.GetBytes(xmlData));
                ms.Seek(0, SeekOrigin.Begin);

                var method = control.GetType().GetMethod("RestoreLayoutFromStream", new[] { typeof(Stream) });
                method?.Invoke(control, new object[] { ms });
            }
            catch (Exception ex)
            {
                LogWarning($"컨트롤 레이아웃 복원 오류: {ex.Message}");
            }
        }

        /// <summary>
        /// 특정 타입의 컴포넌트(예: DockManager, BarManager 등)를 찾아서 레이아웃을 스트림으로 추출하여 번들에 추가합니다.
        /// </summary>
        private void AppendAllComponentsToBundle<T>(XElement root, string typeTag) where T : class
        {
            var comps = ReflectionHelper.FindAllComponents<T>(this, typeof(BaseWorkControl));

            foreach (var kv in comps)
            {
                try
                {
                    using var ms = new MemoryStream();

                    var method = typeof(T).GetMethod("SaveLayoutToStream", new[] { typeof(Stream) });
                    if (method == null) continue;

                    method.Invoke(kv.Value, new object[] { ms });

                    if (ms.Length > 0)
                    {
                        var data = Encoding.UTF8.GetString(ms.ToArray());
                        var node = new XElement("Component",
                            new XAttribute("Type", typeTag),
                            new XAttribute("Name", kv.Key),
                            new XCData(data));

                        root.Add(node);
                    }
                }
                catch (Exception ex)
                {
                    LogWarning($"컴포넌트 레이아웃 저장 중 오류 ({typeTag}:{kv.Key}): {ex.Message}");
                }
            }
        }

        /// <summary>
        /// 번들에 저장된 컴포넌트 레이아웃을 찾아서 해당 컴포넌트에 복원합니다.
        /// </summary>
        private void RestoreAllComponentsFromBundle<T>(XElement root, string typeTag) where T : class
        {
            var comps = ReflectionHelper.FindAllComponents<T>(this, typeof(BaseWorkControl));

            foreach (var kv in comps)
            {
                try
                {
                    var node = root.Elements("Component")
                                   .FirstOrDefault(x => x.Attribute("Type")?.Value == typeTag
                                                     && x.Attribute("Name")?.Value == kv.Key);

                    if (node != null)
                    {
                        using var ms = new MemoryStream(Encoding.UTF8.GetBytes(node.Value));
                        ms.Seek(0, SeekOrigin.Begin);

                        var method = typeof(T).GetMethod("RestoreLayoutFromStream", new[] { typeof(Stream) });
                        method?.Invoke(kv.Value, new object[] { ms });
                    }
                }
                catch (Exception ex)
                {
                    LogWarning($"컴포넌트 레이아웃 복원 중 오류 ({typeTag}:{kv.Key}): {ex.Message}");
                }
            }
        }

        /// <summary>
        /// 해당 컴포넌트 타입 및 선택적 컴포넌트 이름에 대한 레이아웃 파일 경로를 반환합니다.
        /// 경로 형태: %AppData%\nU3\Layouts\{UserId}\{ProgramID}.{componentType}[.{componentName}].xml
        /// </summary>
        /// <param name="componentType">파일에 쓰일 컴포넌트 타입 식별자 (예: bundle, Dock 등)</param>
        /// <param name="componentName">선택적 컴포넌트 이름</param>
        /// <returns>파일 전체 경로</returns>
        protected virtual string GetLayoutFilePath(string componentType, string? componentName = null)
        {
            var userId = _workContext?.CurrentUser?.UserId ?? "default";
            var dir = Path.Combine(LayoutBaseDir, userId);

            string fileName;
            if (string.IsNullOrEmpty(componentName))
            {
                fileName = $"{ProgramID}.{componentType}.xml";
            }
            else
            {
                fileName = $"{ProgramID}.{componentType}.{componentName}.xml";
            }

            return Path.Combine(dir, fileName);
        }

        /// <summary>
        /// 저장된 번들 파일을 삭제하고 내부 캐시를 초기화합니다.
        /// 화면 개발자는 레이아웃 초기화가 필요할 때 이 메서드를 호출하면 됩니다.
        /// </summary>
        public virtual void ResetLayout()
        {
            var bundlePath = GetLayoutFilePath("bundle");

            if (File.Exists(bundlePath))
            {
                File.Delete(bundlePath);
            }

            ReflectionHelper.ClearCache(this);

            ShowInfo("레이아웃 설정이 초기화되었습니다. 화면을 다시 열면 기본값으로 적용됩니다.");
        }

        /// <summary>
        /// Reflection 기반으로 찾은 컴포넌트를 반환합니다.
        /// 주로 테스트 또는 특수한 상황에서만 사용하십시오.
        /// </summary>
        /// <typeparam name="T">찾을 컴포넌트 타입</typeparam>
        /// <returns>해당 컴포넌트(없으면 null)</returns>
        protected T? FindComponent<T>() where T : class
        {
            return ReflectionHelper.FindComponent<T>(this, typeof(BaseWorkControl));
        }

        #endregion

        #region IDisposable 구현 및 이벤트

        /// <summary>
        /// Dispose 패턴 구현. disposing이 true이면 관리 리소스를 해제합니다.
        /// </summary>
        /// <param name="disposing">관리 리소스도 해제할지 여부</param>
        protected override void Dispose(bool disposing)
        {
            if (_isDisposed)
            {
                return;
            }

            if (disposing)
            {
                ReleaseResources();

                _cancellationTokenSource?.Dispose();
            }

            _isDisposed = true;

            base.Dispose(disposing);
        }

        // 디자인타임 디자이너용 스텁 메서드(빈 구현).
        private void InitializeComponent()
        {
        }

        /// <summary>
        /// 전역 환자 선택 이벤트를 발행합니다.
        /// 화면에서 환자 선택 시 다른 모듈에게 전파하려면 이 메서드를 사용하세요.
        /// </summary>
        /// <param name="patient">선택된 환자 정보</param>
        public void PublishPatientSelectedEvent(PatientInfoDto patient)
        {
            if (patient == null)
            {
                return;
            }

            var evt = EventBus?.GetEvent<Events.Contracts.PatientSelectedEvent>();
            evt?.Publish(new Events.Contracts.PatientSelectedEventPayload { Patient = patient, Source = this.ProgramID });

            LogInfo($"환자 선택 이벤트 발행: {patient.PatientName} ({patient.PatientId})");
        }

        #endregion
    }
}