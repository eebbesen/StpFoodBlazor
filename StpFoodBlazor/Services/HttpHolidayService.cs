using Microsoft.Extensions.Caching.Memory;
using StpFoodBlazor.Helpers;

namespace StpFoodBlazor.Services
{
    public class HttpHolidayService(
        IMemoryCache memoryCache,
        HttpClient httpClient,
        ILogger<HttpHolidayService> logger,
        IConfiguration config) : IHolidayService
    {
        private readonly string _baseUrl = config["APPCONFIG:HOLIDAYURL"]
            ?? throw new InvalidOperationException("Configuration 'APPCONFIG:HOLIDAYURL' is not set.");
        private readonly TimeSpan _expiry = TimeSpan.FromMinutes(config.GetValue<int>("CacheDuration:HolidaysMinutes", 400));

        public async Task<Dictionary<string, string[]>> GetTodaysHolidaysAsync() =>
            await memoryCache.GetOrFetchAsync(
                CacheKeys.Holidays,
                () => httpClient.GetFromJsonAsync<Dictionary<string, string[]>>(_baseUrl + "/today/"),
                _expiry,
                logger,
                []);

        public async Task<Dictionary<string, string[]>> GetHolidaysRangeAsync(string startDate, string endDate) =>
            await memoryCache.GetOrFetchAsync(
                CacheKeys.Holidays,
                () => httpClient.GetFromJsonAsync<Dictionary<string, string[]>>($"{_baseUrl}/range/?startDate={startDate}&endDate={endDate}"),
                _expiry,
                logger,
                []);
    }
}
