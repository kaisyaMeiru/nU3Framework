using System;
using System.Collections.Generic;

namespace nU3.Connectivity.Models
{
    /// <summary>
    /// Legacy Backend 통신용 최상위 데이터 컨테이너
    /// </summary>
    public class ValueObjectAssembler : Dictionary<string, object>
    {
        public ValueObjectAssembler() { }

        public ValueObject Get(string key)
        {
            if (this.ContainsKey(key))
            {
                if (this[key] is ValueObject vo)
                    return vo;
                
                // Newtonsoft/System.Text.Json deserialization might result in JObject or JsonElement
                // We might need handling here if explicit casting fails
                if (this[key] is IDictionary<string, object> dict)
                    return new ValueObject(dict);
            }
            return null;
        }

        public ValueObjectAssembler Set(string key, ValueObject vo)
        {
            this[key] = vo;
            return this;
        }

        public static ValueObjectAssembler Create()
        {
            return new ValueObjectAssembler();
        }
    }
}
