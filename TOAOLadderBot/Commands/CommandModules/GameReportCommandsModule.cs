using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace TOAOLadderBot.Commands.CommandModules
{
    public class GameReportCommandsModule : ModuleBase<SocketCommandContext>
    {
        public Task ReportGameAsync(List<IUser> winners, List<IUser> losers, string comments)
        {
            // TODO: defer to LadderReportingService for actual logic
            Debugger.Break();
            return Task.CompletedTask;
        }
        
        [Command("report")]
        public Task ReportGameAsync([Remainder] string text = "")
        {
            // TODO: Reply with command usage info
            Debugger.Break();
            return ReplyAsync(".");
        }
        
        [Command("report")]
        [Summary("Reports a ladder game")]
        public Task ReportGameAsync(
            IUser winner, 
            string delimeter, 
            IUser loser, 
            [Remainder] string comments = "")
        {
            return ReportGameAsync(
                new List<IUser> {winner},
                new List<IUser> {loser},
                comments);
        }
        
        [Command("report")]
        [Summary("Reports a ladder game")]
        public Task ReportGameAsync(
            IUser winner1, IUser winner2, 
            string delimeter, 
            IUser loser1, IUser loser2, 
            [Remainder] string comments = "")
        {
            return ReportGameAsync(
                new List<IUser> {winner1, winner2},
                new List<IUser> {loser1, loser2},
                comments);
        }

        [Command("report")]
        [Summary("Reports a ladder game")]
        public Task ReportGameAsync(
            IUser winner1, IUser winner2, IUser winner3,
            string delimeter, 
            IUser loser1, IUser loser2, IUser loser3,
            [Remainder] string comments = "")
        {
            return ReportGameAsync(
                new List<IUser> {winner1, winner2, winner3},
                new List<IUser> {loser1, loser2, loser3},
                comments);
        }
        
        [Command("report")]
        [Summary("Reports a ladder game")]
        public Task ReportGameAsync(
            IUser winner1, IUser winner2, IUser winner3, IUser winner4,
            string delimeter, 
            IUser loser1, IUser loser2, IUser loser3, IUser loser4,
            [Remainder] string comments = "")
        {
            return ReportGameAsync(
                new List<IUser> {winner1, winner2, winner3, winner4},
                new List<IUser> {loser1, loser2, loser3, loser4},
                comments);
        }
    }
}