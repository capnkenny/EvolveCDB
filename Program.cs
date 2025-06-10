using EvolveCDB.Endpoints;
using EvolveCDB.Endpoints.Extensions;
using EvolveCDB.Model;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, SourceGenerationContext.Default);
    options.SerializerOptions.PropertyNameCaseInsensitive = true;
});

var app = builder.Build();

//TODO: make this read from the GH repo
string json = File.ReadAllText("cards.json");
if (JsonSerializer.Deserialize(json, typeof(List<FlatCard>), SourceGenerationContext.Default) is not List<FlatCard> flatCards)
{
    throw new ApplicationException("Could not properly parse or convert cards.json to card DB.");
}

CardEndpoints cardEndpoints = new(flatCards.ToArray());
app.MapGroup("/api/cards")
    .MapCardEndpoints(cardEndpoints);


app.Run();





[JsonSourceGenerationOptions(WriteIndented = true, PropertyNameCaseInsensitive = true)]
[JsonSerializable(typeof(FlatCard))]
[JsonSerializable(typeof(List<FlatCard>))]
[JsonSerializable(typeof(DeckList))]
[JsonSerializable(typeof(AlternateSide))]
[JsonSerializable(typeof(Card))]
[JsonSerializable(typeof(Card[]))]
internal partial class SourceGenerationContext : JsonSerializerContext
{
}

