using Docker.DotNet.Models;

namespace DockerGui.Application.Containers.Dtos
{
    public class ContainerListResponseDto : ContainerListResponse
    {
        public string CreatedStr { get; set; }
    }
}