namespace StpFoodBlazor.Services
{
    public class DelayedDealService(
        IDealService inner,
        ILogger<DelayedDealService> logger,
        IHostEnvironment environment) : IDealService
    {
        public async Task<Models.DealEvent[]> GetDealsAsync()
        {
            logger.LogError("{Env} environment detected, simulating delay.", environment.EnvironmentName);
            await Task.Delay(1000);
            return await inner.GetDealsAsync();
        }
    }
}
