using StpFoodBlazor.Models;
using System.Text.Json;

namespace StpFoodBlazor.Services
{
    public class HttpHolidayService(HttpClient httpClient, ILogger<HttpHolidayService> logger) : IHolidayService
    {
        private static readonly string URL = Environment.GetEnvironmentVariable("APPCONFIG__HOLIDAYURL")
            ?? throw new InvalidOperationException("Environment variable 'APPCONFIG__HOLIDAYURL' is not set.");
        public async Task<Holiday[]> GetTodaysHolidaysAsync()
        {
            var result = await httpClient.GetFromJsonAsync<Dictionary<string,string[]>>(URL);
            logger.LogInformation("retrieved Holidays: {Url}", URL);
            logger.LogDebug("retrieved Holidays: {Result}", result);

            return ((IHolidayService)this).TransformJson(result);
        }
    }
}
