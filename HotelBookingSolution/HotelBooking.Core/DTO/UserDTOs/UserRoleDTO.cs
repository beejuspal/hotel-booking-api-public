using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Core.DTO.UserDTOs
{
    public class UserRoleDTO
    {
        [Required(ErrorMessage = "User Id is Required")]
        public int UserID { get; set; }
        [Required(ErrorMessage = "Role Id is Required")]
        public int RoleID { get; set; }
    }
}
