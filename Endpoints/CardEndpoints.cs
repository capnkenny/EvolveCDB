using EvolveCDB.Model;
using EvolveCDB.Services;

namespace EvolveCDB.Endpoints
{
    public class CardEndpoints
    {
        private readonly CardService _cardService;

        public CardEndpoints(CardService service) 
        {
            _cardService = service;
        }

        public Card? GetSingleCardById(string cardId) => _cardService.GetSingleCardById(cardId);

        public Card[] GetAllCards(string? cardIdContains, string? nameLike, string? kind, string? classType, int? cost) => 
            _cardService.GetAllCards(cardIdContains, nameLike, kind, classType, cost);
    }
}
