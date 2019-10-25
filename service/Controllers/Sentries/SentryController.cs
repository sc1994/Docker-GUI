using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using DockerGui.Hubs;
using Microsoft.AspNetCore.Mvc;
using DockerGui.Cores.Containers;
using DockerGui.Cores.Sentries;
using System.Threading.Tasks;
using System.Linq;
using DockerGui.Repositories;

namespace DockerGui.Controllers.Sentries
{
    public class SentryController : ApiBaseController
    {
        private readonly IHubContext<BaseHub> _hub;
        private readonly ILogger<SentryController> _log;
        private readonly IContainerCore _container;
        private readonly ISentry _sentry;

        public SentryController(
            IHubContext<BaseHub> hub,
            ILogger<SentryController> log,
            IContainerCore container,
            ISentry sentry
        ) : base(hub, log)
        {
            _hub = hub;
            _log = log;
            _container = container;
            _sentry = sentry;
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
                Task.Run(async () =>
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
                });
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
    }
}