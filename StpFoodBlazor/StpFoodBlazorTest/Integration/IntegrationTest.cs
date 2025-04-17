using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.Extensions;
using System;
using System.IO;

namespace StpFoodBlazorTest.Integration
{
    public abstract class IntegrationTest : IDisposable
    {
        private bool disposedValue;

        public static readonly string BASE_URL = Environment.GetEnvironmentVariable("ASPNETCORE_APPCONFIG__BASE_URL") ?? "http://localhost:5020";
        public required ChromeDriver Driver;

        protected IntegrationTest()
        {
            InitializeDriver();
        }

        protected void InitializeDriver()
        {
            ChromeOptions options = new ChromeOptions();
            options.AddArgument("--headless");
            options.AddArgument("--no-sandbox"); // Bypass OS security model
            options.AddArgument("--disable-dev-shm-usage"); // overcome limited resource problems
            Driver = new ChromeDriver(options);
            Driver.Manage().Window.Size = new System.Drawing.Size(1300, 350);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Driver.Quit();
                    Driver.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected static string ArtifactDir()
        {
            // return "{System.IO.Directory.GetCurrentDirectory()}../../../../../TestResults/";
            return Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "..", "TestResults");    

        }

        protected void SeleniumScreenShot(String name)
        {
            Driver.TakeScreenshot().SaveAsFile(ArtifactDir() + name);
        }

        protected void SeleniumSource(String name)
        {
            using StreamWriter outputFile = new(Path.Combine(ArtifactDir(), name));
            outputFile.WriteLine(Driver.PageSource);
        }

        protected void SeleniumArtifacts(String name)
        {
            SeleniumScreenShot(name + ".png");
            SeleniumSource(name + ".html");
        }
    }
}
