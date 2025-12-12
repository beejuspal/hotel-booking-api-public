using HotelBooking.API.Extensions;
using HotelBooking.API.Filters;
using HotelBooking.Core.DTO;
using HotelBooking.Core.DTO.DashboardDto;
using HotelBooking.Core.DTO.ReservationDto;
using HotelBooking.Core.DTO.UserDTOs;
using HotelBooking.Core.Enums;
using HotelBooking.Core.Helpers;
using HotelBooking.Core.ServiceContracts;
using HotelBooking.Core.ServiceContracts.IDashboard;
using HotelBooking.Core.Services;
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
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;
        private readonly ILogger<DashboardController> _logger;
        private readonly IUserGetterService _userGetterService;
        public DashboardController(IDashboardService dashboardService, ILogger<DashboardController> logger, IUserGetterService userGetterService)
        {
            _dashboardService = dashboardService;
            _logger = logger;
            _userGetterService = userGetterService;
        }


        [RoleAuthorization()]
        [HttpGet("GetDashboardData")]
        public async Task<IActionResult> GetDashboardData()
        {
            int userId = int.Parse(User.FindFirst("id")?.Value!);
            if (userId<=0)
                return ErrorResponseHelper.Create(HttpStatusCode.NotFound, "User not found!!");
            //var userEmail = User.GetEmail();

            //if (string.IsNullOrEmpty(userEmail))
            //    return ErrorResponseHelper.Create(HttpStatusCode.Unauthorized, "Invalid token: Email claim missing.");

            //var user = await _userGetterService.GetUserProfileAsync(userEmail,0);

            //if (user == null)
            //    return ErrorResponseHelper.Create(HttpStatusCode.NotFound, "User not found!!");

          
            var serviceResponse = await _dashboardService.GetUserDashboardAsync(userId);
            if (!serviceResponse.IsSuccess)
                return StatusCode((int)serviceResponse.StatusCode,
              APIResultResponseDto<object>.Fail(serviceResponse.Message, serviceResponse.StatusCode));

            return Ok(APIResultResponseDto<UserDashboardDto>.Success(serviceResponse.Data, "Retrieve all dashboard data successfully!!"));
        }



    }
}
