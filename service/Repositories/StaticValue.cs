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
        public static Dictionary<SentryStatsGapEnum, (int limit, List<SentryStats> ware)> SENTRY_STATS_ROLE { get; } = new Dictionary<SentryStatsGapEnum, (int limit, List<SentryStats> ware)>
        {
            { SentryStatsGapEnum.Second, (1800, new List<SentryStats>()) }, // 最近30分钟
            { SentryStatsGapEnum.ThreeSeconds, (2400, new List<SentryStats>() )}, // 最近2小时
            { SentryStatsGapEnum.TenSeconds, (2880, new List<SentryStats>() )}, // 最近6小时
            { SentryStatsGapEnum.ThirtySeconds, (2880, new List<SentryStats>()) }, // 最近1天
            { SentryStatsGapEnum.Minute, (2880, new List<SentryStats>() )}, // 最近2天
            { SentryStatsGapEnum.ThreeMinute, (2880, new List<SentryStats>()) }, // 最近6天
            { SentryStatsGapEnum.TenMinute, (2880, new List<SentryStats>() )}, // 最近20天
            { SentryStatsGapEnum.ThirtyMinute, (2880, new List<SentryStats>()) }, // 最近60天
            { SentryStatsGapEnum.Hour, (2880, new List<SentryStats>() )}, // 最近120天
            { SentryStatsGapEnum.ThreeHour, (2880, new List<SentryStats>()) } // 最近360天(一年)
        };
    }
}