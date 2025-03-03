using Microsoft.AspNetCore.Mvc;
using CvWebApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Services;
using Context;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using System.Linq.Expressions;
using Microsoft.AspNetCore.JsonPatch.Exceptions;



namespace CvWebApi.Controllers
{
    /// <summary>
    /// This class is the controller that provides API endpoints for managing Card and Carousel entities
    /// as well as their related entities (Badges and CarouselImages respectively).
    /// It interacts with ICardService and ICarouselService to perform CRUD operations.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly ICardService _cardService;
        private readonly ICarouselService _carouselService;
        private readonly IMapper _mapper;  // Add this line to inject IMapper


        /// <summary>
        /// Constructor for HomeController class
        /// </summary>
        /// <param name="cardService">Service for managing Card entities</param>
        /// <param name="carouselService">Service managing Carousel entities</param>
        /// <param name="mapper">AutoMapper instance for mapping entities to DTOs</param>
        public HomeController(ICardService cardService, ICarouselService carouselService, IMapper mapper)
        {
            _cardService = cardService;
            _carouselService = carouselService;
            _mapper = mapper;
        }

        //CARD METHODS

        /// <summary>
        /// Get all Cards
        /// </summary>
        /// <returns></returns>
        [HttpGet("cards")]
        public async Task<ActionResult<IEnumerable<CardModelDTO>>> GetCards()
        {
            try
            {
                var cards = await _cardService.GetCardsAsync();
                return Ok(cards);
            }
            catch (Exception ex) { return StatusCode(500, $"Error when fetching data: {ex.Message}"); }
        }

        /// <summary>
        /// Get a Card by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("cards/{id}")]
        public async Task<ActionResult<CardModelDTO>> GetCardById(int id)
        {
            try
            {
                var card = await _cardService.GetCardByIdAsync(id);
                if (card == null) return NotFound($"Card with ID {id} not found.");
                return Ok(card);
            }
            catch (ArgumentException ex) { return BadRequest(new { message = "Invalid Id.", details = ex.Message }); }
            catch (Exception ex) { return StatusCode(500, $"Error when fetching data: {ex.Message}"); }
        }

        /// <summary>
        /// Create a new Card
        /// </summary>
        /// <param name="cardDTO"></param>
        /// <returns></returns>
        [HttpPost("cards")]
        public async Task<ActionResult<CardModelDTO>> CreateCard([FromBody] CardModelDTO cardDTO)
        {
            try
            {
                if (cardDTO == null) return BadRequest(new { message = "Card data cannot be null." });
                var createdCard = await _cardService.CreateCardAsync(cardDTO);
                return CreatedAtAction(nameof(GetCardById), new { id = createdCard.Id }, createdCard);
            }
            catch (ArgumentException ex) { return BadRequest(new { message = "Invalid card parameters received.", details = ex.Message }); }
            catch (Exception ex) { return StatusCode(400, $"Input incorrect/Mismatched Parameters/Invalid data type: {ex.Message}"); }
        }

        /// <summary>
        /// Update an existing Card
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cardDTO"></param>
        /// <returns></returns>
        [HttpPut("cards/Update")]
        public async Task<IActionResult> UpdateCard(int id,[FromBody] CardModelDTO cardDTO)
        {
            try
            {
                if (cardDTO == null) return BadRequest(new { message = "Card data cannot be null." });

                var updatedCard = await _cardService.UpdateCardAsync(id, cardDTO);
                if (updatedCard == null) return NotFound($"Card with ID {id} not found.");
                return Ok(updatedCard);
            }
            catch (ArgumentException ex) { return BadRequest(new { message = "Invalid card parameters received.", details = ex.Message }); }
            catch (Exception ex) { return StatusCode(500, $"Error when updating data: {ex.Message}"); }
        }

