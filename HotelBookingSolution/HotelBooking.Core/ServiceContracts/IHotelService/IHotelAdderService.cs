using HotelBooking.Core.Domain.Entities;
using HotelBooking.Core.DTO;
using HotelBooking.Core.DTO.HotelDto;
using HotelBooking.Core.DTO.RoomTypeDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Core.ServiceContracts.IHotelService
{
    public interface IHotelAdderService
    {
        Task<ServiceResponse<HotelDto>> CreateHotelAsync(HotelCreateDto dto);
        //Task<ServiceResponse<Hotel?>> GetHotelByIdAsync(int id);
    }
}
