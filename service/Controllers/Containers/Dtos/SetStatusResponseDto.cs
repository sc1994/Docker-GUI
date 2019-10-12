using System.Collections.Generic;
using Docker.DotNet.Models;

namespace src.Controllers.Containers.Dtos
{
    public class SetStatusResponseDto
    {
        public bool Result { get; set; }
        public IList<ContainerListResponseDto> List { get; set; }
    }
}