using HotelBooking.API.Extensions;
using HotelBooking.API.Filters;
using HotelBooking.Core.DTO;
using HotelBooking.Core.DTO.RoomDto;

using HotelBooking.Core.DTO.UserDTOs;
using HotelBooking.Core.Enums;
using HotelBooking.Core.Helpers;
using HotelBooking.Core.ServiceContracts;
using HotelBooking.Core.ServiceContracts.IAmenityService;

using HotelBooking.Core.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Superpower.Model;
using Superpower.Parsers;
using System.Net;
using HotelBooking.Core.DTO.AmenityDto;

namespace HotelBooking.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AmenityController : ControllerBase
    {
        private readonly IAmenityGetterService _amenityGetterService;
        private readonly IAmenityAdderService _amenityAdderService;
        private readonly IAmenityUpdaterService _amenityUpdaterService;
        private readonly IAmenityDeleterService _amenityDeleterService;
        private readonly ILogger<UserController> _logger;
        public AmenityController(IAmenityGetterService amenityGetterService, ILogger<UserController> logger, IAmenityAdderService amenityAdderService, IAmenityUpdaterService amenityUpdaterService, IAmenityDeleterService amenityDeleterService)
        {
            _amenityGetterService = amenityGetterService;
            _logger = logger;
            _amenityAdderService = amenityAdderService;
            _amenityUpdaterService = amenityUpdaterService;
            _amenityDeleterService = amenityDeleterService;
        }



        [HttpPost("AddAmenity")]
        [RoleAuthorization(SD.Role_Admin)]
        public async Task<IActionResult> CreateAmenity([FromBody] AmenityInsertDTO request)
        {
            var userEmail = User.GetEmail();

            request.CreatedBy = userEmail;
            var serviceResponse = await _amenityAdderService.CreateAmenityAsync(request);

            if (!serviceResponse.IsSuccess)
                return StatusCode((int)serviceResponse.StatusCode,
           APIResultResponseDto<object>.Fail(serviceResponse.Message, serviceResponse.StatusCode));

            return Ok(APIResultResponseDto<object>.Success(null, "New amenity created successfully!!"));
           
        }

        [HttpPut("UpdateAmenity/{amenityId}")]
        [RoleAuthorization(SD.Role_Admin)]
        public async  Task<IActionResult> UpdateAmenity(int amenityId, [FromBody] AmenityUpdateDTO request)
        {
            //var userEmail = User.GetEmail();

          
            if (amenityId != request.AmenityID)
            {
                return StatusCode((int)HttpStatusCode.BadRequest,
          APIResultResponseDto<object>.Fail("UpdateAmenity Mismatched Amenity ID!!", HttpStatusCode.BadRequest));
            }
            //request.ModifiedBy = userEmail;

            var serviceResponse = await _amenityUpdaterService.UpdateAmenityAsync(request);

            if (!serviceResponse.IsSuccess)
                return StatusCode((int)serviceResponse.StatusCode,
              APIResultResponseDto<object>.Fail(serviceResponse.Message, serviceResponse.StatusCode));

            return Ok(APIResultResponseDto<object>.Success(null, "Amenity updated successfully!!"));

           
        }

        [HttpGet("GetAllAmenityWithPagination")]
        public async Task<IActionResult> GetAllAmenityWithPagination([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var serviceResponse = await _amenityGetterService.RetrieveAllAmenityWithPaginationAsync(pageNumber,pageSize);

            if (!serviceResponse.IsSuccess)
                return StatusCode((int)serviceResponse.StatusCode,
         APIResultResponseDto<object>.Fail(serviceResponse.Message, serviceResponse.StatusCode));

            return Ok(APIResultResponseDto<PagedDataResultDto<AmenityDetailsDTO>>.Success(serviceResponse.Data, "Retrieve all amenities successfully!!"));
           
        }
        [HttpGet("GetAllAmenities")]
        public async Task<IActionResult> GetAllAmenities()
        {
            var serviceResponse = await _amenityGetterService.RetrieveAllAmenitiesAsync();

            if (!serviceResponse.IsSuccess)
                return StatusCode((int)serviceResponse.StatusCode,
         APIResultResponseDto<object>.Fail(serviceResponse.Message, serviceResponse.StatusCode));

            return Ok(APIResultResponseDto<PagedDataResultDto<AmenityDetailsDTO>>.Success(serviceResponse.Data, "Retrieve all amenities successfully!!"));

        }
        [HttpGet("GetAmenityById/{amenityId}")]
        public async Task<IActionResult> GetAmenityById(int amenityId)
        {
            var serviceResponse = await _amenityGetterService.RetrieveAmenityByIdAsync(amenityId);

           

            if (!serviceResponse.IsSuccess)
                return StatusCode((int)serviceResponse.StatusCode,
           APIResultResponseDto<object>.Fail(serviceResponse.Message, serviceResponse.StatusCode)); ;


            return Ok(APIResultResponseDto<AmenityDetailsDTO>.Success(serviceResponse.Data, "Retrieve amenitye successfully!!"));
        }

        [HttpDelete("DeleteAmenity/{amenityId}")]
        [RoleAuthorization(SD.Role_Admin)]
        public async Task<IActionResult> DeleteAmenity(int amenityId)
        {
            var serviceResponse = await _amenityDeleterService.DeleteAmenityAsync(amenityId);

            if (!serviceResponse.IsSuccess)
                return StatusCode((int)serviceResponse.StatusCode,
              APIResultResponseDto<object>.Fail(serviceResponse.Message, serviceResponse.StatusCode));

            return Ok(APIResultResponseDto<object>.Success(null, "Amenity deleted successfully."));
        }


    }
}
