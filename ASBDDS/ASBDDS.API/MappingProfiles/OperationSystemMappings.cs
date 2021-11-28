using ASBDDS.Shared.Dtos.File;
using ASBDDS.Shared.Dtos.OperationSystem;
using ASBDDS.Shared.Models.Database.DataDb;
using AutoMapper;

namespace ASBDDS.NET.MappingProfiles
{
    public class OperationSystemMappings : Profile
    {
        public OperationSystemMappings()
        {
            CreateMap<OperationSystemModel, OperationSystemDto>()
                .ForMember(
                    os => os.Arch, 
                    opt => opt.AddTransform(val => val.ToLower()))
                .ForMember(
                    os => os.Name, 
                    opt => opt.AddTransform(val => val.ToLower()))
                .ForMember(
                    os => os.Version, 
                    opt => opt.AddTransform(val => val.ToLower()))
                .ReverseMap();
            CreateMap<OperationSystemModel, OperationSystemCreateDto>()
                .ForMember(
                    os => os.Arch, 
                    opt => opt.AddTransform(val => val.ToLower()))
                .ForMember(
                    os => os.Name, 
                    opt => opt.AddTransform(val => val.ToLower()))
                .ForMember(
                    os => os.Version, 
                    opt => opt.AddTransform(val => val.ToLower()))
                .ReverseMap();
            CreateMap<OperationSystemModel, OperationSystemUpdateDto>()
                .ForMember(
                    os => os.Arch, 
                    opt => opt.AddTransform(val => val.ToLower()))
                .ForMember(
                    os => os.Name, 
                    opt => opt.AddTransform(val => val.ToLower()))
                .ForMember(
                    os => os.Version, 
                    opt => opt.AddTransform(val => val.ToLower()))
                .ReverseMap();
        }
    }
}