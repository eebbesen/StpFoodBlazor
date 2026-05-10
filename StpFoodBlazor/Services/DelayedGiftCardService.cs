namespace StpFoodBlazor.Services
{
    public class DelayedGiftCardService(
        IGiftCardService inner,
        ILogger<DelayedGiftCardService> logger,
        IHostEnvironment environment) : DelayedServiceBase(logger, environment), IGiftCardService
    {
        public async Task<Models.GiftCard[]> GetGiftCardsAsync()
        {
            await ApplyDelayAsync();
            return await inner.GetGiftCardsAsync();
        }
    }
}
