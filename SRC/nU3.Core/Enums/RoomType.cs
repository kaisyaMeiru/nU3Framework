using System.ComponentModel.DataAnnotations;

namespace nU3.Core.Enums
{
    /// <summary>
    /// 병실 유형
    /// </summary>
    public enum RoomType
    {
        [Display(Name = "일반실", Description = "다인실 병실", Order = 1)]
        General = 0,

        [Display(Name = "2인실", Description = "2인 병실", Order = 2)]
        SemiPrivate = 1,

        [Display(Name = "1인실", Description = "1인 병실", Order = 3)]
        Private = 2,

        [Display(Name = "특실", Description = "특별 병실", Order = 4)]
        VIP = 3,

        [Display(Name = "중환자실", Description = "중환자 전담 병실", Order = 5)]
        ICU = 10,

        [Display(Name = "격리실", Description = "격리 병실", Order = 6)]
        Isolation = 11,

        [Display(Name = "응급실", Description = "응급실 병상", Order = 7)]
        Emergency = 20
    }
}
