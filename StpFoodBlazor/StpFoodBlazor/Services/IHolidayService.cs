using StpFoodBlazor.Models;

namespace StpFoodBlazor.Services
{
    public interface IHolidayService
    {
        public Task<Holiday[]> GetTodaysHolidaysAsync();
    }
}
