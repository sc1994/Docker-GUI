using AutoMapper;
using Docker.DotNet.Models;
using DockerGui.Controllers.Containers.Dtos;
using DockerGui.Controllers.Sentries.Dtos;
using DockerGui.Cores.Sentries.Models;

namespace DockerGui.Configs
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<SentryStats, SentryStatsDto>();
            CreateMap<ContainerListResponse, ContainerListResponseDto>();
            CreateMap<SentryStats.ReadWrite, SentryStatsDto.ReadWriteDto>();
            CreateMap<SentryStats.UnitValue, SentryStatsDto.UnitValueDto>();
        }
    }
}