using System.Collections.Generic;
using StpFoodBlazor.Helpers;
using StpFoodBlazor.Models;

namespace StpFoodBlazorTest.Helpers
{
    public class DealSorterTest
    {
        [Fact]
        public void SortDealEventsByName_SortsCorrectly()
        {
            var dealEvents = new DealEvent[]
            {
                new() { Name = "Sawatdee Saint Paul" },
                new() { Name = "Asian Express" },
                new() { Name = "Afro Deli" }
            };
            var expectedOrder = new DealEvent[]
            {
                new() { Name = "Afro Deli" },
                new() { Name = "Asian Express" },
                new() { Name = "Sawatdee Saint Paul" }
            };

            var sortedDealEvents = DealSorter.SortDealEventsByName(dealEvents);

            for (int i = 0; i < expectedOrder.Length; i++)
            {
                Assert.Equal(expectedOrder[i].Name, sortedDealEvents[i].Name);
            }
        }

        [Fact]
        public void SortDealEventsByName_EmptyArray()
        {
            // Arrange
            var dealEvents = new DealEvent[]{};

            // Act
            var sortedDealEvents = DealSorter.SortDealEventsByName(dealEvents);

            // Assert
            Assert.Empty(sortedDealEvents);
        }

        [Fact]
        public void SortDealEventsByName_SingleElement()
        {
            var dealEvents = new DealEvent[]
            {
                new() { Name = "Afro Deli" }
            };

            var sortedDealEvents = DealSorter.SortDealEventsByName(dealEvents);

            Assert.Single(sortedDealEvents);
            Assert.Equal("Afro Deli", sortedDealEvents[0].Name);
        }
    }
}
