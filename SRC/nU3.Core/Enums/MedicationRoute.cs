using System.ComponentModel.DataAnnotations;

namespace nU3.Core.Enums
{
    /// <summary>
    /// 투약 경로
    /// </summary>
    public enum MedicationRoute
    {
        [Display(Name = "경구", Description = "입으로 복용", Order = 1)]
        Oral = 1,

        [Display(Name = "정맥주사", Description = "정맥 주사(IV)", Order = 2)]
        Intravenous = 2,

        [Display(Name = "근육주사", Description = "근육 주사(IM)", Order = 3)]
        Intramuscular = 3,

        [Display(Name = "피하주사", Description = "피하 주사(SC)", Order = 4)]
        Subcutaneous = 4,

        [Display(Name = "외용", Description = "피부에 도포", Order = 5)]
        Topical = 5,

        [Display(Name = "흡입", Description = "흡입 투여", Order = 6)]
        Inhalation = 6,

        [Display(Name = "직장", Description = "직장 투여", Order = 7)]
        Rectal = 7,

        [Display(Name = "설하", Description = "혀 밑에 투여", Order = 8)]
        Sublingual = 8,

        [Display(Name = "점안", Description = "눈에 점안", Order = 9)]
        Ophthalmic = 9,

        [Display(Name = "점이", Description = "귀에 점이", Order = 10)]
        Otic = 10,

        [Display(Name = "비강", Description = "코에 투여", Order = 11)]
        Nasal = 11,

        [Display(Name = "기타", Description = "기타 경로", Order = 99)]
        Other = 99
    }
}
