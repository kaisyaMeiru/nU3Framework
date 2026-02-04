using System.ComponentModel.DataAnnotations;

namespace nU3.Core.Enums
{
    /// <summary>
    /// 알레르기 심각도
    /// </summary>
    public enum AllergySeverity
    {
        [Display(Name = "경증", Description = "가벼운 알레르기 반응", Order = 1)]
        Mild = 1,

        [Display(Name = "중등도", Description = "중간 정도의 알레르기 반응", Order = 2)]
        Moderate = 2,

        [Display(Name = "중증", Description = "심각한 알레르기 반응", Order = 3)]
        Severe = 3,

        [Display(Name = "생명위협", Description = "아나필락시스 등 생명 위협", Order = 4)]
        LifeThreatening = 4
    }
}
