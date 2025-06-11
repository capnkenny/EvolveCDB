namespace EvolveCDB.Model
{
    public class AbbreviatedDeckList
    {
        public string? DeckCode { get; set; }

        /// <summary>
        /// Represents a listing of Card Numbers and the count of copies within the Main deck.
        /// </summary>
        public required List<AbbreviatedCard> MainCards { get; set; }

        /// <summary>
        /// Represents a listing of Card Numbers and the count of copies within the Evolve deck.
        /// </summary>
        public required List<AbbreviatedCard> EvolveCards { get; set; }

        /// <summary>
        /// Represents the Card Number for the Leader Card used for the deck.
        /// </summary>
        public required string LeaderCardId { get; set; }
    }
}
