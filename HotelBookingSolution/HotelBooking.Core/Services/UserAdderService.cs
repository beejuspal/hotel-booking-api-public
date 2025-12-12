using HotelBooking.Core.Domain.Entities;
using HotelBooking.Core.Domain.RepositoryContracts;
using HotelBooking.Core.DTO;
using HotelBooking.Core.DTO.HotelDto;
using HotelBooking.Core.DTO.RoomDto;
using HotelBooking.Core.DTO.UserDTOs;
using HotelBooking.Core.Helpers;
using HotelBooking.Core.ServiceContracts;
using HotelBooking.Core.ServiceContracts.IHotelService;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Core.Services
{
    public class UserAdderService : IUserAdderService
    {
        private readonly IUserRepository _userRepository;
        private readonly IImageStorageService _imageStorage;
        private readonly IEmailService _emailService;
        private readonly IPasswordHasherService _passwordHasher;
        private readonly IConfiguration _config;
        public UserAdderService(IUserRepository userRepository, IImageStorageService imageStorage, IEmailService emailService, IPasswordHasherService passwordHasher,IConfiguration config)
        {
            _userRepository = userRepository;
            _imageStorage = imageStorage;
            _emailService = emailService;
            _passwordHasher = passwordHasher;
            _config = config;
        }
        public async Task<CreateUserResponseDTO> AddUser(CreateUserDTO? createuser)
        {
            CreateUserResponseDTO createUserResponseDTO = new CreateUserResponseDTO();

            //Validation: countryAddRequest parameter can't be null
            if (createuser == null)
            {
                throw new ArgumentNullException(nameof(createuser));
            }
            //Model validation
            ValidationHelper.ModelValidation(createuser);

            //Validation: Email can't be duplicate
            if (await _userRepository.GetUserByEmailAsync(createuser.Email) != null)
            {
                throw new ArgumentException("Given email already exists");
            }
            var user = new User
            {
                Email = createuser.Email,
                PasswordHash = createuser.Password,
                RoleID = 1,
                CreatedBy = "user",
                CreatedDate = DateTime.UtcNow,
                IsActive = true
            };
            var userData = await _userRepository.AddUserAsync(user);
            var UserId = userData.userId;
            if (UserId != -1)
            {
                createUserResponseDTO.UserId = UserId;
                createUserResponseDTO.IsCreated = true;

                return createUserResponseDTO;
            }

            return createUserResponseDTO;
        }

        public async Task<ServiceResponse<ProfileDto>> UpdateProfileAsync(int userId, UpdateProfileDto dto)
        {
            var user = await _userRepository.GetUserByEmailAsync("", userId);
            if (user == null) return ServiceResponse<ProfileDto>.Fail(HttpStatusCode.NotFound,
                            "User not found"

                        );

            user.FullName = dto.FullName;
            user.PhoneNumber = dto.PhoneNumber;
            user.Address = dto.Address;
            user.DOB = dto.DOB;
            
            await HandleImagesAsync(user, dto);
            user.ModifiedDate = DateTime.UtcNow;

            int updatedUserId=await _userRepository.UpdateUserAsync(user);
            if(updatedUserId<=0) return ServiceResponse<ProfileDto>.Fail(HttpStatusCode.BadRequest,
                            "Profile update failed"

                        );
            var userReponseDto = new ProfileDto
            {
                Id = user.UserID.ToString(),
                Email = user.Email,
           
                Role = user.Role.RoleName,
                Name = user.FullName,
                HotelName = user.Hotel?.Name ?? "",
                Dob = user.DOB.ToString(),
                Avatar = user.AvatarUrl,
                ContactNo = user.PhoneNumber,
                Address = user.Address
            };
            return ServiceResponse<ProfileDto>.Success(userReponseDto, "Profile updated successfully");
        }

        public async Task<ServiceResponse<bool>> ForgotPasswordAsync(ForgotPasswordDto dto)
        {
            var url = _config.GetSection("KhaltiSettings");
            var webSiteUrl = url["WebSiteUrl"];
            var user = await _userRepository.GetUserByEmailAsync(dto.Email,0);
            if (user == null) return ServiceResponse<bool>.Fail(HttpStatusCode.NotFound,
                             "User not found"

                         );

            var token = Guid.NewGuid().ToString();
            user.PasswordResetToken = token;
            user.PasswordResetExpires = DateTime.UtcNow.AddHours(1);
            await _userRepository.UpdateUserAsync(user);

            var resetLink = $"{webSiteUrl}reset-password?token={token}";
            var emailBody = $@"
            <h2>Reset Password</h2>
            <p>Click below to reset your password. This link expires in 1 hour.</p>
            <a href='{resetLink}' style='background:#007bff;color:white;padding:10px 15px;border-radius:5px;text-decoration:none;'>Reset Password</a>";
            await _emailService.SendEmailAsync(user.Email, "Reset Your Password", emailBody);
            return ServiceResponse<bool>.Success(true, "Please check your email to reset your password");
        }

        public async Task<ServiceResponse<bool>> ResetPasswordAsync(ResetPasswordDto dto)
        {
            var user = await _userRepository.CheckPasswordReset(dto.Token);
                

            if (user == null || dto.NewPassword != dto.ConfirmNewPassword)
                return ServiceResponse<bool>.Fail(HttpStatusCode.NotFound,
                              "Invalid or expired token"

                          );

            user.PasswordHash = _passwordHasher.HashPassword(dto.NewPassword);
            user.PasswordResetToken = null;
            user.PasswordResetExpires = null;

            await _userRepository.UpdateUserAsync(user);
            return ServiceResponse<bool>.Success(true, "Password reset success");
        }

        public async Task<ServiceResponse<bool>> ChangePasswordAsync(int userId, ChangePasswordDto dto)
        {
            var user = await _userRepository.GetUserByEmailAsync("",userId);
            if (user == null) return ServiceResponse<bool>.Fail(HttpStatusCode.NotFound,
                              "User not found"

                          );

            //var result = _passwordHasher.VerifyPassword(  dto.CurrentPassword, user.PasswordHash);
            //if (!result) return ServiceResponse<bool>.Fail(HttpStatusCode.NotFound,
            //                  "Something went wrong");

            if (dto.NewPassword != dto.ConfirmNewPassword)
                return ServiceResponse<bool>.Fail(HttpStatusCode.BadRequest,
                               "Password mismatch");

            user.PasswordHash = _passwordHasher.HashPassword(dto.NewPassword);
            await _userRepository.UpdateUserAsync(user);
            return ServiceResponse<bool>.Success(true, "Password changed success");
        }
        private async Task HandleImagesAsync(User user, UpdateProfileDto dto)
        {
            var existingUrl = dto.ExistingProfileImg ?? "";

            // Delete removed images
            if (!string.IsNullOrEmpty(existingUrl))
            {
                await _imageStorage.DeleteAsync(existingUrl);

            }
            //foreach (var delImg in hotel.HotelImgs.Where(img => !existingUrls.Contains(img.ImageUrl)).ToList())
            //{
            //    await _imageStorage.DeleteAsync(delImg.ImageUrl);
            //    await _hotelRepository.DeleteHotelImageAsync(delImg);
            //}

            // Upload new images
            if (dto.Avatar != null)
            {
                var uploadedUrl = await _imageStorage.UploadAsync(dto.Avatar, "Profile");
                user.AvatarUrl = uploadedUrl;


            }
        }
    }
}
