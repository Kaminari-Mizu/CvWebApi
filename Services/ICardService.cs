using CvWebApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Context;

namespace Services
{
    public interface ICardService
    {
        Task<IEnumerable<CardModelDTO>> GetCardsAsync();
        Task<CardModelDTO> CreateCardAsync(CardModelDTO cardDTO);
    }
}
