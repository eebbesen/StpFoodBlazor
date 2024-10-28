namespace StpFoodBlazor.Services
{
    public interface IDealService
    {
        public Task<Models.Deal[]> GetDealsAsync(bool includeAlcohol);
    }
}
