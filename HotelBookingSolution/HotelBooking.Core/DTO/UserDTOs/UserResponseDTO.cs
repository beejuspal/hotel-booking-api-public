using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Core.DTO.UserDTOs
{
    public class UserResponseDTO
    {
        public int UserID { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public bool IsActive { get; set; }
        public DateTime? LastLogin { get; set; }
        public int RoleID { get; set; }
        public string RoleName { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public int? HotelID { get; set; }
        public string HotelName { get; set; }
        public string? Avatar { get; set; }
        public string? Dob { get; set; }

        public string? ContactNo { get; set; }
        public string? Address { get; set; }
    }
}
