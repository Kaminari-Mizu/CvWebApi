using CvWebApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Context;

namespace Integration
{
    public interface ICardRepository
    {
        Task<IEnumerable<CardModel>> GetAllCardsAsync();
        Task<CardModel> AddCardAsync(CardModel card);
    }
}
