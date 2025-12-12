using HotelBooking.Core.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Threading.Tasks;

namespace HotelBooking.API.Middleware
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class CustomAuthorizationResultHandler : IAuthorizationMiddlewareResultHandler
    {
        private readonly AuthorizationMiddlewareResultHandler _defaultHandler = new();

        public async Task HandleAsync(
            RequestDelegate next,
            HttpContext context,
            AuthorizationPolicy policy,
            PolicyAuthorizationResult authorizeResult)
        {
            if (!authorizeResult.Succeeded)
            {
                // Not authenticated
                if (!context.User.Identity?.IsAuthenticated ?? true)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    await context.Response.WriteAsJsonAsync(new APIResponseDto
                    {
                        StatusCode = HttpStatusCode.Unauthorized,
                        IsSuccess = false,
                        ErrorMessages = new List<string> { "You must be logged in to perform this action." }
                    });
                    return;
                }

                // Authenticated but not authorized
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                await context.Response.WriteAsJsonAsync(new APIResponseDto
                {
                    StatusCode = HttpStatusCode.Forbidden,
                    IsSuccess = false,
                    ErrorMessages = new List<string> { "You are not authorized to add a room type." }
                });
                return;
            }

            // If authorized, continue
            await _defaultHandler.HandleAsync(next, context, policy, authorizeResult);
        }
    }
}
