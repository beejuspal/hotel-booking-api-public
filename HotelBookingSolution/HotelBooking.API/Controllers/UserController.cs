using HotelBooking.API.Extensions;
using HotelBooking.API.Filters;
using HotelBooking.Core.Domain.Entities;
using HotelBooking.Core.DTO;
using HotelBooking.Core.DTO.RoomDto;
using HotelBooking.Core.DTO.UserDTOs;
using HotelBooking.Core.Enums;
using HotelBooking.Core.Helpers;
using HotelBooking.Core.ServiceContracts;
using HotelBooking.Core.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Superpower.Model;
using Superpower.Parsers;
using System.Net;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HotelBooking.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserGetterService _userGetterService;
        private readonly IUserAdderService _userAdderService;
        private readonly ILogger<UserController> _logger;
        public UserController(IUserGetterService userGetterService, ILogger<UserController> logger, IUserAdderService userAdderService)
        {
            _userGetterService = userGetterService;
            _logger = logger;
            _userAdderService = userAdderService;
        }


        [HttpGet("GetUserProfile")]
        //[Authorize(Roles = SD.Role_Customer)]
        [RoleAuthorization()]
        public async Task<IActionResult> GetProfile()
        {
           
            var userEmail = User.GetEmail();
           
            if (string.IsNullOrEmpty(userEmail))
                return ErrorResponseHelper.Create(HttpStatusCode.Unauthorized, "Invalid token: Email claim missing.");

            var user = await _userGetterService.GetUserProfileAsync(userEmail,0);
         
            if (user == null)
                return ErrorResponseHelper.Create(HttpStatusCode.NotFound, "User not found!!");

            var profile = new ProfileDto
            {
                Id = user.UserID.ToString(),
                Email = user.Email,
                Name = user.FullName,
                Role = user.RoleName,
                HotelName=user.HotelName,
                Dob = user.Dob.ToString(),
                Avatar = user.Avatar,
                ContactNo = user.ContactNo,
                Address = user.Address
            };

            return Ok(new APIResponseDto
            {
                StatusCode = HttpStatusCode.OK,
                IsSuccess = true,
                Result = new SignInResDto { Profile = profile }
            });
        }
        [HttpPut("UpdateProfile/{userId}")]
        [RoleAuthorization()]
        public async Task<IActionResult> UpdateRoomType(int userId, [FromForm] UpdateProfileDto request)
        {
            int claimUserId = int.Parse(User.FindFirst("id")?.Value!);
            if (claimUserId <= 0)
                return ErrorResponseHelper.Create(HttpStatusCode.NotFound, "User not found!!");
            if (userId != claimUserId)
            {
                return StatusCode((int)HttpStatusCode.BadRequest,
           APIResultResponseDto<object>.Fail("UpdateProfile Mismatched User ID!!", HttpStatusCode.BadRequest));

            }
            var userEmail = User.GetEmail();
            request.ModifiedBy = userEmail;
            var serviceResponse = await _userAdderService.UpdateProfileAsync(claimUserId,request);

            if (!serviceResponse.IsSuccess)
                return StatusCode((int)serviceResponse.StatusCode,
              APIResultResponseDto<object>.Fail(serviceResponse.Message, serviceResponse.StatusCode));
         
            return Ok(APIResultResponseDto<object>.Success(new ProfileResDto { Profile = serviceResponse.Data }, "Profile updated successfully!!"));
        }

        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto dto)
        {
            var serviceResponse = await _userAdderService.ForgotPasswordAsync(dto);

            if (!serviceResponse.IsSuccess)
                return StatusCode((int)serviceResponse.StatusCode,
            APIResultResponseDto<object>.Fail(serviceResponse.Message, serviceResponse.StatusCode));

            return Ok(APIResultResponseDto<bool>.Success(true, "Password reset link sent to email!!"));
        }

        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto dto)
        {
            if (!ModelState.IsValid)
            {
                // Step 1: Extract all field-specific errors
                var fieldErrors = ModelState
                    .Where(ms => ms.Value.Errors.Count > 0)
                    .Select(ms => new
                    {
                        Field = ms.Key,
                        Messages = ms.Value.Errors.Select(e => e.ErrorMessage).ToList()
                    })
                    .ToList();

                // Step 2: Combine into a single readable string message
                var allMessages = fieldErrors
                    .SelectMany(e => e.Messages.Select(m => $"{e.Field}: {m}"))
                    .ToList();

                var combinedMessage = string.Join("; ", allMessages);
                // Log and return custom API response
                _logger.LogWarning("Validation errors: {Errors}", combinedMessage);
                return StatusCode((int)HttpStatusCode.BadRequest,
                    APIResultResponseDto<object>.Fail(combinedMessage, HttpStatusCode.BadRequest));


                //     return StatusCode((int)HttpStatusCode.BadRequest,
                //APIResultResponseDto<object>.Fail("Invalid Data!!", HttpStatusCode.BadRequest));
            }
            var serviceResponse = await _userAdderService.ResetPasswordAsync(dto);

            if (!serviceResponse.IsSuccess)
                return StatusCode((int)serviceResponse.StatusCode,
            APIResultResponseDto<object>.Fail(serviceResponse.Message, serviceResponse.StatusCode));

            return Ok(APIResultResponseDto<bool>.Success(true, "Password reset successful!!"));
        }
        [RoleAuthorization()]
        [HttpPost("ChangePassword/{userId}")]
        public async Task<IActionResult> ChangePassword(int userId,ChangePasswordDto dto)
        {
            if (!ModelState.IsValid)
            {
                // Step 1: Extract all field-specific errors
                var fieldErrors = ModelState
                    .Where(ms => ms.Value.Errors.Count > 0)
                    .Select(ms => new
                    {
                        Field = ms.Key,
                        Messages = ms.Value.Errors.Select(e => e.ErrorMessage).ToList()
                    })
                    .ToList();

                // Step 2: Combine into a single readable string message
                var allMessages = fieldErrors
                    .SelectMany(e => e.Messages.Select(m => $"{e.Field}: {m}"))
                    .ToList();

                var combinedMessage = string.Join("; ", allMessages);
                // Log and return custom API response
                _logger.LogWarning("Validation errors: {Errors}", combinedMessage);
                return StatusCode((int)HttpStatusCode.BadRequest,
                    APIResultResponseDto<object>.Fail(combinedMessage, HttpStatusCode.BadRequest));


                //     return StatusCode((int)HttpStatusCode.BadRequest,
                //APIResultResponseDto<object>.Fail("Invalid Data!!", HttpStatusCode.BadRequest));
            }
            int claimUserId = int.Parse(User.FindFirst("id")?.Value!);
            if (claimUserId <= 0)
                return ErrorResponseHelper.Create(HttpStatusCode.NotFound, "User not found!!");
            if (userId != claimUserId)
            {
                return StatusCode((int)HttpStatusCode.BadRequest,
           APIResultResponseDto<object>.Fail("Change Password Mismatched User ID!!", HttpStatusCode.BadRequest));

            }
            var serviceResponse = await _userAdderService.ChangePasswordAsync(userId,dto);

            if (!serviceResponse.IsSuccess)
                return StatusCode((int)serviceResponse.StatusCode,
            APIResultResponseDto<object>.Fail(serviceResponse.Message, serviceResponse.StatusCode));

            return Ok(APIResultResponseDto<bool>.Success(true, "Password changed successful!!"));
        }
    }
}
