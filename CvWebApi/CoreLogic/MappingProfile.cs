using AutoMapper;
using Context;

namespace CvWebApi.CoreLogic
{
    /// <summary>
    /// The MappingProfile class is used to define the mapping between the entities and their DTOs.
    /// </summary>
    public class MappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the MappingProfile class.
        /// and subsequently defines the mapping rules between the entities and their DTOs.
        /// </summary>
        public MappingProfile()
        {
            // CardModel ↔ CardModelDTO Mapping
            CreateMap<CardModel, CardModelDTO>()
                .ForMember(dest => dest.Badges, opt => opt.MapFrom(src => src.Badges)) //Maps related badges
                //.ForMember(dest => dest.Id, opt => opt.Ignore()) // Ignore Id for updates (handled separately)
                .ReverseMap(); // 🔹 Enables reverse/bidirectional mapping from DTO to Entity

            // BadgeModel ↔ BadgeModelDTO Mapping
            CreateMap<BadgeModel, BadgeModelDTO>()
                .ReverseMap(); //Enables reverse/bidirectional mapping from DTO to Entity

            // CarouselModel ↔ CarouselModelDTO Mapping
            CreateMap<CarouselModel, CarouselModelDTO>()
                .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.Images))
                .ReverseMap(); // 🔹 Enables reverse/bidirectional mapping from DTO to Entity

            // CarouselImageModel ↔ CarouselImageDTO Mapping
            CreateMap<CarouselImageModel, CarouselImageDTO>()
                .ReverseMap(); //Enables reverse/bidirectional mapping from DTO to Entity
        }
    }
}
