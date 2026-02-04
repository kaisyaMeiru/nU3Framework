using System.ComponentModel.DataAnnotations;

namespace nU3.Core.Enums
{
    /// <summary>
    /// 수술 상태
    /// </summary>
    public enum SurgeryStatus
    {
        [Display(Name = "예정", Description = "수술 예정", Order = 1)]
        Scheduled = 0,

        [Display(Name = "준비중", Description = "수술 준비 중", Order = 2)]
        Preparing = 1,

        [Display(Name = "진행중", Description = "수술 진행 중", Order = 3)]
        InProgress = 2,

        [Display(Name = "완료", Description = "수술 완료", Order = 4)]
        Completed = 3,

        [Display(Name = "회복중", Description = "회복실 대기", Order = 5)]
        Recovery = 4,

        [Display(Name = "연기", Description = "수술 연기", Order = 6)]
        Postponed = 8,

        [Display(Name = "취소", Description = "수술 취소", Order = 7)]
        Cancelled = 9
    }
}
