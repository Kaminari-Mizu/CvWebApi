using AutoMapper;
using CvWebApi.Models;
using Context;

namespace CvWebApi.CoreLogic
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // CardModel ↔ CardModelDTO
            CreateMap<CardModel, CardModelDTO>()
                .ForMember(dest => dest.Badges, opt => opt.MapFrom(src => src.Badges))
                .ReverseMap(); // 🔹 Enables reverse mapping from DTO to Entity

            // BadgeModel ↔ BadgeModelDTO
            CreateMap<BadgeModel, BadgeModelDTO>()
                .ReverseMap();

            // CarouselModel ↔ CarouselModelDTO
            CreateMap<CarouselModel, CarouselModelDTO>()
                .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.Images))
                .ReverseMap(); // 🔹 Enables reverse mapping from DTO to Entity

            // CarouselImageModel ↔ CarouselImageDTO
            CreateMap<CarouselImageModel, CarouselImageDTO>()
                .ReverseMap();
        }
    }
}
