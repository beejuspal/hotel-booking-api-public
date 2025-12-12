using HotelBooking.Core.Domain.Entities;
using HotelBooking.Core.DTO.HotelDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Core.Domain.RepositoryContracts
{
    public interface IHotelRepository
    {
        Task<(List<Hotel>,int totalRecords)> GetAllWithPaginationAsync(int pageNumber = 1, int pageSize = 10);
        Task<(List<Hotel>, int totalRecords)> GetAllAsync();
        Task<bool> HotelExistsAsync(string hotelName,int hotelId);
        Task<Hotel?> GetByIdAsync(int id);
        Task<Hotel> AddAsync(Hotel hotel);
        Task UpdateAsync(Hotel hotel);
        Task DeleteAsync(Hotel hotel);
        Task DeleteHotelImageAsync(HotelImage hotelImg);
        Task<List<RoomSearchDto>> GetHotelRoomAmenitiesAsync(
       RoomSearchRequest request);
        Task<(List<FeaturedHotelDto>, int totalRecords)> GetFeaturedHotelsAsync(HotelSearchRequestDto searchRequest);
    }
}
