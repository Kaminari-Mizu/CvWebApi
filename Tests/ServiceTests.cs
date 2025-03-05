using NUnit.Framework;
using Moq;
using FluentAssertions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Services;
using Integration;
using Context;
using CvWebApi.Models;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;

namespace Tests
{
    /// <summary>
    /// Unit tests for the CardService class. These tests are focused on isolating the 
    /// CardService methods and verifying that they correctly interact with the CardRepository.
    /// These tests also use mocks to simulate dependencies like the repository and mapper
    /// </summary>
    [TestFixture]
    public class CardServiceTests
    {
        private Mock<ICardRepository> _cardRepositoryMock;
        private Mock<IMapper> _mapperMock;
        private CardService _cardService;

        /// <summary>
        /// Initializes the test setup before each test case by initializing mocks and the
        /// CardService instance.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            _cardRepositoryMock = new Mock<ICardRepository>();
            _mapperMock = new Mock<IMapper>();
            _cardService = new CardService(_cardRepositoryMock.Object, _mapperMock.Object);
        }

        /// <summary>
        /// Tests if GetCardsAsync correctly retrieves all cards from the repository and then maps
        /// them to the DTOs.
        /// </summary>
        [Test]
        public async Task GetCardsAsync_ShouldReturnMappedCardDTOs()
        {
            // Arrange: Set up mock behaviour for repository and mapper
            var cards = new List<CardModel> { new CardModel { Id = 1, Image = "Test Card" } };
            var cardDTOs = new List<CardModelDTO> { new CardModelDTO { Image = "Test Card" } };

            _cardRepositoryMock.Setup(repo => repo.GetAllCardsAsync()).ReturnsAsync(cards);
            _mapperMock.Setup(m => m.Map<IEnumerable<CardModelDTO>>(cards)).Returns(cardDTOs);

            // Act: Call the service method
            var result = await _cardService.GetCardsAsync();

            // Assert: Verify the result matches the expected DTOs
            result.Should().BeEquivalentTo(cardDTOs);
        }

        /// <summary>
        /// Tests if CreateCardAsync adds a new card and returns its DTP with an assigned ID.
        /// </summary>
        [Test]
        public async Task CreateCardAsync_ShouldReturnMappedCardDTO()
        {
            // Arrange: Define input DTO and expected entity behaviour
            var cardDTO = new CardModelDTO { Image = "New Card" };
            var card = new CardModel { Id = 1, Image = "New Card" };

            _mapperMock.Setup(m => m.Map<CardModel>(cardDTO)).Returns(card);
            _cardRepositoryMock.Setup(repo => repo.AddCardAsync(card)).ReturnsAsync(card);
            _mapperMock.Setup(m => m.Map<CardModelDTO>(card)).Returns(cardDTO);

            // Act: Call the service method
            var (resultDTO, resultId) = await _cardService.CreateCardAsync(cardDTO);

            // Assert: Check the returned DTO and ID
            resultDTO.Should().BeEquivalentTo(cardDTO);
            resultId.Should().Be(1); // Optional: Verify the ID as well
        }

        /// <summary>
        /// Tests if GetCardByIdAsync returns a mapped CardModelDTO when the card exists.
        /// </summary>
        [Test]
        public async Task GetCardByIdAsync_CardExists_ShouldReturnMappedCardDTO()
        {
            // Arrange: Mock a card retrieval and mapping
            var card = new CardModel { Id = 1, Image = "Test Card" };
            var cardDTO = new CardModelDTO { Image = "Test Card" };

            _cardRepositoryMock.Setup(repo => repo.GetCardByIdAsync(1)).ReturnsAsync(card);
            _mapperMock.Setup(m => m.Map<CardModelDTO>(card)).Returns(cardDTO);

            // Act: Fetch the mocked card
            var result = await _cardService.GetCardByIdAsync(1);

            // Assert: Ensure the DTO matches expectations
            result.Should().BeEquivalentTo(cardDTO);
        }

