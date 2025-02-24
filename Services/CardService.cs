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
    public class CardService : ICardService
    {
        private readonly ICardRepository _cardRepository;
        private readonly IMapper _mapper;

        public CardService(ICardRepository cardRepository, IMapper mapper)
        {
            _cardRepository = cardRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CardModelDTO>> GetCardsAsync()
        {
            var cards = await _cardRepository.GetAllCardsAsync();
            return _mapper.Map<IEnumerable<CardModelDTO>>(cards);
        }

        public async Task<CardModelDTO> CreateCardAsync(CardModelDTO cardDTO)
        {
            var card = _mapper.Map<CardModel>(cardDTO);
            var createdCard = await _cardRepository.AddCardAsync(card);
            return _mapper.Map<CardModelDTO>(createdCard);
        }

        public async Task<CardModelDTO?> GetCardByIdAsync(int id)
        {
            var card = await _cardRepository.GetCardByIdAsync(id);
            return card == null ? null : _mapper.Map<CardModelDTO>(card);
        }

        public async Task<CardModelDTO?> UpdateCardAsync(int id, CardModelDTO cardDTO)
        {
            var existingCard = await _cardRepository.GetCardByIdAsync(id);
            if (existingCard == null) return null;

            _mapper.Map(cardDTO, existingCard);

            var updatedCard = await _cardRepository.UpdateCardAsync(existingCard); // Now correctly returns a CardModel
            return _mapper.Map<CardModelDTO>(updatedCard);
        }

        public async Task<CardModelDTO?> PatchCardAsync(int id, JsonPatchDocument<CardModelDTO> updatedProperties)
        {
            var existingCard = await _cardRepository.GetCardByIdAsync(id);
            if (existingCard == null)
            {
                return null; // Handle the case where the card doesn't exist
            }
            var cardDTO = _mapper.Map<CardModelDTO>(existingCard);
            Debug.WriteLine($"Patch Operations: {updatedProperties.Operations.Count}");
            foreach (var operation in updatedProperties.Operations)
            {
                Debug.WriteLine($"Op: {operation.op}, Path: {operation.path}, Value: {operation.value}");
            }

            // Apply only the modified properties
            updatedProperties.ApplyTo(cardDTO);

            cardDTO.Id = existingCard.Id;

            _mapper.Map(cardDTO, existingCard);

            var updatedCard = await _cardRepository.PatchCardAsync(existingCard);
            return _mapper.Map<CardModelDTO>(updatedCard);
        }


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
