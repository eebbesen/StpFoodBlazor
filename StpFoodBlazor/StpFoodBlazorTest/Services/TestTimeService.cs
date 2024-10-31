using StpFoodBlazor.Services;
using System;

namespace StpFoodBlazorTest.Services
{
    public class TestTimeService : ITimeService
    {
        public string? DayOfWeek { get; set; } = DateTime.Today.DayOfWeek.ToString();
        public string GetDayOfWeek()
        {
            return DayOfWeek ?? string.Empty;
        }
    }
}
