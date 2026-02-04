using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nU3.Core.Enums
{
    public enum UserRole
    {
        [Browsable(true)]
        [Display(Name = "관리자", Description = "시스템 관리자", Order = 1)]
        Admin = 0,

        [Browsable(true)]
        [Display(Name = "사용자", Description = "사용자 권한", Order = 2)]
        Tech = 1,

        [Browsable(true)]
        [Display(Name = "의사", Description = "의사 권한", Order = 3)]
        Doctor = 10,

        [Browsable(true)]
        [Display(Name = "간호사", Description = "간호사 권한", Order = 4)]
        Nurse = 11,

        [Browsable(false)]
        [Display(Name = "환자", Description = "환자 권한", Order = 5)]
        Patient = 100,
    }    
}
