using DashboardDesignExternalTool;
using DevExpress.Charts.Native;
using DevExpress.CodeParser;
using nU3.Core.Events;
using nU3.Core.Events.Contracts;
using nU3.Core.Logging;
using nU3.Core.Attributes;
using nU3.Core.Interfaces;
using System;
using System.Reflection;
using System.ComponentModel;
using System.Windows.Forms;

namespace nU3.Core.UI
{
    /// <summary>
    /// 재사용 가능한 UI 컴포넌트 기본 클래스
    /// EventBus, Logger 등 공통 기능을 제공합니다.
    /// </summary>
    [System.ComponentModel.ToolboxItem(true)]
    [System.ComponentModel.DefaultEvent("PatientSelectedEvent")]
    public class BaseWorkComponent : UserControl, IBaseWorkComponent
    {
        private string _programId;

        #region Properties               

        #region IBaseWorkComponent Implementation
        /// <summary>
        /// 상위(Owner) 이벤트 버스 구현 (자신이 가진 EventBus 반환)
        /// </summary>
        public IEventAggregator OwnerEventBus { get; set; }

        /// <summary>
        /// 상위(Owner) 프로그램 ID 구현 (자신의 ProgramID 반환)
        /// </summary>
        public string OwnerProgramID { get; set; }
        #endregion


        /// <summary>
        /// 이벤트 버스 (애플리케이션 내부 이벎트 통신)
        /// </summary>
        public IEventAggregator EventBus { get; set; }

        /// <summary>
        /// EventBus 전파 사용 여부
        /// </summary>
        [Category("Behavior")]
        [Description("이벤트를 EventBus를 통해 전파할지 여부")]
        public bool EventBusUse { get; set; } = false;

        /// <summary>
        /// 이벤트 전파 소스 식별자
        /// </summary>
        [Category("Data")]
        [Description("이벤트 전파 시 식별자")]
        public string EventSource { get; set; } = nameof(BaseWorkComponent);

        /// <summary>
        /// 로거
        /// 디자인 모드에서는 null을 반환합니다.
        /// </summary>
        protected ILogger Logger
        {
            get
            {
                if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime)
                    return null;
                try
                {
                    return LogManager.Instance.Logger;
                }
                catch
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 감사 로거
        /// 디자인 모드에서는 null을 반환합니다.
        /// </summary>
        protected IAuditLogger AuditLogger
        {
            get
            {
                if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime)
                    return null;
                try
                {
                    return LogManager.Instance.AuditLogger;
                }
                catch
                {
                    return null;
                }
            }
        }
        #endregion

        #region Constructor

        public BaseWorkComponent()
        {
            // 디자인 모드에서는 초기화 스킵
            if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime)
                return;

            this.Dock = DockStyle.Fill;
        }

        #endregion

        #region Lifecycle Methods

