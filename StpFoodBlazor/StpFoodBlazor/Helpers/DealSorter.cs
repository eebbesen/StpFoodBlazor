using StpFoodBlazor.Models;

namespace StpFoodBlazor.Helpers
{
    public static class DealSorter
    {
        public static DealEvent[] Sort(DealEvent[] deals){
            return deals.OrderBy(deal =>
                string.IsNullOrEmpty(deal.Day) ? DayOfWeek.Sunday : Enum.Parse(typeof(DayOfWeek), deal.Day)
            ).ThenBy(deal => deal.Name).
              ThenBy(deal => deal.Deal).ToArray();
        }
    }
}
