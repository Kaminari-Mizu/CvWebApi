using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CvWebApi.Models;
using CvWebApi.CoreLogic;
using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CvWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly CvWebContext _context;
        private readonly IMapper _mapper;

        public HomeController(CvWebContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // Get all Cards (filtered from HomePages table)
        [HttpGet("cards")]
        public async Task<ActionResult<IEnumerable<CardModelDTO>>> GetCards()
        {
            var cards = await _context.Cards
                .Include(c => c.Badges)
                .ToListAsync();

            // Map to DTOs
            var cardDTOs = _mapper.Map<IEnumerable<CardModelDTO>>(cards);

            return Ok(cardDTOs);
        }

        // Get all Carousels (filtered from HomePages table)
        [HttpGet("carousel")]
        public async Task<ActionResult<IEnumerable<CarouselModelDTO>>> GetCarousels()
        {
            var carousels = await _context.Carousels
                .Include(c => c.Images)
                .ToListAsync();

            // Map to DTOs
            var carouselDTOs = _mapper.Map<IEnumerable<CarouselModelDTO>>(carousels);

            return Ok(carouselDTOs);
        }

        // Get all Home Models (both CardModel & CarouselModel)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<HomeModel>>> GetAllHomeModels()
        {
            var homeModels = await _context.Home.ToListAsync();

            // You would likely need to create separate DTOs for HomeModel if you wanted to return both CardModel and CarouselModel, or simply handle it on the client side.
            return Ok(homeModels); // You can adjust based on your needs.
        }

        // Create a new Card with Badges
        [HttpPost("cards")]
        public async Task<ActionResult<CardModelDTO>> CreateCard(CardModel card)
        {
            // Ensure all badges are linked to this card
            foreach (var badge in card.Badges)
            {
                badge.CardModelId = card.Id; // Link badge to card
            }
            _context.Cards.Add(card);
            await _context.SaveChangesAsync();

            // Map to DTO
            var cardDTO = _mapper.Map<CardModelDTO>(card);

            return CreatedAtAction(nameof(GetCards), new { id = card.Id }, cardDTO);
        }

        // Create a new Carousel with Images
        [HttpPost("carousel")]
        public async Task<ActionResult<CarouselModelDTO>> CreateCarousel(CarouselModel carousel)
        {
            // Ensure all images are linked to this carousel
            foreach (var image in carousel.Images)
            {
                image.CarouselModelId = carousel.Id; // Link image to carousel
            }
            _context.Carousels.Add(carousel);
            await _context.SaveChangesAsync();

            // Map to DTO
            var carouselDTO = _mapper.Map<CarouselModelDTO>(carousel);

            return CreatedAtAction(nameof(GetCarousels), new { id = carousel.Id }, carouselDTO);
        }
    }
}
