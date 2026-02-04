using System;

namespace nU3.Core.UI.Controls
{
    public interface InU3Control
    {
        /// <summary>
        /// 컨트롤의 값을 일반 형태로 가져옵니다.
        /// </summary>
        object? GetValue();

        /// <summary>
        /// 컨트롤의 값을 설정합니다.
        /// </summary>
        void SetValue(object? value);

        /// <summary>
        /// 컨트롤의 내용을 초기화(클리어)합니다.
        /// </summary>
        void Clear();

        /// <summary>
        /// 컨트롤의 고유 ID(보통 Name)를 반환합니다.
        /// </summary>
        string GetControlId();
    }
}
