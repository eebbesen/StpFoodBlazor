namespace StpFoodBlazor.Middleware
{
    public class PageAccessLoggingMiddleware(RequestDelegate next, ILogger<PageAccessLoggingMiddleware> logger)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value ?? string.Empty;
            var shouldLogPageVisit = !path.StartsWith("/_", StringComparison.OrdinalIgnoreCase)
                && !path.StartsWith("/api", StringComparison.OrdinalIgnoreCase)
                && !System.IO.Path.HasExtension(path);

            var stopwatch = shouldLogPageVisit ? System.Diagnostics.Stopwatch.StartNew() : null;

            await next(context);

            if (shouldLogPageVisit)
            {
                var ip = context.Request.Headers["X-Forwarded-For"].FirstOrDefault()?.Split(',')[0].Trim()
                    ?? context.Connection.RemoteIpAddress?.ToString()
                    ?? "unknown";

                var endpoint = context.GetEndpoint()?.DisplayName ?? "unknown";
                var referrer = context.Request.Headers.Referer.FirstOrDefault() ?? "unknown";
                var userAgent = context.Request.Headers.UserAgent.FirstOrDefault() ?? "unknown";
                stopwatch?.Stop();

                logger.LogInformation(
                    "Page visit: Method={Method} Path={Path} Endpoint={Endpoint} StatusCode={StatusCode} DurationMs={DurationMs} IP={IP} Referrer={Referrer} UserAgent={UserAgent}",
                    context.Request.Method,
                    path,
                    endpoint,
                    context.Response.StatusCode,
                    stopwatch?.ElapsedMilliseconds ?? 0,
                    ip,
                    referrer,
                    userAgent);
            }
        }
    }
}
