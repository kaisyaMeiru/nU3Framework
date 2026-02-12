using nU3.Models;

namespace nU3.Core.Events
{
    /// <summary>
    /// 환자 선택 이벤트
    /// Payload: PatientSelectedEventPayload
    /// </summary>
    public class PatientSelectedEvent : PubSubEvent { }

    /// <summary>
    /// 환자 선택 이벤트 페이로드
    /// </summary>
    public class PatientSelectedEventPayload
    {
        public PatientInfoDto Patient { get; set; }
        public string Source { get; set; }

        public List<string> Target { get; set; } = new List<string>();
    }

    /// <summary>
    /// 환자 정보 업데이트 이벤트
    /// Payload: PatientUpdatedEventPayload
    /// </summary>
    public class PatientUpdatedEvent : PubSubEvent { }

    public class PatientUpdatedEventPayload
    {
        public PatientInfoDto Patient { get; set; }
        public string Source { get; set; }
        public string UpdatedBy { get; set; }
    }

    /// <summary>
    /// 검사 선택 이벤트
    /// Payload: ExamSelectedEventPayload
    /// </summary>
    public class ExamSelectedEvent : PubSubEvent { }

    public class ExamSelectedEventPayload
    {
        public ExamOrderDto Exam { get; set; }
        public PatientInfoDto Patient { get; set; }
        public string Source { get; set; }
    }

    /// <summary>
    /// 검사 결과 업데이트 이벤트
    /// Payload: ExamResultUpdatedEventPayload
    /// </summary>
    public class ExamResultUpdatedEvent : PubSubEvent { }

    public class ExamResultUpdatedEventPayload
    {
        public ExamResultDto Result { get; set; }
        public string Source { get; set; }
        public string UpdatedBy { get; set; }
    }

    /// <summary>
    /// 예약 선택 이벤트
    /// Payload: AppointmentSelectedEventPayload
    /// </summary>
    public class AppointmentSelectedEvent : PubSubEvent { }

    public class AppointmentSelectedEventPayload
    {
        public AppointmentDto Appointment { get; set; }
        public PatientInfoDto Patient { get; set; }
        public string Source { get; set; }
    }

    /// <summary>
    /// 작업 컨텍스트 변경 이벤트
    /// Payload: WorkContextChangedEventPayload
    /// </summary>
    public class WorkContextChangedEvent : PubSubEvent { }

    public class WorkContextChangedEventPayload
    {
        public Context.WorkContext OldContext { get; set; }
        public Context.WorkContext NewContext { get; set; }
        public string Source { get; set; }
        public string ChangedProperty { get; set; }
    }

    /// <summary>
    /// 새로고침 요청 이벤트
    /// Payload: RefreshRequestEventPayload
    /// </summary>
    public class RefreshRequestEvent : PubSubEvent { }

    public class RefreshRequestEventPayload
    {
        public string TargetScreen { get; set; }
        public string EntityType { get; set; }
        public string EntityId { get; set; }
        public string Source { get; set; }
    }

    /// <summary>
    /// 화면 내비게이션 요청 이벤트
    /// Payload: NavigationRequestEventPayload
    /// </summary>
    public class NavigationRequestEvent : PubSubEvent { }

    public class NavigationRequestEventPayload
    {
        public string TargetScreenId { get; set; }
        public Context.WorkContext Context { get; set; }
        public string Source { get; set; }
    }

    /// <summary>
    /// 화면 닫기 요청 이벤트
    /// Payload: CloseScreenRequestEventPayload
    /// </summary>
    public class CloseScreenRequestEvent : PubSubEvent { }

    public class CloseScreenRequestEventPayload
    {
        public string ScreenId { get; set; }
        public bool Force { get; set; }
        public string Source { get; set; }
    }

    /// <summary>
    /// 모듈 활성화 이벤트 (Shell Title 업데이트 등에 사용)
    /// </summary>
    public class ModuleActivatedEvent : PubSubEvent { }

    public class ModuleActivatedEventPayload
    {
        public string ProgId { get; set; }
        public string ProgramName { get; set; }
        public string ModuleId { get; set; }
        public string Version { get; set; }
    }
}
