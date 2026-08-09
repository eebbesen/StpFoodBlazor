namespace StpFoodBlazor.Middleware
{
    public class PageAccessLoggingMiddleware(RequestDelegate next, ILogger<PageAccessLoggingMiddleware> logger)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value ?? string.Empty;
<<<<<<< HEAD
            var shouldLogPageVisit = !path.StartsWith("/_", StringComparison.OrdinalIgnoreCase)
                && !path.StartsWith("/api", StringComparison.OrdinalIgnoreCase)
                && !System.IO.Path.HasExtension(path);

            var stopwatch = shouldLogPageVisit ? System.Diagnostics.Stopwatch.StartNew() : null;

            await next(context);

            if (shouldLogPageVisit)
=======
            if (!path.StartsWith("/_") && !path.StartsWith("/api") && !System.IO.Path.HasExtension(path))
>>>>>>> main
            {
                var ip = context.Request.Headers["X-Forwarded-For"].FirstOrDefault()?.Split(',')[0].Trim()
                    ?? context.Connection.RemoteIpAddress?.ToString()
                    ?? "unknown";
<<<<<<< HEAD

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
=======
var userAgent = context.Request.Headers["User-Agent"].ToString();
var referrer = context.Request.Headers["Referer"].ToString();
if (Uri.TryCreate(referrer, UriKind.Absolute, out var refUri))
    referrer = refUri.GetLeftPart(UriPartial.Path);

var start = TimeProvider.System.GetTimestamp();
                await next(context);
                var elapsed = TimeProvider.System.GetElapsedTime(start);

                logger.LogInformation(
                    "Page accessed: IP={IP} Path={Path} Status={Status} Duration={Duration}ms UserAgent={UserAgent} Referrer={Referrer}",
                    ip, path, context.Response.StatusCode, (int)elapsed.TotalMilliseconds, userAgent, referrer);
            }
            else
            {
                await next(context);
>>>>>>> main
            }
        }
    }
}
