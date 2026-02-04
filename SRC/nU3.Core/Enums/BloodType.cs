using System.ComponentModel.DataAnnotations;

namespace nU3.Core.Enums
{
    /// <summary>
    /// 혈액형
    /// </summary>
    public enum BloodType
    {
        [Display(Name = "미확인", Description = "혈액형 미확인", Order = 0)]
        Unknown = 0,

        [Display(Name = "A형 Rh+", Description = "A형 Rh 양성", Order = 1)]
        A_Positive = 1,

        [Display(Name = "A형 Rh-", Description = "A형 Rh 음성", Order = 2)]
        A_Negative = 2,

        [Display(Name = "B형 Rh+", Description = "B형 Rh 양성", Order = 3)]
        B_Positive = 3,

        [Display(Name = "B형 Rh-", Description = "B형 Rh 음성", Order = 4)]
        B_Negative = 4,

        [Display(Name = "O형 Rh+", Description = "O형 Rh 양성", Order = 5)]
        O_Positive = 5,

        [Display(Name = "O형 Rh-", Description = "O형 Rh 음성", Order = 6)]
        O_Negative = 6,

        [Display(Name = "AB형 Rh+", Description = "AB형 Rh 양성", Order = 7)]
        AB_Positive = 7,

        [Display(Name = "AB형 Rh-", Description = "AB형 Rh 음성", Order = 8)]
        AB_Negative = 8
    }
}
