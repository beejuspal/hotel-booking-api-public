using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Core.DTO.RefreshTokenDto
{
    public class UserRefreshTokenRequestDto
    {
        [Required]
        public string RefreshToken { get; set; }
    }
}
