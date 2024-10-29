using StpFoodBlazor.Helpers;
using StpFoodBlazor.Models;
using StpFoodBlazor.Services;
using System.Threading.Tasks;
using System;
using System.Runtime.ConstrainedExecution;
using Bunit.Extensions;

namespace StpFoodBlazorTest.Helpers {

    public class DealFilterTest {
        [Fact]
        public void ShouldHandleEmptyDeals() {
            DealFilter filter = new DealFilter();

            DealEvent[] filteredDeals = filter.filter();

            Assert.Empty(filteredDeals);
        }

        [Fact]
        public async Task ShouldReturnInputWhenNoFilterConditions() {
            DealEvent[] deals = await getDeals();
            int dealsLength = deals.Length;
            DealFilter filter = new DealFilter();
            filter.Deals = deals;

            DealEvent[] filteredDeals = filter.filter();

            Assert.Equal(324, dealsLength);
            Assert.Equal(dealsLength, filteredDeals.Length);
        }

        [Fact]
        public async Task ShouldReturnFilteredByDay() {
            String day = "Monday";
            DealEvent[] deals = await getDeals();
            DealFilter filter = new DealFilter();
            filter.Deals = deals;
            filter.Day = day;

            DealEvent[] filteredDeals = filter.filter();

            Assert.Equal(45, filteredDeals.Length);
            Array.ForEach(filteredDeals, deal => Assert.Equal(day, deal.Day));
        }

        [Fact]
        public async Task ShouldReturnFilteredByName() {
            String name = "Pino's Pizza";
            DealEvent[] deals = await getDeals();
            DealFilter filter = new DealFilter();
            filter.Deals = deals;
            filter.Name = name;

            DealEvent[] filteredDeals = filter.filter();

            Assert.Equal(5, filteredDeals.Length);
            Array.ForEach(filteredDeals, deal => Assert.Equal(name, deal.Name));
        }

        [Fact]
        public async Task ShouldReturnFilteredByNameAndDay() {
            String name = "Pino's Pizza";
            String day = "Tuesday";
            DealEvent[] deals = await getDeals();
            DealFilter filter = new DealFilter();
            filter.Deals = deals;
            filter.Name = name;
            filter.Day = day;

            DealEvent[] filteredDeals = filter.filter();

            Assert.Single(filteredDeals);
            Array.ForEach(filteredDeals, deal => {
                Assert.Equal(name, deal.Name);
                Assert.Equal(day, deal.Day);
            });
        }

        [Fact]
        public async Task ShouldReturnFilteredByAlcoholTrue() {
            DealEvent[] deals = await getDeals();
            DealFilter filter = new DealFilter();
            filter.Deals = deals;
            filter.Alcohol = true;

            DealEvent[] filteredDeals = filter.filter();

            Assert.Equal(109, filteredDeals.Length);
            Array.ForEach(filteredDeals, deal => {
                Assert.False(string.IsNullOrEmpty(deal.Alcohol));
            });
        }

        [Fact]
        public async Task ShouldReturnFilteredByAlcoholFalse() {
            DealEvent[] deals = await getDeals();
            DealFilter filter = new DealFilter();
            filter.Deals = deals;
            filter.Alcohol = false;

            DealEvent[] filteredDeals = filter.filter();

            Assert.Equal(215, filteredDeals.Length);
            Array.ForEach(filteredDeals, deal => {
                Assert.True(string.IsNullOrEmpty(deal.Alcohol));
            });
        }

        // Need to convert column header with space to model attribute without one
        // [Fact]
        // public async Task ShouldReturnFilteredByHappyHourTrue() {
        //     DealEvent[] deals = await getDeals();
        //     DealFilter filter = new DealFilter();
        //     filter.Deals = deals;
        //     filter.HappyHour = true;

        //     DealEvent[] filteredDeals = filter.filter();

        //     Assert.Equal(5, filteredDeals.Length);
        //     Array.ForEach(filteredDeals, deal => Assert.NotNull(deal.HappyHour));
        // }

        private static async Task<DealEvent[]> getDeals() {
            TestDealService dealService = new TestDealService();
            return await dealService.GetDealsAsync();
        }

    }

}
