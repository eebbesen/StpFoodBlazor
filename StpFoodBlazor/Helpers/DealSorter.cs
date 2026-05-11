using StpFoodBlazor.Models;

namespace StpFoodBlazor.Helpers
{
    public static class DealSorter
    {
        public static DealEvent[] Sort(DealEvent[] deals)
        {
            return [.. deals.OrderBy(deal =>
                Enum.TryParse<DayOfWeek>(deal.Day, ignoreCase: true, out var dow) ? dow : DayOfWeek.Sunday
            ).ThenBy(deal => deal.Name)
              .ThenBy(deal => deal.Deal)];
        }
    }
}
