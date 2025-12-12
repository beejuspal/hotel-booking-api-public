using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Core.Domain.Entities
{
    public class Room
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RoomID { get; set; }
        public string RoomNumber { get; set; }
        public int RoomTypeID { get; set; }
        public decimal Price { get; set; }
        public string BedType { get; set; }
        public string ViewType { get; set; }
        public string Status { get; set; } // "Available", "Under Maintenance", "Occupied"
        public bool IsActive { get; set; } = true;
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; } 
        public string? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
     
        public RoomType RoomType { get; set; }
        public ICollection<ReservationRoom> ReservationRooms { get; set; }
        public ICollection<RoomImage> RoomImgs { get; set; } = new List<RoomImage>();
    }
}
