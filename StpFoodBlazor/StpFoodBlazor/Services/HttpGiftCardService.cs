using Microsoft.Extensions.Caching.Memory;
using StpFoodBlazor.Helpers;
using StpFoodBlazor.Models;

namespace StpFoodBlazor.Services
{
    public class HttpGiftCardService(
        IMemoryCache memoryCache,
        HttpClient httpClient,
        ILogger<HttpGiftCardService> logger,
        IHostEnvironment environment) : IGiftCardService
    {
        private static readonly string Url = Helper.GetUrl("giftcards");
        private readonly IMemoryCache _cache = memoryCache;
        private readonly ILogger<HttpGiftCardService> _logger = logger;
        private static readonly string CACHE_KEY = "giftcards";

        public async Task<GiftCard[]> GetGiftCardsAsync()
        {
            GiftCard[]? result;

            if (!environment.Equals("Test"))
            {
                Thread.Sleep(1000);
            }

            if (_cache.TryGetValue(CACHE_KEY, out GiftCard[]? cachedGiftcards))
            {
                result = cachedGiftcards;
                _logger.LogInformation("retrieved deals from cache using key: {CacheKey}", CACHE_KEY);
            }
            else
            {
                result = await httpClient.GetFromJsonAsync<GiftCard[]>(Url);
                _cache.Set(CACHE_KEY, result, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(400)
                });
                _logger.LogInformation("retrieved giftcards: {Url}", Url);
            }

            return result ?? [];
        }
    }
}
