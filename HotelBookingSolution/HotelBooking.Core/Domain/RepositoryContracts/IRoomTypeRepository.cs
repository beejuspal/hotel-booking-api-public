using HotelBooking.Core.Domain.Entities;
using HotelBooking.Core.DTO.RoomTypeDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Core.Domain.RepositoryContracts
{
    public interface IRoomTypeRepository
    {
        Task<int> AddRoomTypeAsync(RoomType roomType);
        Task<int> UpdateRoomTypeAsync(RoomType roomType);
        Task DeleteRoomTypeAsync(RoomType roomType);
        Task<bool> RoomTypeExistsAsync(string typeName,int hotelId,int roomTypeId);

        Task<(List<RoomType>, int totalRecords)> RetrieveAllRoomTypesWithPaginationAsync(int pageNumber = 1, int pageSize = 10);
        Task<List<RoomType>> RetrieveAllRoomTypAsyncByHotelId(int hotelId);
        Task<RoomType?> RetrieveRoomTypeByIdAsync(int roomTypeId);


    }
}
