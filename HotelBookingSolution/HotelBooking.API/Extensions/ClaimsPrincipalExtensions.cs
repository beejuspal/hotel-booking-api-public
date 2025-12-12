using System.Security.Claims;

namespace HotelBooking.API.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static string? GetEmail(this ClaimsPrincipal user)
            => user?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
    }

}
