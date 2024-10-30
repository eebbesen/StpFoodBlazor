namespace StpFoodBlazor.Services
{
    public interface IDealService
    {
        public Task<Models.DealEvent[]> GetDealsAsync();
        public string? DayOfWeek { get; set; }
    }
}
