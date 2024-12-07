using StpFoodBlazor.Helpers;
using StpFoodBlazor.Models;

namespace StpFoodBlazorTest.Helpers
{
    public class GiftCardsSorterTest
    {
        private static GiftCard urbanWok = new GiftCard { Name = "Urban Wok",
            Terms = "Must purchase in $25 increments to get deal. Full $50 goes to recipient.",
            Deal = "Buy $25 get $25", Start = "11/22/2024", End = "12/02/2024",
            URL = "https://www.urbanwokusa.com/" };

        private static GiftCard wildBills = new GiftCard { Name = "Wild Bill Sports Saloon",
            Deal = "Buy $25 get $5",
            URL = "https://www.wildbillssportssaloon-stpaul.com/promos" };

        private static GiftCard wildBills2 = new GiftCard { Name = "Wild Bill Sports Saloon",
            Deal = "Buy $100 get $25",
            URL = "https://www.wildbillssportssaloon-stpaul.com/promos" };

        private static GiftCard byLakeElmoInn = new GiftCard { Name = "1881 by Lake Elmo Inn",
            Deal = "Buy $100 get $20",
            Terms = "$20 valid 01/01/2025 - 04/30/2025", End = "12/24",
            URL = "https://1881bylei.com/gift-cards/" };

        private static GiftCard kincaids =  new GiftCard { Name = "Kincaid's",
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

        [Fact]
        public void Sort_SortsCorrectly()
        {
            var expectedOrder = new GiftCard[]
            {
                byLakeElmoInn,
                kincaids,
                urbanWok,
                wildBills
            };

            var sortedDealEvents = GiftCardSorter.Sort(sharedGiftCards);

            for (int i = 0; i < expectedOrder.Length; i++)
            {
                Assert.Equal(expectedOrder[i].Name, sortedDealEvents[i].Name);
            }
        }

        [Fact]
        public void Sort_SortsCorrectlyWhenDuplicateName()
        {
            var expectedOrder = new GiftCard[]
            {
                wildBills2,
                wildBills
            };

            var sortedDealEvents = GiftCardSorter.Sort(new GiftCard[]
            {
                wildBills,
                wildBills2
            });

            for (int i = 0; i < expectedOrder.Length; i++)
            {
                Assert.Equal(expectedOrder[i].Deal, sortedDealEvents[i].Deal);
            }
        }

        [Fact]
        public void Sort_EmptyArray()
        {
            var sortedGiftCards = GiftCardSorter.Sort(new GiftCard[]{});

            Assert.Empty(sortedGiftCards);
        }

        [Fact]
        public void Sort_SingleElement()
        {
            var giftCards = new GiftCard[]
            {
                urbanWok
            };

            var sortedGiftCards = GiftCardSorter.Sort(giftCards);

            Assert.Single(sortedGiftCards);
            Assert.Equal("Urban Wok", sortedGiftCards[0].Name);
        }
    }
}
