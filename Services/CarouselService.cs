using AutoMapper;
using CvWebApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Integration;
using Context;
namespace Services
{
    public class CarouselService : ICarouselService
    {
        private readonly ICarouselRepository _carouselRepository;
        private readonly IMapper _mapper;

        public CarouselService(ICarouselRepository carouselRepository, IMapper mapper)
        {
            _carouselRepository = carouselRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CarouselModelDTO>> GetCarouselsAsync()
        {
            var carousels = await _carouselRepository.GetAllCarouselsAsync();
            return _mapper.Map<IEnumerable<CarouselModelDTO>>(carousels);
        }

        public async Task<CarouselModelDTO> CreateCarouselAsync(CarouselModelDTO carouselDTO)
        {
            var carousel = _mapper.Map<CarouselModel>(carouselDTO);
            var createdCarousel = await _carouselRepository.AddCarouselAsync(carousel);
            return _mapper.Map<CarouselModelDTO>(createdCarousel);
        }
    }
}
