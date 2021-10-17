using ASBDDS.Shared.Dtos.SerialPortSettings;
using ASBDDS.Shared.Models.Database.DataDb;
using AutoMapper;

namespace ASBDDS.NET.MappingProfiles
{
    public class SerialPortSettingsMappings : Profile
    {
        public SerialPortSettingsMappings()
        {
            CreateMap<SerialPortSettings, AdminSerialPortSettingsDto>().ReverseMap();
            CreateMap<SerialPortSettings, AdminSerialPortSettingsUpdateDto>().ReverseMap();
            CreateMap<SerialPortSettings, AdminSerialPortSettingsCreateDto>().ReverseMap();
        }
    }
}