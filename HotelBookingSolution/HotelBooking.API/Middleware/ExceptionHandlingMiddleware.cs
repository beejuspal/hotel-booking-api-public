using HotelBooking.Core.Domain.Entities;
using HotelBooking.Core.DTO;
using HotelBooking.Core.DTO.UserDTOs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Serilog;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;


namespace HotelBooking.API.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private readonly IHostEnvironment _env;
        private readonly int _slowRequestThresholdMs;
        public ExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlingMiddleware> logger,
            IHostEnvironment env,int slowRequestThresholdMs = 500)
        {
            _next = next;
            _logger = logger;
            _env = env;
            _slowRequestThresholdMs = slowRequestThresholdMs;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var stopwatch = Stopwatch.StartNew();
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                await HandleExceptionAsync(httpContext, ex);
            }
            finally
            {
                stopwatch.Stop();
                var elapsedMs = stopwatch.ElapsedMilliseconds;
                var endpoint = $"{httpContext.Request.Method} {httpContext.Request.Path}";
                var statusCode = httpContext.Response.StatusCode;

                // Log normal response
                _logger.LogInformation("HTTP {Endpoint} responded {StatusCode} in {ElapsedMilliseconds} ms",
                    endpoint, statusCode, elapsedMs);

                // Log slow requests
                if (elapsedMs > _slowRequestThresholdMs)
                {
                    _logger.LogWarning("Slow Request: {Endpoint} responded {StatusCode} in {ElapsedMilliseconds} ms",
                        endpoint, statusCode, elapsedMs);
                }
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var ex = GetInnermostException(exception);
           
            // Log with structured logging
            _logger.LogError(ex, "Unhandled exception occurred. {Method} {Path}",
                context.Request.Method,
                context.Request.Path);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            var response = new APIResponseDto
            {
                StatusCode = HttpStatusCode.InternalServerError,
                IsSuccess = false
            };

            response.ErrorMessages.Add(_env.IsDevelopment()
                ? ex.Message // includes stack trace in dev
                : "An unexpected error occurred. Please try again later.");

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
        }

        private Exception GetInnermostException(Exception ex)
        {
            while (ex.InnerException != null)
            {
                ex = ex.InnerException;
            }
            return ex;
        }
    }

    // Extension
    public static class ExceptionHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionHandlingMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }
}
