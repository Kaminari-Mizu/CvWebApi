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
            return await _context.Carousels.Include(c => c.Images).ToListAsync();
        }

        public async Task<CarouselModel> AddCarouselAsync(CarouselModel carousel)
        {
            _context.Carousels.Add(carousel);
            await _context.SaveChangesAsync();
            return carousel;
        }
    }
}
