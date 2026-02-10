using System.ComponentModel.DataAnnotations;

namespace nU3.Core.Enums
{
    /// <summary>
    /// 폼 유형
    /// </summary>
    public enum FormType
    {
        /// <summary>
        /// 모달 팝업 (ShowDialog)
        /// </summary>
        ModalPopup,

        /// <summary>
        /// 모델리스 팝업 (Show / ToolWindow 스타일)
        /// </summary>
        ModelessPopup,

        /// <summary>
        /// 호스트 폼에 내장하여 사용하는 임베디드 컨트롤 (UserControl 등)
        /// </summary>
        Embedded
    }
}
    