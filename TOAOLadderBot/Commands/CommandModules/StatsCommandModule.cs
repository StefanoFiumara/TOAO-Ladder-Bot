using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using TOAOLadderBot.Services;

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

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
                var stats = await _service.GetStatsAsync(user);
                await Context.Message.AddReactionAsync(OkHand);
                await ReplyAsync($"{Context.User.Mention} Here are the stats for {user.Username}:\n{stats}");
            }
            catch(Exception e)
            {
                await Context.Message.AddReactionAsync(ThumbsDown);
                await ReplyAsync($"Sorry! Some unexpected error happened while getting {user.Username}'s stats! A crash log has be DM'd to the ladder admin.\nError Message: {e.Message}");
                    
                var admin = Context.Client.GetUser(Constants.ADMIN_USER_ID);
                await admin.SendMessageAsync($"{Context.User.Username} ran into an unhandled exception while querying stats.\nThe command they tried to execute was: {Context.Message.Content}\nFull exception: \n{e}");
            }
            
        }

        [Command("stats")]
        public async Task GetStatsAsync()
        {
            await GetStatsAsync(Context.User);
        }

        [Command("history")]
        public async Task GetHistoryAsync()
        {
            await GetHistoryAsync(Context.User);
        }

        [Command("history")]
        public async Task GetHistoryAsync(IUser user)
        {
            await GetHistoryAsync(user, 5);
        }

        [Command("history")]
        public async Task GetHistoryAsync(int count)
        {
            await GetHistoryAsync(Context.User, count);
        }

        [Command("history")]
        public async Task GetHistoryAsync(IUser user, int count)
        {
            try
            {
                var history = await _service.GetHistoryAsync(user, count);
                await Context.Message.AddReactionAsync(OkHand);
                await ReplyAsync($"{Context.User.Mention} History of {user.Username}'s last {count} Matches:\n{history}");
            }
            catch(Exception e)
            {
                await Context.Message.AddReactionAsync(ThumbsDown);
                await ReplyAsync($"Sorry! Some unexpected error happened while getting {user.Username}'s match history! A crash log has be DM'd to the ladder admin.\nError Message: {e.Message}");
                    
                var admin = Context.Client.GetUser(Constants.ADMIN_USER_ID);
                await admin.SendMessageAsync($"{Context.User.Username} ran into an unhandled exception while querying match history.\nThe command they tried to execute was: {Context.Message.Content}\nFull exception: \n{e}");
            }
        }

        [Command("leaderboard")]
        public async Task GetLeaderboardAsync()
        {
            try
            {
                var leaderboard = await _service.GetLeaderboardAsync();
                await Context.Message.AddReactionAsync(OkHand);
                await ReplyAsync($"{Context.User.Mention} Here are the current ladder standings!\n{leaderboard}");
            }
            catch(Exception e)
            {
                await Context.Message.AddReactionAsync(ThumbsDown);
                await ReplyAsync($"Sorry! Some unexpected error happened while getting the ladder standings! A crash log has be DM'd to the ladder admin.\nError Message: {e.Message}");
                    
                var admin = Context.Client.GetUser(Constants.ADMIN_USER_ID);
                await admin.SendMessageAsync($"{Context.User.Username} ran into an unhandled exception while querying ladder rankings.\nThe command they tried to execute was: {Context.Message.Content}\nFull exception: \n{e}");
            }
        }
    }
}