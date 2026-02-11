using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using EvolveCDB.Model;
using EvolveCDB.Services;

namespace EvolveCDB.Commands
{
    public class CardCommands : BaseCommandModule
    {
        [Command("cardId")]
        public async Task GetCardById(CommandContext context, string cardId)
        {
            var cardService = context.Services.GetRequiredService<CardService>();
            var card = cardService.GetSingleCardById(cardId);
            if (card == null)
            {
                await new DiscordMessageBuilder()
                    .WithContent($"Sorry! No card with the Card ID '{cardId}' was found")
                    .WithReply(context.Message.Id)
                    .SendAsync(context.Channel);
            }

            var embed = GenerateCardEmbed(card!, context);
            DiscordMessageBuilder message = new();
            var msg = await message.WithReply(context.Message.Id)
                .WithEmbed(embed)
                .SendAsync(context.Channel);

            if (card!.AlternateDetails is not null)
            {
                var repeatEmoji = DiscordEmoji.FromName(context.Client, ":repeat:");
                var result = await msg.WaitForReactionAsync(context.User, repeatEmoji);

                if (!result.TimedOut)
                {
                    await new DiscordMessageBuilder()
                        .WithEmbed(GenerateAlternateCardEmbed(card!))
                        .WithReply(result.Result.Message.Id)
                        .SendAsync(context.Channel);
                }
            }
        }

        [Command("card")]
        public async Task GetCardByName(CommandContext context, params string[] text)
        {
            string cardNameToSearch = string.Join(" ", text);
            var cardService = context.Services.GetRequiredService<CardService>();
            var card = cardService.SearchForCardName(cardNameToSearch);
            if (card == null)
            {
                await new DiscordMessageBuilder()
                       .WithContent($"Sorry! No card with '{cardNameToSearch}' was found.")
                       .WithReply(context.Message.Id)
                       .SendAsync(context.Channel);
            }

            var embed = GenerateCardEmbed(card!, context);
            DiscordMessageBuilder message = new();
            var msg = await message.WithReply(context.Message.Id)
                .WithEmbed(embed)
                .SendAsync(context.Channel);

            if (card!.AlternateDetails is not null)
            {
                var repeatEmoji = DiscordEmoji.FromName(context.Client, ":repeat:");
                var result = await msg.WaitForReactionAsync(context.User, repeatEmoji);

                if (!result.TimedOut)
                {
                    await new DiscordMessageBuilder()
                        .WithEmbed(GenerateAlternateCardEmbed(card!))
                        .WithReply(result.Result.Message.Id)
                        .SendAsync(context.Channel);
                }
            }
        }

        [Command("searchToken")]
        public async Task GetTokenByName(CommandContext context, params string[] text)
        {
            string cardNameToSearch = string.Join(" ", text);
            var cardService = context.Services.GetRequiredService<CardService>();
            var card = cardService.SearchForTokenName(cardNameToSearch);
            if (card == null)
            {
                await new DiscordMessageBuilder()
                       .WithContent($"Sorry! No Token card with '{cardNameToSearch}' was found.")
                       .WithReply(context.Message.Id)
                       .SendAsync(context.Channel);
            }

            var embed = GenerateCardEmbed(card!, context);
            DiscordMessageBuilder message = new();
            var msg = await message.WithReply(context.Message.Id)
                .WithEmbed(embed)
                .SendAsync(context.Channel);

            if (card!.AlternateDetails is not null)
            {
                var repeatEmoji = DiscordEmoji.FromName(context.Client, ":repeat:");
                var result = await msg.WaitForReactionAsync(context.User, repeatEmoji);

                if (!result.TimedOut)
                {
                    await new DiscordMessageBuilder()
                        .WithEmbed(GenerateAlternateCardEmbed(card!))
                        .WithReply(result.Result.Message.Id)
                        .SendAsync(context.Channel);
                }
            }
        }

        [Command("deckCode")]
        public async Task GetDeckByCode(CommandContext context, string deckCode)
        {
            var deckService = context.Services.GetRequiredService<DeckService>();
            DiscordMessageBuilder message = new();
            try
            {
                var deckList = await deckService.GetTextualDeckFromCode(deckCode);
                var embed = GenerateDeckEmbed(deckList!);

                await new DiscordMessageBuilder()
                    .WithReply(context.Message.Id)
                    .WithEmbed(embed)
                    .SendAsync(context.Channel);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                await new DiscordMessageBuilder()
                    .WithReply(context.Message.Id)
                    .WithContent("Sorry, I wasn't able to look up that deck code for some reason... please try again later.")
                    .SendAsync(context.Channel);
            }
        }

