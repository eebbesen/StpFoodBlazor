using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.Extensions;
using OpenQA.Selenium.Support.UI;
using System;
using System.IO;

namespace StpFoodBlazorTest.Integration
{
    public class DealsTest : IDisposable
    {
        private bool disposedValue;

        // move this to config file
        private static readonly string BASE_URL = "http://localhost:5020";
        //move to superclass
        private readonly ChromeDriver driver;

        public DealsTest()
        {
            ChromeOptions options = new ChromeOptions();
            options.AddArgument("--headless");
            options.AddArgument("--no-sandbox"); // Bypass OS security model
            // options.AddArgument("--disable-dev-shm-usage"); // overcome limited resource problems
            driver = new ChromeDriver(options);
            driver.Manage().Window.Size = new System.Drawing.Size(1300, 350);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    driver.Quit();
                    driver.Dispose();
                }

                disposedValue = true;
            }
        }

        ~DealsTest()
        {
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        private void AssertCommon()
        {
            driver.FindElement(By.Id("deals_table_header"));
            SelectElement select = new SelectElement(driver.FindElement(By.Id("day-of-week-select")));
            Assert.Equal(DateTime.Now.DayOfWeek.ToString(), select.SelectedOption.Text);
            driver.FindElement(By.Id("happy-hour-checkbox"));
            Assert.Equal("Gift Cards", driver.FindElement(By.Id("giftcard-nav")).Text);
            Assert.Equal("About", driver.FindElement(By.Id("about-nav")).Text);
        }

        private static string ArtifactDir()
        {
            return "{System.IO.Directory.GetCurrentDirectory()}../../../../../TestResults/";
        }

        void SeleniumScreenShot(String name)
        {
            driver.TakeScreenshot().SaveAsFile(ArtifactDir() + name);
        }

        void SeleniumSource(String name)
        {
            using (StreamWriter outputFile = new StreamWriter(Path.Combine(ArtifactDir(), name)))
            {
                outputFile.WriteLine(driver.PageSource);
            }
        }

        [Fact]
        public void DealsTableBodyPlaceholder()
        {
            try
            {
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(2);
                driver.Navigate().GoToUrl(BASE_URL);
                AssertCommon();
                driver.FindElement(By.Id("deals_table_body_placeholder"));
            }
            catch (Exception)
            {
                SeleniumScreenShot("DealsTableBodyPlaceholder.png");
                SeleniumSource("DealsTableBodyPlaceholder.html");
                throw;
            }
        }

        [Fact]
        public void DealsTableBodyLoads()
        {
            try
            {
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
                driver.Navigate().GoToUrl(BASE_URL);
                Assert.True(3 < driver.FindElement(By.Id("deals_table_body")).FindElements(By.ClassName("row")).Count);
            }
            catch (Exception)
            {
                SeleniumScreenShot("DealsTableBodyLoads.png");
                throw;
            }
        }
    }
}
