using System.ComponentModel.DataAnnotations;

namespace nU3.Core.Enums
{
    /// <summary>
    /// 진료 유형
    /// </summary>
    public enum AppointmentType
    {
        [Display(Name = "외래", Description = "외래 진료", Order = 1)]
        Outpatient = 0,

        [Display(Name = "응급", Description = "응급 진료", Order = 2)]
        Emergency = 1,

        [Display(Name = "입원", Description = "입원 진료", Order = 3)]
        Inpatient = 2,

        [Display(Name = "검진", Description = "건강 검진", Order = 4)]
        HealthCheckup = 3,

        [Display(Name = "예약", Description = "예약 진료", Order = 5)]
        Scheduled = 4,

        [Display(Name = "당일", Description = "당일 진료", Order = 6)]
        WalkIn = 5
    }
}
