using HotelBooking.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Core.Domain.RepositoryContracts
{
    public interface IRoomRepository
    {
        Task<int> AddRoomAsync(Room room);
        Task<int> UpdateRoomAsync(Room room);
        Task DeleteRoomAsync(Room room);
        Task<bool> RoomExistsAsync(string roomNo, int roomTypeId, int roomId);

        Task<List<Room>> RetrieveAllRoomsAsync();
        Task<Room?> RetrieveRoomByIdAsync(int roomId);
        Task<bool> RoomExistsByRoomTypeAsync(int roomTypeId);
        Task<(List<Room>, int totalRecords)> RetrieveAllRoomsWithPaginationAsync(int pageNumber = 1, int pageSize = 10);
        Task<List<Room>> RetrieveRoomsByIdsAsync(List<int> roomIds);
        Task UpdateRoomStatusesAsync(List<int> roomIds, string status);
        Task DeleteRoomImageAsync(RoomImage roomImg);
    }
}
