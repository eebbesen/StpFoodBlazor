using StpFoodBlazor.Models;
using StpFoodBlazor.Services;

namespace StpFoodBlazor.Helpers {
    public class GiftCardFilter {
        private readonly ITimeService _timeService;

        public GiftCardFilter(ITimeService timeService) {
            _timeService = timeService;
        }

        public GiftCard[]? GiftCards { get; set; }

        public GiftCard[] Filter() {
            return FilterByExpiryDate(GiftCards);
        }

        private static DateTime convertStringToDate(string date) {
            return DateTime.Parse(date);
        }

        private GiftCard[] FilterByExpiryDate(GiftCard[] giftcards) {
            return giftcards.Where(giftcard =>
                string.IsNullOrWhiteSpace(giftcard.End) ||
                convertStringToDate(giftcard.End).Date > _timeService.GetCurrentDate().Date
            ).ToArray();
        }
    }
}
