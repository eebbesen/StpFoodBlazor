using Microsoft.Extensions.Caching.Memory;
namespace StpFoodBlazor.Services
{
    public class HttpHolidayService(IMemoryCache memoryCache, HttpClient httpClient, ILogger<HttpHolidayService> logger) : IHolidayService
    {
        private static readonly string URL = Environment.GetEnvironmentVariable("APPCONFIG__HOLIDAYURL")
            ?? throw new InvalidOperationException("Environment variable 'APPCONFIG__HOLIDAYURL' is not set.");
        private static readonly string TODAY_URL = URL + "/today/";
        private static readonly string RANGE_URL = URL + "/range/";
        private static readonly string START_DATE = "startDate";
        private static readonly string END_DATE = "endDate";
        private readonly IMemoryCache _cache = memoryCache;
        private readonly ILogger<HttpHolidayService> _logger = logger;
        private static readonly string CACHE_KEY = "holidays";

        public async Task<Dictionary<string, string[]>> GetTodaysHolidaysAsync()
        {
            Dictionary<string, string[]> result;

            if (_cache.TryGetValue(CACHE_KEY, out Dictionary<string, string[]>? cachedHolidays))
            {
                result = cachedHolidays;
                _logger.LogInformation("retrieved holidays from cache using key: {CacheKey}", CACHE_KEY);
            }
            else
            {
                result = await httpClient.GetFromJsonAsync<Dictionary<string, string[]>>(TODAY_URL);
                _logger.LogInformation("retrieved holidays from cache using key: {CacheKey}", CACHE_KEY);
                _cache.Set(CACHE_KEY, result, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(400)
                });
            }

            return result ?? [];
        }

        public async Task<Dictionary<string, string[]>> GetHolidaysRangeAsync(string startDate, string endDate)
        {
            Dictionary<string, string[]> result;

            if (_cache.TryGetValue(CACHE_KEY, out Dictionary<string, string[]>? cachedHolidays))
            {
                result = cachedHolidays;
                _logger.LogInformation("retrieved holidays from cache using key: {CacheKey}", CACHE_KEY);
            }
            else
            {
                string url = $"{RANGE_URL}?{START_DATE}={startDate}&{END_DATE}={endDate}";
                result = await httpClient.GetFromJsonAsync<Dictionary<string, string[]>>(url);
                _logger.LogInformation("retrieved holidays from API using key: {CacheKey}", CACHE_KEY);
                _cache.Set(CACHE_KEY, result, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(400)
                });
            }

            return result ?? [];
        }
    }
}
