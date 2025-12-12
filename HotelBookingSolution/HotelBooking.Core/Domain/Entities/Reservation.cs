using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Core.Domain.Entities
{
    public class Reservation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ReservationID { get; set; }
        public DateTime BookingDate { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }

      
        public int UserID { get; set; }
        public User User { get; set; }
       
        public decimal TotalCost { get; set; }
        public int NumberOfNights { get; set; }

        public ICollection<ReservationRoom> ReservationRooms { get; set; }
      
        public ICollection<Payment> Payments { get; set; }
        public ICollection<Cancellation> Cancellations { get; set; }
        public ICollection<Feedback> Feedbacks { get; set; }
    }



}
