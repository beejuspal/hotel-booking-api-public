using HotelBooking.Core.Domain.Entities;
using HotelBooking.Core.Domain.RepositoryContracts;
using HotelBooking.Infrastructure.DBContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Infrastructure.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly HotelDbContext _context;

        public AuthRepository(HotelDbContext context)
        {
            _context = context;
        }

        public async Task<int> RegisterUser(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user.UserID;
        }

        public async Task<User?> GetUserByEmail(string email)
        {
            return await _context.Users.Include(u => u.Role).Include(x=>x.Hotel).FirstOrDefaultAsync(temp => temp.Email == email);
        }
        public async Task SaveUserToken(RefreshToken token)
        {
            _context.RefreshTokens.Add(token);
            await _context.SaveChangesAsync();

            
        }
       

    }
}
