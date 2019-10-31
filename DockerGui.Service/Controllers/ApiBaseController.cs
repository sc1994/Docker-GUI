using System;
using System.Threading.Tasks;
using Docker.DotNet;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using DockerGui.Service.Hubs;

namespace DockerGui.Service.Controllers
{
    [ApiController]
    [Route("v1/[controller]")]
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

        protected int Version
        {
            get
            {
                var v = HttpContext.Request.RouteValues["v"].ToString();
                if (int.TryParse(v, out var r))
                {
                    return r;
                }
                return 1;
            }
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