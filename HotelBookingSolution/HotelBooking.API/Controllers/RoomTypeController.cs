using HotelBooking.API.Extensions;
using HotelBooking.API.Filters;
using HotelBooking.Core.DTO;
using HotelBooking.Core.DTO.RoomDto;
using HotelBooking.Core.DTO.RoomTypeDTOs;
using HotelBooking.Core.DTO.UserDTOs;
using HotelBooking.Core.Enums;
using HotelBooking.Core.Helpers;
using HotelBooking.Core.ServiceContracts;
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
    public class RoomTypeController : ControllerBase
    {
        private readonly IRoomTypeGetterService _roomTypeGetterService;
        private readonly IRoomTypeAdderService _roomTypeAdderService;
        private readonly IRoomTypeUpdaterService _roomTypeUpdaterService;
        private readonly IRoomTypeDeleterService _roomTypeDeleterService;
        private readonly ILogger<UserController> _logger;
        public RoomTypeController(IRoomTypeGetterService roomTypeGetterService, ILogger<UserController> logger, IRoomTypeAdderService roomTypeAdderService, IRoomTypeUpdaterService roomTypeUpdaterService, IRoomTypeDeleterService roomTypeDeleterService)
        {
            _roomTypeGetterService = roomTypeGetterService;
            _logger = logger;
            _roomTypeAdderService = roomTypeAdderService;
            _roomTypeUpdaterService = roomTypeUpdaterService;
            _roomTypeDeleterService = roomTypeDeleterService;
        }



        [HttpPost("AddRoomType")]
        [RoleAuthorization(SD.Role_Admin)]
        public async Task<IActionResult> CreateRoomType([FromBody] CreateRoomTypeDTO request)
        {
            var userEmail = User.GetEmail();

            request.CreatedBy= userEmail;
            var serviceResponse = await _roomTypeAdderService.CreateRoomTypeAsync(request);

            if (!serviceResponse.IsSuccess)
                return StatusCode((int)serviceResponse.StatusCode,
           APIResultResponseDto<object>.Fail(serviceResponse.Message, serviceResponse.StatusCode));

            return Ok(APIResultResponseDto<object>.Success(null, "New room type created successfully!!"));
           
        }

        [HttpPut("UpdateRoomType/{roomTypeId}")]
        [RoleAuthorization(SD.Role_Admin)]
        public async  Task<IActionResult> UpdateRoomType(int roomTypeId, [FromBody] UpdateRoomTypeDTO request)
        {
            var userEmail = User.GetEmail();

          
            if (roomTypeId != request.RoomTypeID)
            {
                return StatusCode((int)HttpStatusCode.BadRequest,
          APIResultResponseDto<object>.Fail("UpdateRoomType Mismatched Room Type ID!!", HttpStatusCode.BadRequest));
            }
            request.ModifiedBy = userEmail;

            var serviceResponse = await _roomTypeUpdaterService.UpdateRoomTypeAsync(request);

            if (!serviceResponse.IsSuccess)
                return StatusCode((int)serviceResponse.StatusCode,
              APIResultResponseDto<object>.Fail(serviceResponse.Message, serviceResponse.StatusCode));

            return Ok(APIResultResponseDto<object>.Success(null, "Room type updated successfully!!"));

           
        }

        [HttpGet("GetAllRoomTypeWithPagination")]
        public async Task<IActionResult> GetAllRoomTypeWithPagination([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var serviceResponse = await _roomTypeGetterService.RetrieveAllRoomTypesWithPaginationAsync(pageNumber,pageSize);

            if (!serviceResponse.IsSuccess)
                return StatusCode((int)serviceResponse.StatusCode,
         APIResultResponseDto<object>.Fail(serviceResponse.Message, serviceResponse.StatusCode));

            return Ok(APIResultResponseDto<PagedDataResultDto<RoomTypeDTO>>.Success(serviceResponse.Data, "Retrieve all room types successfully!!"));
           
        }
        [HttpGet("GetRoomTypeById/{roomTypeId}")]
        public async Task<IActionResult> GetRoomTypeById(int roomTypeId)
        {
            var serviceResponse = await _roomTypeGetterService.RetrieveRoomTypeByIdAsync(roomTypeId);

           

            if (!serviceResponse.IsSuccess)
                return StatusCode((int)serviceResponse.StatusCode,
           APIResultResponseDto<object>.Fail(serviceResponse.Message, serviceResponse.StatusCode)); ;


            return Ok(APIResultResponseDto<RoomTypeDTO>.Success(serviceResponse.Data, "Retrieve room type successfully!!"));
        }

        [HttpDelete("DeleteRoomType/{roomTypeId}")]
        [RoleAuthorization(SD.Role_Admin)]
        public async Task<IActionResult> DeleteRoomType(int roomTypeId)
        {
            var serviceResponse = await _roomTypeDeleterService.DeleteRoomTypeAsync(roomTypeId);

            if (!serviceResponse.IsSuccess)
                return StatusCode((int)serviceResponse.StatusCode,
              APIResultResponseDto<object>.Fail(serviceResponse.Message, serviceResponse.StatusCode));

            return Ok(APIResultResponseDto<object>.Success(null, "Hotel type deleted successfully."));
        }

        [HttpGet("GetAllRoomTypeByHotelId/{hotelId}")]
        public async Task<IActionResult> GetAllRoomTypeWithPagination(int hotelId)
        {
            var serviceResponse = await _roomTypeGetterService.RetrieveAllRoomTypesByHotelIdAsync(hotelId);

            if (!serviceResponse.IsSuccess)
                return StatusCode((int)serviceResponse.StatusCode,
         APIResultResponseDto<object>.Fail(serviceResponse.Message, serviceResponse.StatusCode));

            return Ok(APIResultResponseDto<PagedDataResultDto<RoomTypeDTO>>.Success(serviceResponse.Data, "Retrieve all room types successfully!!"));

        }



    }
}
