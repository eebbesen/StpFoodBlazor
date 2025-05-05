using StpFoodBlazor.Models;

namespace StpFoodBlazor.Helpers
{
    public class DealFilter
    {
        public DealEvent[]? Deals { get; set; }
        public string? Day { get; set; }
        public string? Name { get; set; }
        public bool? AlcoholOnly { get; set; }
        public bool? HappyHour { get; set; }
        public bool? Inclusive { get; set; }
        public bool StartInfinity { get; set; } = false;
        public bool EndInfinity { get; set; } = false;

        public DealEvent[] Filter()
        {
            if (Deals == null)
            {
                return [];
            }

            DealEvent[] filteredDeals = Deals;

            filteredDeals = FilterByEndAndStartDates(filteredDeals, StartInfinity, EndInfinity);

            if (!string.IsNullOrWhiteSpace(Day))
            {
                filteredDeals = FilterByDay(filteredDeals, Day);
            }

            if (!string.IsNullOrWhiteSpace(Name))
            {
                filteredDeals = FilterByName(filteredDeals, Name);
            }

            if (AlcoholOnly.HasValue && AlcoholOnly.Value)
            {
                filteredDeals = FilterByAlcohol(filteredDeals, AlcoholOnly.Value);
            }

            if (HappyHour.HasValue)
            {
                filteredDeals = FilterByHappyHour(filteredDeals, HappyHour.Value, Inclusive ?? true);
            }

            return filteredDeals;
        }

        private static DealEvent[] FilterByDay(DealEvent[] deals, String day)
        {
            return [.. deals.Where(deal =>
               !string.IsNullOrWhiteSpace(deal.Day) &&
                deal.Day.Equals(day, StringComparison.OrdinalIgnoreCase)
            )];
        }

        private static DealEvent[] FilterByName(DealEvent[] deals, String name)
        {
            return [.. deals.Where(deal => deal.Name == name)];
        }

        private static DealEvent[] FilterByAlcohol(DealEvent[] deals, Boolean alcoholOnly)
        {
            if (alcoholOnly)
            {
                return [.. deals.Where(deal => !string.IsNullOrWhiteSpace(deal.Alcohol))];
            }

            return [.. deals.Where(deal => string.IsNullOrWhiteSpace(deal.Alcohol))];
        }

        // Need to convert column header with space to model attribute without one
        private static DealEvent[] FilterByHappyHour(DealEvent[] deals, Boolean happyHour, Boolean inclusive)
        {
            if (happyHour)
            {
                if (inclusive)
                {
                    return deals;
                }
                return [.. deals.Where(deal =>
                    deal.HappyHour != null && deal.HappyHour.Trim() != "")];
            }

            return [.. deals.Where(deal =>
                    deal.HappyHour == null || deal.HappyHour.Trim() == "")];
        }

        private static DealEvent[] FilterByEndAndStartDates(
            DealEvent[] deals,
            Boolean startInfinity = false,
            Boolean endInfinity = false)
        {
            return [.. deals.Where(deal =>
                (endInfinity || string.IsNullOrEmpty(deal.End) || DateTime.Parse(deal.End) >= DateTime.Now.Date) &&
                (startInfinity || string.IsNullOrEmpty(deal.Start) || DateTime.Parse(deal.Start) <= DateTime.Now.Date)
            )];
        }
    }
}
