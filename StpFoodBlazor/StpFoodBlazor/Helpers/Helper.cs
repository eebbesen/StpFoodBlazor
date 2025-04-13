using System.Reflection;
using System.Text;
using StpFoodBlazor.Models;

namespace StpFoodBlazor.Helpers
{
    public static class Helper
    {
        public static string GetVersion()
        {
            var attribute = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>();
            return attribute?.InformationalVersion ?? string.Empty;
        }

        public static string GetUrl(string tabName)
        {
            if (string.IsNullOrEmpty(tabName))
            {
                throw new ArgumentException("tabName cannot be empty.", nameof(tabName));
            }

            string? sheetId = Environment.GetEnvironmentVariable("ASPNETCORE_APPCONFIG__SHEETID");
            string? sheetsUrl = Environment.GetEnvironmentVariable("ASPNETCORE_APPCONFIG__SHEETSURL");

            return $"{sheetsUrl}/?sheet_id={sheetId}&tab_name={tabName}";
        }

        public static string BuildHolidayString(Holiday[] holidays, string day = "Today")
        {
            if (holidays == null || holidays.Length == 0)
            {
                return string.Empty;
            }

            var holidayString = new StringBuilder($"{day}: ");
            holidayString.Append(string.Join(", ", holidays.Select(h => h.Text)));

            return holidayString.ToString();
        }
    }
}
