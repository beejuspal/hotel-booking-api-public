using Azure.Core;
using HotelBooking.Core.Domain.Entities;
using HotelBooking.Core.Domain.RepositoryContracts;
using HotelBooking.Core.DTO.HotelDto;
using HotelBooking.Core.DTO.RoomDto;
using HotelBooking.Infrastructure.DBContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HotelBooking.Core.DTO.HotelDto.FeaturedHotelDto;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace HotelBooking.Infrastructure.Repositories
{
    // Repositories/HotelRepository.cs
    public class HotelRepository : IHotelRepository
    {
       
        private readonly HotelDbContext _context;

        public HotelRepository(HotelDbContext context) => _context = context;

        public async Task<(List<Hotel>, int totalRecords)> GetAllWithPaginationAsync(int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber <= 0) pageNumber = 1;
            if (pageSize <= 0) pageSize = 10;
            var query = _context.Hotels.Include(h => h.HotelImgs).AsQueryable();
            int totalRecords = await query.CountAsync();
            var hotels = await query
                .OrderByDescending(h => h.HotelId) // Sorting for consistent results
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            int totalNoPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
            return (hotels,totalRecords);
        }

        public async Task<(List<Hotel>, int totalRecords)> GetAllAsync()
        {
           
            var query = _context.Hotels.AsQueryable();
            int totalRecords = await query.CountAsync();
            var hotels = await query
                .OrderByDescending(h => h.HotelId) // Sorting for consistent results
               
                .ToListAsync();
          
            return (hotels, totalRecords);
        }
        public async Task<Hotel?> GetByIdAsync(int id) =>
            await _context.Hotels.Include(h => h.HotelImgs).FirstOrDefaultAsync(h => h.HotelId == id);
        public async Task<bool> HotelExistsAsync(string hotelName, int hotelId)
        {
            if(hotelId>0)
            return await _context.Hotels.AnyAsync(rt => rt.Name == hotelName && rt.HotelId!=hotelId);
            else return await _context.Hotels.AnyAsync(rt => rt.Name == hotelName);
        }
        public async Task<Hotel> AddAsync(Hotel hotel)
        {
            _context.Hotels.Add(hotel);
            await _context.SaveChangesAsync();
            return hotel;
        }

        public async Task UpdateAsync(Hotel hotel)
        {
            _context.Hotels.Update(hotel);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Hotel hotel)
        {
            _context.Hotels.Remove(hotel);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteHotelImageAsync(HotelImage hotelImg)
        {
            _context.HotelImages.Remove(hotelImg);
            await _context.SaveChangesAsync();
        }
        public async Task<List<RoomSearchDto>> GetHotelRoomAmenitiesAsync(
        RoomSearchRequest request)
        {
            var result = await _context.Set<RoomSearchDto>()
                .FromSqlRaw("EXEC SearchHotelRoomAmenities @HotelID={0}, @RoomTypeName={1}, @AmenityName={2}, @MinPrice={3},@MaxPrice={4}",
                    request.HotelID, request.RoomTypeName, request.AmenityName,request.MinPrice,request.MaxPrice)
                .ToListAsync();
          
            return result;
        }

        public async Task<(List<FeaturedHotelDto>, int totalRecords)> GetFeaturedHotelsAsync(HotelSearchRequestDto searchRequest)
        {
            //var query = _context.Hotels
            //.Include(h => h.HotelImgs)
            //.Include(h => h.RoomTypes).ThenInclude(rt => rt.Rooms)
            //.Include(h => h.RoomTypes).ThenInclude(rt => rt.RoomAmenities).ThenInclude(ra => ra.Amenity)
            //.AsNoTracking();

            var query = _context.Hotels
    .Include(h => h.HotelImgs)
    .Include(h => h.RoomTypes).ThenInclude(rt => rt.Rooms)
    .Include(h => h.RoomTypes).ThenInclude(rt => rt.RoomAmenities).ThenInclude(ra => ra.Amenity)
    .AsNoTracking();
            // 🔍 Filtering
            if (!string.IsNullOrEmpty(searchRequest.SearchQuery))
            {
                query = query.Where(h =>
                    h.Name.Contains(searchRequest.SearchQuery) ||
                    h.Address.Contains(searchRequest.SearchQuery) ||
                    h.RoomTypes.Any(rt => rt.TypeName.Contains(searchRequest.SearchQuery)) ||
                    h.RoomTypes.Any(rt => rt.RoomAmenities.Any(ra => ra.Amenity.Name.Contains(searchRequest.SearchQuery)))
                );
            }
            // 💰 Price range filter (based on hotel rooms)
            if (searchRequest.MinPrice.HasValue)
            {
                query = query.Where(h => h.RoomTypes.Any(rt => rt.Rooms.Any(r => r.Price >= searchRequest.MinPrice.Value)));
            }

            if (searchRequest.MaxPrice.HasValue)
            {
                query = query.Where(h => h.RoomTypes.Any(rt => rt.Rooms.Any(r => r.Price <= searchRequest.MaxPrice.Value)));
            }
            if (searchRequest.HotelId>0)
            {
                query = query.Where(h => h.HotelId == searchRequest.HotelId);
            }
            var totalRecords = await query.CountAsync();

            var hotels = await query
                .Skip((searchRequest.Page - 1) * searchRequest.PageSize)
                .Take(searchRequest.PageSize)
                .Select(h => new FeaturedHotelDto
                {
                    HotelID = h.HotelId,
                    HotelName = h.Name,
                    HotelLocation = h.Address,
                    Description = h.Description,
                    StarRating = h.StarRating,
                    HotelImages = h.HotelImgs.Select(img => new FeaturedHotelImageDto
                    {
                        HotelImageID = img.HotelImageId,
                        ImageUrl = img.ImageUrl
                    }).ToList(),
                    RoomTypes = h.RoomTypes.Where(x=>x.HotelId==h.HotelId).Select(rt => new FeaturedRoomTypeDto
                    {
                        RoomTypeID = rt.RoomTypeID,
                        TypeName = rt.TypeName,
                        AccessibilityFeatures = rt.AccessibilityFeatures,
                        Description = rt.Description,
                        Rooms = rt.Rooms.Select(r => new FeaturedRoomDto
                        {
                            RoomID = r.RoomID,
                            RoomNumber = r.RoomNumber,
                            Price = r.Price,
                            BedType = r.BedType,
                            ViewType = r.ViewType,
                            Status = r.Status,
                            RoomImages = r.RoomImgs.Select(img => new FeaturedRoomImageDto
                            {
                                RoomImageID = img.RoomImageId,
                                ImageUrl = img.ImageUrl
                            }).ToList()
                        }).ToList(),
                        Amenities = rt.RoomAmenities.Select(ra => new FeaturedAmenityDto
                        {
                            AmenityID = ra.Amenity.AmenityID,
                            Name = ra.Amenity.Name
                        }).ToList()
                    }).ToList()
                })
                .ToListAsync();

            int totalNoPages = (int)Math.Ceiling(totalRecords / (double)searchRequest.PageSize);
            return (hotels, totalRecords);
        }
    }

    // Repositories/HotelImageRepository.cs
   

}
