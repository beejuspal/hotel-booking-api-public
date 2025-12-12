using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Core.Domain.Entities
{
    public class RoomType
    {
     
        public int RoomTypeID { get; set; }
        public string TypeName { get; set; }
        public string AccessibilityFeatures { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; } = true;
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; } 
        public string? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? RoomTypeImage { get; set; }
        // Foreign key to Hotel
        public int HotelId { get; set; }
        public Hotel Hotel { get; set; } = null!;
        public ICollection<Room> Rooms { get; set; }
        public ICollection<RoomAmenity> RoomAmenities { get; set; }
    }
}
