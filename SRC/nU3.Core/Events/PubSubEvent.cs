using System;
using System.Collections.Generic;
using System.Reflection;

namespace nU3.Core.Events
{
    /// <summary>
    /// 모듈 간 약결합 통신을 위해 발행/구독 기능을 제공하는 서비스의 인터페이스입니다.
    /// </summary>
    public interface IEventAggregator
    {
        /// <summary>
        /// 지정한 이벤트 타입의 인스턴스를 가져옵니다.
        /// </summary>
        TEvent GetEvent<TEvent>() where TEvent : PubSubEvent, new();
    }

    /// <summary>
    /// 프레임워크 내 모든 이벤트의 기본 베이스 클래스입니다.
    /// </summary>
    public abstract class PubSubEvent
    {
        private readonly List<ISubscription> _subscriptions = new List<ISubscription>();

        /// <summary>
        /// 이벤트에 액션(핸들러)을 구독합니다.
        /// </summary>
        public void Subscribe(Action<object> action)
        {
            lock (_subscriptions)
            {
                _subscriptions.Add(new WeakSubscription(action));
            }
        }

        /// <summary>
        /// 지정한 페이로드로 이벤트를 발행합니다.
        /// </summary>
        public void Publish(object payload)
        {
            List<Action<object>> handlers = new List<Action<object>>();

            lock (_subscriptions)
            {
                // 핸들러를 수집하면서 사라진(가비지 수집된) 참조는 정리합니다.
                for (int i = _subscriptions.Count - 1; i >= 0; i--)
                {
                    var handler = _subscriptions[i].GetHandler();
                    if (handler != null)
                    {
                        handlers.Add(handler);
                    }
                    else
                    {
                        _subscriptions.RemoveAt(i);
                    }
                }
            }

            foreach (var handler in handlers)
            {
                try
                {
                    handler(payload);
                }
                catch
                {
                    // 구독자에서 발생한 예외는 무시합니다(발행자 보호)
                }
            }
        }
    }

    internal interface ISubscription
    {
        Action<object> GetHandler();
    }

    /// <summary>
    /// 구독자를 약한 참조로 보관하여 가비지 컬렉션이 가능하도록 하는 구현입니다.
    /// 내부적으로 대상 객체에 대한 약한 참조와 MethodInfo를 저장하므로,
    /// 델리게이트 인스턴스 자체만을 약한 참조로 보관하는 방식보다 안정적으로 동작합니다.
    /// </summary>
    internal class WeakSubscription : ISubscription
    {
        private readonly WeakReference _weakTarget; // 정적 메서드는 null 참조 가능
        private readonly MethodInfo _method;
        private readonly bool _isStatic;

        public WeakSubscription(Action<object> action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));

            _isStatic = action.Target == null;
            if (!_isStatic)
            {
                _weakTarget = new WeakReference(action.Target);
            }
            else
            {
                _weakTarget = null;
            }

            _method = action.Method;
        }

        public Action<object> GetHandler()
        {
            if (_isStatic)
            {
                // 정적 메서드 처리
                return (payload) =>
                {
                    try
                    {
                        _method.Invoke(null, new object[] { payload });
                    }
                    catch
                    {
                        // 구독자 예외 무시
                    }
                };
            }

            var target = _weakTarget.Target;
            if (target == null)
                return null;

            return (payload) =>
            {
                var t = _weakTarget.Target;
                if (t != null)
                {
                    try
                    {
                        _method.Invoke(t, new object[] { payload });
                    }
                    catch
                    {
                        // 구독자 예외 무시
                    }
                }
            };
        }
    }
}
