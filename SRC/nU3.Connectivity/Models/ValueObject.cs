using System;
using System.Collections.Generic;

namespace nU3.Connectivity.Models
{
    /// <summary>
    /// Legacy Backend와 통신하기 위한 데이터 컨테이너 (Java ValueObject 대응)
    /// </summary>
    public class ValueObject : Dictionary<string, object>
    {
        public ValueObject() { }
        public ValueObject(IDictionary<string, object> dictionary) : base(dictionary) { }

        public string GetString(string key)
        {
            return this.ContainsKey(key) && this[key] != null ? this[key].ToString() : string.Empty;
        }

        public int GetInt(string key)
        {
            return this.ContainsKey(key) && this[key] != null ? Convert.ToInt32(this[key]) : 0;
        }

        public long GetLong(string key)
        {
            return this.ContainsKey(key) && this[key] != null ? Convert.ToInt64(this[key]) : 0L;
        }

        public bool GetBool(string key)
        {
            return this.ContainsKey(key) && this[key] != null && Convert.ToBoolean(this[key]);
        }

        public new ValueObject Add(string key, object value)
        {
            base.Add(key, value);
            return this;
        }

        public ValueObject Set(string key, object value)
        {
            this[key] = value;
            return this;
        }
    }
}
