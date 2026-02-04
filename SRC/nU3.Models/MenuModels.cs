using System;

namespace nU3.Models
{
    /// <summary>
    /// 메뉴 정보를 나타내는 DTO입니다.
    /// 데이터베이스 또는 구성에서 읽은 메뉴 항목을 표현합니다.
    /// </summary>
    public class MenuDto
    {
        /// <summary>메뉴 고유 ID</summary>
        public string MenuId { get; set; }
        /// <summary>부모 메뉴 ID (루트인 경우 null 또는 빈 문자열)</summary>
        public string ParentId { get; set; }
        /// <summary>화면에 표시될 메뉴 이름</summary>
        public string MenuName { get; set; }
        /// <summary>연결된 프로그램/화면의 ProgId</summary>
        public string ProgId { get; set; }
        /// <summary>정렬 순서</summary>
        public int SortOrd { get; set; }
        /// <summary>이 메뉴를 볼 수 있는 권한 레벨</summary>
        public int AuthLevel { get; set; }
    }

    /// <summary>
    /// 프로그램(화면) 정보를 나타내는 DTO입니다.
    /// 모듈 로더나 메뉴와 연동하여 사용됩니다.
    /// </summary>
    public class ProgramDto
    {
        /// <summary>프로그램 식별자 (ProgId)</summary>
        public string ProgId { get; set; }
        /// <summary>소속 모듈 ID</summary>
        public string ModuleId { get; set; }
        /// <summary>실제 클래스(타입) 이름</summary>
        public string ClassName { get; set; }
        /// <summary>프로그램 표시 이름</summary>
        public string ProgName { get; set; }
        /// <summary>프로그램 접근을 위한 권한 레벨</summary>
        public int AuthLevel { get; set; }
        /// <summary>활성화 여부 (Y/N)</summary>
        public string IsActive { get; set; } = "Y";
        /// <summary>프로그램 타입 (기본값 1)</summary>
        public int ProgType { get; set; } = 1;
        
        // 추가 검증용 필드
        /// <summary>시스템 타입 (예: EMR, ADM 등)</summary>
        public string SystemType { get; set; }
        /// <summary>서브시스템 또는 카테고리</summary>
        public string SubSystem { get; set; }
        /// <summary>관련 DLL 이름 (모듈 파일명)</summary>
        public string DllName { get; set; }
    }
}
