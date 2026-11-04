using Microsoft.Extensions.Caching.Memory;
using StpFoodBlazor.Helpers;
using StpFoodBlazor.Models;

namespace StpFoodBlazor.Services
{
    // Reads the Deals tab of a Google Sheet and caches it under its own key.
    public abstract class HttpSheetDealService(
        IMemoryCache memoryCache,
        HttpClient httpClient,
        ILogger logger,
        string cacheKey,
        string url,
        TimeSpan expiry) : IDealService
    {
        public async Task<DealEvent[]> GetDealsAsync() =>
            await memoryCache.GetOrFetchAsync(
                cacheKey,
                () => httpClient.GetFromJsonAsync<DealEvent[]>(url),
                expiry,
                logger,
                []);
    }
}
