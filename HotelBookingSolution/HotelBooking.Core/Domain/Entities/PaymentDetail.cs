using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Core.Domain.Entities
{
    public class PaymentDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PaymentDetailID { get; set; }

        public int PaymentID { get; set; }
        public int ReservationRoomID { get; set; }

        public decimal Amount { get; set; }        // Base amount
        public int NumberOfNights { get; set; }
        public decimal GST { get; set; }           // GST on base amount
        public decimal TotalAmount { get; set; }   // (Amount * NumberOfNights) + GST

        // Navigation
        public Payment Payment { get; set; }
        public ReservationRoom ReservationRoom { get; set; }
    }

}
