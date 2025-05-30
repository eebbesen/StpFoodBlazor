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
            int retryCount = 5;
            for (int i = 0; i < retryCount; i++)
            {
                try
                {
                    SelectElement select = new(Driver.FindElement(By.Id("day-of-week-select")));
                    Assert.Equal(DateTime.Now.DayOfWeek.ToString(), select.SelectedOption.Text);
                }
                catch (StaleElementReferenceException)
                {
                    if (i == retryCount)
                    {
                        throw;
                    }
                }
            }

            Driver.FindElement(By.Id("deals_table_header"));
            Driver.FindElement(By.Id("happy-hour-checkbox"));
            Assert.Equal("Gift Cards", Driver.FindElement(By.Id("giftcard-nav")).Text);
            Assert.Equal("About", Driver.FindElement(By.Id("about-nav")).Text);
        }

        [Fact]
        public void DealsTableBodyPlaceholder()
        {
            Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(2);
            try
            {
                Driver.Navigate().GoToUrl(BASE_URL);
                AssertCommon();
                Driver.FindElement(By.Id("deals_table_body_placeholder"));
            }
            catch (Exception)
            {
                SeleniumArtifacts("DealsTableBodyPlaceholder");
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

                WebDriverWait wait = new(Driver, TimeSpan.FromSeconds(5));
                wait.Until(
                    d => d.FindElement(By.Id("messages")).Text.Length > 10
                );

                var messages = Driver.FindElement(By.Id("messages"));
                Assert.True(messages.Text.Length > 10);
                Assert.StartsWith("Today: ", messages.Text);
            }
            catch (Exception)
            {
                SeleniumArtifacts("DealsTableBodyLoads");
                throw;
            }
        }
    }
}
