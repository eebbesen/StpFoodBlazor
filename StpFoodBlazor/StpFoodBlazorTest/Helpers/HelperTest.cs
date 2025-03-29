using System;
using System.Text.RegularExpressions;
using StpFoodBlazor.Helpers;

namespace StpFoodBlazorTest.Helpers
{
    public partial class HelperTest
    {
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
    }
}
