using StpFoodBlazor.Models;
using StpFoodBlazor.Services;
using System.Text.Json;
using System.IO;
using System.Threading.Tasks;
using System;

namespace StpFoodBlazorTest.Services
{
    public class TestHolidayService : IHolidayService
    {
        private static readonly string HOLIDAY_FIXTURES_PATH =
            Path.Combine(Directory.GetCurrentDirectory(), "fixtures", "holidays.json");
        public Boolean LongRunning { get; set; } = false;
        public Boolean NoRecords { get; set; } = false;

        public async Task<Holiday[]> GetTodaysHolidaysAsync()
        {
            if (NoRecords)
            {
                return Array.Empty<Holiday>();
            }

            if (LongRunning)
            {
                await Task.Delay(7000);
            }

            if (File.Exists(HOLIDAY_FIXTURES_PATH))
            {
                string jsonContent = await File.ReadAllTextAsync(HOLIDAY_FIXTURES_PATH);
                var Holidays = JsonSerializer.Deserialize<Holiday[]>(jsonContent) ??
                    throw new InvalidOperationException("Deserialization resulted in a null value.");
                return Holidays;
            }
            else
            {
                throw new FileNotFoundException($"The file at {HOLIDAY_FIXTURES_PATH} was not found.");
            }
        }
    }
}
