using System.Net;
using System.Text.Json;

namespace DateApp.Middlewares
{
    public class ExceptionMiddlewares(RequestDelegate next, 
        ILogger<ExceptionMiddlewares> logger, 
        IHostEnvironment env)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "{message}",ex.Message);
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                var response = env.IsDevelopment()
                    ? new Errors.ApiException(context.Response.StatusCode, ex.Message, ex.StackTrace?.ToString())
                    : new Errors.ApiException(context.Response.StatusCode, "Internal Server Error", null);
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };
                var json = JsonSerializer.Serialize(response, options);
                await context.Response.WriteAsync(json);
            }
        }
    }
}
