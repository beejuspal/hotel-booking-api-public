using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Core.DTO.ReservationDto
{
    public class ReservationCreateRequestDto
    {
        public List<int> roomIds { get; set; }
        public DateTime checkInDate { get; set; }
        public DateTime checkOutDate { get; set; }
        public GuestDetail guest { get; set; }
        public int UserID { get; set; }
        public string? CreatedBy { get; set; }
    }
    public class GuestDetail
    {
        public int UserID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string AgeGroup { get; set; }
        public string Address { get; set; }
    }
}
