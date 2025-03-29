using StpFoodBlazor.Helpers;
using StpFoodBlazor.Models;

namespace StpFoodBlazor.Services
{
    public class HttpGiftCardService(HttpClient httpClient, ILogger<HttpGiftCardService> logger) : IGiftCardService
    {
        private static readonly string Url = Helper.GetUrl("giftcards");
        public async Task<GiftCard[]> GetGiftCardsAsync()
        {
            GiftCard[]? result = await httpClient.GetFromJsonAsync<GiftCard[]>(Url);
            logger.LogInformation("retrieved giftcards: {Url}", Url);
            return result ?? [];
        }
    }
}
