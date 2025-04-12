using StpFoodBlazor.Models;

namespace StpFoodBlazor.Services
{
    public class HttpHolidayService(HttpClient httpClient, ILogger<HttpHolidayService> logger) : IHolidayService
    {
        private static readonly string URL = Environment.GetEnvironmentVariable("APPCONFIG__HOLIDAYURL")
            ?? throw new InvalidOperationException("Environment variable 'APPCONFIG__HOLIDAYURL' is not set.");
        public async Task<Holiday[]> GetTodaysHolidaysAsync()
        {
            Holiday[]? result = await httpClient.GetFromJsonAsync<Holiday[]>(URL);
            logger.LogInformation("retrieved Holidays: {Url}", URL);
            return result ?? [];
        }
    }
}
