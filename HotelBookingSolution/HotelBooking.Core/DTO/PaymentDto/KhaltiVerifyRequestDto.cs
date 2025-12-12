using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Core.DTO.PaymentDto
{
    public class KhaltiVerifyRequestDto
    {
        public string TransactionId { get; set; }
        public decimal Amount { get; set; }
    }
}
