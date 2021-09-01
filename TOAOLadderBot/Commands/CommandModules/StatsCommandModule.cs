using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using TOAOLadderBot.Services;

namespace TOAOLadderBot.Commands.CommandModules
{
    public class StatsCommandModule : ModuleBase<SocketCommandContext>
    {
        private static readonly Emoji OkHand = new("\uD83D\uDC4C");
        private static readonly Emoji ThumbsDown = new("\uD83D\uDC4E");

        private readonly StatsService _service;

        public StatsCommandModule(StatsService service)
        {
            _service = service;
        }

        [Command("stats")]
        public async Task GetStatsAsync(IUser user)
        {
            try
            {
                var stats = _service.GetStatsAsync(user);
                await ReplyAsync($"{Context.User.Mention} Here are the stats for {user.Username}:\n{stats}");
            }
            catch(Exception e)
            {
                await ReplyAsync($"Sorry! Some unexpected error happened while getting {user.Username}'s stats! A crash log has be DM'd to the ladder admin.\nError Message: {e.Message}");
                    
                var admin = Context.Client.GetUser(Constants.ADMIN_USER_ID);
                await admin.SendMessageAsync($"{Context.User.Username} ran into an unhandled exception while reporting a game.\nThe command they tried to execute was: {Context.Message.Content}\nFull exception: \n{e}");
            }
            
        }

        [Command("stats")]
        public async Task GetStatsAsync()
        {
            await GetStatsAsync(Context.User);
        }
    }
}