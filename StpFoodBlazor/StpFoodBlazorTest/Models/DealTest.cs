using StpFoodBlazor.Models;
using System;

namespace StpFoodBlazorTest.Models
{

    public class DealTest
    {

        [Theory]
        [InlineData("Test Deal")]
        [InlineData("Test Deal.")]
        [InlineData("Test Deal. ")]
        public void DisplayTextEnd_ShouldIncludeEndDateNo(string dealText)
        {
            var deal = new DealEvent
            {
                Deal = dealText,
                Start = "2023-10-01",
                End = "2023-10-04"
            };

            Assert.Equal("Test Deal. Ends 2023-10-04.", deal.DisplayTextEnd());
        }

        [Theory]
        [InlineData("Test Deal", "Test Deal")]
        [InlineData("Test Deal.", "Test Deal.")]
        [InlineData("Test Deal. ", "Test Deal. ")]
        public void DisplayTextEnd_ShouldDisplayProperlyWhenNoEndDate(string dealText, string expected)
        {
            var deal = new DealEvent
            {
                Deal = dealText,
                Start = "2023-10-01"
            };

            Assert.Equal(expected, deal.DisplayTextEnd());
        }
    }
}
