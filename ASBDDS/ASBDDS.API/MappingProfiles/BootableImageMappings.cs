using ASBDDS.Shared.Dtos.BootableImage;
using ASBDDS.Shared.Models.Database.DataDb;
using AutoMapper;

namespace ASBDDS.NET.MappingProfiles
{
    public class BootableImageMappings : Profile
    {
        public BootableImageMappings()
        {
            CreateMap<BootableImage, BootableImageDto>()
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
            CreateMap<BootableImage, BootableImageCreateDto>()
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
            CreateMap<BootableImage, BootableImageUpdateDto>()
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