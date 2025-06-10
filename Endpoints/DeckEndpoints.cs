using EvolveCDB.Model;
using System.Text.Json;

namespace EvolveCDB.Endpoints
{
    public class DeckEndpoints
    {
        private const int ShadowverseEvolveGameId = 6;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly Card[] _cards;

        public DeckEndpoints(Card[] cardList, IHttpClientFactory factory) 
        {
            _cards = cardList;
            _httpClientFactory = factory;
        }

        public async Task<Card[]?> GetDeckFromCode(string code)
        {
            var httpClient = _httpClientFactory.CreateClient("bushiroad");
            httpClient.DefaultRequestHeaders.Add("Referer", $"https://decklog-en.bushiroad.com/view/{code}");
            var response = await httpClient.GetAsync($"system/app/api/view/{code}");
            response.EnsureSuccessStatusCode();

            string responseJson = await response.Content.ReadAsStringAsync();

            if (JsonSerializer.Deserialize(responseJson, typeof(NaviDeckList), SourceGenerationContext.Default) is NaviDeckList naviDeck)
            {
                if (naviDeck.GameId != ShadowverseEvolveGameId)
                {
                    throw new ArgumentException("Deck retrieved was not a valid Shadowverse: Evolve deck.");
                }

                List<Card> resultCards = [];
                //This is dumb, usually only one leader card
                foreach (var lc in naviDeck.LeaderDeck)
                {
                    var matching = _cards.Where(c => c.CardId.Equals(lc.CardNumber, StringComparison.InvariantCultureIgnoreCase));
                    if (matching.Any())
                    {
                        resultCards.AddRange(matching);
                    }
                }

                foreach (var mc in naviDeck.MainDeck)
                {
                    var matchingCard = _cards.FirstOrDefault(c => c.CardId.Equals(mc.CardNumber, StringComparison.InvariantCultureIgnoreCase));
                    if (matchingCard is not null)
                    {
                        for (int i = 0; i < mc.Num; i++)
                        {
                            resultCards.Add(matchingCard);
                        }
                    }
                    else
                        Console.WriteLine($"NOT FOUND: {mc.CardNumber}");
                }

                foreach (var ec in naviDeck.EvolveDeck)
                {
                    var matchingEvolveCard = _cards.FirstOrDefault(c => c.CardId.Equals(ec.CardNumber, StringComparison.InvariantCultureIgnoreCase));
                    if (matchingEvolveCard is not null)
                    {
                        for (int i = 0; i < ec.Num; i++)
                        {
                            resultCards.Add(matchingEvolveCard);
                        }
                    }
                    else
                        Console.WriteLine($"NOT FOUND: {ec.CardNumber}");
                }

                return [.. resultCards];
            }

            return null;
        }

    }


}
