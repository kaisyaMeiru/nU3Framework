using System.ComponentModel.DataAnnotations;

namespace nU3.Core.Enums
{
    /// <summary>
    /// 문서 유형
    /// </summary>
    public enum DocumentType
    {
        [Display(Name = "진단서", Description = "진단서", Order = 1)]
        MedicalCertificate = 1,

        [Display(Name = "소견서", Description = "의사 소견서", Order = 2)]
        MedicalOpinion = 2,

        [Display(Name = "처방전", Description = "처방전", Order = 3)]
        Prescription = 3,

        [Display(Name = "진료기록지", Description = "진료 기록지", Order = 4)]
        MedicalRecord = 4,

        [Display(Name = "수술기록지", Description = "수술 기록지", Order = 5)]
        SurgeryRecord = 5,

        [Display(Name = "검사결과지", Description = "검사 결과지", Order = 6)]
        TestResult = 6,

        [Display(Name = "입원확인서", Description = "입원 확인서", Order = 7)]
        AdmissionCertificate = 7,

        [Display(Name = "퇴원확인서", Description = "퇴원 확인서", Order = 8)]
        DischargeSummary = 8,

        [Display(Name = "사망진단서", Description = "사망 진단서", Order = 9)]
        DeathCertificate = 9,

        [Display(Name = "영수증", Description = "진료비 영수증", Order = 10)]
        Receipt = 10,

        [Display(Name = "동의서", Description = "환자 동의서", Order = 11)]
        ConsentForm = 11,

        [Display(Name = "기타", Description = "기타 문서", Order = 99)]
        Other = 99
    }
}
