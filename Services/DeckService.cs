using EvolveCDB.Model;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace EvolveCDB.Services
{
    public class DeckService(IOptionsMonitor<CardListOptions> cardList, IHttpClientFactory factory)
    {
        private readonly IOptionsMonitor<CardListOptions> _monitor;
        private const int ShadowverseEvolveGameId = 6;
        private readonly IHttpClientFactory _httpClientFactory = factory;

        public async Task<DeckList?> GetDeckFromCode(string code)
        {
            using var httpClient = _httpClientFactory.CreateClient("bushiroad");
            httpClient.DefaultRequestHeaders.Add("Referer", $"https://decklog-en.bushiroad.com/view/{code}");
            var response = await httpClient.GetAsync($"system/app/api/view/{code}");
            response.EnsureSuccessStatusCode();

            string responseJson = await response.Content.ReadAsStringAsync();

            if (JsonSerializer.Deserialize(responseJson, typeof(NaviDeckList)) is NaviDeckList naviDeck)
            {
                if (naviDeck.GameId != ShadowverseEvolveGameId)
                {
                    throw new ArgumentException("Deck retrieved was not a valid Shadowverse: Evolve deck.");
                }

                Card leaderCard = null!;
                List<Card> mainCardResults = [];
                List<Card> evolveCardResults = [];

                foreach (var lc in naviDeck.LeaderDeck)
                {
                    var matching = _monitor.CurrentValue.Cards.Where(c => c.CardId.Equals(lc.CardNumber, StringComparison.InvariantCultureIgnoreCase));
                    if (matching.Any())
                    {
                        leaderCard = matching.First();
                    }
                }

                foreach (var mc in naviDeck.MainDeck)
                {
                    var matchingCard = _monitor.CurrentValue.Cards.FirstOrDefault(c => c.CardId.Equals(mc.CardNumber, StringComparison.InvariantCultureIgnoreCase));
                    if (matchingCard is not null)
                    {
                        for (int i = 0; i < mc.Num; i++)
                        {
                            mainCardResults.Add(matchingCard);
                        }
                    }
                    else
                        Console.WriteLine($"-- NOT FOUND: {mc.CardNumber}");
                }

                foreach (var ec in naviDeck.EvolveDeck)
                {
                    var matchingEvolveCard = _monitor.CurrentValue.Cards.FirstOrDefault(c => c.CardId.Equals(ec.CardNumber, StringComparison.InvariantCultureIgnoreCase));
                    if (matchingEvolveCard is not null)
                    {
                        for (int i = 0; i < ec.Num; i++)
                        {
                            evolveCardResults.Add(matchingEvolveCard);
                        }
                    }
                    else
                        Console.WriteLine($"-- NOT FOUND: {ec.CardNumber}");
                }

                return new DeckList
                {
                    Name = naviDeck.Title,
                    DeckCode = code,
                    LeaderCard = leaderCard,
                    MainCards = mainCardResults,
                    EvolveCards = evolveCardResults
                };
            }
            return null;
        }

        public async Task<AbbreviatedDeckList?> GetShortenedDeckListFromCode(string code)
        {
            DeckList? list = await GetDeckFromCode(code) ?? throw new ArgumentException("Could not get the deck from the deck code provided");
            var distinctMain = list.MainCards.DistinctBy(card => card.CardId);
            var distinctEvolve = list.EvolveCards.DistinctBy(card => card.CardId);

            return new AbbreviatedDeckList
            {
                DeckCode = code,
                LeaderCardId = list.LeaderCard.CardId,
                MainCards = [.. distinctMain.Select(card => {
                    return new AbbreviatedCard
                    {
                        CardId = card.CardId,
                        Copies = list.MainCards.Count(c =>  c.CardId == card.CardId),
                    };
                })],
                EvolveCards = [.. distinctEvolve.Select(card => {
                    return new AbbreviatedCard
                    {
                        CardId = card.CardId,
                        Copies = list.EvolveCards.Count(c => c.CardId == card.CardId),
                    };
                })],
            };
        }

        public async Task<List<string>?> GetTextualDeckFromCode(string code)
        {
            DeckList? list = await GetDeckFromCode(code) ?? throw new ArgumentException("Could not get the deck from the deck code provided");
            var distinctMain = list.MainCards.DistinctBy(card => card.CardId);
            var textMain = distinctMain.Select(card =>
            {
                return new TextualCard 
                { 
                    CardText = $"{card.Name} ({card.CardId}) x{list.MainCards.Count(c => c.CardId == card.CardId)}"
                };
            });

            var distinctEvolve = list.EvolveCards.DistinctBy(card => card.CardId);
            var textEvolve = distinctEvolve.Select(card =>
            {
                return new TextualCard
                {
                    CardText = $"{card.Name} ({card.CardId}) x{list.EvolveCards.Count(c => c.CardId == card.CardId)}"
                };
            });

            return new()
            {
                $"{list.Name}",
                $"{list.DeckCode}",
                $"{list.LeaderCard.ClassType}",
                $"{list.LeaderCard.Name} ({list.LeaderCard.CardId})",
                string.Join("\n",[.. textMain.Select(c => c.CardText)]),
                string.Join("\n",[.. textEvolve.Select(c => c.CardText)]),
            };
        }
    }
}
