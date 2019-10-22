using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Docker.DotNet.Models;

namespace DockerGui.Repositories
{
    /// <summary>
    /// 静态值,
    /// </summary>
    public static class StaticValue
    {
        public static List<ImagesListResponse> ALL_IMAGES { get; } = new List<ImagesListResponse>();

        /// <summary>
        /// 监控中的线程集合
        /// </summary>
        /// <typeparam name="string">type_token</typeparam>
        /// <typeparam name="CancellationTokenSource"></typeparam>
        /// <returns></returns>
        public static ConcurrentDictionary<string, CancellationTokenSource> MONITOR_THREAD { get; } = new ConcurrentDictionary<string, CancellationTokenSource>();

        /// <summary>
        /// 监控中(哨兵监控,常住内存)的线程
        /// </summary>
        public static ConcurrentDictionary<string, CancellationTokenSource> SENTRY_THREAD { get; } = new ConcurrentDictionary<string, CancellationTokenSource>();
    }
}