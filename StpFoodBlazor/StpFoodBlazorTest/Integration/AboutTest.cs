using OpenQA.Selenium;
using System;

namespace StpFoodBlazorTest.Integration
{
    public class AboutTest : IntegrationTest
    {
        private static string BaseUrl()
        {
            return $"{BASE_URL}/about";
        }

        ~AboutTest()
        {
            Dispose(disposing: false);
        }

        private void AssertCommon()
        {
            Assert.Equal("Deals", Driver.FindElement(By.Id("root-nav")).Text);
            Assert.Equal("flex-fill", Driver.FindElement(By.Id("root-nav")).GetAttribute("class"));
            Assert.Equal("Gift Cards", Driver.FindElement(By.Id("giftcard-nav")).Text);
            Assert.Equal("justify-content-end", Driver.FindElement(By.Id("giftcard-nav")).GetAttribute("class"));
            Assert.Equal("d-none", Driver.FindElement(By.Id("about-nav")).GetAttribute("class"));
            Assert.Contains("deals compiled by", Driver.FindElement(By.Id("about-content")).Text);
            Assert.Contains("version: ", Driver.FindElement(By.Id("about-content")).Text);
        }

        [Fact]
        public void About()
        {
            try
            {   
                Driver.Navigate().GoToUrl(BaseUrl());
                
                AssertCommon();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                SeleniumArtifacts("AboutPage");
                throw;
            }
        }
    }
}
