using System;
using System.Threading.Tasks;
using DockerGui.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace DockerGui.Hubs
{
    public interface IBaseHub
    {
        Task Pong(DateTime time);
    }

    public class BaseHub : Hub<IBaseHub>
    {
        private readonly IHttpContextAccessor _accessor; // http上下文
        private readonly ILogger<BaseHub> _log;

        public BaseHub(
            IHttpContextAccessor accessor,
            ILogger<BaseHub> log
        )
        {
            _accessor = accessor;
            _log = log;
        }

        public async Task Ping()
        {
            await Clients.Caller.Pong(DateTime.Now);
        }

        public override async Task OnConnectedAsync()
        {
            if (_accessor.HttpContext.Request.Query.TryGetValue("token", out var v))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, v);
                _log.LogDebug($"OnConnectedAsync({v})");
            }
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            if (_accessor.HttpContext.Request.Query.TryGetValue("token", out var v))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, v);
                if (StaticValue.MONITOR_THREAD.TryRemove(v, out var c))
                {
                    c.Cancel();
                    c.Dispose();
                }
                _log.LogDebug($"OnDisconnectedAsync({v})");
            }
        }
    }
}