        /// <summary>
        /// Applies a partial update to a Card using JSON Patch
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cardDTO"></param>
        /// <returns></returns>
        [HttpPatch("cards/PatchCard")]
        public async Task<IActionResult> PatchCard(int id, [FromBody] JsonPatchDocument<CardModelDTO> cardDTO)
        {
            try
            {
                if (cardDTO == null) return BadRequest(new { message = "Patch data cannot be null." });

                var updatedCard = await _cardService.PatchCardAsync(id, cardDTO);
                if (updatedCard == null)
                    return NotFound($"Card with ID {id} not found.");

                return Ok(updatedCard);
            }
            catch (JsonPatchException ex) { return BadRequest(new { message = "Invalid patch operation.", details = ex.Message }); }
            catch (ArgumentException ex) { return BadRequest(new { message = "Invalid card parameters received.", details = ex.Message }); }
            catch (Exception ex) { return StatusCode(500, $"Error when updating data: {ex.Message}"); }
        }
       


/// <summary>
/// Delete a Card by ID
/// </summary>
/// <param name="id"></param>
/// <returns></returns>
[HttpDelete("cards/{id}")]
        public async Task<IActionResult> DeleteCard(int id)
        {
            try
            {
                var success = await _cardService.DeleteCardAsync(id);
                if (!success) return NotFound($"Card with ID {id} not found.");
                return NoContent();
            }
            catch (ArgumentException ex) { return BadRequest(new { message = "Invalid Id.", details = ex.Message }); }
            catch (Exception ex) { return StatusCode(500, $"Error when deleting data: {ex.Message}"); }
        }

        //CAROUSEL METHODS

        /// <summary>
        /// Get all Carousels
        /// </summary>
        /// <returns></returns>
       [HttpGet("carousel")]
        public async Task<ActionResult<IEnumerable<CarouselModelDTO>>> GetCarousels()
        {
            try
            {
                var carousels = await _carouselService.GetCarouselsAsync();
                return Ok(carousels);
            }
            catch (Exception ex) { return StatusCode(500, $"Error when fetching data: {ex.Message}"); }
        }

        /// <summary>
        /// Get a Carousel by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("carousel/{id}")]
        public async Task<ActionResult<CarouselModelDTO>> GetCarouselById(int id)
        {
            try
            {
                var carousel = await _carouselService.GetCarouselByIdAsync(id);
                if (carousel == null) return NotFound($"Carousel with ID {id} not found.");
                return Ok(carousel);
            }
            catch (Exception ex) { return StatusCode(500, $"Error when fetching data: {ex.Message}"); }
        }

        /// <summary>
        /// Create a new Carousel
        /// </summary>
        /// <param name="carouselDTO"></param>
        /// <returns></returns>
        [HttpPost("carousel")]
        public async Task<ActionResult<CarouselModelDTO>> CreateCarousel(CarouselModelDTO carouselDTO)
        {
            try
            {
                var createdCarousel = await _carouselService.CreateCarouselAsync(carouselDTO);
                return CreatedAtAction(nameof(GetCarouselById), new { id = createdCarousel.Id }, createdCarousel);
            }
            catch (Exception ex) { return StatusCode(400, $"Input incorrect/Mismatched Parameters/Invalid data type: {ex.Message}"); }
        }

        /// <summary>
        /// Update an existing Carousel
        /// </summary>
        /// <param name="id"></param>
        /// <param name="carouselDTO"></param>
        /// <returns></returns>
        [HttpPut("carousel/{id}")]
        public async Task<IActionResult> UpdateCarousel(int id, CarouselModelDTO carouselDTO)
        {
            try
            {
                var updatedCarousel = await _carouselService.UpdateCarouselAsync(id, carouselDTO);
                if (updatedCarousel == null) return NotFound($"Carousel with ID {id} not found.");
                return Ok(updatedCarousel);
            }
            catch (Exception ex) { return StatusCode(500, $"Error when updating data: {ex.Message}"); }
        }

        /// <summary>
        /// Delete a Carousel
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("carousel/{id}")]
        public async Task<IActionResult> DeleteCarousel(int id)
        {
            try
            {
                var success = await _carouselService.DeleteCarouselAsync(id);
                if (!success) return NotFound($"Carousel with ID {id} not found.");
                return NoContent();
            }
            catch (Exception ex) { return StatusCode(500, $"Error when deleting data: {ex.Message}"); }
        }
    }
}
