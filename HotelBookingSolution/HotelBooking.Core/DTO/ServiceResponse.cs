using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Core.DTO
{
    public class ServiceResponse<T>
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public static ServiceResponse<T> Success(T data, string message = "", HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            return new ServiceResponse<T>
            {
                IsSuccess = true,
                Message = message,
                Data = data,
                StatusCode = statusCode
            };
        }

        public static ServiceResponse<T> Fail(HttpStatusCode statusCode,string message, T? data = default)
        {
            return new ServiceResponse<T>
            {
                IsSuccess = false,
                Message = message,
                Data = data,
                StatusCode = statusCode
            };
        }
    }

}
