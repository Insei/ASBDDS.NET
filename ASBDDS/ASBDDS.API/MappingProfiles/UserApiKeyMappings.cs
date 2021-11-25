using ASBDDS.Shared.Dtos.SerialPortSettings;
using ASBDDS.Shared.Dtos.UserApiKey;
using ASBDDS.Shared.Models.Database.DataDb;
using AutoMapper;

namespace ASBDDS.NET.MappingProfiles
{
    public class UserApiKeyMappings : Profile
    {
        public UserApiKeyMappings()
        {
            CreateMap<UserApiKey, UserApiKeyDto>().ReverseMap();
            CreateMap<UserApiKey, UserApiKeyUpdateDto>().ReverseMap();
            CreateMap<UserApiKey, UserApiKeyCreateDto>().ReverseMap();
        }
    }
}