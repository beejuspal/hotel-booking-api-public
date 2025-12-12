using HotelBooking.Core.Domain.Entities;
using HotelBooking.Core.DTO.UserDTOs;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Core.Helpers
{
    public static class TokenHelper
    {

        // Private method responsible for generating a JWT token for an authenticated user
        public static string GenerateJwtToken(UserResponseDTO user, string screteKey, string audience, string issuer, string expTime)
        {

            JwtSecurityTokenHandler tokenHandler = new();
            byte[] key = Encoding.ASCII.GetBytes(screteKey);

            SecurityTokenDescriptor tokenDes = new()
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim("fullName",user.FullName),
                    new Claim(ClaimTypes.NameIdentifier,user.UserID.ToString()),
                     new Claim("id",user.UserID.ToString()),
                    new Claim(ClaimTypes.Email,user.Email),
                     new Claim(ClaimTypes.Role,user.RoleName),
                }),
                Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(expTime)),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)

            };

            SecurityToken token = tokenHandler.CreateToken(tokenDes);

            // Serialize the token to a string
            var authToken = tokenHandler.WriteToken(token);
            // Return the serialized JWT token
            return authToken;
        }

        // Helper method to generate a secure random refresh token
        public static string GenerateRefreshToken()
        {
            //A secure random string is generated using RandomNumberGenerator
            var randomNumber = new byte[64];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }
        public static string HashToken(string token)
        {
            //The refresh token is hashed using SHA256 before storing it in the database to prevent token theft from compromising security.
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(token));
            return Convert.ToBase64String(hashedBytes);
        }
    }
}
