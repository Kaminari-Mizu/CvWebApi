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
    /// Unit tests for the CardService class.
    /// </summary>
    [TestFixture]
    public class CardServiceTests
    {
        private Mock<ICardRepository> _cardRepositoryMock;
        private Mock<IMapper> _mapperMock;
        private CardService _cardService;

        /// <summary>
        /// Initializes the test setup before each test case.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            _cardRepositoryMock = new Mock<ICardRepository>();
            _mapperMock = new Mock<IMapper>();
            _cardService = new CardService(_cardRepositoryMock.Object, _mapperMock.Object);
        }

        /// <summary>
        /// Tests if GetCardsAsync correctly returns mapped CardModelDTOs.
        /// </summary>
        [Test]
        public async Task GetCardsAsync_ShouldReturnMappedCardDTOs()
        {
            // Arrange
            var cards = new List<CardModel> { new CardModel { Id = 1, Image = "Test Card" } };
            var cardDTOs = new List<CardModelDTO> { new CardModelDTO { Id = 1, Image = "Test Card" } };

            _cardRepositoryMock.Setup(repo => repo.GetAllCardsAsync()).ReturnsAsync(cards);
            _mapperMock.Setup(m => m.Map<IEnumerable<CardModelDTO>>(cards)).Returns(cardDTOs);

            // Act
            var result = await _cardService.GetCardsAsync();

            // Assert
            result.Should().BeEquivalentTo(cardDTOs);
        }

        /// <summary>
        /// Tests if CreateCardAsync correctly returns the mapped CardModelDTO.
        /// </summary>
        [Test]
        public async Task CreateCardAsync_ShouldReturnMappedCardDTO()
        {
            // Arrange
            var cardDTO = new CardModelDTO { Id = 1, Image = "New Card" };
            var card = new CardModel { Id = 1, Image = "New Card" };

            _mapperMock.Setup(m => m.Map<CardModel>(cardDTO)).Returns(card);
            _cardRepositoryMock.Setup(repo => repo.AddCardAsync(card)).ReturnsAsync(card);
            _mapperMock.Setup(m => m.Map<CardModelDTO>(card)).Returns(cardDTO);

            // Act
            var result = await _cardService.CreateCardAsync(cardDTO);

            // Assert
            result.Should().BeEquivalentTo(cardDTO);
        }

        /// <summary>
        /// Tests if GetCardByIdAsync returns a mapped CardModelDTO when the card exists.
        /// </summary>
        [Test]
        public async Task GetCardByIdAsync_CardExists_ShouldReturnMappedCardDTO()
        {
            // Arrange
            var card = new CardModel { Id = 1, Image = "Test Card" };
            var cardDTO = new CardModelDTO { Id = 1, Image = "Test Card" };

            _cardRepositoryMock.Setup(repo => repo.GetCardByIdAsync(1)).ReturnsAsync(card);
            _mapperMock.Setup(m => m.Map<CardModelDTO>(card)).Returns(cardDTO);

            // Act
            var result = await _cardService.GetCardByIdAsync(1);

            // Assert
            result.Should().BeEquivalentTo(cardDTO);
        }

        /// <summary>
        /// Tests if GetCardByIdAsync returns null when the card does not exist.
        /// </summary>
        [Test]
        public async Task GetCardByIdAsync_CardDoesNotExist_ShouldReturnNull()
        {
            // Arrange
            _cardRepositoryMock.Setup(repo => repo.GetCardByIdAsync(1)).ReturnsAsync((CardModel)null);

            // Act
            var result = await _cardService.GetCardByIdAsync(1);

            // Assert
            result.Should().BeNull();
        }

        /// <summary>
        /// Tests if DeleteCardAsync returns true when a card is successfully deleted.
        /// </summary>
        [Test]
        public async Task DeleteCardAsync_CardExists_ShouldReturnTrue()
        {
            // Arrange
            var card = new CardModel { Id = 1, Image = "Test Card" };
            _cardRepositoryMock.Setup(repo => repo.GetCardByIdAsync(1)).ReturnsAsync(card);
            _cardRepositoryMock.Setup(repo => repo.DeleteCardAsync(1)).Returns(Task.CompletedTask);

            // Act
            var result = await _cardService.DeleteCardAsync(1);

            // Assert
            result.Should().BeTrue();
        }

        /// <summary>
        /// Tests if DeleteCardAsync returns false when a card does not exist.
        /// </summary>
        [Test]
        public async Task DeleteCardAsync_CardDoesNotExist_ShouldReturnFalse()
        {
            // Arrange
            _cardRepositoryMock.Setup(repo => repo.GetCardByIdAsync(1)).ReturnsAsync((CardModel)null);

            // Act
            var result = await _cardService.DeleteCardAsync(1);

            // Assert
            result.Should().BeFalse();
        }

        /// <summary>
        /// Tests if UpdateCardAsync returns the updated CardModelDTO when a card exists.
        /// </summary>
        [Test]
        public async Task UpdateCardAsync_CardExists_ShouldReturnUpdatedCardDTO()
        {
            // Arrange
            var existingCard = new CardModel { Id = 1, Image = "Old Name" };
            var updatedCardDTO = new CardModelDTO { Id = 1, Image = "New Name" };

            _cardRepositoryMock.Setup(repo => repo.GetCardByIdAsync(1)).ReturnsAsync(existingCard);
            _mapperMock.Setup(m => m.Map(updatedCardDTO, existingCard));
            _cardRepositoryMock.Setup(repo => repo.UpdateCardAsync(existingCard)).ReturnsAsync(existingCard);
            _mapperMock.Setup(m => m.Map<CardModelDTO>(existingCard)).Returns(updatedCardDTO);

            // Act
            var result = await _cardService.UpdateCardAsync(1, updatedCardDTO);

            // Assert
            result.Should().BeEquivalentTo(updatedCardDTO);
        }

        /// <summary>
        /// Tests if PatchCardAsync returns the patched CardModelDTO when a card exists.
        /// </summary>
        [Test]
        public async Task PatchCardAsync_CardExists_ShouldReturnPatchedCardDTO()
        {
            // Arrange
            var existingCard = new CardModel { Id = 1, Image = "Old Name" };
            var cardDTO = new CardModelDTO { Id = 1, Image = "Patched Name" };
            var patchDoc = new JsonPatchDocument<CardModelDTO>();
            patchDoc.Replace(c => c.Image, "Patched Name");

            _cardRepositoryMock.Setup(repo => repo.GetCardByIdAsync(1)).ReturnsAsync(existingCard);
            _mapperMock.Setup(m => m.Map<CardModelDTO>(existingCard)).Returns(cardDTO);
            _mapperMock.Setup(m => m.Map(cardDTO, existingCard));
            _cardRepositoryMock.Setup(repo => repo.PatchCardAsync(existingCard)).ReturnsAsync(existingCard);
            _mapperMock.Setup(m => m.Map<CardModelDTO>(existingCard)).Returns(cardDTO);

            // Act
            var result = await _cardService.PatchCardAsync(1, patchDoc);

            // Assert
            result.Should().BeEquivalentTo(cardDTO);
        }
    }
}
