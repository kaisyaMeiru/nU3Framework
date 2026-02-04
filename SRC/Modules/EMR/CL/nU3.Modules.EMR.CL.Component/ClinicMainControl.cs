using System;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraTab;
using nU3.Core.UI;
using nU3.Core.Context;
using nU3.Core.Events;
using nU3.Core.Attributes;
using nU3.Models;
using nU3.Core.UI.Components.Events;

namespace nU3.Modules.EMR.CL.Component
{
    [nU3ProgramInfo(typeof(ClinicMainControl), "외래진료", "EMR_CLINIC_001")]
    public partial class ClinicMainControl : BaseWorkControl
    {
        public ClinicMainControl()
        {
            InitializeComponent();

            if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Runtime)
            {
                LogInfo("ClinicMainControl 생성됨");
            }
        }

        #region MVVM 패턴: EventBus 구독

        public override void InitializeContext(WorkContext context)
        {
            base.InitializeContext(context);

            if (EventBus == null)
            {
                AddLogMessage("EventBus가 설정되지 않았습니다.");
                return;
            }

            EnableEventListening();
        }

        protected override void OnScreenActivated()
        {
            base.OnScreenActivated();

            AddLogMessage("ClinicMainControl 활성화");
            UpdateStatus("외래진료 대기중", System.Drawing.Color.Green);

            LogInfo("ClinicMainControl 활성화");

            LoadDemoDataIfNeeded();
        }

        private void LoadDemoDataIfNeeded()
        {
            if (Context.CurrentPatient == null && _patientControl != null)
            {
                AddLogMessage("데모 환자 데이터 자동 선택 시작...");
            }
        }

        protected override void OnScreenDeactivated()
        {
            base.OnScreenDeactivated();

            AddLogMessage("ClinicMainControl 비활성화");
            LogInfo("ClinicMainControl 비활성화");
        }

        public void EnableEventListening()
        {
            if (EventBus == null) return;

            EventBus.GetEvent<PatientSelectedEvent>()
                .Subscribe(OnPatientSelectedFromModule);

            EventBus.GetEvent<VisitInfoUpdatedEvent>()
                .Subscribe(OnVisitInfoUpdated);

            AddLogMessage("EventBus 구독 완료: PatientSelectedEvent, VisitInfoUpdatedEvent");
        }

        public void DisableEventListening()
        {
            // EventBus는 WeakReference를 사용하므로 명시적 구독 해제 불필요
            // 컨트롤이 Dispose되면 자동으로 가비지 컬렉션됨
        }

        #endregion

        #region MVVM 패턴: EventBus 이벤트 처리기

        private void OnPatientSelectedFromModule(object payload)
        {
            if (payload is not PatientSelectedEventPayload evt)
                return;

            if (evt.Source == ProgramID)
                return;

            if (evt.Patient == null)
            {
                AddLogMessage("[EventBus] PatientSelectedEvent 수신했으나 환자 정보가 null입니다.");
                return;
            }

            try
            {
                AddLogMessage($"[EventBus] PatientSelectedEvent 수신 (출처: {evt.Source})");
                AddLogMessage($"   환자: {evt.Patient.PatientName} ({evt.Patient.PatientId})");

                if (InvokeRequired)
                {
                    Invoke(new UpdatePatientDelegate(UpdatePatientUI), evt.Patient);
                }
                else
                {
                    UpdatePatientUI(evt.Patient);
                }

                var newContext = Context.Clone();
                newContext.CurrentPatient = evt.Patient;
                UpdateContext(newContext);

                UpdateStatus($"환자 선택됨: {evt.Patient.PatientName}", System.Drawing.Color.Blue);
                LogInfo($"모듈 간 환자 선택 수신: {evt.Patient.PatientName}");
            }
            catch (Exception ex)
            {
                AddLogMessage($"오류: {ex.Message}");
                LogError("OnPatientSelectedFromModule 처리 중 오류", ex);
            }
        }

        private void OnVisitInfoUpdated(object payload)
        {
            if (payload is not VisitInfoUpdatedEventPayload evt)
                return;

            try
            {
                AddLogMessage($"[EventBus] VisitInfoUpdatedEvent 수신 (출처: {evt.Source})");
                AddLogMessage($"   진료ID: {evt.VisitId}, 진료 타입: {evt.VisitType}");

                if (_visitControl != null)
                {
                    _visitControl.LoadVisitRecords(Context.CurrentPatient?.PatientId, showNotification: false);
                }

                UpdateStatus($"진료 정보 업데이트됨", System.Drawing.Color.Orange);
                LogInfo($"진료 정보 업데이트 수신: {evt.VisitId}");
            }
            catch (Exception ex)
            {
                AddLogMessage($"오류: {ex.Message}");
                LogError("OnVisitInfoUpdated 처리 중 오류", ex);
            }
        }

        #endregion

        #region 이벤트 핸들러 패턴: UI Components 이벤트 처리

