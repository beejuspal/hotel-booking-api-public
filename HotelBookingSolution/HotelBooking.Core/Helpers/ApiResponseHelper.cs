using HotelBooking.Core.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Core.Helpers
{
    public static class ApiResponseHelper
    {
        public static IActionResult FromServiceResponse<T>(
            ServiceResponse<T> serviceResponse,
            ILogger logger = null)  // optional logger
        {
            if (serviceResponse == null)
            {
                logger?.LogError("ServiceResponse is null");
                return new ObjectResult(new APIResultResponseDto<object>
                {
                    IsSuccess = false,
                    Message = "No response from service",
                    StatusCode = HttpStatusCode.InternalServerError
                })
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError
                };
            }

            if (!serviceResponse.IsSuccess)
            {
                logger?.LogWarning("Service failed: {Message}", serviceResponse.Message);
                return new ObjectResult(APIResultResponseDto<object>.Fail(
                    serviceResponse.Message, serviceResponse.StatusCode))
                {
                    StatusCode = (int)serviceResponse.StatusCode
                };
            }

            logger?.LogInformation("Service succeeded: {Message}", serviceResponse.Message);
            return new OkObjectResult(APIResultResponseDto<object>.Success(
                serviceResponse.Data, serviceResponse.Message));
        }
    }


}
