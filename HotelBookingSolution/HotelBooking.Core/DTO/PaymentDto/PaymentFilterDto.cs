using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Core.DTO.PaymentDto
{
    public class PaymentFilterDto
    {
        public string? Status { get; set; }   // e.g. "Confirmed", "Cancelled"
      
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string SearchQuery { get; set; }
        public bool ViewOwn { get; set; }
    }
}
