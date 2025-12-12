using HotelBooking.Core.DTO;
using HotelBooking.Core.DTO.RoomCostDto;
using HotelBooking.Core.DTO.RoomDto;
using HotelBooking.Core.DTO.RoomTypeDTOs;
using HotelBooking.Core.DTO.UserDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Core.ServiceContracts.IRoomService
{
    public interface IRoomGetterService
    {
        Task<ServiceResponse<List<RoomDto>>> RetrieveAllRoomsAsync();
        Task<ServiceResponse<RoomDto>> RetrieveRoomByIdAsync(int roomId);
        Task<ServiceResponse<PagedDataResultDto<RoomDto>>> RetrieveAllRoomsWithPaginationAsync(int pageNumber = 1, int pageSize = 10);
        Task<ServiceResponse<List<RoomDto>>> RetrieveRoomsByIdsAsync(List<int> roomIds);
        Task<ServiceResponse<RoomCostResultDto>> CalculateRoomCostsAsync(List<int> roomIds, DateTime checkInDate, DateTime checkOutDate);
    }
}
