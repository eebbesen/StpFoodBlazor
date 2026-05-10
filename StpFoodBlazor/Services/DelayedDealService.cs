namespace StpFoodBlazor.Services
{
    public class DelayedDealService(
        IDealService inner,
        ILogger<DelayedDealService> logger,
        IHostEnvironment environment) : DelayedServiceBase(logger, environment), IDealService
    {
        public async Task<Models.DealEvent[]> GetDealsAsync()
        {
            await ApplyDelayAsync();
            return await inner.GetDealsAsync();
        }
    }
}
