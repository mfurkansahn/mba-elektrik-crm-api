using Microsoft.AspNetCore.Mvc;

namespace MbaCrm.Api.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private readonly IWebHostEnvironment _environment;

        public ExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlingMiddleware> logger,
            IWebHostEnvironment environment)
        {
            _next = next;
            _logger = logger;
            _environment = environment;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception exception)
            {
                await HandleExceptionAsync(
                    context,
                    exception
                );
            }
        }

        private async Task HandleExceptionAsync(
    HttpContext context,
    Exception exception)
        {
            _logger.LogError(
                exception,
                "Beklenmeyen bir sunucu hatası oluştu."
            );

            var problemDetails = new ProblemDetails
            {
                Status =
                    StatusCodes.Status500InternalServerError,

                Title =
                    "Beklenmeyen bir sunucu hatası oluştu.",

                Detail = _environment.IsDevelopment()
                    ? exception.Message
                    : "İşlem sırasında beklenmeyen bir hata oluştu.",

                Instance = context.Request.Path
            };

            problemDetails.Extensions["traceId"] =
                context.TraceIdentifier;

            context.Response.Clear();

            context.Response.StatusCode =
                problemDetails.Status.Value;

            context.Response.ContentType =
                "application/problem+json";

            await context.Response.WriteAsJsonAsync(
                problemDetails
            );
        }
    }

}