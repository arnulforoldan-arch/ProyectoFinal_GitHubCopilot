using Microsoft.AspNetCore.Http;

namespace AdventureWorks.Enterprise.Api.Middleware
{
    public class ApiKeyAuthMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;
        private const string API_KEY_NAME = "X-API-Key";

        public ApiKeyAuthMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.Request.Headers.TryGetValue(API_KEY_NAME, out var extractedApiKey))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsJsonAsync(new { message = "API Key no proporcionada" });
                return;
            }

            var apiKey = _configuration.GetValue<string>("ApiKey");
            
            if (!apiKey.Equals(extractedApiKey))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsJsonAsync(new { message = "API Key no válida" });
                return;
            }

            await _next(context);
        }
    }
}