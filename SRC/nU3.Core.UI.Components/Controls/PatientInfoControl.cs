using System;
using System.Data;
using DevExpress.XtraEditors;
using nU3.Core.Events;
using nU3.Core.UI.Components.Events;
using nU3.Core.UI;
using nU3.Core.UI.Controls;
using nU3.Models;
using PatientSelectedEvent = nU3.Core.Events.PatientSelectedEvent;
using System.ComponentModel;

namespace nU3.Core.UI.Components.Controls
{
    /// <summary>
    /// 환자 정보 컨트롤 - 범용 컴포넌트
    /// EventBus를 통해 환자 선택 이벤트를 구독하여 정보를 표시
    /// </summary>
    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(PatientInfoControl))]
    public partial class PatientInfoControl : BaseWorkComponent
    {
        private string? _currentPatientId;

        /// <summary>
        /// EventBus 전파 활성화 여부 (BaseWorkComponent에서 제공하는 프로퍼티를 재정의)
        /// </summary>
        [Category("Behavior")]
        [Description("이벤트를 EventBus를 통해 전파할지 여부")]
        public new bool EventBusUse
        {
            get => base.EventBusUse;
            set => base.EventBusUse = value;
        }

        /// <summary>
        /// 이벤트 소스 식별자 (BaseWorkComponent에서 제공하는 프로퍼티를 재정의)
        /// </summary>
        [Category("Data")]
        [Description("이벤트 전파 시 식별자")]
        public new string EventSource
        {
            get => base.EventSource;
            set => base.EventSource = value ?? nameof(PatientInfoControl);
        }

        public PatientInfoControl()
        {
            InitializeComponent();
        }

        #region EventBus Subscription

        /// <summary>
        /// 컨트롤 로드 시 EventBus 이벤트 구독 시작
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            SubscribeToPatientSelectedEvent();
        }

        /// <summary>
        /// PatientSelectedEvent 구독
        /// EventBus를 통해 환자 선택 이벤트를 받아 정보를 표시
        /// </summary>
        private void SubscribeToPatientSelectedEvent()
        {
            // 디자인 모드에서는 구독 안 함
            if (DesignMode || LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                return;

            // EventBus가 할당되지 않았으면 부모에서 상속받기
            if (EventBus == null)
                AssignEventBusFromParent();

            if (EventBus == null)
            {
                LogWarning("EventBus가 할당되지 않아 이벤트 구독을 시작할 수 없습니다.");
                return;
            }

            // PatientSelectedEvent 구독
            EventBus.GetEvent<nU3.Core.Events.Contracts.PatientSelectedEvent>()
                .Subscribe(OnPatientSelected);

            LogInfo("PatientSelectedEvent 구독 시작");
        }

        /// <summary>
        /// 환자 선택 이벤트 처리
        /// </summary>
        /// <param name="context">환자 컨텍스트 (PatientId, PatientName, VisitNo)</param>
        private void OnPatientSelected(PatientSelectedEventPayload context)
        {
            if (context == null) return;

            LogInfo($"환자 선택 이벤트 수신: {context.Patient.PatientName} ({context.Patient.PatientId})");

            // 이전 환자 ID와 비교하여 중복 이벤트 방지
            if (_currentPatientId == context.Patient.PatientId)
            {
                LogInfo($"같은 환자 ID로 중복 이벤트 무시: {context.Patient.PatientId}");
                return;
            }

            _currentPatientId = context.Patient.PatientId;


            // 데모 데이터 조회 (실제로는 DB에서 조회)
            SetPatientInfo(context.Patient);
            //var patientInfo = GetPatientInfoByPatientId(context.Patient.PatientId);
            //if (patientInfo != null)
            //{
            //    // 환자 정보 표시
            //    SetPatientInfo(context.Patient);
            //}
            //else
            //{
            //    // 환자 정보가 없으면 초기화
            //    ClearPatientInfo();
            //    LogWarning($"환자 정보를 찾을 수 없음: {context.Patient.PatientId}");
            //}
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 환자 정보를 로드합니다 (환자 ID로 조회)
        /// </summary>
        /// <param name="patientId">환자 ID (환자 번호)</param>
        public void LoadPatientInfo(string patientId)
        {
            if (!string.IsNullOrEmpty(patientId))
            {
                // 데모 데이터 - 실제로는 DB에서 조회
                var patientInfo = GetPatientInfoByPatientId(patientId);

                if (patientInfo != null)
                {
                    SetPatientInfo(patientInfo);
                    LogInfo($"환자 정보 로드: {patientId}");
                }
                else
                {
                    ClearPatientInfo();
                    LogWarning($"환자 정보를 찾을 수 없음: {patientId}");
                }
            }
            else
            {
                ClearPatientInfo();
            }
        }

        /// <summary>
        /// 환자 정보를 직접 설정합니다 (선택된 환자 데이터 사용)
        /// </summary>
        public void SetPatientInfo(PatientInfoDto patientInfo)
        {
            try
            {
                if (patientInfo == null)
                {
                    ClearPatientInfo();
                    return;
                }

                if (txtPatId != null) txtPatId.Text = patientInfo.PatientId ?? string.Empty;
                if (txtPatName != null) txtPatName.Text = patientInfo.PatientName ?? string.Empty;
                if (txtGender != null) txtGender.Text = GetGenderString(patientInfo.Gender);
                if (txtAge != null) txtAge.Text = CalculateAge(patientInfo.BirthDate) + "세";
                if (txtDeptName != null) txtDeptName.Text = patientInfo.DeptName ?? string.Empty;
                if (txtDoctorName != null) txtDoctorName.Text = patientInfo.DoctorName ?? string.Empty;
                if (txtRoomNo != null) txtRoomNo.Text = patientInfo.RoomNo ?? string.Empty;
                if (txtInDate != null) txtInDate.Text = patientInfo.AdmDate != DateTime.MinValue
                    ? patientInfo.AdmDate.ToString("yyyy-MM-dd")
                    : string.Empty;
                if (txtDiagnosis != null) txtDiagnosis.Text = string.Empty; // DTO에 없는 필드

                LogInfo($"환자 정보 설정 완료: {patientInfo.PatientName} ({patientInfo.PatientId})");
            }
            catch (Exception ex)
            {
                LogError($"SetPatientInfo Error: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// 환자 정보를 초기화합니다
        /// </summary>
        public void ClearPatientInfo()
        {
            if (txtPatId != null) txtPatId.Text = string.Empty;
            if (txtPatName != null) txtPatName.Text = string.Empty;
            if (txtGender != null) txtGender.Text = string.Empty;
            if (txtAge != null) txtAge.Text = string.Empty;
            if (txtDeptName != null) txtDeptName.Text = string.Empty;
            if (txtDoctorName != null) txtDoctorName.Text = string.Empty;
            if (txtRoomNo != null) txtRoomNo.Text = string.Empty;
            if (txtInDate != null) txtInDate.Text = string.Empty;
            if (txtDiagnosis != null) txtDiagnosis.Text = string.Empty;

            _currentPatientId = null;
            LogInfo("환자 정보 초기화");
        }

        /// <summary>
        /// 컨트롤 리셋
        /// </summary>
        public void Reset()
        {
            ClearPatientInfo();
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// 환자 ID로 환자 정보 조회 (데모 데이터)
        /// 실제로는 Repository 패턴을 통해 DB에서 조회
        /// </summary>
        private PatientInfoDto? GetPatientInfoByPatientId(string patientId)
        {
            // 데모 데이터
            var patients = new List<PatientInfoDto>
            {
                new PatientInfoDto
                {
                    PatientId = "P001234",
                    PatientName = "홍길동",
                    Gender = 1,
                    BirthDate = new DateTime(1979, 1, 15),
                    PhoneNumber = "02-1234-5678",
                    MobileNumber = "010-1234-5678",
                    Address = "서울특별시 강남구",
                    InNumber = "I2024001",
                    DeptName = "내과",
                    DoctorName = "김의사",
                    RoomNo = "301",
                    AdmDate = new DateTime(2024, 2, 1),
                    DoctorID = "D001",
                    DeptID = "INT"
                },
                new PatientInfoDto
                {
                    PatientId = "P001235",
                    PatientName = "김철수",
                    Gender = 1,
                    BirthDate = new DateTime(1992, 5, 20),
                    PhoneNumber = "02-2345-6789",
                    MobileNumber = "010-2345-6789",
                    Address = "서울특별시 서초구",
                    InNumber = "I2024002",
                    DeptName = "외과",
                    DoctorName = "이의사",
                    RoomNo = "302",
                    AdmDate = new DateTime(2024, 2, 2),
                    DoctorID = "D002",
                    DeptID = "SUR"
                },
                new PatientInfoDto
                {
                    PatientId = "P001236",
                    PatientName = "이영희",
                    Gender = 2,
                    BirthDate = new DateTime(1996, 8, 10),
                    PhoneNumber = "02-3456-7890",
                    MobileNumber = "010-3456-7890",
                    Address = "서울특별시 송파구",
                    InNumber = "I2024003",
                    DeptName = "산부인과",
                    DoctorName = "박의사",
                    RoomNo = "303",
                    AdmDate = new DateTime(2024, 2, 3),
                    DoctorID = "D003",
                    DeptID = "OBS"
                },
                new PatientInfoDto
                {
                    PatientId = "P001237",
                    PatientName = "박민준",
                    Gender = 1,
                    BirthDate = new DateTime(1968, 12, 5),
                    PhoneNumber = "02-4567-8901",
                    MobileNumber = "010-4567-8901",
                    Address = "서울특별시 강동구",
                    InNumber = "I2024004",
                    DeptName = "내과",
                    DoctorName = "최의사",
                    RoomNo = "304",
                    AdmDate = new DateTime(2024, 2, 4),
                    DoctorID = "D001",
                    DeptID = "INT"
                },
                new PatientInfoDto
                {
                    PatientId = "P001238",
                    PatientName = "정수현",
                    Gender = 2,
                    BirthDate = new DateTime(1983, 3, 25),
                    PhoneNumber = "02-5678-9012",
                    MobileNumber = "010-5678-9012",
                    Address = "서울특별시 관악구",
                    InNumber = "I2024005",
                    DeptName = "정형외과",
                    DoctorName = "정의사",
                    RoomNo = "305",
                    AdmDate = new DateTime(2024, 2, 5),
                    DoctorID = "D004",
                    DeptID = "ORT"
                }
            };

            return patients.FirstOrDefault(p => p.PatientId == patientId);
        }

        /// <summary>
        /// 성별 코드를 문자열로 변환
        /// </summary>
        private string GetGenderString(int genderCode)
        {
            return genderCode switch
            {
                1 => "남",
                2 => "여",
                _ => "알수없음"
            };
        }

        /// <summary>
        /// 생년월일로 나이 계산
        /// </summary>
        private int CalculateAge(DateTime? birthDate)
        {
            if (!birthDate.HasValue || birthDate == DateTime.MinValue)
                return 0;

            var today = DateTime.Now;
            int age = today.Year - birthDate.Value.Year;

            // 생일이 아직 안 지났으면 나이 -1
            if (today.Month < birthDate.Value.Month ||
                (today.Month == birthDate.Value.Month && today.Day < birthDate.Value.Day))
            {
                age--;
            }

            return age;
        }

        #endregion

        #region Button Event Handlers

        private void btnAlergy_Click(object sender, EventArgs e)
        {
            LogInfo("알레르기 정보 팝업 요청");
            XtraMessageBox.Show("알레르기 정보 팝업", "환자정보", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnAlert_Click(object sender, EventArgs e)
        {
            LogInfo("주의사항 정보 팝업 요청");
            XtraMessageBox.Show("주의사항 정보 팝업", "환자정보", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnMedi_Click(object sender, EventArgs e)
        {
            LogInfo("투약정보 팝업 요청");
            XtraMessageBox.Show("투약정보 팝업", "환자정보", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnVital_Click(object sender, EventArgs e)
        {
            LogInfo("생체징후 정보 팝업 요청");
            XtraMessageBox.Show("생체징후 정보 팝업", "환자정보", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        #endregion
    }
}
