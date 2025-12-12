using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Core.Domain.Entities
{
    public class RoomReservationGuest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RoomReservationGuestID { get; set; }

       
        public int ReservationRoomID { get; set; }

     
        public int GuestID { get; set; }

        public ReservationRoom ReservationRoom { get; set; }
        public Guest Guest { get; set; }
    }
}
