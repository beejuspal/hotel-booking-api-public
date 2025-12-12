using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Core.DTO.RoomCostDto
{
    public class RoomCostDetailDto
    {
        public int RoomID { get; set; }
        public string RoomNumber { get; set; }
        public decimal RoomPrice { get; set; }  
        public int NumberOfNights { get; set; }
        public decimal TotalPrice { get; set; }
        public string RoomStatus { get; set; }
        public int RoomTypeID { get; set; }


    }

    public class RoomCostResultDto
    {
        public string CheckIn { get; set; }
        public string CheckOut { get; set; }
        public int HotelID { get; set; }
        public decimal BaseAmount { get; set; }
        public decimal GST { get; set; }
        public decimal TotalAmount { get; set; }
        public List<RoomCostDetailDto> RoomCostDetails { get; set; } = new();

    }
}
