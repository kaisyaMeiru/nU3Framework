using System;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using nU3.Models;
using nU3.Core.UI.Components.Events;

namespace nU3.Modules.EMR.CL.Component
{
    public partial class ClinicVisitControl : XtraUserControl
    {
        private PatientInfoDto? _currentPatient;
        private readonly List<VisitRecord> _mockVisitRecords = new();

        public event EventHandler<VisitRecordedEventArgs>? VisitRecorded;

        public ClinicVisitControl()
        {
            InitializeComponent();
            AddDefaultSymptoms();
            LoadMockVisitRecords();
            SetControlsEnabled(false);
        }

        private void AddDefaultSymptoms()
        {
            if (_checklistControl == null) return;

            string[] symptoms = new string[]
            {
                "발열",
                "기침",
                "두통",
                "인후통",
                "근육통",
                "피로감",
                "식욕부진",
                "구토",
                "설사",
                "호흡곤란"
            };
            _checklistControl.AddItems(symptoms);
        }

        private void LoadMockVisitRecords()
        {
            for (int i = 1; i <= 20; i++)
            {
                _mockVisitRecords.Add(new VisitRecord
                {
                    VisitId = $"V{i:000}",
                    PatientId = $"P{(i % 10) + 1:000}",
                    VisitDate = DateTime.Today.AddDays(-i),
                    VisitType = "외래",
                    ChiefComplaint = "두통 및 발열",
                    Symptoms = new[] { "발열", "두통", "기침" },
                    Notes = "3일 전부터 시작된 증상"
                });
            }
        }

        #region 이벤트 핸들러 패턴: UI Component 이벤트 처리

        private void OnDateRangeChanged(object? sender, EventArgs e)
        {
            if (_dateRangeControl == null) return;

            var range = _dateRangeControl.GetDateRange();
            LogInfo($"날짜 범위 변경: {range.StartDate:yyyy-MM-dd} ~ {range.EndDate:yyyy-MM-dd}");

            if (_currentPatient != null)
            {
                LoadVisitRecords(_currentPatient.PatientId, showNotification: false);
            }
        }

        private void OnCheckStateChanged(object? sender, ChecklistChangedEventArgs e)
        {
            LogInfo($"체크리스트 상태 변경: {e.CheckedCount}/{e.TotalCount} 항목 체크됨");
        }

        private void OnLoadVisitRecords(object? sender, EventArgs e)
        {
            if (_currentPatient != null)
            {
                LoadVisitRecords(_currentPatient.PatientId, showNotification: true);
            }
            else
            {
                XtraMessageBox.Show("환자를 먼저 선택하세요.", "알림",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void OnSaveVisitRecord(object? sender, EventArgs e)
        {
            if (_currentPatient == null)
            {
                XtraMessageBox.Show("환자를 먼저 선택하세요.", "알림",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_dateRangeControl == null || _checklistControl == null)
                return;

            var range = _dateRangeControl.GetDateRange();
            var checkedSymptoms = _checklistControl.GetCheckedValues().ToArray();

            var record = new VisitRecord
            {
                VisitId = $"V{DateTime.Now:yyyyMMddHHmmss}",
                PatientId = _currentPatient.PatientId,
                VisitDate = range.StartDate ?? DateTime.Today,
                VisitType = "외래",
                ChiefComplaint = _txtChiefComplaint?.Text ?? "",
                Symptoms = checkedSymptoms,
                Notes = _txtNotes?.Text ?? ""
            };

            VisitRecorded?.Invoke(this, new VisitRecordedEventArgs
            {
                VisitId = record.VisitId,
                PatientId = record.PatientId,
                PatientName = _currentPatient.PatientName,
                VisitType = record.VisitType,
                VisitDate = record.VisitDate
            });

            _mockVisitRecords.Add(record);

            XtraMessageBox.Show($"진료 기록이 저장되었습니다.\n진료ID: {record.VisitId}", "완료",
                MessageBoxButtons.OK, MessageBoxIcon.Information);

            LogInfo($"진료 기록 저장됨: {record.VisitId}");
        }

        #endregion

        #region 공용 메서드

        public void SetCurrentPatient(PatientInfoDto? patient)
        {
            _currentPatient = patient;

            if (patient != null)
            {
                if (_lblPatientInfo != null)
                {
                    _lblPatientInfo.Text =
                        $"환자ID: {patient.PatientId} | " +
                        $"이름: {patient.PatientName} | " +
                        $"생년월일: {patient.BirthDate:yyyy-MM-dd}";

                    _lblPatientInfo.Appearance.ForeColor = System.Drawing.Color.DarkBlue;
                }

                SetControlsEnabled(true);
                LoadVisitRecords(patient.PatientId, showNotification: false);
            }
            else
            {
                if (_lblPatientInfo != null)
                {
                    _lblPatientInfo.Text = "선택된 환자가 없습니다.";
                    _lblPatientInfo.Appearance.ForeColor = System.Drawing.Color.Gray;
                }

                SetControlsEnabled(false);
                ClearForm();
            }
        }

        public void LoadVisitRecords(string? patientId, bool showNotification = false)
        {
            if (string.IsNullOrEmpty(patientId))
                return;

            if (_dateRangeControl == null)
                return;

            var range = _dateRangeControl.GetDateRange();
            var records = _mockVisitRecords
                .Where(r => r.PatientId == patientId)
                .Where(r => r.VisitDate >= (range.StartDate ?? DateTime.MinValue))
                .Where(r => r.VisitDate <= (range.EndDate ?? DateTime.MaxValue))
                .ToList();

            LogInfo($"진료 기록 조회: {records.Count}건 ({patientId})");

            if (records.Count > 0)
            {
                var latest = records.OrderByDescending(r => r.VisitDate).First();

                _txtChiefComplaint!.Text = latest.ChiefComplaint;
                _txtNotes!.Text = latest.Notes;

                _checklistControl!.UncheckAll();
                foreach (var symptom in latest.Symptoms)
                {
                    _checklistControl.CheckByValue(symptom);
                }

                if (showNotification)
                {
                    XtraMessageBox.Show($"{records.Count}건의 진료 기록을 조회했습니다.", "완료",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                if (showNotification)
                {
                    XtraMessageBox.Show("검색된 진료 기록이 없습니다.", "알림",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                ClearForm();
            }
        }

        private void SetControlsEnabled(bool enabled)
        {
            _dateRangeControl!.Enabled = enabled;
            _checklistControl!.Enabled = enabled;
            _txtChiefComplaint!.Enabled = enabled;
            _txtNotes!.Enabled = enabled;
            _btnLoad!.Enabled = enabled;
            _btnSave!.Enabled = enabled;
        }

        private void ClearForm()
        {
            _txtChiefComplaint!.Text = "";
            _txtNotes!.Text = "";
            _checklistControl!.UncheckAll();

            if (_dateRangeControl != null)
            {
                _dateRangeControl.SetDateRange(DateTime.Today, DateTime.Today);
            }
        }

        #endregion

        private void LogInfo(string message)
        {
            System.Diagnostics.Debug.WriteLine($"[ClinicVisitControl] {message}");
        }
    }

    #region 내부 모델

    public class VisitRecord
    {
        public string VisitId { get; set; } = string.Empty;
        public string PatientId { get; set; } = string.Empty;
        public DateTime VisitDate { get; set; }
        public string VisitType { get; set; } = string.Empty;
        public string ChiefComplaint { get; set; } = string.Empty;
        public string[] Symptoms { get; set; } = Array.Empty<string>();
        public string Notes { get; set; } = string.Empty;
    }

    #endregion
}