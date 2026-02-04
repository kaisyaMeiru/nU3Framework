using System;
using System.Collections.Concurrent;

namespace nU3.Core.Events
{
    /// <summary>
    /// 이벤트 애그리게이터 패턴의 구현체입니다.
    /// 모듈 간의 약결합(발행/구독) 통신을 위해 이벤트 인스턴스를 관리하고 제공합니다.
    /// 내부적으로 ConcurrentDictionary를 사용하여 이벤트 타입별 싱글톤 인스턴스를 보관합니다.
    /// </summary>
    public class EventAggregator : IEventAggregator
    {
        private readonly ConcurrentDictionary<Type, PubSubEvent> _events = new ConcurrentDictionary<Type, PubSubEvent>();

        /// <summary>
        /// 요청한 이벤트 타입의 인스턴스를 반환합니다.
        /// 요청된 타입이 처음일 경우 새 인스턴스를 생성하여 캐시에 저장합니다.
        /// </summary>
        public TEvent GetEvent<TEvent>() where TEvent : PubSubEvent, new()
        {
            return (TEvent)_events.GetOrAdd(typeof(TEvent), _ => new TEvent());
        }
    }
}
