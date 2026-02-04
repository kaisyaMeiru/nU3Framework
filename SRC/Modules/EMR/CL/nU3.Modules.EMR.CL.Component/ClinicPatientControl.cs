using System;
using System.Collections.Generic;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using nU3.Models;
using nU3.Core.UI.Components.Events;

namespace nU3.Modules.EMR.CL.Component
{
    public partial class ClinicPatientControl : XtraUserControl
    {
        private PatientInfoDto? _currentPatient;
        private readonly List<PatientInfoDto> _mockPatients = new();
        private bool _isUpdatingSelection = false;

        public event EventHandler<PatientSelectedEventArgs>? PatientSelected;

        public ClinicPatientControl()
        {
            InitializeComponent();
            LoadMockData();
        }

        private void LoadMockData()
        {
            var surnames = new[] { "김", "이", "박", "최", "정", "한", "조", "윤", "장", "임" };
            var givenNames = new[] { "철수", "영희", "민수", "지현", "진우", "서연", "현우", "예진", "준호", "수빈" };

            for (int i = 1; i <= 50; i++)
            {
                var id = $"P{i:000}";
                var surname = surnames[(i - 1) % surnames.Length];
                var given = givenNames[(i - 1) % givenNames.Length];
                var name = surname + given;

                var birth = new DateTime(1970 + (i % 30), (i % 12) + 1, ((i * 3) % 27) + 1);
                var gender = (i % 2) == 0 ? 2 : 1;
                var mobile = $"010-{1000 + (i % 9000):D4}-{(2000 + i):D4}";

                _mockPatients.Add(new PatientInfoDto
                {
                    PatientId = id,
                    PatientName = name,
                    BirthDate = birth,
                    Gender = gender,
                    PhoneNumber = $"02-{100 + (i % 900):D3}-{1000 + i:D4}",
                    MobileNumber = mobile,
                    Address = $"서울특별시 강남구 테스트로 {i}"
                });
            }

            _patientSelector?.SetPatients(_mockPatients);
        }

        #region 이벤트 핸들러 패턴: UI Component 이벤트 처리

        private void OnPatientSelected(object? sender, PatientSelectedEventArgs e)
        {
            if (_isUpdatingSelection)
                return;

            try
            {
                _isUpdatingSelection = true;

                _currentPatient = e.Patient;

                if (_currentPatient != null)
                {
                    UpdateSelectedPatientInfo(_currentPatient);

                    PatientSelected?.Invoke(this, new PatientSelectedEventArgs
                    {
                        Patient = _currentPatient,
                        Source = "ClinicPatientControl"
                    });

                    LogInfo($"환자 선택됨: {_currentPatient.PatientName}");
                }
            }
            finally
            {
                _isUpdatingSelection = false;
            }
        }

        private void OnClear(object? sender, EventArgs e)
        {
            _currentPatient = null;

            if (_lblSelectedPatient != null)
            {
                _lblSelectedPatient.Text = "선택된 환자: 없음";
                _lblSelectedPatient.Appearance.ForeColor = System.Drawing.Color.Gray;
            }

            _patientSelector?.ClearSelection();

            PatientSelected?.Invoke(this, new PatientSelectedEventArgs
            {
                Patient = null,
                Source = "ClinicPatientControl"
            });

            LogInfo("환자 선택 초기화됨");
        }

        #endregion

        #region 공용 메서드

        public void SetSelectedPatient(PatientInfoDto? patient)
        {
            _currentPatient = patient;

            if (patient != null)
            {
                UpdateSelectedPatientInfo(patient);
            }
            else
            {
                if (_lblSelectedPatient != null)
                {
                    _lblSelectedPatient.Text = "선택된 환자: 없음";
                    _lblSelectedPatient.Appearance.ForeColor = System.Drawing.Color.Gray;
                }
            }
        }

        private void UpdateSelectedPatientInfo(PatientInfoDto patient)
        {
            if (_lblSelectedPatient == null) return;

            var genderText = patient.Gender switch
            {
                1 => "남",
                2 => "여",
                _ => "알수없음"
            };

            _lblSelectedPatient.Text =
                $"환자ID: {patient.PatientId} | " +
                $"이름: {patient.PatientName} | " +
                $"생년월일: {patient.BirthDate:yyyy-MM-dd} | " +
                $"성별: {genderText} | " +
                $"연락처: {patient.MobileNumber}";

            _lblSelectedPatient.Appearance.ForeColor = System.Drawing.Color.DarkBlue;
        }

        #endregion

        private void LogInfo(string message)
        {
            System.Diagnostics.Debug.WriteLine($"[ClinicPatientControl] {message}");
        }
    }
}
