using Ecommerce.Shared.Abstraction.Errors;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace Ecommerce.API.MiddleWares
{
    public class ExceptionMiddleWare(RequestDelegate next, ILogger<ExceptionMiddleWare> logger, IWebHostEnvironment environment)
    {
        private readonly RequestDelegate _next = next;
        private readonly ILogger<ExceptionMiddleWare> _logger = logger;
        private readonly IWebHostEnvironment _environment = environment;

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                var errorId = Guid.NewGuid().ToString();

                _logger.LogError(ex, "Unhandled exception occurred. ErrorId: {ErrorId}", errorId);

                int statusCode = ex switch
                {
                    ArgumentException => StatusCodes.Status400BadRequest,
                    KeyNotFoundException => StatusCodes.Status404NotFound,
                    UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
                    _ => StatusCodes.Status500InternalServerError
                };

                httpContext.Response.StatusCode = statusCode;
                httpContext.Response.ContentType = "application/json";

                var response = _environment.IsDevelopment()
                    ? new ApiExceptionError(ex.Message, ex.StackTrace!, statusCode, errorId)
                    : new ApiExceptionError("An unexpected error occurred.", null!, statusCode, errorId);

                var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                var json = JsonSerializer.Serialize(response, options);

                await httpContext.Response.WriteAsync(json);
            }
        }
    }

    public static class ExceptionMiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionMiddleware(this IApplicationBuilder builder)
            => builder.UseMiddleware<ExceptionMiddleWare>();
    }
}
