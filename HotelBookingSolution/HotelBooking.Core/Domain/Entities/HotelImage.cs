using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Core.Domain.Entities
{
    public class HotelImage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int HotelImageId { get; set; }
        public string ImageUrl { get; set; } = string.Empty; // Or local file path
        public string Caption { get; set; } = string.Empty; // Optional

        // Foreign Key
        public int HotelId { get; set; }
        public Hotel Hotel { get; set; } = null!;
    }
}
