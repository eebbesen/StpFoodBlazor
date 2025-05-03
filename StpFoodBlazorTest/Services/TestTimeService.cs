using StpFoodBlazor.Services;
using System;

namespace StpFoodBlazorTest.Services
{
    public class TestTimeService : ITimeService
    {
        public string? DayOfWeek { get; set; } = DateTime.Today.DayOfWeek.ToString();
        public DateTime? CurrentDate { get; set; } = DateTime.Now.Date;

        public string GetDayOfWeek()
        {
            return DayOfWeek ?? string.Empty;
        }

        public DateTime GetCurrentDate()
        {
            return CurrentDate ?? DateTime.Now.Date;
        }
    }
}
