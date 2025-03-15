using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;

namespace StpFoodBlazorTest.Integration
{
    public class DealsTest : IntegrationTest
    {
        ~DealsTest()
        {
            Dispose(disposing: false);
        }

        private void AssertCommon()
        {
            Driver.FindElement(By.Id("deals_table_header"));
            SelectElement select = new SelectElement(Driver.FindElement(By.Id("day-of-week-select")));
            Assert.Equal(DateTime.Now.DayOfWeek.ToString(), select.SelectedOption.Text);
            Driver.FindElement(By.Id("happy-hour-checkbox"));
            Assert.Equal("Gift Cards", Driver.FindElement(By.Id("giftcard-nav")).Text);
            Assert.Equal("About", Driver.FindElement(By.Id("about-nav")).Text);
        }

        [Fact]
        public void DealsTableBodyPlaceholder()
        {
            try
            {
                Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(1);
                Driver.Navigate().GoToUrl(BASE_URL);
                AssertCommon();
                Driver.FindElement(By.Id("deals_table_body_placeholder"));
            }
            catch (Exception)
            {
                SeleniumArtifactts("DealsTableBodyPlaceholder");
                throw;
            }
        }

        [Fact]
        public void DealsTableBodyLoads()
        {
            try
            {
                Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
                Driver.Navigate().GoToUrl(BASE_URL);
                Assert.True(3 < Driver.FindElement(By.Id("deals_table_body")).FindElements(By.ClassName("row")).Count);
            }
            catch (Exception)
            {
                SeleniumArtifactts("DealsTableBodyLoads");
                throw;
            }
        }
    }
}
