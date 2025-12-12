using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Core.Domain.Entities
{
    public class Payment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PaymentID { get; set; }

        public int ReservationID { get; set; }
        public decimal Amount { get; set; }
        public decimal GST { get; set; }
        public decimal TotalAmount { get; set; }

        public DateTime PaymentDate { get; set; } = DateTime.Now;

        [MaxLength(50)]
        public string PaymentMethod { get; set; }

        [MaxLength(50)]
        public string PaymentStatus { get; set; } = "Pending"; // Pending, Completed, Failed, Refunded

        public string FailureReason { get; set; }
        [MaxLength(50)]
        public string TransactionId { get; set; }
        // Navigation
        public Reservation Reservation { get; set; }
        public ICollection<PaymentDetail> PaymentDetails { get; set; }
        public ICollection<Refund> Refunds { get; set; }
    }

}
