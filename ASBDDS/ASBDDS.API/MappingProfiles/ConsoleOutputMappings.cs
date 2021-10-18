using ASBDDS.API.Models;
using ASBDDS.Shared.Dtos;
using AutoMapper;

namespace ASBDDS.NET.MappingProfiles
{
    public class ConsoleOutputMappings : Profile
    {
        public ConsoleOutputMappings()
        {
            CreateMap<ConsoleOutput, ConsoleOutputDto>().ReverseMap();
        }
    }
}