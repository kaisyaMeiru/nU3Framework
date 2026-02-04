using System;

namespace nU3.Models
{
    /// <summary>
    /// 검사 오더 정보 모델
    /// </summary>
    public class ExamOrderDto
    {
        /// <summary>
        /// 검사 오더 ID (Primary Key)
        /// </summary>
        public string ExamOrderId { get; set; }

        /// <summary>
        /// 검사 번호
        /// </summary>
        public string ExamNo { get; set; }

        /// <summary>
        /// 환자 ID (Foreign Key)
        /// </summary>
        public string PatientId { get; set; }

        /// <summary>
        /// 예약/진료 ID (Foreign Key)
        /// </summary>
        public string AppointmentId { get; set; }

        /// <summary>
        /// 검사 유형 (1:혈액, 2:소변, 3:XRay, 4:CT, 5:MRI, 6:초음파, 7:내시경, 8:ECG, 9:심초음파, 10:생검, 11:병리, 99:기타)
        /// </summary>
        public int ExamType { get; set; }

        /// <summary>
        /// 검사 코드
        /// </summary>
        public string ExamCode { get; set; }

        /// <summary>
        /// 검사명
        /// </summary>
        public string ExamName { get; set; }

        /// <summary>
        /// 검사 상태 (0:접수, 1:대기, 2:진행중, 3:완료, 4:판독대기, 5:판독완료, 6:보고완료, 9:취소)
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 처방 의사 ID
        /// </summary>
        public string OrderDoctorId { get; set; }

        /// <summary>
        /// 처방 진료과 코드
        /// </summary>
        public int OrderDepartmentCode { get; set; }

        /// <summary>
        /// 처방일시
        /// </summary>
        public DateTime OrderDateTime { get; set; }

        /// <summary>
        /// 검사 예정일시
        /// </summary>
        public DateTime? ScheduledDateTime { get; set; }

        /// <summary>
        /// 검사 시작일시
        /// </summary>
        public DateTime? StartDateTime { get; set; }

        /// <summary>
        /// 검사 완료일시
        /// </summary>
        public DateTime? CompletedDateTime { get; set; }

        /// <summary>
        /// 검사 부서
        /// </summary>
        public string ExamDepartment { get; set; }

        /// <summary>
        /// 검사실/장비명
        /// </summary>
        public string ExamRoom { get; set; }

        /// <summary>
        /// 검체 채취일시
        /// </summary>
        public DateTime? SpecimenCollectedDateTime { get; set; }

        /// <summary>
        /// 검체 번호
        /// </summary>
        public string SpecimenNo { get; set; }

        /// <summary>
        /// 검체 유형
        /// </summary>
        public string SpecimenType { get; set; }

        /// <summary>
        /// 긴급 검사 여부
        /// </summary>
        public bool IsUrgent { get; set; }

        /// <summary>
        /// 금식 필요 여부
        /// </summary>
        public bool RequiresFasting { get; set; }

        /// <summary>
        /// 환자 준비 사항
        /// </summary>
        public string PreparationInstructions { get; set; }

        /// <summary>
        /// 검사 목적/임상 정보
        /// </summary>
        public string ClinicalInfo { get; set; }

        /// <summary>
        /// 검사 메모
        /// </summary>
        public string Memo { get; set; }

        /// <summary>
        /// 취소 여부
        /// </summary>
        public bool IsCancelled { get; set; }

        /// <summary>
        /// 취소일시
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
