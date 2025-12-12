using HotelBooking.Core.Domain.Entities;
using HotelBooking.Core.DTO;
using HotelBooking.Core.DTO.UserDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Core.ServiceContracts
{
    public interface IUserAdderService
    {
        Task<CreateUserResponseDTO> AddUser(CreateUserDTO? createuser);
        Task<ServiceResponse<ProfileDto>> UpdateProfileAsync(int userId, UpdateProfileDto dto);
        Task<ServiceResponse<bool>> ForgotPasswordAsync(ForgotPasswordDto dto);
        Task<ServiceResponse<bool>> ResetPasswordAsync(ResetPasswordDto dto);
        Task<ServiceResponse<bool>> ChangePasswordAsync(int userId, ChangePasswordDto dto);


    }
}
