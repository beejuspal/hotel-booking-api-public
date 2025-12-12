using HotelBooking.Core.DTO;
using HotelBooking.Core.DTO.RoomTypeDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Core.ServiceContracts.IRoomTypeService
{
    
    public interface IRoomTypeDeleterService
    {
        Task<ServiceResponse<object>> DeleteRoomTypeAsync(int roomTypeId);
    }
}
