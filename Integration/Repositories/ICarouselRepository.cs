using CvWebApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Context;

namespace Integration
{
    public interface ICarouselRepository
    {
        Task<IEnumerable<CarouselModel>> GetAllCarouselsAsync();
        Task<CarouselModel> AddCarouselAsync(CarouselModel carousel);
    }
}
