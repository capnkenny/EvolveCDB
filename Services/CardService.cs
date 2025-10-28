using EvolveCDB.Model;
using Microsoft.Extensions.Options;
using System.Threading;

namespace EvolveCDB.Services
{
    public class CardService(IOptionsMonitor<CardListOptions> cardList)
    {
        private IOptionsMonitor<CardListOptions> _optionsMonitor = cardList;

        public Card? GetSingleCardById(string cardId) => _optionsMonitor.CurrentValue.Cards.FirstOrDefault(card => card.CardId.Equals(cardId, StringComparison.InvariantCultureIgnoreCase));

        public Card[] GetAllCards(string? cardIdContains, string? nameLike, string? kind, string? classType, int? cost)
        {
            IEnumerable<Card> cardList = _optionsMonitor.CurrentValue.Cards.ToList();

            if (cardIdContains is not null)
            {
                cardList = cardList.Where(card => card.CardId.Contains(cardIdContains, StringComparison.InvariantCultureIgnoreCase));
            }

            if (nameLike is not null)
            {
                cardList = cardList.Where(card => card.Name.Contains(nameLike, StringComparison.InvariantCultureIgnoreCase) ||
                    (card.AlternateDetails is not null && card.AlternateDetails.Name.Contains(nameLike, StringComparison.InvariantCultureIgnoreCase)));
            }

            if (kind is not null)
            {
                cardList = cardList.Where(card => card.Kind.Equals(kind, StringComparison.InvariantCultureIgnoreCase));
            }

            if (classType is not null)
            {
                cardList = cardList.Where(card => card.ClassType.Equals(classType, StringComparison.InvariantCultureIgnoreCase));
            }

            if (cost is not null)
            {
                cardList = cardList.Where(card => card.Cost == cost);
            }

            return [.. cardList];
        }
    
        public Card? SearchForCardName(string nameSearch)
        {
            Card? cardResult = null;

            var cardNameArray = _optionsMonitor.CurrentValue.Cards.Select(card => card.Name).ToArray();
            Fastenshtein.Levenshtein lev = new(nameSearch);

            int lowestDistance = 100;
            foreach (var item in cardNameArray)
            {
                int levenshteinDistance = lev.DistanceFrom(item);
                if (levenshteinDistance < lowestDistance)
                {
                    cardResult = _optionsMonitor.CurrentValue.Cards.First(card => card.Name.Equals(item));
                    lowestDistance = levenshteinDistance;
                }
            }

            return cardResult;
        }
    }
}
