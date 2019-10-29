using System.Collections.Generic;
using System.Threading.Tasks;
using Docker.DotNet;
using Docker.DotNet.Models;

namespace DockerGui.Cores.Containers
{
    public interface IContainerCore
    {
        Task<IList<ContainerListResponse>> GetContainerListAsync(DockerClient client, bool refresh = false);
    }
}