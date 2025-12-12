using HotelBooking.Core.Domain.Entities;
using HotelBooking.Core.Domain.RepositoryContracts;
using HotelBooking.Core.DTO.RoomTypeDTOs;
using HotelBooking.Core.DTO.UserDTOs;
using HotelBooking.Infrastructure.DBContext;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Infrastructure.Repositories
{
    public class AmenityRepository : IAmenityRepository
    {
        private readonly HotelDbContext _context;

        public AmenityRepository(HotelDbContext context)
        {
            _context = context;
        }

   

        public async Task<int> AddAmenityeAsync(Amenity amenity)
        {
            _context.Amenities.Add(amenity);
            await _context.SaveChangesAsync();
            return amenity.AmenityID;
        }

        public async Task<int> UpdateAmenityAsync(Amenity amenity)
        {
            _context.Amenities.Update(amenity);
            await _context.SaveChangesAsync();
            return amenity.AmenityID;
        }

        public async Task DeleteAmenityAsync(Amenity amenity)
        {
            _context.Amenities.Remove(amenity);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> AmenityExistsAsync(string amenityName, int amenityId)
        {
            if (amenityId > 0)
                return await _context.Amenities.AnyAsync(rt => rt.Name == amenityName && rt.AmenityID == amenityId );
            else return await _context.Amenities.AnyAsync(rt => rt.Name == amenityName);
        }

        public async Task<(List<Amenity>, int totalRecords)> RetrieveAllAmenityWithPaginationAsync(int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber <= 0) pageNumber = 1;
            if (pageSize <= 0) pageSize = 10;
            var query = _context.Amenities.AsQueryable();
            int totalRecords = await query.CountAsync();
            var amenities = await query
                .OrderByDescending(h => h.AmenityID) // Sorting for consistent results
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            int totalNoPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
            return (amenities, totalRecords);
        }

        public async Task<Amenity?> RetrieveAmenityByIdAsync(int amenityId)
        {
            return await _context.Amenities.FirstOrDefaultAsync(r => r.AmenityID == amenityId);
        }
        public async Task<List<Amenity>> RetrieveAllAmenitiesAsync()
        {
           
            var amenities = await _context.Amenities
                .OrderByDescending(h => h.AmenityID) // Sorting for consistent results
               
                .ToListAsync();

            return amenities;
        }
    }
}
