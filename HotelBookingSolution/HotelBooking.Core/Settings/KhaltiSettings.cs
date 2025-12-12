using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Core.Settings
{
    public class KhaltiSettings
    {
        public string BaseUrl { get; set; }
        public string SecretKey { get; set; }
        public string ReturnUrl { get; set; }
        public string WebSiteUrl { get; set; }
        public string PaymentUrl { get; set; }
        public string VerifyUrl { get; set; }
    }
}
