using StpFoodBlazor.Services;
using System.Text.Json;
using System.IO;
using System.Threading.Tasks;
using System;
using System.Linq;
using System.Collections.Generic;

namespace StpFoodBlazorTest.Services
{
    public class TestHolidayService : IHolidayService
    {
        private static readonly string HOLIDAY_FIXTURES_PATH =
            Path.Combine(Directory.GetCurrentDirectory(), "fixtures", "holidays.json");
        public Boolean LongRunning { get; set; } = false;
        public Boolean NoRecords { get; set; } = false;
        public Dictionary<string, string[]> Data { get; set; } = [];

        public async Task<Dictionary<string, string[]>> GetTodaysHolidaysAsync()
        {
            if (NoRecords)
            {
                return [];
            }

            if (LongRunning)
            {
                await Task.Delay(7000);
            }

            if (Data.Count > 0)
            {
                return Data;
            }

            if (File.Exists(HOLIDAY_FIXTURES_PATH))
            {
                string jsonContent = await File.ReadAllTextAsync(HOLIDAY_FIXTURES_PATH);
                var Holidays = JsonSerializer.Deserialize<Dictionary<string, string[]>>(jsonContent) ??
                    throw new InvalidOperationException("Deserialization resulted in a null value.");
                return Holidays;
            }
            else
            {
                throw new FileNotFoundException($"The file at {HOLIDAY_FIXTURES_PATH} was not found.");
            }
        }

        public async Task<Dictionary<string, string[]>> GetHolidaysRangeAsync(string startDate, string endDate)
        {
            if (NoRecords)
            {
                return [];
            }

            if (LongRunning)
            {
                await Task.Delay(7000);
            }

            if (Data.Count > 0)
            {
                return Data;
            }

            if (File.Exists(HOLIDAY_FIXTURES_PATH))
            {
                string jsonContent = await File.ReadAllTextAsync(HOLIDAY_FIXTURES_PATH);
                var holidays = JsonSerializer.Deserialize<Dictionary<string, string[]>>(jsonContent) ??
                    throw new InvalidOperationException("Deserialization resulted in a null value.");

                return (Dictionary<string, string[]>)holidays
                    .Where(h => h.Key != null)
                    .Where(h =>
                        string.Compare(h.Key, startDate) >= 0 &&
                        string.Compare(h.Key, endDate) <= 0
                    );
            }
            else
            {
                throw new FileNotFoundException($"The file at {HOLIDAY_FIXTURES_PATH} was not found.");
            }
        }
    }
}
