using System;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using TOAOLadderBot.Commands;

namespace TOAOLadderBot
{
    public static class Program
    {
        private static DiscordSocketClient _client;
        private static CommandService _commands;

        private const ulong TEST_CHANNEL = 880576178581291059;
        private const ulong TOAO_SERVER = 140956748163973120;
        private const string TOKEN_FILE = "DiscordToken.txt";

        public static async Task Main(string[] args)
        {
            _client = new DiscordSocketClient();
            _commands = new CommandService(); // TODO: CommandServiceConfig ?

            var commandHandler = new CommandHandler(_client, _commands);
            await commandHandler.InstallCommandsAsync();

            _client.Log += Log;
            _client.Ready += OnClientReady;

            // TODO: Move token to config file
            var token = await File.ReadAllTextAsync(TOKEN_FILE);
            await _client.LoginAsync(TokenType.Bot, token);

            await _client.StartAsync();

            await Task.Delay(-1);
        }

        private static async Task OnClientReady()
        {
            await _client.GetGuild(TOAO_SERVER).GetTextChannel(TEST_CHANNEL).SendMessageAsync("TOAO Ladder Bot Is Now Connected!");

            // TODO: possible to check for missed commands while the bot was offline?
        }

        private static Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}
