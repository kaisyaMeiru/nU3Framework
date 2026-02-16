using System;
using System.ComponentModel;
using nU3.Core.Events;

namespace nU3.Core.Interfaces
{
    /// <summary>
    /// 모든 작업 컴포넌트의 기본 인터페이스입니다.
    /// EventBus와 ProgramID를 노출하여 표준화된 접근을 제공합니다.
    /// </summary>
    public interface IBaseWorkComponent
    {
        /// <summary>
        /// 상위(Owner) 이벤트 버스
        /// </summary>
        IEventAggregator OwnerEventBus { get; }

        /// <summary>
        /// 상위(Owner) 프로그램 ID
        /// </summary>
        string OwnerProgramID { get; }
    }
}
