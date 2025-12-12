using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Core.Domain.Entities
{
    public class Hotel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int HotelId { get; set; } // PK
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int StarRating { get; set; } // e.g., 3, 4, 5 stars
        public string Description { get; set; } = string.Empty;

        // Navigation property - One Hotel can have many RoomTypes
        public ICollection<RoomType> RoomTypes { get; set; } = new List<RoomType>();
        public ICollection<HotelImage> HotelImgs { get; set; } = new List<HotelImage>();
    }
}
