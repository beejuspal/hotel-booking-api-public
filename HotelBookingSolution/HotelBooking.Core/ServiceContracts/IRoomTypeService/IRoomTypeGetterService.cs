using HotelBooking.Core.DTO;
using HotelBooking.Core.DTO.HotelDto;
using HotelBooking.Core.DTO.RoomTypeDTOs;
using HotelBooking.Core.DTO.UserDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Core.ServiceContracts.IRoomTypeService
{
    public interface IRoomTypeGetterService
    {
        //Task<ServiceResponse<List<RoomTypeDTO>>> RetrieveAllRoomTypesAsync();
        Task<ServiceResponse<PagedDataResultDto<RoomTypeDTO>>> RetrieveAllRoomTypesWithPaginationAsync(int pageNumber = 1, int pageSize = 10);
        Task<ServiceResponse<PagedDataResultDto<RoomTypeDTO>>> RetrieveAllRoomTypesByHotelIdAsync(int hotelId );
        Task<ServiceResponse<RoomTypeDTO>> RetrieveRoomTypeByIdAsync(int roomTypeId);
    }
}
