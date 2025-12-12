using Azure.Core;
using HotelBooking.API.Extensions;
using HotelBooking.API.Filters;
using HotelBooking.Core.DTO;
using HotelBooking.Core.DTO.ReservationDto;
using HotelBooking.Core.DTO.RoomCostDto;
using HotelBooking.Core.DTO.RoomDto;
using HotelBooking.Core.DTO.UserDTOs;
using HotelBooking.Core.Enums;
using HotelBooking.Core.Helpers;
using HotelBooking.Core.ServiceContracts;
using HotelBooking.Core.ServiceContracts.IReservation;
using HotelBooking.Core.ServiceContracts.IRoomService;
using HotelBooking.Core.Services;
using HotelBooking.Core.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Superpower.Model;
using Superpower.Parsers;
using System.Net;
using static HotelBooking.Core.Services.ReservationService.ReservationService;

namespace HotelBooking.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationController : ControllerBase
    {
        private readonly IReservationService _reservationService;
        private readonly ILogger<ReservationController> _logger;
        private readonly IUserGetterService _userGetterService;
        public ReservationController(IReservationService reservationService, ILogger<ReservationController> logger, IUserGetterService userGetterService)
        {
            _reservationService = reservationService;
            _userGetterService = userGetterService;
            _logger = logger;
        }


       
        [RoleAuthorization()]
        [HttpPost("CreateReservation")]
        public async Task<IActionResult> CreateReservation([FromBody] ReservationCreateRequestDto request)
        {
            var userEmail = User.GetEmail();
           
            if (string.IsNullOrEmpty(userEmail))
                return ErrorResponseHelper.Create(HttpStatusCode.Unauthorized, "Invalid token: Email claim missing.");

            var user = await _userGetterService.GetUserProfileAsync(userEmail,0);
         
            if (user == null)
                return ErrorResponseHelper.Create(HttpStatusCode.NotFound, "User not found!!");
            request.UserID=user.UserID;
            request.CreatedBy = user.FullName; 
            var serviceResponse = await _reservationService.CreateReservationAsync(request);
            if (!serviceResponse.IsSuccess)
                return StatusCode((int)serviceResponse.StatusCode,
              APIResultResponseDto<object>.Fail(serviceResponse.Message, serviceResponse.StatusCode));

            return Ok(APIResultResponseDto<ReservationResponseDto>.Success(serviceResponse.Data, "Room reserverd successfully"));
        }
        [RoleAuthorization(SD.Role_Admin,SD.Role_HotelManager)]
        [HttpPost("GetAllReservations")]
        public async Task<IActionResult> GetAllReservations([FromBody] ReservationAdminFilterDto filter)
        {
            var userEmail = User.GetEmail();

            if (string.IsNullOrEmpty(userEmail))
                return ErrorResponseHelper.Create(HttpStatusCode.Unauthorized, "Invalid token: Email claim missing.");

            var user = await _userGetterService.GetUserProfileAsync(userEmail,0);

            if (user == null)
                return ErrorResponseHelper.Create(HttpStatusCode.NotFound, "User not found!!");

            //var result = await _reservationService.GetFilteredReservationsByUserAsync(user.UserID, filter);
            //return Ok(result);
            var serviceResponse = await _reservationService.GetAdminFilteredReservationsAsync(filter,user.RoleName,user.HotelID);
            if (!serviceResponse.IsSuccess)
                return StatusCode((int)serviceResponse.StatusCode,
              APIResultResponseDto<object>.Fail(serviceResponse.Message, serviceResponse.StatusCode));

            return Ok(APIResultResponseDto<PagedDataResultDto<ReservationDetailsDto>>.Success(serviceResponse.Data, "Retrieve all reservations successfully!!"));
        }
        [RoleAuthorization()]
        [HttpPost("GetUserReservationDetails")]
        public async Task<IActionResult> GetUserReservationDetails( [FromBody] ReservationFilterDto filter)
        {
            int userId = int.Parse(User.FindFirst("id")?.Value!);
            if (userId <= 0)
                return ErrorResponseHelper.Create(HttpStatusCode.NotFound, "User not found!!");
            //var userEmail = User.GetEmail();

            //if (string.IsNullOrEmpty(userEmail))
            //    return ErrorResponseHelper.Create(HttpStatusCode.Unauthorized, "Invalid token: Email claim missing.");

            //var user = await _userGetterService.GetUserProfileAsync(userEmail,0);

            //if (user == null)
            //    return ErrorResponseHelper.Create(HttpStatusCode.NotFound, "User not found!!");
           
           
            var serviceResponse = await _reservationService.GetFilteredReservationsByUserAsync(userId, filter);
            if (!serviceResponse.IsSuccess)
                return StatusCode((int)serviceResponse.StatusCode,
              APIResultResponseDto<object>.Fail(serviceResponse.Message, serviceResponse.StatusCode));

            return Ok(APIResultResponseDto<PagedDataResultDto<ReservationDetailsDto>>.Success(serviceResponse.Data, "Retrieve all reservations successfully!!"));
        }

        [RoleAuthorization()]
        [HttpPut("CancellReservation/{reservationID}")]
        public async Task<IActionResult> CancellReservation(int reservationID)
        {
            var userEmail = User.GetEmail();

            if (string.IsNullOrEmpty(userEmail))
                return ErrorResponseHelper.Create(HttpStatusCode.Unauthorized, "Invalid token: Email claim missing.");

            var user = await _userGetterService.GetUserProfileAsync(userEmail,0);

            if (user == null)
                return ErrorResponseHelper.Create(HttpStatusCode.NotFound, "User not found!!");

            //var result = await _reservationService.GetFilteredReservationsByUserAsync(user.UserID, filter);
            //return Ok(result);
            var serviceResponse = await _reservationService.CancelReservationAsync(reservationID,user.UserID, user.RoleName);
            if (!serviceResponse.IsSuccess)
                return StatusCode((int)serviceResponse.StatusCode,
              APIResultResponseDto<object>.Fail(serviceResponse.Message, serviceResponse.StatusCode));

            return Ok(APIResultResponseDto<object>.Success(null, "Reservation cancelled successfully"));
        }

    }
}
