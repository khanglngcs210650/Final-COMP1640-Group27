using System.Net;
using System.Text.Json;

namespace API.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var errorId = Guid.NewGuid();
            _logger.LogError($"----------------Exception occured \n" +
                             $"+Exception Id: {errorId} \n" +
                             $"+Exception Message: {exception.Message} \n" +
                             $"+Exception Source: {exception.Source} \n" +
                             $"+Exception Type: {exception.GetType()} \n" +
                             $"+Stacktrace: \n {exception.StackTrace} \n");

            var code = HttpStatusCode.InternalServerError;
            var result = JsonSerializer.Serialize(new { error = "An unexpected error occurred! We are trying to resolve this" });
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;
            return context.Response.WriteAsync(result);
        }
    }
}
