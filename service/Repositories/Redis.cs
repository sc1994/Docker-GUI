using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using DockerGui.Tools;
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
            where T : class
        {
            var r = db.ListLeftPop(key, flags);
            if (r == default) return default;
            return JsonConvert.DeserializeObject<T>(r);
        }

        public static long ListRightPush<T>(this IDatabase db, RedisKey key, T value, When when = When.Always, CommandFlags flags = CommandFlags.None)
            where T : class
        {
            return db.ListRightPush(key, value.Serialize(), when, flags);
        }

        public static long ListRemove<T>(this IDatabase db, RedisKey key, T value, long count = 0, CommandFlags flags = CommandFlags.None)
            where T : class
        {
            return db.ListRemove(key, value.Serialize(), count, flags);
        }

        public static async Task<IEnumerable<T>> ListRangeAsync<T>(this IDatabase db, RedisKey key, Func<T, bool> predicate, int limit = -1, CommandFlags flags = CommandFlags.None)
            where T : class
        {
            var values = await db.ListRangeAsync(key, 0);
            var t = values.Select(x => x.Deserialize<T>())
                          .Where(predicate);
            if (limit > -1)
            {
                return t.Take(limit);
            }
            return t;
        }
    }

    public class RedisKeys
    {
        /// <summary>
        /// 哨兵列表数据
        /// </summary>
        public static string SentryList(SentryEnum sentry, string id) => $"list:sentry:{id}:{sentry.GetHashCode()}";

        /// <summary>
        /// 扩展SentryList到具体统计粒度
        /// </summary>
        /// <param name="key"></param>
        /// <param name="grading"></param>
        /// <returns></returns>
        public static string SentryStatsList(SentryEnum sentry, string id, SentryStatsGapEnum grading) => $"{SentryList(sentry, id)}:{grading.GetHashCode()}";
    }
}