        internal DiscordEmbed GenerateCardEmbed(Card card, CommandContext context)
        {
            var embed = new DiscordEmbedBuilder
            {
                Title = card!.Name,
                Url = $"https://evolvecdb.org/img/{card!.CardId}",
                Color = GetColorBasedOnClassType(card!.ClassType)
            };

            embed.AddField("Affiliation", card!.ClassType);
            embed.AddField("Card Type", card!.Kind);
            if (!card!.Kind.Contains("Evolved", StringComparison.InvariantCultureIgnoreCase)
                && !card!.Kind.Contains("Leader", StringComparison.InvariantCultureIgnoreCase))
            {
                embed.AddField("Cost", card!.Cost.ToString());
            }

            if ((card!.Kind.Contains("Follower", StringComparison.InvariantCultureIgnoreCase) || card!.Kind.Contains("Evolved", StringComparison.InvariantCultureIgnoreCase)) && card!.Attack >= 0)
            {
                embed.AddField("Attack", card!.Attack.ToString(), true);
                embed.AddField("Defense", card!.Defense.ToString(), true);
            }
            if (!string.IsNullOrWhiteSpace(card!.Description))
            {
                embed.AddField("Detail", card!.Description);
            }
            embed.WithImageUrl(card!.ImgUrl);

            DiscordEmoji emoji = DiscordEmoji.FromName(context.Client, ":repeat:");

            embed.WithFooter(card!.AlternateDetails == null ?
                $"Card ID: {card!.CardId}" :
                $"Card ID: {card!.CardId} - React with {emoji} for alternate side");
            
            return embed;
        }

        internal DiscordEmbed GenerateDeckEmbed(List<string> textDeck)
        {
            var embed = new DiscordEmbedBuilder
            {
                Title = $"{textDeck[0]}",
                Url = $"https://decklog-en.bushiroad.com/view/{textDeck[1]}",
                Color = GetColorBasedOnClassType(textDeck[2])
            };

            embed.AddField("Leader Card", textDeck[3]);
            embed.AddField("Main Deck", textDeck[4]);
            embed.AddField("Evolve Deck", textDeck[5]);

            embed.WithFooter($"Deck Code: {textDeck[1]}");

            return embed;
        }


        internal DiscordEmbed GenerateAlternateCardEmbed(Card card)
        {
            var embed = new DiscordEmbedBuilder
            {
                Title = card!.AlternateDetails!.Name,
                Url = $"https://evolvecdb.org/img/{card!.CardId}",
                Color = GetColorBasedOnClassType(card!.ClassType)
            };

            if (!card!.Kind.Contains("Evolved", StringComparison.InvariantCultureIgnoreCase))
            {
                embed.AddField("Cost", card!.Cost.ToString());
            }

            if ((card!.Kind.Contains("Follower", StringComparison.InvariantCultureIgnoreCase) || card!.Kind.Contains("Evolved", StringComparison.InvariantCultureIgnoreCase)) && card!.Attack >= 0)
            {
                embed.AddField("Attack", card!.AlternateDetails!.Attack.ToString(), true);
                embed.AddField("Defense", card!.AlternateDetails!.Defense.ToString(), true);
            }
            embed.AddField("Detail", card!.AlternateDetails!.Description);

            embed.WithImageUrl(card!.AlternateDetails!.ImgUrl);

            return embed;
        }

        internal DiscordColor GetColorBasedOnClassType(string affiliation) => affiliation.ToLowerInvariant() switch
        {
            "forestcraft" => new DiscordColor(100, 171, 41),
            "swordcraft" => new DiscordColor(184, 170, 9),
            "runecraft" => new DiscordColor(109, 125, 184),
            "abysscraft" => new DiscordColor(195, 79, 100),
            "havencraft" => new DiscordColor(198, 181, 138),
            "dragoncraft" => new DiscordColor(169, 94, 27),
            "neutral" => new DiscordColor(37, 150, 190),
            _ => DiscordColor.DarkGray
        };
    }
}
