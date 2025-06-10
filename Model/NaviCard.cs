using System.Text.Json.Serialization;

namespace EvolveCDB.Model
{
    public class NaviCard
    {
        [JsonPropertyName("card_number")]
        public required string CardNumber { get; set; }

        [JsonPropertyName("num")]
        public int Num { get; set; }

        [JsonPropertyName("slot")]
        public int Slot { get; set; }

        [JsonPropertyName("max")]
        public int Limit { get; set; }
    }
}
