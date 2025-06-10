namespace EvolveCDB.Model
{
    public class Card
    {
        public required string CardId { get; set; }
        public required string CardSet { get; set; }
        public required string CardNumber { get; set; }
        public required string Kind { get; set; }
        public required string ClassType { get; set; }
        public required string Name { get; set; }
        public required string ImgUrl { get; set; }
        public required int Cost { get; set; }
        public required string Description { get; set; }
        public required int Attack { get; set; }
        public required int Defense { get; set; }
        public required int LimitedToCount { get; set; }

        public AlternateSide? AlternateDetails { get; set; }
    }
}
