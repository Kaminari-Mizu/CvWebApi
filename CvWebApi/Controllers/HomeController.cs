using Microsoft.AspNetCore.Mvc;
using CvWebApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Services;
using Context;

namespace CvWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly ICardService _cardService;
        private readonly ICarouselService _carouselService;

        public HomeController(ICardService cardService, ICarouselService carouselService)
        {
            _cardService = cardService;
            _carouselService = carouselService;
        }

        // Get all Cards
        [HttpGet("cards")]
        public async Task<ActionResult<IEnumerable<CardModelDTO>>> GetCards()
        {
            try
            {
                var cards = await _cardService.GetCardsAsync();
                return Ok(cards);
            }
            catch (Exception ex) { return StatusCode(500, $"Error when fetching data {ex.Message}"); }
        }

        // Create a new Card
        [HttpPost("cards")]
        public async Task<ActionResult<CardModelDTO>> CreateCard(CardModelDTO cardDTO)
        {
            try
            {
                var createdCard = await _cardService.CreateCardAsync(cardDTO);
                return CreatedAtAction(nameof(GetCards), new { id = createdCard.Id }, createdCard);
            }
            catch (Exception ex) { return StatusCode(400, $"Input incorrect/Mismatched Parameters/Invalid data type {ex.Message}"); }
        }

        // Get all Carousels
        [HttpGet("carousel")]
        public async Task<ActionResult<IEnumerable<CarouselModelDTO>>> GetCarousels()
        {
            try
            {
                var carousels = await _carouselService.GetCarouselsAsync();
                return Ok(carousels);
            }
            catch (Exception ex) { return StatusCode(500, $"Error when fetching data {ex.Message}"); }
        }

        // Create a new Carousel
        [HttpPost("carousel")]
        public async Task<ActionResult<CarouselModelDTO>> CreateCarousel(CarouselModelDTO carouselDTO)
        {
            try
            {
                var createdCarousel = await _carouselService.CreateCarouselAsync(carouselDTO);
                return CreatedAtAction(nameof(GetCarousels), new { id = createdCarousel.Id }, createdCarousel);
            }
            catch (Exception ex) { return StatusCode(400, $"Input incorrect/Mismatched Parameters/Invalid data type {ex.Message}"); }
        }
    }
}
