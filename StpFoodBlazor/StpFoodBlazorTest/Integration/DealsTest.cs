using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;

namespace StpFoodBlazorTest.Integration {
    public class DealsTest {
        // move this to config file
        private static readonly string BASE_URL = "http://localhost:5020";
        //move to superclass
        private readonly ChromeDriver driver;

        public DealsTest() 
        {
            var driver = new ChromeDriver();
            driver.Manage().Window.Size = new System.Drawing.Size(1920, 1200);
        }


        [Fact]
        public void Deals()
        {
            driver.Navigate().GoToUrl(BASE_URL);
            driver.FindElement(By.Id("#deals_table_header"));
            driver.FindElement(By.Id("#deals_table_body"));
        }
    }
}
