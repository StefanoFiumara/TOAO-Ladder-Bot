using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using TOAOLadderBot.Exceptions;
using TOAOLadderBot.Services;

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace TOAOLadderBot.Commands.CommandModules
{
    public class GameReportCommandsModule : ModuleBase<SocketCommandContext>
    {
        private static readonly Emoji OkHand = new("\uD83D\uDC4C");
        private static readonly Emoji ThumbsDown = new("\uD83D\uDC4E");

        private readonly GameReportingService _service;

        public GameReportCommandsModule(GameReportingService service)
        {
            _service = service;
        }

        [Command("report")]
        [Summary("Reports a ladder game")]
        public async Task ReportGameAsync([Remainder] string text)
        {
            if (Context.Channel.Id != Constants.LADDER_CHANNEL_ID)
            {
                var ladderChannel = Context.Client
                    .GetGuild(Constants.TOAO_SERVER_ID)
                    .GetTextChannel(Constants.LADDER_CHANNEL_ID);

                await Context.Message.AddReactionAsync(ThumbsDown);
                await ReplyAsync($"Hey {Context.User.Mention}! You're in the wrong channel! Please use {ladderChannel.Mention} to report ladder games!");
                return;
            }
            
            var delimeter = text.IndexOf("defeat", StringComparison.Ordinal);
            if (delimeter != -1)
            {
                var winners = Context.Message.MentionedUsers
                    .Where(u => text.IndexOf(u.Mention, StringComparison.Ordinal) != -1 && text.IndexOf(u.Mention, StringComparison.Ordinal) < delimeter)
                    .Cast<IUser>()
                    .ToList();
                
                var losers = Context.Message.MentionedUsers
                    .Where(u => text.IndexOf(u.Mention, StringComparison.Ordinal) != -1 && text.IndexOf(u.Mention, StringComparison.Ordinal) > delimeter)
                    .Cast<IUser>()
                    .ToList();
                
                await ReportGameAsync(winners, losers);
            }
            else
            {
                await Context.Message.AddReactionAsync(ThumbsDown);
                await ReplyAsync($"Sorry {Context.User.Mention}, I couldn't understand your report. Make sure you are following the reporting guidelines!");
            }
        }

        private async Task ReportGameAsync(List<IUser> winners, List<IUser> losers)
        {
            if (winners.Concat(losers).Any(u => u == Context.User))
            {
                try
                {
                    var match = await _service.ReportGameAsync(winners, losers);
                    
                    await Context.Message.AddReactionAsync(OkHand);
                    await ReplyAsync($"Thank you {Context.User.Mention}! Your game was reported successfully!\nMatch Details:\n{match}");
                }
                catch (GameReportException e)
                {
                    await Context.Message.AddReactionAsync(ThumbsDown);
                    await ReplyAsync($"Sorry {Context.User.Mention}, your report was not counted! {e.Message}");
                }
                catch (Exception e)
                {
                    await ReplyAsync($"Sorry! Some unexpected error happened while processing your report! A crash log has be DM'd to the ladder admin.\nError Message: {e.Message}");
                    
                    var admin = Context.Client.GetUser(Constants.ADMIN_USER_ID);
                    await admin.SendMessageAsync($"{Context.User.Username} ran into an unhandled exception while reporting a game.\nThe command they tried to execute was: {Context.Message.Content}\nFull exception: \n{e}");
                }
            }
            else
            {
                await Context.Message.AddReactionAsync(ThumbsDown);
                await ReplyAsync($"Sorry {Context.User.Mention}, your report cannot be counted since you are not one of the participants in the match!");
            }
        }
    }
}