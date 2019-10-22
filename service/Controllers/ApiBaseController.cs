using System;
using System.Threading.Tasks;
using Docker.DotNet;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using DockerGui.Hubs;

namespace DockerGui.Controllers
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

        protected T GetClient<T>(Func<DockerClient, T> func)
        {
            var now = DateTime.Now;
            using var client = new DockerClientConfiguration(new Uri("http://localhost:2375")).CreateClient();
            try
            {
                var create = (DateTime.Now - now).TotalMilliseconds;
                now = DateTime.Now;
                var f = func(client);
                _log.LogDebug("Use {ms1} ms do create client\r\nUse {ms2} ms do {name}", create, (DateTime.Now - now).TotalMilliseconds, func.Method.Name);
                return f;
            }
            catch (Exception ex)
            {
                _log.LogWarning("Send error \r\n-----------\r\n{message}      -----------\r\n to {id}", ex.Message, Token);
                _hub.Clients.Group(Token).SendAsync("error", ex.Message);
                throw;
            }
        }

        protected async Task GetClientAsync(Func<DockerClient, Task> func)
        {
            var now = DateTime.Now;
            using var client = new DockerClientConfiguration(new Uri("http://localhost:2375")).CreateClient();
            try
            {
                var create = (DateTime.Now - now).TotalMilliseconds;
                now = DateTime.Now;
                await func(client);
                _log.LogDebug("Use {ms1} ms do create client\r\nUse {ms2} ms do {name}", create, (DateTime.Now - now).TotalMilliseconds, func.Method.Name);
            }
            catch (Exception ex)
            {
                _log.LogWarning("Send error \r\n-----------\r\n{message}      -----------\r\n to {id}", ex.Message, Token);
                await _hub.Clients.Group(Token).SendAsync("error", ex.Message);
                throw;
            }
        }

        protected async Task<T> GetClientAsync<T>(Func<DockerClient, Task<T>> func)
        {
            try
            {
                var now = DateTime.Now;
                using var client = new DockerClientConfiguration(new Uri("http://localhost:2375")).CreateClient();
                var create = (DateTime.Now - now).TotalMilliseconds;
                now = DateTime.Now;
                var f = await func(client);
                _log.LogDebug("Use {ms1} ms do create client\r\nUse {ms2} ms do {name}", create, (DateTime.Now - now).TotalMilliseconds, func.Method.Name);
                return f;
            }
            catch (DockerApiException ex)
            {
                _log.LogWarning("Send error \r\n-----------\r\n{message}      -----------\r\n to {id}", ex.Message, Token);
                await _hub.Clients.Group(Token).SendAsync("error", ex.Message);
                throw;
            }
        }
    }
}