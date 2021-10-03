using System;
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
                .Select((m, i) => $"#{i} | {m.ToHistoryString(userId)}")
                .ToList();

                return $"```\n{string.Join("\n", history)}\n```";
        }

        public async Task<string> GetLeaderboardAsync()
        {
            var players = await Task.Run(() => _playerRepository.Query.OrderByDescending(p => p.Score).ToList());

            return GetPlayerTable(players);
        }

        private string GetPlayerTable(List<Player> players)
        {
            var headers = new List<string> { "Name", "Score", "Rank", "Streak", "Win %" };

            var headerMap = new Dictionary<string, Func<Player, string>>
            {
                { headers[0], t => t.Name },
                { headers[1], t => $"{t.Score}" },
                { headers[2], t => $"{t.Rank}" },
                { headers[3], t => $"{t.Streak}" },
                { headers[4], t => $" {t.WinPercent:P}" },
            };
            
            return Utils.FormatTable(headers, players, headerMap, useIndex: true);
        }
    }
}