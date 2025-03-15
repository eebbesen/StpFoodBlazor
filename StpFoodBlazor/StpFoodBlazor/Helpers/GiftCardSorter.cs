using StpFoodBlazor.Models;

namespace StpFoodBlazor.Helpers
{
    public static class GiftCardSorter
    {
        public static GiftCard[] Sort(GiftCard[] giftcards)
        {
            return [.. giftcards.OrderBy(giftcard => giftcard.Name)
                .ThenBy(giftcard => giftcard.Deal)];
        }
    }
}
