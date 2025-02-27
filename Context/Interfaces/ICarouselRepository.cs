using CvWebApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Context;

///<summary>
///This class sets up the interfaces for the various methods used within CarouselRepository.
/// </summary>
namespace Integration
{
    public interface ICarouselRepository
    {
        Task<IEnumerable<CarouselModel>> GetAllCarouselsAsync();
        Task<CarouselModel?> GetCarouselByIdAsync(int id);
        Task<CarouselModel> AddCarouselAsync(CarouselModel carousel);
        Task UpdateCarouselAsync(CarouselModel carousel);
        Task DeleteCarouselAsync(int id);
    }

}
