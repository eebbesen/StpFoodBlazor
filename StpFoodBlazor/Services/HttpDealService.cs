using Microsoft.Extensions.Caching.Memory;
using StpFoodBlazor.Helpers;

namespace StpFoodBlazor.Services
{
    public class HttpDealService(
        IMemoryCache memoryCache,
        HttpClient httpClient,
        ILogger<HttpDealService> logger,
        IConfiguration config) : HttpSheetDealService(
            memoryCache,
            httpClient,
            logger,
            CacheKeys.Deals,
            Helper.GetUrl("Deals"),
            TimeSpan.FromMinutes(config.GetValue<int>("CacheDuration:DealsMinutes", 200)))
    {
    }
}
