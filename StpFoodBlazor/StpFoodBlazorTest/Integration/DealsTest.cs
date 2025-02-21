using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;

namespace StpFoodBlazorTest.Integration {
    public class DealsTest : IDisposable
    {
        // move this to config file
        private static readonly string BASE_URL = "http://localhost:5020";
        //move to superclass
        private readonly ChromeDriver driver;

        public DealsTest()
        {
            driver = new ChromeDriver();
            driver.Manage().Window.Size = new System.Drawing.Size(1300, 350);
        }

        public void Dispose()
        {
            driver.Quit();
            driver.Dispose();
        }

        private void assertCommon()
        {
            driver.FindElement(By.Id("deals_table_header"));
            SelectElement select = new SelectElement(driver.FindElement(By.Id("day-of-week-select")));
            Assert.Equal(DateTime.Now.DayOfWeek.ToString(), select.SelectedOption.Text);
            driver.FindElement(By.Id("happy-hour-checkbox"));
            Assert.Equal("Gift Cards", driver.FindElement(By.Id("giftcard-nav")).Text);
            Assert.Equal("About", driver.FindElement(By.Id("about-nav")).Text);
        }

        [Fact]
        public void DealsPlaceholder()
        {
            driver.Navigate().GoToUrl(BASE_URL);
            assertCommon();
            driver.FindElement(By.Id("deals_table_body_placeholder"));
        }

        [Fact]
        public void DealsTableBodyLoads()
        {
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(4);
            driver.Navigate().GoToUrl(BASE_URL);
            Assert.True( 3 < driver.FindElement(By.Id("deals_table_body")).FindElements(By.ClassName("row")).Count);
        }
    }
}
