using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Core.DTO
{
   
    public class APIResultResponseDto<T>
    {
        public HttpStatusCode StatusCode { get; set; }
        public bool IsSuccess { get; set; }
        public List<string> ErrorMessages { get; set; } = new();
        public T? Result { get; set; }
        public string Message { get; set; } = string.Empty;

        public static APIResultResponseDto<T> Success(T result, string message = "", HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            return new APIResultResponseDto<T>
            {
                IsSuccess = true,
                StatusCode = statusCode,
                Result = result,
                Message = message
            };
        }

        public static APIResultResponseDto<T> Fail(string message, HttpStatusCode statusCode = HttpStatusCode.BadRequest, List<string>? errors = null)
        {
            return new APIResultResponseDto<T>
            {
                IsSuccess = false,
                StatusCode = statusCode,
                Message = message,
                ErrorMessages = errors ?? new List<string>()
            };
        }
    }
}
