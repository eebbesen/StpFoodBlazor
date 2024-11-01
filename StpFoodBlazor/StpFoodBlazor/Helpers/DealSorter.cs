using StpFoodBlazor.Models;

namespace StpFoodBlazor.Helpers
{
    public static class DealSorter
    {
        public static DealEvent[] SortDealEventsByName(DealEvent[]? DealEvents)
        {
            if (DealEvents == null)
            {
                return [];
            }

            Array.Sort(DealEvents, static (x, y) =>
                string.Compare(x.Name, y.Name, StringComparison.Ordinal));
            return [.. DealEvents];
        }
    }
}
