using System.Reflection;

namespace StpFoodBlazor.Helpers
{
    public static class Helper
    {
        public static string GetVersion()
        {
            var attribute = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>();
            return attribute?.InformationalVersion ?? string.Empty;
        }

        public static String GetUrl(string tabName)
        {
            if (string.IsNullOrEmpty(tabName))
            {
                throw new ArgumentException("tabName cannot be empty.", nameof(tabName));
            }

            string? sheetId = Environment.GetEnvironmentVariable("ASPNETCORE_APPCONFIG__SHEETID");
            string? sheetsUrl = Environment.GetEnvironmentVariable("ASPNETCORE_APPCONFIG__SHEETSURL");

            return $"{sheetsUrl}/?sheet_id={sheetId}&tab_name={tabName}";
        }
    }
}
