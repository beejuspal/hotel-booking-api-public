using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Core.DTO.AmenityDto
{
    public class AmenityInsertDTO
    {
        [Required]
        [StringLength(100, ErrorMessage = "Name length can't be more than 100 characters.")]
        public string Name { get; set; }
        [StringLength(255, ErrorMessage = "Description length can't be more than 255 characters.")]
        public string Description { get; set; }
        public string? CreatedBy { get; set; }
    }
}
