using HotelBooking.Core.Domain.Entities;
using HotelBooking.Core.Domain.RepositoryContracts;
using HotelBooking.Core.DTO;
using HotelBooking.Core.DTO.RoomTypeDTOs;
using HotelBooking.Core.Helpers;
using HotelBooking.Core.ServiceContracts.IAmenityService;
using HotelBooking.Core.ServiceContracts.IRoomTypeService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Core.Services.AmenityService
{
    public class AmenityDeleterService : IAmenityDeleterService
    {
        private readonly IAmenityRepository _amenityRepository;
        public AmenityDeleterService(IAmenityRepository amenityRepository)
        {
            _amenityRepository = amenityRepository;
        }
        public async Task<ServiceResponse<object>> DeleteAmenityAsync(int amenityId)
        {
            var amenity = await _amenityRepository.RetrieveAmenityByIdAsync(amenityId);

            if (amenity == null)
                return ServiceResponse<object>.Fail(HttpStatusCode.NotFound,
                             "Amenity not found"

                         );
           

            await _amenityRepository.DeleteAmenityAsync(amenity);
            return ServiceResponse<object>.Success(null, "Amenity deleted successful");
        }
    }
}
