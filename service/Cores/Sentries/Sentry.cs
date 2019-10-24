using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Docker.DotNet;
using Docker.DotNet.Models;
using DockerGui.Cores.Sentries.Models;
using DockerGui.Repositories;
using DockerGui.Tools;

namespace DockerGui.Cores.Sentries
{
    public class Sentry : ISentry
    {
        public CancellationTokenSource StartLogs(DockerClient client,
                                                 string id,
                                                 Action<string, long> backCall = null)
        {
            var cancellationTokenSource = new CancellationTokenSource();
            var progress = new Progress<string>();
            var queue = new ConcurrentQueue<string>();
            var key = RedisKeys.SentryList(SentryEnum.Log, id);
            // 重置这个redis
            Redis.Database.KeyDelete(key);

            progress.ProgressChanged += (obj, message) =>
            {
                queue.Enqueue(message);
            };

            _ = Task.Run(async () => // 每 1ms 存储一次
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
                        var l = Redis.Database.ListRightPush(key, new { time, log = v });
                        if (backCall != null)
                        {
                            backCall(v, l);
                        }
                    }
                    await Task.Delay(5);
                }
            });

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

        public CancellationTokenSource StartStats(DockerClient client,
                                                  string id,
                                                  Action<SentryStats, SentryStatsGapEnum, long> backCall = null)
        {
            var cancellationTokenSource = new CancellationTokenSource();
            var progress = new Progress<ContainerStatsResponse>();

            var key = RedisKeys.SentryList(SentryEnum.Stats, id);
            progress.ProgressChanged += (obj, message) =>
            {
                var stats = new SentryStats(message);

                foreach (var item in StaticValue.SENTRY_STATS_ROLE)
                {
                    item.Value.ware.Add(stats); // 添加到规则总
                    if (item.Value.ware.Count == item.Key.GetHashCode()) // 满足规则
                    {
                        var x = MixSentryStats(item.Value.ware); // 混合规则总的数据
                        item.Value.ware.Clear(); // 清空数据
                        var l = Redis.Database.ListRightPush(RedisKeys.SentryStatsList(key, item.Key), x); // 添加到对应redis
                        if (l > item.Value.limit) // 防止redis过大
                        {
                            _ = Redis.Database.ListLeftPop(RedisKeys.SentryStatsList(key, item.Key));
                        }
                        if (backCall != null)
                        {
                            backCall(x, item.Key, l);
                        }
                    }
                }
            };

            _ = client.Containers.GetContainerStatsAsync(
                id,
                new ContainerStatsParameters
                {
                    Stream = true
                },
                progress,
                cancellationTokenSource.Token
            );

            return cancellationTokenSource;
        }

        private SentryStats MixSentryStats(List<SentryStats> list)
        {
            if (list == null || !list.Any()) return null;
            if (list.Count == 1) return list[0];
            return new SentryStats
            {
                Time = list.OrderBy(x => x.Time).FirstOrDefault().Time,
                Pids = (ulong)list.Avg(x => x.Pids),
                CpuPercent = list.Avg(x => x.CpuPercent).ToFixed(2),
                MemoryPercent = list.Avg(x => x.MemoryPercent).ToFixed(2),
                MemoryValue = new SentryStats.UnitValue(
                    list.Avg(x => x.MemoryValue.MinUnit),
                    list.Avg(x => x.MemoryValue.Digit),
                    list.Avg(x => x.MemoryValue.SourceValue)
                ),
                MemoryLimit = new SentryStats.UnitValue(
                    list.Avg(x => x.MemoryLimit.MinUnit),
                    list.Avg(x => x.MemoryLimit.Digit),
                    list.Avg(x => x.MemoryLimit.SourceValue)
                ),
                Nets = list.SelectMany(x => x.Nets?.ToList() ?? new List<KeyValuePair<string, SentryStats.ReadWrite>>())
                    .GroupBy(x => x.Key)
                    .ToDictionary(
                         x => x.Key,
                         x => new SentryStats.ReadWrite
                         {
                             Read = new SentryStats.UnitValue(
                                  x.Avg(a => a.Value.Read.MinUnit),
                                  x.Avg(a => a.Value.Read.Digit),
                                  x.Avg(a => a.Value.Read.SourceValue)
                              ),
                             Write = new SentryStats.UnitValue(
                                  x.Avg(a => a.Value.Write.MinUnit),
                                  x.Avg(a => a.Value.Write.Digit),
                                  x.Avg(a => a.Value.Write.SourceValue)
                              )
                         }
                    ),
                Block = new SentryStats.ReadWrite
                {
                    Read = new SentryStats.UnitValue(
                        list.Avg(a => a.Block.Read.MinUnit),
                        list.Avg(a => a.Block.Read.Digit),
                        list.Avg(a => a.Block.Read.SourceValue)
                    ),
                    Write = new SentryStats.UnitValue(
                        list.Avg(a => a.Block.Write.MinUnit),
                        list.Avg(a => a.Block.Write.Digit),
                        list.Avg(a => a.Block.Write.SourceValue)
                    )
                }
            };
        }
    }
}