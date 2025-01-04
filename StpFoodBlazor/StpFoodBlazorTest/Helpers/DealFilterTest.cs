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
        public void ShouldReturnFilteredByDayDealMissingDay() {
            String day = "MonDAY";
            filter.Day = day;
            filter.Deals = new DealEvent[] {
                new DealEvent {
                    Name = "Pino's Pizza",
                    Day = null,
                    Deal = "No day"
                },
                new DealEvent {
                    Name = "Pino's Pizza",
                    Day = "",
                    Deal = "Empty string day"
                },
                new DealEvent {
                    Name = "Pino's Pizza",
                    Day = "   ",
                    Deal = "White space day"
                },
                new DealEvent {
                    Name = "Pino's Pizza",
                    Day = "Monday",
                    Deal = "Monday deal"
                }
            };

            DealEvent[] filteredDeals = filter.Filter();

            Assert.Single(filteredDeals);
            Assert.Equal("Monday deal", filteredDeals[0].Deal);
        }

        [Fact]
        public void ShouldReturnFilteredByDayCaseInsensitive() {
            String day = "mondAy";
            filter.Day = day;

            DealEvent[] filteredDeals = filter.Filter();

            Assert.Equal(45, filteredDeals.Length);
            Array.ForEach(filteredDeals, deal => Assert.Equal("Monday", deal.Day));
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

        [Fact]
        public void ShouldFilterDealsThatEnded() {
            filter.HappyHour = true;

            var yesterday = DateTime.Now.AddDays(-1).ToString("MM/dd/yyyy");
            var dealY = deals[0];
            dealY.End = yesterday;
            var tomorrow = DateTime.Now.AddDays(1).ToString("MM/dd/yyyy");
            var dealT = deals[1];
            dealT.End = tomorrow;
            filter.Deals = [dealY, dealT];

            DealEvent[] filteredDeals = filter.Filter();

            Assert.Single(filteredDeals);
            Assert.Equal(tomorrow, filteredDeals[0].End);
        }

        [Fact]
        public void ShouldFilterDealsThatHaveNotStarted() {
            filter.HappyHour = true;

            var today = DateTime.Now.ToString("MM/dd/yyyy");
            var dealToday = deals[0];
            dealToday.Start = today;
            var tomorrow = DateTime.Now.AddDays(1).ToString("MM/dd/yyyy");
            var dealTomorrow = deals[1];
            dealTomorrow.Start = tomorrow;
            filter.Deals = [dealToday, dealTomorrow];

            DealEvent[] filteredDeals = filter.Filter();

            Assert.Single(filteredDeals);
            Assert.Equal(today, filteredDeals[0].Start);
        }

        private static async Task<DealEvent[]> getDeals() {
            return await new TestDealService().GetDealsAsync();
        }
    }
}
