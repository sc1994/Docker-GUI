using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DockerGui.Application.Sentries.Dtos;
using DockerGui.Core.Containers;
using DockerGui.Core.Hubs;
using DockerGui.Core.Sentries;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;


namespace DockerGui.Application.Sentries
{
    public class SentryController : ApiBaseController
    {
        private readonly ILogger<SentryController> _log;
        private readonly IContainerCore _container;
        private readonly ISentry _sentry;
        private readonly IMapper _mapper;

        public SentryController(
            ILogger<SentryController> log,
            IContainerCore container,
            ISentry sentry,
            IMapper mapper
        ) : base(log)
        {
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
                foreach (var id in ids)
                {
                    // if (!StaticValue.SENTRY_THREAD.ContainsKey((SentryEnum.Log, id)))
                    //     StaticValue.SENTRY_THREAD.TryAdd(
                    //         (SentryEnum.Log, id),
                    //         _sentry.StartLogs(Client, id, (_, __, ___) =>
                    //         {

                    //         })
                    //     );
                    var job = Hangfire.Common.Job.FromExpression<ISentry>(x => x.StartStats(id));
                    var manager = new RecurringJobManager(JobStorage.Current);
                    manager.AddOrUpdate($"stats_{id}", job, Cron.Minutely(), TimeZoneInfo.Local);
                }
                _log.LogWarning("Sentry started");
                return "Done";
            }
        }

        // [HttpGet("stop")]
        // public async Task<string> Stop()
        // {
        //     var contailers = await _container.GetContainerListAsync(Client);
        //     var ids = contailers.Select(x => x.ID).ToArray();

        //     foreach (var id in ids)
        //     {
        //         if (StaticValue.SENTRY_THREAD.TryRemove((SentryEnum.Log, id), out var v1))
        //         {
        //             v1.Cancel();
        //             v1.Dispose();
        //         };
        //         if (StaticValue.SENTRY_THREAD.TryRemove((SentryEnum.Stats, id), out var v2))
        //         {
        //             v2.Cancel();
        //             v2.Dispose();
        //         };
        //     }

        //     return "Done";
        // }

        [HttpGet("{id}/{page}/{count}/log")]
        public async Task<IEnumerable<string>> GetLogs(string id, int page, int count)
        {
            return await _sentry.GetLogsAsync(id, page, count);
        }

        [HttpGet("{id}/{start}/{end}/stats")]
        public async Task<IEnumerable<SentryStatsDto>> GetStats(string id, DateTime start, DateTime end)
        {
            var data = await _sentry.GetStatsAsync(id, new[] { start, end });
            var r = data.Select(_mapper.Map<SentryStatsDto>);
            return r;
        }
    }
}