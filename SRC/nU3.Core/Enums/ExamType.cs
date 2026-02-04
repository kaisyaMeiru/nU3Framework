using System.ComponentModel.DataAnnotations;

namespace nU3.Core.Enums
{
    /// <summary>
    /// 검사 유형
    /// </summary>
    public enum ExamType
    {
        [Display(Name = "혈액검사", Description = "혈액 검사", Order = 1)]
        BloodTest = 1,

        [Display(Name = "소변검사", Description = "소변 검사", Order = 2)]
        UrineTest = 2,

        [Display(Name = "X-Ray", Description = "X-Ray 촬영", Order = 3)]
        XRay = 3,

        [Display(Name = "CT", Description = "CT 촬영", Order = 4)]
        CT = 4,

        [Display(Name = "MRI", Description = "MRI 촬영", Order = 5)]
        MRI = 5,

        [Display(Name = "초음파", Description = "초음파 검사", Order = 6)]
        Ultrasound = 6,

        [Display(Name = "내시경", Description = "내시경 검사", Order = 7)]
        Endoscopy = 7,

        [Display(Name = "심전도", Description = "심전도 검사", Order = 8)]
        ECG = 8,

        [Display(Name = "심초음파", Description = "심초음파 검사", Order = 9)]
        Echocardiography = 9,

        [Display(Name = "생검", Description = "조직 생검", Order = 10)]
        Biopsy = 10,

        [Display(Name = "병리검사", Description = "병리 검사", Order = 11)]
        Pathology = 11,

        [Display(Name = "기타", Description = "기타 검사", Order = 99)]
        Other = 99
    }
}
