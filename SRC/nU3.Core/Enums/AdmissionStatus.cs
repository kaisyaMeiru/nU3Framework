using System.ComponentModel.DataAnnotations;

namespace nU3.Core.Enums
{
    /// <summary>
    /// 입원 상태
    /// </summary>
    public enum AdmissionStatus
    {
        [Display(Name = "입원예정", Description = "입원 예정", Order = 1)]
        Scheduled = 0,

        [Display(Name = "입원중", Description = "현재 입원 중", Order = 2)]
        Admitted = 1,

        [Display(Name = "외출", Description = "외출 중", Order = 3)]
        OnLeave = 2,

        [Display(Name = "외박", Description = "외박 중", Order = 4)]
        Overnight = 3,

        [Display(Name = "퇴원예정", Description = "퇴원 예정", Order = 5)]
        DischargeScheduled = 8,

        [Display(Name = "퇴원", Description = "퇴원 완료", Order = 6)]
        Discharged = 9,

        [Display(Name = "전원", Description = "타 병원으로 전원", Order = 7)]
        Transferred = 10,

        [Display(Name = "사망", Description = "사망으로 퇴원", Order = 8)]
        Deceased = 11
    }
}
