using System;
using Docker.DotNet;
using DockerGui.Core.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace DockerGui.Application
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApiBaseController : ControllerBase
    {
        private readonly IHubContext<BaseHub> _hub;
        private readonly ILogger<ApiBaseController> _log;

        public ApiBaseController(
            IHubContext<BaseHub> hub,
            ILogger<ApiBaseController> log
        )
        {
            _log = log;
            _hub = hub;
        }

        protected string Token => GetToken();

        private string GetToken()
        {
            if (HttpContext.Request.Headers.TryGetValue("token", out var v))
                return v;
            _log.LogWarning("Get token failure");
            return "";
        }

        protected DockerClient Client => GetClient();

        private DockerClient GetClient()
        {
            using var client = new DockerClientConfiguration(new Uri("http://localhost:2375")).CreateClient();
            return client;
        }
    }
}