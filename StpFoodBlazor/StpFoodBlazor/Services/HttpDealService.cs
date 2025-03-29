using StpFoodBlazor.Helpers;
using StpFoodBlazor.Models;

namespace StpFoodBlazor.Services
{
    public class HttpDealService(HttpClient httpClient, ILogger<HttpDealService> logger) : IDealService
    {
        private static readonly string Url = Helper.GetUrl("Deals");
        public async Task<DealEvent[]> GetDealsAsync()
        {
            DealEvent[]? result = await httpClient.GetFromJsonAsync<DealEvent[]>(Url);
            logger.LogInformation("retrieved deals: {Url}", Url);
            return result ?? [];
        }
    }
}
