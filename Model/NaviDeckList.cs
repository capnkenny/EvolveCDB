using System.Text.Json.Serialization;

namespace EvolveCDB.Model
{
    public class NaviDeckList
    {
        [JsonPropertyName("name")]
        public required string Name { get; set; }

        [JsonPropertyName("deckCode")]
        public required string DeckCode { get; set; }

        [JsonPropertyName("mainCards")]
        public NaviCard[] MainDeck { get; set; }

        [JsonPropertyName("evolveCards")]
        public NaviCard[] EvolveDeck { get; set; }

        [JsonPropertyName("leaderCard")]
        public NaviCard LeaderCard { get; set; }
    }
}
