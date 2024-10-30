using StpFoodBlazor.Services;
using System;

namespace StpFoodBlazorTest.Services
{
    public class TestTimeService(DateTime dateTime) : ITimeService
    {
        public string GetDayOfWeek()
        {
            return dateTime.DayOfWeek.ToString();
        }
    }
}
