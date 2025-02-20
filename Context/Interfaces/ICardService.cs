using CvWebApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Context;
using Microsoft.AspNetCore.JsonPatch;

namespace Services
{
    public interface ICardService
    {
        Task<IEnumerable<CardModelDTO>> GetCardsAsync();
        Task<CardModelDTO> CreateCardAsync(CardModelDTO cardDTO);
        Task<CardModelDTO?> GetCardByIdAsync(int id);
        Task<CardModelDTO?> UpdateCardAsync(int id, CardModelDTO cardDTO);
        Task<CardModelDTO?> PatchCardAsync(int id, JsonPatchDocument<CardModelDTO> updateProperties);
        Task<bool> DeleteCardAsync(int id);
    }

}
