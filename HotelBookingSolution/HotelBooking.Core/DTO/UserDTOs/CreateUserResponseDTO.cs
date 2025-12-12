using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Core.DTO.UserDTOs
{
    public class CreateUserResponseDTO
    {
        public int UserId { get; set; }
       
        public bool IsCreated { get; set; }
    }
}
