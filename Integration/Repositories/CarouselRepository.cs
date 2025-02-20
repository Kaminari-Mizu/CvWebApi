using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Context;

namespace Integration
{
    public class CarouselRepository : ICarouselRepository
    {
        private readonly CvWebContext _context;

        public CarouselRepository(CvWebContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CarouselModel>> GetAllCarouselsAsync()
        {
            return await _context.Carousels
                .Include(c => c.Images)
                .ToListAsync();
            // return await _context.Cards
            //    .Include(c => c.Badges) // Ensure badges are loaded
            //    .ToListAsync();
        }

        public async Task<CarouselModel?> GetCarouselByIdAsync(int id)
        {
            return await _context.Carousels
                .Include(c => c.Images)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<CarouselModel> AddCarouselAsync(CarouselModel carousel)
        {
            await _context.Carousels.AddAsync(carousel);
            await _context.SaveChangesAsync();
            return carousel;
        }

        public async Task UpdateCarouselAsync(CarouselModel carousel)
        {
            _context.Carousels.Update(carousel);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCarouselAsync(int id)
        {
            var carousel = await _context.Carousels.FindAsync(id);
            if (carousel != null)
            {
                _context.Carousels.Remove(carousel);
                await _context.SaveChangesAsync();
            }
        }
    }

}
