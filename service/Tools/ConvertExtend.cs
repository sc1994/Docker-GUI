using Newtonsoft.Json;

namespace service.Tools
{
    public static class ConvertExtend
    {
        public static string Serialize(this object value, Formatting formatting, params JsonConverter[] converters)
        {
            return JsonConvert.SerializeObject(value, formatting, converters);
        }

        public static T Deserialize<T>(this string value, JsonSerializerSettings settings)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(value, settings);
            }
            catch
            {
                return default;
            }
        }
    }
}