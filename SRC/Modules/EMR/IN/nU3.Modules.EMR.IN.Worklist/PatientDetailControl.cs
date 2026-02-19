using System;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using nU3.Core.UI;
using nU3.Core.Context;
using nU3.Core.Events;
using nU3.Core.Attributes;
using nU3.Models;

namespace nU3.Modules.EMR.IN.Worklist
{
    /// <summary>
    /// 환자 상세 정보 컨트롤
    /// 이 컨트롤은 환자 기본 정보, 연락처, 이벤트 로그를 표시합니다.
    /// </summary>
    [nU3ProgramInfo(typeof(PatientDetailControl), "환자 상세", "EMR_IN_00002")]
    public partial class PatientDetailControl : BaseWorkControl
    {
        // 생성자
        public PatientDetailControl()
        {
            InitializeLayout();
            LogInfo("PatientDetailControl 생성됨");
        }

        public override void InitializeContext(WorkContext context)
        {
            base.InitializeContext(context);
            // 이 시점에서는 Shell이 EventBus를 할당해 두었음
            if (EventBus == null)
                AddEventLog("EventBus가 설정되지 않았습니다.");
            else
                EnableEventListening();
        }

        #region 이벤트 구독

        protected override void OnScreenActivated()
        {
            base.OnScreenActivated();

            // 더이상 화면 활성화 시 자동으로 이벤트를 구독하지 않습니다.
            // 이벤트 구독은 수동으로 제어하도록 변경되었습니다.
            AddEventLog("PatientDetailControl 화면 활성화 - 수동 이벤트 구독 모드");
            UpdateStatus("이벤트 수신 대기 (수동)", System.Drawing.Color.Green);

            LogInfo("PatientDetailControl 활성화 - 수동 이벤트 구독 모드");
        }

        protected override void OnScreenDeactivated()
        {
            base.OnScreenDeactivated();

            // 더이상 화면 비활성화 시 자동으로 이벤트를 해제하지 않습니다.
            // 필요 시 외부에서 DisableEventListening()을 호출하세요.
            AddEventLog("PatientDetailControl 화면 비활성화 - 수동 이벤트 구독 모드 유지");
            UpdateStatus("이벤트 비활성화 (수동)", System.Drawing.Color.Gray);

            LogInfo("PatientDetailControl 비활성화 - 수동 이벤트 구독 모드 유지");
        }

        private void SubscribeToEvents()
        {
            if (EventBus == null)
            {
                AddEventLog("EventBus가 없습니다");
                return;
            }

            // 강타입 환자 선택 이벤트 구독
            EventBus.GetEvent<Core.Events.Contracts.PatientSelectedEvent>()
                .Subscribe(OnPatientSelectedGeneric);

            // 환자 업데이트 이벤트 구독
            EventBus.GetEvent<PatientUpdatedEvent>()
                .Subscribe(OnPatientUpdated);

            EventBus.GetEvent<nU3.Core.Events.PatientSelectedEvent>()
                .Subscribe(OnPatientSelected);

            // 작업 컨텍스트 변경 이벤트 구독
            EventBus.GetEvent<WorkContextChangedEvent>()
                .Subscribe(OnWorkContextChanged);

            AddEventLog("구독 완료: PatientSelected (Generic), PatientUpdated, WorkContextChanged");
        }

        private void UnsubscribeFromEvents()
        {
            if (EventBus == null)
                return;


            AddEventLog("이벤트 구독 해제 완료");
        }

        /// <summary>
        /// 외부에서 이벤트 수신을 수동으로 시작하도록 호출합니다.
        /// </summary>
        public void EnableEventListening()
        {
            SubscribeToEvents();
            AddEventLog("이벤트 수신 시작(수동)");
        }

        /// <summary>
        /// 외부에서 이벤트 수신을 수동으로 중지하도록 호출합니다.
        /// </summary>
        public void DisableEventListening()
        {
            UnsubscribeFromEvents();
            AddEventLog("이벤트 수신 중지(수동)");
        }

        #endregion

        #region 이벤트 처리기

        /// <summary>
        /// 강타입 환자 선택 이벤트 처리기
        /// </summary>
        private void OnPatientSelectedGeneric(PatientSelectedEventPayload context)
        {
            try
            {
                AddEventLog("[GenericEvent] PatientSelectedEvent 수신");
                AddEventLog($"   환자: {context.Patient.PatientName} ({context.Patient.PatientId})");

                // UI 스레드에서 안전하게 업데이트
                if (InvokeRequired)
                {
                    Invoke(new Action(() => UpdateUIFromContext(context.Patient)));
                }
                else
                {
                    UpdateUIFromContext(context.Patient);
                }

                UpdateStatus($"환자 변경: {context.Patient.PatientName}", System.Drawing.Color.DarkBlue);
                LogInfo($"환자 선택 처리: {context.Patient.PatientName}");
            }
            catch (Exception ex)
            {
                AddEventLog($"오류: {ex.Message}");
                LogError("OnPatientSelectedGeneric 처리 중 오류", ex);
            }
        }

