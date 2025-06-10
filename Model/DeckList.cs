namespace EvolveCDB.Model
{
    public class DeckList
    {
        public string? DeckCode { get; set; }

        /// <summary>
        /// Represents a listing of Card Numbers and the count of copies within the deck.
        /// </summary>
        public required Dictionary<string, int> Cards { get; set; }
    }
}
