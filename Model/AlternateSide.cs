namespace EvolveCDB.Model
{
    public class AlternateSide
    {
        public required string ImgUrl { get; set; }
        public required string Name { get; set; }
        public int Attack { get; set; }
        public int Defense { get; set; }
        public required string Description { get; set; }
        public required string Trait { get; set; }
    }
}
