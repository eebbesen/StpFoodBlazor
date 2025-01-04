using StpFoodBlazor.Models;
using StpFoodBlazor.Services;
using System.Text.Json;
using System.IO;
using System.Threading.Tasks;
using System;

namespace StpFoodBlazorTest.Services
{
    public class TestGiftCardService : IGiftCardService
    {
        private static readonly string GIFTCARD_FIXTURES_PATH = Path.Combine(Directory.GetCurrentDirectory(), "fixtures", "giftcards.json");
        public Boolean LongRunning { get; set; } = false;
        public Boolean NoRecords { get; set; } = false;

        public async Task<GiftCard[]> GetGiftCardsAsync()
        {
            if (NoRecords)
            {
                return Array.Empty<GiftCard>();
            }

            if (LongRunning)
            {
                await Task.Delay(7000);
            }

            if (File.Exists(GIFTCARD_FIXTURES_PATH))
            {
                string jsonContent = await File.ReadAllTextAsync(GIFTCARD_FIXTURES_PATH);
                var giftcards = JsonSerializer.Deserialize<GiftCard[]>(jsonContent) ?? throw new InvalidOperationException("Deserialization resulted in a null value.");
                return giftcards;
            }
            else
            {
                throw new FileNotFoundException($"The file at {GIFTCARD_FIXTURES_PATH} was not found.");
            }
        }
    }
}
