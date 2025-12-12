using HotelBooking.Core.Domain.RepositoryContracts;
using HotelBooking.Core.DTO.UserDTOs;
using HotelBooking.Core.ServiceContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Core.Services
{
    public class UserGetterService : IUserGetterService
    {
        private readonly IUserRepository _userRepository;
        public UserGetterService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<UserResponseDTO> GetUserProfileAsync(string email, int userId)
        {
            //var userespDto = new UserResponseDTO();
            //var res= await _userRepository.GetUserByEmailAsync(email);
            //if (res==null)
            //{
            //    return userespDto;
            //}
            //userespDto.Email = res.Email;
            //userespDto.UserID= res.UserID;
            //userespDto.FullName= res.FullName;
            //userespDto.RoleName=res.Role.RoleName;
            //return userespDto;
            //.Select(user => new UserResponseDTO
            //{
            //    Email = user.Email,
            //    UserID = user.UserID,
            //    FullName = user.FullName,
            //    RoleName = user.Role.RoleName,
            //    HotelID = user.HotelID,
            //    HotelName = user.Hotel != null ? user.Hotel.Name : ""
            //})
            var user = await _userRepository.GetUserByEmailAsync(email, 0);
            return new UserResponseDTO
            {
                Email = user.Email,
                UserID = user.UserID,
                FullName = user.FullName,
                RoleName = user.Role.RoleName,
                HotelID = user.HotelID,
                HotelName = user.Hotel != null ? user.Hotel.Name : "",
                Dob = user.DOB.ToString(),
                Avatar = user.AvatarUrl,
                ContactNo = user.PhoneNumber,
                Address = user.Address
            };
        }
    }
}
