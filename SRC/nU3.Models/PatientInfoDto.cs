using System;

namespace nU3.Models
{
    /// <summary>
    /// 환자 기본 정보 DTO
    /// </summary>
    public class PatientInfoDto
    {
        /// <summary>
        /// 환자 ID (Primary Key)
        /// </summary>
        public string PatientId { get; set; }

        /// <summary>
        /// 입원번호
        /// </summary>
        public string InNumber { get; set; }

        /// <summary>
        /// 차트 번호
        /// </summary>
        public string ChartNo { get; set; }

        /// <summary>
        /// 환자 이름
        /// </summary>
        public string PatientName { get; set; }

        /// <summary>
        /// 주민등록번호 (마스킹 처리 가능)
        /// </summary>
        public string ResidentNo { get; set; }

        /// <summary>
        /// 성별 (0:미지정, 1:남, 2:여, 9:기타)
        /// </summary>
        public int Gender { get; set; }

        /// <summary>
        /// 진료과 이름
        /// </summary>
        public string DeptName { get; set; }

        /// <summary>
        /// 담당의 이름
        /// </summary>
        public string DoctorName { get; set; }

        /// <summary>
        /// 병실 번호
        /// </summary>
        public string RoomNo { get; set; }

        /// <summary>
        /// 입원일
        /// </summary>
        public DateTime AdmDate { get; set; }

        /// <summary>
        /// 담당의 ID
        /// </summary>
        public string DoctorID { get; set; }

        /// <summary>
        /// 진료과 ID
        /// </summary>
        public string DeptID { get; set; }

        /// <summary>
        /// 생년월일
        /// </summary>
        public DateTime BirthDate { get; set; }

        /// <summary>
        /// 나이(계산형)
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
        /// 보험 유형 (예: 1:국민건강보험 등)
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
        /// 보호자 전화번호
        /// </summary>
        public string GuardianPhone { get; set; }

        /// <summary>
        /// 비상 연락처
        /// </summary>
        public string EmergencyContact { get; set; }

        /// <summary>
        /// 비상 연락처 관계
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
        /// 현재 복용 중인 약물
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
        /// 환자 상태 (0:대기, 1:진행중, 2:완료, 3:보류, 4:취소, 10:입원, 11:퇴원)
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
        /// 등록일
        /// </summary>
        public DateTime RegisteredDate { get; set; }

        /// <summary>
        /// 등록자
        /// </summary>
        public string RegisteredBy { get; set; }

        /// <summary>
        /// 최종 내원일
        /// </summary>
        public DateTime? LastVisitDate { get; set; }

        /// <summary>
        /// 수정일
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
