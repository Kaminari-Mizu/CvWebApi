using Microsoft.EntityFrameworkCore;
using Context;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.AspNetCore.JsonPatch;

namespace Integration
{
    /// <summary>
    /// This class manages all interactions with the database with relation to the CardModel entity.
    /// It interacts with CvWebContext (EF Core DbContext) to perform CRUD operations on the database.
    /// This class follows the **Repository Pattern** which abstracts the data access logic from the business logic.
    /// </summary>
    public class CardRepository : ICardRepository
    {
        private readonly CvWebContext _context;
        /// <summary>
        /// Initializes a new instance of the CardRepository class.
        /// Injects the EF Core DbContext instance into the repository to enable database interaction.
        /// </summary>
        /// <param name="context"> Is the EF Core DbContext instance for accessing the CardModel entity</param> 
        public CardRepository(CvWebContext context)
        {
            _context = context;
        }

        /// <summary>
        /// This function fetches all cards, including their associated badges, from the database asynchronously.
        /// </summary>
        /// <returns>A list of all CardModel objects, as well as their associated BadgeModel collection</returns>
        public async Task<IEnumerable<CardModel>> GetAllCardsAsync()
        {
            try
            {
                return await _context.Cards
                    .Include(c => c.Badges) //Eager loading to include related badges
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching all cards: {ex.Message}");
                return new List<CardModel>(); //Return an empty list if an error occurs to prevent crashes
            }
        }

        /// <summary>
        /// Retrieves a specific card by its unique ID.
        /// </summary>
        /// <param name="id">The unique identifier of the card.</param>
        /// <returns>The CardModel object if found, otherwise null.</returns>
        public async Task<CardModel?> GetCardByIdAsync(int id)
        {
            try
            {
                if (id <= 0)
                    throw new ArgumentException("Card ID must be greater than zero.", nameof(id));

                return await _context.Cards
                    .Include(c => c.Badges)
                    .FirstOrDefaultAsync(c => c.Id == id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching card with ID {id}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Adds a new card to the database.
        /// Uses a **transaction** to ensure data integrity by rolling back changes if an error occurs.
        /// </summary>
        /// <param name="card">The CardModel object to be added.</param>
        /// <returns>The newly added CardModel object.</returns>
        public async Task<CardModel> AddCardAsync(CardModel card)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (card == null)
                    throw new ArgumentNullException(nameof(card), "Card data cannot be null.");

                await _context.Cards.AddAsync(card);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return card;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"Error adding card: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Updates an existing card in the database.
        /// </summary>
        /// <param name="card">The CardModel object containing updated data.</param>
        /// <returns>The updated CardModel object.</returns>
        public async Task<CardModel> UpdateCardAsync(CardModel card)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (card == null)
                    throw new ArgumentNullException(nameof(card), "Card data cannot be null.");

                _context.Entry(card).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return card;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"Error updating card with ID {card.Id}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Partially updates a card using **JsonPatchDocument**.
        /// This allows modifying only specific properties of the CardModel without sending full object.
        /// </summary>
        /// <param name="card">The CardModel object with modified properties.</param>
        /// <returns>The patched CardModel object.</returns>
        public async Task<CardModel> PatchCardAsync(CardModel card)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (card == null)
                    throw new ArgumentNullException(nameof(card), "Card data cannot be null.");

                _context.Cards.Attach(card);
                _context.Entry(card).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return card;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"Error during patch operation for card with ID {card.Id}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Deletes a card from the database by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the card to be deleted.</param>
        public async Task DeleteCardAsync(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (id <= 0)
                    throw new ArgumentException("Card ID must be greater than zero.", nameof(id));

                var card = await _context.Cards.FindAsync(id);
                if (card == null)
                {
                    Console.WriteLine($"Attempted to delete non-existent card with ID {id}.");
                    return; //Early return to prevent errors
                }

                _context.Cards.Remove(card);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"Error deleting card with ID {id}: {ex.Message}");
                throw;
            }
        }
    }
}
