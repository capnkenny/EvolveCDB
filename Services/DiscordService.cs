using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;
using EvolveCDB.Commands;

namespace EvolveCDB.Services
{
    public static partial class ServiceExtensions
    {
        public static IServiceCollection AddDiscordClient(this IServiceCollection services,
            DiscordConfiguration config)
        {
            services.AddSingleton(new DiscordClient(config));
            return services;
        }
    }

    public class DiscordService : IHostedService
    {
        private readonly DiscordClient _client;
        public DiscordService(DiscordClient client, IServiceProvider provider)
        {
            
            _client = client;

            _client.UseInteractivity(new InteractivityConfiguration()
            {
                PollBehaviour = PollBehaviour.KeepEmojis,
            });

            CommandsNextExtension commands = _client.UseCommandsNext(new CommandsNextConfiguration()
            {
                Services = provider,
                StringPrefixes = ["!"]
            });

            commands.RegisterCommands<CardCommands>();            
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _client.ConnectAsync();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _client.DisconnectAsync();
        }

    }
}
