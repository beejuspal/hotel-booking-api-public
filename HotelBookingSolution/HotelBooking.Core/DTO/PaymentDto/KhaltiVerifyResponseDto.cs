using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Core.DTO.PaymentDto
{
    public class KhaltiVerifyResponseDto
    {
        public string Pidx { get; set; }
        public string Status { get; set; } // completed, pending, expired, refunded, etc.
        public string TransactionId { get; set; }
        public string Amount { get; set; }
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }
}
