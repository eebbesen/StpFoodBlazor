using StpFoodBlazor.Models;

namespace StpFoodBlazor.Services
{
    public class HttpHolidayService(HttpClient httpClient, ILogger<HttpHolidayService> logger) : IHolidayService
    {
        private static readonly string URL = Environment.GetEnvironmentVariable("APPCONFIG__HOLIDAYURL")
            ?? throw new InvalidOperationException("Environment variable 'APPCONFIG__HOLIDAYURL' is not set.");
        private static readonly string TODAY_URL = URL + "/today/";
        private static readonly string RANGE_URL = URL + "/range/";
        private static readonly string START_DATE = "startDate";
        private static readonly string END_DATE = "endDate";
        public async Task<Holiday[]> GetTodaysHolidaysAsync()
        {
            string url = $"{URL}{TODAY_URL}";
            var result = await httpClient.GetFromJsonAsync<Dictionary<string,string[]>>(url);
            logger.LogInformation("retrieved Holidays: {Url}", url);
            logger.LogDebug("retrieved Holidays: {Result}", result);

            return ((IHolidayService)this).TransformJson(result);
        }

        public async Task<Holiday[]> GetHolidaysRangeAsync(string startDate, string endDate)
        {
            string url = $"{URL}{RANGE_URL}?{START_DATE}={startDate}&{END_DATE}={endDate}";
            var result = await httpClient.GetFromJsonAsync<Dictionary<string,string[]>>(url);
            logger.LogInformation("retrieved Holidays: {Url}", url);
            logger.LogDebug("retrieved Holidays: {Result}", result);

            return ((IHolidayService)this).TransformJson(result);
        }
    }
}
