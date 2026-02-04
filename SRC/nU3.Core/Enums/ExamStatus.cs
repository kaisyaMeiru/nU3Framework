using System.ComponentModel.DataAnnotations;

namespace nU3.Core.Enums
{
    /// <summary>
    /// 검사 상태
    /// </summary>
    public enum ExamStatus
    {
        [Display(Name = "접수", Description = "검사 접수", Order = 1)]
        Registered = 0,

        [Display(Name = "대기", Description = "검사 대기", Order = 2)]
        Waiting = 1,

        [Display(Name = "진행중", Description = "검사 진행 중", Order = 3)]
        InProgress = 2,

        [Display(Name = "완료", Description = "검사 완료", Order = 4)]
        Completed = 3,

        [Display(Name = "판독대기", Description = "판독 대기 중", Order = 5)]
        PendingReview = 4,

        [Display(Name = "판독완료", Description = "판독 완료", Order = 6)]
        Reviewed = 5,

        [Display(Name = "보고완료", Description = "최종 보고 완료", Order = 7)]
        Reported = 6,

        [Display(Name = "취소", Description = "검사 취소", Order = 8)]
        Cancelled = 9
    }
}
