using Microsoft.EntityFrameworkCore;
using Context;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.JsonPatch;

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
            return await _context.Cards
                .Include(c => c.Badges) // Ensure badges are loaded
                .ToListAsync();
        }

        public async Task<CardModel?> GetCardByIdAsync(int id)
        {
            return await _context.Cards
                .Include(c => c.Badges) // Ensure badges are loaded
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<CardModel> AddCardAsync(CardModel card)
        {
            await _context.Cards.AddAsync(card);
            await _context.SaveChangesAsync();
            return card;
        }

        public async Task<CardModel> UpdateCardAsync(CardModel card)
        {

            
            //_context.Cards.Attach(card);
            _context.Entry(card).State = EntityState.Modified; // Mark entity as modified

            await _context.SaveChangesAsync();
            return card;
        }

        public async Task<CardModel> PatchCardAsync(CardModel card)
        {
            

            _context.Cards.Attach(card);
            _context.Entry(card).State = EntityState.Modified; // Mark entity as modified
            await _context.SaveChangesAsync();
            return card;
        }

        public async Task DeleteCardAsync(int id)
        {
            var card = await _context.Cards.FindAsync(id);
            if (card != null)
            {
                _context.Cards.Remove(card);
                await _context.SaveChangesAsync();
            }
        }
    }

}
