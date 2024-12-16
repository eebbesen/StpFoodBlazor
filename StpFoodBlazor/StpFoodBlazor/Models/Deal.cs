using System.Text.Json.Serialization;

namespace StpFoodBlazor.Models
{
    public class DealEvent
    {
        public string? Deal { get; set; }
        public string? Day { get; set; }
        public string? Name { get; set; }
        public string? Alcohol { get; set; }
        public string? URL { get; set; }

        [JsonPropertyName("Happy Hour")]
        public string? HappyHour { get; set; }
        [JsonPropertyName("Start Date")]
        public string? Start { get; set; }
        [JsonPropertyName("End Date")]
        public string? End { get; set; }
    }
}
