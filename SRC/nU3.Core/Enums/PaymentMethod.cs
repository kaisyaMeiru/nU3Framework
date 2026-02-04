using System.ComponentModel.DataAnnotations;

namespace nU3.Core.Enums
{
    /// <summary>
    /// 결제 방법
    /// </summary>
    public enum PaymentMethod
    {
        [Display(Name = "현금", Description = "현금 결제", Order = 1)]
        Cash = 1,

        [Display(Name = "신용카드", Description = "신용카드 결제", Order = 2)]
        CreditCard = 2,

        [Display(Name = "체크카드", Description = "체크카드 결제", Order = 3)]
        DebitCard = 3,

        [Display(Name = "계좌이체", Description = "계좌 이체", Order = 4)]
        BankTransfer = 4,

        [Display(Name = "건강보험", Description = "건강보험 청구", Order = 5)]
        HealthInsurance = 10,

        [Display(Name = "의료급여", Description = "의료급여", Order = 6)]
        MedicalAid = 11,

        [Display(Name = "산재보험", Description = "산재보험", Order = 7)]
        WorkersCompensation = 12,

        [Display(Name = "자동차보험", Description = "자동차보험", Order = 8)]
        AutoInsurance = 13,

        [Display(Name = "미수금", Description = "미수금(후불)", Order = 9)]
        OnCredit = 90,

        [Display(Name = "무료진료", Description = "무료 진료", Order = 10)]
        Free = 99
    }
}
