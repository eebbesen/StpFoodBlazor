using System.Reflection;
using System.Text;

namespace StpFoodBlazor.Helpers
{
    public class HolidayDisplay
    {
        public string DateText { get; set; } = string.Empty;
        public string HolidayNames { get; set; } = string.Empty;
    }

    public static class Helper
    {
        private static readonly string DATE_DASH_FORMAT = "MM-dd";
        private static readonly string DATE_SLASH_FORMAT = "MM/dd";
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

        public static HolidayDisplay[] BuildHolidayStrings(Dictionary<string, string[]> holidays)
        {
            if (holidays == null || holidays.Count == 0)
            {
                return [];
            }

            var holidayDisplays = new List<HolidayDisplay>();

            var sortedHolidays = holidays
                .GroupBy(h => h.Key)
                .OrderBy(h => h.Key)
                .ToArray();

            foreach (var holiday in sortedHolidays)
            {
                var isToday = DateTime.Parse(holiday.Key).ToString(DATE_DASH_FORMAT)
                    .Equals(DateTime.Now.ToString(DATE_DASH_FORMAT));

                var holidayDisplay = new HolidayDisplay
                {
                    DateText = isToday ? "Today" : DateTime.Parse(holiday.Key).ToString(DATE_SLASH_FORMAT),
                    HolidayNames = string.Join(", ", holiday.First().Value)
                };

                holidayDisplays.Add(holidayDisplay);
            }

            return [.. holidayDisplays];
        }
    }
}
