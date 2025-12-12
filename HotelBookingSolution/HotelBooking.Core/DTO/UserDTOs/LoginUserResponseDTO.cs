using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Core.DTO.UserDTOs
{
    public class LoginUserResponseDTO
    {
        public int UserId { get; set; }
        public string Message { get; set; }
        public bool IsLogin { get; set; }
    }
    public class ProfileDto
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public bool Verified { get; set; }
        public string? Avatar { get; set; }
        public string? Dob { get; set; }
       
        public string? ContactNo { get; set; }
        public string? Address { get; set; }
        public string? Role { get; set; }
        public string? HotelName { get; set; }
    }

    public class TokensDto
    {
        public string Refresh { get; set; }
        public string Access { get; set; }
    }

    public class SignInResDto
    {
        public ProfileDto Profile { get; set; }
        public TokensDto Tokens { get; set; }
    }
    public class ProfileResDto
    {
        public ProfileDto Profile { get; set; }
      
    }
}