        /// <summary>
        /// 컨트롤 로드 시 EventBus를 상속받습니다.
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            AssignEventBusFromParent();
        }

        #endregion

        #region Event Publishing Helpers

        /// <summary>
        /// 계층 구조에서 부모 컨트롤의 EventBus를 상속받습니다.
        /// 부모부터 시작하여 루트까지 계층적으로 탐색하며
        /// 첫 번째 BaseWorkControl을 찾아 EventBus를 할당합니다.
        /// </summary>
        protected void AssignEventBusFromParent()
        {
            // 디자인 모드에서는 할당 안 함
            if (DesignMode || LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                return;

            var current = this.Parent;

            // 부모 컨트롤에서 시작하여 루트까지 계층 탐색
            while (current != null)
            {
                // 1. BaseWorkForm (메인 화면) 확인
                if (current is BaseWorkForm baseWorkForm)
                {
                    if (baseWorkForm.SourceControl != null)
                    {
                        // SourceControl이 있는 경우, SourceControl의 EventBus를 우선적으로 가져옴
                        this.OwnerEventBus = baseWorkForm.SourceControl.EventBus;
                        this.OwnerProgramID = baseWorkForm.SourceControl.ProgramID;
                        if (this.OwnerEventBus != null)
                        {
                            this.EventBus = this.OwnerEventBus; // 자신의 EventBus도 동일하게 설정 (선택 사항)
                            LogInfo($"EventBus assigned from BaseWorkForm's SourceControl: {baseWorkForm.SourceControl.GetType().Name} ({this.OwnerProgramID})");
                            break;
                        }
                    }
                }

                // 2. IBaseWorkComponent (중첩된 컴포넌트) 확인
                if (current is IBaseWorkComponent baseComponent)
                {
                    // OwnerEventBus를 통해 상위 EventBus 가져옴
                    this.OwnerEventBus = baseComponent.OwnerEventBus;
                    this.OwnerProgramID = baseComponent.OwnerProgramID;

                    if (this.OwnerEventBus != null)
                    {
                        this.EventBus = this.OwnerEventBus; // 자신의 EventBus도 동일하게 설정 (선택 사항)
                        LogInfo($"EventBus assigned from parent(IBaseWorkComponent): {current.GetType().Name} ({this.OwnerProgramID})");
                    }
                    else
                    {
                        // 만약 부모가 EventBus가 없다면 계속 탐색할지, 아니면 여기서 끊을지?
                        // 보통은 여기서 끊기지만, 명시적으로 EventBus가 null이면 계속 올라가는게 나을 수도 있음. 
                        // 하지만 IBaseWorkComponent를 만났다는건 의도된 계층구조이므로 여기서 멈추는게 맞음.
                    }
                    break;
                }
                
                // 3. BaseWorkControl (메인 화면) 확인
                if (current is BaseWorkControl baseWorkControl)
                {
                    this.OwnerEventBus = baseWorkControl.EventBus;
                    this.OwnerProgramID = baseWorkControl.ProgramID;

                    if (this.OwnerEventBus != null)
                    {
                        this.EventBus = this.OwnerEventBus; // 자신의 EventBus도 동일하게 설정 (선택 사항)
                        LogInfo($"EventBus assigned from parent(BaseWorkControl): {current.GetType().Name} ({this.OwnerProgramID})");
                    }
                    break;
                }

            
                
                // 다음 부모로 이동
                if (current.Parent != null)
                {
                    current = current.Parent;
                }
                else
                {
                    // 부모가 없는 경우 (Top Level Control)
                    // 만약 현재 컨트롤이 Form이고 Owner가 있다면 Owner로 이동 (팝업/모달리스 지원)
                    if (current is Form form && form.Owner != null)
                    {
                        current = form.Owner;
                        LogInfo($"Traversing to Form Owner: {current.GetType().Name}");
                    }
                    else
                    {
                        // 더 이상 부모가 없음
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 정보 로그 기록
        /// </summary>
        protected void LogInfo(string message)
        {
            try
            {
                LogManager.Info("[정보] " + message, EventSource ?? this.GetType().Name);
            }
            catch { }
        }

        /// <summary>
        /// 경고 로그 기록
        /// </summary>
        protected void LogWarning(string message)
        {
            try
            {
                LogManager.Warning("[경고] " + message, EventSource ?? this.GetType().Name);
            }
            catch { }
        }

        /// <summary>
        /// 오류 로그 기록
        /// </summary>
        protected void LogError(string message, Exception exception = null)
        {
            try
            {
                LogManager.Error("[오류] " + message, EventSource ?? this.GetType().Name, exception);
            }
            catch { }
        }

        /// <summary>
        /// 감사 로그 기록
        /// </summary>
        protected void LogAudit(string action, string entityType = null, string entityId = null, string additionalInfo = null)
        {
            try
            {
                LogManager.LogAction("[감사] " + action, this.GetType().Namespace, EventSource ?? this.GetType().Name, additionalInfo);
            }
            catch { }
        }

        #endregion
    }
}
