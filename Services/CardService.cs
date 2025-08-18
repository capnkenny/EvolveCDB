﻿using EvolveCDB.Model;
using Microsoft.Extensions.Options;

namespace EvolveCDB.Services
{
    public class CardService(IOptionsMonitor<Card[]> cardsMonitor)
    {
        private readonly Card[] _cards = cardsMonitor.CurrentValue;

        public Card? GetSingleCardById(string cardId) => _cards.FirstOrDefault(card => card.CardId.Equals(cardId, StringComparison.InvariantCultureIgnoreCase));

        public Card[] GetAllCards(string? cardIdContains, string? nameLike, string? kind, string? classType, int? cost)
        {
            IEnumerable<Card> cardList = _cards.ToList();

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

            var cardNameArray = _cards.Select(card => card.Name).ToArray();
            Fastenshtein.Levenshtein lev = new(nameSearch);

            int lowestDistance = 100;
            foreach (var item in cardNameArray)
            {
                int levenshteinDistance = lev.DistanceFrom(item);
                if (levenshteinDistance < lowestDistance)
                {
                    cardResult = _cards.First(card => card.Name.Equals(item));
                    lowestDistance = levenshteinDistance;
                }
            }

            return cardResult;
        }
    }
}
