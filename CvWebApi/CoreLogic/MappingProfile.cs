using AutoMapper;
using CvWebApi.Models;

namespace CvWebApi.CoreLogic
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Map from CardModel to CardModelDTO
            CreateMap<CardModel, CardModelDTO>()
                .ForMember(dest => dest.Badges, opt => opt.MapFrom(src => src.Badges));

            // Map from BadgeModel to BadgeModelDTO
            CreateMap<BadgeModel, BadgeModelDTO>();

            // Map from CarouselModel to CarouselModelDTO
            CreateMap<CarouselModel, CarouselModelDTO>()
                .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.Images));

            // Map from CarouselImageModel to CarouselImageDTO
            CreateMap<CarouselImageModel, CarouselImageDTO>();
        }
    }
}
