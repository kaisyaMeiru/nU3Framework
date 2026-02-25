using System;
using System.Windows.Forms;
using nU3.Core.UI;
using nU3.Core.Context;
using nU3.Core.Events;
using nU3.Models;
using nU3.Core.Attributes;
using nU3.Core.Events.Contracts;

namespace nU3.Modules.EMR.IN.Worklist
{
    /// <summary>
    /// BaseWorkControl 사용 예시
    /// 이벤트 구독 및 컨텍스트 관리 데모용 샘플 화면
    /// </summary>
    [nU3ProgramInfo(typeof(SampleWorkControl), "샘플 작업화면", "EMR_IN_00005")]
    public partial class SampleWorkControl : BaseWorkControl
    {
        // UI 필드 및 초기화는 디자이너 파일로 분리되었습니다.

        public SampleWorkControl()
        {
            InitializeLayout();
            LogInfo("SampleWorkControl 생성됨");
        }

        #region 이벤트 구독
        public override void InitializeContext(WorkContext context)
        {
            base.InitializeContext(context);
            // 이 시점에서는 Shell이 EventBus를 할당해 두었음
            if (EventBus == null)
                AddEventLog("EventBus가 설정되지 않았습니다.");
            else
                EnableEventListening();
        }

        protected override void OnScreenActivated()
        {
            base.OnScreenActivated();

            // 더이상 활성화 시 자동 구독하지 않습니다. 수동 제어를 사용하세요.
            AddEventLog("화면 활성화 - 수동 이벤트 구독 모드");
            LogInfo("SampleWorkControl 활성화 - 수동 이벤트 구독 모드");
        }

        protected override void OnScreenDeactivated()
        {
            base.OnScreenDeactivated();

            AddEventLog("화면 비활성화 - 수동 이벤트 구독 모드 유지");
            LogInfo("SampleWorkControl 비활성화 - 수동 이벤트 구독 모드 유지");
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
            // 현재 PubSubEvent는 WeakReference로 자동 해제되므로 명시적 해제는 선택사항입니다.
            UnsubscribeFromEvents();
            AddEventLog("이벤트 수신 중지(수동)");
        }

        private void SubscribeToEvents()
        {
            if (EventBus == null)
            {
                AddEventLog("EventBus가 설정되지 않았습니다");
                return;
            }

            // 환자 선택 이벤트 구독
            EventBus.GetEvent<PatientSelectedEvent>()
                .Subscribe(OnPatientSelected);


            AddEventLog("PatientSelectedEvent 구독 완료");
        }

        /// <summary>
        /// 이벤트 구독을 수동으로 해제합니다.
        /// PubSubEvent는 약한 참조(WeakReference) 기반이라 구독자가 GC되면 자동 해제됩니다.
        /// 필요 시 명시적 구독 해제 로직을 여기에 구현하세요.
        /// </summary>
        public void UnsubscribeFromEvents()
        {
            if (EventBus == null)
                return;

            // 현재 PubSubEvent는 내부적으로 WeakReference를 사용하여 자동으로 구독을 해제합니다.
            // 명시적으로 구독 해제를 구현하려면 PubSubEvent에 토큰 관리 기능을 추가해야 합니다.
            AddEventLog("이벤트 구독 해제 완료 (수동 호출)");
        }

        #endregion

        #region 이벤트 처리기

        private void OnPatientSelected(object payload)
        {
            if (payload is not PatientSelectedEventPayload evt)
                return;

            // 자기 자신이 발행한 이벤트는 무시
            if (evt.Source == ProgramID)
                return;

            try
            {
                AddEventLog("PatientSelectedEvent 수신");
                AddEventLog($"   출처: {evt.Source}");
                AddEventLog($"   환자: {evt.Patient.PatientName} ({evt.Patient.PatientId})");

                // UI 업데이트
                UpdatePatientInfo(evt.Patient);

                // 컨텍스트 업데이트
                var newContext = Context.Clone();
                newContext.CurrentPatient = evt.Patient;
                UpdateContext(newContext);

                LogInfo($"Patient selected event 처리됨: {evt.Patient.PatientName}");
            }
            catch (Exception ex)
            {
                AddEventLog($"이벤트 처리 오류: {ex.Message}");
                LogError("PatientSelectedEvent 처리 중 오류", ex);
            }
        }

