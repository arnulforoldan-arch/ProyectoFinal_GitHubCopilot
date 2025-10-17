namespace AdventureWorks.Enterprise.Api.Middleware
{
    public static class ApiKeyAuthMiddlewareExtension
    {
        public static IApplicationBuilder UseApiKeyAuth(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ApiKeyAuthMiddleware>();
        }
    }
}