using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Core.DTO.UserDTOs
{
    public class DeleteUserResponseDTO
    {
        public string Message { get; set; }
        public bool IsDeleted { get; set; }
    }
}
