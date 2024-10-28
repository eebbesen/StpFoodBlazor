namespace StpFoodBlazor.Services
{
    public interface IDealService
    {
        public Task<Models.DealEvent[]> GetDealsAsync(bool includeAlcohol);
    }
}
