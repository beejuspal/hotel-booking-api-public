using HotelBooking.Core.DTO;
using HotelBooking.Core.DTO.AmenityDto;
using HotelBooking.Core.DTO.RoomTypeDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Core.ServiceContracts.IAmenityService
{
    
    public interface IAmenityUpdaterService
    {
        Task<ServiceResponse<AmenityUpdateResponseDTO>> UpdateAmenityAsync(AmenityUpdateDTO request);
    }
}
