using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Core.Domain.Entities
{
    public class State
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int StateID { get; set; }
        public string StateName { get; set; }
        public int CountryID { get; set; }
        public bool IsActive { get; set; } = true;

        public Country Country { get; set; }
        public ICollection<Guest> Guests { get; set; }
    }
}
