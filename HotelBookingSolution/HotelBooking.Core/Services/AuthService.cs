using HotelBooking.Core.Domain.Entities;
using HotelBooking.Core.Domain.RepositoryContracts;
using HotelBooking.Core.DTO;
using HotelBooking.Core.DTO.RefreshTokenDto;
using HotelBooking.Core.DTO.UserDTOs;
using HotelBooking.Core.Enums;
using HotelBooking.Core.Helpers;
using HotelBooking.Core.ServiceContracts;
using HotelBooking.Core.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Core.Services
{
    public class AuthService : IAuthService
    {
        private readonly JwtSettings _jwtSettings;
        private readonly IAuthRepository _authRepository;
        private readonly IUserValidatorService _userValidator;
        private readonly IPasswordHasherService _passwordHasher;
        private readonly ITokenRepository _tokenRepository;
        public AuthService(IAuthRepository authRepository, ITokenRepository tokenRepository, IUserValidatorService userValidator,
        IPasswordHasherService passwordHasher, JwtSettings jwtSettings)
        {
            _authRepository = authRepository;
            _tokenRepository = tokenRepository;
            _userValidator = userValidator;
            _passwordHasher = passwordHasher;
            _jwtSettings = jwtSettings;
        }
        public async Task<ServiceResponse<CreateUserResponseDTO>> RegisterAsync(CreateUserDTO? createUser)
        {
            if (createUser == null)
            {
                return ServiceResponse<CreateUserResponseDTO>.Fail(HttpStatusCode.BadRequest,
                           "Invalid request data"

                       );
            }

            var (isValid, message) = await _userValidator.ValidateNewUser(createUser);
            if (!isValid)
            {
                return ServiceResponse<CreateUserResponseDTO>.Fail(HttpStatusCode.BadRequest,
           message);

            }

            var user = new User
            {
                Email = createUser.Email,
                PasswordHash = _passwordHasher.HashPassword(createUser.Password),
                RoleID = (int)UserRoleEnum.Guest,
                CreatedBy = "user",
                CreatedDate = DateTime.UtcNow,
                IsActive = true,
                FullName = createUser.FullName
            };

            var userId = await _authRepository.RegisterUser(user);
            if (userId == 0)
            {
                return ServiceResponse<CreateUserResponseDTO>.Fail(HttpStatusCode.BadRequest,
          "Registration failed!!");
            }
            var userDto = new CreateUserResponseDTO
            {
                IsCreated = true,

            };

            return ServiceResponse<CreateUserResponseDTO>.Success(null, "Register successful");

        }
        public async Task<ServiceResponse<UserResponseDTO>> LoginAsync(LoginUserDTO loginDto)
        {
            if (loginDto == null)
            {
                return ServiceResponse<UserResponseDTO>.Fail(HttpStatusCode.BadRequest,
                           "Invalid login request",
                           new UserResponseDTO { UserID = -1 }
                       );
            }



            // Validate user credentials
            var user = await _authRepository.GetUserByEmail(loginDto.Email);
            if (user == null || !_passwordHasher.VerifyPassword(loginDto.Password, user.PasswordHash))
            {

                return ServiceResponse<UserResponseDTO>.Fail(HttpStatusCode.BadRequest,
            "Invalid email or password",
            new UserResponseDTO { UserID = -1 }
        );
            }


            var response = new UserResponseDTO
            {
                UserID = user.UserID,
                Email = user.Email,
                RoleID = user.RoleID,
                RoleName = user.Role.RoleName,
                FullName = user.FullName,
                HotelName = user.Hotel?.Name ?? "",
                Dob = user.DOB.ToString(),
                Avatar = user.AvatarUrl,
                ContactNo = user.PhoneNumber,
                Address = user.Address

            };
            // At this point, authentication is successful. Proceed to generate a JWT token.
            var token = TokenHelper.GenerateJwtToken(response, _jwtSettings.Secret, _jwtSettings.Audience, _jwtSettings.Issuer, _jwtSettings.ExpiryMinutes);

            // Generate Refresh Token
            var refreshToken = TokenHelper.GenerateRefreshToken();
            // Hash the refresh token before storing
            var hashedRefreshToken = TokenHelper.HashToken(refreshToken);
            // Create RefreshToken entity
            var refreshTokenEntity = new RefreshToken
            {
                Token = hashedRefreshToken,
                UserId = response.UserID,

                //Refresh tokens are set to expire after 7 days (you can adjust this as needed).
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow,
                IsRevoked = false
            };
            // Save hashed refresh token in DB
            await _tokenRepository.SaveUserToken(refreshTokenEntity);

            var userDto = new UserResponseDTO
            {
                UserID = user.UserID,
                Email = user.Email,
                RoleID = user.RoleID,
                RoleName = user.Role.RoleName,
                FullName = user.FullName,
                AccessToken = token,
                RefreshToken = refreshToken,
                HotelName = user.Hotel?.Name ?? "",
                Dob = user.DOB.ToString(),
                Avatar = user.AvatarUrl,
                ContactNo = user.PhoneNumber,
                Address = user.Address
            };

            return ServiceResponse<UserResponseDTO>.Success(userDto, "Login successful");
        }

        public async Task<ServiceResponse<TokenResponseDTO>> RefreshTokenAsync(UserRefreshTokenRequestDto requestDto)
        {
            if (requestDto == null || string.IsNullOrEmpty(requestDto.RefreshToken))
                return ServiceResponse<TokenResponseDTO>.Fail(HttpStatusCode.BadRequest,
             "Invalid request"
         );

            // Hash the incoming token
            var hashedToken = TokenHelper.HashToken(requestDto.RefreshToken);

            var storedToken = await _tokenRepository.GetRefreshToken(hashedToken);

            if (storedToken == null)
                return ServiceResponse<TokenResponseDTO>.Fail(HttpStatusCode.Unauthorized,
             "Invalid refresh token");


            if (storedToken.IsRevoked)

                return ServiceResponse<TokenResponseDTO>.Fail(HttpStatusCode.Unauthorized,
                "Refresh token has been revoked");


            if (storedToken.ExpiresAt < DateTime.UtcNow)
                return ServiceResponse<TokenResponseDTO>.Fail(HttpStatusCode.Unauthorized,
            "Refresh token has expired");


            var user = storedToken.User;
            var response = new UserResponseDTO
            {
                UserID = user.UserID,
                Email = user.Email,
                RoleID = user.RoleID,
                RoleName = user.Role.RoleName,
                FullName = user.FullName,
                HotelName = user.Hotel?.Name ?? "",
                Dob = user.DOB.ToString(),
                Avatar = user.AvatarUrl,
                ContactNo = user.PhoneNumber,
                Address = user.Address
            };
            // Revoke old token
            storedToken.IsRevoked = true;
            storedToken.RevokedAt = DateTime.UtcNow;

            // Generate new tokens
            var newAccessToken = TokenHelper.GenerateJwtToken(response, _jwtSettings.Secret, _jwtSettings.Audience, _jwtSettings.Issuer, _jwtSettings.ExpiryMinutes);

            var newRefreshTokenValue = TokenHelper.GenerateRefreshToken();
            var hashedNewRefreshToken = TokenHelper.HashToken(newRefreshTokenValue);

            await _tokenRepository.SaveUserToken(new RefreshToken
            {
                Token = hashedNewRefreshToken,
                UserId = user.UserID,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow
            });



            return ServiceResponse<TokenResponseDTO>.Success(new TokenResponseDTO
            {
                User = user,

                Token = newAccessToken,
                RefreshToken = newRefreshTokenValue,

            }, "Refresh token successful");

        }

        public async Task<ServiceResponse<object>> LogoutAsync(LogoutRequestDTO requestDto)
        {
            if (requestDto == null || string.IsNullOrEmpty(requestDto.RefreshToken))
                return ServiceResponse<object>.Fail(HttpStatusCode.BadRequest,
            "Invalid request"

        );

            var hashedToken = TokenHelper.HashToken(requestDto.RefreshToken);
            var storedToken = await _tokenRepository.GetRefreshToken(hashedToken);

            if (storedToken == null)

                return ServiceResponse<object>.Fail(HttpStatusCode.Unauthorized,
               "Invalid refresh token");

            if (storedToken.IsRevoked)

                return ServiceResponse<object>.Fail(HttpStatusCode.Unauthorized,
           "Refresh token already revoked");

            // Revoke this token and optionally all user tokens
            storedToken.IsRevoked = true;
            storedToken.RevokedAt = DateTime.UtcNow;
            await _tokenRepository.RevokeRefreshToken(storedToken.User.UserID);


            return ServiceResponse<object>.Success(null, "Login successful");
        }





    }
}
