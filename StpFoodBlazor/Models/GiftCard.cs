using System.Text.Json.Serialization;

namespace StpFoodBlazor.Models
{
    public class GiftCard
    {
        public string? Deal { get; set; }

        [JsonPropertyName("Start Date")]
        public string? Start { get; set; }
        [JsonPropertyName("End Date")]
        public string? End { get; set; }
        public string? Name { get; set; }
        public string? Terms { get; set; }
        public string? URL { get; set; }
    }
}
