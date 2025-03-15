using StpFoodBlazor.Models;

namespace StpFoodBlazor.Services
{
    public class HttpGiftCardService(HttpClient httpClient, ILogger<HttpDealService> logger) : IGiftCardService
    {
        public async Task<GiftCard[]> GetGiftCardsAsync()
        {
            GiftCard[]? result = await httpClient.GetFromJsonAsync<GiftCard[]>(GetUrl());
            logger.LogInformation("retrieved giftcards: {Url}", GetUrl());
            return result ?? [];
        }

        private static String GetUrl()
        {
            string? sheetId = Environment.GetEnvironmentVariable("ASPNETCORE_APPCONFIG__SHEETID");
            string? sheetsUrl = Environment.GetEnvironmentVariable("ASPNETCORE_APPCONFIG__SHEETSURL");

            return $"{sheetsUrl}/?sheet_id={sheetId}&tab_name=giftcards";
        }
    }
}
