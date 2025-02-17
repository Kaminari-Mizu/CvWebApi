using AutoMapper;
using CvWebApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Context;
using Integration;

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
    }
}
