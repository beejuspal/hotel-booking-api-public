using HotelBooking.Core.DTO;
using HotelBooking.Core.DTO.HotelDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Core.ServiceContracts.IHotelService
{
    public interface IHotelUpdaterService
    {
        Task<ServiceResponse<HotelDto>> UpdateHotelAsync(int id, HotelCreateDto dto);
    }
}
