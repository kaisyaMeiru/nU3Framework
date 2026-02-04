using System.ComponentModel.DataAnnotations;

namespace nU3.Core.Enums
{
    /// <summary>
    /// 환자 상태
    /// </summary>
    public enum PatientStatus
    {
        [Display(Name = "대기", Description = "진료 대기 중", Order = 1)]
        Waiting = 0,

        [Display(Name = "진료중", Description = "현재 진료 중", Order = 2)]
        InProgress = 1,

        [Display(Name = "진료완료", Description = "진료가 완료됨", Order = 3)]
        Completed = 2,

        [Display(Name = "보류", Description = "진료 보류", Order = 4)]
        OnHold = 3,

        [Display(Name = "취소", Description = "진료 취소", Order = 5)]
        Cancelled = 4,

        [Display(Name = "입원", Description = "입원 상태", Order = 6)]
        Admitted = 10,

        [Display(Name = "퇴원", Description = "퇴원 완료", Order = 7)]
        Discharged = 11
    }
}
