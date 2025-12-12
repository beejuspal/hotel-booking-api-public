using HotelBooking.Core.Domain.Entities;
using HotelBooking.Core.Domain.RepositoryContracts;
using HotelBooking.Core.DTO.RoomTypeDTOs;
using HotelBooking.Core.DTO.UserDTOs;
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
    public class UserRepository : IUserRepository
    {
        private readonly HotelDbContext _context;

        public UserRepository(HotelDbContext context)
        {
            _context = context;
        }

        public async Task<(int userId, string errorMessage)> AddUserAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return (user.UserID, null);
        }


        public async Task<User> GetUserByEmailAsync(string email, int UserId = 0)
        {
            if (UserId > 0)
            {
                return await _context.Users
    .Where(u => u.UserID == UserId).Include(x=>x.Role).Include(x=>x.Hotel)
    
    .FirstOrDefaultAsync();
            }
            return await _context.Users
    .Where(u => u.Email == email).Include(x => x.Role).Include(x => x.Hotel)
    //.Select(user => new UserResponseDTO
    //{
    //    Email = user.Email,
    //    UserID = user.UserID,
    //    FullName = user.FullName,
    //    RoleName = user.Role.RoleName,
    //    HotelID=user.HotelID,
    //    HotelName = user.Hotel != null ? user.Hotel.Name : ""
    //})
    .FirstOrDefaultAsync();

        }
        public async Task<int> UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return user.UserID;
        }
        public async Task<User> CheckPasswordReset(string token)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.PasswordResetToken == token && u.PasswordResetExpires > DateTime.UtcNow);
    
        }
    }
}
