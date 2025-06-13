using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace EvolveCDB.Commands
{
    public class PingCommand : BaseCommandModule
    {
        [Command("ping")]
        public async Task Ping(CommandContext context) => await context.RespondAsync($"Pong! Latency is {context.Client.Ping}ms.");
    }
}
