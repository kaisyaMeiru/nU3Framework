using System;

namespace nU3.Core.UI.Controls
{
    /// <summary>
    /// nU3 Framework 표준 컨트롤 인터페이스
    /// 모든 래핑된 컨트롤은 이 인터페이스를 구현하여 공통 기능을 제공합니다.
    /// </summary>
    public interface InU3Control
    {
        /// <summary>
        /// 컨트롤의 핵심 값을 반환합니다. (EditValue, Checked, DataSource 등)
        /// </summary>
        object? GetValue();

        /// <summary>
        /// 컨트롤에 값을 설정합니다.
        /// </summary>
        void SetValue(object? value);

        /// <summary>
        /// 컨트롤의 값을 초기화합니다.
        /// </summary>
        void Clear();

        /// <summary>
        /// 컨트롤의 고유 ID(Name)를 반환합니다.
        /// </summary>
        string GetControlId();
    }
}