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
    public interface IHotelGetterService
    {
        Task<ServiceResponse<PagedDataResultDto<HotelDto>>> RetrieveAllHotelsWithPaginationAsync(int pageNumber = 1, int pageSize = 10);
        Task<ServiceResponse<PagedDataResultDto<HotelForRoomTypeDto>>> RetrieveAllHotelsAsync();
        Task<ServiceResponse<HotelDto>> RetrieveHotelByIdAsync(int hotelId);
        Task<ServiceResponse<PagedDataResultDto<RoomSearchDto>>> GetHotelRoomAmenitiesAsync(
        RoomSearchRequest request);

        Task<ServiceResponse<PagedDataResultDto<FeaturedHotelDto>>> RetrieveAllFeaturedHotelsWithPaginationAsync(HotelSearchRequestDto requestDto);
    }
}
