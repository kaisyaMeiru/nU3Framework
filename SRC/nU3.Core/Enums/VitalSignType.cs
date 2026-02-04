using System.ComponentModel.DataAnnotations;

namespace nU3.Core.Enums
{
    /// <summary>
    /// 활력징후 유형
    /// </summary>
    public enum VitalSignType
    {
        [Display(Name = "혈압", Description = "혈압 측정", Order = 1)]
        BloodPressure = 1,

        [Display(Name = "맥박", Description = "맥박수", Order = 2)]
        Pulse = 2,

        [Display(Name = "호흡", Description = "호흡수", Order = 3)]
        Respiration = 3,

        [Display(Name = "체온", Description = "체온", Order = 4)]
        Temperature = 4,

        [Display(Name = "산소포화도", Description = "혈중 산소포화도(SpO2)", Order = 5)]
        OxygenSaturation = 5,

        [Display(Name = "혈당", Description = "혈당 수치", Order = 6)]
        BloodGlucose = 6,

        [Display(Name = "신장", Description = "키", Order = 7)]
        Height = 7,

        [Display(Name = "체중", Description = "몸무게", Order = 8)]
        Weight = 8,

        [Display(Name = "BMI", Description = "체질량지수", Order = 9)]
        BMI = 9,

        [Display(Name = "통증척도", Description = "통증 정도(0-10)", Order = 10)]
        PainScale = 10
    }
}
