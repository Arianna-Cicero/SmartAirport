using System.Net;
using System.Text.Json;

namespace FlightService.Middleware
{
    public class GlobalExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

        public GlobalExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<GlobalExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            _logger.LogError(exception, "An unhandled exception occurred: {Message}", exception.Message);

            context.Response.ContentType = "application/json";

            var response = new ErrorResponse
            {
                Message = "An internal server error occurred.",
                StatusCode = (int)HttpStatusCode.InternalServerError
            };

            switch (exception)
            {
                case ArgumentNullException argumentNullException:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response.Message = $"Bad Request: {argumentNullException.Message}";
                    break;

                case ArgumentException argumentException:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response.Message = $"Bad Request: {argumentException.Message}";
                    break;

                case KeyNotFoundException keyNotFoundException:
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    response.Message = $"Not Found: {keyNotFoundException.Message}";
                    break;

                case UnauthorizedAccessException:
                    response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    response.Message = "Unauthorized access.";
                    break;

                case InvalidOperationException invalidOperationException:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response.Message = $"Invalid Operation: {invalidOperationException.Message}";
                    break;

                default:
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    response.Message = "An unexpected error occurred. Please try again later.";
                    break;
            }

            context.Response.StatusCode = response.StatusCode;

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var json = JsonSerializer.Serialize(response, options);
            await context.Response.WriteAsync(json);
        }
    }

    public class ErrorResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public string Details { get; set; }
    }
}