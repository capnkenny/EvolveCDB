namespace EvolveCDB.Model
{
    public class DeckList
    {
        public string? DeckCode { get; set; }

        /// <summary>
        /// Represents a listing of cards within the Main deck.
        /// </summary>
        public required List<Card> MainCards { get; set; }

        /// <summary>
        /// Represents a listing of cards within the Evolve deck.
        /// </summary>
        public required List<Card> EvolveCards { get; set; }

        /// <summary>
        /// Represents the Leader Card used for the deck.
        /// </summary>
        public required Card LeaderCard { get; set; }
    }
}
