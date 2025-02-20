using CvWebApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Context;


namespace Integration
{
    public interface ICardRepository
    {
        Task<IEnumerable<CardModel>> GetAllCardsAsync();
        Task<CardModel?> GetCardByIdAsync(int id);
        Task<CardModel> AddCardAsync(CardModel card);
        Task<CardModel> UpdateCardAsync(CardModel card);
        Task<CardModel> PatchCardAsync(CardModel card);
        Task DeleteCardAsync(int id);
    }

}
