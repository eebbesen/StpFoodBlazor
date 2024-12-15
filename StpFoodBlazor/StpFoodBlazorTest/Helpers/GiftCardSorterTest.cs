using StpFoodBlazor.Helpers;
using StpFoodBlazor.Models;
using StpFoodBlazorTest.Fixtures;

namespace StpFoodBlazorTest.Helpers
{
    public class GiftCardsSorterTest
    {

        [Fact]
        public void Sort_SortsCorrectly()
        {
            var expectedOrder = new GiftCard[]
            {
                GiftCardFixtures.byLakeElmoInn,
                GiftCardFixtures.kincaids,
                GiftCardFixtures.urbanWok,
                GiftCardFixtures.wildBills
            };

            var sortedDealEvents = GiftCardSorter.Sort(GiftCardFixtures.allGiftCards);

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
                GiftCardFixtures.wildBills2,
                GiftCardFixtures.wildBills
            };

            var sortedDealEvents = GiftCardSorter.Sort(new GiftCard[]
            {
                GiftCardFixtures.wildBills,
                GiftCardFixtures.wildBills2
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
                GiftCardFixtures.urbanWok
            };

            var sortedGiftCards = GiftCardSorter.Sort(giftCards);

            Assert.Single(sortedGiftCards);
            Assert.Equal("Urban Wok", sortedGiftCards[0].Name);
        }
    }
}
