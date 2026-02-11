using System;

namespace nU3.Models
{
    /// <summary>
    /// 부서 정보 DTO
    /// SYS_DEPT 테이블과 매핑됩니다.
    /// </summary>
    public class DepartmentDto
    {
        /// <summary>
        /// 부서 코드 (Primary Key)
        /// Department enum 값 (1~18)
        /// </summary>
        public string DeptCode { get; set; }

        /// <summary>
        /// 부서 이름 (한글)
        /// </summary>
        public string DeptName { get; set; }

        /// <summary>
        /// 부서 영문명
        /// Department enum의 멤버 이름
        /// </summary>
        public string? DeptNameEng { get; set; }

        /// <summary>
        /// 부서 설명
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// 화면 표시 순서
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// 상위 부서 코드
        /// 계층 구조 지원 (현재는 미사용, NULL)
        /// </summary>
        public string? ParentDept { get; set; }

        /// <summary>
        /// 활성화 상태 (Y/N)
        /// </summary>
        public string IsActive { get; set; } = "Y";

        /// <summary>
        /// 생성일시
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// 수정일시
        /// </summary>
        public DateTime? ModifiedDate { get; set; }

        /// <summary>
        /// ListBox 등에서 표시될 문자열
        /// "[코드] 부서명" 형식
        /// </summary>
        public string DisplayText => $"[{DeptCode}] {DeptName}";

        public override string ToString() => DisplayText;
    }
}
