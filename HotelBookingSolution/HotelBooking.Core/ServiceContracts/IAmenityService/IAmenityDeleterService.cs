using HotelBooking.Core.DTO;
using HotelBooking.Core.DTO.RoomTypeDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Core.ServiceContracts.IAmenityService
{
    
    public interface IAmenityDeleterService
    {
        Task<ServiceResponse<object>> DeleteAmenityAsync(int amenityId);
    }
}
