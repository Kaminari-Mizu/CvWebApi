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

        public async Task<CarouselModelDTO?> GetCarouselByIdAsync(int id)
        {
            var carousel = await _carouselRepository.GetCarouselByIdAsync(id);
            return carousel == null ? null : _mapper.Map<CarouselModelDTO>(carousel);
        }

        public async Task<CarouselModelDTO?> UpdateCarouselAsync(int id, CarouselModelDTO carouselDTO)
        {
            var existingCarousel = await _carouselRepository.GetCarouselByIdAsync(id);
            if (existingCarousel == null)
                return null;

            _mapper.Map(carouselDTO, existingCarousel);
            await _carouselRepository.UpdateCarouselAsync(existingCarousel);
            return _mapper.Map<CarouselModelDTO>(existingCarousel);
        }

        public async Task<bool> DeleteCarouselAsync(int id)
        {
            var carousel = await _carouselRepository.GetCarouselByIdAsync(id);
            if (carousel == null)
                return false;

            await _carouselRepository.DeleteCarouselAsync(id);
            return true;
        }
    }
}
