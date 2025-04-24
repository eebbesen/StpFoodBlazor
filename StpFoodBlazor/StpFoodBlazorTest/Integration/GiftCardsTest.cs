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
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                SeleniumArtifacts("GiftCardsTableBodyPlaceholder");
                throw;
            }
        }

        [Fact]
        public void GiftCards()
        {
            try
            {
                Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
                Driver.Navigate().GoToUrl(BaseUrl());

                AssertCommon();

                // Will fail if there are any active gift cards in the database
                // IWebElement noRecords = Driver.FindElement(By.Id("giftcards_no_records"));
                // Assert.Contains("No active gift card deals.", noRecords.Text);

                // WebDriverWait wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(3));
                // wait.Until  (
                //     d => d.FindElement(By.Id("messages")).Text.Length > 10
                // );
                // var messages = Driver.FindElement(By.Id("messages"));
                // Assert.True(messages.Text.Length > 10);
                // Assert.StartsWith("Today: ", messages.Text);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                SeleniumArtifacts("GiftCardsNoRecordsLoads");
                throw;
            }
        }
    }
}
