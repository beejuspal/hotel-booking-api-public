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
    public class RoomAmenityRepository : IRoomAmenitiesRepository
    {
        private readonly HotelDbContext _context;

        public RoomAmenityRepository(HotelDbContext context)
        {
            _context = context;
        }

        public async Task<List<RoomAmenity>> GetByRoomTypeAsync(int roomTypeId)
        {
            return await _context.RoomAmenities
                .Where(x => x.RoomTypeID == roomTypeId)
                .Include(x => x.Amenity)

                .ToListAsync();
            //      return await _context.RoomAmenities
            //.Include(r => r.Amenity)

            //.FirstOrDefaultAsync(r => r.RoomTypeID == roomTypeId);
        }

        public async Task BulkInsertAsync(IEnumerable<RoomAmenity> entities)
        {
            await _context.RoomAmenities.AddRangeAsync(entities);
            await _context.SaveChangesAsync();
        }

        public async Task BulkUpdateAsync(int roomTypeId, IEnumerable<RoomAmenity> newEntities)
        {
            var existing = _context.RoomAmenities.Where(x => x.RoomTypeID == roomTypeId);
            _context.RoomAmenities.RemoveRange(existing);
            await _context.RoomAmenities.AddRangeAsync(newEntities);
            await _context.SaveChangesAsync();
        }

        public async Task BulkDeltaAsync(int roomTypeId, IEnumerable<int> amenityIds)
        {
            var existing = await _context.RoomAmenities
                .Where(x => x.RoomTypeID == roomTypeId)
                .ToListAsync();

            var existingAmenityIDs = existing.Select(x => x.AmenityID).ToList();

            // Find items to add
            var toAdd = amenityIds
                .Except(existingAmenityIDs)
                .Select(aid => new RoomAmenity { RoomTypeID = roomTypeId, AmenityID = aid });

            // Find items to remove
            var toRemove = existing.Where(x => !amenityIds.Contains(x.AmenityID));

            if (toRemove.Any()) _context.RoomAmenities.RemoveRange(toRemove);
            if (toAdd.Any()) await _context.RoomAmenities.AddRangeAsync(toAdd);

            await _context.SaveChangesAsync();
        }
    }

}
