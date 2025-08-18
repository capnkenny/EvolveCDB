using EvolveCDB.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

namespace EvolveCDB.Endpoints.Extensions
{
    public static class RouteBuilderExtensions
    {
        public static RouteGroupBuilder MapCardEndpoints(this RouteGroupBuilder groupBuilder)
        {
            //Route building
            groupBuilder.MapGet("{cardId}", (CardEndpoints endpointInstance, string cardId) => TypedResults.Ok(endpointInstance.GetSingleCardById(cardId)))
                .WithName("Get Single Card")
                .WithOpenApi(generatedOperation =>
                {
                    generatedOperation.Summary = "Searches for a specific card given the provided card ID.";
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
                ) => TypedResults.Ok(endpointInstance.GetAllCards(cardIdContains, nameLike, kind, classType, cost)))
                    .WithName("Get All Cards")
                    .WithOpenApi(generatedOperation =>
                    {
                        generatedOperation.Summary = "Searches for cards given the provided criteria.";
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
            groupBuilder.MapGet("{deckCode}", async (DeckEndpoints endpointInstance, string deckCode) => TypedResults.Ok(await endpointInstance.GetDeckFromCode(deckCode)))
                .WithName("Get Deck")
                .WithOpenApi(generatedOperation =>
                {
                    OpenApiParameter parameter = generatedOperation.Parameters[0];
                    parameter.Description = "The Deck Code provided by Deck Log for the Shadowverse: Evolve deck.";
                    generatedOperation.Summary = "Gets the fully expanded Main Deck, Evolve Deck, and Leader Card for a given Deck Code.";
                    return generatedOperation;
                });

            groupBuilder.MapGet("{deckCode}/short", async (DeckEndpoints endpointInstance, string deckCode) => TypedResults.Ok(await endpointInstance.GetShortenedDeckListFromCode(deckCode)))
                .WithName("Get Deck (Shortened)")
                .WithOpenApi(generatedOperation =>
                {
                    OpenApiParameter parameter = generatedOperation.Parameters[0];
                    parameter.Description = "The Deck Code provided by Deck Log for the Shadowverse: Evolve deck.";
                    generatedOperation.Summary = "Gets an abbreviated listing of the Card IDs and quantities for the Main Deck and Evolve Deck, and the Leader Card ID for a given Deck Code.";
                    return generatedOperation;
                });

            return groupBuilder;
        }

        public static RouteGroupBuilder MapImageEndpoints(this RouteGroupBuilder groupBuilder)
        {
            //Route building
            groupBuilder.MapGet("{cardId}", async (ImageEndpoints endpointInstance, string cardId) => TypedResults.Ok(await endpointInstance.GetImageByCardId(cardId)))
                .WithName("Get Card Image")
                .WithOpenApi(generatedOperation =>
                {
                    OpenApiParameter parameter = generatedOperation.Parameters[0];
                    parameter.Description = "The Card ID of the card.";
                    generatedOperation.Summary = "Gets the image for a specific card given the provided card ID. .";
                    return generatedOperation;
                });

            return groupBuilder;
        }

    }
}
