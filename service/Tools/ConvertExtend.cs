using System;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace DockerGui
{
    public static class ConvertExtend
    {
        public static string Serialize(this object value)
        {
            if (value == null) return string.Empty;
            return JsonConvert.SerializeObject(value);
        }

        public static T Deserialize<T>(this string value)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(value)) return default;
                return JsonConvert.DeserializeObject<T>(value);
            }
            catch
            {
                return default;
            }
        }

        public static T Deserialize<T>(this RedisValue value)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(value)) return default;
                return JsonConvert.DeserializeObject<T>(value);
            }
            catch
            {
                return default;
            }
        }
    }
}