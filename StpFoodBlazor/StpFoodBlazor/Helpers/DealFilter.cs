using StpFoodBlazor.Models;

namespace StpFoodBlazor.Helpers {
    public class DealFilter {
        public DealEvent[]? Deals { get; set; }
        public string? Day { get; set; }
        public string? Name { get; set; }
        public bool? Alcohol { get; set; }
        public bool? HappyHour { get; set; }
        public bool? Inclusive { get; set; }

        public DealEvent[] Filter() {
            if (Deals == null)
            {
                return Array.Empty<DealEvent>();
            }

            DealEvent[] filteredDeals = Deals;

            filteredDeals = FilterByEndAndStartDates(filteredDeals);

            if (!string.IsNullOrWhiteSpace(Day))
            {
                filteredDeals = FilterByDay(filteredDeals, Day);
            }

            if (!string.IsNullOrWhiteSpace(Name))
            {
                filteredDeals = FilterByName(filteredDeals, Name);
            }

            if (Alcohol.HasValue)
            {
                filteredDeals = FilterByAlcohol(filteredDeals, Alcohol.Value, Inclusive ?? true);
            }

            if (HappyHour.HasValue)
            {
                filteredDeals = FilterByHappyHour(filteredDeals, HappyHour.Value, Inclusive ?? true);
            }

            return filteredDeals;
        }

        private static DealEvent[] FilterByDay(DealEvent[] deals, String day) {
            return deals.Where(deal =>
               !string.IsNullOrWhiteSpace(deal.Day) &&
                deal.Day.Equals(day, StringComparison.OrdinalIgnoreCase)
            ).ToArray();
        }

        private static DealEvent[] FilterByName(DealEvent[] deals, String name) {
            return deals.Where(deal => deal.Name == name).ToArray();
        }

        private static DealEvent[] FilterByAlcohol(DealEvent[] deals, Boolean alcohol, Boolean inclusive) {
            if (alcohol) {
                if (inclusive)
                {
                    return deals;
                }
                return deals.Where(deal => !string.IsNullOrWhiteSpace(deal.Alcohol)).ToArray();
            }

            return deals.Where(deal => string.IsNullOrWhiteSpace(deal.Alcohol)).ToArray();
        }

        // Need to convert column header with space to model attribute without one
        private static DealEvent[] FilterByHappyHour(DealEvent[] deals, Boolean happyHour, Boolean inclusive) {
            if (happyHour) {
                if (inclusive) {
                    return deals;
                }
                return deals.Where(deal =>
                    deal.HappyHour != null && deal.HappyHour.Trim() != "").ToArray();
            }

            return deals.Where(deal =>
                    deal.HappyHour == null || deal.HappyHour.Trim() == "").ToArray();
        }

        private static DealEvent[] FilterByEndAndStartDates(DealEvent[] deals) {
            return deals.Where(deal =>
                (string.IsNullOrEmpty(deal.End) || DateTime.Parse(deal.End) >= DateTime.Now) &&
                (string.IsNullOrEmpty(deal.Start) || DateTime.Parse(deal.Start) <= DateTime.Now)
            ).ToArray();
        }
    }
}
