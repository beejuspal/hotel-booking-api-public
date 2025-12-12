using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Core.Helpers
{
    public static class PasswordHasherHelper
    {
        public static string HashPassword(string password) =>
         BCrypt.Net.BCrypt.HashPassword(password);

        public static bool VerifyPassword(string password, string hash) =>
            BCrypt.Net.BCrypt.Verify(password, hash);
    }
}
