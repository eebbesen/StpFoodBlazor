using Microsoft.Extensions.Caching.Memory;
using StpFoodBlazor.Helpers;
using StpFoodBlazor.Models;
using System.Text.Json;

namespace StpFoodBlazor.Services
{
    // Reads the Deals tab of a Google Sheet and caches it under its own key.
    public abstract class HttpSheetDealService(
        IMemoryCache memoryCache,
        HttpClient httpClient,
        ILogger logger,
        string cacheKey,
        string url,
        TimeSpan expiry) : IDealService
    {
        public async Task<DealEvent[]> GetDealsAsync() =>
            await memoryCache.GetOrFetchAsync(
                cacheKey,
                () => FetchDealsAsync(),
                expiry,
                logger,
                []);

        private async Task<DealEvent[]?> FetchDealsAsync()
        {
            using var response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            await using var stream = await response.Content.ReadAsStreamAsync();
            using var document = await JsonDocument.ParseAsync(stream);

            if (document.RootElement.ValueKind == JsonValueKind.Null)
            {
                return [];
            }

            if (document.RootElement.ValueKind != JsonValueKind.Array)
            {
                throw new JsonException(
                    $"Expected a JSON array from sheet endpoint '{url}', but received {document.RootElement.ValueKind}.");
            }

            return document.RootElement.Deserialize<DealEvent[]>(
                new JsonSerializerOptions(JsonSerializerDefaults.Web));
        }
    }
}
