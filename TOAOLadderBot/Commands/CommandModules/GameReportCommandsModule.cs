using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        private const ulong LADDER_CHANNEL_ID = 880576178581291059;

        private readonly GameReportingService _service;

        public GameReportCommandsModule(GameReportingService service)
        {
            _service = service;
        }

        [Command("report")]
        [Summary("Reports a ladder game")]
        public async Task ReportGameAsync([Remainder] string text)
        {
            // TODO: Should we reply asking the user to use the proper channel?
            if (Context.Channel.Id != LADDER_CHANNEL_ID)
                return;
            
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
                await ReplyAsync($"Sorry {Context.User.Mention}, your message was not formatted correctly so your report was ignored. Make sure you are following the reporting guidelines!");
            }
        }

        private async Task ReportGameAsync(List<IUser> winners, List<IUser> losers)
        {
            if (winners.Concat(losers).Any(u => u == Context.User))
            {
                // TODO: defer to LadderReportingService for actual logic
                try
                {
                    var match = await _service.ReportGameAsync(winners, losers);
                    await Context.Message.AddReactionAsync(OkHand);

                    // TODO: Reply with match details
                    await ReplyAsync($"Thank you {Context.User.Mention}! Your game was reported successfully!\nMatch Details:\n{match}");
                }
                catch (GameReportException e)
                {
                    await Context.Message.AddReactionAsync(ThumbsDown);
                    await ReplyAsync($"Sorry {Context.User.Mention}, your report was not counted! {e.Message}");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    Debugger.Break();
                    throw;
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