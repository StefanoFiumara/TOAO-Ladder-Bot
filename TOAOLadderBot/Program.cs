using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using LiteDB;
using Microsoft.Extensions.DependencyInjection;
using TOAOLadderBot.Commands;
using TOAOLadderBot.DataAccess;
using TOAOLadderBot.Services;

namespace TOAOLadderBot
{
    public static class Constants
    {
        // TODO: Do any of these need the pulled into a configuration file?
        public const string TOKEN_FILE = "DiscordToken.txt";
        public const string DATABASE_PATH = "LadderData.db"; 
        public const ulong TOAO_SERVER_ID = 140956748163973120;
        public const ulong LADDER_CHANNEL_ID = 880576178581291059;
        public const ulong ADMIN_USER_ID = 104988834017607680; // Fano
    }
    public static class Program
    {
        private static DiscordSocketClient _client;
        private static CommandService _commands;

        public static async Task Main(string[] args)
        {
            _client = new DiscordSocketClient(new DiscordSocketConfig() { AlwaysDownloadUsers = true });
            _commands = new CommandService();

            var services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_commands)
                .AddSingleton(_ => new LiteDatabase(Constants.DATABASE_PATH))
                .AddSingleton<CommandHandler>()
                .AddTransient<GameReportingService>()
                .AddTransient<StatsService>()
                .AddTransient<UnitOfWork>()
                .AddTransient<LiteDbContext>()
                .BuildServiceProvider();

            var commandHandler = services.GetService<CommandHandler>();
            Debug.Assert(commandHandler != null, nameof(commandHandler) + " != null");
            
            await commandHandler.InstallCommandsAsync();

            _client.Log += Log;
            _client.Ready += OnClientReady;
            
            var token = await File.ReadAllTextAsync(Constants.TOKEN_FILE);
            await _client.LoginAsync(TokenType.Bot, token);

            await _client.StartAsync();

            await Task.Delay(-1);
        }

        private static async Task OnClientReady()
        {
            await _client
                .GetGuild(Constants.TOAO_SERVER_ID)
                .GetTextChannel(Constants.LADDER_CHANNEL_ID)
                .SendMessageAsync("TOAO Ladder Bot Is Now Connected!");

            // TODO: Possible to check for missed commands while the bot was offline?
            // TODO: Maybe log each successful match report in the DB with a unique timestamp and check if unprocessed messages exist.
        }

        private static Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}
