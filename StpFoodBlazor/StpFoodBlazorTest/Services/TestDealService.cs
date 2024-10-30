using StpFoodBlazor.Models;
using StpFoodBlazor.Services;
using System.Text.Json;
using System.IO;
using System.Threading.Tasks;
using System;
using System.Net.Http;


namespace StpFoodBlazorTest.Services
{
    public class TestDealService(HttpClient? httpClient) : HttpDealService(httpClient ?? new HttpClient())
    {
        private static readonly string DEAL_FIXTURES_PATH = Path.Combine(Directory.GetCurrentDirectory(), "fixtures", "deals.json");

        public new static async Task<DealEvent[]> GetDealsAsync()
        {
            if (File.Exists(DEAL_FIXTURES_PATH))
            {
                string jsonContent = await File.ReadAllTextAsync(DEAL_FIXTURES_PATH);
                var deals = JsonSerializer.Deserialize<DealEvent[]>(jsonContent);
                if (deals == null)
                {
                    throw new InvalidOperationException("Deserialization resulted in a null value.");
                }
                return deals;
            }
            else
            {
                throw new FileNotFoundException($"The file at {DEAL_FIXTURES_PATH} was not found.");
            }
        }
    }
}
