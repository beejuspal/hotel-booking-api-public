using HotelBooking.Core.Helpers;
using HotelBooking.Core.ServiceContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Core.Services
{
    public class PasswordHasherService:IPasswordHasherService
    {
        public string HashPassword(string password)
        {
            return PasswordHasherHelper.HashPassword(password);
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            return PasswordHasherHelper.VerifyPassword(password, hashedPassword);
        }
    }
}
