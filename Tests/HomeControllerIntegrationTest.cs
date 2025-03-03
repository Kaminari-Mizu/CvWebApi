using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Context;
using CvWebApi;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xunit;
using Microsoft.AspNetCore.JsonPatch;
using System.Net.Http.Json;

namespace IntegrationTests
{
    public class HomeControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public HomeControllerIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                // Override services to use an In-Memory Database for tests
                builder.ConfigureServices(services =>
                {
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<CvWebContext>));
                    if (descriptor != null)
                    {
                        services.Remove(descriptor);
                    }

                    // Add In-Memory DbContext for testing
                    services.AddDbContext<CvWebContext>(options =>
                        options.UseInMemoryDatabase("TestDatabase"));
                });
            });

            // Recreate the HttpClient for each test to ensure a fresh client per test
            _client = _factory.CreateClient();
        }

        

        // Setup the in-memory database before tests to ensure state consistency
        private async Task InitializeDb()
        {
            // Scope service provider to retrieve CvWebContext
            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<CvWebContext>();
                context.Database.EnsureCreated();
            }
        }

        // Clean the database after tests
        private async Task CleanupDb()
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<CvWebContext>();
                context.Database.EnsureDeleted();
            }
        }

        [Fact]
        public async Task GetCards_ShouldReturnCards()
        {
            // Arrange: Ensure the DB is initialized and populated
            await InitializeDb();

            // Act: Send a GET request to fetch all cards
            var response = await _client.GetAsync("/api/home/cards");

            // Assert: Check if the response is successful and contains the expected data
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            var cards = JsonConvert.DeserializeObject<List<CardModelDTO>>(responseString);
            Assert.NotEmpty(cards);

            // Clean up the database after the test
            await CleanupDb();
        }

        [Fact]
        public async Task CreateCard_ShouldCreateCard()
        {
            // Arrange: Create a sample card DTO to be posted
            var cardDTO = new CardModelDTO
            {
                Title = "Test Card",
                Image = "test.jpg",
                Country = "Test Country",
                Description = "Test Description",
                Badges = new List<BadgeModelDTO>
                {
                    new BadgeModelDTO { Emoji = "jjj", Label = "rrr" }
                }
            };

            // Act: Send a POST request to create a new card
            var response = await _client.PostAsJsonAsync("/api/home/cards", cardDTO);

            // Assert: Ensure the card was created and returned with status code 201 (Created)
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            var createdCard = JsonConvert.DeserializeObject<CardModelDTO>(responseString);
            Assert.NotEqual(0, createdCard.Id); // Assert that the Id is auto-generated

            // Assert other card fields
            Assert.Equal(cardDTO.Title, createdCard.Title);
            Assert.Equal(cardDTO.Image, createdCard.Image);
            Assert.Equal(cardDTO.Country, createdCard.Country);
            Assert.Equal(cardDTO.Description, createdCard.Description);

            // Assert badge details
            var createdBadge = createdCard.Badges.FirstOrDefault();
            Assert.NotNull(createdBadge);
            Assert.Equal(cardDTO.Badges.First().Emoji, createdBadge.Emoji);
            Assert.Equal(cardDTO.Badges.First().Label, createdBadge.Label);

            // Clean up the database after the test
            await CleanupDb();
        }

        [Fact]
        public async Task UpdateCard_ShouldUpdateCard()
        {
            // Arrange: Create and insert a sample card to update
            var initialCardDTO = new CardModelDTO
            {
                Title = "Initial Title",
                Image = "initial.jpg",
                Country = "Test Country",
                Description = "Test Description",
                Badges = new List<BadgeModelDTO> { new BadgeModelDTO { Emoji = "...", Label = "Test Badge" } }
            };

            var createResponse = await _client.PostAsJsonAsync("/api/home/cards", initialCardDTO);
            createResponse.EnsureSuccessStatusCode();
            var createdCard = JsonConvert.DeserializeObject<CardModelDTO>(await createResponse.Content.ReadAsStringAsync());

            // Log the created card for debugging
            Console.WriteLine($"Created Card: {JsonConvert.SerializeObject(createdCard)}");

            // Make sure the ID is valid
            if (createdCard.Id <= 0)
            {
                throw new InvalidOperationException("Card creation failed, ID is not valid.");
            }

            // Prepare updated card data
            var updatedCardDTO = new CardModelDTO
            {
                Id = createdCard.Id, // Use the actual created card's Id here
                Title = "New Title",
                Image = "New.jpg",
                Country = "Test Country",
                Description = "Test Description",
                Badges = new List<BadgeModelDTO>
                {
                    new BadgeModelDTO { Id = createdCard.Badges.First().Id, Emoji = "sad", Label = "wow" }
                }
            };

            // Log the update request body
            var requestContent = JsonConvert.SerializeObject(updatedCardDTO);
            Console.WriteLine($"Request Body: {requestContent}");

            // Act: Send a PUT request to update the card
            var updateResponse = await _client.PutAsJsonAsync($"/api/home/cards/{createdCard.Id}", updatedCardDTO);

            // Log the response details
            var responseString = await updateResponse.Content.ReadAsStringAsync();
            Console.WriteLine($"Response Body: {responseString}");

            // Assert: Ensure the card was updated successfully
            updateResponse.EnsureSuccessStatusCode();
            var updatedCard = JsonConvert.DeserializeObject<CardModelDTO>(responseString);

            Assert.Equal(updatedCardDTO.Id, updatedCard.Id); // Assert Id matches
            Assert.Equal(updatedCardDTO.Title, updatedCard.Title);
            Assert.Equal(updatedCardDTO.Image, updatedCard.Image);
            Assert.Equal(updatedCardDTO.Country, updatedCard.Country);
            Assert.Equal(updatedCardDTO.Description, updatedCard.Description);

            // Assert the badge details
            var updatedBadge = updatedCard.Badges.FirstOrDefault();
            Assert.NotNull(updatedBadge);
            Assert.Equal(updatedCardDTO.Badges.First().Id, updatedBadge.Id); // Ensure badge Id is correct
            Assert.Equal(updatedCardDTO.Badges.First().Emoji, updatedBadge.Emoji);
            Assert.Equal(updatedCardDTO.Badges.First().Label, updatedBadge.Label);

            // Clean up the database after the test
            await CleanupDb();
        }

        // Similar updates for PatchCard and DeleteCard tests

    }
}