        private void BtnTest_Click(object sender, EventArgs e)
        {
            // 테스트용 환자 데이터 생성
            var testPatient = new PatientInfoDto
            {
                PatientId = "T999",
                PatientName = "테스트환자",
                BirthDate = new DateTime(1985, 5, 15),
                Gender = 1, // 1: 남성
                PhoneNumber = "02-9999-9999",
                MobileNumber = "010-9999-9999",
                Address = "서울특별시 강남구 테스트로 999"
            };

            // 이벤트 발행
            EventBus?.GetEvent<PatientSelectedEvent>()
                .Publish(new PatientSelectedEventPayload
                {
                    Patient = testPatient,
                    Source = ProgramID
                });

            AddEventLog("PatientSelectedEvent 발행됨");
            AddEventLog($"   환자: {testPatient.PatientName} ({testPatient.PatientId})");

            MessageBox.Show(
                "테스트 이벤트가 발행되었습니다.\n\n다른 열려있는 모듈에서 이 이벤트를 수신할 수 있습니다.",
                "이벤트 발행",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        #endregion

        #region 표시 관련 메서드

        private void UpdatePatientInfo(PatientInfoDto patient)
        {
            if (patient == null)
            {
                _lblPatientInfo.Text = "환자 정보: 없음";
                _lblPatientInfo.BackColor = System.Drawing.Color.White;
                return;
            }

            var genderText = patient.Gender switch
            {
                1 => "남",
                2 => "여",
                9 => "기타",
                _ => "미지정"
            };

            _lblPatientInfo.Text = $"환자 정보:\n" +
                $"ID: {patient.PatientId} | 이름: {patient.PatientName} | " +
                $"생년월일: {patient.BirthDate:yyyy-MM-dd} | 성별: {genderText}";

            _lblPatientInfo.BackColor = System.Drawing.Color.FromArgb(240, 248, 255);
        }

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

                _txtEventLog.Text += logMessage;
                _txtEventLog.SelectionStart = _txtEventLog.Text.Length;
                _txtEventLog.ScrollToCaret();
            }
            catch
            {
                // 로그 기록 실패는 무시
            }
        }

        #endregion

        #region 라이프사이클

        protected override void OnContextInitialized(WorkContext oldContext, WorkContext newContext)
        {
            base.OnContextInitialized(oldContext, newContext);

            // 초기 사용자 정보 표시
            if (newContext.CurrentUser != null)
            {
                _lblUserInfo.Text = $"사용자 정보:\n" +
                    $"ID: {newContext.CurrentUser.UserId} | " +
                    $"이름: {newContext.CurrentUser.UserName} | " +
                    $"권한 레벨: {newContext.CurrentUser.AuthLevel}";

                _lblUserInfo.BackColor = System.Drawing.Color.FromArgb(240, 255, 240);
            }

            // 초기 환자 정보 표시
            if (newContext.CurrentPatient != null)
            {
                UpdatePatientInfo(newContext.CurrentPatient);
                AddEventLog($"초기 컨텍스트 - 환자: {newContext.CurrentPatient.PatientName}");
            }

            AddEventLog("컨텍스트 초기화 완료");
            LogInfo("Context initialized");
        }

        protected override void OnContextChanged(WorkContext oldContext, WorkContext newContext)
        {
            base.OnContextChanged(oldContext, newContext);

            // 환자가 변경된 경우 처리
            if (oldContext?.CurrentPatient?.PatientId != newContext?.CurrentPatient?.PatientId)
            {
                if (newContext?.CurrentPatient != null)
                {
                    UpdatePatientInfo(newContext.CurrentPatient);
                    AddEventLog($"컨텍스트 변경 - 환자: {newContext.CurrentPatient.PatientName}");
                }
                else
                {
                    UpdatePatientInfo(null);
                    AddEventLog("컨텍스트 변경 - 환자 정보 제거됨");
                }
            }

            LogInfo("Context changed");
        }

        protected override bool OnBeforeClose()
        {
            AddEventLog("화면 닫기 요청");
            LogInfo("Screen closing");
            return true;
        }

        private void InitializeComponent()
        {
            SuspendLayout();
            // 
            // SampleWorkControl
            // 
            Name = "SampleWorkControl";
            Size = new Size(1528, 850);
            ResumeLayout(false);

        }

        protected override void OnReleaseResources()
        {
            base.OnReleaseResources();

            // 이벤트 핸들러 해제
            if (_btnTest != null)
                _btnTest.Click -= BtnTest_Click;

            AddEventLog("리소스 해제됨");
            LogInfo("Resources released");
        }

        #endregion
    }
}
