using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Core.DTO.ReservationDto
{
    public class ReservationFilterDto
    {
        public string? Status { get; set; }   // e.g. "Confirmed", "Cancelled"
        public DateTime? StartDate { get; set; }  // filter by CheckInDate >=
        public DateTime? EndDate { get; set; }    // filter by CheckOutDate <=
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        
    }
    public class ReservationAdminFilterDto
    {
        public string? Status { get; set; }   // e.g. "Confirmed", "Cancelled"
        public DateTime? StartDate { get; set; }  // filter by CheckInDate >=
        public DateTime? EndDate { get; set; }    // filter by CheckOutDate <=
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string SearchQuery {  get; set; }

    }

}
