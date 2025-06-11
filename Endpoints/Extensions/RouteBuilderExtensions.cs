using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

namespace EvolveCDB.Endpoints.Extensions
{
    public static class RouteBuilderExtensions
    {
        public static RouteGroupBuilder MapCardEndpoints(this RouteGroupBuilder groupBuilder)
        {
            //Route building
            groupBuilder.MapGet("{cardId}", (CardEndpoints endpointInstance, string cardId) => endpointInstance.GetSingleCardById(cardId))
                .WithOpenApi(generatedOperation =>
                {
                    OpenApiParameter parameter = generatedOperation.Parameters[0];
                    parameter.Description = "The Card ID of the card.";
                    return generatedOperation;
                });

            groupBuilder.MapGet("", 
                (
                    [FromServices] CardEndpoints endpointInstance,
                    [FromQuery(Name = "cardId")] string? cardIdContains,
                    [FromQuery(Name = "fname")] string? nameLike,
                    [FromQuery(Name = "kind")] string? kind,
                    [FromQuery(Name = "class")] string? classType,
                    [FromQuery(Name = "name")] string? name,
                    [FromQuery(Name = "cost")] int? cost
                ) => endpointInstance.GetAllCards(cardIdContains, nameLike, kind, classType, cost))
                    .WithOpenApi(generatedOperation =>
                    {
                        OpenApiParameter parameter = generatedOperation.Parameters[0];
                        parameter.Description = "The Card ID of the card.";
                        parameter = generatedOperation.Parameters[1];
                        parameter.Description = "The Name of the card (fuzzy search).";
                        parameter = generatedOperation.Parameters[2];
                        parameter.Description = "The card's kind.";
                        parameter = generatedOperation.Parameters[3];
                        parameter.Description = "The Affiliation of the card.";
                        parameter = generatedOperation.Parameters[4];
                        parameter.Description = "The Name of the card (exact match).";
                        parameter = generatedOperation.Parameters[5];
                        parameter.Description = "The card's cost.";
                        return generatedOperation;
                    });

            return groupBuilder;
        }

        public static RouteGroupBuilder MapDeckEndpoints(this RouteGroupBuilder groupBuilder)
        {
            //Route building
            groupBuilder.MapGet("{deckCode}", async (DeckEndpoints endpointInstance, string deckCode) => await endpointInstance.GetDeckFromCode(deckCode))
                .WithOpenApi(generatedOperation =>
                {
                    OpenApiParameter parameter = generatedOperation.Parameters[0];
                    parameter.Description = "The Deck Code provided by Deck Log for the Shadowverse: Evolve deck.";
                    return generatedOperation;
                });

            groupBuilder.MapGet("{deckCode}/short", async (DeckEndpoints endpointInstance, string deckCode) => await endpointInstance.GetShortenedDeckListFromCode(deckCode))
                .WithOpenApi(generatedOperation =>
                {
                    OpenApiParameter parameter = generatedOperation.Parameters[0];
                    parameter.Description = "The Deck Code provided by Deck Log for the Shadowverse: Evolve deck.";
                    return generatedOperation;
                });

            return groupBuilder;
        }
    }
}
