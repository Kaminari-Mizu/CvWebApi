using AutoMapper;
using CvWebApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Context;
using Integration;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.JsonPatch;
using System.Diagnostics;

namespace Services
{
    /// <summary>
    /// This class handles all business logic related to the CardModel entity.
    /// It interacts with CardRepository for data persistence and uses AutoMapper to map entities
    /// to their DTOs and vice versa.
    /// This class follows the **Service Pattern** which abstracts the business logic from the controller and data access.
    /// </summary>
    public class CardService : ICardService
    {
        private readonly ICardRepository _cardRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the CardService class.
        /// Injects the repository and mapper instances for dependency management.
        /// </summary>
        /// <param name="cardRepository"> The repository responsible for data operations</param>
        /// <param name="mapper">AutoMapper instance for object mapping</param>
        public CardService(ICardRepository cardRepository, IMapper mapper)
        {
            _cardRepository = cardRepository;
            _mapper = mapper;
        }
        /// <summary>
        /// Retrieves all cards from database asynchronously.
        /// </summary>
        /// <returns>A collection of CardModelDTO objects</returns>
        public async Task<IEnumerable<CardModelDTO>> GetCardsAsync()
        {
            var cards = await _cardRepository.GetAllCardsAsync();
            return _mapper.Map<IEnumerable<CardModelDTO>>(cards);
        }

        /// <summary>
        /// Creates a new card in the database.
        /// </summary>
        /// <param name="cardDTO">The DTO containing the card details</param>
        /// <returns>The newly created CardModelDTO object</returns>
        public async Task<(CardModelDTO DTO, int Id)> CreateCardAsync(CardModelDTO cardDTO)
        {
            var card = _mapper.Map<CardModel>(cardDTO);
            var createdCard = await _cardRepository.AddCardAsync(card);
            return (_mapper.Map<CardModelDTO>(createdCard), createdCard.Id);
        }

        /// <summary>
        /// Retrieves a specific card by its unique ID.
        /// </summary>
        /// <param name="id">Unique identifier of the card</param>
        /// <returns>The specific CardModelDTO object if found, otherwise null</returns>
        public async Task<CardModelDTO?> GetCardByIdAsync(int id)
        {
            var card = await _cardRepository.GetCardByIdAsync(id);
            return card == null ? null : _mapper.Map<CardModelDTO>(card);
        }

        /// <summary>
        /// Updates an existing card in the database.
        /// </summary>
        /// <param name="id">Unique identifier of card to be updated</param>
        /// <param name="cardDTO">The updtaded DTO containing the modified card details</param>
        /// <returns>The updated CardModelDTO object if successful, otherwise null</returns>
        public async Task<CardModelDTO?> UpdateCardAsync(int id, CardModelDTO cardDTO)
        {
            var existingCard = await _cardRepository.GetCardByIdAsync(id);
            if (existingCard == null) return null;

            _mapper.Map(cardDTO, existingCard);
            //existingCard.Id = id;

            var updatedCard = await _cardRepository.UpdateCardAsync(existingCard); // Now correctly returns a CardModel
            return _mapper.Map<CardModelDTO>(updatedCard);
        }

        /// <summary>
        /// Partially updates existing card in database using JsonPatchDocument.
        /// This method thus only apples the modified properties to the card instead of replacing the entire entity.
        /// </summary>
        /// <param name="id">The unique identifier of card to patch</param>
        /// <param name="updatedProperties">A JsonPatchDocument containing the changes</param>
        /// <returns>The partially updated CardModelDTO if successful, otherwise null</returns>
        public async Task<CardModelDTO?> PatchCardAsync(int id, JsonPatchDocument<CardModelDTO> updatedProperties)
        {
            var existingCard = await _cardRepository.GetCardByIdAsync(id);
            if (existingCard == null)
            {
                return null; // Handle the case where the card doesn't exist
            }
            //Convert the existing CardModel to a CardModelDTO to apply patch operations
            var cardDTO = _mapper.Map<CardModelDTO>(existingCard);

            // Debugging: Output the patch operations to the console
            Debug.WriteLine($"Patch Operations: {updatedProperties.Operations.Count}");
            foreach (var operation in updatedProperties.Operations)
            {
                Debug.WriteLine($"Op: {operation.op}, Path: {operation.path}, Value: {operation.value}");
            }

            // Apply only the modified properties to the DTO object
            updatedProperties.ApplyTo(cardDTO);

            //Preserve the original ID after patching
           // cardDTO.Id = existingCard.Id;

            // Map the DTO back to the CardModel (original entity)
            _mapper.Map(cardDTO, existingCard);

            //Persist the patched changes in the database
            var updatedCard = await _cardRepository.PatchCardAsync(existingCard);
            return _mapper.Map<CardModelDTO>(updatedCard);
        }

        /// <summary>
        /// Deletes a card from the database.
        /// </summary>
        /// <param name="id">The unique identifier of the card to delete.</param>
        /// <returns>True if deletion was successful, otherwise false.</returns>
        public async Task<bool> DeleteCardAsync(int id)
        {
            var card = await _cardRepository.GetCardByIdAsync(id);
            if (card == null)
                return false;

            await _cardRepository.DeleteCardAsync(id);
            return true;
        }
    }
}
