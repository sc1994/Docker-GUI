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

        private const int Hour = 3600;
        private const int Day = 86400;
        /// <summary>
        /// 哨兵统计数据收集规则
        /// </summary>
        public static Dictionary<SentryStatsGapEnum, SentryRole> SENTRY_STATS_ROLE { get; } = new Dictionary<SentryStatsGapEnum, SentryRole>
        {
            {
                SentryStatsGapEnum.Second, new SentryRole
                {
                    UseLimit = Hour * 1, // 1小时内使用这个粒度的数据
                    MaxLimit = Day * 1 / SentryStatsGapEnum.Second.GetHashCode() // 最多存储最近1天的数据
                }
            },
            {
                SentryStatsGapEnum.ThreeSeconds, new SentryRole
                {
                    UseLimit = Hour * 2, // 2小时内
                    MaxLimit = Day * 3 / SentryStatsGapEnum.ThreeSeconds.GetHashCode() // 最多存储最近3天的数据
                }
            },
            {
                SentryStatsGapEnum.TenSeconds, new SentryRole
                {
                    UseLimit = Hour * 6, // 6小时内
                    MaxLimit = Day * 10 / SentryStatsGapEnum.TenSeconds.GetHashCode() // 10天
                }
            },
            {
                SentryStatsGapEnum.ThirtySeconds, new SentryRole
                {
                    UseLimit = Hour * 24, // 24小时内
                    MaxLimit = Day * 30 / SentryStatsGapEnum.ThirtySeconds.GetHashCode() // 30天
                }
            },
            {
                SentryStatsGapEnum.Minute, new SentryRole
                {
                    UseLimit = Hour * 24 * 2, // 2天内
                    MaxLimit = Day * 60 / SentryStatsGapEnum.Minute.GetHashCode() // 60天
                }
            },
            {
                SentryStatsGapEnum.ThreeMinute, new SentryRole
                {
                    UseLimit = Hour * 24 * 5, // 5天内
                    MaxLimit = Day * 60 * 3 / SentryStatsGapEnum.ThreeMinute.GetHashCode() // 180天
                }
            },
            {
                SentryStatsGapEnum.TenMinute, new SentryRole
                {
                    UseLimit = Hour * 24 * 10, // 10 天
                    MaxLimit = Day * 60 * 10 / SentryStatsGapEnum.TenMinute.GetHashCode() // 600天
                }
            }
        };
    }
}