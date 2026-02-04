using System.ComponentModel.DataAnnotations;

namespace nU3.Core.Enums
{
    /// <summary>
    /// 처방 상태
    /// </summary>
    public enum PrescriptionStatus
    {
        [Display(Name = "작성중", Description = "처방 작성 중", Order = 1)]
        Draft = 0,

        [Display(Name = "처방완료", Description = "처방 완료", Order = 2)]
        Prescribed = 1,

        [Display(Name = "조제중", Description = "약 조제 중", Order = 3)]
        Dispensing = 2,

        [Display(Name = "조제완료", Description = "약 조제 완료", Order = 4)]
        Dispensed = 3,

        [Display(Name = "수령완료", Description = "환자 수령 완료", Order = 5)]
        Received = 4,

        [Display(Name = "취소", Description = "처방 취소", Order = 6)]
        Cancelled = 9
    }
}
