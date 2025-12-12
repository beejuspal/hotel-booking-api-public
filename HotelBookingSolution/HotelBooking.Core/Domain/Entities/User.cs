using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Core.Domain.Entities
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserID { get; set; }
        public int RoleID { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }

        public string PasswordHash { get; set; }
        public DateTime CreatedAt { get; set; } 
        public DateTime? LastLogin { get; set; }
        public bool IsActive { get; set; } = true;
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; } 
        public string? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? AvatarUrl { get; set; } // Profile image
        public string? PhoneNumber { get; set; } // Contact number
        public string? Address { get; set; }
        public DateTime? DOB { get; set; }// Contact address
        public string? PasswordResetToken { get; set; }
        public DateTime? PasswordResetExpires { get; set; }
        public UserRole Role { get; set; }
        public ICollection<Reservation> Reservations { get; set; }
        public ICollection<Guest> Guests { get; set; }
        public int? HotelID { get; set; } // Null for SuperAdmin and Customers
        public Hotel Hotel { get; set; }
        public ICollection<Refund> RefundsProcessed { get; set; }
        // Navigation property for refresh tokens
        public ICollection<RefreshToken> RefreshTokens { get; set; }
    }
}
