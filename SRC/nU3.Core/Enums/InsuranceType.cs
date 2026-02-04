using System.ComponentModel.DataAnnotations;

namespace nU3.Core.Enums
{
    /// <summary>
    /// 보험 유형
    /// </summary>
    public enum InsuranceType
    {
        [Display(Name = "건강보험", Description = "국민건강보험", Order = 1)]
        NationalHealth = 1,

        [Display(Name = "의료급여", Description = "의료급여 1종/2종", Order = 2)]
        MedicalAid = 2,

        [Display(Name = "산재보험", Description = "산업재해보상보험", Order = 3)]
        WorkersCompensation = 3,

        [Display(Name = "자동차보험", Description = "자동차보험", Order = 4)]
        AutoInsurance = 4,

        [Display(Name = "실손보험", Description = "민간 실손보험", Order = 5)]
        PrivateInsurance = 5,

        [Display(Name = "보훈", Description = "국가유공자 의료지원", Order = 6)]
        Veterans = 6,

        [Display(Name = "비급여", Description = "전액 본인 부담", Order = 7)]
        SelfPay = 99
    }
}
