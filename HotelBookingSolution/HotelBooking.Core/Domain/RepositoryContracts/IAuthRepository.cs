using HotelBooking.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Core.Domain.RepositoryContracts
{
    public interface IAuthRepository
    {
        // Task<int> RegisterUser(User user );
        //Task<User?> GetUserByEmail(string email);
        //Task SaveUserToken(RefreshToken token);
        //Task<RefreshToken?> GetRefreshToken(string token);
        //Task RevokeRefreshToken(int userId);
        Task<int> RegisterUser(User user);
        Task<User?> GetUserByEmail(string email);

    }
}
