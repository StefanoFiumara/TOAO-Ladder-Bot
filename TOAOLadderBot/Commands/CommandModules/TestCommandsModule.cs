using System.Threading.Tasks;
using Discord.Commands;

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace TOAOLadderBot.Commands.CommandModules
{
    public class TestCommandsModule : ModuleBase<SocketCommandContext>
    {
        // ~ping hello world -> hello world
        [Command("ping")]
        [Summary("Test connection")]
        public Task PingAsync()
        {
            var messageDetails = $"Received ping message in {Context.Channel.Name} from {Context.User.Mention}";
            return ReplyAsync(messageDetails);
        }
    }
}