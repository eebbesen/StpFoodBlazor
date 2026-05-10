using Microsoft.Extensions.Caching.Memory;
using StpFoodBlazor.Helpers;
using StpFoodBlazor.Models;

namespace StpFoodBlazor.Services
{
    public class HttpGiftCardService : IGiftCardService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly HttpClient _httpClient;
        private readonly ILogger<HttpGiftCardService> _logger;
        private readonly TimeSpan _expiry;
        private readonly string _url;

        public HttpGiftCardService(IMemoryCache memoryCache, HttpClient httpClient, ILogger<HttpGiftCardService> logger, IConfiguration config)
        {
            _memoryCache = memoryCache;
            _httpClient = httpClient;
            _logger = logger;
            _expiry = TimeSpan.FromMinutes(config.GetValue<int>("CacheDuration:GiftCardsMinutes", 400));
            _url = Helper.GetUrl("giftcards");
        }

        public async Task<GiftCard[]> GetGiftCardsAsync() =>
            await _memoryCache.GetOrFetchAsync(
                CacheKeys.GiftCards,
                () => _httpClient.GetFromJsonAsync<GiftCard[]>(_url),
                _expiry,
                _logger,
                []);
    }
}
