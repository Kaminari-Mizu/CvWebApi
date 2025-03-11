using CvWebApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain;
using Microsoft.AspNetCore.JsonPatch;

///<summary>
///This class sets up the interfaces for the various methods used within CardServices.
/// </summary>
namespace Domain
{
    public interface ICardService
    {
        Task<IEnumerable<CardModelDTO>> GetCardsAsync();
        Task<(CardModelDTO DTO, int Id)> CreateCardAsync(CardModelDTO cardDTO);
        Task<CardModelDTO?> GetCardByIdAsync(int id);
        Task<CardModelDTO?> UpdateCardAsync(int id, CardModelDTO cardDTO);
        Task<CardModelDTO?> PatchCardAsync(int id, JsonPatchDocument<CardModelDTO> updateProperties);
        Task<bool> DeleteCardAsync(int id);
    }

}
