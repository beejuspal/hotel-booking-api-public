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
  
    public class RoomRepository : IRoomRepository
    {
        private readonly HotelDbContext _context;

        public RoomRepository(HotelDbContext context)
        {
            _context = context;
        }

        public async Task<int> AddRoomAsync(Room room)
        {
            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();
            return room.RoomID;
        }

        public async Task<bool> RoomExistsAsync(string roomNo, int roomTypeId, int roomId)
        {
            if (roomId > 0)
                return await _context.Rooms.AnyAsync(rt => rt.RoomNumber == roomNo && rt.RoomTypeID == roomTypeId && rt.RoomID != roomId);
            else return await _context.Rooms.AnyAsync(rt => rt.RoomNumber == roomNo && rt.RoomTypeID == roomTypeId);
        }


        public async Task<List<Room>> RetrieveAllRoomsAsync()
        {
            return await _context.Rooms
    .Where(rt => rt.IsActive)
    .ToListAsync();
            //return await _context.Rooms.AsNoTracking().Where(rt => rt.IsActive)
            //    .Select(rt => new Room
            //    {
            //        RoomID = rt.RoomID,
            //        RoomNumber = rt.RoomNumber,
            //        RoomTypeID = rt.RoomTypeID,
            //        Price = rt.Price,
            //        BedType = rt.BedType,
            //        ViewType = rt.ViewType,
            //        IsActive = rt.IsActive,
            //        Status = rt.Status,
                  
            //        CreatedBy = rt.CreatedBy,
            //        CreatedDate = rt.CreatedDate,
            //    })
            //    .ToListAsync();
        }

        public async Task<Room?> RetrieveRoomByIdAsync(int roomId)
        {
            return await _context.Rooms.Include(r => r.RoomImgs)
       .Include(r => r.RoomType)
           .ThenInclude(rt => rt.Hotel)
       .FirstOrDefaultAsync(r => r.RoomID == roomId);
     //       return await _context.Rooms.Include(h => h.RoomType).ThenInclude(h => h.Hotel)
     //.Where(u => u.RoomID == roomId)
     //.Select(rt => new Room
     //{
     //    RoomID = rt.RoomID,
     //    RoomNumber = rt.RoomNumber,
     //    RoomTypeID = rt.RoomTypeID,
     //    Price = rt.Price,
     //    BedType = rt.BedType,
     //    ViewType = rt.ViewType,
     //    IsActive = rt.IsActive,
     //    Status = rt.Status,
     //    RoomType=rt.RoomType,
     //    CreatedBy = rt.CreatedBy,
     //    CreatedDate = rt.CreatedDate,
         
     //})
     //.FirstOrDefaultAsync();
        }

        public async Task<int> UpdateRoomAsync(Room room)
        {
            _context.Rooms.Update(room);
            await _context.SaveChangesAsync();
            return room.RoomID;
        }
        public async Task DeleteRoomAsync(Room room)
        {
            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();
        }
        public async Task<bool> RoomExistsByRoomTypeAsync( int roomTypeId)
        {
            return await _context.Rooms.AnyAsync(rt =>rt.RoomTypeID == roomTypeId);
        }
        public async Task<(List<Room>, int totalRecords)> RetrieveAllRoomsWithPaginationAsync(int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber <= 0) pageNumber = 1;
            if (pageSize <= 0) pageSize = 10;
            var query = _context.Rooms.Include(h => h.RoomImgs).Include(h => h.RoomType).ThenInclude(h=>h.Hotel).AsQueryable();
            int totalRecords = await query.CountAsync();
            var roomTypes = await query
                .OrderByDescending(h => h.RoomID) // Sorting for consistent results
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            int totalNoPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
            return (roomTypes, totalRecords);
        }

        public async Task<List<Room>> RetrieveRoomsByIdsAsync(List<int> roomIds)
        {
            //return await _context.Rooms
            //.Where(r => roomIds.Contains(r.RoomID))
            //.ToListAsync();

            var rooms = await _context.Rooms
        .Where(r => roomIds.Contains(r.RoomID))
        .ToListAsync();

            //// Check if all requested rooms exist
            //if (rooms.Count != roomIds.Count)
            //{
            //    return null;
            //}

            return rooms;
        }

        public async Task UpdateRoomStatusesAsync(List<int> roomIds, string status)
        {
            var rooms = await _context.Rooms
                .Where(r => roomIds.Contains(r.RoomID))
                .ToListAsync();

            foreach (var room in rooms)
                room.Status = status;

            await _context.SaveChangesAsync();
        }
        public async Task DeleteRoomImageAsync(RoomImage roomImg)
        {
            _context.RoomImages.Remove(roomImg);
            await _context.SaveChangesAsync();
        }
    }
}
