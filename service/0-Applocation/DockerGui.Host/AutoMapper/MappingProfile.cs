using AutoMapper;
using Docker.DotNet.Models;
using DockerGui.Application.Containers.Dtos;
using DockerGui.Application.Sentries.Dtos;
using DockerGui.Core.Sentries.Models;

namespace DockerGui.Host.AutoMapper
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
