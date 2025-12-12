using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace HotelBooking.Core.Domain.Entities
{
    
    public class RefreshToken
    {
        [Key]
        public int Id { get; set; }
        // The refresh token string (should be a secure random string)
        [Required]
        public string Token { get; set; }
        // The user associated with the refresh token
        [Required]
        public int UserId { get; set; }
      
        public User User { get; set; }
       
        // Token expiration date
        [Required]
        public DateTime ExpiresAt { get; set; }
        // Indicates if the token has been revoked
        [Required]
        public bool IsRevoked { get; set; } = false;
        // Date when the token was created
        [Required]
        public DateTime CreatedAt { get; set; }
        // Date when the token was revoked
        public DateTime? RevokedAt { get; set; }
    }
}
