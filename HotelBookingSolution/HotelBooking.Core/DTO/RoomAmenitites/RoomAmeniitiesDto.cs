using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Core.DTO.RoomAmenitites
{
    public class RoomAmeniitiesDto
    {
        public int RoomTypeID { get; set; }
        public List<int> AmenityIDs { get; set; } = new();
    }
}