        private void OnPatientSelectedFromControl(object? sender, PatientSelectedEventArgs e)
        {
            if (e.Patient == null) return;

            try
            {
                AddLogMessage($"[Control] PatientSelectorControl에서 환자 선택");
                AddLogMessage($"   환자: {e.Patient.PatientName} ({e.Patient.PatientId})");

                UpdatePatientUI(e.Patient);

                PublishPatientSelected(e.Patient);

                var newContext = Context.Clone();
                newContext.CurrentPatient = e.Patient;
                UpdateContext(newContext);

                UpdateStatus($"환자 선택: {e.Patient.PatientName}", System.Drawing.Color.Blue);
                LogInfo($"컨트롤에서 환자 선택: {e.Patient.PatientName}");
            }
            catch (Exception ex)
            {
                AddLogMessage($"오류: {ex.Message}");
                LogError("OnPatientSelectedFromControl 처리 중 오류", ex);
            }
        }

        private void OnVisitRecordedFromControl(object? sender, VisitRecordedEventArgs e)
        {
            try
            {
                AddLogMessage($"[Control] 진료 기록 등록");
                AddLogMessage($"   환자: {e.PatientName}, 진료 타입: {e.VisitType}");

                PublishVisitInfoUpdated(e);

                UpdateStatus($"진료 기록 저장됨: {e.PatientName}", System.Drawing.Color.Green);
                LogInfo($"진료 기록 등록: {e.PatientName}");
            }
            catch (Exception ex)
            {
                AddLogMessage($"오류: {ex.Message}");
                LogError("OnVisitRecordedFromControl 처리 중 오류", ex);
            }
        }

        #endregion

        #region EventBus 발행

        private void PublishPatientSelected(PatientInfoDto patient)
        {
            try
            {
                EventBus?.GetEvent<PatientSelectedEvent>()
                    .Publish(new PatientSelectedEventPayload
                    {
                        Patient = patient,
                        Source = ProgramID
                    });

                AddLogMessage($"[EventBus] PatientSelectedEvent 발행됨");
            }
            catch (Exception ex)
            {
                LogError("PatientSelectedEvent 발행 중 오류", ex);
            }
        }

        private void PublishVisitInfoUpdated(VisitRecordedEventArgs e)
        {
            try
            {
                EventBus?.GetEvent<VisitInfoUpdatedEvent>()
                    .Publish(new VisitInfoUpdatedEventPayload
                    {
                        VisitId = e.VisitId,
                        PatientId = e.PatientId,
                        VisitType = e.VisitType,
                        Source = ProgramID
                    });

                AddLogMessage($"[EventBus] VisitInfoUpdatedEvent 발행됨");
            }
            catch (Exception ex)
            {
                LogError("VisitInfoUpdatedEvent 발행 중 오류", ex);
            }
        }

        #endregion

        #region UI 업데이트

        private void UpdatePatientUI(PatientInfoDto patient)
        {
            if (_patientControl != null)
            {
                _patientControl.SetSelectedPatient(patient);
            }

            if (_visitControl != null && patient != null)
            {
                _visitControl.SetCurrentPatient(patient);
            }
        }

        #endregion

        #region 유틸리티

        private void UpdateStatus(string message, System.Drawing.Color color)
        {
            _statsControl?.UpdateStatus(message, color);
        }

        private void AddLogMessage(string message)
        {
            _statsControl?.AddLogMessage(message);
        }

        #endregion

        #region 라이프사이클

        protected override void OnContextInitialized(WorkContext oldContext, WorkContext newContext)
        {
            base.OnContextInitialized(oldContext, newContext);

            if (newContext.CurrentPatient != null)
            {
                UpdatePatientUI(newContext.CurrentPatient);
                AddLogMessage($"초기 컨텍스트 - 환자: {newContext.CurrentPatient.PatientName}");
            }

            LogInfo("Context 초기화됨");
        }

        protected override void OnContextChanged(WorkContext oldContext, WorkContext newContext)
        {
            base.OnContextChanged(oldContext, newContext);

            if (oldContext?.CurrentPatient?.PatientId != newContext?.CurrentPatient?.PatientId)
            {
                if (newContext?.CurrentPatient != null)
                {
                    UpdatePatientUI(newContext.CurrentPatient);
                    AddLogMessage($"컨텍스트 변경 - 환자: {newContext.CurrentPatient.PatientName}");
                }
            }

            LogInfo("Context 변경됨");
        }

        protected override bool OnBeforeClose()
        {
            AddLogMessage("화면 닫기 요청");
            LogInfo("화면 닫기 처리");
            return true;
        }

        protected override void OnReleaseResources()
        {
            base.OnReleaseResources();

            try
            {
                DisableEventListening();
            }
            catch
            {
            }

            AddLogMessage("리소스가 해제되었습니다");
            LogInfo("리소스 해제됨");
        }

        #endregion

        #region 델리게이트

        private delegate void UpdatePatientDelegate(PatientInfoDto patient);

        #endregion
    }
}
