using System.Collections.Generic;

namespace DockerGui.Application.Containers.Dtos
{
    public class SetStatusResponseDto
    {
        public bool Result { get; set; }
        public IList<ContainerListResponseDto> List { get; set; }
    }
}