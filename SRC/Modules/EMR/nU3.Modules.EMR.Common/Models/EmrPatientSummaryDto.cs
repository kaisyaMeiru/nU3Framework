using System;

namespace nU3.Modules.EMR.Common.Models
{
    /// <summary>
    /// EMR 도메인 전반에서 사용되는 환자 요약 정보 DTO
    /// </summary>
    public class EmrPatientSummaryDto
    {
        public string PatientId { get; set; } = string.Empty;
        public string PatientName { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty; // Male, Female
        public DateTime BirthDate { get; set; }
        public string BloodType { get; set; } = string.Empty;
        public string DepartmentCode { get; set; } = string.Empty;
        public string WardCode { get; set; } = string.Empty;
        
        /// <summary>
        /// 만 나이 계산 (읽기 전용)
        /// </summary>
        public int Age
        {
            get
            {
                var today = DateTime.Today;
                var age = today.Year - BirthDate.Year;
                if (BirthDate.Date > today.AddYears(-age)) age--;
                return age;
            }
        }

        public override string ToString()
        {
            return $"{PatientName} ({PatientId}) - {Gender}/{Age}";
        }
    }
}
