using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using TOAOLadderBot.DataAccess;
using TOAOLadderBot.DataAccess.Repository;
using TOAOLadderBot.Models;

namespace TOAOLadderBot.Services
{
    public class StatsService
    {
        private readonly IRepository<Player> _playerRepository;
        private readonly IRepository<Match> _matchRepository;

        public StatsService(UnitOfWork unitOfWork)
        {
            _playerRepository = unitOfWork.GetRepository<Player>();
            _matchRepository = unitOfWork.GetRepository<Match>();
        }

        public async Task<string> GetStatsAsync(IUser user)
        {
            var player = await Task.Run(() => _playerRepository.Query.Where(p => p.DiscordId == user.Id).SingleOrDefault());

            if (player == null || player.MatchHistory.Count == 0)
            {
                return $"{user.Username} has not yet played a ladder game!";
            }

            var details = new Dictionary<string, string>()
            {
                { nameof(Player.Score), $"{player.Score}" },
                { nameof(Player.Rank), $"{player.Rank}" },
                { nameof(Player.Wins), $"{player.Wins}" },
                { nameof(Player.Losses), $"{player.Losses}" },
                { nameof(Player.Streak), $"{player.Streak}" },
                { "Win %", $"{player.WinPercent:P}" },
            };
            
            return string.Join("\n", details.Select(d => $"{d.Key}: {d.Value}"));
        }

        public async Task<string> GetHistoryAsync(IUser user, int count)
        {
            var userId = user.Id;
            var matches = await Task.Run(() => 
                _matchRepository.Query
                    .Include(m => m.Winners)
                    .Include(m => m.Losers)
                    .Where(m => m.Winners.Select(p => p.DiscordId).Any(id => id == userId) || 
                                m.Losers.Select(p => p.DiscordId).Any(id => id == userId))
                    .OrderByDescending(m => m.ReportedDate)
                    .Limit(count)
                    .ToList());
            
            if (matches.Count == 0)
            {
                return $"There is no match history for this player!";
            }

            var history = matches
                .Select(m => m.ToHistoryString(userId))
                .ToList();

            return string.Join("\n", history);
        }

        public async Task<string> GetLeaderboardAsync()
        {
            var players = await Task.Run(() => _playerRepository.Query.OrderByDescending(p => p.Score).ToList());

            var sb = new StringBuilder();
            sb.AppendLine("```");
            var headers = new[] { "Rank", "Name", "Score", "Streak", "Win %" };

            int rankWidth = headers[0].Length; // NOTE: very unlikely to need more than 4 characters for this column unless we have more than 999 players...

            int nameWidth = players.Select(p => p.Name).Select(s => s.Length).Max();
            nameWidth = nameWidth > headers[1].Length ? nameWidth : headers[1].Length;
            
            int scoreWidth = players.Select(p => $"{p.Score}").Select(s => s.Length).Max();
            scoreWidth = scoreWidth > headers[2].Length ? scoreWidth : headers[2].Length;
            
            int streakWidth = headers[3].Length;
            int winPWidth = "100.00%".Length; // Hardcoded

            var header = $"{headers[0].PadRight(rankWidth)} | {headers[1].PadRight(nameWidth)} | {headers[2].PadRight(scoreWidth)} | {headers[3].PadRight(streakWidth)} | {headers[4].PadRight(winPWidth)}";
            var underline = $"{new string('=', rankWidth)} | {new string('=', nameWidth)} | {new string('=', scoreWidth)} | {new string('=', streakWidth)} | {new string('=', winPWidth)}";

            sb.AppendLine(header);
            sb.AppendLine(underline);

            for (var i = 0; i < players.Count; i++)
            {
                var p = players[i];
                var rank = $"#{i + 1}".PadRight(rankWidth);
                var name = p.Name.PadRight(nameWidth);
                var score = $"{p.Score}".PadRight(scoreWidth);
                var streak = $"{p.Streak}".PadRight(streakWidth);
                var winP = $"{p.WinPercent:P}".PadRight(winPWidth);

                sb.AppendLine($"{rank} | {name} | {score} | {streak} | {winP}");
            }
            
            sb.AppendLine("```");
            return sb.ToString();
        }
    }
}