using HotelBooking.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Core.DTO.RefreshTokenDto
{
    public class LogOutResponseDTO
    {
       
        public string Message { get; set; }
        public bool IsSuccess { get; set; }
    }
}
