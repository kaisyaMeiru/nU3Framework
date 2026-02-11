using nU3.Core.Events;
using nU3.Core.Contracts.Models;

namespace nU3.Core.Events.Contracts
{
    /// <summary>
    /// 시스템에서 환자가 선택되거나 변경되었음을 브로드캐스트하는 이벤트입니다.
    /// 페이로드로는 선택된 환자 정보가 담긴 PatientContext 레코드를 사용합니다.
    /// 모듈 간에 환자 변경을 통지할 때 사용되며, 구독자는 해당 페이로드를 받아 UI 갱신이나 데이터 로드를 수행할 수 있습니다.
    /// </summary>
    public class PatientSelectedEvent : PubSubEvent<PatientSelectedEventPayload>
    {
    }
}
