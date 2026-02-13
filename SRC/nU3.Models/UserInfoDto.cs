using System;

namespace nU3.Models
{
    /// <summary>
    /// 사용자 정보 모델
    /// </summary>
    public class UserInfoDto
    {
        /// <summary>
        /// 사용자 ID (Primary Key)
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// 사용자 이름
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 비밀번호 (해시)
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 사용자 역할 (0:Admin, 1:Tech, 10:Doctor, 11:Nurse, 100:Patient)
        /// </summary>
        public int UserRole { get; set; }

        /// <summary>
        /// 사용자 역할 코드 (SYS_ROLE.ROLE_CODE)
        /// </summary>
        public string RoleCode { get; set; }

        /// <summary>
        /// 권한 레벨 (1-9)
        /// </summary>
        public int AuthLevel { get; set; }

        /// <summary>
        /// 사용자 권한 플래그 (Flags: 1:Read, 2:Write, 4:Edit, 8:Delete, 16:Access)
        /// </summary>
        public int UserAuth { get; set; }

        /// <summary>
        /// 접속한 시설 코드 (012:서울, 015:은평, 011:여의도)
        /// </summary>
        public string InstitutionCode { get; set; }

        /// <summary>
        /// 소속 부서 코드
        /// </summary>
        public int? DepartmentCode { get; set; }

        /// <summary>
        /// 직원 번호
        /// </summary>
        public string EmployeeNo { get; set; }

        /// <summary>
        /// 의사 면허번호 (의사인 경우)
        /// </summary>
        public string LicenseNo { get; set; }

        /// <summary>
        /// 전문의 자격 (의사인 경우)
        /// </summary>
        public string Specialty { get; set; }

        /// <summary>
        /// 이메일
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 전화번호
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// 활성화 여부
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// 계정 잠김 여부
        /// </summary>
        public bool IsLocked { get; set; }

        /// <summary>
        /// 마지막 로그인 일시
        /// </summary>
        public DateTime? LastLoginDate { get; set; }

        /// <summary>
        /// 로그인 실패 횟수
        /// </summary>
        public int LoginFailCount { get; set; }

        /// <summary>
        /// 비밀번호 변경일
        /// </summary>
        public DateTime? PasswordChangedDate { get; set; }

        /// <summary>
        /// 비밀번호 만료일
        /// </summary>
        public DateTime? PasswordExpiryDate { get; set; }

        /// <summary>
        /// 생성일시
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// 생성자
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

        /// <summary>
        /// 비고
        /// </summary>
        public string Remarks { get; set; }
    }
}
