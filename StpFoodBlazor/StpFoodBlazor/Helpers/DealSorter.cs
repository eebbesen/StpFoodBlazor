using StpFoodBlazor.Models;

namespace StpFoodBlazor.Helpers
{
    public static class DealSorter
    {
        public static DealEvent[] Sort(DealEvent[] deals){
            return [.. deals.OrderBy(deal => deal.Name).ThenBy(deal => deal.Deal)];
        }
    }
}
