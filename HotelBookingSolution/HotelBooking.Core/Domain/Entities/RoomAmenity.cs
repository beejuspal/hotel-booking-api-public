using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Core.Domain.Entities
{
    public class RoomAmenity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RoomAmenityID { get; set; }
        public int RoomTypeID { get; set; }
        public int AmenityID { get; set; }

        public RoomType RoomType { get; set; }
        public Amenity Amenity { get; set; }
    }
}
