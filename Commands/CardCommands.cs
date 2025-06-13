using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using EvolveCDB.Services;

namespace EvolveCDB.Commands
{
    public class CardCommands : BaseCommandModule
    {
        [Command("cardNumber")]
        public async Task GetCardByNumber(CommandContext context, string cardNumber)
        {
            var cardService = context.Services.GetRequiredService<CardService>();
            var card = cardService.GetSingleCardById(cardNumber);
            if (card == null)
            {
                await context.RespondAsync($"Sorry! No card with the Card ID '{cardNumber}' was found");
            }

            var embed = new DiscordEmbedBuilder();
            embed.Title = card!.Name;
            embed.Description = card!.Description;
            embed.Url = $"https://en.shadowverse-evolve.com/cards/?cardno={card!.CardNumber}&view=image";

            var msg = await new DiscordMessageBuilder()
                .WithEmbed(embed)
                .WithReply(context.Message.Id)
                .SendAsync(context.Channel);
        }
    }
}
