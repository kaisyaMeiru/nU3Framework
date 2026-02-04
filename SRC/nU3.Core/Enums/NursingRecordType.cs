using System.ComponentModel.DataAnnotations;

namespace nU3.Core.Enums
{
    /// <summary>
    /// 간호 기록 유형
    /// </summary>
    public enum NursingRecordType
    {
        [Display(Name = "일반간호", Description = "일반 간호 기록", Order = 1)]
        General = 1,

        [Display(Name = "투약", Description = "투약 기록", Order = 2)]
        Medication = 2,

        [Display(Name = "활력징후", Description = "활력징후 측정", Order = 3)]
        VitalSigns = 3,

        [Display(Name = "섭취량", Description = "섭취량 기록", Order = 4)]
        Intake = 4,

        [Display(Name = "배설량", Description = "배설량 기록", Order = 5)]
        Output = 5,

        [Display(Name = "상처관리", Description = "상처 관리", Order = 6)]
        WoundCare = 6,

        [Display(Name = "욕창관리", Description = "욕창 관리", Order = 7)]
        PressureUlcer = 7,

        [Display(Name = "낙상예방", Description = "낙상 예방 활동", Order = 8)]
        FallPrevention = 8,

        [Display(Name = "감염관리", Description = "감염 관리", Order = 9)]
        InfectionControl = 9,

        [Display(Name = "응급상황", Description = "응급 상황 기록", Order = 10)]
        Emergency = 10,

        [Display(Name = "특이사항", Description = "특이사항", Order = 11)]
        SpecialNote = 11
    }
}
