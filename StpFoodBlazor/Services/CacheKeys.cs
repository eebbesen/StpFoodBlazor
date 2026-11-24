namespace StpFoodBlazor.Services
{
    public static class CacheKeys
    {
        public const string Deals = "deals";
        public const string DealsSnelrand = "deals_snelrand";
        public const string GiftCards = "giftcards";
        public const string Holidays = "holidays";

        public static readonly string[] SheetKeys = [Deals, DealsSnelrand, GiftCards, Holidays];
    }
}
