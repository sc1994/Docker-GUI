using System;
using Docker.DotNet;
using Microsoft.AspNetCore.Mvc;

namespace DockerGui.Controllers
{
    [ApiController]
    [Route("v{v}/[controller]")]
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

        protected T GetClientAsync<T>(Func<DockerClient, T> func)
        {
            using (var client = new DockerClientConfiguration(new Uri("http://localhost:2375")).CreateClient())
            {
                return func(client);
            }
        }
    }
}