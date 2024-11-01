using StpFoodBlazor.Models;

namespace StpFoodBlazor.Helpers
{
    public static class DealSorter
    {
        public static List<DealEvent> SortDealEventsByName(List<DealEvent> DealEvents)
        {
            DealEvents.Sort((x, y) => x.Name.CompareTo(y.Name));
            return DealEvents;
        }
    }
}
