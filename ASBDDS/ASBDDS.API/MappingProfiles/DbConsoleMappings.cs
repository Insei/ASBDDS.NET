using ASBDDS.Shared.Dtos.DbConsole;
using ASBDDS.Shared.Models.Database.DataDb;
using ASBDDS.Shared.Models.Requests;
using ASBDDS.Shared.Models.Responses;
using AutoMapper;

namespace ASBDDS.NET.MappingProfiles
{
    public class DbConsoleMappings : Profile
    {
        public DbConsoleMappings()
        {
            CreateMap<DbConsole, AdminDbConsoleCreateDto>().ReverseMap();
            CreateMap<DbConsole, AdminDbConsoleUpdateDto>().ReverseMap();
            CreateMap<DbConsole, AdminDbConsoleDto>().ReverseMap();
        }
    }
}