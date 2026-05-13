using StpFoodBlazor.Models;
using StpFoodBlazor.Services;
using System.Text.Json;
using System.IO;
using System.Threading.Tasks;
using System;

namespace StpFoodBlazorTest.Services
{
    public class TestDealService : IDealService
    {
        private static readonly string DEAL_FIXTURES_PATH = Path.Combine(Directory.GetCurrentDirectory(), "fixtures", "deals.json");
        public TaskCompletionSource<DealEvent[]>? PendingResult { get; set; }
        public DealEvent[] Deals { get; set; } = [];
        public bool NoRecords { get; set; } = false;

        public async Task<DealEvent[]> GetDealsAsync()
        {
            if (PendingResult != null)
            {
                return await PendingResult.Task;
            }

            if (NoRecords)
            {
                return [];
            }

            if (Deals.Length > 0)
            {
                return Deals;
            }

            if (File.Exists(DEAL_FIXTURES_PATH))
            {
                string jsonContent = await File.ReadAllTextAsync(DEAL_FIXTURES_PATH);
                var deals = JsonSerializer.Deserialize<DealEvent[]>(jsonContent) ?? throw new InvalidOperationException("Deserialization resulted in a null value.");
                return deals;
            }
            else
            {
                throw new FileNotFoundException($"The file at {DEAL_FIXTURES_PATH} was not found.");
            }
        }
    }
}
