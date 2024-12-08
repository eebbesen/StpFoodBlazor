using StpFoodBlazor.Helpers;
using StpFoodBlazor.Models;
using System.Threading.Tasks;
using System;
using StpFoodBlazorTest.Services;

namespace StpFoodBlazorTest.Helpers {

    public class DealFilterTest {
        private readonly DealEvent[] deals;
        private readonly DealFilter filter;
        public DealFilterTest() {
            deals = getDeals().Result;
            filter = new DealFilter();
            filter.Deals = deals;
        }

        [Fact]
        public void ShouldHandleEmptyDeals() {
            DealEvent[] filteredDeals = new DealFilter().Filter();

            Assert.Empty(filteredDeals);
        }

        [Fact]
        public void ShouldReturnInputWhenNoFilterConditions() {
            int dealsLength = deals.Length;

            DealEvent[] filteredDeals = filter.Filter();

            Assert.Equal(299, dealsLength);
            Assert.Equal(dealsLength, filteredDeals.Length);
        }

        [Fact]
        public void ShouldReturnFilteredByDay() {
            String day = "Monday";
            filter.Day = day;

            DealEvent[] filteredDeals = filter.Filter();

            Assert.Equal(45, filteredDeals.Length);
            Array.ForEach(filteredDeals, deal => Assert.Equal(day, deal.Day));
        }

        [Fact]
        public void ShouldReturnFilteredByName() {
            String name = "Pino's Pizza";
            filter.Name = name;

            DealEvent[] filteredDeals = filter.Filter();

            Assert.Equal(5, filteredDeals.Length);
            Array.ForEach(filteredDeals, deal => Assert.Equal(name, deal.Name));
        }

        [Fact]
        public void ShouldReturnFilteredByNameAndDay() {
            String name = "Pino's Pizza";
            String day = "Tuesday";
            filter.Name = name;
            filter.Day = day;

            DealEvent[] filteredDeals = filter.Filter();

            Assert.Single(filteredDeals);
            Array.ForEach(filteredDeals, deal => {
                Assert.Equal(name, deal.Name);
                Assert.Equal(day, deal.Day);
            });
        }

        [Fact]
        public void ShouldReturnFilteredByHappyHourExclusive() {
            filter.HappyHour = true;
            filter.Inclusive = false;

            DealEvent[] filteredDeals = filter.Filter();

            Assert.Equal(191, filteredDeals.Length);
            Array.ForEach(filteredDeals, deal => Assert.NotNull(deal.HappyHour));
        }

        [Fact]
        public void ShouldReturnFilteredByHappyHourInclusive()
        {
            filter.HappyHour = true;
            filter.Inclusive = true;

            DealEvent[] filteredDeals = filter.Filter();

            Assert.Equal(299, filteredDeals.Length);
        }

        [Fact]
        public void ShouldReturnFilteredByHappyHourFalse() {
            filter.HappyHour = false;

            DealEvent[] filteredDeals = filter.Filter();

            Assert.Equal(108, filteredDeals.Length);
            Array.ForEach(filteredDeals, deal =>
                Assert.True(string.IsNullOrWhiteSpace(deal.HappyHour)));
        }

        private static async Task<DealEvent[]> getDeals() {
            return await new TestDealService().GetDealsAsync();
        }
    }
}
