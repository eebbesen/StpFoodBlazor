using System;
using StpFoodBlazor.Helpers;
using StpFoodBlazorTest.Services;
using StpFoodBlazor.Models;
using StpFoodBlazorTest.Fixtures;

namespace StpFoodBlazorTest.Helpers
{
    public class GiftCardFilterTest
    {
        private readonly TestTimeService timeService = new();

        public GiftCardFilterTest()
        {
            timeService.CurrentDate = new DateTime(2024, 12, 10, 0, 0, 0, DateTimeKind.Utc);
        }

        [Fact]
        public void Filter_FiltersCorrectly()
        {
            var expected = new GiftCard[]
            {
                GiftCardFixtures.wildBills,
                GiftCardFixtures.byLakeElmoInn,
                GiftCardFixtures.wildBills2,
                GiftCardFixtures.kincaids
            };

            GiftCardFilter GiftCardFilter = new(timeService)
            {
                GiftCards = GiftCardFixtures.allGiftCards
            };

            var filteredGiftCards = GiftCardFilter.Filter();

            for (int i = 0; i < expected.Length; i++)
            {
                Assert.Equal(expected[i].Name, filteredGiftCards[i].Name);
            }
        }

        [Fact]
        public void Filter_FiltersNotStarted()
        {
            var expected = new GiftCard[]{
                GiftCardFixtures.wildBills,
                GiftCardFixtures.wildBills2,
                GiftCardFixtures.kincaids
            };

            GiftCardFilter GiftCardFilter = new(timeService)
            {
                GiftCards = GiftCardFixtures.allGiftCards
            };

            timeService.CurrentDate = new DateTime(2025, 12, 10, 0, 0, 0, DateTimeKind.Utc);

            var filteredGiftCards = GiftCardFilter.Filter();

            for (int i = 0; i < expected.Length; i++)
            {
                Assert.Equal(expected[i].Deal, filteredGiftCards[i].Deal);
            }
        }

        [Fact]
        public void Filter_FiltersEnded()
        {
            var expected = new GiftCard[]
            {
                GiftCardFixtures.byLakeElmoInn
            };

            GiftCardFilter GiftCardFilter = new(timeService)
            {
                GiftCards =
            [
                GiftCardFixtures.urbanWok,
                GiftCardFixtures.byLakeElmoInn
            ]
            };

            timeService.CurrentDate = new DateTime(2024, 12, 10, 0, 0, 0, DateTimeKind.Utc);

            var filteredGiftCards = GiftCardFilter.Filter();

            for (int i = 0; i < expected.Length; i++)
            {
                Assert.Equal(expected[i].Deal, filteredGiftCards[i].Deal);
            }
        }

        [Fact]
        public void Filter_EmptyArray()
        {
            GiftCardFilter GiftCardFilter = new(timeService)
            {
                GiftCards = []
            };

            var filteredGiftCards = GiftCardFilter.Filter();

            Assert.Empty(filteredGiftCards);
        }
    }
}
