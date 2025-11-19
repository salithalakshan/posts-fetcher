using Fetcher.Api.Common.Api;
using Fetcher.Api.Common.Exceptions;
using System.Net;
using System.Text.Json;

namespace Fetcher.Api.Common.Middlewares
{
    public sealed class ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger
        )
    {
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch(AppException ex)
            {
                logger.LogError(ex, "Application exception occurred");
                await WriteErrorResponseAsync(context, ex.StatusCode, ex.Message);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An unhandled exception occurred");
                await WriteErrorResponseAsync(context, HttpStatusCode.InternalServerError, "An unexpected error occurred.");
            }
        }

        private static async Task WriteErrorResponseAsync(HttpContext context, HttpStatusCode statusCode, string message)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var response = ApiResponse<object>.Error(
                statusCode.ToString(),
                message,
                context.TraceIdentifier
                );

            var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            await context.Response.WriteAsync(jsonResponse);
        }
    }
}
