using AutoMapper;
using Docker.DotNet.Models;
using DockerGui.Application.Containers.Dtos;
using DockerGui.Application.Sentries.Dtos;
using DockerGui.Core.Sentries.Models;

namespace DockerGui.Infrastructure.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ContainerListResponse, ContainerListResponseDto>()
                .ForMember(x => x.CreatedStr, x => x.MapFrom(src => src.Created.ToString("yyyy-MM-dd HH:mm:ss")));

            CreateMap<SentryStats, SentryStatsDto>();
            CreateMap<SentryStatsReadWrite, SentryStatsDto.ReadWriteDto>();
            CreateMap<SentryStatsUnitValue, SentryStatsDto.UnitValueDto>();
        }
    }
}
