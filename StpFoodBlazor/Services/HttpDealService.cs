using Microsoft.Extensions.Caching.Memory;
using StpFoodBlazor.Helpers;
using StpFoodBlazor.Models;

namespace StpFoodBlazor.Services
{
    public class HttpDealService(
        IMemoryCache memoryCache,
        HttpClient httpClient,
        ILogger<HttpDealService> logger,
        IConfiguration config) : IDealService
    {
        private readonly TimeSpan _expiry = TimeSpan.FromMinutes(config.GetValue<int>("CacheDuration:DealsMinutes", 200));
        private readonly string _url = Helper.GetUrl("Deals");

        public async Task<DealEvent[]> GetDealsAsync() =>
            await memoryCache.GetOrFetchAsync(
                CacheKeys.Deals,
                () => httpClient.GetFromJsonAsync<DealEvent[]>(_url),
                _expiry,
                logger,
                []);
    }
}
