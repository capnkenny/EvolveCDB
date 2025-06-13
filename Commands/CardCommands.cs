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
                await context.RespondAsync($"Sorry! No card with the Card ID '{cardId}' was found");
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

        internal DiscordEmbed GenerateCardEmbed(Card card, CommandContext context)
        {
            var embed = new DiscordEmbedBuilder
            {
                Title = card!.Name,
                Url = $"https://en.shadowverse-evolve.com/cards/?cardno={card!.CardId}&view=image",
                Color = GetColorBasedOnClassType(card!.ClassType)
            };

            embed.AddField("Affiliation", card!.ClassType);
            embed.AddField("Card Type", card!.Kind);
            if (!card!.Kind.Contains("Evolved", StringComparison.InvariantCultureIgnoreCase))
            {
                embed.AddField("Cost", card!.Cost.ToString());
            }

            if ((card!.Kind.Contains("Follower", StringComparison.InvariantCultureIgnoreCase) || card!.Kind.Contains("Evolved", StringComparison.InvariantCultureIgnoreCase)) && card!.Attack >= 0)
            {
                embed.AddField("Attack", card!.Attack.ToString(), true);
                embed.AddField("Defense", card!.Defense.ToString(), true);
            }
            embed.AddField("Detail", card!.Description);
            embed.WithImageUrl(card!.ImgUrl);

            DiscordEmoji emoji = DiscordEmoji.FromName(context.Client, ":repeat:");

            embed.WithFooter(card!.AlternateDetails == null ?
                $"Card ID: {card!.CardId}" :
                $"Card ID: {card!.CardId} - React with {emoji} for alternate side");
            
            return embed;
        }

        internal DiscordEmbed GenerateAlternateCardEmbed(Card card)
        {
            var embed = new DiscordEmbedBuilder
            {
                Title = card!.AlternateDetails!.Name,
                Url = $"https://en.shadowverse-evolve.com/cards/?cardno={card!.CardId}&view=image",
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
