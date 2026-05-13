using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;

namespace StpFoodBlazorTest.Integration
{
    public class UpcomingDealsTest : IntegrationTest
    {

        private static string BaseUrl()
        {
            return $"{BASE_URL}/upcoming-deals";
        }

        ~UpcomingDealsTest()
        {
            Dispose(disposing: false);
        }

        private void AssertCommon()
        {
            Driver.FindElement(By.Id("deals_table_header"));
            Assert.StartsWith("Gift Cards", Driver.FindElement(By.Id("giftcard-nav")).Text);
            Assert.Equal("About", Driver.FindElement(By.Id("about-nav")).Text);
        }

        // will only pass when there are upcoming deals
        // [Fact]
        // public void DealsTableBodyLoads()
        // {
        //     try
        //     {
        //         Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
        //         Driver.Navigate().GoToUrl(BaseUrl());
        //         Assert.True(0 < Driver.FindElement(By.Id("deals_table_body")).FindElements(By.ClassName("row")).Count);

        //         WebDriverWait wait = new(Driver, TimeSpan.FromSeconds(5));
        //         wait.Until  (
        //             d => d.FindElement(By.Id("messages")).Text.Length > 10
        //         );

        //         var messages = Driver.FindElement(By.Id("messages"));
        //         Assert.True(messages.Text.Length > 10);
        //         Assert.StartsWith("Today: ", messages.Text);
        //     }
        //     catch (Exception)
        //     {
        //         SeleniumArtifacts("UpcomingDealsTableBodyLoads");
        //         throw;
        //     }
        // }
    }
}
