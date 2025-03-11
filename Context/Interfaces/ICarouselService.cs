using CvWebApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain;

///<summary>
///This class sets up the interfaces for the various methods used within CarouselServices.
///</summary>
namespace Domain
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
