using Microsoft.Extensions.Caching.Memory;
using StpFoodBlazor.Services;
using System.Security.Cryptography;

namespace StpFoodBlazor.Endpoints
{
    internal record InvalidatedKeysResponse(string[] InvalidatedKeys);
    internal record CacheStatusResponse(Dictionary<string, bool> Keys);

    public static class CacheEndpoints
    {
        public static void Map(WebApplication app)
        {
            app.MapPost("/api/cache/invalidate", Handle);
            app.MapGet("/api/cache", HandleStatus);
        }

        internal static IResult Handle(
            HttpContext ctx,
            IMemoryCache cache,
            ILoggerFactory loggerFactory,
            IConfiguration config,
            ICacheRefreshService refreshService,
            string? key = null)
        {
            var logger = loggerFactory.CreateLogger(nameof(CacheEndpoints));
            var authResult = ValidateAuth(ctx, config, logger);
            if (authResult is not null) return authResult;

            string[] keysToInvalidate;
            if (key is null)
            {
                keysToInvalidate = CacheKeys.SheetKeys;
            }
            else if (CacheKeys.SheetKeys.Contains(key))
            {
                keysToInvalidate = [key];
            }
            else
            {
                logger.LogWarning("Cache invalidation request rejected: unknown cache key '{Key}'.", key);
                return Results.BadRequest($"Unknown cache key '{key}'. Valid keys: {string.Join(", ", CacheKeys.SheetKeys)}");
            }

            foreach (var k in keysToInvalidate)
                cache.Remove(k);

            _ = refreshService.RefreshAsync(keysToInvalidate)
                .ContinueWith(
                    t => logger.LogError(t.Exception, "Background cache refresh failed for keys: {Keys}", string.Join(", ", keysToInvalidate)),
                    TaskContinuationOptions.OnlyOnFaulted);

            logger.LogInformation("Cache invalidated and refresh triggered for keys: {Keys}", string.Join(", ", keysToInvalidate));
            return Results.Ok(new InvalidatedKeysResponse(keysToInvalidate));
        }

        internal static IResult HandleStatus(
            HttpContext ctx,
            IMemoryCache cache,
            ILoggerFactory loggerFactory,
            IConfiguration config)
        {
            var logger = loggerFactory.CreateLogger(nameof(CacheEndpoints));
            var authResult = ValidateAuth(ctx, config, logger);
            if (authResult is not null) return authResult;

            var allKeys = CacheKeys.SheetKeys.Append(CacheKeys.Holidays);
            var status = allKeys.ToDictionary(k => k, k => cache.TryGetValue(k, out _));

            return Results.Ok(new CacheStatusResponse(status));
        }

        private static IResult? ValidateAuth(HttpContext ctx, IConfiguration config, ILogger logger)
        {
            var expectedKey = config["APPCONFIG:CACHEINVALIDATIONKEY"];
            if (string.IsNullOrEmpty(expectedKey))
            {
                logger.LogError("Cache invalidation key is not configured.");
                return Results.StatusCode(503);
            }

            var providedKey = ctx.Request.Headers["X-Cache-Invalidation-Key"].FirstOrDefault() ?? string.Empty;
            var expectedBytes = System.Text.Encoding.UTF8.GetBytes(expectedKey);
            var providedBytes = System.Text.Encoding.UTF8.GetBytes(providedKey);
            if (!CryptographicOperations.FixedTimeEquals(providedBytes, expectedBytes))
            {
                logger.LogWarning("Cache request rejected: invalid key. IP={IP}", ctx.Connection.RemoteIpAddress);
                return Results.Unauthorized();
            }

            return null;
        }
    }
}
