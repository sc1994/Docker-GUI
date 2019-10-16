using DockerGui.Controllers;
using DockerGui.Controllers.Images;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using src.Hubs;

namespace service.Controllers.Identities
{
    /// <summary>
    /// 
    /// </summary>
    public class IdentityController : ApiBaseController
    {
        private readonly IHubContext<BaseHub> _hub;
        private readonly ILogger<IdentityController> _log;

        public IdentityController(
            IHubContext<src.Hubs.BaseHub> hub,
            ILogger<IdentityController> log
        ) : base(hub, log)
        {
            _hub = hub;
            _log = log;
        }
    }
}