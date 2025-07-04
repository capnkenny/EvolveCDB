using DSharpPlus;
using EvolveCDB.Endpoints;
using EvolveCDB.Endpoints.Extensions;
using EvolveCDB.Model;
using EvolveCDB.Services;
using Microsoft.AspNetCore.Routing.Constraints;
using System.Text.Json;
using System.Text.Json.Serialization;


namespace EvolveCDB
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateSlimBuilder(args);

            builder.Configuration.AddEnvironmentVariables();
            builder.Services.Configure<RouteOptions>(options => options.SetParameterPolicy<RegexInlineRouteConstraint>("regex"));
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options => 
            {                
                options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo 
                {
                    Version = "v1",
                    Title = "EvolveCDB",
                    Description = "A RESTful API for retrieving Shadowverse: Evolve cards and decks.\n\n<i>All literal information presented by the API and on this site about Shadowverse and Shadowverse: Evolve, including card images and card text is copyright Cygames, Inc. This website is not produced by, endorsed by, supported by, or affiliated with Cygames Inc.</i>",
                    Contact = new Microsoft.OpenApi.Models.OpenApiContact
                    {
                        Name = "Ken \"kennybrew\" Johnson",
                        Url = new Uri("https://github.com/capnkenny/EvolveCDB")
                    }

                });
            });

            builder.Services.ConfigureHttpJsonOptions(options =>
            {
                options.SerializerOptions.TypeInfoResolverChain.Insert(0, SourceGenerationContext.Default);
                options.SerializerOptions.PropertyNameCaseInsensitive = true;
            });

            builder.Services.AddHttpClient("bushiroad", conf =>
            {
                conf.BaseAddress = new Uri("https://decklog-en.bushiroad.com/");
                conf.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:139.0) Gecko/20100101 Firefox/139.0");
                conf.DefaultRequestHeaders.Add("Accept", "application/json, text/plain, */*");
                conf.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.5");
                conf.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br, zstd");
                conf.DefaultRequestHeaders.Add("Origin", "https://decklog-en.bushiroad.com");
                conf.DefaultRequestHeaders.Add("Connection", "keep-alive");
                conf.DefaultRequestHeaders.Add("Cookie", "CookieConsent={stamp:%273DYKV73AFO5pbjzWoPswMtCoN6lk1uQ2so6frmuwtakIxpvXO/uRgg==%27%2Cnecessary:true%2Cpreferences:true%2Cstatistics:true%2Cmarketing:true%2Cmethod:%27explicit%27%2Cver:1%2Cutc:1714065861850%2Cregion:%27us-34%27};");
                conf.DefaultRequestHeaders.Add("Sec-Fetch-Dest", "empty");
                conf.DefaultRequestHeaders.Add("Sec-Fetch-Mode", "cors");
                conf.DefaultRequestHeaders.Add("Sec-Fetch-Site", "same-origin");
                conf.DefaultRequestHeaders.Add("Pragma", "no-cache");
                conf.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
                conf.DefaultRequestHeaders.Add("TE", "trailers");
            });

            builder.Services.AddHttpClient("github", conf =>
            {
                conf.BaseAddress = new("https://raw.githubusercontent.com");
            });

            builder.Services.AddSingleton(factory => {
                using var githubClient = factory.GetRequiredService<IHttpClientFactory>()!.CreateClient("github");
                string? json = githubClient.GetStringAsync("capnkenny/SVEDB_Extract/refs/heads/main/cards.json").GetAwaiter().GetResult();
                if (JsonSerializer.Deserialize(json, typeof(List<FlatCard>), SourceGenerationContext.Default) is not List<FlatCard> flatCards)
                {
                    throw new ApplicationException("Could not properly parse or convert cards.json to card DB.");
                }

                return CardExtensions.MapToCardTypes([.. flatCards]);
            });
            builder.Services.AddSingleton<CardService>();
            builder.Services.AddSingleton<DeckService>();
            builder.Services.AddScoped<CardEndpoints>();
            builder.Services.AddScoped<DeckEndpoints>();
            var cf = builder.Configuration.GetSection("BOT_TOKEN");

            builder.Services.AddHostedService<DiscordService>()
                .AddDiscordClient(new DiscordConfiguration()
                {
                    Token = cf.Value,
                    TokenType = TokenType.Bot,
                    Intents = DiscordIntents.AllUnprivileged | DiscordIntents.MessageContents,
                    MinimumLogLevel = LogLevel.Debug
                });
                
            var app = builder.Build();

            //DiscordService discordService = new(app.Services.GetRequiredService<IOptions<RootConfig>>());
            //await discordService.Init();

            app.MapSwagger("/{documentName}/swagger.json");
            app.UseSwaggerUI(options => {
                //Temporary until UI is built.
                options.SwaggerEndpoint("/v1/swagger.json", "v1");
                options.RoutePrefix = string.Empty;
            });


            app.MapGroup("/api/cards")
                .MapCardEndpoints();

            app.MapGroup("/api/deck")
                .MapDeckEndpoints();

            app.Run();
        }
    }
}

[JsonSourceGenerationOptions(WriteIndented = true, PropertyNameCaseInsensitive = true)]
[JsonSerializable(typeof(FlatCard))]
[JsonSerializable(typeof(List<FlatCard>))]
[JsonSerializable(typeof(DeckList))]
[JsonSerializable(typeof(List<AbbreviatedCard>))]
[JsonSerializable(typeof(AbbreviatedCard))]
[JsonSerializable(typeof(AbbreviatedDeckList))]
[JsonSerializable(typeof(AlternateSide))]
[JsonSerializable(typeof(Card))]
[JsonSerializable(typeof(Card[]))]
[JsonSerializable(typeof(NaviDeckList))]
[JsonSerializable(typeof(NaviCard))]
[JsonSerializable(typeof(NaviCard[]))]
internal partial class SourceGenerationContext : JsonSerializerContext
{
}