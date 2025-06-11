using Microsoft.AspNetCore.Mvc;

namespace EvolveCDB.Endpoints.Extensions
{
    public static class RouteBuilderExtensions
    {
        public static RouteGroupBuilder MapCardEndpoints(this RouteGroupBuilder groupBuilder)
        {
            //Route building
            groupBuilder.MapGet("{cardId}", (CardEndpoints endpointInstance, string cardId) => endpointInstance.GetSingleCardById(cardId));

            groupBuilder.MapGet("", 
                (
                    [FromServices] CardEndpoints endpointInstance,
                    [FromQuery(Name = "cardId")] string? cardIdContains,
                    [FromQuery(Name = "name")] string? nameLike,
                    [FromQuery(Name = "kind")] string? kind,
                    [FromQuery(Name = "class")] string? classType,
                    [FromQuery(Name = "name")] string? name,
                    [FromQuery(Name = "cost")] int? cost
                ) => endpointInstance.GetAllCards(cardIdContains, nameLike, kind, classType, cost));

            return groupBuilder;
        }

        public static RouteGroupBuilder MapDeckEndpoints(this RouteGroupBuilder groupBuilder)
        {
            //Route building
            groupBuilder.MapGet("{deckCode}", async (DeckEndpoints endpointInstance, string deckCode) => await endpointInstance.GetDeckFromCode(deckCode));

            groupBuilder.MapGet("{deckCode}/short", async (DeckEndpoints endpointInstance, string deckCode) => await endpointInstance.GetShortenedDeckListFromCode(deckCode));

            return groupBuilder;
        }
    }
}
