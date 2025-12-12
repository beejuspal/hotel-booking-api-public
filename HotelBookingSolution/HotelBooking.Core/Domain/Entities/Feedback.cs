using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Core.Domain.Entities
{
    public class Feedback
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int FeedbackID { get; set; }
        public int ReservationID { get; set; }
        public int GuestID { get; set; }
        public int Rating { get; set; } // 1-5
        public string Comment { get; set; }
        public DateTime FeedbackDate { get; set; }

        public Reservation Reservation { get; set; }
        public Guest Guest { get; set; }
    }
}
