using HotelBooking.API.Extensions;
using HotelBooking.API.Filters;
using HotelBooking.Core.Domain.Entities;
using HotelBooking.Core.DTO;
using HotelBooking.Core.DTO.HotelDto;
using HotelBooking.Core.DTO.RoomCostDto;
using HotelBooking.Core.DTO.RoomDto;
using HotelBooking.Core.DTO.RoomTypeDTOs;
using HotelBooking.Core.DTO.UserDTOs;
using HotelBooking.Core.Enums;
using HotelBooking.Core.Helpers;
using HotelBooking.Core.ServiceContracts;
using HotelBooking.Core.ServiceContracts.IRoomService;
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
    public class RoomController : ControllerBase
    {
        private readonly IRoomGetterService _roomGetterService;
        private readonly IRoomAdderService _roomAdderService;
        private readonly IRoomUpdaterService _roomUpdaterService;
        private readonly IRoomDeleterService _roomDeleterService;
        private readonly ILogger<UserController> _logger;
        public RoomController(IRoomGetterService roomGetterService, ILogger<UserController> logger, IRoomAdderService roomAdderService, IRoomUpdaterService roomUpdaterService, IRoomDeleterService roomDeleterService)
        {
            _roomGetterService = roomGetterService;
            _logger = logger;
            _roomAdderService = roomAdderService;
            _roomUpdaterService = roomUpdaterService;
            _roomDeleterService = roomDeleterService;
        }



        [HttpPost("AddRoom")]
        [RoleAuthorization(SD.Role_Admin)]
        public async Task<IActionResult> CreateRoom([FromForm] CreateRoomRequestDTO request)
        {
            var userEmail = User.GetEmail();

            request.CreatedBy= userEmail;
            var serviceResponse = await _roomAdderService.CreateRoomAsync(request);

            if (!serviceResponse.IsSuccess)
                return StatusCode((int)serviceResponse.StatusCode,
            APIResultResponseDto<object>.Fail(serviceResponse.Message, serviceResponse.StatusCode));

            return Ok(APIResultResponseDto<object>.Success(null, "New room created successfully!!"));
           
        }

        [HttpPut("UpdateRoom/{roomId}")]
        [RoleAuthorization(SD.Role_Admin)]
        public async  Task<IActionResult> UpdateRoomType(int roomId, [FromForm] UpdateRoomRequestDTO request)
        {
            var userEmail = User.GetEmail();

           
            if (roomId != request.RoomID)
            {
                return StatusCode((int)HttpStatusCode.BadRequest,
           APIResultResponseDto<object>.Fail("UpdateRoom Mismatched Room ID!!", HttpStatusCode.BadRequest));
               
            }
            request.ModifiedBy = userEmail;
            var serviceResponse = await _roomUpdaterService.UpdateRoomAsync(request);

            if (!serviceResponse.IsSuccess)
                return StatusCode((int)serviceResponse.StatusCode,
              APIResultResponseDto<object>.Fail(serviceResponse.Message, serviceResponse.StatusCode));

            return Ok(APIResultResponseDto<object>.Success(null, "Room updated successfully!!"));
           
        }

        [HttpGet("GetAllRoom")]
        public async Task<IActionResult> GetAllRoom()
        {
            var serviceResponse = await _roomGetterService.RetrieveAllRoomsAsync();

            if (!serviceResponse.IsSuccess)
                return StatusCode((int)serviceResponse.StatusCode,
           APIResultResponseDto<object>.Fail(serviceResponse.Message, serviceResponse.StatusCode));

            return Ok(APIResultResponseDto<List<RoomDto>>.Success(serviceResponse.Data, "Retrieve all rooms successfully!!"));
           
        }

        [HttpGet("GetAllRoomsWithPagination")]
        public async Task<IActionResult> GetAllRoomsWithPagination([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var serviceResponse = await _roomGetterService.RetrieveAllRoomsWithPaginationAsync(pageNumber, pageSize);

            if (!serviceResponse.IsSuccess)
                return StatusCode((int)serviceResponse.StatusCode,
         APIResultResponseDto<object>.Fail(serviceResponse.Message, serviceResponse.StatusCode));

            return Ok(APIResultResponseDto<PagedDataResultDto<RoomDto>>.Success(serviceResponse.Data, "Retrieve all rooms successfully!!"));

        }
        [HttpGet("GetRoomById/{roomId}")]
        public async Task<IActionResult> GetRoomById(int roomId)
        {
            var serviceResponse = await _roomGetterService.RetrieveRoomByIdAsync(roomId);

            if (!serviceResponse.IsSuccess)
                return StatusCode((int)serviceResponse.StatusCode,
           APIResultResponseDto<object>.Fail(serviceResponse.Message, serviceResponse.StatusCode)); ;


            return Ok(APIResultResponseDto<RoomDto>.Success(serviceResponse.Data, "Retrieve room type successfully!!"));
            
        }

        [HttpDelete("DeleteRoom/{roomId}")]
        [RoleAuthorization(SD.Role_Admin)]
        public async Task<IActionResult> DeleteRoomType(int roomId)
        {
            var serviceResponse = await _roomDeleterService.DeleteRoomAsync(roomId);

            if (!serviceResponse.IsSuccess)
                return StatusCode((int)serviceResponse.StatusCode,
              APIResultResponseDto<object>.Fail(serviceResponse.Message, serviceResponse.StatusCode));

            return Ok(APIResultResponseDto<object>.Success(null, "Hotel deleted successfully."));
            
        }
        [HttpPost("CalculateRoomPrice")]
        public async Task<IActionResult> CalculateRoomCosts([FromBody] RoomCostRequestDto request)
        {
            var serviceResponse = await _roomGetterService.CalculateRoomCostsAsync(request.roomIds,request.checkInDate,request.checkOutDate);
            if (!serviceResponse.IsSuccess)
                return StatusCode((int)serviceResponse.StatusCode,
              APIResultResponseDto<object>.Fail(serviceResponse.Message, serviceResponse.StatusCode));

            return Ok(APIResultResponseDto<RoomCostResultDto>.Success(serviceResponse.Data, "Room price calculated successfully."));
        }



        }
}
