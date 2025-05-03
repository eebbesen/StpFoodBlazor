namespace StpFoodBlazor.Services
{
    public interface IGiftCardService
    {
        public Task<Models.GiftCard[]> GetGiftCardsAsync();
    }
}
