using Azure;
using Azure.Core;
using HotelBooking.Core.Domain.Entities;
using HotelBooking.Core.DTO;
using HotelBooking.Core.DTO.RefreshTokenDto;
using HotelBooking.Core.DTO.UserDTOs;
using HotelBooking.Core.Helpers;
using HotelBooking.Core.ServiceContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using Superpower.Model;
using Superpower.Parsers;
using System.Net;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HotelBooking.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        private readonly ILogger<UserController> _logger;


        public AuthController(IAuthService authService, ILogger<UserController> logger)
        {
            _authService = authService;
            _logger = logger;

        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] CreateUserDTO createUserDTO)
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

            var serviceResponse = await _authService.RegisterAsync(createUserDTO);

            _logger.LogInformation("RegisterUser Response From Repository: {@CreateUserResponseDTO}", serviceResponse);

            if (!serviceResponse.IsSuccess)
            {

                return StatusCode((int)serviceResponse.StatusCode,
            APIResultResponseDto<object>.Fail(serviceResponse.Message, serviceResponse.StatusCode));
            }

            return Ok(APIResultResponseDto<object>.Success(null, "User created successfully!!"));


        }
        // Define the Login endpoint that responds to POST requests at 'api/Auth/Login'
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginUserDTO loginDto)
        {
            var responseDto = new APIResponseDto();

            if (!ModelState.IsValid)
            {

                return StatusCode((int)HttpStatusCode.BadRequest,
           APIResultResponseDto<object>.Fail("Invalid Data!!", HttpStatusCode.BadRequest));

            }


            var serviceResponse = await _authService.LoginAsync(loginDto);
            if (!serviceResponse.IsSuccess)
            {

                return StatusCode((int)serviceResponse.StatusCode,
           APIResultResponseDto<object>.Fail(serviceResponse.Message, serviceResponse.StatusCode));
            }

            var profile = new ProfileDto

            {
                Id = serviceResponse.Data.UserID.ToString(),
                Email = serviceResponse.Data.Email,
                Name = serviceResponse.Data.FullName,
                Verified = true,
                Role = serviceResponse.Data.RoleName,
                HotelName = serviceResponse.Data.HotelName,
                Dob = serviceResponse.Data.Dob.ToString(),
                Avatar = serviceResponse.Data.Avatar,
                ContactNo = serviceResponse.Data.ContactNo,
                Address = serviceResponse.Data.Address


            };

            var tokens = new TokensDto
            {
                Refresh = serviceResponse.Data.RefreshToken,
                Access = serviceResponse.Data.AccessToken
            };

            return Ok(APIResultResponseDto<SignInResDto>.Success(new SignInResDto { Profile = profile, Tokens = tokens }, "Login success!!"));

        }

        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody] UserRefreshTokenRequestDto requestDto)
        {
            var responseDto = new APIResponseDto();
            if (!ModelState.IsValid)
            {
                return StatusCode((int)HttpStatusCode.BadRequest,
          APIResultResponseDto<object>.Fail("Invalid Data!!", HttpStatusCode.BadRequest));
            }

            var serviceResponse = await _authService.RefreshTokenAsync(requestDto);
            if (!serviceResponse.IsSuccess)
            {
                return StatusCode((int)serviceResponse.StatusCode,
           APIResultResponseDto<object>.Fail(serviceResponse.Message, serviceResponse.StatusCode));
            }

            var userReponseDto = new UserResponseDTO
            {
                UserID = serviceResponse.Data.User.UserID,
                Email = serviceResponse.Data.User.Email,
                RoleID = serviceResponse.Data.User.RoleID,
                RoleName = serviceResponse.Data.User.Role.RoleName,
                FullName = serviceResponse.Data.User.FullName,
                HotelName = serviceResponse.Data.User.Hotel?.Name ?? "",
                Dob = serviceResponse.Data.User.DOB.ToString(),
                Avatar = serviceResponse.Data.User.AvatarUrl,
                ContactNo = serviceResponse.Data.User.PhoneNumber,
                Address = serviceResponse.Data.User.Address
            };

            var profile = new ProfileDto

            {
                Id = serviceResponse.Data.User.UserID.ToString(),
                Email = serviceResponse.Data.User.Email,
                Name = serviceResponse.Data.User.FullName,
                Verified = true,
                Role = serviceResponse.Data.User.Role.RoleName,
                HotelName = serviceResponse.Data.User.Hotel?.Name ?? "",
                Dob = serviceResponse.Data.User.DOB.ToString(),
                Avatar = serviceResponse.Data.User.AvatarUrl,
                ContactNo = serviceResponse.Data.User.PhoneNumber,
                Address = serviceResponse.Data.User.Address


            };

            var tokens = new TokensDto
            {
                Refresh = serviceResponse.Data.RefreshToken,
                Access = serviceResponse.Data.Token
            };
            return Ok(APIResultResponseDto<SignInResDto>.Success(new SignInResDto { Profile = profile, Tokens = tokens }, "Refresh token generated successfully!!"));

        }



        [HttpPost("Logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutRequestDTO requestDto)
        {
            var responseDto = new APIResponseDto();

            // Validate request body
            if (!ModelState.IsValid || string.IsNullOrEmpty(requestDto.RefreshToken))
            {
                return StatusCode((int)HttpStatusCode.BadRequest,
            APIResultResponseDto<object>.Fail("Invalid Data!!", HttpStatusCode.BadRequest));
            }
            var serviceResponse = await _authService.LogoutAsync(requestDto);
            if (!serviceResponse.IsSuccess)
            {
                return StatusCode((int)serviceResponse.StatusCode,
          APIResultResponseDto<object>.Fail(serviceResponse.Message, serviceResponse.StatusCode));

            }
            return Ok(APIResultResponseDto<object>.Success(null, "Log out success!!"));

        }

    }
}
