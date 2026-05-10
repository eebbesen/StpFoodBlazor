namespace StpFoodBlazor.Middleware
{
    public class PageAccessLoggingMiddleware(RequestDelegate next, ILogger<PageAccessLoggingMiddleware> logger)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value ?? string.Empty;
            if (!path.StartsWith("/_") && !path.StartsWith("/api") && !System.IO.Path.HasExtension(path))
            {
                var ip = context.Request.Headers["X-Forwarded-For"].FirstOrDefault()?.Split(',')[0].Trim()
                    ?? context.Connection.RemoteIpAddress?.ToString()
                    ?? "unknown";
                logger.LogInformation("Page accessed: IP={IP} Path={Path}", ip, path);
            }
            await next(context);
        }
    }
}
