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
    public static class Program
    {
        private static DiscordSocketClient _client;
        private static CommandService _commands;

        private const ulong TEST_CHANNEL = 880576178581291059;
        private const ulong TOAO_SERVER = 140956748163973120;
        private const string TOKEN_FILE = "DiscordToken.txt";
        private const string DATABASE_PATH = "LadderData.db"; // TODO: Configurable?

        public static async Task Main(string[] args)
        {
            _client = new DiscordSocketClient();
            _commands = new CommandService(); // TODO: CommandServiceConfig ?
            
            var services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_commands)
                .AddSingleton<CommandHandler>()
                .AddTransient<GameReportingService>()
                .AddTransient<UnitOfWork>()
                .AddTransient<LiteDbContext>()
                .AddSingleton(_ => new LiteDatabase(DATABASE_PATH))
                .BuildServiceProvider();

            var commandHandler = services.GetService<CommandHandler>();
            Debug.Assert(commandHandler != null, nameof(commandHandler) + " != null");
            
            await commandHandler.InstallCommandsAsync();

            _client.Log += Log;
            _client.Ready += OnClientReady;
            
            var token = await File.ReadAllTextAsync(TOKEN_FILE);
            await _client.LoginAsync(TokenType.Bot, token);

            await _client.StartAsync();

            await Task.Delay(-1);
        }

        private static async Task OnClientReady()
        {
            await _client
                .GetGuild(TOAO_SERVER)
                .GetTextChannel(TEST_CHANNEL)
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
