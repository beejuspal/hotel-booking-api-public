using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Core.DTO.HotelDto
{
    public class HotelSearchRequestDto
    {
        public string? SearchQuery { get; set; }
      
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
       
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int HotelId { get; set; }
    }
}
