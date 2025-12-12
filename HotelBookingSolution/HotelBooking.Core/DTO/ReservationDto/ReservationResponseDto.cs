using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Core.DTO.ReservationDto
{
    public class ReservationResponseDto
    {
        public int ReservationId { get; set; }
        public double TotalCost { get; set; }
        public double BaseCost { get; set; }
        public int NumberOfNights { get; set; }
        public decimal Tax { get; set; }
        public DateTime BookingDate { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
    }
}
