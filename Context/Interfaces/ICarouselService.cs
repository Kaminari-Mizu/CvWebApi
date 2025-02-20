using CvWebApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Context;

namespace Services
{
    public interface ICarouselService
    {
        Task<IEnumerable<CarouselModelDTO>> GetCarouselsAsync();
        Task<CarouselModelDTO> CreateCarouselAsync(CarouselModelDTO carouselDTO);
        Task<CarouselModelDTO?> GetCarouselByIdAsync(int id);
        Task<CarouselModelDTO?> UpdateCarouselAsync(int id, CarouselModelDTO carouselDTO);
        Task<bool> DeleteCarouselAsync(int id);
    }

}
