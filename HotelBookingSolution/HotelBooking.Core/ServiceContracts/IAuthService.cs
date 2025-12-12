using HotelBooking.Core.DTO;
using HotelBooking.Core.DTO.RefreshTokenDto;
using HotelBooking.Core.DTO.UserDTOs;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Core.ServiceContracts
{
    public interface IAuthService
    {
        //Task<CreateUserResponseDTO> RegisterUser(CreateUserDTO? createuser);
        //Task<UserResponseDTO> CheckUser(LoginUserDTO? loginDto);
        //Task SaveUserRefreshToken(RefreshTokenRequestDTO? refreshTokenDto);
        //Task<TokenResponseDTO> GetRefreshToken(string hashedToken);
        //Task RevokeUserRefreshToken(int userId);

        Task<ServiceResponse<CreateUserResponseDTO>> RegisterAsync(CreateUserDTO createUserDTO);
        Task<ServiceResponse<UserResponseDTO>> LoginAsync(LoginUserDTO loginDto);
        Task<ServiceResponse<TokenResponseDTO>> RefreshTokenAsync(UserRefreshTokenRequestDto requestDto);
        Task<ServiceResponse<object>> LogoutAsync(LogoutRequestDTO requestDto);
    }
}
