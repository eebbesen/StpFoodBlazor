using StpFoodBlazor.Helpers;
using StpFoodBlazor.Models;

namespace StpFoodBlazor.Services
{
    public class HttpGiftCardService(HttpClient httpClient, ILogger<HttpDealService> logger) : IGiftCardService
    {
        private static readonly string URL = Helper.GetUrl("giftcards");
        public async Task<GiftCard[]> GetGiftCardsAsync()
        {
            GiftCard[]? result = await httpClient.GetFromJsonAsync<GiftCard[]>(URL);
            logger.LogInformation("retrieved giftcards: {Url}", URL);
            return result ?? [];
        }
    }
}
