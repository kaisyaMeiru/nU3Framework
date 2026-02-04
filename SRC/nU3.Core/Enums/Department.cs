using System.ComponentModel.DataAnnotations;

namespace nU3.Core.Enums
{
    /// <summary>
    /// 진료과 (진료 부서)
    /// </summary>
    public enum Department
    {
        [Display(Name = "내과", Description = "내과 진료", Order = 1)]
        InternalMedicine = 1,

        [Display(Name = "외과", Description = "외과 진료", Order = 2)]
        Surgery = 2,

        [Display(Name = "소아청소년과", Description = "소아청소년과 진료", Order = 3)]
        Pediatrics = 3,

        [Display(Name = "산부인과", Description = "산부인과 진료", Order = 4)]
        ObstetricsGynecology = 4,

        [Display(Name = "정형외과", Description = "정형외과 진료", Order = 5)]
        Orthopedics = 5,

        [Display(Name = "신경외과", Description = "신경외과 진료", Order = 6)]
        Neurosurgery = 6,

        [Display(Name = "안과", Description = "안과 진료", Order = 7)]
        Ophthalmology = 7,

        [Display(Name = "이비인후과", Description = "이비인후과 진료", Order = 8)]
        Otolaryngology = 8,

        [Display(Name = "피부과", Description = "피부과 진료", Order = 9)]
        Dermatology = 9,

        [Display(Name = "비뇨기과", Description = "비뇨기과 진료", Order = 10)]
        Urology = 10,

        [Display(Name = "정신건강의학과", Description = "정신건강의학과 진료", Order = 11)]
        Psychiatry = 11,

        [Display(Name = "재활의학과", Description = "재활의학과 진료", Order = 12)]
        Rehabilitation = 12,

        [Display(Name = "영상의학과", Description = "영상의학과", Order = 13)]
        Radiology = 13,

        [Display(Name = "병리과", Description = "병리과", Order = 14)]
        Pathology = 14,

        [Display(Name = "응급의학과", Description = "응급의학과", Order = 15)]
        EmergencyMedicine = 15,

        [Display(Name = "마취통증의학과", Description = "마취통증의학과", Order = 16)]
        Anesthesiology = 16,

        [Display(Name = "가정의학과", Description = "가정의학과 진료", Order = 17)]
        FamilyMedicine = 17,

        [Display(Name = "치과", Description = "치과 진료", Order = 18)]
        Dentistry = 18
    }
}
