using DashboardDesignExternalTool;
using DevExpress.Charts.Native;
using DevExpress.CodeParser;
using nU3.Core.Contracts.Models;
using nU3.Core.Events;
using nU3.Core.Events.Contracts;
using nU3.Core.Logging;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace nU3.Core.UI
{
    /// <summary>
    /// 재사용 가능한 UI 컴포넌트 기본 클래스
    /// EventBus, Logger 등 공통 기능을 제공합니다.
    /// </summary>
    [System.ComponentModel.ToolboxItem(true)]
    [System.ComponentModel.DefaultEvent("PatientSelected")]
    public class BaseWorkComponent : UserControl
    {
        #region Properties

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
                // BaseWorkControl을 찾으면 EventBus 할당
                if (current is BaseWorkControl baseWorkControl)
                {
                    this.EventBus = baseWorkControl.EventBus;

                    if (this.EventBus != null)
                    {
                        LogInfo($"EventBus assigned from parent: {current.GetType().Name}");
                    }
                    break;
                }

                // 다음 부모로 이동
                current = current.Parent;
            }
        }

        /// <summary>
        /// 정보 로그 기록
        /// </summary>
        protected void LogInfo(string message)
        {
            try
            {
                LogManager.Info(message, EventSource ?? this.GetType().Name);
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
                LogManager.Warning(message, EventSource ?? this.GetType().Name);
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
                LogManager.Error(message, EventSource ?? this.GetType().Name, exception);
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
                LogManager.LogAction(action, this.GetType().Namespace, EventSource ?? this.GetType().Name, additionalInfo);
            }
            catch { }
        }

        #endregion
    }
}
