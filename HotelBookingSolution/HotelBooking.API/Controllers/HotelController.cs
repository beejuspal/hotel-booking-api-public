using Azure.Core;
using HotelBooking.API.Extensions;
using HotelBooking.API.Filters;
using HotelBooking.Core.DTO;
using HotelBooking.Core.DTO.HotelDto;
using HotelBooking.Core.DTO.RoomTypeDTOs;
using HotelBooking.Core.DTO.UserDTOs;
using HotelBooking.Core.Enums;
using HotelBooking.Core.Helpers;
using HotelBooking.Core.ServiceContracts;
using HotelBooking.Core.ServiceContracts.IHotelService;
using HotelBooking.Core.ServiceContracts.IRoomTypeService;

using HotelBooking.Core.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Superpower.Model;
using Superpower.Parsers;
using System.Net;

namespace HotelBooking.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotelController : ControllerBase
    {
        private readonly IHotelAdderService _hotelAdderService;
        private readonly IHotelUpdaterService _hotelUpdaterService;

        private readonly IHotelDeleterService _hotelDeleterService;
        private readonly IHotelGetterService _hotelGetterService;
        private readonly ILogger<UserController> _logger;
        public HotelController(IHotelAdderService hotelAdderService, ILogger<UserController> logger, IRoomTypeUpdaterService roomTypeUpdaterService, IHotelDeleterService hotelDeleterService, IHotelUpdaterService hotelUpdaterService, IHotelGetterService hotelGetterService)
        {
            _hotelAdderService = hotelAdderService;
            _logger = logger;
            _hotelDeleterService = hotelDeleterService;
            _hotelUpdaterService = hotelUpdaterService;
            _hotelGetterService = hotelGetterService;
        }



        [HttpPost("AddHotel")]
        [RoleAuthorization(SD.Role_Admin)]
        public async Task<IActionResult> CreatNewHotel([FromForm] HotelCreateDto dto)
        {
            var serviceResponse = await _hotelAdderService.CreateHotelAsync(dto);

            if (!serviceResponse.IsSuccess)
                return StatusCode((int)serviceResponse.StatusCode,
             APIResultResponseDto<object>.Fail(serviceResponse.Message, serviceResponse.StatusCode));


            return Ok(APIResultResponseDto<object>.Success(null, "Hotel created successfully!!"));
           
        }

        [HttpPut("UpdateHotel/{hotelId}")]
        [RoleAuthorization(SD.Role_Admin)]
        public async Task<IActionResult> UpdateHotel(int hotelId, [FromForm] HotelCreateDto dto)
        {
            var serviceResponse = await _hotelUpdaterService.UpdateHotelAsync(hotelId, dto);

            if (!serviceResponse.IsSuccess)
                return StatusCode((int)serviceResponse.StatusCode,
             APIResultResponseDto<object>.Fail(serviceResponse.Message, serviceResponse.StatusCode));


            return Ok(APIResultResponseDto<object>.Success(null, "Hotel updated successfully!!"));
           
        }


        [HttpGet("GetAllHotelWitPagination")]
        public async Task<IActionResult> GetAllHotelWitPagination([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var serviceResponse = await _hotelGetterService.RetrieveAllHotelsWithPaginationAsync(pageNumber,pageSize);

            if (!serviceResponse.IsSuccess)
                return StatusCode((int)serviceResponse.StatusCode,
             APIResultResponseDto<object>.Fail(serviceResponse.Message, serviceResponse.StatusCode));

            return Ok(APIResultResponseDto<PagedDataResultDto<HotelDto>>.Success(serviceResponse.Data, "Retrieve all hotel successfully!!"));
           
        }
        [HttpGet("GetAllHotels")]
        public async Task<IActionResult> GetAllHotels()
        {
            var serviceResponse = await _hotelGetterService.RetrieveAllHotelsAsync();

            if (!serviceResponse.IsSuccess)
                return StatusCode((int)serviceResponse.StatusCode,
            APIResultResponseDto<object>.Fail(serviceResponse.Message, serviceResponse.StatusCode));

            return Ok(APIResultResponseDto<PagedDataResultDto<HotelForRoomTypeDto>>.Success(serviceResponse.Data, "Retrieve all hotel successfully!!"));
            
        }
        [HttpGet("GetHotelById/{hotelId}")]
        public async Task<IActionResult> GetHotelById(int hotelId)
        {
            var serviceResponse = await _hotelGetterService.RetrieveHotelByIdAsync(hotelId);

            if (!serviceResponse.IsSuccess)
                return StatusCode((int)serviceResponse.StatusCode,
            APIResultResponseDto<object>.Fail(serviceResponse.Message, serviceResponse.StatusCode));

            return Ok(APIResultResponseDto<HotelDto>.Success(serviceResponse.Data, "Retrieve hotel successfully!!"));
            
        }

        [HttpDelete("DeleteHotel/{hotelId}")]
        [RoleAuthorization(SD.Role_Admin)]
        public async Task<IActionResult> DeleteHotel(int hotelId)
        {
            var serviceResponse = await _hotelDeleterService.DeleteHotelAsync(hotelId);

            if (!serviceResponse.IsSuccess)
                return StatusCode((int)serviceResponse.StatusCode,
             APIResultResponseDto<object>.Fail(serviceResponse.Message, serviceResponse.StatusCode));


            return Ok(APIResultResponseDto<object>.Success(null, "Hotel deleted successfully."));
           
        }

        [HttpPost("SearchHotelRooms")]
        public async Task<IActionResult> SearchHotelRooms([FromBody] RoomSearchRequest request)
        {
            var serviceResponse = await _hotelGetterService.GetHotelRoomAmenitiesAsync(request);

           

            if (!serviceResponse.IsSuccess)
                return StatusCode((int)serviceResponse.StatusCode,
             APIResultResponseDto<object>.Fail(serviceResponse.Message, serviceResponse.StatusCode));

            return Ok(APIResultResponseDto<PagedDataResultDto<RoomSearchDto>>.Success(serviceResponse.Data, "Retrieve all hotel successfully!!"));
        }

        [HttpGet("GetAllFeaturedHoteslWithPagination")]
        public async Task<IActionResult> GetAllFeaturedHoteslWithPagination([FromQuery] HotelSearchRequestDto reqeustDto)
        {
            var serviceResponse = await _hotelGetterService.RetrieveAllFeaturedHotelsWithPaginationAsync(reqeustDto);

            if (!serviceResponse.IsSuccess)
                return StatusCode((int)serviceResponse.StatusCode,
             APIResultResponseDto<object>.Fail(serviceResponse.Message, serviceResponse.StatusCode));

            return Ok(APIResultResponseDto<PagedDataResultDto<FeaturedHotelDto>>.Success(serviceResponse.Data, "Retrieve all hotel successfully!!"));

        }


    }
}
