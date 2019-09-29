using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace src.Hubs
{
    public interface IBaseHub
    {
        Task Pong(DateTime time);
    }

    public class BaseHub : Hub<IBaseHub>
    {
        public async Task Ping()
        {
            await Clients.Caller.Pong(DateTime.Now);
        }
    }
}