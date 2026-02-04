using System.ComponentModel.DataAnnotations;

namespace nU3.Core.Enums
{
    /// <summary>
    /// 긴급도
    /// </summary>
    public enum Urgency
    {
        [Display(Name = "응급", Description = "즉시 처치 필요", Order = 1)]
        Emergency = 1,

        [Display(Name = "긴급", Description = "빠른 처치 필요", Order = 2)]
        Urgent = 2,

        [Display(Name = "준긴급", Description = "가능한 빠른 처치", Order = 3)]
        SemiUrgent = 3,

        [Display(Name = "일반", Description = "일반 진료", Order = 4)]
        Routine = 4,

        [Display(Name = "예약", Description = "예약 진료", Order = 5)]
        Scheduled = 5
    }
}
