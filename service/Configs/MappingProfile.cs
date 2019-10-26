using AutoMapper;
using DockerGui.Controllers.Sentries.Dtos;
using DockerGui.Cores.Sentries.Models;

namespace service.Configs
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Add as many of these lines as you need to map your objects
            CreateMap<SentryStats, SentryStatsDto>();
            CreateMap<SentryStats.ReadWrite, SentryStatsDto.ReadWriteDto>();
            CreateMap<SentryStats.UnitValue, SentryStatsDto.UnitValueDto>();
        }
    }
}