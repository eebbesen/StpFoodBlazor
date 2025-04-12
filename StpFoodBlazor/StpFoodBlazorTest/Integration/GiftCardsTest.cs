using OpenQA.Selenium;
using System;

namespace StpFoodBlazorTest.Integration
{
    public class GiftCardsTest : IntegrationTest
    {
        private static string BaseUrl()
        {
            return $"{BASE_URL}/giftcards";
        }

        ~GiftCardsTest()
        {
            Dispose(disposing: false);
        }

        private void AssertCommon()
        {
            Assert.Equal("Deals", Driver.FindElement(By.Id("root-nav")).Text);
            Assert.Equal("About", Driver.FindElement(By.Id("about-nav")).Text);
        }

        [Fact]
        public void GiftCardsTableBodyPlaceholder()
        {
            try
            {
                Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(2);
                Driver.Navigate().GoToUrl(BaseUrl());
                AssertCommon();
                Driver.FindElement(By.Id("giftcards_table_body_placeholder"));
                Driver.FindElement(By.Id("giftcards_table_header"));
            }
            catch (Exception)
            {
                SeleniumArtifactts("GiftCardsTableBodyPlaceholder");
                throw;
            }
        }

        // will fail when records are there
        // [Fact]
        // public void GiftCardsNoRecordsLoads()
        // {
        //     try
        //     {
        //         Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
        //         Driver.Navigate().GoToUrl(BaseUrl());
        //         IWebElement noRecords = Driver.FindElement(By.Id("giftcards_no_records"));
        //         Assert.Contains("No active gift card deals.", noRecords.Text);
        //     }
        //     catch (Exception)
        //     {
        //         SeleniumArtifactts("GiftCardsNoRecordsLoads");
        //         throw;
        //     }
        // }
    }
}
