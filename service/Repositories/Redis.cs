using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DockerGui.Values;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace DockerGui.Repositories
{
    public class Redis : IRedis
    {
        private readonly IConfiguration _config;
        private readonly ILogger<Redis> _logger;
        public Redis(IConfiguration config, ILogger<Redis> logger)
        {
            _config = config;
            _logger = logger;
        }

        private ConnectionMultiplexer _connection;

        public ConnectionMultiplexer Connection
        {
            get
            {
                if (_connection == null)
                {
                    _logger.LogInformation("Create redis connection");
                    _connection = ConnectionMultiplexer.Connect(_config.GetConnectionString("redis"));
                    _connection.ConnectionFailed += (obj, @event) =>
                    {
                        _logger.LogError("Redis connectionFailed {obj} {event}", obj, @event);
                        _connection.Dispose();
                        _connection.Close();
                        _connection = null;
                    };
                }
                return _connection;
            }
        }

        public IDatabase Database
        {
            get
            {
                return Connection.GetDatabase();
            }
        }

        public void Dispose()
        {
            if (_connection != null)
            {
                _connection.Dispose();
                _connection.Close();
            }
        }
    }

    public interface IRedis : IDisposable
    {
        ConnectionMultiplexer Connection { get; }
        IDatabase Database { get; }
    }

    public static class RedisExtend
    {
        public static long Append<T>(this IDatabase db, RedisKey key, T value, When when = When.Always, CommandFlags flags = CommandFlags.None)
            where T : class
        {
            return db.ListRightPush(key, value.Serialize(), when, flags);
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