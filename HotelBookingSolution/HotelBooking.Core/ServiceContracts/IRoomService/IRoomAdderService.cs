using HotelBooking.Core.DTO;
using HotelBooking.Core.DTO.RoomDto;
using HotelBooking.Core.DTO.RoomTypeDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Core.ServiceContracts.IRoomService
{
    public interface IRoomAdderService
    {
        Task<ServiceResponse<CreateRoomResponseDTO>> CreateRoomAsync(CreateRoomRequestDTO request);
    }
}
