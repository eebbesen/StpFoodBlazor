namespace StpFoodBlazor.Services
{
    public class CacheRefreshService(
        HttpDealService dealService,
        HttpGiftCardService giftCardService,
        ILogger<CacheRefreshService> logger) : ICacheRefreshService
    {
        public async Task RefreshAsync(string[] keys)
        {
            foreach (var key in keys)
            {
                try
                {
                    switch (key)
                    {
                        case CacheKeys.Deals:
                            await dealService.GetDealsAsync();
                            break;
                        case CacheKeys.GiftCards:
                            await giftCardService.GetGiftCardsAsync();
                            break;
                    }
                    logger.LogInformation("Cache refreshed for key '{Key}'.", key);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Cache refresh failed for key '{Key}'.", key);
                }
            }
        }
    }
}
