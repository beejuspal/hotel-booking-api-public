using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Core.DTO.PaymentDto
{
    public class KhaltiInitiateResponseDto
    {
        [JsonProperty("pidx")]
        public string Pidx { get; set; }

        [JsonProperty("payment_url")]
        public string PaymentUrl { get; set; }

        [JsonProperty("expires_at")]
        public DateTime ExpiresAt { get; set; }
    }

}
