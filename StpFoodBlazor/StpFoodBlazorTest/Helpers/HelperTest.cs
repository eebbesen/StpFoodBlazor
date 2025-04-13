using System;
using System.Text.RegularExpressions;
using StpFoodBlazor.Helpers;
using StpFoodBlazor.Models;

namespace StpFoodBlazorTest.Helpers
{
    public partial class HelperTest
    {
        private Holiday[] holidays = new[]
            {
                new Holiday { Day = "2025-04-13", Text = "Make Lunch Count Day" },
                new Holiday { Day = "2025-04-13", Text = "National Peach Cobbler Day" }
            };

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void GetUrl_WithNullTabName_ThrowsArgumentNullException(string tabName)
        {
            var exception = Assert.Throws<ArgumentException>(() => Helper.GetUrl(tabName));
            Assert.Contains("tabName cannot be empty.", exception.Message);
        }

        [Theory]
        [InlineData("Deals")]
        [InlineData("GiftCards")]
        public void GetUrl_WithPredefinedTabs_ReturnsCorrectUrls(string tabName)
        {
            string sheetId = Environment.GetEnvironmentVariable("ASPNETCORE_APPCONFIG__SHEETID") ?? string.Empty;
            string sheetsUrl = Environment.GetEnvironmentVariable("ASPNETCORE_APPCONFIG__SHEETSURL") ?? string.Empty;
            string expectedUrl = $"{sheetsUrl}/?sheet_id={sheetId}&tab_name={tabName}";

            string result = Helper.GetUrl(tabName);

            Assert.Equal(expectedUrl, result);
        }

        [Fact]
        public void GetVersion()
        {
            string version = Helper.GetVersion();
            var match = MyRegex().Match(version);
            Assert.True(match.Success, $"Version '{version}' could not be parsed into parts");
        }

        [GeneratedRegex(@"^(\d+\.\d+\.\d+)\+([a-f0-9]{40})$")]
        private static partial Regex MyRegex();

        [Fact]
        public void BuildHolidayString_WithNullOrEmptyHolidays_ReturnsEmptyString()
        {
            var result = Helper.BuildHolidayString(null);
            Assert.Equal(string.Empty, result);

            result = Helper.BuildHolidayString([]);
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void BuildHolidayString_WithHoliday()
        {
            var result = Helper.BuildHolidayString([holidays[0]]);
            Assert.Equal("Today: Make Lunch Count Day", result);
        }

        [Fact]
        public void BuildHolidayString_WithHolidays()
        {
            var result = Helper.BuildHolidayString(holidays);

            Assert.Equal("Today: Make Lunch Count Day, National Peach Cobbler Day", result);
        }

        [Fact]
        public void BuildHolidayString_WithHolidaysAndLabel()
        {
            var result = Helper.BuildHolidayString(holidays, "Tomorrow");

            Assert.Equal("Tomorrow: Make Lunch Count Day, National Peach Cobbler Day", result);
        }
    }
}
