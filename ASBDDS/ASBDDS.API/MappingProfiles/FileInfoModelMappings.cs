using ASBDDS.Shared.Dtos.DbConsole;
using ASBDDS.Shared.Dtos.File;
using ASBDDS.Shared.Models.Database.DataDb;
using AutoMapper;

namespace ASBDDS.NET.MappingProfiles
{
    public class FileMappings : Profile
    {
        public FileMappings()
        {
            CreateMap<FileInfoModel, FileInfoModelDto>().ReverseMap();
        }
    }
}