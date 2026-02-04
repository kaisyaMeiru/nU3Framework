using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nU3.Core.Enums
{
    /// <summary>
    /// 사용자 권한을 나타내는 열거형
    /// </summary>
    [Flags]
    public enum UserAuth
    {
        /// <summary>
        /// 권한 없음
        /// </summary>
        [Display(Name = "권한 없음")]
        [Description("권한 없음")]
        None = 0,

        /// <summary>
        /// 읽기 권한
        /// </summary>
        [Display(Name = "읽기")]
        [Description("읽기 권한")]
        Read = 1,

        /// <summary>
        /// 쓰기 권한
        /// </summary>
        [Display(Name = "쓰기")]
        [Description("쓰기 권한")]
        Write = 2,

        /// <summary>
        /// 편집 권한
        /// </summary>
        [Display(Name = "편집")]
        [Description("편집 권한")]
        Edit = 4,

        /// <summary>
        /// 삭제 권한
        /// </summary>
        [Display(Name = "삭제")]
        [Description("삭제 권한")]
        Delete = 8,

        /// <summary>
        /// 접근 권한
        /// </summary>
        [Display(Name = "접근")]
        [Description("접근 권한")]
        Access = 16,

        /// <summary>
        /// 관리자 권한 (모든 권한)
        /// </summary>
        [Display(Name = "관리자")]
        [Description("관리자 권한")]
        Admin = Read | Write | Edit | Delete | Access
    }    
}
