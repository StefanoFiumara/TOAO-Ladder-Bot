using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace TOAOLadderBot.Commands.CommandModules
{
    public class GameReportCommandsModule : ModuleBase<SocketCommandContext>
    {
        private static readonly Emoji OkHand = new("\uD83D\uDC4C");
        private static readonly Emoji ThumbsDown = new("\uD83D\uDC4E");

        private async Task ReportGameAsync(List<IUser> winners, List<IUser> losers)
        {
            // TODO: Should we check for delimiter == "defeat" or "defeats" ?
            // TODO: Only respond to reports in the ladder reports channel
            if (winners.Concat(losers).Any(u => u == Context.User))
            {
                // TODO: defer to LadderReportingService for actual logic
                await Context.Message.AddReactionAsync(OkHand);
            
                // TODO: Reply with match details
                await ReplyAsync($"Thank you {Context.User.Mention}! Your game was reported successfully!");
            }
            else
            {
                await Context.Message.AddReactionAsync(ThumbsDown);
                await ReplyAsync($"Sorry {Context.User.Mention}, your report cannot be counted since you are not one of the participants in the match!");
            }
            
        }
        
        [Command("report")]
        public async Task ReportGameAsync([Remainder] string text = "")
        {
            await Context.Message.AddReactionAsync(ThumbsDown);
            await ReplyAsync($"Sorry {Context.User.Mention}, your message was not formatted correctly so your report was ignored. Make sure you are following the reporting guidelines!");
        }
        
        [Command("report")]
        [Summary("Reports a ladder game")]
        public async Task ReportGameAsync(
            IUser winner, 
            string delimeter, 
            IUser loser)
        {
            await ReportGameAsync(
                new List<IUser> {winner},
                new List<IUser> {loser});
        }
        
        [Command("report")]
        [Summary("Reports a ladder game")]
        public async Task ReportGameAsync(
            IUser winner1, IUser winner2, 
            string delimeter, 
            IUser loser1, IUser loser2)
        {
            await ReportGameAsync(
                new List<IUser> {winner1, winner2},
                new List<IUser> {loser1, loser2});
        }

        [Command("report")]
        [Summary("Reports a ladder game")]
        public async Task ReportGameAsync(
            IUser winner1, IUser winner2, IUser winner3,
            string delimeter, 
            IUser loser1, IUser loser2, IUser loser3)
        {
            await ReportGameAsync(
                new List<IUser> {winner1, winner2, winner3},
                new List<IUser> {loser1, loser2, loser3});
        }
        
        [Command("report")]
        [Summary("Reports a ladder game")]
        public async Task ReportGameAsync(
            IUser winner1, IUser winner2, IUser winner3, IUser winner4,
            string delimeter, 
            IUser loser1, IUser loser2, IUser loser3, IUser loser4)
        {
            await ReportGameAsync(
                new List<IUser> {winner1, winner2, winner3, winner4},
                new List<IUser> {loser1, loser2, loser3, loser4});
        }
    }
}