        private void UpdateUIFromContext(PatientInfoDto context)
        {
            lblPatientIdValue.Text = context.PatientId;
            lblPatientNameValue.Text = context.PatientName;
            lblBirthDateValue.Text = "-";
            lblGenderValue.Text = "-";
            lblAgeValue.Text = "-";

            // 시각적 강조 적용
            grpPatientInfo.Appearance.BackColor = System.Drawing.Color.LightCyan;
        }

        /// <summary>
        /// 기존(비강타입) 환자 선택 이벤트 처리기
        /// </summary>
        private void OnPatientSelected(object payload)
        {
            if (payload is not PatientSelectedEventPayload evt)
                return;

            // 동일 화면에서 발행된 이벤트는 무시
            if (evt.Source == ProgramID)
            {
                AddEventLog($"자기 발행 이벤트 무시: {evt.Source}");
                return;
            }

            try
            {
                AddEventLog($"PatientSelectedEvent 수신 (출처: {evt.Source})");
                AddEventLog($"   환자: {evt.Patient.PatientName} ({evt.Patient.PatientId})");

                // UI 스레드에서 안전하게 업데이트
                if (InvokeRequired)
                {
                    Invoke(new Action(() => DisplayPatientInfo(evt.Patient)));
                }
                else
                {
                    DisplayPatientInfo(evt.Patient);
                }

                // 컨텍스트 업데이트
                var newContext = Context.Clone();
                newContext.CurrentPatient = evt.Patient;
                UpdateContext(newContext);

                UpdateStatus($"환자 선택: {evt.Patient.PatientName}", System.Drawing.Color.Blue);

                LogInfo($"PatientSelectedEvent 처리됨: {evt.Patient.PatientName}");
            }
            catch (Exception ex)
            {
                AddEventLog($"환자 선택 이벤트 처리 중 오류: {ex.Message}");
                LogError("OnPatientSelected 처리 중 오류", ex);
            }
        }

