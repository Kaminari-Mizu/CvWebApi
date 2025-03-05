using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Context;
using Integration;
using CvWebApi.Models;
using NUnit.Framework; // Using NUnit.Framework
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Linq;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace IntegrationTests
{
    /// <summary>
    /// Integration tests for the CardRepository, testing its interaction with the in-memory database.
    /// These tests focus on the repository layer downwards, excluding controllers and services.
    /// </summary>
    [TestFixture]
    public class CardRepositoryIntegrationTests
    {
        /// <summary>
        /// Creates a fresh in-memory database and CardRepository instance for each test.
        /// </summary>
        private async Task<(CvWebContext, ICardRepository)> CreateTestContextAsync()
        {
            var serviceProvider = new ServiceCollection()
                .AddDbContext<CvWebContext>(options =>
                    options.UseInMemoryDatabase("TestDatabase_" + Guid.NewGuid().ToString())
                           .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                           .ReplaceService<IModelCustomizer, NoSeedModelCustomizer>())
                .BuildServiceProvider();

            var context = serviceProvider.GetRequiredService<CvWebContext>();
            context.Database.EnsureDeleted(); // Start fresh
            context.Database.EnsureCreated();
            Console.WriteLine("Database created fresh.");

            // Debug: Check initial state
            var initialCards = await context.Cards.ToListAsync();
            Console.WriteLine($"Initial cards after creation: {initialCards.Count}");
            foreach (var card in initialCards)
            {
                Console.WriteLine($"Initial Card: ID={card.Id}, Title={card.Title}");
            }

            var repository = new CardRepository(context);
            return (context, repository);
        }

        private async Task InitializeDb(CvWebContext context)
        {
            var card = new CardModel
            {
                Title = "Seed Card",
                Image = "seed.jpg",
                Country = "Seed Country",
                Description = "Seed Description",
                Badges = new List<BadgeModel> { new BadgeModel { Emoji = "🌟", Label = "Seed Badge" } }
            };
            context.Cards.Add(card);
            await context.SaveChangesAsync();
            Console.WriteLine($"Seeded Card: ID={card.Id}, Title={card.Title}");
        }

        private async Task CleanupDb(CvWebContext context)
        {
            context.Database.EnsureDeleted();
            Console.WriteLine("Database reset.");
        }

        [Test]
        public async Task GetAllCardsAsync_OneCardSeeded_ShouldReturnSingleCards()
        {
            // Arrange
            var (context, repository) = await CreateTestContextAsync();
            await InitializeDb(context);

            // Debug: Verify database state before acting
            var dbCardsBefore = await context.Cards.ToListAsync();
            Console.WriteLine($"Cards in DB before GetAllCardsAsync: {dbCardsBefore.Count}");
            foreach (var card in dbCardsBefore)
            {
                Console.WriteLine($"DB Card: ID={card.Id}, Title={card.Title}");
            }

            // Act
            var cards = await repository.GetAllCardsAsync();

            // Assert
            var cardList = cards.ToList();
            Assert.That(cardList, Is.Not.Empty); // Modern syntax for non-empty collection
            Console.WriteLine($"Retrieved Cards: {cardList.Count}");
            foreach (var card in cardList)
            {
                Console.WriteLine($"Retrieved Card: ID={card.Id}, Title={card.Title}");
            }

            // Find the seeded card by title
            var seededCard = cardList.FirstOrDefault(c => c.Title == "Seed Card");
            Assert.That(seededCard, Is.Not.Null); // Modern syntax for not null
            Assert.That(seededCard.Title, Is.EqualTo("Seed Card")); // Modern syntax for equality
            Assert.That(seededCard.Image, Is.EqualTo("seed.jpg"));

            // Ensure only one card exists
            Assert.That(cardList.Count, Is.EqualTo(1));

            // Cleanup
            await CleanupDb(context);
        }

        /// <summary>
        /// Verifies that AddCardAsync creates a new card and persists it to the database.
        /// </summary>
        [Test]
        public async Task AddCardAsync_ValidCard_ShouldCreateAndPersistsCard()
        {
            // Arrange
            var (context, repository) = await CreateTestContextAsync();
            var card = new CardModel
            {
                Title = "Test Card",
                Image = "test.jpg",
                Country = "Test Country",
                Description = "Test Description",
                Badges = new List<BadgeModel> { new BadgeModel { Emoji = "jjj", Label = "rrr" } }
            };

            // Act
            var createdCard = await repository.AddCardAsync(card);

            // Assert
            Assert.That(createdCard, Is.Not.Null); // Modern syntax for not null
            Assert.That(createdCard.Id, Is.GreaterThan(0)); // Modern syntax for greater than
            Assert.That(createdCard.Title, Is.EqualTo(card.Title));
            Assert.That(createdCard.Image, Is.EqualTo(card.Image));
            Assert.That(createdCard.Country, Is.EqualTo(card.Country));
            Assert.That(createdCard.Description, Is.EqualTo(card.Description));
            var createdBadge = createdCard.Badges.FirstOrDefault();
            Assert.That(createdBadge, Is.Not.Null);
            Assert.That(createdBadge.Emoji, Is.EqualTo("jjj"));
            Assert.That(createdBadge.Label, Is.EqualTo("rrr"));

            // Verify persistence in the database
            var dbCard = await context.Cards.Include(c => c.Badges).FirstOrDefaultAsync(c => c.Id == createdCard.Id);
            Assert.That(dbCard, Is.Not.Null);
            Assert.That(dbCard.Title, Is.EqualTo("Test Card"));

            // Cleanup
            await CleanupDb(context);
        }

        /// <summary>
        /// Verifies that UpdateCardAsync updates an existing card in the database.
        /// </summary>
        [Test]
        public async Task UpdateCardAsync_ExistingCard_ShouldUpdateAndPersistsCard()
        {
            // Arrange
            var (context, repository) = await CreateTestContextAsync();
            await InitializeDb(context);
            var existingCard = await context.Cards.FirstAsync();
            existingCard.Title = "Updated Title";
            existingCard.Image = "updated.jpg";
            existingCard.Badges = new List<BadgeModel> { new BadgeModel { Emoji = "sad", Label = "Updated Badge" } };

            // Act
            var updatedCard = await repository.UpdateCardAsync(existingCard);

            // Assert
            Assert.That(updatedCard, Is.Not.Null); // Modern syntax for not null
            Assert.That(updatedCard.Title, Is.EqualTo("Updated Title"));
            Assert.That(updatedCard.Image, Is.EqualTo("updated.jpg"));
            var updatedBadge = updatedCard.Badges.FirstOrDefault();
            Assert.That(updatedBadge, Is.Not.Null);
            Assert.That(updatedBadge.Emoji, Is.EqualTo("sad"));
            Assert.That(updatedBadge.Label, Is.EqualTo("Updated Badge"));

            // Verify persistence in the database
            var dbCard = await context.Cards.Include(c => c.Badges).FirstOrDefaultAsync(c => c.Id == existingCard.Id);
            Assert.That(dbCard, Is.Not.Null);
            Assert.That(dbCard.Title, Is.EqualTo("Updated Title"));
            Assert.That(dbCard.Image, Is.EqualTo("updated.jpg"));

            // Cleanup
            await CleanupDb(context);
        }

        /// <summary>
        /// Verifies that DeleteCardAsync removes a card from the database.
        /// </summary>
        [Test]
        public async Task DeleteCardAsync_ValidId_ShouldRemoveCard()
        {
            // Arrange
            var (context, repository) = await CreateTestContextAsync();
            await InitializeDb(context);
            var card = await context.Cards.FirstAsync();
            int cardId = card.Id;

            // Act
            await repository.DeleteCardAsync(cardId);

            // Assert
            var deletedCard = await context.Cards.FirstOrDefaultAsync(c => c.Id == cardId);
            Assert.That(deletedCard, Is.Null); // Modern syntax for null

            // Cleanup
            await CleanupDb(context);
        }
    }

    // Custom model customizer to disable seeding
    public class NoSeedModelCustomizer : IModelCustomizer
    {
        public void Customize(ModelBuilder modelBuilder, DbContext context)
        {
            // Do nothing to avoid applying HasData from CvWebContext
        }
    }
}