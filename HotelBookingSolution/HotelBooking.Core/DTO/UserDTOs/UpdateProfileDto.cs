using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Core.DTO.UserDTOs
{
    public class UpdateProfileDto
    {
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public IFormFile Avatar { get; set; } // for image upload
        public DateTime DOB { get; set; }

        public string ModifiedBy { get; set; }
        public string? ExistingProfileImg { get; set; }
    }
}
