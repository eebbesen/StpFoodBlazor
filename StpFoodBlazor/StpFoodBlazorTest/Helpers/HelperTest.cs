using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using StpFoodBlazor.Helpers;

namespace StpFoodBlazorTest.Helpers
{
    public partial class HelperTest
    {
        private static readonly string DATE_FORMAT = "MM-dd";
        private readonly Dictionary<string, string[]> holidays = new Dictionary<string, string[]>
        {
            { DateTime.Now.ToString(DATE_FORMAT), new[] { "Make Lunch Count Day, National Peach Cobbler Day" } },
            { DateTime.Now.AddDays(1).ToString(DATE_FORMAT), new[] { "McDonald's Day, National Glazed Spiral Ham Day" } }
        };
        private static readonly string[] value = new[] { "National Burrito Day" };

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
        public void BuildHolidayString_WithNullOrEmptyHolidays_ReturnsEmptyArray()
        {
            var result = Helper.BuildHolidayStrings(null);
            Assert.Empty(result);

            result = Helper.BuildHolidayStrings([]);
            Assert.Empty(result);
        }

        [Fact]
        public void BuildHolidayString_WithHoliday()
        {
            var holiday = new Dictionary<string, string[]>
                { { DateTime.Now.ToString(DATE_FORMAT), ["National Burrito Day"] } };

            var result = Helper.BuildHolidayStrings(holiday);
            Assert.Equal("Today: National Burrito Day", result[0]);
        }

        [Fact]
        public void BuildHolidayString_WithHolidays()
        {
            var today = DateTime.Now;
            holidays.Add(today.AddDays(2).ToString(DATE_FORMAT), ["National Gyro Day"]);

            var result = Helper.BuildHolidayStrings(holidays);

            Assert.Equal("Today: Make Lunch Count Day, National Peach Cobbler Day", result[0]);
            Assert.Equal($"{today.AddDays(1).ToString(DATE_FORMAT)}: McDonald's Day, National Glazed Spiral Ham Day", result[1]);
            Assert.Equal( $"{today.AddDays(2).ToString(DATE_FORMAT)}: National Gyro Day", result[2]);
        }
    }
}
