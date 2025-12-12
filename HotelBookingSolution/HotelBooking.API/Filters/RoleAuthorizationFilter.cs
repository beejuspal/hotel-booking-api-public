using HotelBooking.Core.DTO;
using HotelBooking.Core.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;
using System.Security.Claims;

namespace HotelBooking.API.Filters
{
    //[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    //public class RoleAuthorizationAttribute : Attribute, IAuthorizationFilter
    //{
    //    private readonly string? _requiredRole;

    //    public RoleAuthorizationAttribute(string? requiredRole = null)
    //    {
    //        _requiredRole = requiredRole;
    //    }

    //    public void OnAuthorization(AuthorizationFilterContext context)
    //    {
    //        var user = context.HttpContext.User;

    //        // 1️⃣ Not logged in
    //        if (!user.Identity?.IsAuthenticated ?? true)
    //        {
    //            context.Result = new JsonResult(new APIResponseDto
    //            {
    //                StatusCode = HttpStatusCode.Unauthorized,
    //                IsSuccess = false,
    //                ErrorMessages = new List<string> { "You must be logged in to perform this action." }
    //            })
    //            {
    //                StatusCode = (int)HttpStatusCode.Unauthorized
    //            };
    //            return;
    //        }

    //        // 2️⃣ Role check if specified
    //        if (!string.IsNullOrEmpty(_requiredRole))
    //        {
    //            var role = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
    //            if (role != _requiredRole)
    //            {
    //                context.Result = new JsonResult(new APIResponseDto
    //                {
    //                    StatusCode = HttpStatusCode.Forbidden,
    //                    IsSuccess = false,
    //                    ErrorMessages = new List<string> { $"Only {_requiredRole} users can perform this action." }
    //                })
    //                {
    //                    StatusCode = (int)HttpStatusCode.Forbidden
    //                };
    //            }
    //        }
    //    }
    //}
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class RoleAuthorizationAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string[] _allowedRoles;

        public RoleAuthorizationAttribute(params string[] allowedRoles)
        {
            _allowedRoles = allowedRoles;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            // 1️⃣ Not logged in
            if (!user.Identity?.IsAuthenticated ?? true)
            {
                context.Result = new JsonResult(new APIResponseDto
                {
                    StatusCode = HttpStatusCode.Unauthorized,
                    IsSuccess = false,
                    ErrorMessages = new List<string> { "You must be logged in to perform this action." }
                })
                {
                    StatusCode = (int)HttpStatusCode.Unauthorized
                };
                return;
            }

            // 2️⃣ Role-based authorization
            var userRole = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            if (_allowedRoles.Length > 0 && !_allowedRoles.Contains(userRole))
            {
                context.Result = new JsonResult(new APIResponseDto
                {
                    StatusCode = HttpStatusCode.Forbidden,
                    IsSuccess = false,
                    ErrorMessages = new List<string> { $"Access denied. Allowed roles: {string.Join(", ", _allowedRoles)}" }
                })
                {
                    StatusCode = (int)HttpStatusCode.Forbidden
                };
            }
        }
    }
}