        /// <summary>
        /// 환자 정보 업데이트 이벤트 처리기
        /// </summary>
        private void OnPatientUpdated(object payload)
        {
            if (payload is not PatientUpdatedEventPayload evt)
                return;

            try
            {
                AddEventLog($"PatientUpdatedEvent 수신 (출처: {evt.Source})");
                AddEventLog($"   업데이트 수행자: {evt.UpdatedBy}");

                // 현재 표시 중인 환자와 동일한지 확인
                if (Context.CurrentPatient?.PatientId == evt.Patient.PatientId)
                {
                    DisplayPatientInfo(evt.Patient);

                    MessageBox.Show(
                        $"환자 정보가 업데이트되었습니다.\n\n환자: {evt.Patient.PatientName}",
                        "정보",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    AddEventLog($"환자 정보 갱신: {evt.Patient.PatientName}");
                    LogInfo($"환자 업데이트 처리됨: {evt.Patient.PatientName}");
                }
                else
                {
                    AddEventLog("다른 환자 정보이므로 갱신하지 않음");
                }
            }
            catch (Exception ex)
            {
                AddEventLog($"환자 업데이트 이벤트 처리 중 오류: {ex.Message}");
                LogError("OnPatientUpdated 처리 중 오류", ex);
            }
        }

        /// <summary>
        /// 작업 컨텍스트 변경 이벤트 처리기
        /// </summary>
        private void OnWorkContextChanged(object payload)
        {
            if (payload is not WorkContextChangedEventPayload evt)
                return;

            try
            {
                AddEventLog($"WorkContextChangedEvent 수신 (출처: {evt.Source})");

                // 환자 변경 여부 확인
                if (evt.OldContext?.CurrentPatient?.PatientId != evt.NewContext?.CurrentPatient?.PatientId)
                {
                    if (evt.NewContext?.CurrentPatient != null)
                    {
                        AddEventLog($"   환자 변경: {evt.NewContext.CurrentPatient.PatientName}");

                        if (InvokeRequired)
                        {
                            Invoke(new Action(() => DisplayPatientInfo(evt.NewContext.CurrentPatient)));
                        }
                        else
                        {
                            DisplayPatientInfo(evt.NewContext.CurrentPatient);
                        }
                    }
                    else
                    {
                        AddEventLog("   환자 정보 제거됨");
                        ClearPatientInfo();
                    }
                }

                LogInfo("WorkContextChangedEvent 처리됨");
            }
            catch (Exception ex)
            {
                AddEventLog($"작업 컨텍스트 변경 처리 중 오류: {ex.Message}");
                LogError("OnWorkContextChanged 처리 중 오류", ex);
            }
        }

        #endregion

        #region 표시 관련 메서드

        /// <summary>
        /// 환자 정보 표시
        /// </summary>
        private void DisplayPatientInfo(PatientInfoDto patient)
        {
            if (patient == null)
            {
                ClearPatientInfo();
                return;
            }

            try
            {
                // 기본 정보 설정
                lblPatientIdValue.Text = patient.PatientId;
                lblPatientNameValue.Text = patient.PatientName;
                lblBirthDateValue.Text = patient.BirthDate.ToString("yyyy-MM-dd");

                // 성별 표시
                var genderText = patient.Gender switch
                {
                    1 => "남",
                    2 => "여",
                    9 => "기타",
                    _ => ""
                };
                lblGenderValue.Text = genderText;

                // 나이 설정
                lblAgeValue.Text = $"{patient.Age}";

                // 연락처 설정
                lblPhoneValue.Text = patient.MobileNumber ?? patient.PhoneNumber ?? "-";
                lblAddressValue.Text = patient.Address ?? "-";

                // 시각적 강조
                grpPatientInfo.Appearance.BackColor = System.Drawing.Color.FromArgb(240, 248, 255);
                grpContactInfo.Appearance.BackColor = System.Drawing.Color.FromArgb(240, 248, 255);

                AddEventLog($"환자 정보 표시됨: {patient.PatientName} ({patient.PatientId})");
                LogInfo($"DisplayPatientInfo: {patient.PatientName} ({patient.PatientId})");
            }
            catch (Exception ex)
            {
                LogError("DisplayPatientInfo 처리 중 오류", ex);
            }
        }

        /// <summary>
        /// 환자 정보 초기화
        /// </summary>
        private void ClearPatientInfo()
        {
            lblPatientIdValue.Text = "-";
            lblPatientNameValue.Text = "-";
            lblBirthDateValue.Text = "-";
            lblGenderValue.Text = "-";
            lblAgeValue.Text = "-";
            lblPhoneValue.Text = "-";
            lblAddressValue.Text = "-";

            grpPatientInfo.Appearance.BackColor = System.Drawing.Color.White;
            grpContactInfo.Appearance.BackColor = System.Drawing.Color.White;

            AddEventLog("환자 정보 초기화됨");
        }

        /// <summary>
        /// 이벤트 로그에 메시지 추가
        /// </summary>
        private void AddEventLog(string message)
        {
            try
            {
                if (InvokeRequired)
                {
                    Invoke(new Action(() => AddEventLog(message)));
                    return;
                }

                var timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
                var logMessage = $"[{timestamp}] {message}\r\n";

                memoEventLog.Text += logMessage;

                // 자동 스크롤
                memoEventLog.SelectionStart = memoEventLog.Text.Length;
                memoEventLog.ScrollToCaret();

                // 로그 길이 제한 (1000줄 이상이면 최근 500줄만 유지)
                var lines = memoEventLog.Lines;
                if (lines.Length > 1000)
                {
                    var newText = string.Join("\r\n", lines, lines.Length - 500, 500);
                    memoEventLog.Text = newText;
                }
            }
            catch
            {
                // 무시
            }
        }

        /// <summary>
        /// 상태 표시 업데이트
        /// </summary>
        private void UpdateStatus(string message, System.Drawing.Color color)
        {
            try
            {
                if (InvokeRequired)
                {
                    Invoke(new Action(() => UpdateStatus(message, color)));
                    return;
                }

                lblStatus.Text = $"상태: {message}";
                lblStatus.Appearance.ForeColor = color;
            }
            catch
            {
                // 무시
            }
        }

        #endregion

        #region 라이프사이클

        protected override void OnContextInitialized(WorkContext oldContext, WorkContext newContext)
        {
            base.OnContextInitialized(oldContext, newContext);

            // 초기 환자 정보 표시
            if (newContext.CurrentPatient != null)
            {
                DisplayPatientInfo(newContext.CurrentPatient);
                AddEventLog($"초기 컨텍스트 - 환자: {newContext.CurrentPatient.PatientName}");
            }

            LogInfo("컨텍스트 초기화됨");
        }

        protected override void OnContextChanged(WorkContext oldContext, WorkContext newContext)
        {
            base.OnContextChanged(oldContext, newContext);

            // 환자 변경 여부 처리
            if (oldContext?.CurrentPatient?.PatientId != newContext?.CurrentPatient?.PatientId)
            {
                if (newContext?.CurrentPatient != null)
                {
                    DisplayPatientInfo(newContext.CurrentPatient);
                    AddEventLog($"컨텍스트 변경 - 환자: {newContext.CurrentPatient.PatientName}");
                }
                else
                {
                    ClearPatientInfo();
                    AddEventLog("컨텍스트 변경 - 환자 정보 제거됨");
                }
            }

            LogInfo("컨텍스트 변경 처리됨");
        }

        protected override bool OnBeforeClose()
        {
            AddEventLog("화면 닫기 요청");
            LogInfo("화면 닫기 처리");
            return true;
        }

        protected override void OnReleaseResources()
        {
            base.OnReleaseResources();

            try
            {
                // 이벤트 구독 해제
                DisableEventListening();
            }
            catch
            {
                // 무시
            }

            AddEventLog("리소스가 해제되었습니다");
            LogInfo("리소스 해제됨");
        }

        #endregion

    }
}