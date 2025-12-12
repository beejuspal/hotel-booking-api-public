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
    public class RoomTypeRepository : IRoomTypeRepository
    {
        private readonly HotelDbContext _context;

        public RoomTypeRepository(HotelDbContext context)
        {
            _context = context;
        }

        public async Task<int> AddRoomTypeAsync(RoomType roomType)
        {
            _context.RoomTypes.Add(roomType);
            await _context.SaveChangesAsync();
            return roomType.RoomTypeID;
        }

        public async Task<bool> RoomTypeExistsAsync(string typeName, int hotelId, int roomTypeId)
        {
            if (roomTypeId > 0)
                return await _context.RoomTypes.AnyAsync(rt => rt.TypeName == typeName && rt.HotelId == hotelId && rt.RoomTypeID != roomTypeId);
            else return await _context.RoomTypes.AnyAsync(rt => rt.TypeName == typeName && rt.HotelId == hotelId);
        }


        //public async Task<List<RoomType>> RetrieveAllRoomTypesWithPaginationAsync(int pageNumber = 1, int pageSize = 10)
        //{
        //    return await _context.RoomTypes.AsNoTracking().Where(rt => rt.IsActive)
        //        .Select(rt => new RoomType
        //        {
        //            RoomTypeID = rt.RoomTypeID,
        //            TypeName = rt.TypeName,
        //            AccessibilityFeatures = rt.AccessibilityFeatures,
        //            Description = rt.Description,
        //            IsActive = rt.IsActive,
        //            HotelId = rt.HotelId,
        //            CreatedBy = rt.CreatedBy,CreatedDate = rt.CreatedDate,
        //        })
        //        .ToListAsync();
        //}
        public async Task<(List<RoomType>, int totalRecords)> RetrieveAllRoomTypesWithPaginationAsync(int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber <= 0) pageNumber = 1;
            if (pageSize <= 0) pageSize = 10;
            var query = _context.RoomTypes.Include(h => h.Hotel).AsQueryable();
            int totalRecords = await query.CountAsync();
            var roomTypes = await query
                .OrderByDescending(h => h.RoomTypeID) // Sorting for consistent results
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            int totalNoPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
            return (roomTypes, totalRecords);
        }
        public async Task<RoomType?> RetrieveRoomTypeByIdAsync(int roomTypeId)
        {
            return await _context.RoomTypes.FirstOrDefaultAsync(r => r.RoomTypeID == roomTypeId);
            //       return await _context.RoomTypes
            //.Where(u => u.RoomTypeID == roomTypeId)
            //.Select(rt => new RoomType
            //{
            //    RoomTypeID = rt.RoomTypeID,
            //    TypeName = rt.TypeName,
            //    AccessibilityFeatures = rt.AccessibilityFeatures,
            //    Description = rt.Description,
            //    IsActive = rt.IsActive,
            //    HotelId = rt.HotelId,
            //    CreatedBy=rt.CreatedBy,
            //    CreatedDate = rt.CreatedDate,
            //})
            //.FirstOrDefaultAsync();
        }

        public async Task<int> UpdateRoomTypeAsync(RoomType roomType)
        {
            _context.RoomTypes.Update(roomType);
            await _context.SaveChangesAsync();
            return roomType.RoomTypeID;
        }
        public async Task DeleteRoomTypeAsync(RoomType roomType)
        {
            _context.RoomTypes.Remove(roomType);
            await _context.SaveChangesAsync();
        }

        public async Task<List<RoomType>> RetrieveAllRoomTypAsyncByHotelId(int hotelId)
        {
            return await _context.RoomTypes
   .Where(rt => rt.IsActive && rt.HotelId == hotelId)
   .ToListAsync();
            //return await _context.RoomTypes.AsNoTracking().Where(rt => rt.IsActive && rt.HotelId == hotelId)
            //    .Select(rt => new RoomType
            //    {
            //        RoomTypeID = rt.RoomTypeID,
            //        TypeName = rt.TypeName,
            //        AccessibilityFeatures = rt.AccessibilityFeatures,
            //        Description = rt.Description,
            //        IsActive = rt.IsActive,
            //        HotelId = rt.HotelId,
            //        CreatedBy = rt.CreatedBy,
            //        CreatedDate = rt.CreatedDate,
            //    })
            //    .ToListAsync();
        }
    }
}
