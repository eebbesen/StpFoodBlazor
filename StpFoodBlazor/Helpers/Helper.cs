using System.Globalization;
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
        private const string DATE_DASH_FORMAT = "MM-dd";
        private const string DATE_SLASH_FORMAT = "MM/dd";
        private static readonly string[] CommitShaVariableNames = ["RENDER_GIT_COMMIT", "GITHUB_SHA", "SOURCE_VERSION"];

        public static string GetVersion()
        {
            var attribute = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>();
            var version = attribute?.InformationalVersion ?? string.Empty;

            foreach (var variableName in CommitShaVariableNames)
            {
                var commitSha = Environment.GetEnvironmentVariable(variableName);
                if (!string.IsNullOrWhiteSpace(commitSha))
                {
                    var baseVersion = version.Split('+')[0];
                    return $"{baseVersion}+{commitSha.Trim()}";
                }
            }

            return version;
        }

        public const string DEFAULT_SHEET_ID_VARIABLE = "APPCONFIG__SHEETID";
        public const string SNELRAND_SHEET_ID_VARIABLE = "APPCONFIG__SNELRANDSHEETID";

        public static string GetUrl(string? tabName, string sheetIdVariable = DEFAULT_SHEET_ID_VARIABLE)
        {
            if (string.IsNullOrEmpty(tabName))
            {
                throw new ArgumentException("tabName cannot be empty.", nameof(tabName));
            }

            string? sheetId = Environment.GetEnvironmentVariable(sheetIdVariable);
            string? sheetsUrl = Environment.GetEnvironmentVariable("APPCONFIG__SHEETSURL");

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
                .OrderBy(h => h.Key)
                .ToArray();

            foreach (var holiday in sortedHolidays)
            {
                var parsed = DateTime.Parse(holiday.Key, CultureInfo.InvariantCulture);
                var isToday = parsed.ToString(DATE_DASH_FORMAT, CultureInfo.InvariantCulture)
                    .Equals(DateTime.Now.ToString(DATE_DASH_FORMAT, CultureInfo.InvariantCulture));

                var holidayDisplay = new HolidayDisplay
                {
                    DateText = isToday ? "Today" : parsed.ToString(DATE_SLASH_FORMAT, CultureInfo.InvariantCulture),
                    HolidayNames = string.Join(", ", holiday.Value)
                };

                holidayDisplays.Add(holidayDisplay);
            }

            return [.. holidayDisplays];
        }
    }
}
