using System;
using Docker.DotNet;
using Microsoft.AspNetCore.Mvc;

namespace DockerGui.Controllers
{
    [ApiController]
    [Route("v1/[controller]")]
    public class ApiBaseController : ControllerBase
    {
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
            using (var client = new DockerClientConfiguration(new Uri("http://localhost:2375")).CreateClient())
            {
                action(client);
            }
        }

        protected T GetClientAsync<T>(Func<DockerClient, T> func)
        {
            using (var client = new DockerClientConfiguration(new Uri("http://localhost:2375")).CreateClient())
            {
                return func(client);
            }
        }
    }
}