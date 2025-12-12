using HotelBooking.Core.Domain.Entities;
using HotelBooking.Core.Domain.RepositoryContracts;
using HotelBooking.Infrastructure.DBContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Infrastructure.Repositories
{
    public class TokenRepository : ITokenRepository
    {
        private readonly HotelDbContext _context;

        public TokenRepository(HotelDbContext context)
        {
            _context = context;
        }

        public async Task SaveUserToken(RefreshToken token)
        {
            _context.RefreshTokens.Add(token);
            await _context.SaveChangesAsync();
        }

        public async Task<RefreshToken?> GetRefreshToken(string hashedToken)
        {
            return await _context.RefreshTokens
     .Include(rt => rt.User)
         .ThenInclude(u => u.Hotel)
     .Include(rt => rt.User)
         .ThenInclude(u => u.Role)
     .FirstOrDefaultAsync(rt => rt.Token == hashedToken);
        }

        public async Task RevokeRefreshToken(int userId)
        {
            var userRefreshTokens = await _context.RefreshTokens
                .Where(rt => rt.UserId == userId && !rt.IsRevoked)
                .ToListAsync();

            foreach (var token in userRefreshTokens)
            {
                token.IsRevoked = true;
                token.RevokedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
        }
    }

}
