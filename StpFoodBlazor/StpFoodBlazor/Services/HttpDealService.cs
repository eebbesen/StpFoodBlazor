using Microsoft.Extensions.Caching.Memory;
using StpFoodBlazor.Helpers;
using StpFoodBlazor.Models;

namespace StpFoodBlazor.Services
{
    public class HttpDealService(IMemoryCache memoryCache, HttpClient httpClient, ILogger<HttpDealService> logger) : IDealService
    {
        private static readonly string Url = Helper.GetUrl("Deals");
        private readonly IMemoryCache _cache = memoryCache;
        private readonly ILogger<HttpDealService> _logger = logger;
        private readonly static string CACHE_KEY = "deals";

        public async Task<DealEvent[]> GetDealsAsync()
        {
            DealEvent[]? result;

            if (_cache.TryGetValue(CACHE_KEY, out DealEvent[]? cachedDeals))
            {
                result = cachedDeals;
                _logger.LogInformation("retrieved deals from cache using key: {CacheKey}", CACHE_KEY);
            } 
            else
            {
                result = await httpClient.GetFromJsonAsync<DealEvent[]>(Url);
                _cache.Set(CACHE_KEY, result, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(200)
                });
                _logger.LogInformation("retrieved deals via HTTP call: {Url}", Url);
            }
            
            return result ?? [];
        }
    }
}
