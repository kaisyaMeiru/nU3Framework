using System;

namespace nU3.Models
{
    /// <summary>
    /// 활력징후 정보 모델
    /// </summary>
    public class VitalSignDto
    {
        /// <summary>
        /// 활력징후 ID (Primary Key)
        /// </summary>
        public string VitalSignId { get; set; }

        /// <summary>
        /// 환자 ID (Foreign Key)
        /// </summary>
        public string PatientId { get; set; }

        /// <summary>
        /// 예약/진료 ID (Foreign Key)
        /// </summary>
        public string AppointmentId { get; set; }

        /// <summary>
        /// 측정일시
        /// </summary>
        public DateTime MeasuredDateTime { get; set; }

        /// <summary>
        /// 수축기 혈압 (mmHg)
        /// </summary>
        public int? SystolicBP { get; set; }

        /// <summary>
        /// 이완기 혈압 (mmHg)
        /// </summary>
        public int? DiastolicBP { get; set; }

        /// <summary>
        /// 맥박 (회/분)
        /// </summary>
        public int? Pulse { get; set; }

        /// <summary>
        /// 호흡수 (회/분)
        /// </summary>
        public int? Respiration { get; set; }

        /// <summary>
        /// 체온 (°C)
        /// </summary>
        public decimal? Temperature { get; set; }

        /// <summary>
        /// 산소포화도 (%)
        /// </summary>
        public int? OxygenSaturation { get; set; }

        /// <summary>
        /// 혈당 (mg/dL)
        /// </summary>
        public int? BloodGlucose { get; set; }

        /// <summary>
        /// 신장 (cm)
        /// </summary>
        public decimal? Height { get; set; }

        /// <summary>
        /// 체중 (kg)
        /// </summary>
        public decimal? Weight { get; set; }

        /// <summary>
        /// BMI (계산값)
        /// </summary>
        public decimal? BMI
        {
            get
            {
                if (Height.HasValue && Weight.HasValue && Height.Value > 0)
                {
                    var heightInMeters = Height.Value / 100;
                    return Math.Round(Weight.Value / (heightInMeters * heightInMeters), 2);
                }
                return null;
            }
        }

        /// <summary>
        /// 통증 척도 (0-10)
        /// </summary>
        public int? PainScale { get; set; }

        /// <summary>
        /// 측정 장소 (병동, 외래 등)
        /// </summary>
        public string MeasuredLocation { get; set; }

        /// <summary>
        /// 측정자 ID
        /// </summary>
        public string MeasuredBy { get; set; }

        /// <summary>
        /// 특이사항
        /// </summary>
        public string Remarks { get; set; }

        /// <summary>
        /// 등록일시
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// 등록자
        /// </summary>
        public string CreatedBy { get; set; }
    }
}
