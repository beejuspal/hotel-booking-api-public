using HotelBooking.Core.DTO;
using HotelBooking.Core.DTO.AmenityDto;
using HotelBooking.Core.DTO.HotelDto;
using HotelBooking.Core.DTO.RoomTypeDTOs;
using HotelBooking.Core.DTO.UserDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Core.ServiceContracts.IAmenityService
{
    public interface IAmenityGetterService
    {
        Task<ServiceResponse<PagedDataResultDto<AmenityDetailsDTO>>> RetrieveAllAmenitiesAsync();
        Task<ServiceResponse<PagedDataResultDto<AmenityDetailsDTO>>> RetrieveAllAmenityWithPaginationAsync(int pageNumber = 1, int pageSize = 10);
     
        Task<ServiceResponse<AmenityDetailsDTO>> RetrieveAmenityByIdAsync(int amenityId);
    }
}
