namespace StpFoodBlazor.Services
{
    public class DelayedGiftCardService(
        IGiftCardService inner,
        ILogger<DelayedGiftCardService> logger,
        IHostEnvironment environment) : IGiftCardService
    {
        public async Task<Models.GiftCard[]> GetGiftCardsAsync()
        {
            logger.LogError("{Env} environment detected, simulating delay.", environment.EnvironmentName);
            await Task.Delay(1000);
            return await inner.GetGiftCardsAsync();
        }
    }
}
