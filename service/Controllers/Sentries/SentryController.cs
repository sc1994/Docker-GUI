using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using DockerGui.Hubs;
using System.Collections.Generic;
using DockerGui.Repositories;
using Microsoft.AspNetCore.Mvc;


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

            foreach (var id in ids)
            {
                // StaticValue.SENTRY_THREAD.TryAdd(
                //     (SentryEnum.Log, id),
                //     StartLogs(client, id)
                // );
                // StaticValue.SENTRY_THREAD.TryAdd(
                //     (SentryEnum.Stats, id),
                //     StartStats(client, id)
                // );
            }
            
            return "Done";
        }
    }
}