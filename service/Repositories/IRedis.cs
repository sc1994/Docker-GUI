using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace DockerGui.Repositories
{
    public interface IRedis : IDisposable, IDatabase
    {
        ConnectionMultiplexer Connection { get; }

        long ListRightPush<T>(RedisKey key, T value, When when = When.Always, CommandFlags flags = CommandFlags.None)
            where T : class;

        Task<IEnumerable<T>> ListRangeAsync<T>(RedisKey key, Func<T, bool> predicate, int limit = -1, CommandFlags flags = CommandFlags.None)
            where T : class;
    }
}