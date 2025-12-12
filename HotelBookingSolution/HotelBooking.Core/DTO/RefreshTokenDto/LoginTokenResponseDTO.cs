using HotelBooking.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Core.DTO.RefreshTokenDto
{
    public class LoginTokenResponseDTO
    {
      
        public string Token { get; set; }
        public string RefreshToken { get; set; }
       
    }
}
