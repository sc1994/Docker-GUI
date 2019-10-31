using System;
using Newtonsoft.Json;
using StackExchange.Redis;

public static class ConvertExtend
{
    private static DateTime _startTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);

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

    public static long ToTimeStampMilliseconds(this DateTime time)
    {
        var t = (time.Ticks - _startTime.Ticks) / 10000;
        return t;
    }

    public static long ToTimeStampSeconds(this DateTime time)
    {
        var t = (time.Ticks - _startTime.Ticks) / 10000 / 1000;
        return t;
    }

    public static long ToTimeStampMinutes(this DateTime time)
    {
        var t = (time.Ticks - _startTime.Ticks) / 10000 / 1000 / 60;
        return t;
    }
}