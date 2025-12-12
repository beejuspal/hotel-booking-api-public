using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Core.Domain.Entities
{
    public class ReservationRoom
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ReservationRoomID { get; set; }

        public int ReservationID { get; set; }
        public int RoomID { get; set; }

        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }

        // Navigation properties
        public Reservation Reservation { get; set; }
        public Room Room { get; set; }
        public ICollection<RoomReservationGuest> RoomReservationGuests { get; set; }
    }

}
