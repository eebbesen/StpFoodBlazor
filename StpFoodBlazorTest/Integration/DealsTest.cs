using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Linq;

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
                    if (i == retryCount - 1)
                    {
                        throw;
                    }
                }
            }

            Driver.FindElement(By.Id("deals_table_header"));
            Driver.FindElement(By.Id("happy-hour-checkbox"));
            Assert.StartsWith("Gift Cards", Driver.FindElement(By.Id("giftcard-nav")).Text);
            Assert.Equal("About", Driver.FindElement(By.Id("about-nav")).Text);
        }

        [Fact]
        public void DealsTableBodyLoads()
        {
            try
            {
                Driver.Navigate().GoToUrl(BASE_URL);

                WebDriverWait wait = new(Driver, TimeSpan.FromSeconds(20));
                var body = wait.Until(d => {
                    try
                    {
                        var elem = d.FindElement(By.Id("deals_table_body"));
                        return elem.FindElements(By.ClassName("row")).Count > 3 ? elem : null;
                    }
                    catch (NoSuchElementException) { return null; }
                    catch (StaleElementReferenceException) { return null; }
                });

                Assert.NotNull(body);

                wait.Until(d => d.FindElement(By.Id("messages")).Text.Length > 10);

                var messages = Driver.FindElement(By.Id("messages"));
                Assert.True(messages.Text.Length > 10);
                Assert.Equal("Today:", messages.FindElements(By.TagName("strong"))[0].GetAttribute("innerHTML"));
            }
            catch (Exception)
            {
                SeleniumArtifacts("DealsTableBodyLoads");
                throw;
            }
        }
    }
}
