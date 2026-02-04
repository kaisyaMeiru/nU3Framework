using nU3.Core.Context;

namespace nU3.Core.Interfaces
{
    /// <summary>
    /// 화면 식별 정보 제공 인터페이스
    /// 화면의 고유 ID와 표시 제목을 제공
    /// </summary>
    public interface IBaseWorkControl
    {
        /// <summary>
        /// 화면의 고유 ID
        /// </summary>
        /// <remarks>
        /// - 시스템 전체에서 유일해야 함
        /// - 메뉴 설정의 ProgId와 매핑됨
        /// - 예: "EMR_PATIENT_REG_001", "ADM_USER_MGMT_001"
        /// </remarks>
        string ProgramID { get; }

        /// <summary>
        /// 화면의 표시 제목
        /// </summary>
        /// <remarks>
        /// - 탭이나 창 제목으로 표시됨
        /// - 사용자에게 친숙한 이름
        /// - 다국어 지원 가능
        /// - 예: "환자 등록", "Patient Registration"
        /// </remarks>
        string ProgramTitle { get; }
    }

  
}
