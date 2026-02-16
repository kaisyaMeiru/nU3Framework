using DevExpress.XtraEditors;
using nU3.Core.Attributes;
using nU3.Core.Context;
using nU3.Core.Events;
using nU3.Core.Interfaces;
using nU3.Core.Logging;
using nU3.Core.Services;
using nU3.Models;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace nU3.Core.UI
{
    /// <summary>
    /// 병원 MDI 자식 작업 화면의 기본 클래스입니다.
    /// 화면 생명주기(활성화/비활성화), 컨텍스트 공유, 리소스 해제 등을 관리합니다.
    /// </summary>
    public class BaseWorkControl : XtraUserControl, IBaseWorkControl, IBaseWorkControlExpand, IDisposable
    {
        private WorkContext _workContext;
        private bool _isActivated;
        private bool _isDisposed;
        private readonly List<IDisposable> _disposables;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private string _programId;
        private string _programTitle;

        #region IScreenIdentifier Implementation

        /// <summary>
        /// 화면 식별자(Program ID)
        /// nU3ProgramInfo 어트리뷰트가 설정된 경우 해당 값을 사용합니다.
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
            protected set => _programId = value;
        }

        /// <summary>
        /// 화면 표시 제목
        /// nU3ProgramInfo 어트리뷰트가 설정된 경우 해당 값을 사용합니다.
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
            protected set => _programTitle = value;
        }

        #endregion

        #region Properties

        /// <summary>
        /// 이벤트 버스 (애플리케이션 내부 이벤트 통신)
        /// </summary>
        public nU3.Core.Events.IEventAggregator EventBus { get; set; }

        /// <summary>
        /// 현재 작업 컨텍스트
        /// </summary>
        public WorkContext Context => _workContext;

        /// <summary>
        /// 연결성 매니저 (전역 싱글톤)
        /// DB, 파일 전송, 로그 업로드 등을 관리합니다.
        /// 디자인 모드에서는 null을 반환합니다.
        /// </summary>
        protected ConnectivityManager Connectivity
        {
            get
            {
                if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime)
                    return null;
                return ConnectivityManager.Instance;
            }
        }

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

        /// <summary>
        /// 취소 토큰
        /// </summary>
        protected CancellationToken CancellationToken => _cancellationTokenSource.Token;

        /// <summary>
        /// 활성화 여부
        /// </summary>
        public bool IsActivated => _isActivated;

        #endregion

        #region Constructor

        public BaseWorkControl()
        {
            // 디자인 모드에서는 초기화 스킵
            if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime)
                return;

            _disposables = new List<IDisposable>();
            _cancellationTokenSource = new CancellationTokenSource();
            _workContext = new WorkContext();

            try
            {
                LogManager.Debug($"BaseWorkControl created: {this.GetType().Name}", "UI");
            }
            catch { }
        }

        #endregion

        #region IWorkContextProvider Implementation

        /// <summary>
        /// 작업 컨텍스트 초기화 (IWorkContextProvider)
        /// </summary>
        public virtual void InitializeContext(WorkContext context)
        {
            // 디자인 모드에서는 스킵
            if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime)
                return;

            if (context == null)
            {
                LogWarning("Attempted to initialize with null context");
                return;
            }

            var oldContext = _workContext;
            _workContext = context.Clone();

            OnContextInitialized(oldContext, _workContext);
            LogInfo($"Context initialized - User: {_workContext.CurrentUser?.UserId}, Patient: {_workContext.CurrentPatient?.PatientId}");
        }

        /// <summary>
        /// 작업 컨텍스트 업데이트 (IWorkContextProvider)
        /// </summary>
        public virtual void UpdateContext(WorkContext context)
        {
            // 디자인 모드에서는 스킵
            if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime)
                return;

            if (context == null)
            {
                LogWarning("Attempted to update with null context");
                return;
            }

            var oldContext = _workContext;
            _workContext = context.Clone();

            OnContextChanged(oldContext, _workContext);
            LogInfo($"Context updated");
        }

        /// <summary>
        /// 현재 컨텍스트 복사본을 반환합니다.
        /// </summary>
        public WorkContext GetContext()
        {
            // 디자인 모드에서는 새 인스턴스 반환
            if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime)
                return new WorkContext();

            return _workContext?.Clone();
        }

        /// <summary>
        /// 컨텍스트 초기화 시 호출되는 훅(하위 클래스에서 재정의)
        /// </summary>
        protected virtual void OnContextInitialized(WorkContext oldContext, WorkContext newContext)
        {
        }

        /// <summary>
        /// 컨텍스트 변경 시 호출되는 훅(하위 클래스에서 재정의)
        /// </summary>
        protected virtual void OnContextChanged(WorkContext oldContext, WorkContext newContext)
        {
        }

        #endregion

        #region ILifecycleAware Implementation

        /// <summary>
        /// 화면 활성화 시 호출됩니다. (ILifecycleAware)
        /// </summary>
        public virtual void OnActivated()
        {
            // 디자인 모드에서는 스킵
            if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime)
                return;

            if (_isActivated)
                return;

            _isActivated = true;

            try
            {
                LogManager.Debug($"Screen activated: {ProgramTitle ?? this.GetType().Name}", "UI");
                OnScreenActivated();

                // 모듈 활성화 이벤트 발행 (Shell Title 등 업데이트용)
                // 모듈에서 직접 데이터를 전달하여 정확성을 높임
                var attr = this.GetType().GetCustomAttribute<nU3ProgramInfoAttribute>();
                EventBus?.GetEvent<ModuleActivatedEvent>().Publish(new ModuleActivatedEventPayload
                {
                    ProgId = this.ProgramID,
                    ProgramName = this.ProgramTitle,
                    ModuleId = attr?.GetModuleId(),
                    Version = this.GetType().Assembly.GetName().Version?.ToString()
                });
            }
            catch (Exception ex)
            {
                LogError("Error during activation", ex);
            }
        }

        /// <summary>
        /// 화면 비활성화 시 호출됩니다. (ILifecycleAware)
        /// </summary>
        public virtual void OnDeactivated()
        {
            // 디자인 모드에서는 스킵
            if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime)
                return;

            if (!_isActivated)
                return;

            _isActivated = false;

            try
            {
                LogManager.Debug($"Screen deactivated: {ProgramTitle ?? this.GetType().Name}", "UI");
                OnScreenDeactivated();
            }
            catch (Exception ex)
            {
                LogError("Error during deactivation", ex);
            }
        }

        /// <summary>
        /// 닫기 가능 여부 확인 (ILifecycleAware)
        /// </summary>
        public virtual bool CanClose()
        {
            // 디자인 모드에서는 항상 true
            if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime)
                return true;

            try
            {
                return OnBeforeClose();
            }
            catch (Exception ex)
            {
                LogError("Error checking CanClose", ex);
                return false;
            }
        }

        /// <summary>
        /// 화면 활성화 시 호출되는 훅
        /// </summary>
        protected virtual void OnScreenActivated()
        {
        }

        /// <summary>
        /// 화면 비활성화 시 호출되는 훅
        /// </summary>
        protected virtual void OnScreenDeactivated()
        {
        }

        /// <summary>
        /// 닫기 직전 호출되는 훅 (기본적으로 true 반환)
        /// </summary>
        protected virtual bool OnBeforeClose()
        {
            return true; // 기본적으로 닫기 허용
        }

        #endregion

        #region IResourceManager Implementation

        /// <summary>
        /// 리소스 해제 (IResourceManager)
        /// </summary>
        public virtual void ReleaseResources()
        {
            // 디자인 모드에서는 스킵
            if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime)
                return;

            if (_isDisposed)
                return;

            try
            {
                LogInfo($"Releasing resources for {ProgramTitle ?? this.GetType().Name}");

                // 비동기 작업 취소 요청
                _cancellationTokenSource?.Cancel();

                // 하위 클래스 리소스 해제 훅 호출
                OnReleaseResources();

                // 등록된 Disposable 인스턴스 정리
                foreach (var disposable in _disposables)
                {
                    try
                    {
                        disposable?.Dispose();
                    }
                    catch (Exception ex)
                    {
                        LogWarning($"Error disposing resource: {ex.Message}");
                    }
                }
                _disposables.Clear();

                LogInfo($"Resources released for {ProgramTitle ?? this.GetType().Name}");
            }
            catch (Exception ex)
            {
                LogError("Error releasing resources", ex);
            }
        }

        /// <summary>
        /// 하위 클래스에서 리소스 해제 로직을 재정의합니다.
        /// 예: 타이머 중지, 이벤트 핸들러 제거, 외부 리소스 반환 등
        /// </summary>
        protected virtual void OnReleaseResources()
        {
        }

        #endregion

        #region Resource Registration

        /// <summary>
        /// Disposable 리소스를 등록합니다.
        /// </summary>
        protected void RegisterDisposable(IDisposable disposable)
        {
            if (disposable != null && !_disposables.Contains(disposable))
            {
                _disposables.Add(disposable);
            }
        }

        /// <summary>
        /// Disposable 리소스 등록을 해제합니다.
        /// </summary>
        protected void UnregisterDisposable(IDisposable disposable)
        {
            if (disposable != null)
            {
                _disposables.Remove(disposable);
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// 레이아웃 초기화 (하위 클래스에서 재정의)
        /// </summary>
        protected virtual void InitializeLayout()
        {
            // DevExpress LayoutControl 등 초기화 로직
        }

        /// <summary>
        /// 정보 로그 기록
        /// </summary>
        protected void LogInfo(string message)
        {
            try
            {
                LogManager.Info(message, ProgramID ?? this.GetType().Name);
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
                LogManager.Warning(message, ProgramID ?? this.GetType().Name);
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
                LogManager.Error(message, ProgramID ?? this.GetType().Name, exception);
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
                LogManager.LogAction(action, this.GetType().Namespace, ProgramID ?? this.GetType().Name, additionalInfo);
            }
            catch { }
        }

        /// <summary>
        /// 권한 검사
        /// </summary>
        protected bool HasPermission(Func<ModulePermissions, bool> permissionCheck)
        {
            try
            {
                return _workContext?.Permissions != null && permissionCheck(_workContext.Permissions);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 읽기 권한 여부
        /// </summary>
        protected bool CanRead => HasPermission(p => p.CanRead);

        /// <summary>
        /// 생성 권한 여부
        /// </summary>
        protected bool CanCreate => HasPermission(p => p.CanCreate);

        /// <summary>
        /// 수정 권한 여부
        /// </summary>
        protected bool CanUpdate => HasPermission(p => p.CanUpdate);

        /// <summary>
        /// 삭제 권한 여부
        /// </summary>
        protected bool CanDelete => HasPermission(p => p.CanDelete);

        /// <summary>
        /// 인쇄 권한 여부
        /// </summary>
        protected bool CanPrint => HasPermission(p => p.CanPrint);

        /// <summary>
        /// 엑셀저장 권한 여부
        /// </summary>
        protected bool CanExport => HasPermission(p => p.CanExport);

        #endregion

        #region IDisposable Implementation

        protected override void Dispose(bool disposing)
        {
            if (_isDisposed)
                return;

            if (disposing)
            {
                // 리소스 정리
                ReleaseResources();

                // 취소 토큰 정리
                _cancellationTokenSource?.Dispose();

                LogInfo($"BaseWorkControl disposed: {ProgramTitle ?? this.GetType().Name}");
            }

            _isDisposed = true;
            base.Dispose(disposing);
        }

        #endregion


        #region Common Event Bus Helpers
        //public void PublishPatientSelectedEvent(PatientInfoDto patient)
        //{
        //    if (patient == null) return;

        //    // EventBus를 통해 다른 모듈에 이벤트 발행
        //    var evenPub = EventBus?.GetEvent<nU3.Core.Events.Contracts.PatientSelectedEvent>();
        //    evenPub?.Publish(new PatientSelectedEventPayload
        //    {
        //        Patient = patient,
        //        Source = this.ProgramID,
        //    });

        //    LogInfo($"PatientSelectedEvent 발행: {patient.PatientName} ({patient.PatientId}) - {this.ProgramID}");
        //    LogInfo($"Patient selected event published: {patient.PatientId}");
        //}
        #endregion
    }
}