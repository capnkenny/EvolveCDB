using EvolveCDB.Model;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EvolveCDB.Endpoints
{
    public class CardEndpoints
    {
        private readonly Card[] _cards;

        public CardEndpoints(FlatCard[] flatCards) 
        {
            
            _cards = MapToCardTypes(flatCards);
        }

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

            if(kind is not null)
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

            return cardList.ToArray();
        }

        internal Card[] MapToCardTypes(FlatCard[] flats)
        {
            List<Card> cardArray = new();
            foreach (FlatCard card in flats)
            {
                Card c = new()
                {
                    CardId = card.CardId,
                    CardNumber = card.CardNumber,
                    CardSet = card.CardSet,
                    Attack = card.Atk,
                    Defense = card.Def,
                    Description = card.Description,
                    Name = card.Name,
                    ClassType = card.ClassType,
                    Kind = card.Kind,
                    ImgUrl = card.ImgUrl,
                    LimitedToCount = card.LimitedToCount,
                    Cost = card.Cost,
                    AlternateDetails = !card.DoubleSided ? null : new AlternateSide() 
                        {
                            ImgUrl = card.AltImgUrl,
                            Description = card.AltDescription,
                            Name = card.AltName,
                            Attack = card.AltAtk,
                            Defense = card.AltDef
                        }
                };

                cardArray.Add(c);
            }

            return cardArray.ToArray();
        }
    }
}
