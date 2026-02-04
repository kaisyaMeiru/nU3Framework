using System;
using System.Collections.Generic;
using System.Reflection;

namespace nU3.Core.Events
{
    /// <summary>
    /// 제네릭 기반의 타입 안전한 발행/구독(Pub/Sub) 이벤트의 추상 기본 클래스입니다.
    /// TPayload 타입의 페이로드를 사용하는 강타입 이벤트를 구현할 때 상속하여 사용합니다.
    /// 내부적으로 약한 참조(WeakReference)를 사용하여 구독자를 보관하므로,
    /// 구독자가 가비지 컬렉션되면 자동으로 해당 구독이 제거되어 메모리 누수를 방지합니다.
    /// </summary>
    /// <typeparam name="TPayload">이벤트가 전달하는 페이로드 타입</typeparam>
    public abstract class PubSubEvent<TPayload> : PubSubEvent
    {
        // 구독자 목록 (타입 안전한 ISubscription<TPayload> 컬렉션)
        private readonly List<ISubscription<TPayload>> _subscriptions = new List<ISubscription<TPayload>>();

        /// <summary>
        /// 이벤트에 핸들러(Action)를 구독합니다.
        /// 구독은 내부적으로 약한 참조로 보관되므로, 구독자 객체가 더 이상 참조되지 않으면 가비지 컬렉션 대상이 되고 자동으로 정리됩니다.
        /// 보통 인스턴스 메서드를 구독할 때 사용하며, 정적 메서드도 지원합니다.
        /// </summary>
        /// <param name="action">페이로드를 받는 콜백 액션</param>
        public void Subscribe(Action<TPayload> action)
        {
            lock (_subscriptions)
            {
                _subscriptions.Add(new WeakSubscription<TPayload>(action));
            }
        }

        /// <summary>
        /// 지정한 페이로드로 이벤트를 발행합니다.
        /// 발행 시 현재 활성화된 모든 구독자에게 순차적으로 호출됩니다.
        /// 구독자에서 예외가 발생해도 다른 구독자에게 영향을 주지 않도록 내부에서 예외를 무시합니다.
        /// </summary>
        /// <param name="payload">구독자에게 전달할 데이터</param>
        public void Publish(TPayload payload)
        {
            List<Action<TPayload>> handlers = new List<Action<TPayload>>();

            lock (_subscriptions)
            {
                // 역순으로 검사하여 만료된 구독은 제거
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

            // 수집한 핸들러를 호출
            foreach (var handler in handlers)
            {
                try
                {
                    handler(payload);
                }
                catch
                {
                    // 구독자 예외는 발행자 보호를 위해 무시합니다.
                }
            }
        }

        private interface ISubscription<T>
        {
            Action<T> GetHandler();
        }

        /// <summary>
        /// 약한 참조(WeakReference)를 사용하여 구독자를 보관하는 내부 구현체입니다.
        /// 대상 인스턴스와 MethodInfo를 저장하여, 대상이 살아있는 동안에만 MethodInfo를 호출합니다.
        /// 정적 메서드의 경우 Target이 null이므로 정적으로 호출 가능한 래퍼를 반환합니다.
        /// </summary>
        private class WeakSubscription<T> : ISubscription<T>
        {
            private readonly WeakReference _weakTarget; // 정적 메서드는 null
            private readonly MethodInfo _method;
            private readonly bool _isStatic;

            public WeakSubscription(Action<T> action)
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

            public Action<T> GetHandler()
            {
                if (_isStatic)
                {
                    // 정적 메서드는 Target이 없으므로 null 대상에 대해 MethodInfo.Invoke 호출
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
}
