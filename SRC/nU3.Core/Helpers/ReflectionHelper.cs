using System;
using System.Collections.Generic;
using System.Reflection;
using System.ComponentModel;
using System.Linq;

namespace nU3.Core.Helpers
{
    /// <summary>
    /// 리플렉션 및 컴포넌트 탐색을 위한 공통 헬퍼 클래스입니다.
    /// </summary>
    public static class ReflectionHelper
    {
        private static readonly Dictionary<string, object> _componentCache = new Dictionary<string, object>();

        /// <summary>
        /// 인스턴스에서 특정 타입의 컴포넌트(필드)를 탐색하며 결과를 캐싱합니다.
        /// </summary>
        public static T? FindComponent<T>(object? instance, Type? stopAtBaseType = null) where T : class
        {
            return FindAllComponents<T>(instance, stopAtBaseType).Values.FirstOrDefault();
        }

        /// <summary>
        /// 인스턴스 내에 정의된 특정 타입의 모든 컴포넌트(필드)를 탐색하여 이름과 함께 반환합니다.
        /// </summary>
        public static Dictionary<string, T> FindAllComponents<T>(object? instance, Type? stopAtBaseType = null) where T : class
        {
            var result = new Dictionary<string, T>();
            if (instance == null) return result;

            var type = instance.GetType();
            while (type != null && type != (stopAtBaseType ?? typeof(object)))
            {
                var fields = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                foreach (var field in fields)
                {
                    if (typeof(T).IsAssignableFrom(field.FieldType))
                    {
                        var val = field.GetValue(instance) as T;
                        if (val != null && !result.ContainsKey(field.Name))
                        {
                            result.Add(field.Name, val);
                        }
                    }
                }
                type = type.BaseType;
            }

            // Designer components 컨테이너 탐색
            var componentsField = instance.GetType().GetField("components", BindingFlags.Instance | BindingFlags.NonPublic);
            if (componentsField?.GetValue(instance) is IContainer container)
            {
                foreach (var component in container.Components)
                {
                    if (component is T target)
                    {
                        // 컴포넌트는 필드 이름을 알 수 없는 경우가 많으므로 타입명이나 고유 속성 활용
                        string key = (component as Component)?.Site?.Name ?? $"{typeof(T).Name}_{component.GetHashCode()}";
                        if (!result.ContainsKey(key)) result.Add(key, target);
                    }
                }
            }

            return result;
        }

        public static void ClearCache(object? instance)
        {
            if (instance == null) return;
            var keysToRemove = _componentCache.Keys.Where(k => k.StartsWith($"{instance.GetHashCode()}_")).ToList();
            foreach (var key in keysToRemove) _componentCache.Remove(key);
        }
    }
}
