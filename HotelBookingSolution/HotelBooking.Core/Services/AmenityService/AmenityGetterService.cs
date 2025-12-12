using HotelBooking.Core.Domain.Entities;
using HotelBooking.Core.Domain.RepositoryContracts;
using HotelBooking.Core.DTO;
using HotelBooking.Core.DTO.AmenityDto;
using HotelBooking.Core.DTO.HotelDto;
using HotelBooking.Core.DTO.RoomTypeDTOs;
using HotelBooking.Core.DTO.UserDTOs;
using HotelBooking.Core.ServiceContracts;
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

    public class AmenityGetterService : IAmenityGetterService
    {
        private readonly IAmenityRepository _amenityRepository;
        public AmenityGetterService(IAmenityRepository amenityRepository)
        {
            _amenityRepository = amenityRepository;
        }

        public async Task<ServiceResponse<PagedDataResultDto<AmenityDetailsDTO>>> RetrieveAllAmenitiesAsync()
        {
            var amenityList = await _amenityRepository.RetrieveAllAmenitiesAsync();


            if (amenityList == null || amenityList.Count == 0)
                return ServiceResponse<PagedDataResultDto<AmenityDetailsDTO>>.Fail(HttpStatusCode.NotFound,
                                "No any amenity detail found"

                            );


            // Map entity list to DTO list
            var result = amenityList.Select(rt => new AmenityDetailsDTO
            {
                Name = rt.Name,
                AmenityID = rt.AmenityID,
                IsActive = rt.IsActive,
                Description = rt.Description,

            }).ToList();
            var pagedDtoResult = new PagedDataResultDto<AmenityDetailsDTO>
            {
                Items = result,
              
            };
            return ServiceResponse<PagedDataResultDto<AmenityDetailsDTO>>.Success(pagedDtoResult, "Amenity retrieve successful");
        }

        public async Task<ServiceResponse<PagedDataResultDto<AmenityDetailsDTO>>> RetrieveAllAmenityWithPaginationAsync(int pageNumber = 1, int pageSize = 10)
        {
            var amenityList = await _amenityRepository.RetrieveAllAmenityWithPaginationAsync(pageNumber,pageSize);


            if (amenityList.Item1 == null || amenityList.Item1.Count == 0)
                return ServiceResponse<PagedDataResultDto<AmenityDetailsDTO>>.Fail(HttpStatusCode.NotFound,
                                "No any amenity detail found"

                            );


            // Map entity list to DTO list
            var result = amenityList.Item1.Select(rt => new AmenityDetailsDTO
            {
                Name = rt.Name,
                AmenityID = rt.AmenityID,
                IsActive = rt.IsActive,
                Description = rt.Description,
               
            }).ToList();
            var pagedDtoResult = new PagedDataResultDto<AmenityDetailsDTO>
            {
                Items = result,
                TotalRecords = amenityList.totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(amenityList.totalRecords / (double)pageSize)
            };
            return ServiceResponse<PagedDataResultDto<AmenityDetailsDTO>>.Success(pagedDtoResult, "Amenity retrieve successful");
        }

        public async Task<ServiceResponse<AmenityDetailsDTO>> RetrieveAmenityByIdAsync(int amenityId)
        {
            var amenity = await _amenityRepository.RetrieveAmenityByIdAsync(amenityId);

            if (amenity == null)
                return ServiceResponse<AmenityDetailsDTO>.Fail(HttpStatusCode.NotFound,
                                 "Amenity not found"

                             );

            var res= new AmenityDetailsDTO
            {
                AmenityID = amenity.AmenityID,
                Name = amenity.Name,
                IsActive = amenity.IsActive,
                Description = amenity.Description,
               
            };
            return ServiceResponse<AmenityDetailsDTO>.Success(res, "Amenity found");
        }
    }
}
