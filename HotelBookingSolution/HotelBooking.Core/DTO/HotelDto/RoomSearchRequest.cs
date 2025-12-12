using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Core.DTO.HotelDto
{
    public class RoomSearchRequest
    {
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string? RoomTypeName { get; set; }
        public string? AmenityName { get; set; }
      
        public int? HotelID { get; set; }
    }
}
