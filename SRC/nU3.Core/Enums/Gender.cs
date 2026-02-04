using System.ComponentModel.DataAnnotations;

namespace nU3.Core.Enums
{
    /// <summary>
    /// 성별
    /// </summary>
    public enum Gender
    {
        [Display(Name = "미지정", Description = "성별 미지정", Order = 0)]
        Unspecified = 0,

        [Display(Name = "남성", Description = "남성", Order = 1)]
        Male = 1,

        [Display(Name = "여성", Description = "여성", Order = 2)]
        Female = 2,

        [Display(Name = "기타", Description = "기타", Order = 3)]
        Other = 9
    }
}
