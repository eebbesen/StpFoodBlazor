using StpFoodBlazor.Models;

namespace StpFoodBlazor.Services
{
    public interface IHolidayService
    {
        public Task<Holiday[]> GetTodaysHolidaysAsync();

        public Holiday[] TransformJson(Dictionary<string, string[]> jsonData)
        {
            var holidays = new List<Holiday>();
            foreach (var kvp in jsonData)
            {
                string dateKey = kvp.Key;
                string[] holidayNames = kvp.Value;

                // For each holiday name in the array, create a Holiday object
                foreach (string holidayName in holidayNames)
                {
                    var holiday = new Holiday
                    {
                        Day = dateKey,
                        Text = holidayName
                    };

                    holidays.Add(holiday);
                }
            }
            return [.. holidays];
        }
    }
}
