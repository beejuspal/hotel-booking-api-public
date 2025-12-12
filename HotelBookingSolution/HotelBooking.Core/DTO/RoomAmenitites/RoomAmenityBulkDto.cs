using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Core.DTO.RoomAmenitites
{
    public class RoomAmenityBulkDto
    {
        public int RoomTypeID { get; set; }
        public List<int> AmenityIDs { get; set; } = new();
    }
}
