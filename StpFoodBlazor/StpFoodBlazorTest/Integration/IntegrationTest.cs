using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.Extensions;
using System;
using System.IO;
using System.Runtime.InteropServices;

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
            return Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "..", "..", "TestArtifacts");    

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

// On Windows:
// Error Message:
//    System.FormatException : The input is not a valid Base-64 string as it contains a non-base 64 character, more than two padding characters, or an illegal character among the padding characters.
//   Stack Trace:
//      at System.Convert.FromBase64CharPtr(Char* inputPtr, Int32 inputLength)
//    at System.Convert.FromBase64String(String s)
//    at OpenQA.Selenium.EncodedFile..ctor(String base64EncodedFile)
//    at OpenQA.Selenium.Screenshot..ctor(String base64EncodedScreenshot)
//    at OpenQA.Selenium.WebDriver.GetScreenshot()
//    at OpenQA.Selenium.Support.Extensions.WebDriverExtensions.TakeScreenshot(IWebDriver driver)
//    at StpFoodBlazorTest.Integration.IntegrationTest.SeleniumScreenShot(String name) in C:\Users\User\source\repos\StpFoodBlazor\StpFoodBlazor\StpFoodBlazorTest\Integration\IntegrationTest.cs:line 59
//    at StpFoodBlazorTest.Integration.IntegrationTest.SeleniumArtifacts(String name) in C:\Users\User\source\repos\StpFoodBlazor\StpFoodBlazor\StpFoodBlazorTest\Integration\IntegrationTest.cs:line 70
//    at StpFoodBlazorTest.Integration.DealsTest.DealsTableBodyLoads() in C:\Users\User\source\repos\StpFoodBlazor\StpFoodBlazor\StpFoodBlazorTest\Integration\DealsTest.cs:line 61
//    at System.RuntimeMethodHandle.InvokeMethod(Object target, Void** arguments, Signature sig, Boolean isConstructor)
//    at System.Reflection.MethodBaseInvoker.InvokeWithNoArgs(Object obj, BindingFlags invokeAttr)
        protected void SeleniumArtifacts(String name)
        {
            if (!System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                SeleniumScreenShot(name + ".png");
            }

            SeleniumSource(name + ".html");
        }
    }
}
