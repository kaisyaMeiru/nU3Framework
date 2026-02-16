using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using nU3.Core.Events;
using nU3.Core.UI;
using nU3.Core.Events.Contracts;

namespace nU3.Core.UI.Components.Controls
{
    /// <summary>
    /// 가족력 조회/표시 컨트롤
    /// 환자 선택 이벤트(PatientSelectedEvent)를 수신하여 해당 환자의 가족력을 표시합니다.
    /// </summary>
    [ToolboxItem(true)]
    [Description("환자의 가족력을 조회하고 표시하는 컨트롤입니다.")]
    public partial class FamilyHistoryControl : BaseWorkComponent
    {
        private string? _currentPatientId;

        /// <summary>
        /// 가족력 데이터 모델 (DTO)
        /// </summary>
        public class FamilyHistoryDto
        {
            /// <summary>
            /// 관계 (부, 모, 형제 등)
            /// </summary>
            public string Relation { get; set; } = string.Empty;

            /// <summary>
            /// 진단명 (질환명)
            /// </summary>
            public string DiseaseName { get; set; } = string.Empty;

            /// <summary>
            /// 진단일자
            /// </summary>
            public string DiagnosisDate { get; set; } = string.Empty;

            /// <summary>
            /// 상태 (생존, 사망, 치료중 등)
            /// </summary>
            public string Status { get; set; } = string.Empty;

            /// <summary>
            /// 비고 (메모)
            /// </summary>
            public string Note { get; set; } = string.Empty;
        }

        public FamilyHistoryControl()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            SubscribeToEvents();
        }

        private void SubscribeToEvents()
        {
            if (DesignMode || LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                return;

            if (EventBus == null)
                AssignEventBusFromParent();

            if (EventBus != null)
            {
                EventBus.GetEvent<nU3.Core.Events.Contracts.PatientSelectedEvent>()
                .Subscribe(OnPatientSelected);
                LogInfo("가족력 컨트롤: 환자 선택 이벤트 구독 시작");
            }
            else
            {
                LogWarning("가족력 컨트롤: EventBus를 찾을 수 없어 이벤트를 구독하지 못했습니다.");
            }
        }

        private void OnPatientSelected(PatientSelectedEventPayload payload)
        {
            if (payload?.Patient == null) return;

            if (payload.Source != this.OwnerProgramID)
                return;


            // 중복 호출 방지
            if (_currentPatientId == payload.Patient.PatientId)
                return;

            _currentPatientId = payload.Patient.PatientId;
            LogInfo($"가족력 조회 요청: 환자명={payload.Patient.PatientName}, ID={_currentPatientId}");

            LoadFamilyHistory(_currentPatientId);
        }

        /// <summary>
        /// 가족력 데이터를 로드하고 그리드에 표시합니다.
        /// </summary>
        /// <param name="patientId">환자 ID</param>
        public void LoadFamilyHistory(string? patientId)
        {
            if (string.IsNullOrEmpty(patientId))
            {
                gridControl1.DataSource = null;
                return;
            }

            try
            {
                var historyList = GetDummyFamilyHistory(patientId);
                gridControl1.DataSource = historyList;
                gridView1.RefreshData();
                
                LogInfo($"가족력 데이터 로드 완료: {historyList.Count}건");
            }
            catch (Exception ex)
            {
                LogError($"가족력 로드 중 오류 발생: {ex.Message}", ex);
                XtraMessageBox.Show("가족력 데이터를 불러오는 중 오류가 발생했습니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 덤프(Dummy) 데이터 생성
        /// 실제 DB 연동 전 UI 확인용 데이터입니다.
        /// </summary>
        private List<FamilyHistoryDto> GetDummyFamilyHistory(string patientId)
        {
            var list = new List<FamilyHistoryDto>();

            // 환자 ID에 따라 조금 다른 데이터를 보여주도록 설정 (데모 효과)
            int seed = patientId.GetHashCode();
            var rnd = new Random(seed);

            // 공통 데이터 (부모)
            list.Add(new FamilyHistoryDto 
            { 
                Relation = "부", 
                DiseaseName = "고혈압", 
                DiagnosisDate = "2010-05-20", 
                Status = "치료중", 
                Note = "약물 복용 중" 
            });

            if (rnd.Next(2) == 0)
            {
                list.Add(new FamilyHistoryDto 
                { 
                    Relation = "모", 
                    DiseaseName = "당뇨병", 
                    DiagnosisDate = "2015-11-12", 
                    Status = "관리중", 
                    Note = "식이요법 병행" 
                });
            }

            // 추가 랜덤 데이터
            if (rnd.Next(3) == 0)
            {
                list.Add(new FamilyHistoryDto 
                { 
                    Relation = "조부", 
                    DiseaseName = "위암", 
                    DiagnosisDate = "2005-03-10", 
                    Status = "사망", 
                    Note = "수술 후 경과 관찰 중 사망" 
                });
            }

            if (rnd.Next(3) == 0)
            {
                list.Add(new FamilyHistoryDto 
                { 
                    Relation = "형제", 
                    DiseaseName = "고지혈증", 
                    DiagnosisDate = "2020-01-15", 
                    Status = "진단", 
                    Note = "정기 검진에서 발견" 
                });
            }

            return list;
        }

        /// <summary>
        /// 컨트롤 초기화 (데이터 클리어)
        /// </summary>
        public void Clear()
        {
            _currentPatientId = null;
            gridControl1.DataSource = null;
        }
    }
}
