using StpFoodBlazor.Models;
using StpFoodBlazor.Services;
using System.Globalization;

namespace StpFoodBlazor.Helpers {
    public class GiftCardFilter {
        private readonly ITimeService _timeService;

        public GiftCardFilter(ITimeService timeService) {
            _timeService = timeService;
        }

        public GiftCard[]? GiftCards { get; set; }

        public GiftCard[] Filter() {
            if (GiftCards == null) {
                return [];
            }
            return FilterByDates(GiftCards);
        }

        private static DateTime convertStringToDate(string date) {
            return DateTime.Parse(date, CultureInfo.InvariantCulture);
        }

        private GiftCard[] FilterByDates(GiftCard[] giftcards) {
            return giftcards.Where(giftcard =>
                (string.IsNullOrWhiteSpace(giftcard.Start) || convertStringToDate(giftcard.Start).Date <= _timeService.GetCurrentDate().Date) &&
                (string.IsNullOrWhiteSpace(giftcard.End) || convertStringToDate(giftcard.End).Date >= _timeService.GetCurrentDate().Date)
            ).ToArray();
        }
    }
}
