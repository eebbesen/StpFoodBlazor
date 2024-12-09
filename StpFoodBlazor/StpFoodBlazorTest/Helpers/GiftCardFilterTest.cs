using System;
using StpFoodBlazor.Helpers;
using StpFoodBlazorTest.Services;
using StpFoodBlazor.Models;

namespace StpFoodBlazorTest.Helpers
{
    public class GiftCardFilterTest
    {
        private static GiftCard urbanWok = new()
        { Name = "Urban Wok",
            Terms = "Must purchase in $25 increments to get deal. Full $50 goes to recipient.",
            Deal = "Buy $25 get $25", Start = "11/22/2024", End = "12/02/2024",
            URL = "https://www.urbanwokusa.com/" };

        private static GiftCard wildBills = new()
        { Name = "Wild Bill Sports Saloon",
            Deal = "Buy $25 get $5",
            URL = "https://www.wildbillssportssaloon-stpaul.com/promos" };

        private static GiftCard wildBills2 = new()
        { Name = "Wild Bill Sports Saloon",
            Deal = "Buy $100 get $25",
            URL = "https://www.wildbillssportssaloon-stpaul.com/promos" };

        private static GiftCard byLakeElmoInn = new()
        { Name = "1881 by Lake Elmo Inn",
            Deal = "Buy $100 get $20",
            Terms = "$20 valid 01/01/2025 - 04/30/2025", End = "12/24/2024",
            URL = "https://1881bylei.com/gift-cards/" };

        private static GiftCard kincaids =  new()
        { Name = "Kincaid's",
            Deal = "Buy $100 get 2 $20 bonus cards",
            Terms = "Bonus cards good 01/01/2025 through 03/31/2025 excluding 02/14/2025",
            URL = "https://landrys-inc.cashstar.com/store/recipient?locale=en-us" };

        private static GiftCard[] sharedGiftCards = new GiftCard[]
        {
            urbanWok,
            wildBills,
            byLakeElmoInn,
            wildBills2,
            kincaids
        };
        private TestTimeService timeService = new();

        public GiftCardFilterTest()
        {
            timeService.CurrentDate = new DateTime(2024, 12, 10, 0, 0, 0, DateTimeKind.Utc);
        }

        [Fact]
        public void Filter_FiltersCorrectly()
        {
            var expected = new GiftCard[]
            {
                wildBills,
                byLakeElmoInn,
                wildBills2,
                kincaids
            };

            GiftCardFilter GiftCardFilter = new(timeService);
            GiftCardFilter.GiftCards = sharedGiftCards;

            var filteredGiftCards = GiftCardFilter.Filter();

            for (int i = 0; i < expected.Length; i++)
            {
                Assert.Equal(expected[i].Name, filteredGiftCards[i].Name);
            }
        }

        [Fact]
        public void Filter_FiltersExpired()
        {
            var expected = new GiftCard[]
            {
                byLakeElmoInn
            };

            GiftCardFilter GiftCardFilter = new(timeService);
            GiftCardFilter.GiftCards = new GiftCard[]
            {
                urbanWok,
                byLakeElmoInn
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
