using System;

namespace nU3.Models
{
    /// <summary>
    /// 진료 예약/접수 정보 모델
    /// </summary>
    public class AppointmentDto
    {
        /// <summary>
        /// 예약 ID (Primary Key)
        /// </summary>
        public string AppointmentId { get; set; }

        /// <summary>
        /// 환자 ID (Foreign Key)
        /// </summary>
        public string PatientId { get; set; }

        /// <summary>
        /// 예약 번호
        /// </summary>
        public string AppointmentNo { get; set; }

        /// <summary>
        /// 예약/진료 유형 (0:Outpatient, 1:Emergency, 2:Inpatient, 3:HealthCheckup, 4:Scheduled, 5:WalkIn)
        /// </summary>
        public int AppointmentType { get; set; }

        /// <summary>
        /// 진료과 코드
        /// </summary>
        public int DepartmentCode { get; set; }

        /// <summary>
        /// 담당 의사 ID
        /// </summary>
        public string DoctorId { get; set; }

        /// <summary>
        /// 예약일시
        /// </summary>
        public DateTime AppointmentDateTime { get; set; }

        /// <summary>
        /// 접수일시
        /// </summary>
        public DateTime? CheckInDateTime { get; set; }

        /// <summary>
        /// 진료 시작일시
        /// </summary>
        public DateTime? StartDateTime { get; set; }

        /// <summary>
        /// 진료 종료일시
        /// </summary>
        public DateTime? EndDateTime { get; set; }

        /// <summary>
        /// 환자 상태 (0:Waiting, 1:InProgress, 2:Completed, 3:OnHold, 4:Cancelled)
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 긴급도 (1:Emergency, 2:Urgent, 3:SemiUrgent, 4:Routine, 5:Scheduled)
        /// </summary>
        public int Urgency { get; set; }

        /// <summary>
        /// 주 증상/방문 사유
        /// </summary>
        public string ChiefComplaint { get; set; }

        /// <summary>
        /// 진료실 번호
        /// </summary>
        public string RoomNo { get; set; }

        /// <summary>
        /// 대기 번호
        /// </summary>
        public int? WaitingNo { get; set; }

        /// <summary>
        /// 예상 대기 시간 (분)
        /// </summary>
        public int? EstimatedWaitingTime { get; set; }

        /// <summary>
        /// 재진 여부
        /// </summary>
        public bool IsRevisit { get; set; }

        /// <summary>
        /// 이전 진료 ID
        /// </summary>
        public string PreviousVisitId { get; set; }

        /// <summary>
        /// 예약 메모
        /// </summary>
        public string Memo { get; set; }

        /// <summary>
        /// 취소 여부
        /// </summary>
        public bool IsCancelled { get; set; }

        /// <summary>
        /// 취소 일시
        /// </summary>
        public DateTime? CancelledDateTime { get; set; }

        /// <summary>
        /// 취소 사유
        /// </summary>
        public string CancelReason { get; set; }

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
