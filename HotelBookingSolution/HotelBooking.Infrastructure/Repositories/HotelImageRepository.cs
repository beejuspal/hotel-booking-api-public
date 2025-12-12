using HotelBooking.Core.Domain.Entities;
using HotelBooking.Core.Domain.RepositoryContracts;
using HotelBooking.Infrastructure.DBContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Infrastructure.Repositories
{
    public class HotelImageRepository : IHotelImageRepository
    {
        private readonly HotelDbContext _context;
        public HotelImageRepository(HotelDbContext context) => _context = context;

        public async Task<List<HotelImage>> GetByHotelIdAsync(int hotelId) =>
            await _context.HotelImages.Where(i => i.HotelId == hotelId).ToListAsync();

        public async Task<HotelImage> AddAsync(HotelImage image)
        {
            _context.HotelImages.Add(image);
            await _context.SaveChangesAsync();
            return image;
        }

        public async Task DeleteAsync(HotelImage image)
        {
            _context.HotelImages.Remove(image);
            await _context.SaveChangesAsync();
        }
    }
}
