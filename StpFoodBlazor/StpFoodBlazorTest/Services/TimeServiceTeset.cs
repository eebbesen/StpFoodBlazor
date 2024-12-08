using StpFoodBlazor.Services;
using System;

namespace StpFoodBlazorTest.Services {

    public class TimeServiceTest {

        [Fact]
        public void ShouldGetDayOfWeek() {
            ITimeService timeService = new TimeService();

            string day = timeService.GetDayOfWeek();

            Assert.Equal(DateTime.Now.DayOfWeek.ToString(), day);
        }
    }
}
