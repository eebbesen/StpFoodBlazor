using StpFoodBlazor.Helpers;
using StpFoodBlazor.Models;

namespace StpFoodBlazor.Services
{
    public class HttpDealService(HttpClient httpClient, ILogger<HttpDealService> logger) : IDealService
    {
        private static readonly string URL = Helper.GetUrl("Deals");
        public async Task<DealEvent[]> GetDealsAsync()
        {
            DealEvent[]? result = await httpClient.GetFromJsonAsync<DealEvent[]>(URL);
            logger.LogInformation("retrieved deals: {Url}", URL);
            return result ?? [];
        }
    }
}
