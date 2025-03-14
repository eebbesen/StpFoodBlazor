using StpFoodBlazor.Helpers;
using StpFoodBlazor.Models;

namespace StpFoodBlazor.Services
{
    public class HttpDealService(HttpClient httpClient, ILogger<HttpDealService> logger) : IDealService
    {
        public async Task<DealEvent[]> GetDealsAsync()
        {
            DealEvent[]? result = await httpClient.GetFromJsonAsync<DealEvent[]>(GetUrl());
            logger.LogInformation("retrieved deals: {Url}", GetUrl());
            return result ?? [];
        }

        private static String GetUrl()
        {
            string? sheetId = Environment.GetEnvironmentVariable("ASPNETCORE_APPCONFIG__SHEETID");
            string? sheetsUrl = Environment.GetEnvironmentVariable("ASPNETCORE_APPCONFIG__SHEETSURL");

            return $"{sheetsUrl}/?sheet_id={sheetId}&tab_name=Deals";
        }
    }
}
