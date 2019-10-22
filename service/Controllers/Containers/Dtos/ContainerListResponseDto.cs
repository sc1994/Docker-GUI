using Docker.DotNet.Models;

namespace DockerGui.Controllers.Containers.Dtos
{
    public class ContainerListResponseDto : ContainerListResponse
    {
        public string CreatedStr { get; set; }
    }
}