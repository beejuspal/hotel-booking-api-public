using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Core.DTO.RefreshTokenDto
{
    public class RefreshTokenRequestDTO
    {
        [Required]
        public string RefreshToken { get; set; }
        [Required]
        public int UserId { get; set; }
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
