using DockerGui.Core.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace DockerGui.Application.Identities
{
    /// <summary>
    /// 
    /// </summary>
    public class IdentityController : ApiBaseController
    {
        private readonly IHubContext<BaseHub> _hub;
        private readonly ILogger<IdentityController> _log;

        public IdentityController(
            IHubContext<BaseHub> hub,
            ILogger<IdentityController> log
        ) : base(hub, log)
        {
            _hub = hub;
            _log = log;
        }
    }
}