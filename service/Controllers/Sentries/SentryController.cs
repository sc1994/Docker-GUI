using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using DockerGui.Hubs;
using System;
using System.Collections.Generic;
using System.Threading;
using Docker.DotNet;
using Docker.DotNet.Models;
using DockerGui.Repositories;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using service.Controllers.Sentries.Dtos;
using service.Tools;
using System.Linq;

namespace DockerGui.Controllers.Sentries
{
    public class SentryController : ApiBaseController
    {
        private readonly IHubContext<BaseHub> _hub;
        private readonly ILogger<SentryController> _log;
        // private readonly ContainerController _container;

        public SentryController(
            IHubContext<BaseHub> hub,
            ILogger<SentryController> log
        ) : base(hub, log)
        {
            _hub = hub;
            _log = log;
        }

        [HttpGet("start")]
        public string Start()
        {
            // var contailers = await _container.GetContainerList();
            var ids = new[]
            {
                "edad3df3b64f",
                // "d6a639c35ad"
            };
            return GetClient(client =>
            {
                foreach (var id in ids)
                {
                    // StaticValue.SENTRY_THREAD.TryAdd(
                    //     (SentryEnum.Log, id),
                    //     StartLogs(client, id)
                    // );
                    StaticValue.SENTRY_THREAD.TryAdd(
                        (SentryEnum.Stats, id),
                        StartStats(client, id)
                    );
                }
                return "Done";
            });
        }

        private CancellationTokenSource StartLogs(DockerClient client, string id)
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
                        _log.LogDebug("id:{id}---length:{length}", id, l);
                    }
                    await Task.Delay(3);
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

        private CancellationTokenSource StartStats(DockerClient client, string id)
        {
            var cancellationTokenSource = new CancellationTokenSource();
            var progress = new Progress<ContainerStatsResponse>();

            var l1 = new List<SentryStats>(); // 第一级())
            var l2 = new List<SentryStats>(); // 一小时
            var l3 = new List<SentryStats>(); // 三小时

            Dictionary<SentryStatsGapEnum, List<SentryStats>> role = new Dictionary<SentryStatsGapEnum, List<SentryStats>>
            {
                { SentryStatsGapEnum.Second,new List<SentryStats>() },
                { SentryStatsGapEnum.ThreeSeconds,new List<SentryStats>() },
                { SentryStatsGapEnum.TenSeconds,new List<SentryStats>() },
                { SentryStatsGapEnum.ThirtySeconds,new List<SentryStats>() },
                { SentryStatsGapEnum.Minute,new List<SentryStats>() },
                { SentryStatsGapEnum.ThreeMinute,new List<SentryStats>() },
                { SentryStatsGapEnum.TenMinute,new List<SentryStats>() },
                { SentryStatsGapEnum.ThirtyMinute,new List<SentryStats>() },
                { SentryStatsGapEnum.Hour,new List<SentryStats>() },
                { SentryStatsGapEnum.ThreeHour,new List<SentryStats>() },
            };

            var key = RedisKeys.SentryList(SentryEnum.Stats, id);
            progress.ProgressChanged += (obj, message) =>
            {
                var l = Redis.Database.ListRightPush(RedisKeys.SentryStatsList(key, SentryStatsGapEnum.Second), new SentryStats(message));
                _log.LogDebug("id:{id}---length:{length}---key:{key}", id, l, SentryStatsGapEnum.Second);

                // var t = Redis.Database.ListLeftPop<SentryStats>(RedisKeys.SentryStatsList(key, SentryStatsGapEnum.Second));
                // l1.Add(t);
                // l2.Add(t);
                // l3.Add(t);

                // if (l1.Count == 3) // 半小时聚合
                // {
                //     var m = MixSentryStats(l1);
                //     l1.Clear();
                //     l = Redis.Database.ListRightPush(RedisKeys.SentryStatsList(key, SentryStatsGapEnum.半小时), m);
                //     _log.LogDebug("id:{id}---length:{length}---key:{key}", id, l, SentryStatsGapEnum.半小时);
                // }
                // if (l2.Count == 6) // 一小时聚合
                // {
                //     var m = MixSentryStats(l2);
                //     l2.Clear();
                //     l = Redis.Database.ListRightPush(RedisKeys.SentryStatsList(key, SentryStatsGapEnum.一小时), m);
                //     _log.LogDebug("id:{id}---length:{length}---key:{key}", id, l, SentryStatsGapEnum.一小时);
                // }
                // if (l3.Count == 18) // 三小时聚合
                // {
                //     var m = MixSentryStats(l3);
                //     l3.Clear();
                //     l = Redis.Database.ListRightPush(RedisKeys.SentryStatsList(key, SentryStatsGapEnum.三小时), m);
                //     _log.LogDebug("id:{id}---length:{length}---key:{key}", id, l, SentryStatsGapEnum.三小时);
                // }
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
                Nets = list.SelectMany(x => x.Nets.ToList())
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