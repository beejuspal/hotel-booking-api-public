using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Core.Domain.Entities
{
    public class RefundMethod
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MethodID { get; set; }
        public string MethodName { get; set; }
        public bool IsActive { get; set; } = true;

        public ICollection<Refund> Refunds { get; set; }
    }
}
