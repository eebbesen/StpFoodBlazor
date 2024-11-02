using StpFoodBlazor.Helpers;
using StpFoodBlazor.Models;

namespace StpFoodBlazorTest.Helpers
{
    public class DealSorterTest
    {
        [Fact]
        public void Sort_SortsCorrectly()
        {
            var dealEvents = new DealEvent[]
            {
                new() { Name = "Sawatdee Saint Paul", Deal = "$14.32 lunch buffet (tax included)" },
                new() { Name = "Asian Express", Deal = "$12.95 combo" },
                new() { Name = "Sawatdee Saint Paul", Deal = "$5 Fried Tofu or Edamame https://www.sawatdee.com/st-paul-menu" }
            };
            var expectedOrder = new DealEvent[]
            {
                new() { Name = "Asian Express", Deal = "$12.95 combo" },
                new() { Name = "Sawatdee Saint Paul", Deal = "$14.32 lunch buffet (tax included)" },
                new() { Name = "Sawatdee Saint Paul", Deal = "$5 Fried Tofu or Edamame https://www.sawatdee.com/st-paul-menu" }
            };

            var sortedDealEvents = DealSorter.Sort(dealEvents);

            for (int i = 0; i < expectedOrder.Length; i++)
            {
                Assert.Equal(expectedOrder[i].Name, sortedDealEvents[i].Name);
            }
        }

        [Fact]
        public void Sort_EmptyArray()
        {
            // Arrange
            var dealEvents = new DealEvent[]{};

            // Act
            var sortedDealEvents = DealSorter.Sort(dealEvents);

            // Assert
            Assert.Empty(sortedDealEvents);
        }

        [Fact]
        public void Sort_SingleElement()
        {
            var dealEvents = new DealEvent[]
            {
                new() { Name = "Asian Express", Deal = "$12.95 combo" }
            };

            var sortedDealEvents = DealSorter.Sort(dealEvents);

            Assert.Single(sortedDealEvents);
            Assert.Equal("Asian Express", sortedDealEvents[0].Name);
        }
    }
}
