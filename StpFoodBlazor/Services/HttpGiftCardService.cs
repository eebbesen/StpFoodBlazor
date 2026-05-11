using Microsoft.Extensions.Caching.Memory;
using StpFoodBlazor.Helpers;
using StpFoodBlazor.Models;

namespace StpFoodBlazor.Services
{
    public class HttpGiftCardService(
        IMemoryCache memoryCache,
        HttpClient httpClient,
        ILogger<HttpGiftCardService> logger,
        IConfiguration config) : IGiftCardService
    {
        private readonly TimeSpan _expiry = TimeSpan.FromMinutes(config.GetValue<int>("CacheDuration:GiftCardsMinutes", 400));
        private readonly string _url = Helper.GetUrl("giftcards");

        public async Task<GiftCard[]> GetGiftCardsAsync() =>
            await memoryCache.GetOrFetchAsync(
                CacheKeys.GiftCards,
                () => httpClient.GetFromJsonAsync<GiftCard[]>(_url),
                _expiry,
                logger,
                []);
    }
}
