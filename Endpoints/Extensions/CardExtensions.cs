using EvolveCDB.Model;
using System.Text.RegularExpressions;

namespace EvolveCDB.Endpoints.Extensions
{
    public static partial class CardExtensions
    {
        public static Card[] MapToCardTypes(FlatCard[] flats)
        {
            List<Card> cardArray = [];
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
                    Kind = PascalCaseRegex().Replace(card.Kind, " / "),
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

            return [.. cardArray];
        }


        [GeneratedRegex("(?<=[a-z])(?=[A-Z])|(?<=[0-9])(?=[A-Z])|(?<=[A-Z])(?=[A-Z][a-z])", RegexOptions.CultureInvariant)]
        internal static partial Regex PascalCaseRegex();

    }
}

