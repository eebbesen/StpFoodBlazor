using Microsoft.Extensions.Caching.Memory;
using StpFoodBlazor.Helpers;
using StpFoodBlazor.Models;

namespace StpFoodBlazor.Services
{
    public class HttpDealService : IDealService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly HttpClient _httpClient;
        private readonly ILogger<HttpDealService> _logger;
        private readonly TimeSpan _expiry;
        private readonly string _url;

        public HttpDealService(IMemoryCache memoryCache, HttpClient httpClient, ILogger<HttpDealService> logger, IConfiguration config)
        {
            _memoryCache = memoryCache;
            _httpClient = httpClient;
            _logger = logger;
            _expiry = TimeSpan.FromMinutes(config.GetValue<int>("CacheDuration:DealsMinutes", 200));
            _url = Helper.GetUrl("Deals");
        }

        public async Task<DealEvent[]> GetDealsAsync() =>
            await _memoryCache.GetOrFetchAsync(
                CacheKeys.Deals,
                () => _httpClient.GetFromJsonAsync<DealEvent[]>(_url),
                _expiry,
                _logger,
                []);
    }
}
