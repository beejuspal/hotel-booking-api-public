using HotelBooking.Core.Domain.Entities;
using HotelBooking.Core.DTO.UserDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Core.Domain.RepositoryContracts
{
    public interface IUserRepository
    {
        Task<(int userId, string errorMessage)> AddUserAsync(User user);

        Task<User> GetUserByEmailAsync(string email, int UserId = 0);
        Task<int> UpdateUserAsync(User user);
        Task<User> CheckPasswordReset(string token);


    }
}
