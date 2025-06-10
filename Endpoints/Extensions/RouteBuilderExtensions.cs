using Microsoft.AspNetCore.Mvc;

namespace EvolveCDB.Endpoints.Extensions
{
    public static class RouteBuilderExtensions
    {
        public static RouteGroupBuilder MapCardEndpoints(this RouteGroupBuilder groupBuilder, CardEndpoints endpointInstance)
        {
            //Route building
            groupBuilder.MapGet("{cardId}", (string cardId) => endpointInstance.GetSingleCardById(cardId));

            groupBuilder.MapGet("", 
                (
                    [FromQuery(Name = "cardId")] string? cardIdContains,
                    [FromQuery(Name = "name")] string? nameLike,
                    [FromQuery(Name = "kind")] string? kind,
                    [FromQuery(Name = "class")] string? classType,
                    [FromQuery(Name = "name")] string? name,
                    [FromQuery(Name = "cost")] int? cost
                ) => endpointInstance.GetAllCards(cardIdContains, nameLike, kind, classType, cost));

            return groupBuilder;
        }
    }
}
