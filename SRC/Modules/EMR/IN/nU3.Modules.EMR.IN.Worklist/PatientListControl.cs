using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Columns;
using nU3.Core.UI;
using nU3.Core.Context;
using nU3.Core.Events;
using nU3.Core.Attributes;
using nU3.Models;

namespace nU3.Modules.EMR.IN.Worklist
{
    /// <summary>
    /// 환자 목록 화면 - 이벤트 발행자
    /// 환자 선택 시 다른 모듈로 이벤트 전파
    /// </summary>
    [nU3ProgramInfo(typeof(PatientListControl), "환자 목록", "EMR_IN_00003")]
    public partial class PatientListControl : BaseWorkControl
    {
        // UI 필드는 디자이너 파일로 분리되었습니다.
        private List<PatientInfoDto> _patients;

        public PatientListControl()
        {
            InitializeLayout();
            LoadSampleData();
            LogInfo("PatientListControl 생성됨");
        }

        #region 이벤트 핸들러

        private void GridView_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            if (e.FocusedRowHandle < 0)
                return;

            var selectedPatient = gridView.GetRow(e.FocusedRowHandle) as PatientInfoDto;
            if (selectedPatient != null)
            {
                // 컨텍스트 업데이트
                var newContext = Context.Clone();
                newContext.CurrentPatient = selectedPatient;
                UpdateContext(newContext);

                // 다른 모듈에 환자 선택 이벤트 발행 (약결합 통신)
                PublishPatientSelected(selectedPatient);

                LogInfo($"환자 선택: {selectedPatient.PatientName} ({selectedPatient.PatientId})");
            }
        }

        private void GridView_DoubleClick(object sender, EventArgs e)
        {
            var selectedPatient = GetSelectedPatient();
            if (selectedPatient != null)
            {
                // 환자 상세 화면 열기 요청
                var context = Context.Clone();
                context.CurrentPatient = selectedPatient;
                context.SetParameter("Mode", "View");

                EventBus?.GetEvent<NavigationRequestEvent>()
                    .Publish(new NavigationRequestEventPayload
                    {
                        TargetScreenId = "EMR_IN_00002",
                        Context = context,
                        Source = ProgramID
                    });

                LogInfo($"환자 상세 화면으로 이동 요청: {selectedPatient.PatientName}");
            }
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            var keyword = txtSearch.Text?.Trim();
            if (string.IsNullOrEmpty(keyword))
            {
                gridControl.DataSource = _patients;
                return;
            }

            var filtered = _patients.Where(p =>
                p.PatientName.Contains(keyword) ||
                p.PatientId.Contains(keyword)).ToList();

            gridControl.DataSource = filtered;
            gridView.RefreshData();

            LogInfo($"검색 수행: {keyword}, 결과 수: {filtered.Count}");
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            LoadSampleData();
            txtSearch.Text = string.Empty;
            LogInfo("데이터 새로고침 완료");
        }

        // 내부 로컬 함수으로 정의된 LoadSampleData
        void LoadSampleData()
        {
            var patients = new List<Models.PatientInfoDto>();

            var surnames = new[] { "김", "이", "박", "최", "정", "강", "조", "윤", "장", "임" };
            var givenNames = new[] { "철수", "영희", "민수", "지은", "태영", "수진", "하늘", "지훈", "현우", "유진" };

            for (int i = 1; i <= 40; i++)
            {
                var id = $"P{i:000}";
                var surname = surnames[(i - 1) % surnames.Length];
                var given = givenNames[(i - 1) % givenNames.Length];
                var name = surname + given;

                // Stagger birthdays across years
                var birth = new DateTime(1970 + (i % 30), (i % 12) + 1, ((i * 3) % 27) + 1);

                var gender = (i % 2) == 0 ? 2 : 1; // alternate gender

                var mobile = $"010-{1000 + (i % 9000):D4}-{(2000 + i):D4}";
                var phone = $"02-{100 + (i % 900):D3}-{1000 + i:D4}";

                patients.Add(new Models.PatientInfoDto
                {
                    PatientId = id,
                    PatientName = name,
                    BirthDate = birth,
                    Gender = gender,
                    PhoneNumber = phone,
                    MobileNumber = mobile,
                    Address = $"서울특별시 강남구 예시로 {i}"
                });
            }

            _patients = patients;
            gridControl.DataSource = _patients;
        }
        #endregion

        #region 이벤트 발행

        /// <summary>
        /// 환자 선택 이벤트 발행 (다른 모듈로 전파)
        /// </summary>
        private void PublishPatientSelected(PatientInfoDto patient)
        {
            try
            {
                // EventBus를 통해 다른 모듈에 이벤트 발행
                EventBus?.GetEvent<PatientSelectedEvent>()
                    .Publish(new PatientSelectedEventPayload
                    {
                        Patient = patient,
                        Source = ProgramID
                    });

                LogInfo($"PatientSelectedEvent 발행: {patient.PatientName} ({patient.PatientId})");
                LogAudit(AuditAction.Read, "Patient", patient.PatientId,
                    $"Patient selected and broadcasted to other modules");
            }
            catch (Exception ex)
            {
                LogError($"환자 선택 이벤트 발행 실패", ex);
            }
        }


        #endregion

        #region 라이프사이클

        protected override void OnScreenActivated()
        {
            base.OnScreenActivated();
            LogInfo("PatientListControl 활성화됨");
        }

        protected override void OnScreenDeactivated()
        {
            base.OnScreenDeactivated();
            LogInfo("PatientListControl 비활성화됨");
        }

        protected override void OnContextInitialized(WorkContext oldContext, WorkContext newContext)
        {
            base.OnContextInitialized(oldContext, newContext);
            LogInfo("컨텍스트 초기화됨");
        }

        protected override void OnContextChanged(WorkContext oldContext, WorkContext newContext)
        {
            base.OnContextChanged(oldContext, newContext);
            LogInfo("컨텍스트 변경됨");
        }

        protected override void OnReleaseResources()
        {
            base.OnReleaseResources();

            // 이벤트 구독 해제
            if (gridView != null)
            {
                gridView.FocusedRowChanged -= GridView_FocusedRowChanged;
                gridView.DoubleClick -= GridView_DoubleClick;
            }

            if (btnSearch != null)
                btnSearch.Click -= BtnSearch_Click;

            if (btnRefresh != null)
                btnRefresh.Click -= BtnRefresh_Click;

            LogInfo("리소스 해제됨");
        }

        #endregion

        #region 헬퍼 메서드

        private PatientInfoDto GetSelectedPatient()
        {
            var rowHandle = gridView.FocusedRowHandle;
            if (rowHandle < 0)
                return null;

            return gridView.GetRow(rowHandle) as PatientInfoDto;
        }

        #endregion
    }
}