        /// <summary>
        /// Tests if GetCardByIdAsync returns null when the card does not exist.
        /// </summary>
        [Test]
        public async Task GetCardByIdAsync_CardDoesNotExist_ShouldReturnNull()
        {
            // Arrange: Mock a non-existent card
            _cardRepositoryMock.Setup(repo => repo.GetCardByIdAsync(1)).ReturnsAsync((CardModel)null);

            // Act: Fetch the card
            var result = await _cardService.GetCardByIdAsync(1);

            // Assert: Ensure null is returned
            result.Should().BeNull();
        }

        /// <summary>
        /// Tests if DeleteCardAsync returns true when a card is successfully deleted.
        /// </summary>
        [Test]
        public async Task DeleteCardAsync_CardExists_ShouldReturnTrue()
        {
            // Arrange: Mock card existence and deletion
            var card = new CardModel { Id = 1, Image = "Test Card" };
            _cardRepositoryMock.Setup(repo => repo.GetCardByIdAsync(1)).ReturnsAsync(card);
            _cardRepositoryMock.Setup(repo => repo.DeleteCardAsync(1)).Returns(Task.CompletedTask);

            // Act: Delete the card
            var result = await _cardService.DeleteCardAsync(1);

            // Assert: Check for successful deletion
            result.Should().BeTrue();
        }

        /// <summary>
        /// Tests if DeleteCardAsync returns false when a card does not exist.
        /// </summary>
        [Test]
        public async Task DeleteCardAsync_CardDoesNotExist_ShouldReturnFalse()
        {
            // Arrange: Mock a non-existent card
            _cardRepositoryMock.Setup(repo => repo.GetCardByIdAsync(1)).ReturnsAsync((CardModel)null);

            // Act: Attempt to delete the card
            var result = await _cardService.DeleteCardAsync(1);

            // Assert: Ensure failure is indicated
            result.Should().BeFalse();
        }

        /// <summary>
        /// Tests if UpdateCardAsync updates an existing card and then returns its updated
        /// CardModelDTO.
        /// </summary>
        [Test]
        public async Task UpdateCardAsync_CardExists_ShouldReturnUpdatedCardDTO()
        {
            // Arrange: Mock card update process
            var existingCard = new CardModel { Id = 1, Image = "Old Name" };
            var updatedCardDTO = new CardModelDTO { Image = "New Name" };

            _cardRepositoryMock.Setup(repo => repo.GetCardByIdAsync(1)).ReturnsAsync(existingCard);
            _mapperMock.Setup(m => m.Map(updatedCardDTO, existingCard));
            _cardRepositoryMock.Setup(repo => repo.UpdateCardAsync(existingCard)).ReturnsAsync(existingCard);
            _mapperMock.Setup(m => m.Map<CardModelDTO>(existingCard)).Returns(updatedCardDTO);

            // Act: Update the card
            var result = await _cardService.UpdateCardAsync(1, updatedCardDTO);

            // Assert: Verify the updated DTO
            result.Should().BeEquivalentTo(updatedCardDTO);
        }

        /// <summary>
        /// Tests if PatchCardAsync applies a patch (partial update) to an existing card returns its updated DTO.
        /// </summary>
        [Test]
        public async Task PatchCardAsync_CardExists_ShouldReturnPatchedCardDTO()
        {
            // Arrange: Mock patch operation
            var existingCard = new CardModel { Id = 1, Image = "Old Name" };
            var cardDTO = new CardModelDTO { Image = "Patched Name" };
            var patchDoc = new JsonPatchDocument<CardModelDTO>();
            patchDoc.Replace(c => c.Image, "Patched Name");

            _cardRepositoryMock.Setup(repo => repo.GetCardByIdAsync(1)).ReturnsAsync(existingCard);
            _mapperMock.Setup(m => m.Map<CardModelDTO>(existingCard)).Returns(cardDTO);
            _mapperMock.Setup(m => m.Map(cardDTO, existingCard));
            _cardRepositoryMock.Setup(repo => repo.PatchCardAsync(existingCard)).ReturnsAsync(existingCard);
            _mapperMock.Setup(m => m.Map<CardModelDTO>(existingCard)).Returns(cardDTO);

            // Act: Apply the patch
            var result = await _cardService.PatchCardAsync(1, patchDoc);

            // Assert: Verify the patched DTO
            result.Should().BeEquivalentTo(cardDTO);
        }
    }
}
