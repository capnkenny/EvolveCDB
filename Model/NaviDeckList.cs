using System.Text.Json.Serialization;

namespace EvolveCDB.Model
{
    public class NaviDeckList
    {
        [JsonPropertyName("title")]
        public required string Title { get; set; }

        [JsonPropertyName("game_title_id")]
        public required int GameId { get; set; }

        [JsonPropertyName("deck_id")]
        public required string DeckId { get; set; }

        [JsonPropertyName("list")]
        public NaviCard[] MainDeck { get; set; }

        [JsonPropertyName("sub_list")]
        public NaviCard[] EvolveDeck { get; set; }

        [JsonPropertyName("p_list")]
        public NaviCard[] LeaderDeck { get; set; }
    }
}
