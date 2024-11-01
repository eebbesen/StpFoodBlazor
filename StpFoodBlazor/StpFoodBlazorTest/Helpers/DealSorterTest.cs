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
            // Arrange
            var dealEvents = new List<DealEvent>
            {
                new() { Name = "Banana Deal" },
                new() { Name = "Apple Deal" },
                new() { Name = "Cherry Deal" }
            };

            var expectedOrder = new List<DealEvent>
            {
                new() { Name = "Apple Deal" },
                new() { Name = "Banana Deal" },
                new() { Name = "Cherry Deal" }
            };

            // Act
            var sortedDealEvents = DealSorter.SortDealEventsByName(dealEvents);

            // Assert
            for (int i = 0; i < expectedOrder.Count; i++)
            {
                Assert.Equal(expectedOrder[i].Name, sortedDealEvents[i].Name);
            }
        }

        [Fact]
        public void SortDealEventsByName_EmptyList()
        {
            // Arrange
            var dealEvents = new List<DealEvent>();

            // Act
            var sortedDealEvents = DealSorter.SortDealEventsByName(dealEvents);

            // Assert
            Assert.Empty(sortedDealEvents);
        }

        [Fact]
        public void SortDealEventsByName_SingleElement()
        {
            // Arrange
            var dealEvents = new List<DealEvent>
            {
                new() { Name = "Single Deal" }
            };

            // Act
            var sortedDealEvents = DealSorter.SortDealEventsByName(dealEvents);

            // Assert
            Assert.Single(sortedDealEvents);
            Assert.Equal("Single Deal", sortedDealEvents[0].Name);
        }
    }
}
