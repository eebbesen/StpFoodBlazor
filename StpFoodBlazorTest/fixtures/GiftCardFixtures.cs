using StpFoodBlazor.Models;

namespace StpFoodBlazorTest.Fixtures
{
    public static class GiftCardFixtures
    {
        public static GiftCard urbanWok = new()
        { Name = "Urban Wok",
            Terms = "Must purchase in $25 increments to get deal. Full $50 goes to recipient.",
            Deal = "Buy $25 get $25", Start = "11/22/2024", End = "12/02/2024",
            URL = "https://www.urbanwokusa.com/" };

        public static GiftCard wildBills = new()
        { Name = "Wild Bill Sports Saloon",
            Deal = "Buy $25 get $5",
            URL = "https://www.wildbillssportssaloon-stpaul.com/promos" };

        public static GiftCard wildBills2 = new()
        { Name = "Wild Bill Sports Saloon",
            Deal = "Buy $100 get $25",
            URL = "https://www.wildbillssportssaloon-stpaul.com/promos" };

        public static GiftCard byLakeElmoInn = new()
        { Name = "1881 by Lake Elmo Inn",
            Deal = "Buy $100 get $20",
            Terms = "$20 valid 01/01/2025 - 04/30/2025", End = "12/24/2024",
            URL = "https://1881bylei.com/gift-cards/" };

        public static GiftCard kincaids =  new()
        { Name = "Kincaid's",
            Deal = "Buy $100 get 2 $20 bonus cards",
            Terms = "Bonus cards good 01/01/2025 through 03/31/2025 excluding 02/14/2025",
            URL = "https://landrys-inc.cashstar.com/store/recipient?locale=en-us" };

        public static GiftCard[] allGiftCards =
        [
            urbanWok,
            wildBills,
            byLakeElmoInn,
            wildBills2,
            kincaids
        ];

    }
}