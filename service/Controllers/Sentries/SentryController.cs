using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using DockerGui.Hubs;
using Microsoft.AspNetCore.Mvc;
using DockerGui.Cores.Containers;
using DockerGui.Cores.Sentries;
using System.Threading.Tasks;
using System.Linq;
using DockerGui.Repositories;
using System.Collections.Generic;
using System;
using AutoMapper;
using DockerGui.Controllers.Sentries.Dtos;
using DockerGui.Cores.Sentries.Models;

namespace DockerGui.Controllers.Sentries
{
    public class SentryController : ApiBaseController
    {
        private readonly IHubContext<BaseHub> _hub;
        private readonly ILogger<SentryController> _log;
        private readonly IContainerCore _container;
        private readonly ISentry _sentry;
        private readonly IMapper _mapper;

        public SentryController(
            IHubContext<BaseHub> hub,
            ILogger<SentryController> log,
            IContainerCore container,
            ISentry sentry,
            IMapper mapper
        ) : base(hub, log)
        {
            _hub = hub;
            _log = log;
            _container = container;
            _sentry = sentry;
            _mapper = mapper;
        }

        [HttpGet("start")]
        public async Task<string> Start()
        {
            var contailers = await _container.GetContainerListAsync(Client);
            var ids = contailers.Select(x => x.ID).ToArray();
            lock ("1")
            {
                var count = 0L;
                foreach (var id in ids)
                {
                    if (!StaticValue.SENTRY_THREAD.ContainsKey((SentryEnum.Log, id)))
                        StaticValue.SENTRY_THREAD.TryAdd(
                            (SentryEnum.Log, id),
                            _sentry.StartLogs(Client, id, (_, __, ___) =>
                            {
                                count++;
                            })
                        );

                    if (!StaticValue.SENTRY_THREAD.ContainsKey((SentryEnum.Stats, id)))
                        StaticValue.SENTRY_THREAD.TryAdd(
                            (SentryEnum.Stats, id),
                            _sentry.StartStats(Client, id, (_, __, ___, ____) =>
                            {
                                count++;
                            })
                        );
                }

                async Task liveMessage()
                {
                    while (true)
                    {
                        if (count != 0)
                        {
                            _log.LogDebug("Is Live--->" + count);
                            count = 0;
                        }

                        await Task.Delay(3000);
                    }
                }

                _ = Task.Run(liveMessage);
                return "Done";
            }

        }

        [HttpGet("stop")]
        public async Task<string> Stop()
        {
            var contailers = await _container.GetContainerListAsync(Client);
            var ids = contailers.Select(x => x.ID).ToArray();

            foreach (var id in ids)
            {
                if (StaticValue.SENTRY_THREAD.TryRemove((SentryEnum.Log, id), out var v1))
                {
                    v1.Cancel();
                    v1.Dispose();
                };
                if (StaticValue.SENTRY_THREAD.TryRemove((SentryEnum.Stats, id), out var v2))
                {
                    v2.Cancel();
                    v2.Dispose();
                };
            }

            return "Done";
        }

        [HttpGet("{id}/{page}/{count}/log")]
        public async Task<IEnumerable<string>> GetLogs(string id, int page, int count)
        {
            return await _sentry.GetLogsAsync(id, page, count);
        }

        [HttpGet("{id}/{start}/{end}/stats")]
        public async Task<IEnumerable<SentryStatsDto>> GetStats(string id, DateTime start, DateTime end)
        {
            var r = await _sentry.GetStatsAsync(id, new[] { start, end });
            return r.Select(_mapper.Map<SentryStatsDto>);
        }
    }
}