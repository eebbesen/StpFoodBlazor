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

        public static string[] BuildHolidayStrings(Dictionary<string, string[]> holidays)
        {
            if (holidays == null || holidays.Count == 0)
            {
                return [];
            }

            var holidayStrings = new List<string>();

            var sortedHolidays = holidays
                .GroupBy(h => h.Key)
                .OrderBy(h => h.Key)
                .ToArray();

            foreach (var holiday in sortedHolidays)
            {
                var holidayString = new StringBuilder();
                holidayString.Append(
                    DateTime.Parse(holiday.Key).ToString("MM-dd")
                    .Equals(DateTime.Now.ToString("MM-dd"))
                        ? "Today: "
                        : DateTime.Parse(holiday.Key).ToString("MM-dd") + ": ");
                holidayString.Append(holiday.First().Value[0]);
                holidayStrings.Add(holidayString.ToString());
            }

            return holidayStrings.ToArray();
        }
    }
}
