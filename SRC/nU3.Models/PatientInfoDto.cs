using System;

namespace nU3.Models
{
    /// <summary>
    /// 환자 기본 정보 모델
    /// </summary>
    public class PatientInfoDto
    {
        /// <summary>
        /// 환자 ID (Primary Key)
        /// </summary>
        public string PatientId { get; set; }

        /// <summary>
        /// 차트 번호
        /// </summary>
        public string ChartNo { get; set; }

        /// <summary>
        /// 환자 이름
        /// </summary>
        public string PatientName { get; set; }

        /// <summary>
        /// 주민등록번호 (암호화)
        /// </summary>
        public string ResidentNo { get; set; }

        /// <summary>
        /// 성별 (0:Unspecified, 1:Male, 2:Female, 9:Other)
        /// </summary>
        public int Gender { get; set; }

        /// <summary>
        /// 생년월일
        /// </summary>
        public DateTime BirthDate { get; set; }

        /// <summary>
        /// 나이 (계산)
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

        /// <summary>
        /// 혈액형 (0:Unknown, 1:A+, 2:A-, 3:B+, 4:B-, 5:O+, 6:O-, 7:AB+, 8:AB-)
        /// </summary>
        public int BloodType { get; set; }

        /// <summary>
        /// 전화번호
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// 휴대전화
        /// </summary>
        public string MobileNumber { get; set; }

        /// <summary>
        /// 이메일
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 우편번호
        /// </summary>
        public string PostalCode { get; set; }

        /// <summary>
        /// 주소
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 상세 주소
        /// </summary>
        public string AddressDetail { get; set; }

        /// <summary>
        /// 보험 유형 (1:건강보험, 2:의료급여, 3:산재, 4:자동차, 5:실손, 6:보훈, 99:비급여)
        /// </summary>
        public int InsuranceType { get; set; }

        /// <summary>
        /// 보험 번호
        /// </summary>
        public string InsuranceNo { get; set; }

        /// <summary>
        /// 보호자 이름
        /// </summary>
        public string GuardianName { get; set; }

        /// <summary>
        /// 보호자 관계
        /// </summary>
        public string GuardianRelation { get; set; }

        /// <summary>
        /// 보호자 연락처
        /// </summary>
        public string GuardianPhone { get; set; }

        /// <summary>
        /// 비상연락처
        /// </summary>
        public string EmergencyContact { get; set; }

        /// <summary>
        /// 비상연락처 관계
        /// </summary>
        public string EmergencyRelation { get; set; }

        /// <summary>
        /// 알레르기 정보
        /// </summary>
        public string Allergies { get; set; }

        /// <summary>
        /// 만성질환 정보
        /// </summary>
        public string ChronicDiseases { get; set; }

        /// <summary>
        /// 복용 중인 약물
        /// </summary>
        public string CurrentMedications { get; set; }

        /// <summary>
        /// 흡연 여부
        /// </summary>
        public bool IsSmoker { get; set; }

        /// <summary>
        /// 음주 여부
        /// </summary>
        public bool IsDrinker { get; set; }

        /// <summary>
        /// 환자 상태 (0:Waiting, 1:InProgress, 2:Completed, 3:OnHold, 4:Cancelled, 10:Admitted, 11:Discharged)
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// VIP 여부
        /// </summary>
        public bool IsVIP { get; set; }

        /// <summary>
        /// 사망 여부
        /// </summary>
        public bool IsDeceased { get; set; }

        /// <summary>
        /// 사망일
        /// </summary>
        public DateTime? DeceasedDate { get; set; }

        /// <summary>
        /// 최초 등록일
        /// </summary>
        public DateTime RegisteredDate { get; set; }

        /// <summary>
        /// 최초 등록자
        /// </summary>
        public string RegisteredBy { get; set; }

        /// <summary>
        /// 마지막 방문일
        /// </summary>
        public DateTime? LastVisitDate { get; set; }

        /// <summary>
        /// 수정일시
        /// </summary>
        public DateTime? ModifiedDate { get; set; }

        /// <summary>
        /// 수정자
        /// </summary>
        public string ModifiedBy { get; set; }

        /// <summary>
        /// 비고
        /// </summary>
        public string Remarks { get; set; }
    }
}
