using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Docker.DotNet;
using Docker.DotNet.Models;
using DockerGui.Core.Sentries.Models;
using DockerGui.EfCore;
using DockerGui.Entity;
using DockerGui.Redis;
using DockerGui.Tools.Values;
using Microsoft.Extensions.Logging;

namespace DockerGui.Core.Sentries
{
    public class Sentry : ISentry
    {
        private readonly ILogger<Sentry> _log;
        private readonly IRedisContext _redis;
        private readonly IMySqlContext _dbContext;
        private readonly IMapper _mapper;

        public Sentry(
            ILogger<Sentry> log,
            IRedisContext redis,
            IMySqlContext dbContext,
            IMapper mapper)
        {
            _log = log;
            _redis = redis;
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<List<string>> GetLogsAsync(string id, int page, int count)
        {
            var key = RedisKeys.SentryList(SentryEnum.Log, id);

            var l = await _redis.ListLengthAsync(key);
            if (l < 1) return new List<string>();

            var s = l - count * page;
            if (s < 1) return new List<string>();

            var e = l - count * (page - 1);
            if (e < 0) e = 0;

            var r = await _redis.ListRangeAsync(
                key,
                l - count * page,
                l - count * (page - 1)
            );
            return r.Select(x => x.ToString()).ToList();
        }

        public async Task<List<SentryStats>> GetStatsAsync(string id, DateTime[] timeRange)
        {
            var timeSpan = timeRange[1] - timeRange[0];
            SentryRoleItem role = null;
            foreach (var item in new SentryRole().List)
            {
                if (item.UseLimit >= timeSpan.TotalSeconds)
                {
                    role = item;
                    break;
                }
            }
            if (role == null) throw new Exception($"{timeRange[0]}~{timeRange[1]}没有任何可以匹配的展示规则, 请重新选择时间范围");

            var key = RedisKeys.SentryStatsList(SentryEnum.Stats, id, role.SecondGap);
            var f = await _redis.ListRangeAsync<SentryStats>(key, x => x.Time >= timeRange[0] && x.Time <= timeRange[1]);

            return f.OrderBy(x => x.Time).ToList();
        }

        public CancellationTokenSource StartLogs(DockerClient client,
                                                 string id,
                                                 Action<string, string, long> backCall = null)
        {
            var cancellationTokenSource = new CancellationTokenSource();
            var progress = new Progress<string>();
            var queue = new ConcurrentQueue<string>();
            var key = RedisKeys.SentryList(SentryEnum.Log, id);
            // 重置这个redis
            _redis.KeyDelete(key);

            progress.ProgressChanged += (obj, message) =>
            {
                queue.Enqueue(message);
            };

            _ = Task.Run(async () => // 每 5ms 存储一次
            {
                while (true)
                {
                    if (queue.TryDequeue(out var message))
                    {
                        var rule = "[0-9]{4}-[0-9]{2}-[0-9]{2}T[0-9]{2}:[0-9]{2}:[0-9]{2}.[0-9]{9}Z";
                        var time = Regex.Matches(message, rule)[0].Value;
                        var v = message.Split(new[] { time }, StringSplitOptions.None)[1]
                                       .Replace("\u001b[40m\u001b[1m\u001b[33mwarn\u001b[39m\u001b[22m\u001b[49m:", "[warn]")
                                       .Replace("\u001B[41m\u001B[30mfail\u001B[39m\u001B[22m\u001B[49m", "[fail]");
                        var l = _redis.ListRightPush(key, new { time, log = v });
                        if (backCall != null)
                        {
                            backCall(id, v, l);
                        }
                    }
                    await Task.Delay(5);
                }
            }, cancellationTokenSource.Token);

            _ = client.Containers.GetContainerLogsAsync(
                id,
                new ContainerLogsParameters
                {
                    // as the error said, you have to choose one stream either stdout or stderr. If you don't input any of these option to be true, it would panic.
                    ShowStdout = true,
                    ShowStderr = true,
                    Timestamps = true,
                    Follow = true
                },
                cancellationTokenSource.Token,
                progress
            );

            return cancellationTokenSource;
        }

        public void StartStats(string id)
        {
            using var client = new DockerClientConfiguration(new Uri("http://localhost:2375")).CreateClient();

            var cancellationTokenSource = new CancellationTokenSource();
            var progress = new Progress<ContainerStatsResponse>();
            var role = new SentryRole();
            var time = DateTime.Now;
            time = time.AddSeconds(-time.Second);

            progress.ProgressChanged += (obj, message) =>
            {
                try
                {
                    lock (Consts.LOCKSTATSADD)
                    {
                        message.Read = time; // 统一时间为调用时间,以少量的时间误差,换取数据间隔的整齐
                        var stats = new SentryStats(message);
                        var d = _dbContext.StatsEntity.Add(_mapper.Map<StatsEntity>(stats));
                        _dbContext.SaveChanges();
                    }

                    // _redis.ListRightPush(getKey(SentryStatsGapEnum.Minute), stats);
                    // foreach (var item in role.List)
                    // {
                    //     item.TempList.Add(stats); // 添加到规则汇总
                    //     if (item.TempList.Count >= item.SecondGap.GetHashCode()) // 满足规则
                    //     {
                    //         var x = MixSentryStats(item.TempList); // 混合规则汇总的数据
                    //         item.TempList.Clear(); // 清空规则汇总的数据(为下一次汇总做准备)
                    //         var l = _redis.Database.ListRightPush(getKey(item.SecondGap), x); // 添加到对应redis
                    //         if (backCall != null)
                    //         {
                    //             backCall(id, x, item.SecondGap, l);
                    //         }
                    //     }
                    // }
                }
                catch (Exception ex)
                {
                    _log.LogError(ex, "");
                }
            };

            _ = client.Containers.GetContainerStatsAsync(
                id,
                new ContainerStatsParameters
                {
                    Stream = false
                },
                progress,
                cancellationTokenSource.Token
            );
        }

        /// <summary>
        /// 混合统计数据
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private SentryStats MixSentryStats(List<SentryStats> list)
        {
            if (list == null || !list.Any()) return null;
            if (list.Count == 1) return list[0];
            return new SentryStats
            {
                ContainerId = string.Join(",", list.Select(x => x.ContainerId).Distinct()),
                Time = list.OrderBy(x => x.Time).FirstOrDefault().Time,
                Pids = (ulong)list.Avg(x => x.Pids),
                CpuPercent = list.Avg(x => x.CpuPercent).ToFixed(2),
                MemoryPercent = list.Avg(x => x.MemoryPercent).ToFixed(2),
                MemoryValue = new SentryStatsUnitValue(
                    list.Avg(x => x.MemoryValue.MinUnit),
                    list.Avg(x => x.MemoryValue.Digit),
                    list.Avg(x => x.MemoryValue.SourceValue)
                ),
                MemoryLimit = new SentryStatsUnitValue(
                    list.Avg(x => x.MemoryLimit.MinUnit),
                    list.Avg(x => x.MemoryLimit.Digit),
                    list.Avg(x => x.MemoryLimit.SourceValue)
                ),
                Nets = list.SelectMany(x => x.Nets?.ToList() ?? new List<KeyValuePair<string, SentryStatsReadWrite>>())
                    .GroupBy(x => x.Key)
                    .ToDictionary(
                         x => x.Key,
                         x => new SentryStatsReadWrite
                         {
                             Read = new SentryStatsUnitValue(
                                  x.Avg(a => a.Value.Read.MinUnit),
                                  x.Avg(a => a.Value.Read.Digit),
                                  x.Avg(a => a.Value.Read.SourceValue)
                              ),
                             Write = new SentryStatsUnitValue(
                                  x.Avg(a => a.Value.Write.MinUnit),
                                  x.Avg(a => a.Value.Write.Digit),
                                  x.Avg(a => a.Value.Write.SourceValue)
                              )
                         }
                    ),
                Block = new SentryStatsReadWrite
                {
                    Read = new SentryStatsUnitValue(
                        list.Avg(a => a.Block.Read.MinUnit),
                        list.Avg(a => a.Block.Read.Digit),
                        list.Avg(a => a.Block.Read.SourceValue)
                    ),
                    Write = new SentryStatsUnitValue(
                        list.Avg(a => a.Block.Write.MinUnit),
                        list.Avg(a => a.Block.Write.Digit),
                        list.Avg(a => a.Block.Write.SourceValue)
                    )
                }
            };
        }
    }
}