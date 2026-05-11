using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace StpFoodBlazor.Helpers
{
    public static class MemoryCacheExtensions
    {
        public static async Task<T> GetOrFetchAsync<T>(
            this IMemoryCache cache,
            string key,
            Func<Task<T?>> fetch,
            TimeSpan expiry,
            ILogger logger,
            T defaultValue) where T : class
        {
            if (cache.TryGetValue(key, out T? cached))
            {
                logger.LogInformation("Cache hit for key '{Key}'.", key);
                return cached ?? defaultValue;
            }

            try
            {
                var result = await fetch();
                if (result is not null)
                {
                    cache.Set(key, result, new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = expiry
                    });
                    logger.LogInformation("Cache populated for key '{Key}'.", key);
                }
                return result ?? defaultValue;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to fetch data for cache key '{Key}'.", key);
                return defaultValue;
            }
        }
    }
}
