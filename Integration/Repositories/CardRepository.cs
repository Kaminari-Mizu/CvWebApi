using Microsoft.EntityFrameworkCore;
using Context;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Integration
{
    public class CardRepository : ICardRepository
    {
        private readonly CvWebContext _context;

        public CardRepository(CvWebContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CardModel>> GetAllCardsAsync()
        {
            return await _context.Cards.Include(c => c.Badges).ToListAsync();
        }

        public async Task<CardModel> AddCardAsync(CardModel card)
        {
            _context.Cards.Add(card);
            await _context.SaveChangesAsync();
            return card;
        }
    }
}
