using System;

namespace nU3.Models
{
    /// <summary>
    /// 알레르기 정보 모델
    /// </summary>
    public class AllergyDto
    {
        /// <summary>
        /// 알레르기 ID (Primary Key)
        /// </summary>
        public string AllergyId { get; set; }

        /// <summary>
        /// 환자 ID (Foreign Key)
        /// </summary>
        public string PatientId { get; set; }

        /// <summary>
        /// 알레르기 원인 물질
        /// </summary>
        public string Allergen { get; set; }

        /// <summary>
        /// 알레르기 유형 (약물, 음식, 환경 등)
        /// </summary>
        public string AllergyType { get; set; }

        /// <summary>
        /// 알레르기 반응 증상
        /// </summary>
        public string Reaction { get; set; }

        /// <summary>
        /// 심각도 (1:Mild, 2:Moderate, 3:Severe, 4:LifeThreatening)
        /// </summary>
        public int Severity { get; set; }

        /// <summary>
        /// 발생일
        /// </summary>
        public DateTime? OnsetDate { get; set; }

        /// <summary>
        /// 활성화 여부
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// 확인 의사 ID
        /// </summary>
        public string ConfirmedDoctorId { get; set; }

        /// <summary>
        /// 확인일시
        /// </summary>
        public DateTime? ConfirmedDate { get; set; }

        /// <summary>
        /// 비고
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

        /// <summary>
        /// 수정일시
        /// </summary>
        public DateTime? ModifiedDate { get; set; }

        /// <summary>
        /// 수정자
        /// </summary>
        public string ModifiedBy { get; set; }
    }
}
