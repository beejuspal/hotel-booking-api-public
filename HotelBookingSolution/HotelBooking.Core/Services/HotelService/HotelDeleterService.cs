using HotelBooking.Core.Domain.RepositoryContracts;
using HotelBooking.Core.DTO;
using HotelBooking.Core.ServiceContracts.IHotelService;
using HotelBooking.Core.ServiceContracts.IRoomTypeService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Core.Services.HotelService
{
   
    public class HotelDeleterService : IHotelDeleterService
    {
        private readonly IHotelRepository _hotelRepository;

        public HotelDeleterService(IHotelRepository hotelRepository)
        {
            _hotelRepository = hotelRepository;

        }

      

        public async Task<ServiceResponse<object>> DeleteHotelAsync(int hotelId)
        {
            var hotel = await _hotelRepository.GetByIdAsync(hotelId);

            if (hotel == null)
                return ServiceResponse<object>.Fail(HttpStatusCode.NotFound,
                             "Hotel not found"

                         );

            await _hotelRepository.DeleteAsync(hotel);
            return ServiceResponse<object>.Success(null, "Hotel deleted successful");
        }
    }
}
