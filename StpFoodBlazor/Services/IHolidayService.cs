
namespace StpFoodBlazor.Services
{
    public interface IHolidayService
    {
        public Task<Dictionary<string, string[]>> GetTodaysHolidaysAsync();
        public Task<Dictionary<string, string[]>> GetHolidaysRangeAsync(string startDate, string endDate);
    }
}
