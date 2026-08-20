// ===================================THIS FILE WAS AUTO GENERATED===================================
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;

namespace Arcora.Api.Middleware
{
    /// <summary>
    /// Middleware that catches unhandled exceptions and returns a standardized JSON error response.
    /// </summary>
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;
        /// <summary>
        /// Initializes a new instance of the <see cref = "GlobalExceptionMiddleware"/> class.
        /// </summary>
        /// <param name = "next">The next middleware in the pipeline.</param>
        /// <param name = "logger">The logger instance.</param>
        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        /// <summary>
        /// Invokes the middleware, catching and handling exceptions.
        /// </summary>
        /// <param name = "context">The HTTP context.</param>
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access attempt");
                await WriteErrorResponse(context, HttpStatusCode.Unauthorized, "You are not authorized to perform this action.");
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Configuration or operation error");
                await WriteErrorResponse(context, HttpStatusCode.BadRequest, ex.Message);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument provided");
                await WriteErrorResponse(context, HttpStatusCode.BadRequest, ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Resource not found");
                await WriteErrorResponse(context, HttpStatusCode.NotFound, ex.Message);
            }
            catch (FileNotFoundException ex)
            {
                _logger.LogWarning(ex, "Resource not found");
                await WriteErrorResponse(context, HttpStatusCode.NotFound, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred");
                await WriteErrorResponse(context, HttpStatusCode.InternalServerError, "An unexpected error occurred. Please try again later.");
            }
        }

        private static async Task WriteErrorResponse(HttpContext context, HttpStatusCode statusCode, string message)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;
            var response = new
            {
                StatusCode = (int)statusCode,
                Message = message
            };
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            await context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
        }
    }
}