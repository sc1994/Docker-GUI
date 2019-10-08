using System;
using Docker.DotNet;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DockerGui.Controllers
{
    [ApiController]
    [Route("v1/[controller]")]
    public class ApiBaseController : ControllerBase
    {
        private readonly ILogger<ApiBaseController> _log;

        public ApiBaseController(
            ILogger<ApiBaseController> log
        )
        {
            _log = log;
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

        protected string ConnectionId
        {
            get
            {
                try
                {
                    var c = HttpContext.Request.Headers["connectionId"].ToString();
                    return c;
                }
                catch
                {
                    return "";
                }
            }
        }

        protected void GetClientAsync(Action<DockerClient> action)
        {
            var now = DateTime.Now;
            using (var client = new DockerClientConfiguration(new Uri("http://localhost:2375")).CreateClient())
            {
                _log.LogDebug("Use {ms} ms do create client", (DateTime.Now - now).TotalMilliseconds);
                now = DateTime.Now;
                action(client);
                _log.LogDebug("Use {ms} ms do {name}", (DateTime.Now - now).TotalMilliseconds, action.Method.Name);
            }
        }

        protected T GetClientAsync<T>(Func<DockerClient, T> func)
        {
            var now = DateTime.Now;
            using (var client = new DockerClientConfiguration(new Uri("http://localhost:2375")).CreateClient())
            {
                _log.LogDebug("Use {ms} ms do create client", (DateTime.Now - now).TotalMilliseconds);
                now = DateTime.Now;
                var f = func(client);

                _log.LogDebug("Use {ms} ms do {name}", (DateTime.Now - now).TotalMilliseconds, func.Method.Name);
                return f;
            }
        }
    }
}