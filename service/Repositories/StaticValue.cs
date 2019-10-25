using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Docker.DotNet.Models;
using DockerGui.Cores.Sentries.Models;

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
        public static ConcurrentDictionary<(SentryEnum type, string id), CancellationTokenSource> SENTRY_THREAD { get; } = new ConcurrentDictionary<(SentryEnum type, string id), CancellationTokenSource>();

        /// <summary>
        /// 哨兵统计数据收集规则
        /// </summary>
        public static List<SentryRole> SENTRY_STATS_ROLE { get; } = new List<SentryRole>
        {
            new SentryRole(SentryStatsGapEnum.ThreeSeconds),
            new SentryRole(SentryStatsGapEnum.Second),
            new SentryRole(SentryStatsGapEnum.TenSeconds),
            new SentryRole(SentryStatsGapEnum.ThirtySeconds),
            new SentryRole(SentryStatsGapEnum.Minute),
            new SentryRole(SentryStatsGapEnum.ThreeMinute),
            new SentryRole(SentryStatsGapEnum.TenMinute),
            new SentryRole(SentryStatsGapEnum.ThirtyMinute)
        };
    }
}