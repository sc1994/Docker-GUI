using System;
using System.Diagnostics;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace DockerGui.Repositories
{
    public static class Redis
    {
        private static IDatabase _db;
        public static IDatabase Database
        {
            get
            {
                if (_db == null)
                {
                    var connect = ConnectionMultiplexer.Connect("localhost:6379,password=1qaz2wsx3edc");
                    connect.ConnectionFailed += (obj, @event) =>
                    {
                        Debug.WriteLine("");
                        Debug.WriteLine($"-----------------------{DateTime.Now}: Redis ConnectionFailed-----------------------");
                        Debug.WriteLine(obj);
                        Debug.WriteLine(@event);
                        Debug.WriteLine("");
                    };
                    _db = connect.GetDatabase();
                }
                return _db;
            }
        }
    }

    public static class RedisExtend
    {
        public static T ListLeftPop<T>(this IDatabase db, RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            var r = db.ListLeftPop(key, flags);
            if (r == default) return default;
            return JsonConvert.DeserializeObject<T>(r);
        }

        public static long ListRightPush<T>(this IDatabase db, RedisKey key, T value, When when = When.Always, CommandFlags flags = CommandFlags.None)
            where T : class
        {
            if (value == null) return db.ListLength(key);
            return db.ListRightPush(key, JsonConvert.SerializeObject(value), when, flags);
        }
    }

    public class RedisKeys
    {
        /// <summary>
        /// 哨兵列表数据
        /// </summary>
        public static string SentryList(SentryEnum sentry, string id) => $"list:sentry:{id}:{sentry.GetHashCode()}";

        public static string SentryStatsList(string key, SentryStatsGapEnum grading) => $"{key}:{grading.GetHashCode()}";
    }
}