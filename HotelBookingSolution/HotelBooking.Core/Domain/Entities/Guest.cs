using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Core.Domain.Entities
{
    public class Guest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int GuestID { get; set; }
        public int? UserID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string AgeGroup { get; set; } // "Adult", "Child", "Infant"
        public string Address { get; set; }
        public int CountryID { get; set; }
        public int StateID { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public User User { get; set; }
        public Country Country { get; set; }
        public State State { get; set; }
        public ICollection<RoomReservationGuest> RoomReservationGuests { get; set; }
        public ICollection<Feedback> Feedbacks { get; set; }
    }
}
