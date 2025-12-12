using HotelBooking.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Core.Domain.RepositoryContracts
{
    public interface ITokenRepository
    {
        Task SaveUserToken(RefreshToken token);
        Task<RefreshToken?> GetRefreshToken(string hashedToken);
        Task RevokeRefreshToken(int userId);
    }
}
