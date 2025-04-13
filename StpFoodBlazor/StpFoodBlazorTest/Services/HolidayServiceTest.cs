using StpFoodBlazor.Models;
using StpFoodBlazor.Services;
using System.Collections.Generic;

namespace StpFoodBlazorTest.Services
{

    public class HolidayServiceTest
    {
        private static readonly string[] value = ["Make Lunch Count Day", "National Peach Cobbler Day"];
        private readonly IHolidayService _holidayService = new HttpHolidayService(null, null);

        [Fact]
        public void TransformData_ShouldHandleMulitpleHolidays()
        {
            Dictionary<string, string[]> jsonData = new()
            {
                { "2025-04-13", value }
            };

            Holiday[] holidays = _holidayService.TransformJson(jsonData);

            Assert.Equal(2, holidays.Length);
            Assert.Equal("2025-04-13", holidays[0].Day);
            Assert.Equal("Make Lunch Count Day", holidays[0].Text);
            Assert.Equal("2025-04-13", holidays[1].Day);
            Assert.Equal("National Peach Cobbler Day", holidays[1].Text);
        }

        [Fact]
        public void TransformData_ShouldHandleOneHoliday()
        {
            Dictionary<string, string[]> jsonData = new()
            {
                { "2025-04-13", [value[0]] }
            };

            Holiday[] holidays = _holidayService.TransformJson(jsonData);

            Assert.Single(holidays);
            Assert.Equal("2025-04-13", holidays[0].Day);
            Assert.Equal("Make Lunch Count Day", holidays[0].Text);
        }

        [Fact]
        public void TransformData_ShouldHandleNoHolidays()
        {
            Dictionary<string, string[]> jsonData = [];

            Holiday[] holidays = _holidayService.TransformJson(jsonData);

            Assert.Empty(holidays);
        }
    }
}
