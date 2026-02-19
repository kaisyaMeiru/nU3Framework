using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using nU3.Core.UI.Components.Events;
using nU3.Core.UI;
using nU3.Models;
using nU3.Core.Events.Contracts;
using nU3.Core.Events;
using nU3.Core.Enums;
using PatientSelectedEvent = nU3.Core.Events.PatientSelectedEvent;
using System.ComponentModel.DataAnnotations;
using DevExpress.XtraGrid.Views.Base;

namespace nU3.Core.UI.Components.Controls
{
    /// <summary>
    /// 입원환자 목록 컨트롤 - 범용 컴포넌트
    /// DataTable과 PatientInfoDto 모두 지원
    /// EventBus를 통해 이벤트 전파 지원
    /// </summary>
    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(PatientListControl))]
    public partial class PatientListControl : BaseWorkComponent
    {
        private BindingList<PatientInfoDto> _patients = new();
        private PatientInfoDto? _selectedPatient;

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
            set => base.EventSource = value ?? nameof(PatientListControl);
        }

        /// <summary>
        /// 환자 선택 이벤트 - DTO 기반
        /// </summary>
        public event EventHandler<PatientSelectedEventArgs>? PatientSelected;

        /// <summary>
        /// 환자 선택 이벤트를 EventBus와 인스턴스 이벤트 모두 전파
        /// </summary>
        public void PublishPatientSelectedEvent(PatientInfoDto patient)
        {
            if (patient == null) return;

            // EventBus 전파
            if (EventBusUse && EventBus != null)
            {

                // EventBus를 통해 다른 모듈에 이벤트 발행
                var evenPub = EventBus?.GetEvent < nU3.Core.Events.Contracts.PatientSelectedEvent > ();
                evenPub?.Publish(new PatientSelectedEventPayload
                {
                        Patient = patient,
                        Source = this.OwnerProgramID,                    
                });

                LogInfo($"PatientSelectedEvent 발행: {patient.PatientName} ({patient.PatientId})");                
            }

            // 인스턴스 이벤트 발생            
            PatientSelected?.Invoke(this, new PatientSelectedEventArgs
            {
                Patient = patient,
                Source = EventSource
            });            

        }

        /// <summary>
        /// 선택된 환자 정보 (DTO)
        /// </summary>
        [Category("Data")]
        [Browsable(false)]
        public PatientInfoDto? SelectedPatient
        {
            get => _selectedPatient;
        }

        #region 하위 호환성을 위한 레거시 프로퍼티들
        // OCS 모듈에서 기존에 사용하던 Selected* 프로퍼티 유지
        [Category("Data")]
        [Browsable(false)]
        public string SelectedInNumber => _selectedPatient?.PatientId ?? string.Empty;

        [Category("Data")]
        [Browsable(false)]
        public string SelectedPatiNumber => _selectedPatient?.PatientId ?? string.Empty;

        [Category("Data")]
        [Browsable(false)]
        public string SelectedPatiName => _selectedPatient?.PatientName ?? string.Empty;

        [Category("Data")]
        [Browsable(false)]
        public string SelectedGender => _selectedPatient?.Gender switch
        {
            1 => "남",
            2 => "여",
            _ => "알수없음"
        };

        [Category("Data")]
        [Browsable(false)]
        public int SelectedAge => _selectedPatient?.BirthDate != null
            ? DateTime.Now.Year - _selectedPatient.BirthDate.Year
            : 0;

        [Category("Data")]
        [Browsable(false)]
        public string SelectedDeptName => string.Empty; // DTO에 없는 필드, 필요시 확장

        [Category("Data")]
        [Browsable(false)]
        public string SelectedDoctorName => string.Empty; // DTO에 없는 필드, 필요시 확장

        [Category("Data")]
        [Browsable(false)]
        public string SelectedRoomNo => string.Empty; // DTO에 없는 필드, 필요시 확장

        [Category("Data")]
        [Browsable(false)]
        public DateTime SelectedAdmDate => DateTime.MinValue; // DTO에 없는 필드, 필요시 확장

        [Category("Data")]
        [Browsable(false)]
        public string SelectedDoctorID => string.Empty; // DTO에 없는 필드, 필요시 확장

        [Category("Data")]
        [Browsable(false)]
        public string SelectedDeptID => string.Empty; // DTO에 없는 필드, 필요시 확장
        
        #endregion

        public PatientListControl()
        {
            InitializeComponent();

            EventBusUse = true;

            this.Load += (s, e) => LoadDemoData();  
        }

        /// <summary>
        /// 데모 데이터 로드 (개발/테스트용)
        /// </summary>
        private void LoadDemoData()
        {
            // 디자인 타임 체크
            if (DesignMode || LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                return;

            // gridControl null 체크
            if (gridControl == null)
                return;

            try
            {
                // PatientInfoDto 리스트 생성
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

                SetPatients(patients);
            }
            catch (Exception ex)
            {
                // 디자인 타임에서는 예외 무시
                System.Diagnostics.Debug.WriteLine($"LoadDemoData Error: {ex.Message}");
            }
        }

        /// <summary>
        /// 환자 목록 설정 (PatientInfoDto 기반)
        /// </summary>
        public void SetPatients(IEnumerable<PatientInfoDto> patients)
        {
            _patients.Clear();
            foreach (var patient in patients)
            {
                _patients.Add(patient);
            }

            if (gridControl != null)
            {
                gridControl.DataSource = _patients;
            }
        }

        /// <summary>
        /// 환자 목록 설정 (DataTable 기반 - 하위 호환성)
        /// </summary>
        public void SetDataTable(DataTable dataTable)
        {
            if (dataTable == null)
                return;

            var patients = new List<PatientInfoDto>();

            foreach (DataRow row in dataTable.Rows)
            {
                int age = 0;
                bool hasAgeColumn = row.Table.Columns.Contains("Age") && int.TryParse(row["Age"]?.ToString(), out age);

                var patient = new PatientInfoDto
                {
                    PatientId = row.Table.Columns.Contains("PatiNumber") ? row["PatiNumber"]?.ToString() ?? "" : "",
                    PatientName = row.Table.Columns.Contains("PatiName") ? row["PatiName"]?.ToString() ?? "" : "",
                    Gender = row.Table.Columns.Contains("Gender") && row["Gender"]?.ToString() == "남" ? 1 : 2,
                    BirthDate = hasAgeColumn && age > 0
                        ? DateTime.Now.AddYears(-age)
                        : DateTime.Now,
                    InNumber = row.Table.Columns.Contains("InNumber") ? row["InNumber"]?.ToString() ?? "" : "",
                    DeptName = row.Table.Columns.Contains("DeptName") ? row["DeptName"]?.ToString() ?? "" : "",
                    DoctorName = row.Table.Columns.Contains("DoctorName") ? row["DoctorName"]?.ToString() ?? "" : "",
                    RoomNo = row.Table.Columns.Contains("RoomNo") ? row["RoomNo"]?.ToString() ?? "" : "",
                    AdmDate = row.Table.Columns.Contains("AdmDate") && row["AdmDate"] != DBNull.Value
                        ? Convert.ToDateTime(row["AdmDate"])
                        : DateTime.Now,
                    DoctorID = row.Table.Columns.Contains("DoctorID") ? row["DoctorID"]?.ToString() ?? "" : "",
                    DeptID = row.Table.Columns.Contains("DeptID") ? row["DeptID"]?.ToString() ?? "" : "",
                };

                patients.Add(patient);
            }

            SetPatients(patients);
        }

        /// <summary>
        /// 선택된 환자 정보 가져오기
        /// </summary>
        public PatientInfoDto? GetSelectedPatient()
        {
            return _selectedPatient;
        }

        /// <summary>
        /// 데이터 새로고침
        /// </summary>
        public void Refresh()
        {
            LoadDemoData();
        }

        private void gridView_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            // 디자인 타임 체크
            if (DesignMode || LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                return;

            // null 체크
            if (gridView == null || gridView.FocusedRowHandle < 0)
                return;

            try
            {
                _selectedPatient = gridView.GetRow(gridView.FocusedRowHandle) as PatientInfoDto;

                // EventBus 전파 (명시적인 메서드 사용)
                if (_selectedPatient != null)
                {
                    TriggerPatientSelectedEvent(_selectedPatient);
                }
            }
            catch (Exception ex)
            {
                LogError($"gridView_FocusedRowChanged Error: {ex.Message}", ex);
            }
        }

        private void gridView_DoubleClick(object sender, EventArgs e)
        {
            //// 디자인 타임 체크
            //if (DesignMode || LicenseManager.UsageMode == LicenseUsageMode.Designtime)
            //    return;

            //// null 체크
            //if (gridView == null || gridView.FocusedRowHandle < 0)
            //    return;

            //try
            //{
            //    _selectedPatient = gridView.GetRow(gridView.FocusedRowHandle) as PatientInfoDto;

            //    if (_selectedPatient != null)
            //    {
            //        // EventBus 전파 (명시적인 메서드 사용)
            //        PublishPatientSelectedEvent(_selectedPatient);

            //        // 더블클릭 시 환자 선택 이벤트 발생
            //        var eventArgs = new PatientSelectedEventArgs
            //        {
            //            Patient = _selectedPatient,
            //            Source = EventSource
            //        };

            //        // 인스턴스 기반 이벤트 발생
            //        PatientSelected?.Invoke(this, eventArgs);

            //        // 하위 호환성을 위한 레거시 이벤트 발생
            //        OnPatientSelected?.Invoke(this, EventArgs.Empty);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    LogError($"gridView_DoubleClick Error: {ex.Message}", ex);
            //}
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            // 디자인 타임 체크
            if (DesignMode || LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                return;

            // 데이터 새로고침
            LoadDemoData();
        }

        /// <summary>
        /// 컬럼 표시 텍스트 커스텀 (성별 코드를 문자열로 변환)
        /// </summary>
        private void gridView_CustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e)
        {
            if (DesignMode || LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                return;

            if (e == null || gridView == null || e.Column == null)
                return;

            if (e.Column.FieldName == "Gender" && e.Value != null)
            {
                try
                {
                    if (int.TryParse(e.Value.ToString(), out int genderCode))
                    {
                        Gender gender = (Gender)genderCode;

                        var displayAttribute = gender.GetType()
                            .GetField(gender.ToString())
                            ?.GetCustomAttributes(typeof(DisplayAttribute), false)
                            .FirstOrDefault() as DisplayAttribute;

                        if (displayAttribute != null && !string.IsNullOrEmpty(displayAttribute.Name))
                        {
                            e.DisplayText = displayAttribute.Name;
                        }
                        else
                        {
                            e.DisplayText = genderCode.ToString();
                        }
                    }
                    else
                    {
                        e.DisplayText = e.Value.ToString() ?? string.Empty;
                    }
                }
                catch (Exception ex)
                {
                    LogError($"CustomColumnDisplayText Error: {ex.Message}", ex);
                    e.DisplayText = e.Value.ToString() ?? string.Empty;
                }
            }
        }

        /// <summary>
        /// PatientSelected 이벤트를 EventBus와 인스턴스 이벤트 모두 전파
        /// </summary>
        public void TriggerPatientSelectedEvent(PatientInfoDto patient)
        {
            PublishPatientSelectedEvent(patient);
        }
    }
}
