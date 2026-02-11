namespace nU3.Core.Contracts.Models
{
    /// <summary>
    /// 선택된 환자 정보를 나타내는 컨텍스트 레코드입니다.
    /// 모듈 간에 환자 관련 상태(예: 환자 ID, 이름, 내원 번호)를 안전하게 전달하기 위해 사용됩니다.
    /// 스마트 컨텍스트 패턴에서 사용되며, 이벤트 페이로드나 작업 컨텍스트의 일부로 전달됩니다.
    /// </summary>
    /// <param name="PatientId">환자를 고유하게 식별하는 ID(예: 병원 등록 번호)</param>
    /// <param name="PatientName">환자의 표시 이름(예: 홍길동)</param>
    /// <param name="VisitNo">현재 내원/접수 번호(선택 항목). 동일 환자의 여러 내원 중 특정 내원을 구분할 때 사용합니다.</param>
    //public record PatientContext(string PatientId, string PatientName, string VisitNo = null);
}
