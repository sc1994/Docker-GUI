using System.Collections.Generic;
using System.Threading.Tasks;
using Docker.DotNet;
using DockerGui.Controllers.Containers.Dtos;

namespace DockerGui.Cores.Containers
{
    public interface IContainerCore
    {
        Task<IList<ContainerListResponseDto>> GetContainerListAsync(DockerClient client, bool refresh = false);
    }
}