using HotelBooking.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Core.ServiceContracts.IHotelService
{
    public interface IHotelDeleterService
    {
        Task<ServiceResponse<object>> DeleteHotelAsync(int hotelId);
    }
}
