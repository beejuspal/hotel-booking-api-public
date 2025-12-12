using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Core.Domain.Entities
{
    public class Cancellation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CancellationID { get; set; }
        public int ReservationID { get; set; }
        public DateTime CancellationDate { get; set; }
        public string Reason { get; set; }
        public decimal CancellationFee { get; set; }
        public string CancellationStatus { get; set; } // "Pending", "Approved", "Denied"
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; } 
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public Reservation Reservation { get; set; }
    }
}
