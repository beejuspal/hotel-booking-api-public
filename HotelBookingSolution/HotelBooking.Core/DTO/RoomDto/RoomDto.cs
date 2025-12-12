using HotelBooking.Core.DTO.HotelDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Core.DTO.RoomDto
{
    public class RoomDto
    {
        public int RoomID { get; set; }
        public string RoomNumber { get; set; }
        public int RoomTypeID { get; set; }
        public int HotelId { get; set; }
        public decimal Price { get; set; }
        public string BedType { get; set; }
        public string ViewType { get; set; }
        public string Status { get; set; } // "Available", "Under Maintenance", "Occupied"
        public bool IsActive { get; set; } = true;
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public List<RoomImageDto> RoomImageUrls { get; set; } = new();
    }
}
