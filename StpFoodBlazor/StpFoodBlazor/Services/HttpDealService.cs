using StpFoodBlazor.Models;
using System.Net.Http.Json;

namespace StpFoodBlazor.Services
{
    public class HttpDealService(HttpClient httpClient, IConfiguration configuration) : IDealService
    {
        public Task<DealEvent[]> GetDealsAsync(bool includeAlcohol)
        {
            configuration.GetValue<string>("SheetsUrl");
            return httpClient.GetFromJsonAsync<DealEvent[]>(GetUrl());
        }

        private static String GetUrl()
        {
            string? sheetId = Environment.GetEnvironmentVariable("ASPNETCORE_AppConfig__SheetId");
            string? sheetsUrl = Environment.GetEnvironmentVariable("ASPNETCORE_AppConfig__SheetsUrl");

            return $"{sheetsUrl}/?sheet_id={sheetId}&tab_name=Deals";
        }
    }
}
