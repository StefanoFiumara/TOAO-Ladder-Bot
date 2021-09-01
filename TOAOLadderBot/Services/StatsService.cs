using System.Collections.Generic;
using System.Linq;
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
        public StatsService(UnitOfWork unitOfWork)
        {
            _playerRepository = unitOfWork.GetRepository<Player>();
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
                { nameof(Player.Name), $"{player.Name}" },
                { nameof(Player.Score), $"{player.Score}" },
                { nameof(Player.Rank), $"{player.Rank}" },
                { nameof(Player.Wins), $"{player.Wins}" },
                { nameof(Player.Losses), $"{player.Losses}" },
                { nameof(Player.Streak), $"{player.Streak}" },
                { nameof(Player.WinPercent), $"{player.WinPercent}%" },
            };
            
            return string.Join("\n", details.Select(d => $"{d.Key}: {d.Value}"));
        }
    }
}