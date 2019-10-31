using AutoMapper;
using Docker.DotNet.Models;
using DockerGui.Service.Controllers.Containers.Dtos;
using DockerGui.Service.Controllers.Sentries.Dtos;
using DockerGui.Service.Cores.Sentries.Models;

namespace DockerGui.Service.Configs
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