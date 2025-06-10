using System.Text.Json.Serialization;

namespace EvolveCDB.Model
{
    public class FlatCard
    {
        [JsonPropertyName("cardId")]
        public string CardId { get; set; }

        [JsonPropertyName("cardSet")]
        public string CardSet { get; set; }

        [JsonPropertyName("cardNumber")]
        public string CardNumber { get; set; }

        [JsonPropertyName("kind")]
        public string Kind { get; set; }

        [JsonPropertyName("class")]
        public string ClassType { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("imgUrl")]
        public string ImgUrl { get; set; }

        [JsonPropertyName("cost")]
        public int Cost { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("atk")]
        public int Atk { get; set; }

        [JsonPropertyName("def")]
        public int Def { get; set; }

        [JsonPropertyName("limitedToCount")]
        public int LimitedToCount { get; set; }

        [JsonPropertyName("doubleSided")]
        public bool DoubleSided { get; set; }

        [JsonPropertyName("altImgUrl")]
        public string AltImgUrl { get; set; }

        [JsonPropertyName("altName")]
        public string AltName { get; set; }

        [JsonPropertyName("altAtk")]
        public int AltAtk { get; set; }

        [JsonPropertyName("altDef")]
        public int AltDef { get; set; }

        [JsonPropertyName("altDescription")]
        public string AltDescription { get; set; }
    }
}
