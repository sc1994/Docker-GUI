using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using DockerGui.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Docker.DotNet;
using Docker.DotNet.Models;
using DockerGui.Controllers.Containers;
using DockerGui.Repositories;
using Newtonsoft.Json;
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
        public void Start()
        {
            // var contailers = await _container.GetContainerList();
            var ids = new[] { "edad3df3b64f" };
            GetClient(client =>
            {
                var tokens = new List<CancellationTokenSource>();
                foreach (var id in ids)
                {
                    tokens.Add(StartLogs(client, id));
                }
                return tokens;
            });
        }

        private CancellationTokenSource StartLogs(DockerClient client, string id)
        {
            var cancellationTokenSource = new CancellationTokenSource();
            var progress = new Progress<string>();

            progress.ProgressChanged += (obj, message) =>
            {
                var length = Redis.Database.ListRightPush(RedisKeys.SentryLog(id), JsonConvert.SerializeObject(new { }));
                _log.LogDebug("id:{id}--length:{length}", id, length);
            };

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
    }
}