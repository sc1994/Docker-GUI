using Docker.DotNet.Models;

namespace DockerGui.Controllers.Containers.Dtos
{
    public class ContainerListResponseDto : ContainerListResponse
    {
        public string CreatedStr { get; set; }

        public new string ID => base.ID.Substring(0, 6);
    }
}