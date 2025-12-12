using HotelBooking.Core.DTO;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Core.Helpers
{
    public static class ErrorResponseHelper
    {
        public static IActionResult Create(HttpStatusCode code, string message)
        {
            var response = new APIResponseDto
            {
                StatusCode = code,
                IsSuccess = false
            };
            response.ErrorMessages.Add(message);

            // Return ObjectResult with proper status code
            return new ObjectResult(response) { StatusCode = (int)code };
        }
    }
}
