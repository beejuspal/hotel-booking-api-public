using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Core.DTO.RoomTypeDTOs
{
    public class CreateRoomTypeDTO
    {
        [Required]
        public string TypeName { get; set; }
        public string AccessibilityFeatures { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public int HotelId { get; set; }
        [Required]
        public string CreatedBy { get; set; }
    }
}
