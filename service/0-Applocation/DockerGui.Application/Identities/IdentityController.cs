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
        private readonly ILogger<IdentityController> _log;

        public IdentityController(
            ILogger<IdentityController> log
        ) : base(log)
        {
            _log = log;
        }
    }
}