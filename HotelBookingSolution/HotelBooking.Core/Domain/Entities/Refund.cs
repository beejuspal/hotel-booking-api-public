using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Core.Domain.Entities
{
    public class Refund
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RefundID { get; set; }

        public int PaymentID { get; set; }
        public decimal RefundAmount { get; set; }
        public DateTime RefundDate { get; set; } = DateTime.Now;
        [MaxLength(255)]
        public string RefundReason { get; set; }
        public int RefundMethodID { get; set; }
        public int ProcessedByUserID { get; set; }
        [MaxLength(50)]
        public string RefundStatus { get; set; } // e.g., Pending, Completed

        // Navigation
        public Payment Payment { get; set; }
        public RefundMethod RefundMethod { get; set; }
        public User ProcessedByUser { get; set; }
    }

}
