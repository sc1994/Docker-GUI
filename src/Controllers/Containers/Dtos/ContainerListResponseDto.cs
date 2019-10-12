using Docker.DotNet.Models;

namespace src.Controllers.Containers.Dtos
{
    public class ContainerListResponseDto : ContainerListResponse
    {
        public string CreatedStr { get; set; }
    }
}