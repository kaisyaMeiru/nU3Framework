using System;

namespace nU3.Models
{
    /// <summary>
    /// 검사 결과 정보 모델
    /// </summary>
    public class ExamResultDto
    {
        /// <summary>
        /// 검사 결과 ID (Primary Key)
        /// </summary>
        public string ExamResultId { get; set; }

        /// <summary>
        /// 검사 오더 ID (Foreign Key)
        /// </summary>
        public string ExamOrderId { get; set; }

        /// <summary>
        /// 검사 항목 코드
        /// </summary>
        public string ResultItemCode { get; set; }

        /// <summary>
        /// 검사 항목명
        /// </summary>
        public string ResultItemName { get; set; }

        /// <summary>
        /// 결과값
        /// </summary>
        public string ResultValue { get; set; }

        /// <summary>
        /// 결과값 (숫자형)
        /// </summary>
        public decimal? NumericValue { get; set; }

        /// <summary>
        /// 결과값 (텍스트형)
        /// </summary>
        public string TextValue { get; set; }

        /// <summary>
        /// 단위
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// 참고치 최소값
        /// </summary>
        public decimal? ReferenceMin { get; set; }

        /// <summary>
        /// 참고치 최대값
        /// </summary>
        public decimal? ReferenceMax { get; set; }

        /// <summary>
        /// 참고치 범위 (텍스트)
        /// </summary>
        public string ReferenceRange { get; set; }

        /// <summary>
        /// 정상/비정상 구분 (H: High, L: Low, N: Normal)
        /// </summary>
        public string AbnormalFlag { get; set; }

        /// <summary>
        /// 위험도 (C: Critical, A: Abnormal, N: Normal)
        /// </summary>
        public string SeverityFlag { get; set; }

        /// <summary>
        /// 판독 소견
        /// </summary>
        public string Interpretation { get; set; }

        /// <summary>
        /// 판독 의사 ID
        /// </summary>
        public string ReviewedDoctorId { get; set; }

        /// <summary>
        /// 판독일시
        /// </summary>
        public DateTime? ReviewedDateTime { get; set; }

        /// <summary>
        /// 보고일시
        /// </summary>
        public DateTime? ReportedDateTime { get; set; }

        /// <summary>
        /// 결과 확인 의사 ID
        /// </summary>
        public string ConfirmedDoctorId { get; set; }

        /// <summary>
        /// 결과 확인일시
        /// </summary>
        public DateTime? ConfirmedDateTime { get; set; }

        /// <summary>
        /// 첨부파일 경로 (이미지 등)
        /// </summary>
        public string AttachmentPath { get; set; }

        /// <summary>
        /// 메모
        /// </summary>
        public string Memo { get; set; }

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
