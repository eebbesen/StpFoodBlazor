using Microsoft.Extensions.Caching.Memory;
using StpFoodBlazor.Helpers;

namespace StpFoodBlazor.Services
{
    public class HttpSnelrandDealService(
        IMemoryCache memoryCache,
        HttpClient httpClient,
        ILogger<HttpSnelrandDealService> logger,
        IConfiguration config) : HttpSheetDealService(
            memoryCache,
            httpClient,
            logger,
            CacheKeys.DealsSnelrand,
            Helper.GetUrl("Deals", Helper.SNELRAND_SHEET_ID_VARIABLE),
            TimeSpan.FromMinutes(config.GetValue<int>("CacheDuration:DealsSnelrandMinutes", 200))), ISnelrandDealService
    {
    }
}
