using System.Text.Json.Serialization;

namespace StpFoodBlazor.Models
{
    public class Holiday
    {
        public required string Text { get; set; }
        public required DateTime Day { get; set; }
    }
}
