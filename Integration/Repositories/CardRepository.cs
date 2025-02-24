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
            try
            {
                return await _context.Cards
                    .Include(c => c.Badges) // Ensure badges are loaded
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching all cards: {ex.Message}");
                return new List<CardModel>(); // Return an empty list on failure
            }
        }

        public async Task<CardModel?> GetCardByIdAsync(int id)
        {
            try
            {
                if (id <= 0)
                    throw new ArgumentException("Card ID must be greater than zero.", nameof(id));

                return await _context.Cards
                    .Include(c => c.Badges) // Ensure badges are loaded
                    .FirstOrDefaultAsync(c => c.Id == id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching card with ID {id}: {ex.Message}");
                throw;
            }
        }

        public async Task<CardModel> AddCardAsync(CardModel card)
        {
            try
            {
                if (card == null)
                    throw new ArgumentNullException(nameof(card), "Card data cannot be null.");

                await _context.Cards.AddAsync(card);
                await _context.SaveChangesAsync();
                return card;
            }
            catch (DbUpdateException ex) // Database-related error
            {
                Console.WriteLine($"Database error when adding card: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error when adding card: {ex.Message}");
                throw;
            }
        }

        public async Task<CardModel> UpdateCardAsync(CardModel card)
        {
            try
            {
                if (card == null)
                    throw new ArgumentNullException(nameof(card), "Card data cannot be null.");

                _context.Entry(card).State = EntityState.Modified; // Mark entity as modified
                await _context.SaveChangesAsync();
                return card;
            }
            catch (DbUpdateConcurrencyException ex) // Concurrency issue
            {
                Console.WriteLine($"Concurrency conflict when updating card with ID {card.Id}: {ex.Message}");
                throw;
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"Database error when updating card with ID {card.Id}: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error updating card with ID {card.Id}: {ex.Message}");
                throw;
            }
        }

        public async Task<CardModel> PatchCardAsync(CardModel card)
        {


            try
            {
                if (card == null)
                    throw new ArgumentNullException(nameof(card), "Card data cannot be null.");

                _context.Cards.Attach(card);
                _context.Entry(card).State = EntityState.Modified; // Mark entity as modified
                await _context.SaveChangesAsync();
                return card;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                Console.WriteLine($"Concurrency issue during patch operation for card with ID {card.Id}: {ex.Message}");
                throw;
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"Database error during patch operation for card with ID {card.Id}: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error during patch operation for card with ID {card.Id}: {ex.Message}");
                throw;
            }
        }

        public async Task DeleteCardAsync(int id)
        {
            try
            {
                if (id <= 0)
                    throw new ArgumentException("Card ID must be greater than zero.", nameof(id));

                var card = await _context.Cards.FindAsync(id);
                if (card == null)
                {
                    Console.WriteLine($"Attempted to delete non-existent card with ID {id}.");
                    return;
                }

                _context.Cards.Remove(card);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"Database error when deleting card with ID {id}: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error when deleting card with ID {id}: {ex.Message}");
                throw;
            }
        
        }
    }

}
