using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Core.DTO.UserDTOs
{
    public class UpdateUserResponseDTO
    {
        public int UserId { get; set; }
        public string Message { get; set; }
        public bool IsUpdated { get; set; }
    }
}
